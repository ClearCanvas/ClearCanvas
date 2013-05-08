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
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Common.Exceptions;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class ServerTransientError:Exception
    {
        public ServerTransientError(){}

        public ServerTransientError(String message)
            : base(message)
        {
        }
        public ServerTransientError(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class StudyAccessException : ServerTransientError
    {
        private QueueStudyStateEnum _studyState; 
        public StudyAccessException(String message, QueueStudyStateEnum state, Exception innerException)
            : base(message, innerException)
        {
            _studyState = state;
        }

        public QueueStudyStateEnum StudyState
        {
            get { return _studyState; }
            set { _studyState = value; }
        }
    }

    class StudyStorageLoader
	{
		#region Private Members
		private static readonly ServerSessionList _serverSessions = new ServerSessionList();
        private static readonly StudyStorageLoaderStatistics _statistics = new StudyStorageLoaderStatistics();
        private readonly string _sessionId;
        private bool _cacheEnabled = true;
        private TimeSpan _cacheRetentionTime = TimeSpan.FromSeconds(10); //default
		#endregion

		#region Public Properties
		public StudyStorageLoader(string sessionId)
        {
            _sessionId = sessionId;
        }

        public bool CacheEnabled
        {
            get { return _cacheEnabled; }
            set { _cacheEnabled = value; }
        }

        public TimeSpan CacheRetentionTime
        {
            get { return _cacheRetentionTime; }
            set { _cacheRetentionTime = value; }
		}

		#endregion

		#region Public Methods

        /// <summary>
        /// Finds the <see cref="StudyStorageLocation"/> for the specified study
        /// </summary>
        /// <param name="studyInstanceUid"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        /// 
    	public StudyStorageLocation Find(string studyInstanceUid, ServerPartition partition)
        {
            TimeSpan STATS_WINDOW = TimeSpan.FromMinutes(1);
            StudyStorageLocation location;
            if (!CacheEnabled)
            {
            	FilesystemMonitor.Instance.GetReadableStudyStorageLocation(partition.Key, studyInstanceUid,
            	                                                           StudyRestore.True, StudyCache.False,
            	                                                           out location);
            }
            else
            {
                Session session = _serverSessions[_sessionId];
                StudyStorageCache cache;
                lock (session)
                {
                    cache = session["StorageLocationCache"] as StudyStorageCache;

                    if (cache == null)
                    {
                        cache = new StudyStorageCache {RetentionTime = CacheRetentionTime};
                    	session.Add("StorageLocationCache", cache);
                    }
                }

                // lock the cache instead of the list so that we won't block requests from other
                // clients if we need to fetch from the database.
                lock (cache)
                {
                    location = cache.Find(studyInstanceUid);
                    if (location == null)
                    {
                        _statistics.Misses++;
                        
						FilesystemMonitor.Instance.GetReadableStudyStorageLocation(partition.Key, studyInstanceUid, StudyRestore.True,StudyCache.False, out location);

						cache.Insert(location, studyInstanceUid);
                        Platform.Log(LogLevel.Info, "Cache (since {0}): Hits {1} [{3:0}%], Miss {2}",
                                         _statistics.StartTime,
                                         _statistics.Hits, _statistics.Misses,
                                         (float)_statistics.Hits / (_statistics.Hits + _statistics.Misses) * 100f);
                    }
                    else
                    {
                        _statistics.Hits++;
                    }

                    if (_statistics.ElapsedTime > STATS_WINDOW)
                    {
                        _statistics.Reset();
                    }

                }
            
            }

        	//TODO (CR April 2011): Should this be "not found"?
            if (location == null)
                throw new StudyIsNearlineException(false);
               
            return location;
		}
		#endregion
	}

    class StudyStorageLoaderStatistics : StatisticsSet
    {
        public DateTime StartTime = DateTime.Now;

        public TimeSpan ElapsedTime
        {
            get
            {
                return DateTime.Now - StartTime;
            }
        }

        /// <summary>
        /// Total number of hits
        /// </summary>
        public ulong Hits
        {
            get
            {
                if (this["Hits"] == null)
                    this["Hits"] = new Statistics<ulong>("Hits");

                return (this["Hits"] as Statistics<ulong>).Value;
            }
            set
            {
                this["Hits"] = new Statistics<ulong>("Hits", value);
            }
        }

        /// <summary>
        /// Total number of misses
        /// </summary>
        public ulong Misses
        {
            get
            {
                if (this["Misses"] == null)
                    this["Misses"] = new Statistics<ulong>("Misses");

                return (this["Misses"] as Statistics<ulong>).Value;
            }
            set
            {
                this["Misses"] = new Statistics<ulong>("Misses", value);
            }
        }

        public void Reset()
        {
            StartTime = DateTime.Now;
            Hits = 0;
            Misses = 0;
        }
    }

}