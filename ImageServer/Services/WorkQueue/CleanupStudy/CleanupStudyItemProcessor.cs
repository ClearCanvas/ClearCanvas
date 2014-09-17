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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.Dicom.Utilities.Command;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupStudy
{
    /// <summary>
    /// For processing 'CleanupStudy' WorkQueue items.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    public class CleanupStudyItemProcessor : BaseItemProcessor
    {

        protected override bool CanStart()
        {
            return true; // can start anytime
        }


        protected override void ProcessItem(Model.WorkQueue item)
        {
			if (!LoadWritableStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing CleanupStudy WorkQueue item, rescheduling");
                PostponeItem(item.ScheduledTime.AddMinutes(2), item.ExpirationTime.GetValueOrDefault(Platform.Time).AddMinutes(2), "Unable to find readable location.");

				return;
			}

            LoadUids(item);

            if (WorkQueueUidList.Count == 0)
            {
                if (item.ExpirationTime <= Platform.Time)
                {
                    OnAllWorkQueueUidsProcessed();
                }
                else
                {
                    PostProcessing(item, WorkQueueProcessorStatus.IdleNoDelete /*not sure why use this status, copied from the old code */, 
                                    WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                }

                return;
            }
            else
            {
                ProcessWorkQueueUids();

                PostProcessing(item,
                    WorkQueueProcessorStatus.Pending,
                    WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            }
        }

        /// <summary>
        /// Called when all WorkQueue UIDs have been processed
        /// </summary>
        private void OnAllWorkQueueUidsProcessed()
        {
            if (Study != null)
            {
                Platform.Log(LogLevel.Info,
                             "StudyProcess Cleanup completed for study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);

                Platform.Log(LogLevel.Info, "Applying rules engine to study being cleaned up to ensure disk management is applied.");

                // Run Study / Series Rules Engine.
                var engine = new StudyRulesEngine(StorageLocation, ServerPartition);
                engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);
                StorageLocation.LogFilesystemQueue();

                PostProcessing(WorkQueueItem,
                           WorkQueueProcessorStatus.Complete,
                           WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            }
            else
            {
                Platform.Log(LogLevel.Info, "StudyProcess Cleanup completed. Performing final checks..");

                // Study is never processed
                if (EnsureNoOrphanFiles())
                {
                    DeleteStudyStorage(WorkQueueItem);

                    Platform.Log(LogLevel.Info, "StudyProcess Cleanup completed. Study no longer exists in the system");
                    PostProcessing(WorkQueueItem,
                           WorkQueueProcessorStatus.Complete,
                           WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                }
            }
        }

        /// <summary>
        /// If the study is not present, check if the study folder is empty and fail the WQI if otherwise.
        /// </summary>
        /// <returns>true if the study folder is empty or the Study record exists (ie, files are no orphan)</returns>
        private bool EnsureNoOrphanFiles()
        {
            if (Study==null)
            {
                // Anything left in the study folder will become orphan
                // We can delete all of them but it's better to let the user take a look at them first.
                var path = StorageLocation.GetStudyPath();
                if (Directory.Exists(path) && !DirectoryUtility.IsEmpty(path))
                {
                    WorkQueueItem.FailureDescription = string.Format("Some files are unexpectedly found in the study folder {0}. They must be removed manually before continue", path);
                    PostProcessingFailure(WorkQueueItem, WorkQueueProcessorFailureType.Fatal);

                    return false;
                }
                

                DirectoryUtility.DeleteIfExists(path, true);
            }

            return true;
        }

        /// <summary>
        /// Deletes the <see cref="StudyStorage"/> record
        /// </summary>
        /// <param name="item"></param>
        private void DeleteStudyStorage(Model.WorkQueue item)
        {
            using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                var study = context.GetBroker<IStudyEntityBroker>();
                var criteria = new StudySelectCriteria();

                criteria.StudyInstanceUid.EqualTo(StorageLocation.StudyInstanceUid);
                criteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);

                int count = study.Count(criteria);
                if (count == 0)
                {
                    Platform.Log(LogLevel.Debug, "StudyProcess Cleanup: removing study storage record...");
                    var delete = context.GetBroker<IDeleteStudyStorage>();

                    var parms = new DeleteStudyStorageParameters
                    {
                        ServerPartitionKey = item.ServerPartitionKey,
                        StudyStorageKey = item.StudyStorageKey
                    };

                    delete.Execute(parms);

                    context.Commit();
                }

            }
        }


        /// <summary>
        /// Removes all WorkQueueUids from the database and delete the corresponding DICOM files from the filesystem.
        /// </summary>
        private void ProcessWorkQueueUids()
        {
            if (Study == null)
                Platform.Log(LogLevel.Info, "Begin StudyProcess Cleanup (Study has not been created): Attempt #{0}. {1} unprocessed files will be removed",
                                        WorkQueueItem.FailureCount + 1,
                                        WorkQueueUidList.Count);
            else
                Platform.Log(LogLevel.Info,
                             "Begin StudyProcess Cleanup for study {0},  Patient {1} (PatientId:{2} A#:{3}) on Partition {4}. Attempt #{5}. {6} unprocessed files will be removed",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description,
                             WorkQueueItem.FailureCount + 1,
                             WorkQueueUidList.Count
                             );

            

            foreach (WorkQueueUid sop in WorkQueueUidList)
            {
                string path = GetFileStoredPath(sop);

                Platform.Log(LogLevel.Info, "Cleaning up {0}", path);

                using (ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", sop.SopInstanceUid)))
                {
                    // delete the file
                    FileDeleteCommand deleteFile = new FileDeleteCommand(path, true);
                    processor.AddCommand(deleteFile);

                    // delete the WorkQueueUID from the database
                    DeleteWorkQueueUidCommand deleteUid = new DeleteWorkQueueUidCommand(sop);
                    processor.AddCommand(deleteUid);

                    try
                    {
                        // delete the directory (if empty)
                        var fileInfo = new FileInfo(path);
                        ClearCanvas.ImageServer.Core.Command.DeleteDirectoryCommand deleteDir = new ClearCanvas.ImageServer.Core.Command.DeleteDirectoryCommand(fileInfo.Directory.FullName, false, true);
                        processor.AddCommand(deleteDir);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        // ignore
                    }

                    if (!processor.Execute())
                    {
                        throw new Exception(String.Format("Unable to delete SOP {0}", sop.SopInstanceUid), processor.FailureException);
                    }
                }

                // Delete the base directory if it's empty
                var baseDir = GetBaseDirectory(sop);
                if (Directory.Exists(baseDir))
                {
                    using (ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", sop.SopInstanceUid)))
                    {
                        ClearCanvas.ImageServer.Core.Command.DeleteDirectoryCommand deleteDir = new ClearCanvas.ImageServer.Core.Command.DeleteDirectoryCommand(baseDir, false, true);
                        processor.AddCommand(deleteDir);

                        if (!processor.Execute())
                        {
                            throw new Exception(String.Format("Unable to delete {0}", baseDir), processor.FailureException);
                        }
                    }
                }

            }
			
        }


        /// <summary>
        /// Gets the base directory for the <see cref="WorkQueueUid"/> where the corresponding file is stored (see 
        /// <see cref="GetFileStoredPath"/>)
        /// </summary>
        /// <param name="sop"></param>
        /// <returns></returns>
        protected string GetBaseDirectory(WorkQueueUid sop)
        {
            if (!sop.Duplicate)
            {
                return StorageLocation.GetStudyPath();
            }
            else
            {
                string path = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
                path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
                path = Path.Combine(path, sop.GroupID);

                Debug.Assert(GetFileStoredPath(sop).IndexOf(path)==0, "Should be consistent with what GetFileStoredPath() returns");
                return path;
            }
        }

        /// <summary>
        /// Gets the path to DICOM file referenced by the <see cref="WorkQueueUid"/>. This is either the study folder or the reconcile folder.
        /// </summary>
        /// <param name="sop"></param>
        /// <returns></returns>
        protected string GetFileStoredPath(WorkQueueUid uid)
        {
            Platform.CheckForNullReference(uid, "uid");

            if (!uid.Duplicate)
            {
                return StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }
            else
            {
                // For StudyProcess (which this processor is designed to clean up), duplicates are stored in the Reconcile folder (see SopInstanceImporter)
                string path = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
                path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
                path = Path.Combine(path, uid.GroupID); 

                if (string.IsNullOrEmpty(uid.RelativePath))
                {
                    path = Path.Combine(path, StorageLocation.StudyInstanceUid);
                    var extension = uid.Extension ?? ServerPlatform.DicomFileExtension;
                    path = Path.Combine(path, uid.SopInstanceUid + "." + extension);
                }
                else
                    path = Path.Combine(path, uid.RelativePath);

                return path;
            }
        }
    }
}
