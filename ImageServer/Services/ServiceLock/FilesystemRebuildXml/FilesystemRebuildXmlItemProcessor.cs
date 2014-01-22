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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Rebuild;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemRebuildXml
{
	/// <summary>
	/// Class for processing 'FilesystemRebuildXml' <see cref="Model.ServiceLock"/> rows.
	/// </summary>
	public class FilesystemRebuildXmlItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
	{
		#region Private Members

		private IList<ServerPartition> _partitions;
		#endregion

		#region Private Methods
		/// <summary>
		/// Traverse the filesystem directories for studies to rebuild the XML for.
		/// </summary>
		/// <param name="filesystem"></param>
		private void TraverseFilesystemStudies(Filesystem filesystem)
		{
			List<StudyStorageLocation> lockFailures = new List<StudyStorageLocation>();
			ServerPartition partition;

			DirectoryInfo filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

			foreach (DirectoryInfo partitionDir in filesystemDir.GetDirectories())
			{
				if (GetServerPartition(partitionDir.Name, out partition) == false)
					continue;

				foreach (DirectoryInfo dateDir in partitionDir.GetDirectories())
				{
					if (dateDir.FullName.EndsWith("Deleted")
						|| dateDir.FullName.EndsWith(ServerPlatform.ReconcileStorageFolder))
						continue;

                    foreach (DirectoryInfo studyDir in dateDir.GetDirectories())
                    {
                        // Check for Cancel message
                        if (CancelPending) return;

                        String studyInstanceUid = studyDir.Name;

                        StudyStorageLocation location;
                        try
                        {
                            FilesystemMonitor.Instance.GetWritableStudyStorageLocation(partition.Key, studyInstanceUid,
                                                                                       StudyRestore.False,
                                                                                       StudyCache.False, out location);
                        }
                        catch (StudyNotFoundException)
                        {
                            List<FileInfo> fileList = LoadSopFiles(studyDir, true);
                            if (fileList.Count == 0)
                            {
                                Platform.Log(LogLevel.Warn, "Found empty study folder: {0}\\{1}", dateDir.Name,
                                             studyDir.Name);
                                continue;
                            }

                            DicomFile file = LoadFileFromList(fileList);
                            if (file == null)
                            {
                                Platform.Log(LogLevel.Warn, "Found directory with no readable files: {0}\\{1}",
                                             dateDir.Name, studyDir.Name);
                                continue;
                            }

                            // Do a second check, using the study instance uid from a file in the directory.
                            // had an issue with trailing periods on uids causing us to not find the 
                            // study storage, and insert a new record into the database.
                            studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
                            if (!studyInstanceUid.Equals(studyDir.Name))
                                try
                                {
                                    FilesystemMonitor.Instance.GetWritableStudyStorageLocation(partition.Key,
                                                                                               studyInstanceUid,
                                                                                               StudyRestore.False,
                                                                                               StudyCache.False,
                                                                                               out location);
                                }
                                catch (Exception e)
                                {
                                    Platform.Log(LogLevel.Warn,
                                                 "Study {0} on filesystem partition {1} not found {2}: {3}",
                                                 studyInstanceUid,
                                                 partition.Description, studyDir.ToString(), e.Message);
                                    continue;
                                }
                            else
                            {
                                Platform.Log(LogLevel.Warn, "Study {0} on filesystem partition {1} not found {2}",
                                             studyInstanceUid,
                                             partition.Description, studyDir.ToString());
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Warn, "Study {0} on filesystem partition {1} not found {2}: {3}",
                                         studyInstanceUid,
                                         partition.Description, studyDir.ToString(), e.Message);
                            continue;
                        }

                        // Location has been loaded, make sure its on the same filesystem
                        if (!location.FilesystemKey.Equals(filesystem.Key))
                        {
                            Platform.Log(LogLevel.Warn,
                                         "Study {0} on filesystem in directory: {1} is stored in different directory in the database: {2}",
                                         studyInstanceUid,
                                         studyDir.ToString(), location.GetStudyPath());
                            try
                            {
                                // Here due to defect #9673, attempting to cleanup errors from this ticket.                                    
                                if (Directory.Exists(location.GetStudyPath()))
                                {
                                    if (File.Exists(location.GetStudyXmlPath()))
                                    {
                                        Platform.Log(LogLevel.Warn,
                                                     "Deleting study {0}'s local directory.  The database location has valid study: {1}",
                                                     studyInstanceUid, studyDir.FullName);
                                        Directory.Delete(studyDir.FullName,true);
                                        continue;
                                    }

                                    Platform.Log(LogLevel.Warn,
                                                 "Deleting study {0} directory stored in database, it does not have a study xml file: {1}",
                                                 studyInstanceUid, location.GetStudyPath());
                                    // Delete the Database's location, and we'll just adjust the database to point to the current directory
                                    Directory.Delete(location.GetStudyPath(),true);
                                }

                                using (
                                    var readContext =
                                        PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(
                                            UpdateContextSyncMode.Flush))
                                {
                                    var update = new FilesystemStudyStorageUpdateColumns
                                                     {
                                                         FilesystemKey = filesystem.Key
                                                     };

                                    var broker = readContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
                                    broker.Update(location.FilesystemStudyStorageKey, update);
                                    readContext.Commit();
                                }

                                Platform.Log(LogLevel.Warn,
                                             "Updated Study {0} FilesystemStudyStorage to point to the current filesystem.",
                                             studyInstanceUid);
                                FilesystemMonitor.Instance.GetWritableStudyStorageLocation(partition.Key,
                                                                                           studyInstanceUid,
                                                                                           StudyRestore.False,
                                                                                           StudyCache.False,
                                                                                           out location);

                            }
                            catch (Exception x)
                            {
                                Platform.Log(LogLevel.Error, x,
                                             "Unexpected error attempting to update storage location for study: {0}",
                                             studyInstanceUid);

                            }
                        }

                        try
                        {
                            if (!location.AcquireWriteLock())
                            {
                                Platform.Log(LogLevel.Warn, "Unable to lock study: {0}, delaying rebuild",
                                             location.StudyInstanceUid);
                                lockFailures.Add(location);
                                continue;
                            }

                            var rebuilder = new StudyXmlRebuilder(location);
                            rebuilder.RebuildXml();

                            location.ReleaseWriteLock();
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, e,
                                         "Unexpected exception when rebuilding study xml for study: {0}",
                                         location.StudyInstanceUid);
                            lockFailures.Add(location);
                        }
                    }


				    // Cleanup the parent date directory, if its empty
					DirectoryUtility.DeleteIfEmpty(dateDir.FullName);
				}
			}

			// Re-do all studies that failed locks one time
			foreach (StudyStorageLocation location in lockFailures)
			{
				try
				{
					if (!location.AcquireWriteLock())
					{
						Platform.Log(LogLevel.Warn, "Unable to lock study: {0}, skipping rebuild", location.StudyInstanceUid);
						continue;
					}

					StudyXmlRebuilder rebuilder = new StudyXmlRebuilder(location);
					rebuilder.RebuildXml();

					location.ReleaseWriteLock();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception on retry when rebuilding study xml for study: {0}",
										 location.StudyInstanceUid);
				}
			}
		}

		/// <summary>
		/// Get the server partition
		/// </summary>
		/// <param name="partitionFolderName"></param>
		/// <param name="partition"></param>
		/// <returns></returns>
		private bool GetServerPartition(string partitionFolderName, out ServerPartition partition)
		{
			foreach (ServerPartition part in _partitions)
			{
				if (part.PartitionFolder == partitionFolderName)
				{
					partition = part;
					return true;
				}
			}

			partition = null;
			return false;
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Process the <see cref="ServiceLock"/> item.
		/// </summary>
		/// <param name="item"></param>
        protected override void OnProcess(Model.ServiceLock item)
		{
			PersistentStoreRegistry.GetDefaultStore();

			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				IServerPartitionEntityBroker broker = context.ReadContext.GetBroker<IServerPartitionEntityBroker>();
				ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
				criteria.AeTitle.SortAsc(0);

				_partitions = broker.Find(criteria);
			}

			ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

			Platform.Log(LogLevel.Info, "Starting rebuilding of Study XML files for filesystem: {0}", info.Filesystem.Description);

			TraverseFilesystemStudies(info.Filesystem);

			item.ScheduledTime = item.ScheduledTime.AddDays(1);

			if (CancelPending)
			{
				Platform.Log(LogLevel.Info,
							 "FilesystemRebuildXml of {0} has been canceled, rescheduling.  Note that the entire Filesystem will be rebuilt again.",
							 info.Filesystem.Description);
				UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
			}
			else
				UnlockServiceLock(item, false, Platform.Time.AddDays(1));

			Platform.Log(LogLevel.Info, "Completed rebuilding of the Study XML files for filesystem: {0}", info.Filesystem.Description);
		}


		#endregion

		public new void Dispose()
		{
			base.Dispose();
		}
	}
}
