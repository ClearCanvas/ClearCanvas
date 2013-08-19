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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Core.Helpers
{
    /// <summary>
    /// Helper class shared by all code.
    /// </summary>
    public static class ServerHelper
    {
        #region Private Fields
        private static readonly object _syncLock = new object();
        private static DateTime? _systemModeLastCheckTimestamp;
        private static ServerOperatingMode _serverMode;
        #endregion

     
    	/// <summary>
        /// Insert a request to restore the specified <seealso cref="StudyStorage"/>
        /// </summary>
        /// <param name="storage"></param>
        /// <returns>Reference to the <see cref="RestoreQueue"/> that was inserted.</returns>
        static public RestoreQueue InsertRestoreRequest(StudyStorage storage)
        {
            // TODO:
            // Check the stored procedure to see if it will insert another request if one already exists

            Platform.CheckForNullReference(storage, "storage");

            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertRestoreQueue broker = updateContext.GetBroker<IInsertRestoreQueue>();

                InsertRestoreQueueParameters parms = new InsertRestoreQueueParameters {StudyStorageKey = storage.Key};

            	RestoreQueue queue = broker.FindOne(parms);

                if (queue == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to request restore for study {0}", storage.StudyInstanceUid);
                    return null;
                }

                updateContext.Commit();
                Platform.Log(LogLevel.Info, "Restore requested for study {0}", storage.StudyInstanceUid);
                return queue;
            }
        }

        /// <summary>
        /// Insert a request to restore the specified <seealso cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="storageLocation"></param>
        /// <returns>Reference to the <see cref="RestoreQueue"/> that was inserted.</returns>
        static public RestoreQueue InsertRestoreRequest(StudyStorageLocation storageLocation)
        {
            Platform.CheckForNullReference(storageLocation, "storageLocation");

            return InsertRestoreRequest(storageLocation.StudyStorage);
        }

        /// <summary>
        /// Finds a list of <see cref="StudyIntegrityQueue"/> related to the specified <see cref="StudyStorage"/>.
        /// </summary>
		/// <param name="studyStorageKey"></param>
        /// <param name="filter">A delegate that will be used to filter the returned list. Pass in Null to get the entire list.</param>
        /// <returns>A list of  <see cref="StudyIntegrityQueue"/></returns>
        static public IList<StudyIntegrityQueue> FindSIQEntries(ServerEntityKey studyStorageKey, Predicate<StudyIntegrityQueue> filter)
        {
            using (ServerExecutionContext scope = new ServerExecutionContext())
            {
                IStudyIntegrityQueueEntityBroker broker = scope.PersistenceContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                StudyIntegrityQueueSelectCriteria criteria = new StudyIntegrityQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(studyStorageKey);
                criteria.InsertTime.SortDesc(0);
                IList<StudyIntegrityQueue> list = broker.Find(criteria);
                if (filter != null)
                {
                    CollectionUtils.Remove(list, filter);
                }
                return list;
            }
        }

		/// <summary>
		/// Checks for the existinance of a SOP for a given Study in the <see cref="StudyIntegrityQueue"/>.
		/// </summary>
		/// <param name="studyStorageKey">The StudyStorage primary key</param>
		/// <param name="sopInstanceUid">The Sop Instance ot look for</param>
		/// <param name="seriesInstanceUid">The Series Instance Uid of the Sop</param>
		/// <returns>true if an entry exists, false if it doesn't</returns>
		static public bool StudyIntegrityUidExists(ServerEntityKey studyStorageKey, string seriesInstanceUid, string sopInstanceUid)
		{
			Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

			using (ServerExecutionContext scope = new ServerExecutionContext())
			{
				IStudyIntegrityQueueEntityBroker broker = scope.PersistenceContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
				StudyIntegrityQueueUidSelectCriteria uidSelectCriteria = new StudyIntegrityQueueUidSelectCriteria();
				uidSelectCriteria.SeriesInstanceUid.EqualTo(seriesInstanceUid);
				uidSelectCriteria.SopInstanceUid.EqualTo(sopInstanceUid);
				StudyIntegrityQueueSelectCriteria selectCriteria = new StudyIntegrityQueueSelectCriteria();
				selectCriteria.StudyStorageKey.EqualTo(studyStorageKey);
				selectCriteria.StudyIntegrityQueueUidRelatedEntityCondition.Exists(uidSelectCriteria);

				return broker.Count(selectCriteria) > 0;
			}
		}





        /// <summary>
        /// Gets a string value that represents the group ID for a <see cref="DicomFile"/> based on
        /// the source/destination AE title and the application-provided timestamp.
        /// </summary>
        /// <param name="file">The <see cref="DicomFile"/></param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the file belongs to</param>
        /// <param name="timestamp">Optional timestamp to be used to generate the group ID. 
        /// If null, the current timestamp will be used.</param>
        /// <returns></returns>
        public static string GetUidGroup(DicomFile file, ServerPartition partition, DateTime? timestamp)
        {
            return String.Format("{0}_{1}",
                                 String.IsNullOrEmpty(file.SourceApplicationEntityTitle)
                                     ? partition.AeTitle
                                     : file.SourceApplicationEntityTitle,
                                     timestamp!=null? timestamp.Value.ToString("yyyyMMddHHmmss"): Platform.Time.ToString("yyyyMMddHHmmss"));
        }

        /// <summary>
        /// Sets the Queue Study State of the study.
        /// </summary>
        /// <param name="studyStorageKey">The <see cref="ServerEntityKey"/> of the <see cref="StudyStorage"/> record.</param>
        /// <param name="state">The state of the study to set</param>
        /// <param name="failureReason">A string value describing why the state could not be set.</param>
        /// <returns>True if the state of the study was successfully set. False otherwise.</returns>
        public static bool LockStudy(ServerEntityKey studyStorageKey, QueueStudyStateEnum state, out string failureReason)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
                LockStudyParameters lockStudyParams = new LockStudyParameters
                                                      	{
                                                      		StudyStorageKey = studyStorageKey,
                                                      		QueueStudyStateEnum = state
                                                      	};

            	if (!lockStudyBroker.Execute(lockStudyParams) || !lockStudyParams.Successful)
                {
                    failureReason = lockStudyParams.FailureReason;
                    return false;
                }
            	updateContext.Commit();
            	failureReason = null;
            	return true;
            }
        }

        /// <summary>
        /// Resets the state of the study set by <see cref="LockStudy"/>.
        /// </summary>
        /// <param name="studyStorageKey"></param>
        /// <returns></returns>
        public static bool UnlockStudy(ServerEntityKey studyStorageKey)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
                LockStudyParameters lockStudyParams = new LockStudyParameters
                                                      	{
                                                      		StudyStorageKey = studyStorageKey,
                                                      		QueueStudyStateEnum = QueueStudyStateEnum.Idle
                                                      	};

            	if (!lockStudyBroker.Execute(lockStudyParams) || !lockStudyParams.Successful)
                    return false;

            	updateContext.Commit();
            	return true;
            }
        }

        /// <summary>
        /// Returns the name of the current user.
        /// </summary>
        public static string CurrentUserName
        {
            get
            {
            	if (Thread.CurrentPrincipal is CustomPrincipal)
                    return (Thread.CurrentPrincipal as CustomPrincipal).DisplayName;
            	return Thread.CurrentPrincipal.Identity.Name;
            }
        }

        //// <summary>
        /// Returns the name of the current user.
        /// </summary>
        public static string CurrentUserId
        {
            get
            {
            	if (Thread.CurrentPrincipal is CustomPrincipal)
                    return (Thread.CurrentPrincipal as CustomPrincipal).Credentials.UserName;
            	return Thread.CurrentPrincipal.Identity.Name;
            }
        }

        /// <summary>
        /// Helper method to return the path to the duplicate image (in the Reconcile folder)
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <param name="sop"></param>
        /// <returns></returns>
        public static String GetDuplicateUidPath(StudyStorageLocation studyStorage, WorkQueueUid sop)
        {
            string dupPath = GetDuplicateGroupPath(studyStorage, sop);
            dupPath = string.IsNullOrEmpty(sop.RelativePath)
                        ? Path.Combine(dupPath,
                                       Path.Combine(studyStorage.StudyInstanceUid, sop.SopInstanceUid + "." + sop.Extension))
                        : Path.Combine(dupPath, sop.RelativePath);

            #region BACKWARD_COMPATIBILTY_CODE

            if (string.IsNullOrEmpty(sop.RelativePath) && !File.Exists(dupPath))
            {
                string basePath = Path.Combine(studyStorage.GetStudyPath(), sop.SeriesInstanceUid);
                basePath = Path.Combine(basePath, sop.SopInstanceUid);
                if (sop.Extension != null)
                    dupPath = basePath + "." + sop.Extension;
                else
                    dupPath = basePath + ".dcm";
            }

            #endregion

            return dupPath;
        }

        /// <summary>
        /// Helper method to return the path to the folder containing the duplicate images (in the Reconcile folder)
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <param name="sop"></param>
        /// <returns></returns>
        public static String GetDuplicateGroupPath(StudyStorageLocation studyStorage, WorkQueueUid sop)
        {
            String groupFolderPath = Path.Combine(studyStorage.FilesystemPath, studyStorage.PartitionFolder);
            groupFolderPath = Path.Combine(groupFolderPath, ServerPlatform.ReconcileStorageFolder);
            groupFolderPath = Path.Combine(groupFolderPath, sop.GroupID);

            return groupFolderPath;
        }

        /// <summary>
        /// Helper method to return the path to the root folder under which duplicates are stored.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        public static String GetDuplicateFolderRootPath(StudyStorageLocation studyStorage)
        {
            String path = Path.Combine(studyStorage.FilesystemPath, studyStorage.PartitionFolder);
            path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);

            return path;
        }

        /// <summary>
        /// Helper method to return the path to the folder containing the duplicate images (in the Reconcile folder)
        /// </summary>
        /// <param name="storageLocation"></param>
        /// <param name="queueItem"></param>
        /// <returns></returns>
        public static string GetDuplicateGroupPath(StudyStorageLocation storageLocation, WorkQueue queueItem)
        {
            string path = Path.Combine(storageLocation.FilesystemPath, storageLocation.PartitionFolder);
            path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
            path = Path.Combine(path, queueItem.GroupID);
            return path;
        }

        /// <summary>
        /// Indicates if the ImageServer is operating as a temporary cache mode 
        /// (i.e., images will not be archived and will be deleted per delete rule)
        /// </summary>
        public static ServerOperatingMode ServerOperatingMode
        {
            get
            {
                CheckSystemMode();
                return _serverMode;
            }
        }

        private static void CheckSystemMode()
        {
            var now = Platform.Time;

            if (!_systemModeLastCheckTimestamp.HasValue || now - _systemModeLastCheckTimestamp > TimeSpan.FromSeconds(15))
            {
                lock (_syncLock)
                {
                    if (!_systemModeLastCheckTimestamp.HasValue || now - _systemModeLastCheckTimestamp > TimeSpan.FromSeconds(15))
                    {
                        try
                        {
                            using (var context = new ServerExecutionContext())
                            {
                                var deleteRuleBroker = context.ReadContext.GetBroker<IServerRuleEntityBroker>();
                                var deleteRuleSearchCriteria = new ServerRuleSelectCriteria();
                                deleteRuleSearchCriteria.ServerRuleTypeEnum.EqualTo(ServerRuleTypeEnum.StudyDelete);
                                deleteRuleSearchCriteria.Enabled.EqualTo(true);
                                var deleteRules = deleteRuleBroker.Find(deleteRuleSearchCriteria);

                                if (deleteRules == null || deleteRules.Count == 0)
                                    _serverMode = ServerOperatingMode.Archive;

                                var defaultDeleteRuleExists = deleteRules.Any(r => r.RuleName.Equals("Default Delete"));
                                var customDeleteRuleExists = deleteRules.Any(r => !r.RuleName.Equals("Default Delete"));

                                if (defaultDeleteRuleExists)
                                    _serverMode = ServerOperatingMode.TemporaryCache;
                                else
                                {
                                    if (customDeleteRuleExists)
                                        _serverMode = ServerOperatingMode.MixedMode;
                                    else
                                        _serverMode = ServerOperatingMode.Archive;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex);
                        }
                        finally
                        {
                            _systemModeLastCheckTimestamp = now;
                        }
                    }
                }
            }
        }
    }

    public enum ServerOperatingMode
    {
        Archive,
        TemporaryCache,
        MixedMode
    }
}
