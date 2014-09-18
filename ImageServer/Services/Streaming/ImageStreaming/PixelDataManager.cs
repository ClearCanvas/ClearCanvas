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
using System.Threading;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    class PixelDataManager
    {
        private static TimeSpan _cacheRetentionTime = ImageStreamingServerSettings.Default.CacheRetentionWindow;

        private static readonly object _insertLock = new object();
        static private readonly Cache _cache = HttpRuntime.Cache;
        
        private readonly StudyStorageLocation _storage;

        public static PixelDataManager GetInstance(StudyStorageLocation storage)
        {
            string key = storage.Key.ToString();
            var instance = _cache[key] as PixelDataManager;
            if (instance != null) return instance;

            lock (_insertLock)
            {
                instance = _cache[key] as PixelDataManager;
                if (instance == null)
                {
                    instance = new PixelDataManager(storage);
                    _cache.Add(key, instance, null, Cache.NoAbsoluteExpiration, _cacheRetentionTime, CacheItemPriority.Default, null);
                }

            }
            return instance;
        }

        private PixelDataManager(StudyStorageLocation storage)
        {
            _storage = storage;
        }

        public DicomPixelData GetPixelData(string seriesInstanceUid, string sopInstanceUid)
        {
            DicomPixelData pd = null;

            for (int i = 0; i < 5; i++)
            {
                // look at the cache first
                pd = DicomPixelDataCache.Find(_storage, _storage.StudyInstanceUid, seriesInstanceUid, sopInstanceUid);
                if (pd != null)
                    break;

                try
                {
	                pd = DicomPixelData.CreateFrom(_storage.GetSopInstancePath(seriesInstanceUid, sopInstanceUid), DicomReadOptions.StorePixelDataReferences);
                    DicomPixelDataCache.Insert(_storage, _storage.StudyInstanceUid, seriesInstanceUid, sopInstanceUid, pd);
                    break;
                }
                catch(FileNotFoundException)
                {
                    throw;
                }
                catch (IOException)
                {
                    var rand = new Random();
                    Thread.Sleep(rand.Next(100, 500));
                }
            }
            return pd;
        }
    }
}