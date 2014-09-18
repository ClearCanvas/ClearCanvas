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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Archiving.Shreds
{
	/// <summary>
	/// Manager for handling the ImageServer archiving service.
	/// </summary>
	public class ArchivingShredManager: ThreadedService
	{
		#region PartitionArchiveService Class
		/// <summary>
		/// Class to represent archive service 
		/// </summary>
		protected class PartitionArchiveService
		{
			private readonly IImageServerArchivePlugin _archive;
			private PartitionArchive _partitionArchive;

			public PartitionArchiveService(IImageServerArchivePlugin archive, PartitionArchive partitionArchive, ServerPartition partition)
			{
				_archive = archive;
				_partitionArchive = partitionArchive;
				ServerPartition = partition;
			}

			public PartitionArchive PartitionArchive
			{
				get { return _partitionArchive; }
                set
                {
                    _partitionArchive = value;
                    _archive.PartitionArchive = value;
                }
			}

			public IImageServerArchivePlugin ArchivePlugin
			{
				get { return _archive; }
			}

			public ServerPartition ServerPartition { get; set; }
		}
		#endregion

		#region Private Members
		private static ArchivingShredManager _instance;
		private readonly List<PartitionArchiveService> _archiveServiceList = new List<PartitionArchiveService>();
		private readonly object _syncLock = new object();
		#endregion

		#region Constructors
		/// <summary>
		/// **** For internal use only***
		/// </summary>
		private ArchivingShredManager(string name)
			: base(name)
		{ }
		#endregion

		#region Properties
		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static ArchivingShredManager Instance
		{
			get { return _instance ?? (_instance = new ArchivingShredManager("Archiving")); }
			set
			{
				_instance = value;
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Load the list of <see cref="PartitionArchive"/> entries that are enabled.
		/// </summary>
		/// <returns>The list of <see cref="PartitionArchive"/> instances from the persistant store</returns>
		private static IList<PartitionArchive> LoadEnabledPartitionArchives()
		{
			using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				var broker = readContext.GetBroker<IPartitionArchiveEntityBroker>();

				var criteria = new PartitionArchiveSelectCriteria();

				criteria.Enabled.EqualTo(true);

				return broker.Find(criteria);
			}
		}

		/// <summary>
		/// Load the list of currently configured <see cref="ServerPartition"/> instances.
		/// </summary>
		/// <returns>The partition list.</returns>
		private static IList<ServerPartition> LoadPartitions()
		{
			//Get partitions
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

			using (IReadContext read = store.OpenReadContext())
			{
				var broker = read.GetBroker<IServerPartitionEntityBroker>();
				var criteria = new ServerPartitionSelectCriteria();
				return broker.Find(criteria);
			}
		}

		/// <summary>
		/// Check the currently configured archives and plugins to see if any have been disabled.
		/// </summary>
		private void CheckConfiguredArchives()
		{
			IList<ServerPartition> partitionList = LoadPartitions();

			lock (_syncLock)
			{
				IList<PartitionArchiveService> partitionsToDelete = new List<PartitionArchiveService>();

				foreach (PartitionArchiveService archiveService in _archiveServiceList)
				{
					archiveService.PartitionArchive = PartitionArchive.Load(archiveService.PartitionArchive.GetKey());
					if (!archiveService.PartitionArchive.Enabled)
					{
						Platform.Log(LogLevel.Info, "PartitionArchive {0} has been disabled, stopping plugin.", archiveService.PartitionArchive.Description);
						archiveService.ArchivePlugin.Stop();
						partitionsToDelete.Add(archiveService);
					}
					else
					{
						bool bFound = false;
						foreach (ServerPartition serverPartition in partitionList)
						{
							if (serverPartition.GetKey().Equals(archiveService.ServerPartition.GetKey()) && serverPartition.Enabled)
							{
								bFound = true;
								break;
							}
						}

						if (!bFound)
						{
							Platform.Log(LogLevel.Info, "Partition was deleted or disabled, shutting down archive server {0}",
										 archiveService.ServerPartition.Description);
							archiveService.ArchivePlugin.Stop();
							partitionsToDelete.Add(archiveService);
						}
					}
				}

				// Remove the services from our internal list.
				foreach (PartitionArchiveService archivePlugin in partitionsToDelete)
					_archiveServiceList.Remove(archivePlugin);

				// Load the current extension list
				var ep = new ImageServerArchiveExtensionPoint();
				ExtensionInfo[] extensionInfoList = ep.ListExtensions();


				// Scan the current list of enabled partition archives to see if any
				// new archives have been added
				foreach (PartitionArchive partitionArchive in LoadEnabledPartitionArchives())
				{
					ServerPartition newPartition = ServerPartition.Load(partitionArchive.ServerPartitionKey);

					if (!newPartition.Enabled)
						continue;

					bool bFound = false;
					foreach (PartitionArchiveService service in _archiveServiceList)
					{
						if (!partitionArchive.GetKey().Equals(service.PartitionArchive.GetKey()))
							continue;

						// Reset the context partition, incase its changed.
						service.PartitionArchive = partitionArchive;

						bFound = true;
						break;
					}

					if (!bFound)
					{
						// No match, scan the current extensions for a matching extension
						// to run the service
						foreach (ExtensionInfo extensionInfo in extensionInfoList)
						{
							var archive =
								(IImageServerArchivePlugin)ep.CreateExtension(new ClassNameExtensionFilter(extensionInfo.FormalName));

							if (archive.ArchiveType.Equals(partitionArchive.ArchiveTypeEnum))
							{
								var service = new PartitionArchiveService(archive, partitionArchive, newPartition);
								Platform.Log(LogLevel.Info, "Detected PartitionArchive was added, starting archive {0}", partitionArchive.Description);
								service.ArchivePlugin.Start(partitionArchive);
								_archiveServiceList.Add(service);
								break;
							}
						}
					}
				}
			}
		}
		#endregion

		#region Protected Methods
		protected override bool Initialize()
		{
			_archiveServiceList.Clear();


			// Force a read context to be opened.  When developing the retry mechanism 
			// for startup when the DB was down, there were problems when the type
			// initializer for enumerated values were failng first.  For some reason,
			// when the database went back online, they would still give exceptions.
			// changed to force the processor to open a dummy DB connect and cause an 
			// exception here, instead of getting to the enumerated value initializer.

			IList<PartitionArchive> partitionArchiveList = LoadEnabledPartitionArchives();


			var ep = new ImageServerArchiveExtensionPoint();
			ExtensionInfo[] extensionInfoList = ep.ListExtensions();

			foreach (PartitionArchive partitionArchive in partitionArchiveList)
			{
				ServerPartition partition = ServerPartition.Load(partitionArchive.ServerPartitionKey);


				if (!partition.Enabled)
				{
					Platform.Log(LogLevel.Info, "Server Partition '{0}' is disabled, not starting PartitionArchive '{1}'", partition.Description,
					             partitionArchive.Description);
					continue;
				}

				if (!partitionArchive.Enabled)
				{
					Platform.Log(LogLevel.Info, "PartitionArchive '{0}' is disabled, not starting", partitionArchive.Description);
					continue;					
				}

				foreach (ExtensionInfo extensionInfo in extensionInfoList)
				{
					var archive =
						(IImageServerArchivePlugin) ep.CreateExtension(new ClassNameExtensionFilter(extensionInfo.FormalName));

					if (archive.ArchiveType.Equals(partitionArchive.ArchiveTypeEnum))
					{
						_archiveServiceList.Add(new PartitionArchiveService(archive, partitionArchive, partition));
						break;
					}
				}
			}
            return true;
		}

		protected override void Run()
		{
			foreach (PartitionArchiveService node in _archiveServiceList)
			{
				Platform.Log(LogLevel.Info, "Starting partition archive: {0}", node.PartitionArchive.Description);
				node.ArchivePlugin.Start(node.PartitionArchive);
			}

			while (!CheckStop(60000))
			{
				CheckConfiguredArchives();
			}
		}

		protected override void Stop()
		{
			//TODO CR (Jan 2014): Move this into the base if it applies to all subclasses?
			PersistentStoreRegistry.GetDefaultStore().ShutdownRequested = true;
			lock (_syncLock)
			{
				foreach (PartitionArchiveService node in _archiveServiceList)
				{
					Platform.Log(LogLevel.Info, "Stopping partition archive: {0}", node.PartitionArchive.Description);
					node.ArchivePlugin.Stop();
				}
			}
		}
		#endregion
	}
}