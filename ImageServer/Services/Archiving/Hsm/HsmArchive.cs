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

using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// HSM Based archive plugin.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="HsmArchive"/> class is a plugin to implement an 
	/// Hierarchical Storage Management archive.  Among the HSM style interfaces 
	/// to automated tape libraries are the Sun/StorageTek QFS/SAMFS and Legato DiskXtender.  
	/// </para>
	/// <para>
	/// The <see cref="HsmArchive"/> class takes as input an accessable directory where
	/// the filesystem for the HSM has been mounted.  When storing studies to the HSM
	/// filesystem, a hierarchical folder structure is created.  At the root, a folder
	/// typically based on Study Date is created.  Next, a folder named after Study Instance
	/// UID of the study being archived is created.  Finally, the ZIP file for the study
	/// is placed in this folder.  The zip file has a timestamp as the filename.
	/// </para>
	/// <para>
	/// The zip file created is not in a compressed format.  It assumes the images themselves
	/// are compressed, or the HSM filesystem / underlying tape drives are doing the compression.
	/// </para>
	/// <para>
	/// When a restore of a study occurs, the HsmArchive will do an initial read of the zip 
	/// file.  If the read fails, it will reschedule the read after a configurable time delay,
	/// allowing the HSM system to read the zip file off disk and restore it.
	/// </para>
	/// <para>
	/// The HsmArchive class is basically a shell for the archive.  A configurable number of threads
	/// are created to handle the actual archiving and restoring of data.
	/// </para>
	/// </remarks>
	[ExtensionOf(typeof(ImageServerArchiveExtensionPoint))]
	public class HsmArchive : ImageServerArchiveBase
	{
        private readonly object _syncLock = new object();
		private HsmArchiveService _archiveService;
		private HsmRestoreService _restoreService;
		
		private string _hsmPath;

		public string HsmPath
		{
			get { lock (_syncLock) return _hsmPath; }
            private set { lock (_syncLock) _hsmPath = value; }
		}

		/// <summary>
		/// The <see cref="PartitionArchive"/> associated with the HsmArchive.
		/// </summary>
		public override PartitionArchive PartitionArchive
		{
            get { lock (_syncLock) return _partitionArchive; }
            set
            {
                lock (_syncLock)
                {
                    _partitionArchive = value;

                    //Hsm Archive specific Xml data.
                    XmlElement element = _partitionArchive.ConfigurationXml.DocumentElement;
                    if (element != null)
                        foreach (XmlElement node in element.ChildNodes)
                            if (node.Name.Equals("RootDir"))
                                if (!HsmPath.Equals(node.InnerText))
                                {
                                    HsmPath = node.InnerText;
                                    Platform.Log(LogLevel.Info, "HSM Path has changed for PartitionArchive {0} to {1}",
                                                 _partitionArchive.Description, HsmPath);
                                }
                }
            }
		}

		/// <summary>
		/// The Archive Type.
		/// </summary>
		public override ArchiveTypeEnum ArchiveType
		{
			get { return ArchiveTypeEnum.HsmArchive; }
		}

		public override RestoreQueue GetRestoreCandidate()
		{
			RestoreQueue queueItem;

			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryRestoreQueueParameters parms = new QueryRestoreQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServerPlatform.ProcessorId;
				parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Restoring;
				IQueryRestoreQueue broker = updateContext.GetBroker<IQueryRestoreQueue>();

				// Stored procedure only returns 1 result.
				queueItem = broker.FindOne(parms);

	

				if (queueItem != null)
					updateContext.Commit();
			}
			if (queueItem == null)
			{
				using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					RestoreQueueSelectCriteria criteria = new RestoreQueueSelectCriteria();
					criteria.RestoreQueueStatusEnum.EqualTo(RestoreQueueStatusEnum.Restoring);
					IRestoreQueueEntityBroker restoreQueueBroker = updateContext.GetBroker<IRestoreQueueEntityBroker>();

					if (restoreQueueBroker.Count(criteria) > HsmSettings.Default.MaxSimultaneousRestores)
						return null;

					QueryRestoreQueueParameters parms = new QueryRestoreQueueParameters();

					parms.PartitionArchiveKey = _partitionArchive.GetKey();
					parms.ProcessorId = ServerPlatform.ProcessorId;
					parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Pending;
					IQueryRestoreQueue broker = updateContext.GetBroker<IQueryRestoreQueue>();

					parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Pending;
					queueItem = broker.FindOne(parms);

					if (queueItem != null)
						updateContext.Commit();
				}
			}
			return queueItem;
		}

		/// <summary>
		/// Start the archive.
		/// </summary>
		/// <param name="archive">The <see cref="PartitionArchive"/> to start.</param>
		public override void Start(PartitionArchive archive)
		{
            HsmPath = string.Empty;
            
            PartitionArchive = archive;

			LoadServerPartition();
				
			//Hsm Archive specific Xml data.
			XmlElement element = archive.ConfigurationXml.DocumentElement;
            if (element!=null)
			    foreach (XmlElement node in element.ChildNodes)
				    if (node.Name.Equals("RootDir"))
					    HsmPath = node.InnerText;
			
			// Start the restore service
			_restoreService = new HsmRestoreService("HSM Restore", this);
			_restoreService.StartService();

			// If not "readonly", start the archive service.
			if (!PartitionArchive.ReadOnly)
			{
				_archiveService = new HsmArchiveService("HSM Archive", this);	
				_archiveService.StartService();
			}			
		}

		/// <summary>
		/// Stop the archive.
		/// </summary>
		public override void Stop()
		{
			if (_restoreService != null)
			{
				_restoreService.StopService();
				_restoreService = null;
			}

			if (_archiveService != null)
			{
				_archiveService.StopService();
				_archiveService = null;
			}
		}
	}
}
