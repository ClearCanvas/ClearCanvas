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
using System.Threading;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{

    class PixelDataManager
    {
        static private readonly Cache _cache = HttpRuntime.Cache;
        private readonly StudyStorageLocation _storage;

        private readonly Dictionary<string, SeriesPrefetch> _prefetchers = new Dictionary<string, SeriesPrefetch>();
        
        public static PixelDataManager GetInstance(StudyStorageLocation storage)
        {
            string key = storage.Key.ToString();
            lock (_cache)
            {
                PixelDataManager instance = _cache[key] as PixelDataManager;
                if (instance == null)
                {
                    instance = new PixelDataManager(storage);
                    _cache.Add(key, instance, null, Cache.NoAbsoluteExpiration, ImageStreamingServerSettings.Default.CacheRetentionWindow, CacheItemPriority.Default, UnloadPixelDataManager);

                }
                return instance;
            }
        }

        public void Dispose()
        {
            foreach (SeriesPrefetch prefetcher in _prefetchers.Values)
            {
                prefetcher.Stop();
            }
        }

        private static void UnloadPixelDataManager(string key, object value, CacheItemRemovedReason reason)
        {
            PixelDataManager instance = value as PixelDataManager;
            instance.Dispose();
        }

        private PixelDataManager(StudyStorageLocation storage)
        {
            _storage = storage;
        }


        public DicomPixelData GetPixelData(string seriesInstanceUid, string sopInstanceUid, string nextSeriesInstanceUid, string nextSopInstanceUid)
        {
            DicomPixelData pd = null;

            for (int i = 0; i < 5; i++)
            {
                // look at the cache first
                pd = DicomPixelDataCache.Find(_storage, _storage.StudyInstanceUid, seriesInstanceUid, sopInstanceUid);
                if (pd != null)
                    break;

                // Check if the file has been requested to open by the client
                string key = string.Format("ImageStreamPrefetchStream/{0}/{1}/{2}/{3}",
                                           _storage.ServerPartition.AeTitle, _storage.StudyInstanceUid,
                                           seriesInstanceUid, sopInstanceUid);
                
                Stream preopenStream = HttpRuntime.Cache[key] as Stream;
                if (preopenStream!=null)
                {
                    pd = DicomPixelData.CreateFrom(preopenStream);
                    DicomPixelDataCache.Insert(_storage, _storage.StudyInstanceUid, seriesInstanceUid, sopInstanceUid, pd);
                    preopenStream.Close();
                    HttpRuntime.Cache.Remove(key);
                    break;
                }
                else
                {
                    try
                    {
                        pd = DicomPixelData.CreateFrom(_storage.GetSopInstancePath(seriesInstanceUid, sopInstanceUid));
                        DicomPixelDataCache.Insert(_storage, _storage.StudyInstanceUid, seriesInstanceUid, sopInstanceUid, pd);
                        break;
                    }
                    catch(FileNotFoundException )
                    {
                        throw;
                    }
                    catch (IOException)
                    {
                        Random rand = new Random();
                        Thread.Sleep(rand.Next(100, 500));
                    }
                }
                
            }
            OnPixelDataLoaded(sopInstanceUid, nextSeriesInstanceUid, nextSopInstanceUid);
            return pd;
        }

        private SeriesPrefetch GetSeriesPrefetcher(string seriesInstanceUid)
        {
            lock (_prefetchers)
            {
                if (_prefetchers.ContainsKey(seriesInstanceUid))
                    return _prefetchers[seriesInstanceUid];
                else
                {
                    SeriesPrefetch prefetcher = new SeriesPrefetch(_storage, seriesInstanceUid);
                    _prefetchers.Add(seriesInstanceUid, prefetcher);
                    return prefetcher;
                }
            }

        }
        private void OnPixelDataLoaded(string seriesInstanceUid, string nextSeriesInstanceUid, string nextSopInstanceUid)
        {
            if (string.IsNullOrEmpty(nextSeriesInstanceUid)==false && string.IsNullOrEmpty(nextSopInstanceUid)==false)
            {
                SeriesPrefetch prefetcher = GetSeriesPrefetcher(seriesInstanceUid);
                prefetcher.OnImageLoaded(nextSeriesInstanceUid, nextSopInstanceUid);
            } 
        }
    }


    class PrefetchQueueItem
    {
        private string _seriesInstanceUid;
        private string _sopInstanceUid;
        private string _instanceNumner;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }

        public string InstanceNumner
        {
            get { return _instanceNumner; }
            set { _instanceNumner = value; }
        }
    }

    class SeriesPrefetch
    {
        private readonly StudyStorageLocation _storage;
        private bool _stop;
        private readonly string _seriesInstanceUid;
        readonly AutoResetEvent _enqueueEvent = new AutoResetEvent(false);

        readonly Queue<PrefetchQueueItem> _prefetchQueue = new Queue<PrefetchQueueItem>();
        
        public SeriesPrefetch(StudyStorageLocation storage, string seriesInstanceUid)
        {
            _storage = storage;
            _seriesInstanceUid = seriesInstanceUid;
            Thread prefetchThread = new Thread(Run);
            prefetchThread.Start();
        }

        private void Run()
        {
            while (!_stop)
            {
                while (_prefetchQueue.Count > 0)
                {
                    PrefetchQueueItem item = _prefetchQueue.Dequeue();
                    if (item != null)
                        Prefetch(item);
                }

                _enqueueEvent.WaitOne(1000, true);

            }

            Platform.Log(LogLevel.Debug, "Prefetch has stopped. Series {0}", _seriesInstanceUid);
        }

        public void Stop()
        {
            _stop = true;
        }

        private void Prefetch(PrefetchQueueItem item)
        {
            try{
                string key = string.Format("ImageStreamPrefetchStream/{0}/{1}/{2}/{3}",
                                           _storage.ServerPartition.AeTitle, _storage.StudyInstanceUid,
                                           item.SeriesInstanceUid, item.SopInstanceUid);

                for (int i = 0; i < 5; i++)
                {
                    Stream stream = HttpRuntime.Cache[key] as Stream;
                    if (stream != null)
                    {
                        // already pre-open
                        break;
                    }

                    else
                    {
                        try
                        {
                            stream = new BufferedStream(File.OpenRead(_storage.GetSopInstancePath(item.SeriesInstanceUid, item.SopInstanceUid)));
                            HttpRuntime.Cache.Insert(key, stream, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, PrefetchUnloaded);
                            break;
                        }
                        catch (FileNotFoundException)
                        {
                            throw;
                        }
                        catch (IOException)
                        {
                            Random rand = new Random();
                            Thread.Sleep(rand.Next(100, 500));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error occurred during prefetch SOP: {0}", item.SopInstanceUid);
            }
            
        }

        private void PrefetchUnloaded(string key, object value, CacheItemRemovedReason reason)
        {
            Stream stream = value as Stream;
            if (stream!=null)
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public void OnImageLoaded(string nextSeriesInstanceUid, string nextSopInstanceUid)
        {
            PrefetchQueueItem item = new PrefetchQueueItem();
            item.SeriesInstanceUid = nextSeriesInstanceUid;
            item.SopInstanceUid = nextSopInstanceUid;
            _prefetchQueue.Enqueue(item);
            _enqueueEvent.Set();
        }
    }


}