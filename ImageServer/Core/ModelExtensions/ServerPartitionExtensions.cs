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
using System.Linq;
using ClearCanvas.ImageServer.Common.Authentication;
using ClearCanvas.ImageServer.Model;
using System.IO;
using ClearCanvas.Web.Enterprise.Authentication;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.ModelExtensions
{
    public static class ServerPartitionExtensions
    {

        /// <summary>
        /// Return the absolute path to the currently active Incoming folder for this partition
        /// or null if there's no incoming folder (e.g., Import Service is not running) or if it's undeterministic (e.g., misconfiguration)
        /// </summary>
        /// <returns></returns>
        public static string GetIncomingFolder(this ServerPartition partition)
        {
            var importServices = ServiceLock.FindServicesOfType(ServiceLockTypeEnum.ImportFiles);
            if (importServices == null || importServices.Count == 0)
                return null;

            // Only expect to have one instance of the Import File service
            if (importServices.Count>1)
            {
                Platform.Log(LogLevel.Warn, "Detect multiple instances of the Import Files Service!");
                return null;
            }

            var activeService = importServices.SingleOrDefault(s => s.Enabled);
            if (activeService == null)
                return null;

            var filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(activeService.FilesystemKey);

            // Note: Import Service doesn't care if the filesystem is disabled or readonly so we don't need to care either
            var incomingPath = Path.Combine(filesystem.Filesystem.FilesystemPath, String.Format("{0}_{1}", partition.PartitionFolder, FilesystemMonitor.ImportDirectorySuffix));
            return incomingPath;
        }


        /// <summary>
        /// Checks if the specified user is allowed to access this partition.
        /// </summary>
        /// <returns></returns>
        public static bool IsUserAccessAllowed(this ServerPartition partition, CustomPrincipal user)
        {
            Platform.CheckForNullReference(user, "user cannot be null");

            if (partition.ServerPartitionTypeEnum.Equals(ServerPartitionTypeEnum.VFS))
	        {
		        return user.IsInRole(AuthorityTokens.Vfs.ViewPartitions);
	        }

	        // If user has the "access all" token, return true
            if (user.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions))
                return true;
            
            // If user belongs to any data access authority group which can access the partition, return true
            var isAllowed = user.Credentials.DataAccessAuthorityGroups != null
                && user.Credentials.DataAccessAuthorityGroups.Any(g => partition.IsAuthorityGroupAllowed(g.ToString()));

            return isAllowed;
        }
    }
}

