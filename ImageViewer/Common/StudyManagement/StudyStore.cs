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
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	public abstract class StudyStore : IStudyStoreQuery
	{
		static StudyStore()
		{
			InitializeIsSupported();
		}

		internal static void InitializeIsSupported()
		{
			try
			{
				var service = Platform.GetService<IStudyStoreQuery>();
				IsSupported = service != null;
				var disposable = service as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
			catch (EndpointNotFoundException)
			{
				//This doesn't mean it's not supported, it means it's not running.
				IsSupported = true;
			}
			catch (NotSupportedException)
			{
				IsSupported = false;
				Platform.Log(LogLevel.Debug, "Study Store is not supported.");
			}
			catch (UnknownServiceException)
			{
				IsSupported = false;
				Platform.Log(LogLevel.Debug, "Study Store is not supported.");
			}
			catch (Exception e)
			{
				IsSupported = false;
				Platform.Log(LogLevel.Debug, e, "Study Store is not supported.");
			}
		}

		public static bool IsSupported { get; private set; }

		public abstract GetStudyCountResult GetStudyCount(GetStudyCountRequest request);
		public abstract GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request);
		public abstract GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request);
		public abstract GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request);

		private static readonly object _syncLock = new object();
		private static StorageConfiguration _storageConfigurationCache;
		private static long _storageConfigurationCacheTime;
		private const long _storageConfigurationCacheExpiry = 10000; // really short timeout, we're just mitigating against access in tight loops

		public static void UpdateConfiguration(StorageConfiguration configuration)
		{
			Platform.GetService<IStorageConfiguration>(s =>
			                                           	{
			                                           		s.UpdateConfiguration(new UpdateStorageConfigurationRequest {Configuration = configuration});
			                                           		lock (_syncLock)
			                                           		{
			                                           			// we don't actually just update the cache, because the database is shared and may be updated by other processes
			                                           			_storageConfigurationCache = null;
			                                           		}
			                                           	});
		}

		public static StorageConfiguration GetConfiguration()
		{
			return GetConfiguration(true);
		}

		public static StorageConfiguration GetConfiguration(bool forceReload)
		{
			if (!forceReload)
			{
				lock (_syncLock)
				{
					if (_storageConfigurationCache != null && _storageConfigurationCacheTime > Environment.TickCount - _storageConfigurationCacheExpiry)
					{
						return _storageConfigurationCache.Clone();
					}
				}
			}

			StorageConfiguration configuration = null;
			Platform.GetService<IStorageConfiguration>(s =>
			                                           	{
			                                           		configuration = s.GetConfiguration(new GetStorageConfigurationRequest()).Configuration;
			                                           		if (configuration != null)
			                                           		{
			                                           			lock (_syncLock)
			                                           			{
			                                           				_storageConfigurationCache = configuration.Clone();
			                                           				_storageConfigurationCacheTime = Environment.TickCount;
			                                           			}
			                                           		}
			                                           	});
			return configuration;
		}

		public static string FileStoreDirectory
		{
			get { return GetConfiguration(false).FileStoreDirectory; }
		}

		public static long? MinimumFreeSpaceBytes
		{
			get { return GetConfiguration(false).MinimumFreeSpaceBytes; }
		}
	}
}