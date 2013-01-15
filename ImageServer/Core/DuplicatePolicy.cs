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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Common.Caching;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Core
{
    public static class DuplicatePolicy
    {
        const string CacheId = "ClearCanvas.ImageServer.Core.DuplicatePolicy";

        /// <summary>
        /// Indicates whether or not the ServerParition duplicate policy is overridden for the specified study 
        /// </summary>
        /// <param name="studyStorageLocation"></param>
        /// <returns></returns>
        public static bool IsParitionDuplicatePolicyOverridden(StudyStorageLocation studyStorageLocation)
        {
            var list = GetStudyUIDsWithDuplicatePolicyOverride();
            if (list != null && list.Any(uid => uid.Equals(studyStorageLocation.StudyInstanceUid)))
                return true;

            return false;
        }

        #region Helpers

        private static string[] GetStudyUIDsWithDuplicatePolicyOverride()
        {
            var settings = GetSettings();

            if (!string.IsNullOrEmpty(settings.OverrideDuplicatePolicyForStudyUIDs))
            {
                var list = settings.OverrideDuplicatePolicyForStudyUIDs.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                return list;
            }
            return null;
        }

        private static Settings GetSettings()
        {
            Settings settings = null;

            if (Cache.IsSupported())
            {
                using (var cacheClient = Cache.CreateClient(CacheId))
                {
                    settings = cacheClient.Get("Settings", new CacheGetOptions("default")) as Settings;
                    if (settings == null)
                    {
                        settings = new Settings(); 
                        cacheClient.Put("Settings", settings, new CachePutOptions("default", TimeSpan.FromSeconds(5), true));
                    }
                }
            }

            return settings ?? new Settings();
        }

        #endregion

    }
}
