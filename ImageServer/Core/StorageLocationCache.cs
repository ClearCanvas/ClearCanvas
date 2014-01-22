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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// A cache of <see cref="StudyStorageLocation"/> objects.
	/// </summary>
	public class StorageLocationCache : IDisposable
	{
		#region Private Members
		private readonly Dictionary<ServerEntityKey, ServerCache<string, StudyStorageLocation>> _caches = new Dictionary<ServerEntityKey, ServerCache<string, StudyStorageLocation>>();
		private readonly object _lock = new object();
		#endregion

		#region Public Methods
		public StudyStorageLocation GetCachedStudy(ServerEntityKey partitionKey, string studyUid)
		{
			ServerCache<string,StudyStorageLocation> partitionCache;
			lock (_lock)
			{
				if (!_caches.TryGetValue(partitionKey, out partitionCache))
					return null;
			}

			return partitionCache.GetValue(studyUid);
		}

		public void AddCachedStudy(StudyStorageLocation theLocation)
		{
			ServerCache<string, StudyStorageLocation> partitionCache;
			lock (_lock)
			{
				if (!_caches.TryGetValue(theLocation.ServerPartitionKey, out partitionCache))
				{
					partitionCache = new ServerCache<string, StudyStorageLocation>(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
					_caches.Add(theLocation.ServerPartitionKey,partitionCache);
				}
			}

			partitionCache.Add(theLocation.StudyInstanceUid, theLocation);
		}
		#endregion

		#region IDisposable Implementation
		public void Dispose()
		{
			foreach (ServerCache<string, StudyStorageLocation> partitionCache in _caches.Values)
				partitionCache.Dispose();
		}
		#endregion
	}
}
