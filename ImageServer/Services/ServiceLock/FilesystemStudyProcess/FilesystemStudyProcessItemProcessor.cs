#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    /// <summary>
    /// Class for reprocessing the rules engine for studies on a filesystem.
    /// </summary>
    public class FilesystemStudyProcessItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
        #region Private Members
        private readonly FilesystemReprocessStatistics _stats = new FilesystemReprocessStatistics();
        private readonly Dictionary<ServerPartition, ServerRulesEngine> _engines = new Dictionary<ServerPartition, ServerRulesEngine>();
		private readonly Dictionary<ServerPartition, ServerRulesEngine> _postArchivalEngines = new Dictionary<ServerPartition, ServerRulesEngine>();
        private readonly Dictionary<ServerPartition, ServerRulesEngine> _dataAccessEngine = new Dictionary<ServerPartition, ServerRulesEngine>();
        #endregion

        #region Private Methods

        /// <summary>
        /// Load the first instance from the first series of the StudyXml file for a study.
        /// </summary>
        /// <param name="location">The storage location of the study.</param>
        /// <returns></returns>
        private static DicomFile LoadInstance(StudyStorageLocation location)
        {
            string studyXml = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

            if (!File.Exists(studyXml))
            {
                return null;
            }

			FileStream stream = FileStreamOpener.OpenForRead(studyXml, FileMode.Open);
            XmlDocument theDoc = new XmlDocument();
            StudyXmlIo.Read(theDoc, stream);
            stream.Close();
            stream.Dispose();
            StudyXml xml = new StudyXml();
            xml.SetMemento(theDoc);           
            
            IEnumerator<SeriesXml> seriesEnumerator = xml.GetEnumerator();
            if (seriesEnumerator.MoveNext())
            {
                SeriesXml seriesXml = seriesEnumerator.Current;
                IEnumerator<InstanceXml> instanceEnumerator = seriesXml.GetEnumerator();
                if (instanceEnumerator.MoveNext())
                {
                    InstanceXml instance = instanceEnumerator.Current;
					DicomFile file = new DicomFile("file.dcm",new DicomAttributeCollection(), instance.Collection)
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
        private static void ProcessStudy(ServerPartition partition, StudyStorageLocation location, ServerRulesEngine engine, ServerRulesEngine postArchivalEngine, ServerRulesEngine dataAccessEngine)
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

        /// <summary>
        /// Reprocess a file systems
        /// </summary>
        /// <param name="filesystem"></param>
        private void ReprocessFilesystem(Filesystem filesystem)
        {
            DirectoryInfo filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

            foreach (DirectoryInfo partitionDir in filesystemDir.GetDirectories())
            {
                ServerPartition partition;
                if (GetServerPartition(partitionDir.Name, out partition) == false)
                {
					if (!partitionDir.Name.EndsWith("_Incoming") && !partitionDir.Name.Equals("temp")
					 && !partitionDir.Name.Equals("ApplicationLog") && !partitionDir.Name.Equals("AlertLog"))
	                    Platform.Log(LogLevel.Error, "Unknown partition folder '{0}' in filesystem: {1}", partitionDir.Name,
                                 filesystem.Description);
                    continue;
                }

                // Since we found a partition, we should find a rules engine too.
                ServerRulesEngine engine = _engines[partition];
            	ServerRulesEngine postArchivalEngine = _postArchivalEngines[partition];
                ServerRulesEngine dataAccessEngine = _dataAccessEngine[partition];

				foreach (DirectoryInfo dateDir in partitionDir.GetDirectories())
				{
					if (dateDir.FullName.EndsWith("Deleted")
						|| dateDir.FullName.EndsWith(ServerPlatform.ReconcileStorageFolder))
						continue;

					foreach (DirectoryInfo studyDir in dateDir.GetDirectories())
					{
						String studyInstanceUid = studyDir.Name;
						try
						{
							StudyStorageLocation location = LoadReadableStorageLocation(partition.GetKey(), studyInstanceUid);
							if (location == null)
							{
								foreach (DirectoryInfo seriesDir in studyDir.GetDirectories())
								{
									FileInfo[] sopInstanceFiles = seriesDir.GetFiles("*.dcm");

									DicomFile file = null;
									foreach (FileInfo sopFile in sopInstanceFiles)
									{
										if (!sopFile.FullName.EndsWith(ServerPlatform.DicomFileExtension))
											continue;

										try
										{
											file = new DicomFile(sopFile.FullName);
                                            file.Load(DicomTags.StudyId, DicomReadOptions.DoNotStorePixelDataInDataSet | DicomReadOptions.Default);
											break;
										}
										catch (Exception e)
										{
											Platform.Log(LogLevel.Warn, e, "Unexpected failure loading file: {0}.  Continuing to next file.",
											             sopFile.FullName);
											file = null;
										}
									}
									if (file != null)
									{
										studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
										break;
									}
								}

								location = LoadReadableStorageLocation(partition.GetKey(), studyInstanceUid);
								if (location == null)
									continue;
							}

                            ProcessStudy(partition, location, engine, postArchivalEngine, dataAccessEngine);
							_stats.NumStudies++;

							if (CancelPending) return;
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Unexpected error while processing study: {0} on partition {1}.", studyInstanceUid,
							             partition.Description);
						}
					}

					// Cleanup the directory, if its empty.
					DirectoryUtility.DeleteIfEmpty(dateDir.FullName);
				}
            }
        }

        /// <summary>
        /// Get a ServerPartition object from the list of loaded partitions.
        /// </summary>
        /// <param name="partitionFolderName">The folder for the partition.</param>
        /// <param name="partition">The ServerPartition object.</param>
        /// <returns>true on success.</returns>
        private bool GetServerPartition(string partitionFolderName, out ServerPartition partition)
        {
            foreach (ServerPartition part in _engines.Keys)
            {
                if (part.PartitionFolder == partitionFolderName)
                {
                    partition = part;
                    return true;
                }
            }

            partition = null;
            return false;
        }

        /// <summary>
        /// Load the <see cref="ServerRulesEngine"/> for each partition.
        /// </summary>
        private void LoadRulesEngine()
        {
			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				var broker = context.ReadContext.GetBroker<IServerPartitionEntityBroker>();
				var criteria = new ServerPartitionSelectCriteria();
				IList<ServerPartition> partitions = broker.Find(criteria);

				foreach (ServerPartition partition in partitions)
				{
					var engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyProcessed, partition.Key);
                    engine.Load();
                    _engines.Add(partition, engine);
					
					engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyArchived, partition.Key);
                    engine.Load();
                    _postArchivalEngines.Add(partition, engine);

                    engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyProcessed, partition.Key);
				    engine.AddIncludeType(ServerRuleTypeEnum.DataAccess);
                    engine.Load(); 
                    _dataAccessEngine.Add(partition, engine);
                    
				}
			}
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Process the <see cref="ServiceLock"/> item.
        /// </summary>
        /// <param name="item"></param>
        protected override void OnProcess(Model.ServiceLock item)
        {
        	LoadRulesEngine();

        	ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

        	_stats.StudyRate.Start();
        	_stats.Filesystem = info.Filesystem.Description;

        	Platform.Log(LogLevel.Info, "Starting reprocess of filesystem: {0}", info.Filesystem.Description);

        	ReprocessFilesystem(info.Filesystem);

        	Platform.Log(LogLevel.Info, "Completed reprocess of filesystem: {0}", info.Filesystem.Description);

        	_stats.StudyRate.SetData(_stats.NumStudies);
        	_stats.StudyRate.End();

        	StatisticsLogger.Log(LogLevel.Info, _stats);

        	item.ScheduledTime = item.ScheduledTime.AddDays(1);

			if (CancelPending)
			{
				Platform.Log(LogLevel.Info,
				             "Filesystem Reprocess of {0} has been canceled, rescheduling.  Entire filesystem will be reprocessed.",
				             info.Filesystem.Description);
				UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
			}
			else
	        	UnlockServiceLock(item, false, Platform.Time.AddDays(1));
        }

    	public new void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }
}
