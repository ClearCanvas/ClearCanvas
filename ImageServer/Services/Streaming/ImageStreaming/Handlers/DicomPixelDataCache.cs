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
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    internal class DicomPixelDataCache
    {
        private static TimeSpan _retentionTime = ImageStreamingServerSettings.Default.CacheRetentionWindow;
        private static readonly Cache _cache = HttpRuntime.Cache;

        public static TimeSpan RetentionTime
        {
            get { return _retentionTime; }
            set { _retentionTime = value; }
        }

        static public void Insert(StudyStorageLocation storageLocation, string studyInstanceUId, string seriesInstanceUid, string sopInstanceUid,  DicomPixelData pixeldata)
        {
            string key = String.Format("{0}/{1}/{2}/{3}", storageLocation.GetKey().Key, studyInstanceUId, seriesInstanceUid, sopInstanceUid);
            _cache.Add(key, pixeldata, null, Cache.NoAbsoluteExpiration, _retentionTime, CacheItemPriority.Normal, null);
        }

        static public DicomPixelData Find(StudyStorageLocation storageLocation, string studyInstanceUId, string seriesInstanceUid, string sopInstanceUid )
        {
            string key = String.Format("{0}/{1}/{2}/{3}", storageLocation.GetKey().Key, studyInstanceUId, seriesInstanceUid, sopInstanceUid);
            return _cache.Get(key) as DicomPixelData;
        }
    }
}