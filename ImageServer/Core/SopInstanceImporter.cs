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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Common.WorkQueue;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Diagnostics;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using SaveDicomFileCommand = ClearCanvas.ImageServer.Core.Command.SaveDicomFileCommand;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Encapsulates the context of the application when <see cref="SopInstanceImporter"/> is called.
	/// </summary>
	public class SopInstanceImporterContext
	{
		#region Private Members

		private readonly String _contextID;
		private readonly string _sourceAE;
		private readonly ServerPartition _partition;
		private readonly ServerPartitionAlternateAeTitle _alternateAe;
		private readonly ExternalRequestQueue _request;
		private readonly WorkQueuePriorityEnum _priorityEnum;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates an instance of <see cref="SopInstanceImporterContext"/> to be used
		/// by <see cref="SopInstanceImporter"/> 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="sourceAe">Source AE title where the image(s) are imported from</param>
		/// <param name="serverAe">Target AE title where the image(s) will be imported to</param>
		public SopInstanceImporterContext(string id, string sourceAe, string serverAe)
			:
				this(id, sourceAe, ServerPartitionMonitor.Instance.GetPartition(serverAe))
		{
			_alternateAe = ServerPartitionMonitor.Instance.GetPartitionAlternateAe(serverAe);
		}

		/// <summary>
		/// Creates an instance of <see cref="SopInstanceImporterContext"/> to be used
		/// by <see cref="SopInstanceImporter"/> 
		/// </summary>
		/// <param name="contextId">The ID assigned to the context. This will be used as the name of storage folder in case of duplicate.</param>
		/// <param name="sourceAe">Source AE title of the image(s) to be imported</param>
		/// <param name="partition">The <see cref="ServerPartition"/> which the image(s) will be imported to</param>
		/// <param name="request">An external request that triggered this operation.</param>
		/// <param name="priority">The priority that resulting <see cref="WorkQueue"/> will have upon insertion.</param>
		public SopInstanceImporterContext(string contextId, string sourceAe, ServerPartition partition,
		                                  ExternalRequestQueue request = null, WorkQueuePriorityEnum priority=null)
		{
			Platform.CheckForEmptyString(contextId, "contextID");
			Platform.CheckForNullReference(partition, "partition");
			_contextID = contextId;
			_sourceAE = sourceAe;
			_partition = partition;
			_request = request;
			_priorityEnum = priority;
		}

		#endregion

		/// <summary>
		/// Gets the ID of this context
		/// </summary>
		public string ContextID
		{
			get { return _contextID; }
		}

		/// <summary>
		/// Gets the source AE title where the image(s) are imported from
		/// </summary>
		public string SourceAE
		{
			get { return _sourceAE; }
		}

		/// <summary>
		/// Gets <see cref="ServerPartition"/> where the image(s) will be imported to
		/// </summary>
		public ServerPartition Partition
		{
			get { return _partition; }
		}

		/// <summary>
		/// The Alternate AE Title that the SOP Instance was imported under, if applicable.
		/// </summary>
		public ServerPartitionAlternateAeTitle AlternateAe
		{
			get { return _alternateAe; }
		}

		/// <summary>
		/// The <see cref="ExternalRequestQueue"/> entry associated with the import, if applicable.
		/// </summary>
		public ExternalRequestQueue Request
		{
			get { return _request; }
		}

		/// <summary>
		/// If set, the <see cref="WorkQueuePriorityEnum"/> to set resultant <see cref="WorkQueue"/> items.
		/// </summary>
		public WorkQueuePriorityEnum Priority
		{
			get { return _priorityEnum; }
		}

		/// <summary>
		/// If set, sets the DuplicateProcessing policy for the imported SOP.
		/// </summary>
		public DuplicateProcessingEnum? DuplicateProcessing { get; set; }

		/// <summary>
		/// If the import is through a file stream, the filestream is stored here between import calls.
		/// </summary>
		public FileStream FileStream { get; set; }
	}

	/// <summary>
    /// Helper class to import a DICOM image into the system.
    /// </summary>
    /// <remarks>
    /// <see cref="SopInstanceImporter"/> provides a consistent mean of
    /// getting DICOM instances into Image Server. Imported DICOM instances
    /// will be inserted into the <see cref="WorkQueue"/> for processing. Proper checks will be
    /// done if the duplicate object policy is set for the partition. Duplicates will be 
    /// ignored, rejected or inserted into the <see cref="StudyIntegrityQueue"/> for manual intervention.
    /// 
    /// </remarks>
	public class SopInstanceImporter
	{
        #region Private Members
        private readonly SopInstanceImporterContext _context; 
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SopInstanceImporter"/> to import DICOM object(s)
        /// into the system.
        /// </summary>
        /// <param name="context">The context of the operation.</param>
        public SopInstanceImporter(SopInstanceImporterContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
        } 
        #endregion

        #region Public Methods

		 public bool GetStreamedFileStorageFolder(DicomMessageBase message, out string folder, out string filesystemStreamingFolder)
		 {
			 var location = GetWritableOnlineStorage(message);

			 using (
				 var commandProcessor =
					 new ServerCommandProcessor(String.Format("Streaming folder for Study Instance {0}", location.StudyInstanceUid)))
			 {

				 String path = Path.Combine(location.FilesystemPath, location.PartitionFolder);
				 commandProcessor.AddCommand(new CreateDirectoryCommand(path));

				 filesystemStreamingFolder = path = Path.Combine(path, ServerPlatform.StreamingStorageFolder);
				 commandProcessor.AddCommand(new CreateDirectoryCommand(path));

				 path = Path.Combine(path, _context.ContextID /* the AE title + timestamp */);
				 commandProcessor.AddCommand(new CreateDirectoryCommand(path));

				 path = Path.Combine(path, location.StudyInstanceUid);
				 commandProcessor.AddCommand(new CreateDirectoryCommand(path));

				 if (!commandProcessor.Execute())
				 {
					 folder = null;
					 Platform.Log(LogLevel.Warn, "Unable to create directory to store study: {0}: {1}", path,
					              commandProcessor.FailureReason);
					 return false;
				 }
				 folder = path;
				 return true;
			 }
		 }
		/// <summary>
		/// Import a file that already resides at an accessible path on a filesystem.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		/// <param name="message"></param>
		/// <param name="sourceFilename"></param>
		/// <returns></returns>
		public DicomProcessingResult ImportFile(DicomMessageBase message, string sourceFilename)
		{
			Platform.CheckForNullReference(message, "message");
			String studyInstanceUid;
			String seriesInstanceUid;
			String sopInstanceUid;
			String accessionNumber;
			String patientsName;

			LoadMessageUids(message, out studyInstanceUid, out seriesInstanceUid, out sopInstanceUid,
								 out accessionNumber, out patientsName);

			// Scrub the name for invalid characters.
			string newName = XmlUtils.XmlCharacterScrub(patientsName);
			if (!newName.Equals(patientsName))
				message.DataSet[DicomTags.PatientsName].SetStringValue(newName);

			var result = new DicomProcessingResult
			{
				Successful = true,
				StudyInstanceUid = studyInstanceUid,
				SeriesInstanceUid = seriesInstanceUid,
				SopInstanceUid = sopInstanceUid,
				AccessionNumber = accessionNumber
			};

			try
			{
				Validate(message);
			}
			catch (DicomDataException e)
			{
				result.SetError(DicomStatuses.ProcessingFailure, e.Message);
				return result;
			}

			// Use the command processor for rollback capabilities.
			using (var commandProcessor = new ServerCommandProcessor(String.Format("Processing Sop Instance {0}", sopInstanceUid)))
			{
				try
				{
					string failureMessage;
					StudyStorageLocation studyLocation = GetWritableOnlineStorage(message);

					// GetWritableOnlineStorage should throw an exception if the study location cannot be found.
					Platform.CheckForNullReference(studyLocation, "studyLocation");

					if (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle)
						&& (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled)))
					{
						failureMessage = String.Format("Study {0} on partition {1} is being processed: {2}, can't accept new images.",
													   studyLocation.StudyInstanceUid, _context.Partition.Description, studyLocation.QueueStudyStateEnum.Description);
						result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
						return result;
					}
					if (studyLocation.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy))
					{
						if (studyLocation.IsLatestArchiveLossless)
						{
							result.DicomStatus = DicomStatuses.StorageStorageOutOfResources;
							failureMessage = String.Format("Study {0} on partition {1} can't accept new images due to lossy compression of the study.  Restoring study.",
														   studyLocation.StudyInstanceUid, _context.Partition.Description);
							Platform.Log(LogLevel.Error, failureMessage);
							if (ServerHelper.InsertRestoreRequest(studyLocation) == null)
							{
								Platform.Log(LogLevel.Warn, "Unable to insert Restore Request for Study");
							}

							result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
							result.RestoreRequested = true;
							return result;
						}
					}

					String path = studyLocation.FilesystemPath;
					String finalDest = studyLocation.GetSopInstancePath(seriesInstanceUid, sopInstanceUid);

					if (HasUnprocessedCopy(studyLocation.Key, seriesInstanceUid, sopInstanceUid))
					{
						var accept = false;

						// This is a special case: #10569
						// Allow user to revive an orphaned study by reprocessing the files found in the filesystem
						if (File.Exists(finalDest))
						{
							accept = DuplicatePolicy.IsParitionDuplicatePolicyOverridden(studyLocation);
						}

						if (!accept)
						{
							failureMessage = string.Format("Another copy of the SOP Instance was received but has not been processed: {0}", sopInstanceUid);
							result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
							return result;
						}
					}

					var data = new StudyProcessWorkQueueData
					{
						ReceivingAeTitle = _context.AlternateAe == null
											  ? _context.Partition.AeTitle
											  : _context.AlternateAe.AeTitle
					};

					if (File.Exists(finalDest))
					{
						result = HandleDuplicateFile(sopInstanceUid, studyLocation, commandProcessor, message, sourceFilename, data);
						if (!result.Successful)
							return result;
					}
					else
					{
						HandleNonDuplicateFile(seriesInstanceUid, sopInstanceUid, studyLocation, commandProcessor, message, sourceFilename, path,
										   false, data);
					}

					if (commandProcessor.Execute())
					{
						result.DicomStatus = DicomStatuses.Success;
					}
					else
					{
						failureMessage =
							String.Format("Failure processing message: {0}. Sending failure status.",
										  commandProcessor.FailureReason);
						result.SetError(DicomStatuses.ProcessingFailure, failureMessage);
						// processor already rolled back
						return result;
					}
				}
				catch (NoWritableFilesystemException)
				{
					String failureMessage = String.Format("Unable to process image, no writable filesystem found for Study UID {0}.", sopInstanceUid);
					commandProcessor.Rollback();
					result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
				}
				catch (StudyIsNearlineException e)
				{
					String failureMessage = e.RestoreRequested
												? String.Format("{0}. Restore has been requested.", e.Message)
												: e.Message;

					Platform.Log(LogLevel.Error, failureMessage);
					commandProcessor.Rollback();
					result.SetError(DicomStatuses.ProcessingFailure, failureMessage);
				}
				catch (FilesystemNotWritableException)
				{
					commandProcessor.Rollback();

					string folder;
					if (!FilesystemMonitor.Instance.GetWriteableIncomingFolder(_context.Partition, out folder))
					{
						String failureMessage =
							String.Format("Unable to process image, study storage location is missing or not writeable: {0}.", sopInstanceUid);
						result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
						return result;
					}

					string path = Path.Combine(folder, sopInstanceUid);
					path += ServerPlatform.DicomFileExtension;

					if (!File.Exists(path))
					{
						File.Move(sourceFilename, path);

						Platform.Log(LogLevel.Info, "Saved existing SOP without writeable storage location to {0} folder: {1}",
								 FilesystemMonitor.ImportDirectorySuffix, sopInstanceUid);
						result.DicomStatus = DicomStatuses.Success;
						return result;
					}
					
					FileUtils.Delete(sourceFilename);

					Platform.Log(LogLevel.Info, "Unable to save temporary file to incoming folder, file already exists there: {0}, rejecting SOP as processing failure",
							 path);
					result.DicomStatus = DicomStatuses.ProcessingFailure;
					return result;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", commandProcessor.Description);
					commandProcessor.Rollback();
					result.SetError(result.DicomStatus ?? DicomStatuses.ProcessingFailure, e.Message);
				}
			}

			return result;
		}

        /// <summary>
        /// Imports the specified <see cref="DicomMessageBase"/> object into the system.
        /// The object will be inserted into the <see cref="WorkQueue"/> for processing and
        /// if it's a duplicate, proper checks will be done and depending on the policy, it will be 
        /// ignored, rejected or inserted into the <see cref="StudyIntegrityQueue"/> for manual intervention.
        /// </summary>
        /// <param name="message">The DICOM object to be imported.</param>
        /// <returns>An instance of <see cref="DicomProcessingResult"/> that describes the result of the processing.</returns>
        /// <exception cref="DicomDataException">Thrown when the DICOM object contains invalid data</exception>
        public DicomProcessingResult Import(DicomMessageBase message)
        {
            Platform.CheckForNullReference(message, "message");
            String studyInstanceUid;
            String seriesInstanceUid;
            String sopInstanceUid;
            String accessionNumber;
            String patientsName;

	        LoadMessageUids(message, out studyInstanceUid, out seriesInstanceUid, out sopInstanceUid,
	                             out accessionNumber, out patientsName);

        	DicomFile file = null;

            // Scrub the name for invalid characters.
            string newName = XmlUtils.XmlCharacterScrub(patientsName);
            if (!newName.Equals(patientsName))
                message.DataSet[DicomTags.PatientsName].SetStringValue(newName);

			var result = new DicomProcessingResult
			                               	{
			                               		Successful = true,
			                               		StudyInstanceUid = studyInstanceUid,
			                               		SeriesInstanceUid = seriesInstanceUid,
			                               		SopInstanceUid = sopInstanceUid,
			                               		AccessionNumber = accessionNumber
			                               	};

        	try
			{
				Validate(message);
			}
			catch (DicomDataException e)
			{
				result.SetError(DicomStatuses.ProcessingFailure, e.Message);
				return result;
			}

            // Use the command processor for rollback capabilities.
            using (var commandProcessor = new ServerCommandProcessor(String.Format("Processing Sop Instance {0}", sopInstanceUid)))
            {
                try
                {
                    string failureMessage;
                    StudyStorageLocation studyLocation = GetWritableOnlineStorage(message);

                    // GetWritableOnlineStorage should throw an exception if the study location cannot be found.
                    Platform.CheckForNullReference(studyLocation, "studyLocation");

                    if (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle)
                        && (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled)))
                    {
                        failureMessage = String.Format("Study {0} on partition {1} is being processed: {2}, can't accept new images.",
                                                       studyLocation.StudyInstanceUid, _context.Partition.Description, studyLocation.QueueStudyStateEnum.Description);
                        result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
                        return result;
                    }
                	if (studyLocation.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy))
                	{
                		if (studyLocation.IsLatestArchiveLossless)
                		{
                			result.DicomStatus = DicomStatuses.StorageStorageOutOfResources;
                			failureMessage = String.Format("Study {0} on partition {1} can't accept new images due to lossy compression of the study.  Restoring study.",
                			                               studyLocation.StudyInstanceUid, _context.Partition.Description);
                			Platform.Log(LogLevel.Error, failureMessage);
                			if (ServerHelper.InsertRestoreRequest(studyLocation) == null)
                			{
                				Platform.Log(LogLevel.Warn, "Unable to insert Restore Request for Study");
                			}

                			result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
                            result.RestoreRequested = true;
                			return result;
                		}
                	}

                	String path = studyLocation.FilesystemPath;
                    String finalDest = studyLocation.GetSopInstancePath(seriesInstanceUid, sopInstanceUid);
                    file = ConvertToDicomFile(message, finalDest, _context.SourceAE);

                    if (HasUnprocessedCopy(studyLocation.Key, seriesInstanceUid, sopInstanceUid))
                    {
                        var accept = false;

                        // This is a special case: #10569
                        // Allow user to revive an orphaned study by reprocessing the files found in the filesystem
                        if (File.Exists(finalDest))
                        {
                            accept = DuplicatePolicy.IsParitionDuplicatePolicyOverridden(studyLocation);
                        }
                        
                        if (!accept)
                        {
                            failureMessage = string.Format("Another copy of the SOP Instance was received but has not been processed: {0}", sopInstanceUid);
                            result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
                            return result;
                        }
                    }

                    var data = new StudyProcessWorkQueueData
                        {
                            ReceivingAeTitle = _context.AlternateAe == null
                                                  ? _context.Partition.AeTitle
                                                  : _context.AlternateAe.AeTitle
                        };

                	if (File.Exists(finalDest))
                	{
                		result = HandleDuplicate(sopInstanceUid, studyLocation, commandProcessor, file, data);
                		if (!result.Successful)
                			return result;
                	}
                	else
                	{
                	    HandleNonDuplicate(seriesInstanceUid, sopInstanceUid, studyLocation, commandProcessor, file, path,
                	                       false, data);
                	}

                	if (commandProcessor.Execute())
                	{
                		result.DicomStatus = DicomStatuses.Success;
                	}
                	else
                	{
                		failureMessage =
                			String.Format("Failure processing message: {0}. Sending failure status.",
                			              commandProcessor.FailureReason);
                		result.SetError(DicomStatuses.ProcessingFailure, failureMessage);
                		// processor already rolled back
                		return result;
                	}
                }
                catch(NoWritableFilesystemException)
                {
                    String failureMessage = String.Format("Unable to process image, no writable filesystem found for Study UID {0}.", sopInstanceUid);
                    commandProcessor.Rollback();
                    result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
                }
                catch(StudyIsNearlineException e)
                {
                    String failureMessage = e.RestoreRequested
                                                ? String.Format("{0}. Restore has been requested.", e.Message)
                                                : e.Message;

                    Platform.Log(LogLevel.Error, failureMessage);
                    commandProcessor.Rollback();
                    result.SetError(DicomStatuses.ProcessingFailure, failureMessage);
                }
                catch (FilesystemNotWritableException)
                {
					commandProcessor.Rollback();

                	string folder;
					if (!FilesystemMonitor.Instance.GetWriteableIncomingFolder(_context.Partition, out folder))
					{
						String failureMessage =
							String.Format("Unable to process image, study storage location is missing or not writeable: {0}.", sopInstanceUid);
						result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
						return result;
					}

                	if (file == null)
                		file = ConvertToDicomFile(message, string.Empty, _context.SourceAE);

                	if (!SaveToFolder(folder, sopInstanceUid, studyInstanceUid, file))
                	{
						String failureMessage =
							String.Format("Study storage location not writeable and no writeable incoming folder: {0}.", sopInstanceUid);
						result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
						return result;
                	}

                	Platform.Log(LogLevel.Info, "Saved existing SOP without writeable storage location to {0} folder: {1}",
                	             FilesystemMonitor.ImportDirectorySuffix, sopInstanceUid);
					result.DicomStatus = DicomStatuses.Success;
                	return result;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", commandProcessor.Description);
                    commandProcessor.Rollback();
                    result.SetError(result.DicomStatus ?? DicomStatuses.ProcessingFailure, e.Message);
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

		static private void LoadMessageUids(DicomMessageBase message, out string studyInstanceUid, out string seriesInstanceUid, out string sopInstanceUid, out string accessionNumber, out string patientsName )
		{
			studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
			accessionNumber = message.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);
			patientsName = message.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);
		}

        /// <summary>
        /// Gets the online storage location for a <see cref="DicomMessageBase"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="StudyIsNearlineException">Thrown if the study is nearline.</exception>
        /// <exception cref="FilesystemNotWritableException">Thrown if the study is online but the filesystem is not writable.</exception>
        /// <exception cref="SopInstanceProcessingException">Thrown ifor other exceptions.</exception>
        private StudyStorageLocation GetWritableOnlineStorage(DicomMessageBase message)
        {
            string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            string studyDate = message.DataSet[DicomTags.StudyDate].GetString(0, String.Empty);
            Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

        	StudyStorageLocation studyLocation;

			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				bool created;
				studyLocation = FilesystemMonitor.Instance.GetOrCreateWritableStudyStorageLocation(studyInstanceUid,
				                                                                                   studyDate,
				                                                                                   message.TransferSyntax,
				                                                                                   updateContext,
				                                                                                   _context.Partition,
				                                                                                   out created);
				if (created)
					updateContext.Commit();
			}

        	Platform.CheckForNullReference(studyLocation, "studyLocation");
            return studyLocation;
        }

        private void HandleNonDuplicate(string seriesInstanceUid, string sopInstanceUid, StudyStorageLocation studyLocation, ServerCommandProcessor commandProcessor, DicomFile file, string path, bool dupImage, StudyProcessWorkQueueData data)
        {
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, studyLocation.PartitionFolder);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, studyLocation.StudyFolder);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, studyLocation.StudyInstanceUid);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, seriesInstanceUid);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, sopInstanceUid);
            path += ServerPlatform.DicomFileExtension;

            commandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true));

            WorkQueueUidData uidData = null;
            if (_context.Request != null && !string.IsNullOrEmpty(_context.Request.OperationToken))
            {
                uidData = new WorkQueueUidData
                {
                    OperationToken = _context.Request.OperationToken
                };
            }

            commandProcessor.AddCommand(
                new UpdateWorkQueueCommand(file, studyLocation, dupImage, data, uidData, _context.Request, _context.Priority));

            #region SPECIAL CODE FOR TESTING
            if (Diagnostics.Settings.SimulateFileCorruption)
            {
                commandProcessor.AddCommand(new CorruptDicomFileCommand(path));
            }
            #endregion
        }

		private void HandleNonDuplicateFile(string seriesInstanceUid, string sopInstanceUid, StudyStorageLocation studyLocation, ServerCommandProcessor commandProcessor, DicomMessageBase message, string sourcePath, string path, bool dupImage, StudyProcessWorkQueueData data)
		{
			commandProcessor.AddCommand(new CreateDirectoryCommand(path));

			path = Path.Combine(path, studyLocation.PartitionFolder);
			commandProcessor.AddCommand(new CreateDirectoryCommand(path));

			path = Path.Combine(path, studyLocation.StudyFolder);
			commandProcessor.AddCommand(new CreateDirectoryCommand(path));

			path = Path.Combine(path, studyLocation.StudyInstanceUid);
			commandProcessor.AddCommand(new CreateDirectoryCommand(path));

			path = Path.Combine(path, seriesInstanceUid);
			commandProcessor.AddCommand(new CreateDirectoryCommand(path));

			path = Path.Combine(path, sopInstanceUid);
			path += ServerPlatform.DicomFileExtension;

			commandProcessor.AddCommand(new RenameFileCommand(sourcePath, path, true));

			WorkQueueUidData uidData = null;
			if (_context.Request != null && !string.IsNullOrEmpty(_context.Request.OperationToken))
			{
				uidData = new WorkQueueUidData
				{
					OperationToken = _context.Request.OperationToken
				};
			}

			commandProcessor.AddCommand(
				new UpdateWorkQueueCommand(message, studyLocation, dupImage, data, uidData, _context.Request, _context.Priority));

			#region SPECIAL CODE FOR TESTING
			if (Diagnostics.Settings.SimulateFileCorruption)
			{
				commandProcessor.AddCommand(new CorruptDicomFileCommand(path));
			}
			#endregion
		}


		private static bool SaveToFolder(string folder, string sopInstanceUid, string studyInstanceUid, DicomFile file)
		{
			using (var commandProcessor =
				new ServerCommandProcessor(String.Format("Saving Sop Instance to Incoming {0}", sopInstanceUid)))
			{
				string path = Path.Combine(folder, studyInstanceUid);
				commandProcessor.AddCommand(new CreateDirectoryCommand(path));

				path = Path.Combine(path, sopInstanceUid);
				path += ServerPlatform.DicomFileExtension;

				if (File.Exists(path))
					return false;

				commandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true));

				return commandProcessor.Execute();
			}
		}

    	private DicomProcessingResult HandleDuplicate(string sopInstanceUid, StudyStorageLocation studyLocation, ServerCommandProcessor commandProcessor, DicomFile file, StudyProcessWorkQueueData data)
        {
        	Study study = studyLocation.Study ??
                          studyLocation.LoadStudy(ServerExecutionContext.Current.PersistenceContext);
            if (study != null)
                Platform.Log(LogLevel.Info, "Received duplicate SOP {0} (A#:{1} StudyUid:{2}  Patient: {3}  ID:{4})",
                             sopInstanceUid,
                             study.AccessionNumber, study.StudyInstanceUid,
                             study.PatientsName, study.PatientId);
            else
                Platform.Log(LogLevel.Info,
                             "Received duplicate SOP {0} (StudyUid:{1}). Existing files haven't been processed.",
                             sopInstanceUid, studyLocation.StudyInstanceUid);

    	    var sopProcessingContext = new SopInstanceProcessorContext(commandProcessor, studyLocation, _context.ContextID,
    	                                                        _context.Request)
	    	    {
		    	    DuplicateProcessing = _context.DuplicateProcessing
	    	    };
    		DicomProcessingResult result = DuplicateSopProcessorHelper.Process(sopProcessingContext, file, data);
            return result;
        }

		private DicomProcessingResult HandleDuplicateFile(string sopInstanceUid, StudyStorageLocation studyLocation, ServerCommandProcessor commandProcessor, DicomMessageBase message, string sourceFilename, StudyProcessWorkQueueData data)
		{
			Study study = studyLocation.Study ??
						  studyLocation.LoadStudy(ServerExecutionContext.Current.PersistenceContext);
			if (study != null)
				Platform.Log(LogLevel.Info, "Received duplicate SOP {0} (A#:{1} StudyUid:{2}  Patient: {3}  ID:{4})",
							 sopInstanceUid,
							 study.AccessionNumber, study.StudyInstanceUid,
							 study.PatientsName, study.PatientId);
			else
				Platform.Log(LogLevel.Info,
							 "Received duplicate SOP {0} (StudyUid:{1}). Existing files haven't been processed.",
							 sopInstanceUid, studyLocation.StudyInstanceUid);

			var sopProcessingContext = new SopInstanceProcessorContext(commandProcessor, studyLocation, _context.ContextID,
																_context.Request)
			{
				DuplicateProcessing = _context.DuplicateProcessing
			};
			DicomProcessingResult result = DuplicateSopProcessorHelper.Process(sopProcessingContext, message, data,
			                                                                   sourceFilename);
			return result;
		}

        private static void Validate(DicomMessageBase message)
        {
            var validator = new DicomSopInstanceValidator();
            validator.Validate(message);
        }

        static private DicomFile ConvertToDicomFile(DicomMessageBase message, string filename, string sourceAe)
        {
            // This routine sets some of the group 0x0002 elements.
            DicomFile file;
            if (message is DicomFile)
            {
                file = message as DicomFile;
            }
            else if (message is DicomMessage)
            {
                file = new DicomFile(message as DicomMessage, filename);
            }
            else
            {
                throw new NotSupportedException(String.Format("Cannot convert {0} to DicomFile", message.GetType()));
            }

            file.SourceApplicationEntityTitle = sourceAe;
            file.TransferSyntax = message.TransferSyntax;

            return file;
        }

        static private bool HasUnprocessedCopy(ServerEntityKey storageLocationKey, string seriesUid, string sopUid)
        {
			if (WorkQueueHelper.WorkQueueUidExists(storageLocationKey, seriesUid, sopUid))
				return true;

			return ServerHelper.StudyIntegrityUidExists(storageLocationKey, seriesUid, sopUid);
        }
        
        #endregion
	}


    /// <summary>
	/// Class used for testing purposes to simulate a corrupted file.
	/// </summary>
    internal class CorruptDicomFileCommand : CommandBase
    {
        private readonly string _path;

        public CorruptDicomFileCommand(string path):base("Corrupt file", false)
        {
            _path = path;
        }

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			var rand = new Random();

			if (Diagnostics.Settings.SimulateFileCorruption)
			{
				RandomError.Generate(
					rand.Next()%2 == 0,
					String.Format("Corrupting the file {0}", _path),
					delegate
						{
							var f = new FileInfo(_path);
							long size = rand.Next(0, (int) f.Length/2);
							if (size <= 0)
							{
								FileStream s = FileStreamOpener.OpenForSoleUpdate(_path, FileMode.Truncate);
								s.Flush();
								s.Close();
							}
							else
							{
								FileStream s = FileStreamOpener.OpenForRead(_path, FileMode.Open);
								var buffer = new byte[size];
								int bytesRead = s.Read(buffer, 0, buffer.Length);
								s.Close();

								s = FileStreamOpener.OpenForSoleUpdate(_path, FileMode.Truncate);
								s.Write(buffer, 0, bytesRead);
								s.Flush();
								s.Close();
							}
						}
					);
			}
		}

    	protected override void OnUndo()
        {
            
        }
    }
}