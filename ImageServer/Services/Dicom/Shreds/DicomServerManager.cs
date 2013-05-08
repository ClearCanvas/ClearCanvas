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
using System.Net.Sockets;
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
        private readonly List<DicomScp<DicomScpContext>> _alternateAeListenerList = new List<DicomScp<DicomScpContext>>();
        private readonly object _syncLock = new object();
		private static DicomServerManager _instance;
		IList<ServerPartition> _partitions;
        IList<ServerPartitionAlternateAeTitle> _alternateAes;

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
			get { return _instance ?? (_instance = new DicomServerManager("DICOM Service Manager")); }
		    set
			{
				_instance = value;
			}
		}
		#endregion

		#region Private Methods

	    private void StartPartitionListener(ServerPartition part)
	    {
			var parms = new DicomScpContext(part);

	        if (DicomSettings.Default.ListenIPV4)
	        {
	            var ipV4Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify,
	                                                        AssociationAuditLogger.InstancesTransferredAuditLogger)
	                {
	                    ListenPort = part.Port,
	                    AeTitle = part.AeTitle,
	                    ListenAddress = IPAddress.Any
	                };

	            StartScp(ipV4Scp, _listenerList);
	        }

	        if (DicomSettings.Default.ListenIPV6)
	        {
	            var ipV6Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify,
	                                                        AssociationAuditLogger.InstancesTransferredAuditLogger)
	                {
	                    ListenPort = part.Port,
	                    AeTitle = part.AeTitle,
	                    ListenAddress = IPAddress.IPv6Any
	                };
	            StartScp(ipV6Scp, _listenerList);
	        }
	    }
	    private void StartAlternateAeTitleListener(ServerPartition part, ServerPartitionAlternateAeTitle alternateAe)
	    {
	        var context = new DicomScpContext(part)
	            {
	                AlternateAeTitle = alternateAe
	            };

	        if (DicomSettings.Default.ListenIPV4)
	        {
	            var ipV4Scp = new DicomScp<DicomScpContext>(context, AssociationVerifier.Verify,
	                                                        AssociationAuditLogger.InstancesTransferredAuditLogger)
	                {
	                    ListenPort = alternateAe.Port,
	                    AeTitle = alternateAe.AeTitle,
	                    ListenAddress = IPAddress.Any
	                };
	            StartScp(ipV4Scp, _alternateAeListenerList);
			}

	        if (DicomSettings.Default.ListenIPV6)
	        {
	            var ipV6Scp = new DicomScp<DicomScpContext>(context, AssociationVerifier.Verify,
	                                                        AssociationAuditLogger.InstancesTransferredAuditLogger)
	                {
	                    ListenPort = alternateAe.Port,
	                    AeTitle = alternateAe.AeTitle,
	                    ListenAddress = IPAddress.IPv6Any
	                };

	            StartScp(ipV6Scp, _alternateAeListenerList);
	        }
		}

	    private void StartScp(DicomScp<DicomScpContext> listener, List<DicomScp<DicomScpContext>> list)
        {
            if (listener.Start())
            {
                list.Add(listener);
                var helper = new ApplicationActivityAuditHelper(
                                        ServerPlatform.AuditSource,
                                        EventIdentificationContentsEventOutcomeIndicator.Success,
                                        ApplicationActivityType.ApplicationStarted,
                                        new AuditProcessActiveParticipant(listener.AeTitle));
                ServerAuditHelper.LogAuditMessage(helper);
            }
            else
            {
                var helper = new ApplicationActivityAuditHelper(
                    ServerPlatform.AuditSource,
                    EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable,
                    ApplicationActivityType.ApplicationStarted,
                    new AuditProcessActiveParticipant(listener.AeTitle));
                ServerAuditHelper.LogAuditMessage(helper);

                Platform.Log(LogLevel.Error, "Unable to add {1} SCP handler for server partition {0}",
                             listener.Context.Partition.Description,
                             listener.ListenAddress.AddressFamily == AddressFamily.InterNetworkV6
                                 ? "IPv6"
                                 : "IPv4");
                Platform.Log(LogLevel.Error,
                             "Partition {0} will not accept IPv6 incoming DICOM associations.",
                             listener.Context.Partition.Description);

                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, "DICOM Listener",
                                     AlertTypeCodes.UnableToStart, null, TimeSpan.Zero,
                                     "Unable to start {2} DICOM listener on {0} : {1}",
                                     listener.AeTitle, listener.ListenPort,
                                     listener.ListenAddress.AddressFamily == AddressFamily.InterNetworkV6
                                         ? "IPv6"
                                         : "IPv4");
            }
        }

	    private void RemoveDisabledPartitions()
	    {
	        var scpsToDelete = new List<DicomScp<DicomScpContext>>();
	        var alternateAeScpsToDelete = new List<DicomScp<DicomScpContext>>();

	        // First, search for removed ServerPartitions, then remove all related ServerPartitionAlternateAeTitles
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
	                Platform.Log(LogLevel.Info, "Partition was deleted, shutting down listener {0}:{1}", scp.AeTitle,
	                             scp.ListenPort);
	                scp.Stop();
	                scpsToDelete.Add(scp);

	                ServerAuditHelper.LogAuditMessage(new ApplicationActivityAuditHelper(
	                                                      ServerPlatform.AuditSource,
	                                                      EventIdentificationContentsEventOutcomeIndicator.Success,
	                                                      ApplicationActivityType.ApplicationStopped,
	                                                      new AuditProcessActiveParticipant(scp.AeTitle)));

	                // Cleanup any alternate AEs for the partition
	                foreach (DicomScp<DicomScpContext> alternateAeScp in _alternateAeListenerList)
	                {
	                    if (scp.Context.Partition.Key.Equals(alternateAeScp.Context.AlternateAeTitle.ServerPartitionKey))
	                    {
	                        Platform.Log(LogLevel.Info,
	                                     "Partition was deleted, shutting down Alternate AE listener {0}:{1}",
	                                     alternateAeScp.AeTitle, alternateAeScp.ListenPort);

	                        alternateAeScp.Stop();
	                        alternateAeScpsToDelete.Add(alternateAeScp);

	                        ServerAuditHelper.LogAuditMessage(new ApplicationActivityAuditHelper(
	                                                              ServerPlatform.AuditSource,
	                                                              EventIdentificationContentsEventOutcomeIndicator.Success,
	                                                              ApplicationActivityType.ApplicationStopped,
	                                                              new AuditProcessActiveParticipant(alternateAeScp.AeTitle)));
	                    }
	                }
	            }
	        }

	        foreach (DicomScp<DicomScpContext> scp in scpsToDelete)
	            _listenerList.Remove(scp);
	        foreach (DicomScp<DicomScpContext> scp in alternateAeScpsToDelete)
	            _alternateAeListenerList.Remove(scp);	      
	    }

        private void RemoveDisabledAlternateAes()
        {
            var alternateAeScpsToDelete = new List<DicomScp<DicomScpContext>>();

    	    // Now search for any alternate AEs that are disabled/missing
            foreach (DicomScp<DicomScpContext> alternateAeScp in _alternateAeListenerList)
            {
                bool bFound = false;
                foreach (ServerPartitionAlternateAeTitle altAe in _alternateAes)
                {
                    if (altAe.Port == alternateAeScp.ListenPort && altAe.AeTitle.Equals(alternateAeScp.AeTitle) && altAe.Enabled
                        && (altAe.AllowKOPR || altAe.AllowQuery || altAe.AllowRetrieve || altAe.AllowStorage))
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    Platform.Log(LogLevel.Info, "Shutting down Alternate AE listener {0}:{1}", alternateAeScp.AeTitle, alternateAeScp.ListenPort);
                    alternateAeScp.Stop();
                    alternateAeScpsToDelete.Add(alternateAeScp);

                    ServerAuditHelper.LogAuditMessage(new ApplicationActivityAuditHelper(
                                                          ServerPlatform.AuditSource,
                                                          EventIdentificationContentsEventOutcomeIndicator.Success,
                                                          ApplicationActivityType.ApplicationStopped,
                                                          new AuditProcessActiveParticipant(alternateAeScp.AeTitle)));
                }
            }

            foreach (DicomScp<DicomScpContext> scp in alternateAeScpsToDelete)
                _alternateAeListenerList.Remove(scp);
        }

	    private void AddNewPartitions()
	    {
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
	                Platform.Log(LogLevel.Info, "Starting ServerPartition listener {0}:{1}", part.AeTitle,
	                             part.Port);
	                StartPartitionListener(part);
	            }
	        }
	    }

	    private void AddNewAlternateAes()
	    {
	        foreach (ServerPartitionAlternateAeTitle altAe in _alternateAes)
	        {
	            bool bFound = false;
	            foreach (DicomScp<DicomScpContext> scp in _alternateAeListenerList)
	            {
	                if (altAe.Port != scp.ListenPort || !altAe.AeTitle.Equals(scp.AeTitle))
	                    continue;

	                // Reset the context partition, incase its changed.
	                scp.Context.AlternateAeTitle = altAe;

	                bFound = true;
	                break;
	            }

	            if (bFound) continue;

	            if ((!altAe.AllowKOPR && !altAe.AllowStorage && !altAe.AllowQuery && !altAe.AllowRetrieve) ||
	                !altAe.Enabled)
	                continue;

	            ServerPartition theAePartition = null;
	            foreach (var p in _partitions)
	                if (p.Key.Equals(altAe.ServerPartitionKey))
	                    theAePartition = p;

	            if (theAePartition == null || !theAePartition.Enabled)
	                continue;

	            Platform.Log(LogLevel.Info, "Starting Alternate AE Title listener {0}:{1} for Partition {2}",
	                         altAe.AeTitle, altAe.Port, theAePartition.AeTitle);

	            StartAlternateAeTitleListener(theAePartition, altAe);
	        }
	    }

	    private void CheckPartitions()
		{
			lock (_syncLock)
			{
                // Reload the current Partition information
				_partitions = new List<ServerPartition>(ServerPartitionMonitor.Instance.Partitions);
                _alternateAes = new List<ServerPartitionAlternateAeTitle>(ServerPartitionMonitor.Instance.PartitionAeTitles);

			    RemoveDisabledPartitions();

                RemoveDisabledAlternateAes();

			    AddNewPartitions();

			    AddNewAlternateAes();
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

				_partitions = new List<ServerPartition>(ServerPartitionMonitor.Instance.Partitions);
                _alternateAes = new List<ServerPartitionAlternateAeTitle>(ServerPartitionMonitor.Instance.PartitionAeTitles);
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
	        AddNewPartitions();
	        AddNewAlternateAes();
	    }

	    /// <summary>
		/// Method called when stopping the DICOM SCP.
		/// </summary>
		protected override void Stop()
		{
			lock (_syncLock)
			{
                ServerPartitionMonitor.Instance.Changed -= _changedEvent;

				foreach (DicomScp<DicomScpContext> scp in _listenerList)
				{
					scp.Stop();
					var helper = new ApplicationActivityAuditHelper(
								ServerPlatform.AuditSource,
								EventIdentificationContentsEventOutcomeIndicator.Success,
								ApplicationActivityType.ApplicationStopped,
								new AuditProcessActiveParticipant(scp.AeTitle));
                    ServerAuditHelper.LogAuditMessage(helper);
				}

                foreach (DicomScp<DicomScpContext> scp in _alternateAeListenerList)
                {
                    scp.Stop();
                    var helper = new ApplicationActivityAuditHelper(
                                ServerPlatform.AuditSource,
                                EventIdentificationContentsEventOutcomeIndicator.Success,
                                ApplicationActivityType.ApplicationStopped,
                                new AuditProcessActiveParticipant(scp.AeTitle));
                    ServerAuditHelper.LogAuditMessage(helper);
                }		
			}
		}
		#endregion
	}
}