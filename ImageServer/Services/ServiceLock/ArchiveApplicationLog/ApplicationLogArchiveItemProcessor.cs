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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.ArchiveApplicationLog
{
	/// <summary>
	/// Processor for archiving the ApplicationLog from the database.
	/// </summary>
	public class ApplicationLogArchiveItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor
	{
		
		private static void UpdateFilesystemKey(Model.ServiceLock item)
		{
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				ServiceLockUpdateColumns columns = new ServiceLockUpdateColumns();
				columns.FilesystemKey = item.FilesystemKey;

				IServiceLockEntityBroker broker = ctx.GetBroker<IServiceLockEntityBroker>();
				broker.Update(item.Key, columns);
				ctx.Commit();
			}
		}

        protected override void OnProcess(Model.ServiceLock item)
		{
			ServiceLockSettings settings = ServiceLockSettings.Default;

			ServerFilesystemInfo archiveFilesystem = null;

			if (item.FilesystemKey != null)
			{
				archiveFilesystem = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);
				if (archiveFilesystem == null)
				{
					Platform.Log(LogLevel.Warn,"Filesystem for archiving logs is no longer valid.  Assigning new filesystem.");
					item.FilesystemKey = null;
					UpdateFilesystemKey(item);
				}
			}

			if (archiveFilesystem == null)
			{
				ServerFilesystemInfo selectedFs = null;
				foreach (ServerFilesystemInfo fs in FilesystemMonitor.Instance.GetFilesystems())
				{
					if (selectedFs == null)
						selectedFs = fs;
					else if (fs.Filesystem.FilesystemTierEnum.Enum > selectedFs.Filesystem.FilesystemTierEnum.Enum)
						selectedFs = fs; // Lower tier
					else if ((fs.Filesystem.FilesystemTierEnum.Enum == selectedFs.Filesystem.FilesystemTierEnum.Enum)
					         &&(fs.FreeBytes > selectedFs.FreeBytes))
						selectedFs = fs; // same tier
				}
				if (selectedFs == null)
				{
					Platform.Log(LogLevel.Info, "No writable filesystems for archiving logs, delaying archival.");
					UnlockServiceLock(item, true, Platform.Time.AddHours(2));
					return;
				}
				item.FilesystemKey = selectedFs.Filesystem.Key;
				archiveFilesystem = selectedFs;
				UpdateFilesystemKey(item);
				Platform.Log(LogLevel.Info, "Selecting Filesystem {0} for archiving of ApplicationLog",
				             selectedFs.Filesystem.Description);
			}
		

			DateTime scheduledTime;
			if (!archiveFilesystem.Writeable)
			{
				Platform.Log(LogLevel.Warn, "Filesystem {0} is not writeable. Unable to archive log files.", archiveFilesystem.Filesystem.Description);
				scheduledTime = Platform.Time.AddMinutes(settings.ApplicationLogRecheckDelay);
			}
			else
			{
				Platform.Log(LogLevel.Info, "Checking for logs to archive to: {0}",
				             archiveFilesystem.Filesystem.Description);
				if (!ArchiveLogs(archiveFilesystem))
					scheduledTime = Platform.Time.AddMinutes(settings.ApplicationLogRecheckDelay);
				else
				{
					DateTime tomorrow = Platform.Time.Date.AddDays(1);
					// Set for 12:01 tomorrow morning
					scheduledTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 1, 0);
					Platform.Log(LogLevel.Info, "Completed archival of logs, rescheduling log archive for {0}",
								 scheduledTime.ToLongTimeString());
				}
			}

			UnlockServiceLock(item, true, scheduledTime);     
		}

		private bool ArchiveLogs(ServerFilesystemInfo archiveFs)
		{
			try
			{
				using (ServerExecutionContext context = new ServerExecutionContext())
				{
					string archivePath = Path.Combine(archiveFs.Filesystem.FilesystemPath, "ApplicationLog");

					ApplicationLogSelectCriteria criteria = new ApplicationLogSelectCriteria();
					criteria.Timestamp.SortAsc(0);
					IApplicationLogEntityBroker broker = context.ReadContext.GetBroker<IApplicationLogEntityBroker>();
					ApplicationLog firstLog = broker.FindOne(criteria);
					if (firstLog == null)
						return true;

					DateTime currentCutOffTime = firstLog.Timestamp.AddMinutes(5);

					int cachedDays = ServiceLockSettings.Default.ApplicationLogCachedDays;
					if (cachedDays < 0) cachedDays = 0;

					DateTime cutOffTime = Platform.Time.Date.AddDays(cachedDays*-1);

					if (currentCutOffTime > cutOffTime)
						return true;

					using (
						ImageServerLogWriter<ApplicationLog> writer =
							new ImageServerLogWriter<ApplicationLog>(archivePath, "ApplicationLog"))
					{
						while (currentCutOffTime < cutOffTime)
						{
							if (!ArchiveTimeRange(writer, currentCutOffTime))
							{
								writer.FlushLog();
								return false;
							}
							currentCutOffTime = currentCutOffTime.AddMinutes(5);
						}

						// Now flush the last potential 5 minutes.
						if (!ArchiveTimeRange(writer, cutOffTime))
						{
							writer.FlushLog();
							return false;
						}

						writer.FlushLog();
					}

					return true;
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when writing log file.");
				return false;
			}
		}

		private static bool ArchiveTimeRange(ImageServerLogWriter<ApplicationLog> writer, DateTime cutOffTime)
		{

			ApplicationLogSelectCriteria criteria = new ApplicationLogSelectCriteria();
			criteria.Timestamp.LessThan(cutOffTime);
			criteria.Timestamp.SortAsc(0);

			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				IApplicationLogEntityBroker broker = context.ReadContext.GetBroker<IApplicationLogEntityBroker>();

				List<ServerEntityKey> keyList = new List<ServerEntityKey>(1000);
				try
				{
					broker.Find(criteria, delegate(ApplicationLog result)
					                      	{
					                      		keyList.Add(result.Key);

					                      		if (writer.WriteLog(result, result.Timestamp))
					                      		{
					                      			// The logs been flushed, delete the log entries cached.
					                      			// Purposely use a read context here, even though we're doing
					                      			// an update, so we don't use transaction wrappers, optimization
					                      			// is more important at this point.
					                      			using (
					                      				IReadContext update =
					                      					PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
					                      			{
					                      				IApplicationLogEntityBroker updateBroker =
					                      					update.GetBroker<IApplicationLogEntityBroker>();
					                      				foreach (ServerEntityKey key in keyList)
					                      					updateBroker.Delete(key);
					                      			}
					                      			keyList = new List<ServerEntityKey>(1000);
					                      		}
					                      	});

					if (keyList.Count > 0)
					{
						// Purposely use a read context here, even though we're doing an update, so we 
						// don't have to do an explicit commit and don't use transaction wrappers.
						using (
							IReadContext update =
								PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
						{
							IApplicationLogEntityBroker updateBroker = update.GetBroker<IApplicationLogEntityBroker>();
							foreach (ServerEntityKey key in keyList)
								updateBroker.Delete(key);
						}
					}

				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when purging log files.");
					return false;
				}

				return true;
			}
		}

	}
}