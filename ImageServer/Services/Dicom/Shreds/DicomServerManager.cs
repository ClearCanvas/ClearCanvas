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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Dicom.Shreds
{
	/// <summary>
	/// This class manages the DICOM SCP Shred for the ImageServer.
	/// </summary>
	public class DicomServerManager : ThreadedService
	{
		#region Private Members
		private readonly List<DicomScp<DicomScpContext>> _listenerList = new List<DicomScp<DicomScpContext>>();
		private readonly object _syncLock = new object();
		private static DicomServerManager _instance;
		IList<ServerPartition> _partitions;
		private EventHandler<ServerPartitionChangedEventArgs> _changedEvent;
		#endregion

		#region Constructor
		public DicomServerManager(string name) : base(name)
		{}
		#endregion

		#region Properties
		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static DicomServerManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new DicomServerManager("DICOM Service Manager");

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
		#endregion

		#region Private Methods

		private void StartListeners(ServerPartition part)
		{
			DicomScpContext parms =
				new DicomScpContext(part);

			if (DicomSettings.Default.ListenIPV4)
			{
				DicomScp<DicomScpContext> ipV4Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify, AssociationAuditLogger.InstancesTransferredAuditLogger);

				ipV4Scp.ListenPort = part.Port;
				ipV4Scp.AeTitle = part.AeTitle;

				if (ipV4Scp.Start(IPAddress.Any))
				{
					_listenerList.Add(ipV4Scp);
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
											ServerPlatform.AuditSource, 
											EventIdentificationContentsEventOutcomeIndicator.Success, 
											ApplicationActivityType.ApplicationStarted, 
											new AuditProcessActiveParticipant(ipV4Scp.AeTitle));
                    ServerAuditHelper.LogAuditMessage(helper);
				}
				else
				{
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
											ServerPlatform.AuditSource,
											EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable,
											ApplicationActivityType.ApplicationStarted,
											new AuditProcessActiveParticipant(ipV4Scp.AeTitle));
                    ServerAuditHelper.LogAuditMessage(helper);
					Platform.Log(LogLevel.Error, "Unable to add IPv4 SCP handler for server partition {0}",
								 part.Description);
					Platform.Log(LogLevel.Error,
								 "Partition {0} will not accept IPv4 incoming DICOM associations.",
								 part.Description);
					ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, "DICOM Listener",
                                         AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, "Unable to start IPv4 DICOM listener on {0} : {1}",
					                     ipV4Scp.AeTitle, ipV4Scp.ListenPort);
				}
			}

			if (DicomSettings.Default.ListenIPV6)
			{
				DicomScp<DicomScpContext> ipV6Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify, AssociationAuditLogger.InstancesTransferredAuditLogger);

				ipV6Scp.ListenPort = part.Port;
				ipV6Scp.AeTitle = part.AeTitle;

				if (ipV6Scp.Start(IPAddress.IPv6Any))
				{
					_listenerList.Add(ipV6Scp);
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
											ServerPlatform.AuditSource,
											EventIdentificationContentsEventOutcomeIndicator.Success,
											ApplicationActivityType.ApplicationStarted,
											new AuditProcessActiveParticipant(ipV6Scp.AeTitle));
                    ServerAuditHelper.LogAuditMessage(helper);
				}
				else
				{
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
						ServerPlatform.AuditSource,
						EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable,
						ApplicationActivityType.ApplicationStarted,
						new AuditProcessActiveParticipant(ipV6Scp.AeTitle));
                    ServerAuditHelper.LogAuditMessage(helper);

					Platform.Log(LogLevel.Error, "Unable to add IPv6 SCP handler for server partition {0}",
								 part.Description);
					Platform.Log(LogLevel.Error,
								 "Partition {0} will not accept IPv6 incoming DICOM associations.",
								 part.Description);
					ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, "DICOM Listener",
                                         AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, "Unable to start IPv6 DICOM listener on {0} : {1}",
										 ipV6Scp.AeTitle, ipV6Scp.ListenPort);
				}
			}
		}

		private void CheckPartitions()
		{
    	
			lock (_syncLock)
			{
				_partitions = new List<ServerPartition>(ServerPartitionMonitor.Instance);
				IList<DicomScp<DicomScpContext>> scpsToDelete = new List<DicomScp<DicomScpContext>>();

				foreach (DicomScp<DicomScpContext> scp in _listenerList)
				{
					bool bFound = false;
					foreach (ServerPartition part in _partitions)
					{
						if (part.Port == scp.ListenPort && part.AeTitle.Equals(scp.AeTitle) && part.Enabled)
						{
							bFound = true;
							break;
						}
					}

					if (!bFound)
					{
						Platform.Log(LogLevel.Info, "Partition was deleted, shutting down listener {0}:{1}", scp.AeTitle, scp.ListenPort);
						scp.Stop();
						scpsToDelete.Add(scp);
						ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
												ServerPlatform.AuditSource,
												EventIdentificationContentsEventOutcomeIndicator.Success,
												ApplicationActivityType.ApplicationStopped,
												new AuditProcessActiveParticipant(scp.AeTitle));
                        ServerAuditHelper.LogAuditMessage(helper);
					}
				}

				foreach (DicomScp<DicomScpContext> scp in scpsToDelete)
					_listenerList.Remove(scp);

				foreach (ServerPartition part in _partitions)
				{
					if (!part.Enabled)
						continue;

					bool bFound = false;
					foreach (DicomScp<DicomScpContext> scp in _listenerList)
					{
						if (part.Port != scp.ListenPort || !part.AeTitle.Equals(scp.AeTitle))
							continue;

						// Reset the context partition, incase its changed.
						scp.Context.Partition = part;

						bFound = true;
						break;
					}

					if (!bFound)
					{
						Platform.Log(LogLevel.Info, "Detected partition was added, starting listener {0}:{1}", part.AeTitle, part.Port);
						StartListeners(part);
					}
				}
			}
		}
		#endregion

		#region Public Methods
		protected override bool Initialize()
		{
			if (_partitions == null)
			{
				// Force a read context to be opened.  When developing the retry mechanism 
				// for startup when the DB was down, there were problems when the type
				// initializer for enumerated values were failing first.  For some reason,
				// when the database went back online, they would still give exceptions.
				// changed to force the processor to open a dummy DB connect and cause an 
				// exception here, instead of getting to the enumerated value initializer.
				using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
				}

				_changedEvent = delegate
				                	{
				                		CheckPartitions();
				                	};
				ServerPartitionMonitor.Instance.Changed += _changedEvent;

				_partitions = new List<ServerPartition>(ServerPartitionMonitor.Instance);
			}

            return true;
		}

		/// <summary>
		/// Method called when starting the DICOM SCP.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method starts a <see cref="DicomScp{DicomScpParameters}"/> instance for each server partition configured in
		/// the database.  It assumes that the combination of the configured AE Title and Port for the 
		/// partition is unique.  
		/// </para>
		/// </remarks>
		protected override void Run()
		{
			foreach (ServerPartition part in _partitions)
			{
				if (part.Enabled)
				{
					StartListeners(part);
				}
			}
		}

		/// <summary>
		/// Method called when stopping the DICOM SCP.
		/// </summary>
		protected override void Stop()
		{
			lock (_syncLock)
			{
				foreach (DicomScp<DicomScpContext> scp in _listenerList)
				{
					scp.Stop();
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
								ServerPlatform.AuditSource,
								EventIdentificationContentsEventOutcomeIndicator.Success,
								ApplicationActivityType.ApplicationStopped,
								new AuditProcessActiveParticipant(scp.AeTitle));
                    ServerAuditHelper.LogAuditMessage(helper);
	
				}
				ServerPartitionMonitor.Instance.Changed -= _changedEvent;
			}
		}
		#endregion
	}
}