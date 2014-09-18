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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
    /// <summary>
    /// Base class with common routines for processors of <see cref="Model.ServiceLock"/> entries.
    /// </summary>
    public abstract class BaseServiceLockItemProcessor: IDisposable
	{
		#region Private Members
		private bool _cancelPending;
        private readonly object _syncRoot = new object();
		#endregion

		#region Protected Properties
		protected bool CancelPending
		{
			get { lock (_syncRoot) return _cancelPending; }
		}

        #endregion

        #region Contructors
        public BaseServiceLockItemProcessor()
        {
        }
        #endregion

        #region Protected Methods
		/// <summary>
		/// Get a list of candidates from the <see cref="FilesystemQueue"/>.
		/// </summary>
		/// <param name="item">The ServiceLock item.</param>
		/// <param name="scheduledTime">The scheduled time to query against</param>
		/// <param name="type">The type of FilesystemQueue entry.</param>
		/// <param name="statusCheck">If true, check for specific status value WorkQueue entries already existing, otherwise check for any WorkQueue entry.</param>
		/// <returns>The list of queue entries.</returns>
        protected IList<FilesystemQueue> GetFilesystemQueueCandidates(Model.ServiceLock item, DateTime scheduledTime, FilesystemQueueTypeEnum type, bool statusCheck)
        {
			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				IFilesystemQueueEntityBroker broker = context.ReadContext.GetBroker<IFilesystemQueueEntityBroker>();
				FilesystemQueueSelectCriteria fsQueueCriteria = new FilesystemQueueSelectCriteria();

				fsQueueCriteria.FilesystemKey.EqualTo(item.FilesystemKey);
				fsQueueCriteria.ScheduledTime.LessThanOrEqualTo(scheduledTime);
				fsQueueCriteria.FilesystemQueueTypeEnum.EqualTo(type);

				// Do the select based on the QueueStudyState (used to be based on a link to the WorkQueue table)
				StudyStorageSelectCriteria studyStorageSearchCriteria = new StudyStorageSelectCriteria();
				studyStorageSearchCriteria.QueueStudyStateEnum.EqualTo(QueueStudyStateEnum.Idle);
				fsQueueCriteria.StudyStorage.Exists(studyStorageSearchCriteria);

				fsQueueCriteria.ScheduledTime.SortAsc(0);

				IList<FilesystemQueue> list = broker.Find(fsQueueCriteria, 0, ServiceLockSettings.Default.FilesystemQueueResultCount);

				return list;
			}
        }

        /// <summary>
        /// Load the storage location for a Study and partition.
        /// </summary>
        protected static StudyStorageLocation LoadReadableStorageLocation(ServerEntityKey serverPartitionKey, String studyInstanceUid)
        {
            StudyStorageLocation storageLocation;
        	try
        	{
        		FilesystemMonitor.Instance.GetReadableStudyStorageLocation(serverPartitionKey, studyInstanceUid,
        		                                                           StudyRestore.False, StudyCache.False,
        		                                                           out storageLocation);
        	}
        	catch (Exception)
        	{
        		return null;
			}
            return storageLocation;
        }

		/// <summary>
		/// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
		/// </summary>
		/// <param name="location">The location a study is stored.</param>
		/// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
		protected virtual StudyXml LoadStudyXml(StudyStorageLocation location)
		{
			StudyXml theXml = new StudyXml();

			String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");
			if (File.Exists(streamFile))
			{
				using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
				{
					var theMemento = new StudyXmlMemento();

					StudyXmlIo.Read(theMemento, fileStream);

					theXml.SetMemento(theMemento);

					fileStream.Close();
				}
			}

			return theXml;
		}

    	/// <summary>
		/// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
		/// </summary>
		/// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
		/// <param name="context"></param>
		/// <param name="workQueueKey"></param>
		protected static void InsertWorkQueueUidFromStudyXml(StudyXml studyXml, IUpdateContext context, ServerEntityKey workQueueKey)
		{
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					WorkQueueUidUpdateColumns updateColumns = new WorkQueueUidUpdateColumns();
					updateColumns.Duplicate = false;
					updateColumns.Failed = false;
					updateColumns.SeriesInstanceUid = seriesXml.SeriesInstanceUid;
					updateColumns.SopInstanceUid = instanceXml.SopInstanceUid;
					updateColumns.WorkQueueKey = workQueueKey;

					IWorkQueueUidEntityBroker broker = context.GetBroker<IWorkQueueUidEntityBroker>();

					broker.Insert(updateColumns);
				}
			}
		}

		/// <summary>
		/// Estimate the folder size for a study
		/// </summary>
		/// <remarks>
		/// This routine loads the StudyXml file and traverses through each series
		/// for the study.  It then looks at the size of the first image in the series,
		/// and assumes the series size is equal to the first image size times the number
		/// of images within the series.  If the file sizes vary within the series, this
		/// algorithm will fall down a bit.
		/// </remarks>
		/// <param name="location">The StudyStorageLocation object for the study.</param>
		/// <returns></returns>
		protected float EstimateFolderSizeFromStudyXml(StudyStorageLocation location)
		{
			float folderSize = 0.0f;
			string studyFolder = location.GetStudyPath();

			string file = Path.Combine(studyFolder, location.StudyInstanceUid + ".xml");

			var finfo1 = new FileInfo(file);
			if (finfo1.Exists)
				folderSize += finfo1.Length;
	
			file = Path.Combine(studyFolder, location.StudyInstanceUid + ".xml.gz");
			
			var finfo2 = new FileInfo(file);
			if (finfo2.Exists)
				folderSize += finfo2.Length;

			StudyXml study = LoadStudyXml(location);
			foreach (SeriesXml series in study)
			{
				string seriesFolder = Path.Combine(studyFolder, series.SeriesInstanceUid);

				foreach (InstanceXml instance in series)
				{
					if (instance.FileSize != 0)
					{
						folderSize += instance.FileSize;
					}
					else
					{
						file = Path.Combine(seriesFolder, String.Format("{0}.dcm", instance.SopInstanceUid));
						var finfo = new FileInfo(file);
						if (finfo.Exists)
							folderSize += finfo.Length;
					}
				}
			}

			return folderSize;
		}

		protected bool GetStudyStorageLocation(ServerEntityKey partitionKey, string studyInstanceUid, out StudyStorageLocation location)
		{
			using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				var procedure = context.GetBroker<IQueryStudyStorageLocation>();
				var parms = new StudyStorageLocationQueryParameters();
				parms.ServerPartitionKey = partitionKey;
				parms.StudyInstanceUid = studyInstanceUid;
				location = procedure.FindOne(parms);

				return location != null;
			}
		}

		protected DicomFile LoadFileFromList(List<FileInfo> fileList)
		{
			DicomFile file;
			foreach (FileInfo fInfo in fileList)
				try
				{
					file = new DicomFile(fInfo.FullName);
                    file.Load(DicomTags.StudyId, DicomReadOptions.DoNotStorePixelDataInDataSet | DicomReadOptions.Default);
					return file;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Unexpected failure loading file: {0}.  Continuing to next file.",
								 fInfo.FullName);
				}

			return null;
		}

		protected List<FileInfo> LoadSopFiles(DirectoryInfo studyDir, bool cleanup)
		{
			List<string> filesDeleted = new List<string>();
			List<FileInfo> fileList = new List<FileInfo>();
			FileProcessor.Process(studyDir.FullName, "*.*",
			                      delegate(string filePath, out bool cancel)
			                      	{
			                      		cancel = CancelPending;
			                      		if (cancel)
			                      		{
			                      			return;
			                      		}

			                      		FileInfo file = new FileInfo(filePath);

			                      		// if the file is located in a "deleted" directory then skip it
			                      		if (file.DirectoryName != null)
			                      			if (file.DirectoryName.EndsWith("Deleted", StringComparison.InvariantCultureIgnoreCase))
			                      				return;


			                      		if (file.Extension.Equals(ServerPlatform.DicomFileExtension,
			                      		                          StringComparison.InvariantCultureIgnoreCase))
			                      		{
			                      			fileList.Add(file);
			                      		}
			                      		else
			                      		{
			                      			if (file.Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase) ||
			                      			    file.Extension.Equals(".gz", StringComparison.InvariantCultureIgnoreCase))
			                      			{
			                      				// is header file
			                      			}
			                      			else
			                      			{
			                      				// TODO: Should we be smarter when dealing with left-over files?
			                      				// For eg, if we encounter 123.dcm_temp that appears to be
			                      				// a complete version of a corrupted 123.dcm, shouldn't we replace
			                      				// 123.dcm with the 123.dcm_temp instead of deleting 123.dcm_temp?

			                      				// Delete it
			                      				if (cleanup)
			                      				{
			                      					file.Delete();
			                      					filesDeleted.Add(filePath);
			                      				}
			                      			}

			                      		}

			                      	},
			                      true);

			if (filesDeleted.Count > 0)
			{
				// Raise alerts. Each alert lists 10 files that were deleted.
				int count = 0;
				StringBuilder msg = new StringBuilder();
				foreach (string file in filesDeleted)
				{
					count++;
					msg.AppendLine(String.Format("{0};", file));

					if (count % 10 == 0 || count == filesDeleted.Count)
					{
						ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, "Reinventory", 10000, null, TimeSpan.Zero, "Following files were removed:{0}", msg.ToString());
						msg = new StringBuilder();
					}
				}

			}
			return fileList;
		}
        #endregion

		#region Public Methods

        public void Process(Model.ServiceLock item)
        {
            using (ServiceLockProcessorContext executionContext = new ServiceLockProcessorContext(item))
            {
                OnProcess(item);
            }
        }

        public void Cancel()
        {
            lock (_syncRoot)
            {
                _cancelPending = true;
            }

            OnCancelling();
        }

        
        protected virtual void OnCancelling()
        {
            
        }

        #endregion

		#region Static Methods
		/// <summary>
		/// Calculate the folder size based on recursing through all files
		/// within the folder and adding up their sizes.
		/// </summary>
		/// <param name="folder">The path of the folder to calculate the size for.</param>
		/// <returns></returns>
        protected static float CalculateFolderSize(string folder)
        {
            float folderSize = 0.0f;
            try
            {
            	//Checks if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
            	try
            	{
            		foreach (string file in Directory.GetFiles(folder))
            		{
            			var finfo = new FileInfo(file);
						if (finfo.Exists)
            				folderSize += finfo.Length;
            		}

            		foreach (string dir in Directory.GetDirectories(folder))
            			folderSize += CalculateFolderSize(dir);
            	}
            	catch (NotSupportedException e)
            	{
            		Platform.Log(LogLevel.Error, e, "Unable to calculate folder size");
            	}
            }
            catch (UnauthorizedAccessException e)
            {
                Platform.Log(LogLevel.Error, e, "Unable to calculate folder size");
            }
            return folderSize;
        }

        /// <summary>
        /// Set a <see cref="ServiceLock"/> entry to pending.
        /// </summary>
        /// <param name="item">The <see cref="ServiceLock"/> entry to set.</param>
        /// <param name="scheduledTime"></param>
        /// <param name="enabled">Bool telling if the ServiceLock entry should be enabled after unlock.</param>
        protected static void UnlockServiceLock(Model.ServiceLock item, bool enabled, DateTime scheduledTime)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // Update the WorkQueue item status and times.
                IUpdateServiceLock update = updateContext.GetBroker<IUpdateServiceLock>();

                ServiceLockUpdateParameters parms = new ServiceLockUpdateParameters
                                                    	{
                                                    		ServiceLockKey = item.GetKey(),
                                                    		Lock = false,
                                                    		ScheduledTime = scheduledTime,
                                                    		ProcessorId = item.ProcessorId,
                                                    		Enabled = enabled
                                                    	};

            	if (false == update.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unable to update StudyLock GUID Status: {0}",
                                 item.GetKey().ToString());
                }

                updateContext.Commit();
            }
        }
        #endregion

        
		#region Abstract Methods
        protected abstract void OnProcess(Model.ServiceLock item);
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion
    }
}
