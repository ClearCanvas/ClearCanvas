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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Responsible for alerting the user about failed work items.
	/// </summary>
	internal class ActivityMonitorFailureWatcher : IDisposable
	{
		#region ConnectionState classes

		abstract class ConnectionState
		{
			protected ConnectionState(ActivityMonitorFailureWatcher owner)
			{
				Owner = owner;
			}

			public abstract ConnectionState Update();

			protected ActivityMonitorFailureWatcher Owner { get; private set; }
		}

		class ConnectedState : ConnectionState
		{
			public ConnectedState(ActivityMonitorFailureWatcher component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (this.Owner.ActivityMonitor.IsConnected)
					return this;

				// stop listening for workItem changes, so that if the service reconnects, we have a chance
				// to perform an initial query first prior to receiving changes
				this.Owner.ActivityMonitor.WorkItemsChanged -= this.Owner.ActivityMonitorWorkItemsChanged;
				return new DisconnectedState(this.Owner);
			}
		}

		class DisconnectedState : ConnectionState
		{
			public DisconnectedState(ActivityMonitorFailureWatcher component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (!this.Owner.ActivityMonitor.IsConnected)
					return this;

				this.Owner.AlertTotalFailedWorkItems();

				// only start listening to workItem changes after calling AlertTotalFailedWorkItems,
				// so that we can limit the number of alerts we log
				this.Owner.ActivityMonitor.WorkItemsChanged += this.Owner.ActivityMonitorWorkItemsChanged;
				return new ConnectedState(this.Owner);
			}
		}

		#endregion

		private readonly DesktopWindow _window;
		private readonly Action _showActivityMonitor;
		private readonly HashSet<long> _failedWorkItems;
		private ConnectionState _connectionState;

		public ActivityMonitorFailureWatcher(DesktopWindow window, Action showActivityMonitor)
		{
			_window = window;
			_showActivityMonitor = showActivityMonitor;
			_failedWorkItems = new HashSet<long>();
			_connectionState = new DisconnectedState(this);
		}

		public void Initialize()
		{
			this.ActivityMonitor = WorkItemActivityMonitor.Create(true);
			this.ActivityMonitor.IsConnectedChanged += ActivityMonitorIsConnectedChanged;

			_connectionState = _connectionState.Update();
		}

		public void Dispose()
		{
			if (this.ActivityMonitor != null)
			{
				this.ActivityMonitor.WorkItemsChanged -= ActivityMonitorWorkItemsChanged;
				this.ActivityMonitor.IsConnectedChanged -= ActivityMonitorIsConnectedChanged;
				this.ActivityMonitor.Dispose();
				this.ActivityMonitor = null;
			}
		}

		private IWorkItemActivityMonitor ActivityMonitor { get; set; }

		private void ActivityMonitorIsConnectedChanged(object sender, EventArgs eventArgs)
		{
			_connectionState = _connectionState.Update();
		}

		private void ActivityMonitorWorkItemsChanged(object sender, WorkItemsChangedEventArgs e)
		{
			foreach (var item in e.ChangedItems)
			{
				// check for a new failure, and raise an alert if necessary
				if (!_failedWorkItems.Contains(item.Identifier) && item.Status == WorkItemStatusEnum.Failed)
				{
					_failedWorkItems.Add(item.Identifier);

                    if (item.Patient != null && item.Study != null)
                    {
                        var studyDate = DateParser.Parse(item.Study.StudyDate);
                        var message = string.Format(SR.MessageWorkItemFailed,
                                                    item.Request.ActivityTypeString,
                                                    item.Patient.PatientsName,
                                                    studyDate.HasValue ? Format.Date(studyDate.Value) : string.Empty,
                                                    item.Study.AccessionNumber);
                        _window.ShowAlert(AlertLevel.Error, message, SR.LinkOpenActivityMonitor,
                                          window => _showActivityMonitor(), true);
                    }
                    else
                    {
                        var message = string.Format(SR.MessageNonStudyWorkItemFailed,
                                                item.Request.ActivityTypeString);
                        _window.ShowAlert(AlertLevel.Error, message, SR.LinkOpenActivityMonitor,
                                          window => _showActivityMonitor(), true);
                    }
				}

				// if a previously failed item is re-tried, remove it from the set of failed items
				if (_failedWorkItems.Contains(item.Identifier) && item.Status != WorkItemStatusEnum.Failed)
				{
					_failedWorkItems.Remove(item.Identifier);
				}
			}
		}

		private void AlertTotalFailedWorkItems()
		{
			_failedWorkItems.Clear();

			try
			{
				Platform.GetService<IWorkItemService>(
					service =>
					{
						var failedItems = service.Query(new WorkItemQueryRequest { Status = WorkItemStatusEnum.Failed }).Items;
						if (!failedItems.Any())
							return;

						foreach (var item in failedItems)
						{
							_failedWorkItems.Add(item.Identifier);
						}

						var message = _failedWorkItems.Count == 1 ? SR.MessageOneFailedWorkItem
							: string.Format(SR.MessageTotalFailedWorkItems, _failedWorkItems.Count);
						_window.ShowAlert(AlertLevel.Error, message, SR.LinkOpenActivityMonitor, window => _showActivityMonitor(), true);
					});

			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}
	}
}
