using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Rules;
using ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess;

namespace ClearCanvas.ImageServer.Services.ServiceLock.PartitionReapplyRules
{
	public abstract class BaseReapplyRulesServiceLockItemProcessor : BaseServiceLockItemProcessor
	{
		/// <summary>
		/// Load the first instance from the first series of the StudyXml file for a study.
		/// </summary>
		/// <param name="location">The storage location of the study.</param>
		/// <returns></returns>
		protected static DicomFile LoadInstance(StudyStorageLocation location)
		{
			string studyXml = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

			if (!File.Exists(studyXml))
			{
				return null;
			}

			FileStream stream = FileStreamOpener.OpenForRead(studyXml, FileMode.Open);
			var theDoc = new XmlDocument();
			StudyXmlIo.Read(theDoc, stream);
			stream.Close();
			stream.Dispose();
			var xml = new StudyXml();
			xml.SetMemento(theDoc);
            
			IEnumerator<SeriesXml> seriesEnumerator = xml.GetEnumerator();
			if (seriesEnumerator.MoveNext())
			{
				SeriesXml seriesXml = seriesEnumerator.Current;
				IEnumerator<InstanceXml> instanceEnumerator = seriesXml.GetEnumerator();
				if (instanceEnumerator.MoveNext())
				{
					InstanceXml instance = instanceEnumerator.Current;
					var file = new DicomFile("file.dcm",new DicomAttributeCollection(), instance.Collection)
						{TransferSyntax = instance.TransferSyntax};
					return file;
				}
			}

			return null;
		}

		/// <summary>
		/// Reprocess a specific study.
		/// </summary>
		/// <param name="partition">The ServerPartition the study is on.</param>
		/// <param name="location">The storage location of the study to process.</param>
		/// <param name="engine">The rules engine to use when processing the study.</param>
		/// <param name="postArchivalEngine">The rules engine used for studies that have been archived.</param>
		/// <param name="dataAccessEngine">The rules engine strictly used for setting data acess.</param>
		protected static void ProcessStudy(ServerPartition partition, StudyStorageLocation location, ServerRulesEngine engine, ServerRulesEngine postArchivalEngine, ServerRulesEngine dataAccessEngine)
		{
			if (!location.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle) || !location.AcquireWriteLock())
			{
				Platform.Log(LogLevel.Error, "Unable to lock study {0}. The study is being processed. (Queue State: {1})", location.StudyInstanceUid,location.QueueStudyStateEnum.Description); 
			}
			else
			{
				try
				{
					DicomFile msg = LoadInstance(location);
					if (msg == null)
					{
						Platform.Log(LogLevel.Error, "Unable to load file for study {0}", location.StudyInstanceUid);
						return;
					}

					bool archiveQueueExists;
					bool archiveStudyStorageExists;
					bool filesystemDeleteExists;
					using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
					{
						// Check for existing archive queue entries
						var archiveQueueBroker = read.GetBroker<IArchiveQueueEntityBroker>();
						var archiveQueueCriteria = new ArchiveQueueSelectCriteria();
						archiveQueueCriteria.StudyStorageKey.EqualTo(location.Key);
						archiveQueueExists = archiveQueueBroker.Count(archiveQueueCriteria) > 0;


						var archiveStorageBroker = read.GetBroker<IArchiveStudyStorageEntityBroker>();
						var archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
						archiveStudyStorageCriteria.StudyStorageKey.EqualTo(location.Key);
						archiveStudyStorageExists = archiveStorageBroker.Count(archiveStudyStorageCriteria) > 0;

						var filesystemQueueBroker = read.GetBroker<IFilesystemQueueEntityBroker>();
						var filesystemQueueCriteria = new FilesystemQueueSelectCriteria();
						filesystemQueueCriteria.StudyStorageKey.EqualTo(location.Key);
						filesystemQueueCriteria.FilesystemQueueTypeEnum.EqualTo(FilesystemQueueTypeEnum.DeleteStudy);
						filesystemDeleteExists = filesystemQueueBroker.Count(filesystemQueueCriteria) > 0;
					}

					var context = new ServerActionContext(msg, location.FilesystemKey, partition, location.Key);
					using (context.CommandProcessor = new ServerCommandProcessor("Study Rule Processor"))
					{
						// Check if the Study has been archived 
						if (archiveStudyStorageExists && !archiveQueueExists && !filesystemDeleteExists)
						{
							// Add a command to delete the current filesystemQueue entries, so that they can 
							// be reinserted by the rules engine.
							context.CommandProcessor.AddCommand(new DeleteFilesystemQueueCommand(location.Key, ServerRuleApplyTimeEnum.StudyArchived));

							// How to deal with exiting FilesystemQueue entries is problematic here.  If the study
							// has been migrated off tier 1, we probably don't want to modify the tier migration 
							// entries.  Compression entries may have been entered when the Study was initially 
							// processed, we don't want to delete them, because they might still be valid.  
							// We just re-run the rules engine at this point, and delete only the StudyPurge entries,
							// since those we know at least would only be applied for archived studies.
							postArchivalEngine.Execute(context);

							// Post Archive doesn't allow data access rules.  Force Data Access rules to be reapplied
							// to these studies also.
							dataAccessEngine.Execute(context);
						}
						else
						{
							// Add a command to delete the current filesystemQueue entries, so that they can 
							// be reinserted by the rules engine.
							context.CommandProcessor.AddCommand(new DeleteFilesystemQueueCommand(location.Key,ServerRuleApplyTimeEnum.StudyProcessed));

							// Execute the rules engine, insert commands to update the database into the command processor.
							engine.Execute(context);

							// Re-do insert into the archive queue.
							// Note: the stored procedure will update the archive entry if it already exists
							context.CommandProcessor.AddCommand(
								new InsertArchiveQueueCommand(location.ServerPartitionKey, location.Key));
						}

						// Do the actual database updates.
						if (false == context.CommandProcessor.Execute())
						{
							Platform.Log(LogLevel.Error, "Unexpected failure processing Study level rules for study {0}", location.StudyInstanceUid);
						}

						// Log the FilesystemQueue related entries
						location.LogFilesystemQueue();
					}
				}
				finally
				{
					location.ReleaseWriteLock();
				}
			}
		}
	}
}
