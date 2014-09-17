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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyStorageLocation : ServerEntity, ICloneable, IEquatable<StudyStorageLocation>
    {
        private const string StudyXmlExtension = "xml";
        private const string StudyXmlGzipExtension = "xml.gz";
        
        #region Constructors
        public StudyStorageLocation()
            : base("StudyStorageLocation")
        {
        }

        /// <summary>
        /// Create a copy of the specified <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="original"></param>
        public StudyStorageLocation(StudyStorageLocation original)
            : base("StudyStorageLocation")
        {
            SetKey(original.Key);
            Enabled = original.Enabled;
            FilesystemKey = original.FilesystemKey;
            FilesystemPath = original.FilesystemPath;
            FilesystemStudyStorageKey = original.FilesystemStudyStorageKey;
            FilesystemTierEnum = original.FilesystemTierEnum;
            InsertTime = original.InsertTime;
            IsReconcileRequired = original.IsReconcileRequired;
            LastAccessedTime = original.LastAccessedTime;
            WriteLock = original.WriteLock;
            PartitionFolder = original.PartitionFolder;
            QueueStudyStateEnum = original.QueueStudyStateEnum;
            ReadOnly = original.ReadOnly;
            ServerPartitionKey = original.ServerPartitionKey;
            ServerTransferSyntaxKey = original.ServerTransferSyntaxKey;
            StudyFolder = original.StudyFolder;
            StudyInstanceUid = original.StudyInstanceUid;
            StudyStatusEnum = original.StudyStatusEnum;
            StudyUidFolder = original.StudyUidFolder;
            TransferSyntaxUid = original.TransferSyntaxUid;
            WriteOnly = original.WriteOnly;
        }
        #endregion

        #region Private Members
	    private IList<StudyIntegrityQueue> _integrityQueueItems;

        private ServerPartition _partition;
        private volatile Study _study;
        private StudyXml _studyXml;
        private string _studyUidFolder;
        private string _studyFolderRelativePath;
        private StudyStorage _studyStorage;
        private Patient _patient;
        private IList<ArchiveStudyStorage> _archives;

        #endregion

        #region Public Properties

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "ServerPartitionGUID")]
    	public ServerEntityKey ServerPartitionKey { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemGUID")]
    	public ServerEntityKey FilesystemKey { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "ServerTransferSyntaxGUID")]
    	public ServerEntityKey ServerTransferSyntaxKey { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "StudyInstanceUid")]
    	public string StudyInstanceUid { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "LastAccessedTime")]
    	public DateTime LastAccessedTime { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "InsertTime")]
    	public DateTime InsertTime { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "WriteLock")]
    	public bool WriteLock { get; set; }

		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "ReadLock")]
		public short ReadLock { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "StudyStatusEnum")]
    	public StudyStatusEnum StudyStatusEnum { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemPath")]
    	public string FilesystemPath { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "PartitionFolder")]
    	public string PartitionFolder { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "StudyFolder")]
    	public string StudyFolder { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "Enabled")]
    	public bool Enabled { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "ReadOnly")]
    	public bool ReadOnly { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "WriteOnly")]
    	public bool WriteOnly { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemTierEnum")]
    	public FilesystemTierEnum FilesystemTierEnum { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "TransferSyntaxUid")]
    	public string TransferSyntaxUid { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemStudyStorageGUID")
    	]
    	public ServerEntityKey FilesystemStudyStorageKey { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "QueueStudyStateEnum")]
    	public QueueStudyStateEnum QueueStudyStateEnum { get; set; }

    	[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "IsReconcileRequired")]
    	public bool IsReconcileRequired { get; set; }

        public string StudyUidFolder
        {
            get
            {
            	if (String.IsNullOrEmpty(_studyUidFolder))
                    return StudyInstanceUid;
            	return _studyUidFolder;
            }
        	set { _studyUidFolder = value; }
        }

        /// <summary>
        /// Gets the <see cref="ServerPartition"/> of the study.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get
            {
                lock (SyncRoot)
                {
                    if (_partition == null)
                    {
                        _partition = ServerPartition.Load(ServerPartitionKey);
                    }
                }
                return _partition;
            }
        }

        /// <summary>
        /// Gets the related <see cref="ArchiveStudyStorage"/>.
        /// </summary>
        public IList<ArchiveStudyStorage> ArchiveLocations
        {
            get
            {
                if (_archives == null)
                {
                    lock (SyncRoot)
                    {
                        _archives = GetArchiveLocations(Key);
                    }
                }
                return _archives;
            }
        }

        /// <summary>
        /// Gets the related <see cref="Study"/> entity.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public Study Study
        {
            get
            {
                if (_study==null)
                {
                    using(var context = new ServerExecutionContext())
                    {
                        _study = LoadStudy(context.ReadContext);
                    }
                }
                return _study;
            }
        }

		public string StudyFolderRelativePath
		{
			get
			{
				if (_studyFolderRelativePath != null)
					return _studyFolderRelativePath;
				string path = Path.Combine(PartitionFolder, StudyFolder);
				path = Path.Combine(path, StudyUidFolder);
				return path;
			}
			set { _studyFolderRelativePath = value; }
		}

		/// <summary>
		/// Gets the related <seealso cref="StudyStorage"/> entity.
		/// </summary>
		public StudyStorage StudyStorage
		{
			get
			{
				if (_studyStorage == null)
				{
					lock (SyncRoot)
					{
						using (var context = new ServerExecutionContext())
						{
                            _studyStorage = StudyStorage.Load(context.ReadContext, Key);
						}
					}
				}
				return _studyStorage;
			}
		}

		/// <summary>
		/// Gets the related <seealso cref="Patient"/> entity.
		/// </summary>
		public Patient Patient
		{
			get
			{

				if (_patient == null)
				{
					lock (SyncRoot)
					{
                        using (var context = new ServerExecutionContext())
                        {
							_patient = Patient.Load(context.ReadContext, StudyStorage.GetStudy().PatientKey);
						}
					}
				}
				return _patient;
			}
		}

		/// <summary>
		/// Returns a boolean indicating whether the study has been archived and the latest 
		/// copy in the archive is lossless.
		/// </summary>
		public bool IsLatestArchiveLossless
		{
			get
			{
				if (ArchiveLocations == null || ArchiveLocations.Count == 0)
					return false;
				return ArchiveLocations[0].ServerTransferSyntax.Lossless;
			}
		}

        #endregion

        #region Public Methods
        
        public Study LoadStudy(IPersistenceContext context)
        {
			if (_study == null)
				lock (SyncRoot)
				{
					if (_study == null)
						_study = Study.Find(context, StudyInstanceUid, ServerPartition);
				}
            return _study;
            
        }


		/// <summary>
		/// Returns the path of the folder for the StudyStorageLocation.
		/// </summary>
		/// <returns></returns>
        public string GetStudyPath()
        {
            string path = Path.Combine(FilesystemPath, StudyFolderRelativePath);
            return path;
        }

		/// <summary>
		/// Returns the path of the folder for the specified series.
		/// </summary>
		/// <param name="seriesInstanceUid"></param>
		public String GetSeriesPath(string seriesInstanceUid)
		{
			return Path.Combine(GetStudyPath(), seriesInstanceUid);
		}

		/// <summary>
		/// Returns the path of the sop instance with the specified series and sop instance uid.
		/// </summary>
		/// <param name="seriesInstanceUid"></param>
		/// <param name="sopInstanceUid"></param>
		public String GetSopInstancePath(string seriesInstanceUid, string sopInstanceUid)
		{
			String path = Path.Combine(GetSeriesPath(seriesInstanceUid), sopInstanceUid + ".dcm");
			return path;
		}

		/// <summary>
        /// Acquires a lock on the study for processing
        /// </summary>
        /// <returns>
        /// <b>true</b> if the study is successfully locked.
        /// <b>false</b> if the study cannot be locked or is being locked by another process.
        /// </returns>
        /// <remarks>
        /// This method is non-blocking. Caller must check the return value to ensure the study has been
        /// successfully locked.
        /// </remarks>
        public bool AcquireWriteLock()
        {
            IUpdateContext context =
                PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
            using (context)
            {
                var lockStudyBroker = context.GetBroker<ILockStudy>();
                var parms = new LockStudyParameters {StudyStorageKey = GetKey(), WriteLock = true};
            	if (!lockStudyBroker.Execute(parms))
                    return false;

                context.Commit();
                return parms.Successful;
            }
        }

        /// <summary>
        /// Releases a lock acquired via <see cref="AcquireWriteLock"/>
        /// </summary>
        /// <returns>
        /// <b>true</b> if the study is successfully unlocked.
        /// </returns>
        /// <remarks>
        /// This method is non-blocking. Caller must check the return value to ensure the study has been
        /// successfully unlocked.
        /// </remarks>
        public bool ReleaseWriteLock()
        {
            IUpdateContext context =
                PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
            using (context)
            {
                var lockStudyBroker = context.GetBroker<ILockStudy>();
                var parms = new LockStudyParameters {StudyStorageKey = GetKey(), WriteLock = false};
            	if (!lockStudyBroker.Execute(parms))
                    return false;

                context.Commit();
                return parms.Successful;
            }
        }

        /// <summary>
        /// Acquires a read lock on the study
        /// </summary>
        /// <returns>
        /// <b>true</b> if the study is successfully locked.
        /// <b>false</b> if the study cannot be locked or is being locked by another process.
        /// </returns>
        /// <remarks>
        /// This method is non-blocking. Caller must check the return value to ensure the study has been
        /// successfully locked.
        /// </remarks>
        public bool AcquireReadLock()
        {
            IUpdateContext context =
                PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
            using (context)
            {
                var lockStudyBroker = context.GetBroker<ILockStudy>();
                var parms = new LockStudyParameters { StudyStorageKey = GetKey(), ReadLock = true };
                if (!lockStudyBroker.Execute(parms))
                    return false;

                context.Commit();
                return parms.Successful;
            }
        }

        /// <summary>
        /// Releases a lock acquired via <see cref="AcquireReadLock"/>
        /// </summary>
        /// <returns>
        /// <b>true</b> if the study is successfully unlocked.
        /// </returns>
        /// <remarks>
        /// This method is non-blocking. Caller must check the return value to ensure the study has been
        /// successfully unlocked.
        /// </remarks>
        public bool ReleaseReadLock()
        {
            IUpdateContext context =
                PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
            using (context)
            {
                var lockStudyBroker = context.GetBroker<ILockStudy>();
                var parms = new LockStudyParameters { StudyStorageKey = GetKey(), ReadLock = false };
                if (!lockStudyBroker.Execute(parms))
                    return false;

                context.Commit();
                return parms.Successful;
            }
        }

        /// <summary>
        /// Return snapshot of all related items in the Study Integrity Queue.
        /// </summary>
        /// <returns></returns>
        public IList<StudyIntegrityQueue> GetRelatedStudyIntegrityQueueItems()
        {
            lock (SyncRoot) // make this thread-safe
            {
                if (_integrityQueueItems == null)
                {
                    using (var context = new ServerExecutionContext())
                    {
                        var integrityQueueBroker =
                            context.ReadContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                        var parms = new StudyIntegrityQueueSelectCriteria();

                        parms.StudyStorageKey.EqualTo(GetKey());

                        _integrityQueueItems = integrityQueueBroker.Find(parms);
                    }
                }
            }
            return _integrityQueueItems;
        }

        /// <summary>
        /// Returns a value indicating whether this study can be updated.
        /// It does not necessarily mean a work queue entry can be inserted. Other conditions may require (eg, . </summary>
		/// <param name="reason"></param>
        /// <returns></returns>
        public bool CanUpdate(out string reason)
        {
            reason = null;
            if (StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy) && IsLatestArchiveLossless)
                reason = String.Format("Study {0} cannot be updated because it has been archived as lossless and is currently lossy compressed.", StudyInstanceUid);

            return string.IsNullOrEmpty(reason);
        }

        /// <summary>
        /// Checks if the study can be used for diagnostics at this time.
        /// </summary>
        public bool CanBeUsedForDiagnostics()
        {
            // TODO (CR Phoenix5 - Med): What if it has a failed entry? Will it be unusable until the work item is removed?
            var allowed = QueueStudyStateEnum == QueueStudyStateEnum.Idle;
            if (!allowed)
                Platform.Log(LogLevel.Error, "Study cannot be used for diagnostic purposes at this time because its state is {0}", QueueStudyStateEnum.Description);

            return allowed;
        }

		/// <summary>
		/// Log FilesystemQueue related entries.
		/// </summary>
		public void LogFilesystemQueue()
		{
			var criteria = new FilesystemQueueSelectCriteria();
			criteria.StudyStorageKey.EqualTo(Key);

            using (var context = new ServerExecutionContext())
            {
				var broker = context.ReadContext.GetBroker<IFilesystemQueueEntityBroker>();

				IList<FilesystemQueue> list = broker.Find(criteria);
				foreach (FilesystemQueue queueItem in list)
				{
					if (queueItem.FilesystemQueueTypeEnum.Equals(FilesystemQueueTypeEnum.LosslessCompress) ||
						queueItem.FilesystemQueueTypeEnum.Equals(FilesystemQueueTypeEnum.LossyCompress))
					{
						XmlElement element = queueItem.QueueXml.DocumentElement;

						string syntax = element.Attributes["syntax"].Value;

						TransferSyntax compressSyntax = TransferSyntax.GetTransferSyntax(syntax);
						if (compressSyntax != null)
						{
							Platform.Log(LogLevel.Info, "{0}: Study {1} on partition {2} scheduled for {3} compression at {4}",
										 queueItem.FilesystemQueueTypeEnum.Description, StudyInstanceUid, ServerPartition.AeTitle,
										 compressSyntax.Name, queueItem.ScheduledTime);
						}
						else
							Platform.Log(LogLevel.Info, "{0}: Study {1} on partition {2} scheduled for {3} compression at {4}",
										 queueItem.FilesystemQueueTypeEnum.Description, StudyInstanceUid, ServerPartition.AeTitle,
										 syntax, queueItem.ScheduledTime);
					}
					else
					{
						Platform.Log(LogLevel.Info, "{0}: Study {1} on partition {2} scheduled for {3}",
									 queueItem.FilesystemQueueTypeEnum.Description, StudyInstanceUid, ServerPartition.AeTitle,
									 queueItem.ScheduledTime);
					}
				}
			}
		}

		/// <summary>
		/// Gets the path of the study xml.
		/// </summary>
		/// <returns></returns>
		public String GetStudyXmlPath()
		{
			String path = Path.Combine(GetStudyPath(), StudyInstanceUid);
			path += "." + StudyXmlExtension;

			return path;
		}

		/// <summary>
		/// Gets the path of the compressed study xml.
		/// </summary>
		/// <returns></returns>
		public String GetCompressedStudyXmlPath()
		{
			String path = Path.Combine(GetStudyPath(), StudyInstanceUid);
			path += "." + StudyXmlGzipExtension;

			return path;
		}

		/// <summary>
		/// Gets the <see cref="StudyXml"/> if it exists in this storage location.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <returns></returns>
		public StudyXml LoadStudyXml()
		{
			// TODO: Use FileStreamOpener instead
			// Can't do it until we break the dependency of ImageServer.Common on Model
			if (_studyXml == null)
			{
				lock (SyncRoot)
				{
					try
					{
						Stream xmlStream = Open(GetStudyXmlPath());
						if (xmlStream != null)
						{
							var theMemento = new StudyXmlMemento();
							using (xmlStream)
							{
								StudyXmlIo.Read(theMemento, xmlStream);
								xmlStream.Close();
							}

							_studyXml = new StudyXml();
							_studyXml.SetMemento(theMemento);

						}
					}
					catch (Exception)
					{ }
				}

			}


			return _studyXml;
		}

		public StudyXml LoadStudyXml(bool refresh)
		{
			lock (SyncRoot)
			{
				Stream xmlStream = Open(GetStudyXmlPath());
				if (xmlStream != null)
				{
					var theMemento = new StudyXmlMemento();
					using (xmlStream)
					{
						StudyXmlIo.Read(theMemento, xmlStream);
						xmlStream.Close();
					}

					_studyXml = new StudyXml();
					_studyXml.SetMemento(theMemento);

				}

			}

			return _studyXml;
		}


        #endregion

		#region Helper Methods

		public delegate void SopInstanceProcessCallback(StudyStorageLocation location, SeriesXml seriesXml, InstanceXml instanceXml);

		public void forEachSopInstanceInSeries(string seriesInstanceUid, SopInstanceProcessCallback callback)
		{
			var studyXml = this.LoadStudyXml();
			var seriesXml = studyXml[seriesInstanceUid];
			foreach (var instanceXml in seriesXml)
			{
				callback(this, seriesXml, instanceXml);
			} 
		}

		public int GetNumberOfSeriesRelatedInstancesCount(string seriesInstanceUid)
		{
			var studyXml = this.LoadStudyXml();
			var seriesXml = studyXml[seriesInstanceUid];
			return seriesXml.NumberOfSeriesRelatedInstances;
		}
		
		#endregion

		#region Static Methods

		/// <summary>
        /// Find all <see cref="StudyStorageLocation"/> associcated with the specified <see cref="StudyStorage"/>
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        static public IList<StudyStorageLocation> FindStorageLocations(StudyStorage storage)
        {
            using (var context = new ServerExecutionContext())
            {
                return FindStorageLocations(context.ReadContext, storage, null);
            }
        }

		static public IList<StudyStorageLocation> FindStorageLocations(ServerEntityKey partitionKey, string studyInstanceUid)
		{
            using (var context = new ServerExecutionContext())
            {
				IQueryStudyStorageLocation locQuery = context.ReadContext.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters
				                                               	{
				                                               		StudyInstanceUid = studyInstanceUid,
				                                               		ServerPartitionKey = partitionKey
				                                               	};
				IList<StudyStorageLocation> list = locQuery.Find(locParms);
				return list;
			}
		}

        static public IList<StudyStorageLocation> FindStorageLocations(ServerEntityKey studyStorageKey)
		{
            using (var context = new ServerExecutionContext())
            {
                var parms = new StudyStorageLocationQueryParameters { StudyStorageKey = studyStorageKey };
                var broker = context.ReadContext.GetBroker<IQueryStudyStorageLocation>();
                return broker.Find(parms);
			}
		}        

        static public IList<StudyStorageLocation> FindStorageLocations(
                    IPersistenceContext context, StudyStorage storage)
        {
            return FindStorageLocations(context, storage, null);
        }

        static public IList<StudyStorageLocation> FindStorageLocations(
                   IPersistenceContext context, StudyStorage storage,
                   Predicate<StudyStorageLocation> filter)
        {
            IQueryStudyStorageLocation locQuery = context.GetBroker<IQueryStudyStorageLocation>();
            StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters
                                                           	{
                                                           		StudyInstanceUid = storage.StudyInstanceUid,
                                                           		ServerPartitionKey = storage.ServerPartitionKey
                                                           	};
        	IList<StudyStorageLocation> list = locQuery.Find(locParms);
            if (filter != null)
                CollectionUtils.Remove(list, filter);
            return list;
		}

        /// <summary>
		/// Query for the all archival records for a study, sorted by archive time descendingly (latest first).
		/// </summary>
		/// <param name="studyStorageKey">The primary key of the StudyStorgae table.</param>
		/// <returns>null if not found, else the value.</returns>
        static public IList<ArchiveStudyStorage> GetArchiveLocations(ServerEntityKey studyStorageKey)
        {
            using (var context = new ServerExecutionContext())
            {
                ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
                archiveStudyStorageCriteria.StudyStorageKey.EqualTo(studyStorageKey);
                archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

                IArchiveStudyStorageEntityBroker broker = context.ReadContext.GetBroker<IArchiveStudyStorageEntityBroker>();

                return broker.Find(archiveStudyStorageCriteria);
            }
		}

        // TODO: Replace this method with FileStreamOpener
        private static Stream Open(string path)
        {
            FileStream stream = null;

            for (int i = 0; i<50; i++)
            {
            	try
                {
                    stream = new FileStream(path, FileMode.Open, 
                                            FileAccess.Read, 
                                            FileShare.None /* deny sharing */);
                    break;
                }
                catch (FileNotFoundException)
                {
                    // Maybe it is being swapped?
                }
                catch (DirectoryNotFoundException)
                {
                    // The path is invalid
                    throw;
                }
                catch (PathTooLongException)
                {
                    // The path is too long
                    throw;
                }
                catch (IOException)
                {
                    // other IO exceptions should be treated as retry
                	Random rand = new Random();
                    Thread.Sleep(rand.Next(50, 100));
                }
            }
            return stream;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
        {
            return new StudyStorageLocation(this);
        }

        #endregion

        #region IEquatable<StudyStorageLocation> Members

        public bool Equals(StudyStorageLocation other)
        {
            // TODO: We may need more sophisticated method to determine if 2 locations are the same, especially if mapped drives are used.
            return this.GetStudyPath().Equals(other.GetStudyPath(), StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

    }
}
