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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Utilities.CleanupReconcile
{
    internal class ScanResultEntry
    {
        public string Path { get; set; }
        public DateTime StudyInsertTime { get; set; }
        public string StudyInstanceUid { get; set; }
		public ServerEntityKey ServerPartitionKey { get; set; }
		public bool Skipped { get; set; }
        public bool IsInSIQ { get; set; }
        public bool BackupFilesOnly { get; set; }
        public bool IsEmpty { get; set; }
        public bool StudyWasOnceDeleted { get; set; }
        public bool StudyWasResent { get; set; }
        public bool Undetermined { get; set; }
        public DateTime DirectoryLastWriteTime { get; set; }
        public bool ScanFailed { get; set; }
        public string FailReason { get; set; }
        public bool StudyNoLongerExists { get; set; }

        public bool IsInWorkQueue { get; set; }
        public StudyStorage Storage { get; set; }
        
    }

    internal class ScanResultSet
    {
        public int TotalScanned { get { return Results.Count; } }
        public int SkippedCount { get { return Results.FindAll(item => item.Skipped).Count; } }
        public int InSIQCount { get { return Results.FindAll(item => item.IsInSIQ).Count; } }
        public int EmptyCount { get { return Results.FindAll(item => item.IsEmpty).Count; } }
        public int BackupOrTempOnlyCount { get { return Results.FindAll(item => item.BackupFilesOnly).Count; } }
        public int DeletedStudyCount { get { return Results.FindAll(item => item.StudyWasOnceDeleted).Count; } }
        public int StudyWasResentCount { get { return Results.FindAll(item => item.StudyWasResent).Count; } }
        public int UnidentifiedCount { get { return Results.FindAll(item => item.Undetermined).Count; } }
        public int StudyDoesNotExistCount { get { return Results.FindAll(item => item.StudyNoLongerExists).Count; } }
        public int InWorkQueueCount { get { return Results.FindAll(item => item.IsInWorkQueue).Count; } }
        public int ScanFailedCount { get { return Results.FindAll(item => item.ScanFailed).Count; } }

        public List<ScanResultEntry> Results { get; set; }

        public BackgroundTaskProgress Progress { get; internal set; }


        public ScanResultSet()
        {
            Results = new List<ScanResultEntry>();
        }
    }

    internal class FolderScanner
    {
        public string Path { get; set; }

        private IList<StudyDeleteRecord> _deletedStudies;
        private BackgroundTask _worker;
        private int _foldersCount;
        private List<StudyIntegrityQueue> _siqEntries = new List<StudyIntegrityQueue>();
        
        public ScanResultSet ScanResultSet { get; private set; }

        public event EventHandler ProgressUpdated;
        public event EventHandler Terminated;

        private void LoadSIQEntries()
        {
            using(IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyIntegrityQueueEntityBroker broker = ctx.GetBroker<IStudyIntegrityQueueEntityBroker>();
                _siqEntries = new List<StudyIntegrityQueue>(broker.Find(new StudyIntegrityQueueSelectCriteria()));
            }
           
        }

        private void LoadDeletedStudies()
        {
            using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyDeleteRecordEntityBroker broker = context.GetBroker<IStudyDeleteRecordEntityBroker>();
                _deletedStudies = broker.Find(new StudyDeleteRecordSelectCriteria());
            }
        }
        
        private void StartScaning(IBackgroundTaskContext context)
        {
            FolderScanner owner = context.UserState as FolderScanner;
            DirectoryInfo dir = new DirectoryInfo(owner.Path);
            int dirCount = 0;
            if (dir.Exists)
            {
                context.ReportProgress(new BackgroundTaskProgress(0, "Starting.."));
                
                DirectoryInfo[] subDirs = dir.GetDirectories();
                _foldersCount = subDirs.Length;

            	string path = dir.FullName.TrimEnd(new[] {System.IO.Path.DirectorySeparatorChar});

				string[] dirs = path.Split(new [] { System.IO.Path.DirectorySeparatorChar });

				string partitionFolder = dirs.Length >= 2
											? dirs[dirs.Length - 2]
											: string.Empty;

            	ServerEntityKey partitionKey = GetPartitionKey(partitionFolder);
				if (partitionKey==null)
				{
					context.ReportProgress(new BackgroundTaskProgress(100, "Folder does not match a partition..."));
					return;
				}

                foreach (DirectoryInfo subDir in subDirs)
                {
                    if (context.CancelRequested)
                    {
                        break;
                    }
                  
                    try
                    {
                        var result = ProcessDir(subDir, partitionKey);
                        ScanResultSet.Results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        dirCount++;
                        context.ReportProgress(new BackgroundTaskProgress(dirCount * 100 / _foldersCount, String.Format("Scanning {0}", subDir.FullName)));
                   
                    }
                }


            }

        }

        private ScanResultEntry ProcessDir(DirectoryInfo groupDir, ServerEntityKey partitionKey)
        {

            var result = new ScanResultEntry
                             {
                                 Path = groupDir.FullName,
                                 DirectoryLastWriteTime = groupDir.LastWriteTimeUtc
                             };

            if (groupDir.LastWriteTimeUtc >= Platform.Time.ToUniversalTime() - TimeSpan.FromMinutes(30))
            {
                return new ScanResultEntry { Path = groupDir.FullName, Skipped = true };
            }

            if (CheckInSIQ(groupDir, result))
                return result;

            
            if (CheckIfFolderIsEmpty(groupDir, result))
                return result;

            // check if all files are backup files
            if (ContainsOnlyBackupFiles(groupDir, result))
                return result;

            try
            {
                result.StudyInstanceUid = FindStudyUid(groupDir);
                result.ServerPartitionKey = partitionKey;
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Error, ex);
                result.ScanFailed = true;
                result.FailReason = ex.Message;
                return result;
            }

            if (CheckIfStudyNoLongerExists(result))
            {
                return result;
            }

            Debug.Assert(result.Storage != null);
            if (CheckIfStudyIsInWorkQueue(result))
                return result;
            
            if (CheckIfStudyWasDeletedAfterward(groupDir, result))
            {
                return result;
            }

            if (CheckIfStudyWasInsertedAfter(groupDir, result))
                return result;

            result.Undetermined = true;
            return result;
        }

		private static ServerEntityKey GetPartitionKey(string partitionFolder)
		{

			using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				IServerPartitionEntityBroker broker = ctx.GetBroker<IServerPartitionEntityBroker>();
				ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
				criteria.PartitionFolder.EqualTo(partitionFolder);
				ServerPartition partition = broker.FindOne(criteria);
				if (partition != null)
					return partition.Key;
			}

			return null;
		}

        private static bool CheckIfStudyIsInWorkQueue(ScanResultEntry scanResult)
        {

            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IWorkQueueEntityBroker broker = ctx.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(scanResult.Storage.Key);
                var list = broker.Find(criteria);
                scanResult.IsInWorkQueue = list != null && list.Count > 0;
            } 
            
            return scanResult.IsInWorkQueue;
        }

        private static bool CheckIfFolderIsEmpty(DirectoryInfo dir, ScanResultEntry scanResult)
        {
            DirectoryInfo[] subDirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            if (subDirs.Length == 0 && files.Length == 0)
            {
                scanResult.IsEmpty = true;
                return true;
            }

            if (files.Length > 0)
            {
                scanResult.IsEmpty = false;
                return false;
            }

            foreach(DirectoryInfo subDir in subDirs)
            {
                if (!CheckIfFolderIsEmpty(subDir, scanResult))
                {
                    scanResult.IsEmpty = false;
                    return false;
                }
            }

            scanResult.IsEmpty = true;
            return true;
        }

        private static bool CheckIfStudyWasInsertedAfter(FileSystemInfo dir, ScanResultEntry scanResult)
        {
            scanResult.StudyWasResent = dir.LastWriteTimeUtc < scanResult.StudyInsertTime.ToUniversalTime();

            return scanResult.StudyWasResent;
        }

        private static bool CheckIfStudyNoLongerExists(ScanResultEntry scanResult)
        {
            StudyStorage storage = FindStudyStorage(scanResult);

            scanResult.StudyNoLongerExists = storage == null;
            if (storage != null)
            {
                scanResult.StudyInsertTime = storage.InsertTime;
                scanResult.Storage = storage;
            }

            return scanResult.StudyNoLongerExists;
        }

        private bool CheckInSIQ(FileSystemInfo groupDir, ScanResultEntry scanResult)
        {
            Guid guid = Guid.Empty;
            try
            {
                guid = new Guid(groupDir.Name);
            }
            catch (Exception)
            { }

            scanResult.IsInSIQ = !guid.Equals(Guid.Empty) ? FindSIQByGuid(guid) : FindSIQByGroupID(groupDir.Name);

            return scanResult.IsInSIQ;
        }

		private static StudyStorage FindStudyStorage(ScanResultEntry result)
        {
            using(IReadContext ctx= PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyStorageEntityBroker broker = ctx.GetBroker<IStudyStorageEntityBroker>();
                StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
                criteria.StudyInstanceUid.EqualTo(result.StudyInstanceUid);
            	criteria.ServerPartitionKey.EqualTo(result.ServerPartitionKey);
                return broker.FindOne(criteria);
            }
        }

        private static string FindStudyUid(DirectoryInfo dir)
        {
            FileInfo f = GetFirstFile(dir);

            if (f == null)
                throw new Exception(String.Format("{0}: Cannot find the study instance uid without any dicom files", dir.FullName));

            DicomFile file = new DicomFile(f.FullName);
            file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet | DicomReadOptions.Default);

            return file.DataSet[DicomTags.StudyInstanceUid].ToString();

        }

        private static bool ContainsOnlyBackupFiles(DirectoryInfo dir, ScanResultEntry scanResult)
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string ext = file.Name.ToUpper();
                if (!ext.Contains("BAK") && !ext.Contains("TMP") && !ext.Contains("TEMP"))
                {
                    scanResult.BackupFilesOnly = false;
                    return false;
                }

            }

            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirs)
            {
                if (!ContainsOnlyBackupFiles(subDir, scanResult))
                {
                    scanResult.BackupFilesOnly = false; 
                    return false;
                }
            }

            scanResult.BackupFilesOnly = true; 
            return true;
        }

        private bool CheckIfStudyWasDeletedAfterward(FileSystemInfo dir, ScanResultEntry scanResult)
        {
            scanResult.StudyWasOnceDeleted = (null != CollectionUtils.SelectFirst(_deletedStudies,
                    record => record.StudyInstanceUid == scanResult.StudyInstanceUid 
                    && record.Timestamp.ToUniversalTime() > dir.LastWriteTimeUtc
                ));
            return scanResult.StudyWasOnceDeleted;
        }


        private bool FindSIQByGuid(Guid guid)
        {
            return null != _siqEntries.Find(entry => entry.Key.Key.Equals(guid));
        }

        private static FileInfo GetFirstFile(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles();
            if (files.Length > 0)
                return files[0];

            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirs)
            {
                FileInfo f = GetFirstFile(subDir);
                if (f != null)
                    return f;
            }

            return null;
        }

        private bool FindSIQByGroupID(string group)
        {
            return null != _siqEntries.Find(row => row.GroupID != null && row.GroupID.Equals(group));
        }

        public void StartAsync()
        {
            ScanResultSet = new ScanResultSet();
            LoadDeletedStudies();
            LoadSIQEntries(); 
            
            _worker = new BackgroundTask(StartScaning, true, this);
            _worker.ProgressUpdated += WorkerProgressUpdated;
            _worker.Terminated += WorkerTerminated;
            _worker.Run();

        }

        void WorkerTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            EventsHelper.Fire(Terminated, this, null);
        }

        void WorkerProgressUpdated(object sender, BackgroundTaskProgressEventArgs e)
        {
            ScanResultSet.Progress = e.Progress;
            EventsHelper.Fire(ProgressUpdated, this, null);
        }

        public void Stop()
        {
            if (_worker != null)
                _worker.RequestCancel();
        }
    }
}