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
using System.ComponentModel;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Command.Archiving;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
    /// <summary>
	/// Support class for archiving a specific study with an <see cref="HsmArchive"/>.
	/// </summary>
	public class HsmStudyArchive
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hsmArchive">The HsmArchive to work with.</param>
		public HsmStudyArchive(HsmArchive hsmArchive)
		{
			_hsmArchive = hsmArchive;
		}
		private StudyXml _studyXml;
		private StudyStorageLocation _storageLocation;
		private readonly HsmArchive _hsmArchive;
		
		/// <summary>
		/// Retrieves the storage location fromthe database for the specified study.
		/// </summary>
		/// <param name="queueItem">The queueItem.</param>
		/// <returns>true if a location was found, false otherwise.</returns>
		public bool GetStudyStorageLocation(ArchiveQueue queueItem)
		{
			return FilesystemMonitor.Instance.GetReadableStudyStorageLocation(queueItem.StudyStorageKey, out _storageLocation);
		}

		/// <summary>
		/// Load the StudyXml file.
		/// </summary>
		/// <param name="studyXmlFile"></param>
		public void LoadStudyXml(string studyXmlFile)
		{
			using (Stream fileStream = FileStreamOpener.OpenForRead(studyXmlFile, FileMode.Open))
			{
				var theMemento = new StudyXmlMemento();

				StudyXmlIo.Read(theMemento, fileStream);

				_studyXml = new StudyXml(_storageLocation.StudyInstanceUid);
				_studyXml.SetMemento(theMemento);

				fileStream.Close();
			}
		}


		/// <summary>
		/// Archive the specified <see cref="ArchiveQueue"/> item.
		/// </summary>
		/// <param name="queueItem">The ArchiveQueue item to archive.</param>
		public void Run(ArchiveQueue queueItem)
		{
            using (ArchiveProcessorContext executionContext = new ArchiveProcessorContext(queueItem))
            {
                try
                {
                    if (!GetStudyStorageLocation(queueItem))
                    {
                        Platform.Log(LogLevel.Error,
                                     "Unable to find readable study storage location for archival queue request {0}.  Delaying request.",
                                     queueItem.Key);
                        queueItem.FailureDescription = "Unable to find readable study storage location for archival queue request.";
                        _hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));
                        return;
                    }

                    // First, check to see if we can lock the study, if not just reschedule the queue entry.
                    if (!_storageLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle))
                    {
                        Platform.Log(LogLevel.Info, "Study {0} on partition {1} is currently locked, delaying archival.", _storageLocation.StudyInstanceUid, _hsmArchive.ServerPartition.Description);
                        queueItem.FailureDescription = "Study is currently locked, delaying archival.";
                        _hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));
                        return;
                    }

                    StudyIntegrityValidator validator = new StudyIntegrityValidator();
                    validator.ValidateStudyState("Archive", _storageLocation, StudyIntegrityValidationModes.Default);

                    using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        ILockStudy studyLock = update.GetBroker<ILockStudy>();
                        LockStudyParameters parms = new LockStudyParameters
                                                    	{
                                                    		StudyStorageKey = queueItem.StudyStorageKey,
                                                    		QueueStudyStateEnum = QueueStudyStateEnum.ArchiveScheduled
                                                    	};
                    	bool retVal = studyLock.Execute(parms);
                        if (!parms.Successful || !retVal)
                        {
                            Platform.Log(LogLevel.Info, "Study {0} on partition {1} failed to lock, delaying archival.", _storageLocation.StudyInstanceUid, _hsmArchive.ServerPartition.Description);
                            queueItem.FailureDescription = "Study failed to lock, delaying archival.";
                            _hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));
                            return;
                        }
                        update.Commit();
                    }
					
                    string studyXmlFile = _storageLocation.GetStudyXmlPath(); 
                    
                    // Load the study Xml file, this is used to generate the list of dicom files to archive.
                    LoadStudyXml(studyXmlFile);

                    DicomFile file = LoadFileFromStudyXml();

                	string patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);
					string patientId = file.DataSet[DicomTags.PatientId].GetString(0, string.Empty);
					string accessionNumber = file.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);

                	Platform.Log(LogLevel.Info,
                	             "Starting archival of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4} on archive {5}",
                	             _storageLocation.StudyInstanceUid, patientsName, patientId,
                	             accessionNumber, _hsmArchive.ServerPartition.Description,
                	             _hsmArchive.PartitionArchive.Description);

                    // Use the command processor to do the archival.
                    using (ServerCommandProcessor commandProcessor = new ServerCommandProcessor("Archive"))
                    {

						var archiveStudyCmd = new ArchiveStudyCommand(_storageLocation, _hsmArchive.HsmPath, executionContext.TempDirectory, _hsmArchive.PartitionArchive) 
								{ ForceCompress = HsmSettings.Default.CompressZipFiles };

						commandProcessor.AddCommand(archiveStudyCmd);
	                    commandProcessor.AddCommand(new UpdateArchiveQueueItemCommand(queueItem.GetKey(),_storageLocation.GetKey(), ArchiveQueueStatusEnum.Completed));
                        
                    	StudyRulesEngine studyEngine = new StudyRulesEngine(_storageLocation, _hsmArchive.ServerPartition, _studyXml);
                    	studyEngine.Apply(ServerRuleApplyTimeEnum.StudyArchived, commandProcessor);
						

                        if (!commandProcessor.Execute())
                        {
                            Platform.Log(LogLevel.Error,
                                         "Unexpected failure archiving study ({0}) to archive {1}: {2}, zip filename: {3}",
                                         _storageLocation.StudyInstanceUid, _hsmArchive.PartitionArchive.Description,
										 commandProcessor.FailureReason, archiveStudyCmd.OutputZipFilePath);

                            queueItem.FailureDescription = commandProcessor.FailureReason;
                            _hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Failed, Platform.Time);
                        }
                        else
                            Platform.Log(LogLevel.Info, "Successfully archived study {0} on {1} to zip {2}",
                                         _storageLocation.StudyInstanceUid,
										 _hsmArchive.PartitionArchive.Description, archiveStudyCmd.OutputZipFilePath);

						// Log the current FilesystemQueue settings
						_storageLocation.LogFilesystemQueue();
                    }
                }
                catch (StudyIntegrityValidationFailure ex)
                {
                    StringBuilder error = new StringBuilder();
                    error.AppendLine(String.Format("Partition  : {0}", ex.ValidationStudyInfo.ServerAE));
                    error.AppendLine(String.Format("Patient    : {0}", ex.ValidationStudyInfo.PatientsName));
                    error.AppendLine(String.Format("Study Uid  : {0}", ex.ValidationStudyInfo.StudyInstaneUid));
                    error.AppendLine(String.Format("Accession# : {0}", ex.ValidationStudyInfo.AccessionNumber));
                    error.AppendLine(String.Format("Study Date : {0}", ex.ValidationStudyInfo.StudyDate));

                    queueItem.FailureDescription = error.ToString();
                    _hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Failed, Platform.Time);
                }
                catch (Exception e)
                {
                    String msg = String.Format("Unexpected exception archiving study: {0} on {1}: {2}",
                                 _storageLocation.StudyInstanceUid, _hsmArchive.PartitionArchive.Description, e.Message);

                    Platform.Log(LogLevel.Error, e, msg);
                    queueItem.FailureDescription = msg;
                    _hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Failed, Platform.Time);
                }
                finally
                {
                    // Unlock the Queue Entry
                    using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        ILockStudy studyLock = update.GetBroker<ILockStudy>();
                        LockStudyParameters parms = new LockStudyParameters
                                                    	{
                                                    		StudyStorageKey = queueItem.StudyStorageKey,
                                                    		QueueStudyStateEnum = QueueStudyStateEnum.Idle
                                                    	};
                    	bool retVal = studyLock.Execute(parms);
                        if (!parms.Successful || !retVal)
                        {
                            Platform.Log(LogLevel.Info, "Study {0} on partition {1} is failed to unlock.", _storageLocation.StudyInstanceUid, _hsmArchive.ServerPartition.Description);
                        }
                        update.Commit();
                    }
                }
            }			
		}

		/// <summary>
		/// Simple class to load a sample image file from the study.
		/// </summary>
		/// <returns></returns>
		private DicomFile LoadFileFromStudyXml()
		{
			DicomFile defaultFile = null;
			foreach (SeriesXml seriesXml in _studyXml)
				foreach (InstanceXml instanceXml in seriesXml)
				{
					// Skip non-image objects
					string path;
					if (instanceXml.SopClass.Equals(SopClass.KeyObjectSelectionDocumentStorage)
					    || instanceXml.SopClass.Equals(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass)
					    || instanceXml.SopClass.Equals(SopClass.BlendingSoftcopyPresentationStateStorageSopClass)
					    || instanceXml.SopClass.Equals(SopClass.ColorSoftcopyPresentationStateStorageSopClass))
					{
						if (defaultFile == null)
						{
							path = Path.Combine(_storageLocation.GetStudyPath(), seriesXml.SeriesInstanceUid);
							path = Path.Combine(path, instanceXml.SopInstanceUid);
							path += ServerPlatform.DicomFileExtension;
							defaultFile = new DicomFile(path);
							defaultFile.Load(path);
						}
						continue;
					}

					path = Path.Combine(_storageLocation.GetStudyPath(), seriesXml.SeriesInstanceUid);
					path = Path.Combine(path, instanceXml.SopInstanceUid);
					path += ServerPlatform.DicomFileExtension;
					defaultFile = new DicomFile(path);
					defaultFile.Load(path);

					return defaultFile;
				}

			return defaultFile;
		}
	}

	/// <summary>
	/// Command to update the status of an <see cref="ArchiveQueue"/> record
	/// </summary>
	class UpdateArchiveQueueItemCommand : ServerDatabaseCommand
	{
		private readonly ServerEntityKey _archiveQueueKey;
		private readonly ServerEntityKey _studyStorageKey;
		private readonly ArchiveQueueStatusEnum _status;

		public UpdateArchiveQueueItemCommand(ServerEntityKey archiveQueueKey, ServerEntityKey studyStorageKey, ArchiveQueueStatusEnum status)
			: base("Update Archive Queue Item")
		{
			_archiveQueueKey = archiveQueueKey;
			_studyStorageKey = studyStorageKey;
			_status = status;
		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			var parms = new UpdateArchiveQueueParameters
			{
				ArchiveQueueKey = _archiveQueueKey,
				ArchiveQueueStatusEnum = _status,
				ScheduledTime = Platform.Time,
				StudyStorageKey = _studyStorageKey
			};


			var broker = updateContext.GetBroker<IUpdateArchiveQueue>();

			if (!broker.Execute(parms))
				throw new ApplicationException("UpdateArchiveQueueItemCommand failed");
		}
	}
}
