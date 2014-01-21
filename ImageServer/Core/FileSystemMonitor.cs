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
using System.Collections.ObjectModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using Timer = System.Threading.Timer;

namespace ClearCanvas.ImageServer.Core
{
	public enum StudyRestore
	{
		True,
		False
	}

	public enum StudyCache
	{
		True,
		False
	}

	/// <summary>
	/// Event args for partition monitor
	/// </summary>
	public class FilesystemChangedEventArgs : EventArgs
	{
		private readonly FilesystemMonitor _monitor;
		public FilesystemChangedEventArgs(FilesystemMonitor theMonitor)
		{
			_monitor = theMonitor;
		}

		public FilesystemMonitor Monitor
		{
			get { return _monitor; }
		}
	}

    /// <summary>
    /// Represents a lookup table mapping a <see cref="FilesystemTierEnum"/> to a list of <see cref="ServerFilesystemInfo"/> on that tier.
    /// </summary>
    class TierInfo : Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>>
    {
    }

    /// <summary>
    /// Represents a collection of <See cref="ServerFilesystemInfo"/>
    /// </summary>
    class FilesystemInfoCollection : Collection<ServerFilesystemInfo>
    {
        /// <summary>
        /// Create a new <see cref="FilesystemInfoCollection"/> with copies of the items in another list
        /// </summary>
        /// <param name="anotherList"></param>
        public FilesystemInfoCollection(IEnumerable<ServerFilesystemInfo> anotherList)
        {
            if (anotherList!=null)
            {
                foreach (ServerFilesystemInfo fs in anotherList)
                {
                    Add(new ServerFilesystemInfo(fs));
                }
            }
            
        }
    }

	/// <summary>
	/// Class for monitoring the status of filesystems.
	/// </summary>
	/// <remarks>
	/// The class creates a background thread that monitors the current usage of the filesystems.  
	/// The class will also update itself and retrieve updated filesystem information from
	/// the database periodically.
	/// </remarks>
	public class FilesystemMonitor : IDisposable
	{
		#region Private Static Members
		private static FilesystemMonitor _singleton;
		private static readonly object SyncLock = new object();
		#endregion

		#region Private Members
		private readonly Dictionary<ServerEntityKey, ServerFilesystemInfo> _filesystemList = new Dictionary<ServerEntityKey, ServerFilesystemInfo>();
		private TierInfo _tierInfo = new TierInfo();
		private readonly IPersistentStore _store;
		private Timer _dbTimer;
		private Timer _fsTimer;
		private EventHandler<FilesystemChangedEventArgs> _changedListener;
		private readonly StorageLocationCache _storageLocationCache = new StorageLocationCache();
		#endregion

		#region Private Constructors
		private FilesystemMonitor()
		{
			_store = PersistentStoreRegistry.GetDefaultStore();
		}
		#endregion

		#region Events
		/// <summary>
		/// Event handler for changes in filesystems.
		/// </summary>
		public event EventHandler<FilesystemChangedEventArgs> Changed
		{
			add { _changedListener += value; }
			remove { _changedListener -= value; }
		}
		#endregion

		#region Public Static Members

		public static String ImportDirectorySuffix = "Incoming";

