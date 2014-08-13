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
using System.Linq;
using System.Text;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Common;
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.ModelExtensions
{
    public static class WorkQueueExtensions
    {
        /// <summary>
        /// Indicates whether or not this WQI will result in patient/study information change.
        /// This usually indicates if the operation can be safely deleted from the system without any major consequences.
        /// </summary>
        public static bool WillResultInDataChanged(this WorkQueue item)
        {
            var harmlessWQITypes = new[]{
                WorkQueueTypeEnum.AutoRoute,
                WorkQueueTypeEnum.CompressStudy, // not changing patient/study info 
                WorkQueueTypeEnum.PurgeStudy, // nearline or online
                WorkQueueTypeEnum.MigrateStudy,
                WorkQueueTypeEnum.WebMoveStudy,
				WorkQueueTypeEnum.StudyAutoRoute
            };

            return !harmlessWQITypes.Contains(item.WorkQueueTypeEnum);
        }

        /// <summary>
        /// Finds all <see cref="WorkQueueUids"/> this item. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IList<WorkQueueUid> LoadAllWorkQueueUid(this WorkQueue item)
        {
            using (var readctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IWorkQueueUidEntityBroker broker = readctx.GetBroker<IWorkQueueUidEntityBroker>();
                var criteria = new WorkQueueUidSelectCriteria();
                criteria.WorkQueueKey.EqualTo(item.Key);
                return broker.Find(criteria);
            }
        }

        /// <summary>
        /// Returns <see cref="FilesystemDynamicPath"/> for all <see cref="WorkQueueUids"/> in this item. 
        /// Note: only StudyProcess is currently supported. An empty list will be returned for other types of WQI.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<FilesystemDynamicPath> GetAllWorkQueueUidPaths(this WorkQueue item)
        {
            List<FilesystemDynamicPath> paths = new List<FilesystemDynamicPath>();
            if (item.WorkQueueTypeEnum == WorkQueueTypeEnum.StudyProcess)
            {
                var workQueueUids = item.LoadAllWorkQueueUid();
                if (workQueueUids != null && workQueueUids.Count > 0)
                {
                    foreach (var uid in workQueueUids)
                    {
                        var sopPath = item.GetWorkQueueUidPath(uid);
                        if (sopPath != null)
                            paths.Add(sopPath);
                    }
                }
            }

            return paths;
        }

        /// <summary>
        /// Gets the path of the specified <see cref="WorkQueueUid"/>. 
        /// Note: only StudyProcess is currently supported. Other type of WQI will cause InvalidOperationException
        /// </summary>
        /// <param name="item"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static FilesystemDynamicPath GetWorkQueueUidPath(this WorkQueue item, WorkQueueUid uid)
        {
            // Check for invalid use of this method
            if (!uid.WorkQueueKey.Equals(item.Key))
                throw new InvalidOperationException("uid.WorkQueueKey and item.Key do not match");

            var studyStorage = item.StudyStorage;

            // Logic for determining WorkQueueUid path for StudyProcess WQI
            #region Logic forStudyProcess

            if (item.WorkQueueTypeEnum == WorkQueueTypeEnum.StudyProcess)
            {
                if (!uid.Duplicate)
                {
                    var path = Path.Combine(uid.SeriesInstanceUid, uid.SopInstanceUid + ServerPlatform.DicomFileExtension);
                    return new FilesystemDynamicPath(path, FilesystemDynamicPath.PathType.RelativeToStudyFolder);
                }
                else
                {
                    // full path = \\FS\Partition\Reconcile\UidGroup\sopUID.ext
                    // relative path = UidGroup\sopUID.ext
                    var ext = string.IsNullOrEmpty(uid.Extension) ? ServerPlatform.DicomFileExtension : uid.Extension;

                    var path = uid.GroupID;
                    if (string.IsNullOrEmpty(uid.RelativePath))
                    {
                        path = Path.Combine(path, studyStorage.StudyInstanceUid);
                        path = Path.Combine(path, uid.SopInstanceUid + "." + ext);
                    }
                    else
                    {
                        path = Path.Combine(path, uid.RelativePath);
                    }
                    return new FilesystemDynamicPath(path, FilesystemDynamicPath.PathType.RelativeToReconcileFolder);

                }


            }
            #endregion

            throw new InvalidOperationException(string.Format("GetWorkQueueUidPath should not be used for {0} WQI", item.WorkQueueTypeEnum));
        }
    }
}


    
