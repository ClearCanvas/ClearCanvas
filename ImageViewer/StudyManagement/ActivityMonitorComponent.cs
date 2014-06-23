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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using Action = ClearCanvas.Desktop.Actions.Action;
using Timer = ClearCanvas.Common.Utilities.Timer;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	[ExtensionPoint]
	public sealed class ActivityMonitorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(ActivityMonitorComponentViewExtensionPoint))]
	public class ActivityMonitorComponent : ApplicationComponent
	{
		#region ConnectionState classes

		abstract class ConnectionState
		{
			protected ConnectionState(ActivityMonitorComponent component)
			{
				Component = component;
			}

			public abstract ConnectionState Update();

			protected ActivityMonitorComponent Component { get; private set; }
		}

		class ConnectedState : ConnectionState
		{
			public ConnectedState(ActivityMonitorComponent component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (this.Component.ActivityMonitor.IsConnected)
					return this;

				this.Component.ActivityMonitor.WorkItemsChanged -= this.Component.WorkItemsChanged;
				return new DisconnectedState(this.Component);
			}
		}

		class DisconnectedState : ConnectionState
		{
			public DisconnectedState(ActivityMonitorComponent component)
				: base(component)
			{
			}

			public override ConnectionState Update()
			{
				if (!this.Component.ActivityMonitor.IsConnected)
					return this;

				// whatever is on the screen is out-of-date and should be refreshed
				this.Component.ActivityMonitor.WorkItemsChanged += this.Component.WorkItemsChanged;
				this.Component.RefreshInternal();
				return new ConnectedState(this.Component);
			}
		}

		#endregion

		#region WorkItem class

		internal class WorkItem : IEquatable<WorkItem>
		{
			private readonly WorkItemData _data;

			public WorkItem(WorkItemData data)
			{
				_data = data;
			}

			public WorkItemData Data
			{
				get { return _data; }
			}

			public long Id
			{
				get { return _data.Identifier; }
			}

			/// <summary>
			/// In order to make this behave well in the table view, need to defined equality by Id property.
			/// </summary>
			/// <param name="that"></param>
			/// <returns></returns>
			public bool Equals(WorkItem that)
			{
				return that != null && this.Id == that.Id;
			}

			/// <summary>
			/// In order to make this behave well in the table view, need to defined equality by Id property.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public override bool Equals(object obj)
			{
				return Equals(obj as WorkItem);
			}

			/// <summary>
			/// In order to make this behave well in the table view, need to defined equality by Id property.
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				return this.Id.GetHashCode();
			}

			public string PatientId
			{
				get { return _data.Patient != null ? _data.Patient.PatientId : null; }
			}

			public string PatientsName
			{
				get { return _data.Patient != null ? _data.Patient.PatientsName : null; }
			}

			public string PatientsBirthDate
			{
				get { return _data.Patient != null ? _data.Patient.PatientsBirthDate : null; }
			}

			public string PatientsSex
			{
				get { return _data.Patient != null ? _data.Patient.PatientsSex : null; }
			}

			public bool CancellationCanResultInPartialStudy
			{
				get { return _data.Request.CancellationCanResultInPartialStudy; }
			}

			public string PatientInfo
			{
				get
				{
					var p = _data.Patient;
					if (p == null)
						return null;
					return string.Format("{0} \u00B7 {1}", new PersonName(p.PatientsName).FormattedName, p.PatientId);
				}
			}

			public string StudyDate
			{
				get { return _data.Study != null ? _data.Study.StudyDate : null; }
			}

			public string StudyTime
			{
				get { return _data.Study != null ? _data.Study.StudyTime : null; }
			}

			public string AccessionNumber
			{
				get { return _data.Study != null ? _data.Study.AccessionNumber : null; }
			}

			public string StudyDescription
			{
				get { return _data.Study != null ? _data.Study.StudyDescription : null; }
			}

			public string StudyInfo
			{
				get
				{
					if (_data.Study == null) return string.Empty;

					// TODO (Marmot) Code taken from DicomImageSetDescriptor.GetName()  Should be consolidated?  TBD
					DateTime studyDate;
					DateParser.Parse(StudyDate, out studyDate);
					DateTime studyTime;
					TimeParser.Parse(StudyTime, out studyTime);

					string modalitiesInStudy = null;
					if (_data.Study != null && _data.Study.ModalitiesInStudy != null)
						modalitiesInStudy = StringUtilities.Combine(_data.Study.ModalitiesInStudy, ", ");

					var nameBuilder = new StringBuilder();
					nameBuilder.AppendFormat("{0} {1}", studyDate.ToString(Format.DateFormat),
														studyTime.ToString(Format.TimeFormat));

					if (!String.IsNullOrEmpty(AccessionNumber))
						nameBuilder.AppendFormat(", A#: {0}", AccessionNumber);

					nameBuilder.AppendFormat(", [{0}] {1}", modalitiesInStudy ?? string.Empty, StudyDescription);

					return nameBuilder.ToString();
				}
			}

			public WorkItemPriorityEnum Priority
			{
				get { return _data.Priority; }
			}

			public static Comparison<WorkItem> PriorityComparison
			{
				get
				{
					var list = (IList<WorkItemPriorityEnum>)_priorityOrdering;
					return (x, y) => list.IndexOf(x.Priority).CompareTo(list.IndexOf(y.Priority));
				}
			}

			public WorkItemStatusEnum Status
			{
				get { return _data.Status; }
			}

			public static Comparison<WorkItem> StatusComparison
			{
				get
				{
					var list = (IList<WorkItemStatusEnum>)_statusOrdering;
					return (x, y) => list.IndexOf(x.Status).CompareTo(list.IndexOf(y.Status));
				}
			}

			public string ActivityType
			{
				get { return _data.Request != null ? _data.Request.ActivityTypeString : ReindexRequest.WorkItemTypeString; }
			}

			public string ActivityDescription
			{
				get { return _data.Request != null ? _data.Request.ActivityDescription : null; }
			}

			public DateTime ScheduledTime
			{
				get { return _data.ScheduledTime; }
			}

			public DateTime RequestedTime
			{
				get { return _data.RequestedTime; }
			}

			public string ProgressStatus
			{
				get
				{
					if (_data.Progress == null) return null;

				    string status = !string.IsNullOrEmpty(ProgressStatusDescription) 
				                        ? string.Format("{0} [{1}]", _data.Progress.Status, ProgressStatusDescription) 
				                        : _data.Progress.Status;

                    if (!string.IsNullOrEmpty(_data.RetryStatus))
                        status = string.Format("{0}, {1}", _data.RetryStatus, status);
				    return status;
				}
			}

			public string ProgressStatusDescription
			{
				get { return _data.Progress != null ? _data.Progress.StatusDetails : null; }
			}

			public decimal ProgressValue
			{
				get { return _data.Progress != null ? _data.Progress.PercentComplete : 0; }
			}

			public static Comparison<WorkItem> ProgressComparison
			{
				get { return (x, y) => x.ProgressValue.CompareTo(y.ProgressValue); }
			}

			public bool IsCancelable
			{
				get { return _data.Progress == null || _data.Progress.IsCancelable; }
			}

			public IconSet ProgressIcon
			{
				get { return GetProgressIcon(_data.Progress, _data.Status); }
			}

			public bool ContainsText(string text)
			{
				return ContainsText(text,
									w => w.PatientInfo,
									w => w.StudyInfo,
									w => w.ActivityDescription,
									w => w.ProgressStatus,
									w => w.ProgressStatusDescription,
									w => w.ActivityType,
									w => w.Priority.GetDescription(),
									w => w.Status.GetDescription());
			}

			private bool ContainsText(string text, params Func<WorkItem, string>[] fields)
			{
				text = text.ToLower();
				return fields.Any(f =>
									{
										var value = f(this);
										return !String.IsNullOrEmpty(value) && value.ToLower().Contains(text);
									});
			}
		}

		#endregion

		#region WorkItemUpdateManager class

		internal class WorkItemUpdateManager
		{
			private readonly ItemCollection<WorkItem> _items;
			private readonly Dictionary<long, WorkItem> _failures;
			private readonly Predicate<WorkItem> _filter;
			private readonly System.Action _failedItemCountChanged;

			public WorkItemUpdateManager(ItemCollection<WorkItem> itemCollection, Predicate<WorkItem> filter, System.Action failedItemCountChanged)
			{
				_items = itemCollection;
				_filter = filter;
				_failedItemCountChanged = failedItemCountChanged;
				_failures = new Dictionary<long, WorkItem>();
			}

			public int FailedItemCount
			{
				get { return _failures.Values.Count(item => item.Status == WorkItemStatusEnum.Failed); }
			}

			public void Clear()
			{
				_items.Clear();
				if (_failures.Count == 0)
					return;

				_failures.Clear();
				_failedItemCountChanged();
			}

			public bool Update(IList<WorkItem> items, Comparison<WorkItem> sortComparison)
			{
				// if we're dealing with a batch (e.g. via Refresh), we need to do things differently
				// than for a few items
				var isBatch = items.Count > 10;
				var batchAddList = new List<WorkItem>();

				foreach (var item in items)
				{
					var index = _items.FindIndex(w => w.Id == item.Id);
					if (index > -1)
					{
						// the item is currently in the list
						// if the item is marked deleted, or if it no longer meets the filter criteria, remove it
						// otherwise update it
						if (item.Status == WorkItemStatusEnum.Deleted || item.Status == WorkItemStatusEnum.DeleteInProgress || !Include(item))
							_items.RemoveAt(index);
						else
							UpdateItem(item, index, sortComparison, isBatch);
					}
					else
					{
						// the item is not currently in the list
						// if not deleted and it meets the filter criteria, add it
						if (item.Status != WorkItemStatusEnum.Deleted && item.Status != WorkItemStatusEnum.DeleteInProgress && Include(item))
						{
							if(isBatch)
								batchAddList.Add(item);
							else
								AddItem(item, sortComparison);
						}
					}

					var failureCount = _failures.Count;

					if (item.Status != WorkItemStatusEnum.Failed) //remove anything that's not failed, which includes restarted items.
						_failures.Remove(item.Id);
					else
						_failures[item.Id] = item;

					if (_failures.Count != failureCount)
						_failedItemCountChanged();
				}
				if(isBatch && batchAddList.Count > 0)
				{
					_items.AddRange(batchAddList);
				}

				// return a value indicating whether a sort is required
				return isBatch;
			}

			private void AddItem(WorkItem item, Comparison<WorkItem> sortComparison)
			{
				var i = _items.FindInsertionPoint(item, sortComparison);
				_items.Insert(i, item);
			}

			private void UpdateItem(WorkItem item, int index, Comparison<WorkItem> sortComparison, bool isBatch)
			{
				var oldItem = _items[index];

				// if update does not affect sort order, then we can update the item in-place
				// Also, if it's part of a batch, then just update it because it will be sorted later
				if (isBatch || sortComparison(oldItem, item) == 0)
				{
					_items[index] = item;
				}
				else
				{
					// otherwise we need to remove and re-insert it
					_items.RemoveAt(index);
					var i = _items.FindInsertionPoint(item, sortComparison);
					_items.Insert(i, item);
				}
			}

			private bool Include(WorkItem item)
			{
				return _filter(item);
			}
		}

		#endregion

		#region StudyCountWatcher class

		class StudyCountWatcher : IDisposable
		{
			private int _studyCount = -1;
			private readonly Timer _throttleTimer;
			private readonly System.Action _onChanged;

			public StudyCountWatcher(System.Action onChanged)
			{
				_throttleTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(5));
				_onChanged = onChanged;
			}

			public void Invalidate()
			{
				_throttleTimer.Start();
			}

			public void Dispose()
			{
				_throttleTimer.Dispose();
			}

			public int StudyCount
			{
				get
				{
					if (_studyCount == -1)
					{
						try
						{
							Platform.GetService<IStudyStoreQuery>(s => _studyCount = s.GetStudyCount(new GetStudyCountRequest()).StudyCount);
						}
						catch (Exception e)
						{
							//TODO (Marmot): Show something to the user?
							Platform.Log(LogLevel.Error, e, "Error getting the count of studies in the local store.");
						}
					}
					return _studyCount;
				}
			}

			private void OnTimerElapsed(object state)
			{
				_throttleTimer.Stop();
				_studyCount = -1;	// invalidate
				_onChanged();
			}
		}

		#endregion

		#region WorkItemActionModel

		class WorkItemActionModel : SimpleActionModel
		{
			private readonly ActivityMonitorComponent _owner;
			private readonly IItemCollection<WorkItem> _workItems;
			private IList<long> _selectedWorkItemIDs;

			public WorkItemActionModel(IItemCollection<WorkItem> workItems, ActivityMonitorComponent owner)
				: base(new ApplicationThemeResourceResolver(typeof(ActivityMonitorComponent).Assembly, new ApplicationThemeResourceResolver(typeof(CrudActionModel).Assembly)))
			{
				_workItems = workItems;
				_owner = owner;
                if (PermissionsHelper.IsInRole(AuthorityTokens.ActivityMonitor.WorkItems.Stop))
				    this.CancelAction = AddAction("cancel", SR.MenuStopWorkItem, "CancelToolSmall.png", SR.TooltipStopWorkItem, CancelSelectedWorkItems);

                if (PermissionsHelper.IsInRole(AuthorityTokens.ActivityMonitor.WorkItems.Restart))
				    this.RestartAction = AddAction("restart", SR.MenuRestartWorkItem, "RestartToolSmall.png", SR.TooltipRestartWorkItem, RestartSelectedWorkItems);

                if (PermissionsHelper.IsInRole(AuthorityTokens.ActivityMonitor.WorkItems.Delete))
                    this.DeleteAction = AddAction("delete", SR.MenuDeleteWorkItem, "DeleteToolSmall.png", SR.TooltipDeleteWorkItem, DeleteSelectedWorkItems);
                
                if (PermissionsHelper.IsInRole(AuthorityTokens.ActivityMonitor.WorkItems.Prioritize))
                    this.StatAction = AddAction("stat", SR.MenuStatWorkItem, "StatToolSmall.png", SR.TooltipStatWorkItem, StatSelectedWorkItems);

				UpdateActionEnablement();
			}

			public IList<long> SelectedWorkItemIDs
			{
				get { return _selectedWorkItemIDs ?? (_selectedWorkItemIDs = new List<long>()); }
				set
				{
					_selectedWorkItemIDs = value;
					UpdateActionEnablement();
				}
			}

			private IEnumerable<WorkItem> SelectedWorkItems
			{
				get { return _workItems.Where(w => SelectedWorkItemIDs.Contains(w.Id)); }
			}

			public void OnWorkItemsChanged(IEnumerable<WorkItem> items)
			{
				if (items.Any(item => SelectedWorkItemIDs.Contains(item.Id)))
					UpdateActionEnablement();
			}

			private void UpdateActionEnablement()
			{
				var items = this.SelectedWorkItems.ToList();
				var nonEmpty = items.Count > 0;

				if (DeleteAction != null)
					DeleteAction.Enabled = nonEmpty && items.All(IsDeletable);

				if (CancelAction != null)
					CancelAction.Enabled = nonEmpty && items.All(IsCancelable);

				if (RestartAction != null)
					RestartAction.Enabled = nonEmpty && items.All(IsRestartable);

				if (StatAction != null)
					StatAction.Enabled = nonEmpty && items.All(IsStatable);
			}

			private bool IsDeletable(WorkItem w)
			{
				return w.Status == WorkItemStatusEnum.Complete
					   || (w.Status == WorkItemStatusEnum.Pending && w.IsCancelable)
					   || w.Status == WorkItemStatusEnum.Failed
					   || w.Status == WorkItemStatusEnum.Canceled;
			}
			private bool IsCancelable(WorkItem w)
			{
				return (w.Status == WorkItemStatusEnum.InProgress
						 || w.Status == WorkItemStatusEnum.Idle
						 || w.Status == WorkItemStatusEnum.Pending) && w.IsCancelable;
			}
			private bool IsRestartable(WorkItem w)
			{
				// Cannot restart Delete Study Requests
				return (w.Status == WorkItemStatusEnum.Canceled
						 || w.Status == WorkItemStatusEnum.Failed)
						 && !w.Data.Request.WorkItemType.Equals(DeleteStudyRequest.WorkItemTypeString);
			}
			private bool IsStatable(WorkItem w)
			{
				// if the item is already stat, can't change it
				if (w.Priority == WorkItemPriorityEnum.Stat)
					return false;

				// item must be in Pending or Idle status
				return w.Status == WorkItemStatusEnum.Pending
					   || w.Status == WorkItemStatusEnum.Idle;
			}

			private void RestartSelectedWorkItems()
			{
				try
				{
					var client = new WorkItemBridge();
					ProcessItems(SelectedWorkItems, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Reset();
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void CancelSelectedWorkItems()
			{
				var items = this.SelectedWorkItems.ToList();
				if (items.Any(item => item.CancellationCanResultInPartialStudy))
				{
					var action = _owner.Host.ShowMessageBox(SR.MessageConfirmCancelWorkItems, MessageBoxActions.YesNo);
					if (action == DialogBoxAction.No)
						return;
				}

				try
				{
					var client = new WorkItemBridge();
					ProcessItems(items, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Cancel();
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void DeleteSelectedWorkItems()
			{
				try
				{
					var client = new WorkItemBridge();
					ProcessItems(SelectedWorkItems, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Delete();
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void StatSelectedWorkItems()
			{
				try
				{
					var client = new WorkItemBridge();
					ProcessItems(SelectedWorkItems, workItem =>
					{
						client.WorkItem = workItem.Data;
						client.Reprioritize(WorkItemPriorityEnum.Stat);
					}, false);
				}
				catch (EndpointNotFoundException)
				{
					HandleEndpointNotFound();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _owner.Host.DesktopWindow);
				}
			}

			private void ProcessItems<T>(IEnumerable<T> items, Action<T> processAction, bool cancelable)
			{
				var itemsToProcess = items.ToList();

				// if just one item, process it synchronously
				if (itemsToProcess.Count == 1)
				{
					processAction(itemsToProcess[0]);
					return;
				}

				// otherwise do progress dialog
				ProgressDialog.Show(_owner.Host.DesktopWindow,
					itemsToProcess,
					(item, i) =>
					{
						processAction(item);
						return string.Format(SR.MessageProcessedItemsProgress, i + 1, itemsToProcess.Count);
					},
					cancelable);
			}

			private void HandleEndpointNotFound()
			{
				_owner.Host.ShowMessageBox(SR.MessageLocalServerNotRunning, MessageBoxActions.Ok);
			}

			private Action DeleteAction { get; set; }
			private Action CancelAction { get; set; }
			private Action RestartAction { get; set; }
			private Action StatAction { get; set; }
		}

		#endregion

		enum StatusFilterValue
		{
			Active,
			Complete,
			Stopped,
			Failed
		}

		/// <summary>
		/// Defines the sort order for Status column, based on "completedness", which may differ
		/// from the order in which the enum constants are defined.
		/// </summary>
		private static readonly WorkItemStatusEnum[] _statusOrdering = new[] {
			WorkItemStatusEnum.Pending,
			WorkItemStatusEnum.InProgress,
			WorkItemStatusEnum.Idle,
			WorkItemStatusEnum.Complete,
			WorkItemStatusEnum.Canceling,
			WorkItemStatusEnum.Canceled,
			WorkItemStatusEnum.Failed,
			WorkItemStatusEnum.DeleteInProgress,
			WorkItemStatusEnum.Deleted,
		};

		/// <summary>
		/// Defines the sort order for Priority column, which may differ
		/// from the order in which the enum constants are defined.
		/// </summary>
		private static readonly WorkItemPriorityEnum[] _priorityOrdering = new[] {
			WorkItemPriorityEnum.Normal,
			WorkItemPriorityEnum.High,
			WorkItemPriorityEnum.Stat,
		};

		private static readonly object NoFilter = new object();

		private readonly WorkItemActionModel _workItemActionModel;
		private readonly Table<WorkItem> _workItems = new Table<WorkItem>();
		private readonly WorkItemUpdateManager _workItemManager;

		private ConnectionState _connectionState;
		private StatusFilterValue? _statusFilter;
		private string _activityFilter;

		private string _textFilter;
		private readonly Timer _textFilterTimer;

		private readonly LocalServerWatcher _localServerWatcher;
		private readonly StudyCountWatcher _studyCountWatcher;

		public ActivityMonitorComponent()
		{
			_connectionState = new DisconnectedState(this);
			_localServerWatcher = LocalServerWatcher.Instance;

			_textFilterTimer = new Timer(OnTextFilterTimerElapsed, null, 1000);
			_studyCountWatcher = new StudyCountWatcher(OnStudyCountChanged);
			_workItemManager = new WorkItemUpdateManager(_workItems.Items, Include, OnFailureCountChanged);
			_workItemActionModel = new WorkItemActionModel(_workItems.Items, this);
		}

		public override void Start()
		{
			base.Start();

			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPatient, w => w.PatientInfo) { WidthFactor = .7f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStudy, w => w.StudyInfo) { WidthFactor = .9f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnActivityDescription, w => w.ActivityDescription) { WidthFactor = .7f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatus, w => w.Status.GetDescription())
									   {
										   WidthFactor = .3f,
										   TooltipTextProvider = w => string.IsNullOrEmpty(w.ProgressStatusDescription)
																		? string.Empty
																		: w.ProgressStatusDescription,
										   Comparison = WorkItem.StatusComparison
									   });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnStatusDescription, w => w.ProgressStatus) { WidthFactor = 1.5f });
			var requestedTimeColumn = new DateTimeTableColumn<WorkItem>(SR.ColumnRequestedTime, w => w.RequestedTime) { WidthFactor = .5f };
			_workItems.Columns.Add(requestedTimeColumn);
			_workItems.Columns.Add(new DateTimeTableColumn<WorkItem>(SR.ColumnScheduledTime, w => w.ScheduledTime) { WidthFactor = .5f });
			_workItems.Columns.Add(new TableColumn<WorkItem, string>(SR.ColumnPriority, w => w.Priority.GetDescription())
									{
										WidthFactor = .25f,
										Comparison = WorkItem.PriorityComparison
									});
			_workItems.Columns.Add(new TableColumn<WorkItem, IconSet>(SR.ColumnProgress, w => w.ProgressIcon)
									{
										WidthFactor = .4f,
										Comparison = WorkItem.ProgressComparison
									});


			// establish initial sort order
			_workItems.Sort(new TableSortParams(requestedTimeColumn, false));

			this.ActivityMonitor = WorkItemActivityMonitor.Create(true);
			_connectionState = _connectionState.Update();

			this.ActivityMonitor.IsConnectedChanged += ActivityMonitorIsConnectedChanged;

			_localServerWatcher.DicomServerConfigurationChanged += OnDicomServerConfigurationChanged;
			_localServerWatcher.StudyStorageConfigurationChanged += OnStorageConfigurationChanged;
			_localServerWatcher.DiskSpaceUsageChanged += OnDiskspaceChanged;
		}

		public override void Stop()
		{
			ActivityMonitor.WorkItemsChanged -= WorkItemsChanged;
			ActivityMonitor.IsConnectedChanged -= ActivityMonitorIsConnectedChanged;
			ActivityMonitor.Dispose();
			ActivityMonitor = null;

			_localServerWatcher.DicomServerConfigurationChanged -= OnDicomServerConfigurationChanged;
			_localServerWatcher.StudyStorageConfigurationChanged -= OnStorageConfigurationChanged;
			_localServerWatcher.DiskSpaceUsageChanged -= OnDiskspaceChanged;

			_textFilterTimer.Dispose();
			_studyCountWatcher.Dispose();

			base.Stop();
		}

		#region Presentation Model

		public bool IsConnected
		{
			get { return _connectionState is ConnectedState; }
		}

		public string AeTitle
		{
			get { return _localServerWatcher.AETitle; }
		}

		public string HostName
		{
			get { return _localServerWatcher.HostName; }
		}

		public int Port
		{
			get { return _localServerWatcher.Port; }
		}

		public string FileStore
		{
			get { return _localServerWatcher.FileStoreDirectory; }
		}

		public string DiskspaceUsed
		{
			get
			{
				return string.Format(SR.DiskspaceTemplate,
										Diskspace.FormatBytes(_localServerWatcher.DiskSpaceUsed),
										Diskspace.FormatBytes(_localServerWatcher.DiskSpaceTotal),
										_localServerWatcher.DiskSpaceUsedPercent.ToString("F1"));
			}
		}

		public int DiskspaceUsedPercent
		{
			get { return (int)Math.Round(_localServerWatcher.DiskSpaceUsedPercent); }
		}

		public bool IsMaximumDiskspaceUsageExceeded
		{
			get { return _localServerWatcher.IsMaximumDiskspaceUsageExceeded; }
		}

		public string DiskSpaceWarningMessage
		{
			get
			{
				if (!IsMaximumDiskspaceUsageExceeded)
					return String.Empty;

				return SR.MessageMaximumDiskUsageExceeded;
			}
		}

		public string DiskSpaceWarningDescription
		{
			get
			{
				if (!IsMaximumDiskspaceUsageExceeded)
					return String.Empty;

				return SR.DescriptionMaximumDiskUsageExceeded;
			}
		}

		public int TotalStudies
		{
			get { return _studyCountWatcher.StudyCount; }
		}

		public int Failures
		{
			get { return _workItemManager.FailedItemCount; }
		}

		public ActionModelNode WorkItemActions
		{
			get { return _workItemActionModel; }
		}

		public ITable WorkItemTable
		{
			get { return _workItems; }
		}

		public ISelection WorkItemSelection
		{
			get
			{
				var q = from id in _workItemActionModel.SelectedWorkItemIDs
						let item = _workItems.Items.FirstOrDefault(item => item.Id == id)
						where item != null
						select item;
				return new Selection(q);
			}
			set
			{
				var ids = value.Items.Cast<WorkItem>().Select(w => w.Id).ToArray();
				SetWorkItemSelection(ids);
			}
		}

		public IList StatusFilterChoices
		{
			get { return new[] { NoFilter }.Concat(Enum.GetValues(typeof(StatusFilterValue)).Cast<object>().OrderBy<object, string>(FormatStatusFilter)).ToList(); }
		}

		public string FormatStatusFilter(object value)
		{
			if (value == NoFilter)
				return SR.NoFilterItem;
			switch ((StatusFilterValue)value)
			{
				case StatusFilterValue.Active:
					return SR.StatusFilterValueActive;
				case StatusFilterValue.Complete:
					return SR.StatusFilterValueComplete;
				case StatusFilterValue.Stopped:
					return SR.StatusFilterValueStopped;
				case StatusFilterValue.Failed:
					return SR.StatusFilterValueFailed;
				default:
					throw new ArgumentOutOfRangeException("value");
			}
		}

		public object StatusFilter
		{
			get { return _statusFilter.HasValue ? _statusFilter.Value : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (StatusFilterValue?)null : (StatusFilterValue)value;
				if (_statusFilter != v)
				{
					_statusFilter = v;
					NotifyPropertyChanged("StatusFilter");
					RefreshInternal();
				}
			}
		}

		public IList ActivityTypeFilterChoices
		{
			get { return new[] { NoFilter }.Concat(WorkItemRequestHelper.GetActivityTypes().OrderBy<object, string>(FormatActivityTypeFilter)).ToList(); }
		}

		public string FormatActivityTypeFilter(object value)
		{
			return value == NoFilter ? SR.NoFilterItem : ((string)value);
		}

		public object ActivityTypeFilter
		{
			get { return !string.IsNullOrEmpty(_activityFilter) ? _activityFilter : NoFilter; }
			set
			{
				var v = (value == NoFilter) ? (string)null : (string)value;
				if (_activityFilter != v)
				{
					_activityFilter = v;
					NotifyPropertyChanged("ActivityTypeFilter");
					RefreshInternal();
				}
			}
		}

		public string TextFilter
		{
			get { return _textFilter; }
			set
			{
				if (value != _textFilter)
				{
					_textFilter = value;
					NotifyPropertyChanged("TextFilter");

					// we don't want to do a full refresh on every keystroke, so
					// rather than refreshing immediately, we just re-start the timer
					_textFilterTimer.Stop();
					_textFilterTimer.Start();
				}
			}
		}

		public void StartReindex()
		{
			ReindexTool.StartReindex(Host.DesktopWindow);
		}

		public void OpenFileStore()
		{
			var path = this.FileStore;
			if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
			{
				Process.Start(path);
			}
			else
			{
				this.Host.ShowMessageBox(SR.MessageFilestoreDirectoryDoesNotExist, MessageBoxActions.Ok);
			}
		}

		public void OpenSystemConfigurationPage()
		{
			HandleLink(ActivityMonitorQuickLink.SystemConfiguration);
		}

		public bool StudyManagementRulesLinkVisible
		{
			get { return ActivityMonitorQuickLink.StudyManagementRules.CanInvoke(); }
		}

		public void OpenStudyManagementRules()
		{
			HandleLink(ActivityMonitorQuickLink.StudyManagementRules);
		}

		public void OpenLogFiles()
		{
			var logdir = Platform.LogDirectory;
			if (!string.IsNullOrEmpty(logdir) && Directory.Exists(logdir))
				Process.Start(logdir);
		}

		#endregion

		private IWorkItemActivityMonitor ActivityMonitor { get; set; }

		private static IconSet GetProgressIcon(WorkItemProgress progress, WorkItemStatusEnum status)
		{
			if (progress == null)
				return null;

			return new ProgressBarIconSet("progress", new Size(80, 10), progress.PercentComplete * 100, ActivityMonitorProgressBar.GetColor(progress, status));
		}

		private void ActivityMonitorIsConnectedChanged(object sender, EventArgs e)
		{
			_connectionState = _connectionState.Update();
			NotifyPropertyChanged("IsConnected");
		}

		private void WorkItemsChanged(object sender, WorkItemsChangedEventArgs e)
		{
			var workItems = e.ChangedItems;
			if (workItems.Any(item => item.Request.ConcurrencyType == WorkItemConcurrency.StudyUpdate
									|| item.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete
									|| item.Type.Equals(ReindexRequest.WorkItemTypeString)))
			{
				_studyCountWatcher.Invalidate();
			}

			var items = workItems.Select(item => new WorkItem(item)).ToList();
			
			// by wrapping this in a transaction, we help the view to retain the current selection
			// and scroll position where possible
			using (_workItems.Items.BeginTransaction())
			{
				var needSort = _workItemManager.Update(items, _workItems.SortParams.GetComparer().Compare);
				if (needSort)
				{
					_workItems.Sort();
				}
			}

			_workItemActionModel.OnWorkItemsChanged(items);
		}

		private void SetWorkItemSelection(IList<long> ids)
		{
			if (_workItemActionModel.SelectedWorkItemIDs.SequenceEqual(ids))
				return;

			_workItemActionModel.SelectedWorkItemIDs = ids;
			NotifyPropertyChanged("WorkItemSelection");
		}

		private void RefreshInternal()
		{
			_workItemManager.Clear();

			try
			{
				this.ActivityMonitor.Refresh();
			}
			catch (Exception e)
			{
				// don't show a message box here, since the user may not even be looking at this workspace
				Platform.Log(LogLevel.Error, e);
			}
		}

		private bool Include(WorkItem item)
		{
			if (!string.IsNullOrEmpty(_activityFilter) && !item.ActivityType.Equals(_activityFilter))
				return false;

			if (_statusFilter.HasValue && WorkItemStatuses(_statusFilter.Value).All(s => s != item.Status))
				return false;

			if (!string.IsNullOrEmpty(_textFilter) && !item.ContainsText(_textFilter))
				return false;

			return true;
		}

		private void OnTextFilterTimerElapsed(object state)
		{
			_textFilterTimer.Stop();
			RefreshInternal();
		}

		private void OnStudyCountChanged()
		{
			NotifyPropertyChanged("TotalStudies");
		}

		private void OnFailureCountChanged()
		{
			NotifyPropertyChanged("Failures");
		}

		private void OnDicomServerConfigurationChanged(object sender, EventArgs eventArgs)
		{
			NotifyPropertyChanged("AeTitle");
			NotifyPropertyChanged("HostName");
			NotifyPropertyChanged("Port");
		}

		private void OnStorageConfigurationChanged(object sender, EventArgs eventArgs)
		{
			NotifyPropertyChanged("FileStore");
			NotifyPropertyChanged("IsMaximumDiskspaceUsageExceeded");

			// if FileStore path changed, diskspace may have changed too
			OnDiskspaceChanged(sender, eventArgs);
		}

		private void OnDiskspaceChanged(object sender, EventArgs eventArgs)
		{
			NotifyPropertyChanged("DiskspaceUsed");
			NotifyPropertyChanged("DiskspaceUsedPercent");
			NotifyPropertyChanged("DiskSpaceWarningLabel");
			NotifyPropertyChanged("DiskSpaceWarningMessage");
		}

		private void HandleLink(ActivityMonitorQuickLink link)
		{
			try
			{
				link.Invoke(this.Host.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private WorkItemStatusEnum[] WorkItemStatuses(StatusFilterValue filterValue)
		{
			switch (filterValue)
			{
				case StatusFilterValue.Active:
					return new[] { WorkItemStatusEnum.Pending, WorkItemStatusEnum.InProgress, WorkItemStatusEnum.Idle };
				case StatusFilterValue.Complete:
					return new[] { WorkItemStatusEnum.Complete };
				case StatusFilterValue.Stopped:
					return new[] { WorkItemStatusEnum.Canceled };
				case StatusFilterValue.Failed:
					return new[] { WorkItemStatusEnum.Failed };
				default:
					throw new ArgumentOutOfRangeException("filterValue");
			}
		}

		private static Comparison<TItem> GetComparison<TItem, TProperty>(TProperty[] order, Func<TItem, TProperty> sortProperty)
		{
			var list = (IList<TProperty>)order;
			return (x, y) => list.IndexOf(sortProperty(x)).CompareTo(list.IndexOf(sortProperty(y)));
		}
	}
}