		/// <summary>
		/// Singleton FilesystemMonitor created the first time its referenced.
		/// </summary>
		public static FilesystemMonitor Instance
		{
			get
			{
				if (_singleton == null)
				{
					lock (SyncLock)
					{
						if (_singleton == null)
						{
							_singleton = new FilesystemMonitor();
							_singleton.Initialize();
						}
						return _singleton;
					}
				}
				return _singleton;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Return a snapshot the current filesystems.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ServerFilesystemInfo> GetFilesystems()
		{
			lock (SyncLock)
			{
                // Return a list with copies of the current <see cref="ServerFilesystemInfo"/>
			    return new FilesystemInfoCollection(_filesystemList.Values);
			}
		}

        /// <summary>
        /// Return a snapshot the current filesystems.
        /// </summary>
        /// <returns></returns>
        public IList<ServerFilesystemInfo> GetFilesystems(Predicate<ServerFilesystemInfo> filter)
        {
            lock (SyncLock)
            {
                FilesystemInfoCollection copy = new FilesystemInfoCollection(_filesystemList.Values);
                CollectionUtils.Remove(copy, filter);
                return copy;
            }
        }


		/// <summary>
		/// Get the snapshot of the specific filesystem.
		/// </summary>
		/// <param name="filesystemKey">The primary key of a filesystem to get info for.</param>
		/// <returns>A <see cref="ServerFilesystemInfo"/> structure for the filesystem, or null if the filesystem ahs not been found.</returns>
		public ServerFilesystemInfo GetFilesystemInfo(ServerEntityKey filesystemKey)
		{
			lock (SyncLock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				
                // TODO: Should we throw an exception instead?
				if (info==null)
					return null;

				return new ServerFilesystemInfo(info); // return a copy 
			}
		}

		/// <summary>
		/// Calculate the number of bytes to remove from a filesystem to get it to the low watermark.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
		public float CheckFilesystemBytesToRemove(ServerEntityKey filesystemKey)
		{
			lock (SyncLock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
					return info.BytesToRemove;
			}
			return 0.0f;
		}

		/// <summary>
		/// Check if a filesystem is writeable.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <param name="reason">The reason the filesystem is not writeable</param>
		/// <returns></returns>
		public bool CheckFilesystemWriteable(ServerEntityKey filesystemKey, out string reason)
		{
			reason = string.Empty;
			lock (SyncLock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
				{
					if (!info.Writeable)
					{
						if (!info.Enable)
							reason = "Filesystem is disabled";
						return false;
					}
					return true;
				}
			}
			return false;
		}

        /// <summary>
        /// Helper method to verify if the specified <see cref="StudyStorageLocation"/> 
        /// is writable and throw <see cref="FilesystemNotWritableException"/> if it is not.
        /// </summary>
        /// <param name="location"></param>
        public void EnsureStorageLocationIsWritable(StudyStorageLocation location)
        {
            ServerFilesystemInfo fs = GetFilesystemInfo(location.FilesystemKey);

            if (!fs.Enable)
                throw new FilesystemNotWritableException(fs.Filesystem.FilesystemPath) { Reason = "It is disabled" };

            if (!fs.Online)
                throw new FilesystemNotWritableException(fs.Filesystem.FilesystemPath) { Reason = "It is offline/unreachable" };

            if (fs.ReadOnly)
                throw new FilesystemNotWritableException(fs.Filesystem.FilesystemPath) { Reason = "It is read-only" };

            if (fs.Full)
                throw new FilesystemNotWritableException(fs.Filesystem.FilesystemPath) { Reason = "It is full" };

        }


		/// <summary>
		/// Check if a filesystem is readable.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <param name="reason">The reason the filesystem isn't readable.</param>
		/// <returns></returns>
		public bool CheckFilesystemReadable(ServerEntityKey filesystemKey, out string reason)
		{
			reason = string.Empty;

			lock (SyncLock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
				{
					if (!info.Readable)
					{
						if (!info.Enable)
							reason = "Filesystem is disabled";
						else if (!info.Online)
							reason = "Filesystem is not online";
						else if (info.Filesystem.WriteOnly)
							reason = "Filesystem is write only";
					}
					return info.Readable;
				}
			}
			return false;
		}

		/// <summary>
		/// Check if a filesystem is online.
		/// </summary>
		/// <param name="filesystemKey">The filesystem primary Key</param>
		/// <param name="reason">The reason the filesystem isn't online</param>
		/// <returns></returns>
		public bool CheckFilesystemOnline(ServerEntityKey filesystemKey, out string reason)
		{
			reason = string.Empty;
			lock (SyncLock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
				{
					if (!info.Online)
						reason = "The filesystem is offline.";
					return info.Online;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the first filesystem in lower tier for storage purpose.
		/// </summary>
		/// <param name="filesystem">The current filesystem</param>
		/// <returns></returns>
		public ServerFilesystemInfo GetLowerTierFilesystemForStorage(ServerFilesystemInfo filesystem)
		{
            lock(SyncLock)
            {
                List<FilesystemTierEnum> lowerTiers = FindLowerTierFilesystems(filesystem);
                if (lowerTiers == null || lowerTiers.Count == 0)
                    return null;

                List<ServerFilesystemInfo> list = new List<ServerFilesystemInfo>();
                foreach (FilesystemTierEnum tier in lowerTiers)
                {
                    list.AddRange(_tierInfo[tier]);
                }
                CollectionUtils.Remove(list, fs => !fs.Writeable);
                list = CollectionUtils.Sort(list, FilesystemSorter.SortByFreeSpace);
                ServerFilesystemInfo lowtierFilesystem= CollectionUtils.FirstElement(list);
				if (lowtierFilesystem == null)
					return null;
                return new ServerFilesystemInfo(lowtierFilesystem);//return a copy
            }
			
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study.  Checks if the filesystem is readable.
		/// </summary>
		/// <param name="partitionKey"></param>
		/// <param name="studyInstanceUid"></param>
		/// <param name="restore"></param>
		/// <param name="cache"></param>
		/// <param name="location"></param>
		public void GetReadableStudyStorageLocation(ServerEntityKey partitionKey, string studyInstanceUid, StudyRestore restore, StudyCache cache,
					out StudyStorageLocation location)
		{
			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				// Get the cached value, if it exists, otherwise fall down and recheck
				// and handle any nearline issues below
				location = _storageLocationCache.GetCachedStudy(partitionKey, studyInstanceUid);
				if (location != null)
				{
					string reason;
					if (CheckFilesystemReadable(location.FilesystemKey, out reason))
					{
						return;
					}
				}

				IQueryStudyStorageLocation procedure = context.ReadContext.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters
				                                            	{
				                                            		StudyInstanceUid = studyInstanceUid,
				                                            		ServerPartitionKey = partitionKey
				                                            	};
				IList<StudyStorageLocation> locationList = procedure.Find(parms);

				bool foundStudy = false;

				FilesystemNotReadableException x = new FilesystemNotReadableException();

				foreach (StudyStorageLocation studyLocation in locationList)
				{
					string reason;
					if (CheckFilesystemReadable(studyLocation.FilesystemKey, out reason))
					{
						location = studyLocation;

						if (cache == StudyCache.True)
							_storageLocationCache.AddCachedStudy(location);

						return;
					}
					foundStudy = true;
					x.Path = studyLocation.FilesystemPath;
					x.Reason = reason;
				}

				if (foundStudy)
					throw x;

				CheckForStudyRestore(context.ReadContext, partitionKey, studyInstanceUid, restore);
			}
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study storage key.  Checks if the filesystem is online.
		/// </summary>
		/// <param name="studyStorageKey"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public bool GetReadableStudyStorageLocation(ServerEntityKey studyStorageKey, out StudyStorageLocation location)
		{
			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				IQueryStudyStorageLocation procedure = context.ReadContext.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters
				                                            	{StudyStorageKey = studyStorageKey};
				IList<StudyStorageLocation> locationList = procedure.Find(parms);

				foreach (StudyStorageLocation studyLocation in locationList)
				{
					string reason;
					if (CheckFilesystemReadable(studyLocation.FilesystemKey, out reason))
					{
						location = studyLocation;
						return true;
					}
				}

				// TODO: throw new FilesystemIsNotReadableException();
				location = null;
				return false;
			}
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study.  Checks if the filesystem is writable.
		/// </summary>
		/// <param name="location">The output storage location</param>
		/// <param name="partitionKey">The primark key of the ServerPartition table.</param>
		/// <param name="studyInstanceUid">The Study Instance UID of the study</param>
		/// <param name="cache">Should the study location be cached?</param>
		/// <param name="restore">If nearline, should the study be restored?</param>
		/// <returns></returns>
		public void GetWritableStudyStorageLocation(ServerEntityKey partitionKey, string studyInstanceUid, StudyRestore restore, StudyCache cache, out StudyStorageLocation location)
		{
			using (var context = new ServerExecutionContext())
			{
				string reason;

				if (cache == StudyCache.True)
				{
					location = _storageLocationCache.GetCachedStudy(partitionKey, studyInstanceUid);
					if (location != null)
					{
						if (CheckFilesystemWriteable(location.FilesystemKey, out reason))
							return;
					}
				}
				else
					location = null;

				IQueryStudyStorageLocation procedure = context.ReadContext.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters
				                                            	{
				                                            		ServerPartitionKey = partitionKey,
				                                            		StudyInstanceUid = studyInstanceUid
				                                            	};
				IList<StudyStorageLocation> locationList = procedure.Find(parms);

				bool found = false;
				FilesystemNotWritableException x = new FilesystemNotWritableException();

				foreach (StudyStorageLocation studyLocation in locationList)
				{
					if (CheckFilesystemWriteable(studyLocation.FilesystemKey, out reason))
					{
						location = studyLocation;
						if (cache == StudyCache.True)
							_storageLocationCache.AddCachedStudy(location);
						return;
					}
					found = true;
					x.Reason = reason;
					x.Path = studyLocation.StudyFolder;
				}

				if (found)
					throw x;

				CheckForStudyRestore(context.ReadContext, partitionKey, studyInstanceUid, restore);
			}
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study storage key.  Checks if the filesystem is online.
		/// </summary>
		/// <param name="studyStorageKey"></param>
		/// <param name="location"></param>
		/// <returns></returns>
        public bool GetWritableStudyStorageLocation(ServerEntityKey studyStorageKey, out StudyStorageLocation location)
		{
            // NOTE: THIS METHOD SHOULD NOT LOAD THE RECORD FROM THE CACHE

			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				IQueryStudyStorageLocation procedure = context.ReadContext.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters
				                                            	{StudyStorageKey = studyStorageKey};
				IList<StudyStorageLocation> locationList = procedure.Find(parms);

				foreach (StudyStorageLocation studyLocation in locationList)
				{
					string reason;
					if (CheckFilesystemOnline(studyLocation.FilesystemKey, out reason))
					{
						location = studyLocation;
						return true;
					}
				}

				// TODO: throw new FilesystemIsNotWritableException();
				location = null;
				return false;
			}
		}

		/// <summary>
		/// Checks for a storage location for the study in the database, and creates a new location
		/// in the database if it doesn't exist.
		/// </summary>
		/// <param name="partition">The partition where the study is being sent to</param>
		/// <param name="created"></param>
		/// <param name="studyDate"></param>
		/// <param name="studyInstanceUid"></param>
		/// <param name="syntax"></param>
		/// <param name="updateContext">The update context to create the study on</param>
		/// <returns>A <see cref="StudyStorageLocation"/> instance.</returns>
		public StudyStorageLocation GetOrCreateWritableStudyStorageLocation(string studyInstanceUid, string studyDate, TransferSyntax syntax, IUpdateContext updateContext, ServerPartition partition, out bool created)
		{
			created = false;

			StudyStorageLocation location;
			try
			{
				GetWritableStudyStorageLocation(partition.Key, studyInstanceUid, StudyRestore.True,
				                                StudyCache.True, out location);
				return location;
			}
			catch (StudyNotFoundException)
			{
			}

			FilesystemSelector selector = new FilesystemSelector(Instance);
			ServerFilesystemInfo filesystem = selector.SelectFilesystem();
			if (filesystem == null)
			{
				throw new NoWritableFilesystemException();
			}

			IInsertStudyStorage locInsert = updateContext.GetBroker<IInsertStudyStorage>();
			InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters
			                                           	{
			                                           		ServerPartitionKey = partition.GetKey(),
			                                           		StudyInstanceUid = studyInstanceUid,
			                                           		Folder =
			                                           			ResolveStorageFolder(partition.Key, studyInstanceUid, studyDate,
			                                           			                     updateContext, false
			                                           			/* set to false for optimization because we are sure it's not in the system */),
			                                           		FilesystemKey = filesystem.Filesystem.GetKey(),
			                                           		QueueStudyStateEnum = QueueStudyStateEnum.Idle
			                                           	};

			if (syntax.LosslessCompressed)
			{
				insertParms.TransferSyntaxUid = syntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
			}
			else if (syntax.LossyCompressed)
			{
				insertParms.TransferSyntaxUid = syntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
			}
			else
			{
				insertParms.TransferSyntaxUid = syntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.Online;
			}

			location = locInsert.FindOne(insertParms);
			created = true;

			return location;
		}

		/// <summary>
		/// Get an Online and Writeable <see cref="ServiceLock"/> related incoming directory.
		/// </summary>
		/// <param name="partition">The ServerPartion</param>
		/// <param name="folder">The absolute path of the output folder</param>
		/// <returns>true on a found writeable path, false on failure or no writeable path</returns>
		public bool GetWriteableIncomingFolder(ServerPartition partition, out string folder)
		{
			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				IServiceLockEntityBroker broker = context.ReadContext.GetBroker<IServiceLockEntityBroker>();
				ServiceLockSelectCriteria criteria = new ServiceLockSelectCriteria();
				criteria.ServiceLockTypeEnum.EqualTo(ServiceLockTypeEnum.ImportFiles);

				IList<ServiceLock> rows = broker.Find(criteria);
				foreach (ServiceLock serviceLock in rows)
				{
					if (!serviceLock.Enabled)
						continue;

					string reason;
					if (!CheckFilesystemOnline(serviceLock.FilesystemKey, out reason))
						continue;

					if (!CheckFilesystemWriteable(serviceLock.FilesystemKey, out reason))
						continue;

					String incomingFolder = String.Format("{0}_{1}", partition.PartitionFolder, ImportDirectorySuffix);
					folder = serviceLock.Filesystem.GetAbsolutePath(incomingFolder);
					return true;
				}

				folder = string.Empty;
				return false;
			}
		}

        /// <summary>
        /// Returns a value indicating whether the filesystem with the specified key
        /// is writable.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsWritable(ServerEntityKey key)
        {
            ServerFilesystemInfo fs = GetFilesystemInfo(key);
            return fs.Writeable;
        }


		#endregion

		#region Private Methods
		/// <summary>
		/// Method for intializing.
		/// </summary>
		private void Initialize()
		{
			lock (SyncLock)
			{
				LoadFilesystems();
                StringBuilder log = new StringBuilder();
			    log.AppendLine("Filesystem Status:");
                foreach(ServerFilesystemInfo fs in _filesystemList.Values)
                {
                    log.AppendLine(String.Format("\t{0} : {1}", fs.Filesystem.Description, fs.StatusString));
                }
                Platform.Log(LogLevel.Info, log.ToString());

				_fsTimer = new Timer(CheckFilesystems, this, TimeSpan.FromSeconds(Settings.Default.FilesystemCheckDelaySeconds), TimeSpan.FromSeconds(Settings.Default.FilesystemCheckDelaySeconds));

				_dbTimer = new Timer(ReLoadFilesystems, this, TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds), TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds));
			}
		}

		/// <summary>
		/// Timer delegate for reloading filesystem information from the database.
		/// </summary>
		/// <param name="state"></param>
		private void ReLoadFilesystems(object state)
		{
			try
			{
				LoadFilesystems();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when monitoring filesystem.");
			}
		}

		/// <summary>
		/// Load filesystem information from the database.
		/// </summary>
		private void LoadFilesystems()
		{
			bool changed = false;

			lock (SyncLock)
			{
                try
                {
                    List<FilesystemTierEnum> tiers = FilesystemTierEnum.GetAll();

                    // sorted by enum values
                    tiers.Sort((tier1, tier2) => tier1.Enum.CompareTo(tier2.Enum));

                    _tierInfo = new TierInfo();

                    foreach (FilesystemTierEnum tier in tiers)
                    {
                        _tierInfo.Add(tier, new List<ServerFilesystemInfo>());
                    }

                    using (IReadContext read = _store.OpenReadContext())
                    {
                        IFilesystemEntityBroker filesystemSelect = read.GetBroker<IFilesystemEntityBroker>();
                        FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();
                        IList<Filesystem> filesystemList = filesystemSelect.Find(criteria);

                        foreach (Filesystem filesystem in filesystemList)
                        {
                            if (_filesystemList.ContainsKey(filesystem.Key))
                            {
                                if ((filesystem.HighWatermark != _filesystemList[filesystem.Key].Filesystem.HighWatermark)
                                    || (filesystem.LowWatermark != _filesystemList[filesystem.Key].Filesystem.LowWatermark))
                                    Platform.Log(LogLevel.Info, "Watermarks have changed for filesystem {0}, Low: {1}, High: {2}",
                                                 filesystem.Description, filesystem.LowWatermark, filesystem.HighWatermark);
                                _filesystemList[filesystem.Key].Filesystem = filesystem;
                                _tierInfo[filesystem.FilesystemTierEnum].Add(_filesystemList[filesystem.Key]);
                            }
                            else
                            {
                                ServerFilesystemInfo info = new ServerFilesystemInfo(filesystem);
                                _filesystemList.Add(filesystem.Key, info);
                                _tierInfo[filesystem.FilesystemTierEnum].Add(info);
                                info.LoadFreeSpace();
                                changed = true;
                            }
                        }
                    }
                    if (changed && _changedListener != null)
                        EventsHelper.Fire(_changedListener, this, new FilesystemChangedEventArgs(this));
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex,
                                 "Exception has occurred while updating the filesystem list from the datbase. Retry later");
                }
			}

			
		}

		/// <summary>
		/// Timer callback for checking filesystem status.
		/// </summary>
		/// <param name="state"></param>
		private void CheckFilesystems(object state)
		{
			// Load the filesystem objects into a dedicated list, in case the fileysstem list changes
			// while we're doing this.  
			IList<ServerFilesystemInfo> tempList;

			lock (SyncLock)
			{
				tempList = new List<ServerFilesystemInfo>(_filesystemList.Count);

				foreach (ServerFilesystemInfo info in _filesystemList.Values)
				{
					tempList.Add(info);
				}
			}

			foreach (ServerFilesystemInfo info in tempList)
			{
				info.LoadFreeSpace();
			}
		}

		/// <summary>
		/// Returns the name of the directory in the filesytem
		/// where the study with the specified information will be stored.
		/// </summary>
		/// <returns></returns>
		/// 
		private static string ResolveStorageFolder(ServerEntityKey partitionKey, string studyInstanceUid, string studyDate, IPersistenceContext persistenceContext, bool checkExisting)
		{
			string folder;

			if (checkExisting)
			{
				StudyStorage storage = StudyStorage.Load(persistenceContext, partitionKey, studyInstanceUid);
				if (storage != null)
				{
					folder = ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder
								 ? storage.InsertTime.ToString("yyyyMMdd")
								 : String.IsNullOrEmpty(studyDate)
									   ? ImageServerCommonConfiguration.DefaultStudyRootFolder
									   : studyDate;
					return folder;
				}
			}

			folder = ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder
						 ? Platform.Time.ToString("yyyyMMdd")
						 : String.IsNullOrEmpty(studyDate)
							   ? ImageServerCommonConfiguration.DefaultStudyRootFolder
							   : studyDate;
			return folder;
		}

		/// <summary>
		/// Find lower tier filesystems.
		/// </summary>
		/// <param name="filesystem"></param>
		/// <returns></returns>
		private List<FilesystemTierEnum> FindLowerTierFilesystems(ServerFilesystemInfo filesystem)
		{
			List<FilesystemTierEnum> lowerTiers = new List<FilesystemTierEnum>();

			foreach (FilesystemTierEnum tier in _tierInfo.Keys)
			{
				if (tier.Enum > filesystem.Filesystem.FilesystemTierEnum.Enum)
					lowerTiers.Add(tier);
			}

			return lowerTiers;
		}

		/// <summary>
		/// Check if a study is nearline and restore if requested.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="partitionKey"></param>
		/// <param name="studyInstanceUid"></param>
		/// <param name="restore"></param>
		private static void CheckForStudyRestore(IPersistenceContext context, ServerEntityKey partitionKey, string studyInstanceUid, StudyRestore restore)
		{
			IStudyStorageEntityBroker selectBroker = context.GetBroker<IStudyStorageEntityBroker>();
			StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();

			criteria.ServerPartitionKey.EqualTo(partitionKey);
			criteria.StudyInstanceUid.EqualTo(studyInstanceUid);

			StudyStorage storage = selectBroker.FindOne(criteria);
			if (storage != null)
			{
				if (restore == StudyRestore.True)
				{
					RestoreQueue restoreRq = storage.InsertRestoreRequest();
					if (restoreRq != null)
						throw new StudyIsNearlineException(true);
				}

				throw new StudyIsNearlineException(false);
			}

			throw new StudyNotFoundException(studyInstanceUid);
		}

		#endregion

		#region IDisposable Implementation
		public void Dispose()
		{
			if (_fsTimer != null)
			{
				_fsTimer.Dispose();
				_fsTimer = null;
			}
			if (_dbTimer != null)
			{
				_dbTimer.Dispose();
				_dbTimer = null;
			}
		}
		#endregion
	}
}
