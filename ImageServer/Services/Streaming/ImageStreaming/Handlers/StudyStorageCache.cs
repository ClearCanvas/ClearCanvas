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
using System.Web;
using System.Web.Caching;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class StudyStorageCache
    {
        private TimeSpan _retentionTime = TimeSpan.FromSeconds(10);

        private readonly Cache _cache = HttpRuntime.Cache;

        public TimeSpan RetentionTime
        {
            get { return _retentionTime; }
            set { _retentionTime = value; }
        }

        public void Insert(StudyStorageLocation storageLocation, string studyInstanceUid)
        {
            lock (_cache)
            {

                _cache.Add(studyInstanceUid, storageLocation, null, Cache.NoAbsoluteExpiration, _retentionTime, CacheItemPriority.Normal, null);
            }
        }

        public StudyStorageLocation Find(string studyInstanceUId)
        {
            lock (_cache)
            {
                object cached = _cache.Get(studyInstanceUId);
                if (cached != null)
                {
                    return cached as StudyStorageLocation;
                }
                else
                    return null;
            }
        }
    }
}