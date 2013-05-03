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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using Timer = System.Threading.Timer;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Event args for partition monitor
	/// </summary>
    public class ServerPartitionChangedEventArgs:EventArgs
    {
    	private readonly ServerPartitionMonitor _monitor;
		public ServerPartitionChangedEventArgs(ServerPartitionMonitor theMonitor)
		{
			_monitor = theMonitor;
		}

    	public ServerPartitionMonitor Monitor
    	{
    		get { return _monitor; }
    	}
    }

	/// <summary>
	/// Singleton class that monitors the currently loaded server partitions.
	/// </summary>
    public class ServerPartitionMonitor :  IEnumerable<ServerPartition>, IDisposable
	{
		#region Private Members
		private readonly object _partitionsLock = new Object();
        private Dictionary<string, ServerPartition> _partitions = new Dictionary<string,ServerPartition>();
        private EventHandler<ServerPartitionChangedEventArgs> _changedListener;
        private readonly Timer _timer;
        private static readonly ServerPartitionMonitor _instance = new ServerPartitionMonitor();
		#endregion

		#region Static Properties
		/// <summary>
		/// Singleton monitor class for the <see cref="ServerPartition"/> table.
		/// </summary>
		static public ServerPartitionMonitor Instance
        {
            get
            {
                return _instance;
            }
		}
		#endregion

		#region Private Constructors
		/// <summary>
        /// ***** internal use only ****
        /// </summary>
        private ServerPartitionMonitor()
        {
            LoadPartitions();

            _timer = new Timer(SynchDB, null, TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds), TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds));
		}
		#endregion

		#region Events
		/// <summary>
		/// Event for notification when partitions change.
		/// </summary>
		public event EventHandler<ServerPartitionChangedEventArgs> Changed
		{
			add { _changedListener += value; }
			remove { _changedListener -= value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Get a partition based on an AE Title.
		/// </summary>
		/// <param name="serverAe"></param>
		/// <returns></returns>
		public ServerPartition GetPartition(string serverAe)
        {
            if (String.IsNullOrEmpty(serverAe))
                return null;

            lock(_partitionsLock)
            {
                if (_partitions.ContainsKey(serverAe))
                    return _partitions[serverAe];
                return null;
            }
        }

        public ServerPartition FindPartition(ServerEntityKey key)
        {
            lock(_partitionsLock)
            {
                return CollectionUtils.SelectFirst(
                           this,
                           partition => partition.GetKey().Equals(key));    
            }
            
        }
		#endregion

		#region Private Methods
		/// <summary>
		/// Internal method for loading partition information fromt he database.
		/// </summary>
		private void LoadPartitions()
        {
            bool changed = false;
            lock (_partitionsLock)
            {
                try
                {
                    var templist = new Dictionary<string, ServerPartition>();
                    IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
                    using (IReadContext ctx = store.OpenReadContext())
                    {
                        var broker = ctx.GetBroker<IServerPartitionEntityBroker>();
                        var criteria = new ServerPartitionSelectCriteria();
                        IList<ServerPartition> list = broker.Find(criteria);
                        foreach (ServerPartition partition in list)
                        {
                            if (IsChanged(partition))
                            {
                                changed = true;
                            }

                            templist.Add(partition.AeTitle, partition);
                        }
                    }

                    _partitions = templist;

                    if (changed && _changedListener != null)
                    {
                        EventsHelper.Fire(_changedListener, this, new ServerPartitionChangedEventArgs(this));
                    }
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex,
                             "Unexpected exception when loading partitions, possible database error.  Operation will be retried later");
                }
            }// lock
        }

		/// <summary>
		/// Timer method for synchronizing with the database.
		/// </summary>
		/// <param name="state"></param>
		private void SynchDB(object state)
		{
			try
			{
			    Platform.Log(LogLevel.Debug, "Updating server partition list from the database");
				LoadPartitions();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e,
				             "Unexpected exception when synchronizing from the database, possible database error.  Operation will be retried later");
			}
		}

		private bool IsChanged(ServerPartition p2)
        {
            if (_partitions.ContainsKey(p2.AeTitle))
            {
                ServerPartition p1 = _partitions[p2.AeTitle];
                if (p1.AcceptAnyDevice != p2.AcceptAnyDevice)
                {
                    return true;
                }
                if (p1.AutoInsertDevice != p2.AutoInsertDevice)
                {
                    return true;
                }

                if (p1.DefaultRemotePort != p2.DefaultRemotePort)
                {
                    return true;
                }

                if (p1.Description != p2.Description)
                {
                    return true;
                }

                if (!p1.DuplicateSopPolicyEnum.Equals(p2.DuplicateSopPolicyEnum))
            		return true;

				if (p1.MatchAccessionNumber != p2.MatchAccessionNumber
					|| p1.MatchIssuerOfPatientId != p2.MatchIssuerOfPatientId
					|| p1.MatchPatientId != p2.MatchPatientId
					|| p1.MatchPatientsBirthDate != p2.MatchPatientsBirthDate
					|| p1.MatchPatientsName != p2.MatchPatientsName
					|| p1.MatchPatientsSex != p2.MatchPatientsSex)
					return true;

                if (p1.Enabled != p2.Enabled)
                {
                    return true;
                }

                if (p1.PartitionFolder != p2.PartitionFolder)
                {
                    return true;
                }

                if (p1.Port != p2.Port)
                {
                    return true;
                }

                if (p1.AuditDeleteStudy != p2.AuditDeleteStudy)
                {
                    return true;
                }

                // nothing has changed
                return false;

            }
		    // this is new partition
		    return true;
        }
		#endregion

        #region IEnumerable<ServerPartition> Members

        public IEnumerator<ServerPartition> GetEnumerator()
        {
            return _partitions.Values.GetEnumerator();
        }

        #endregion


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _partitions.Values.GetEnumerator();
        }

        #endregion

    	public void Dispose()
    	{
    		_timer.Dispose();
    	}
    }
}
