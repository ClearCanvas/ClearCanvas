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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public enum WorklistEditorMode
	{
		Add,
		Edit,
		Duplicate
	}

	public class StaffSelectorTable : Table<StaffSummary>
	{
		public StaffSelectorTable()
		{
			this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffName, item => PersonNameFormat.Format(item.Name), 1.0f));
			this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffRole, item => item.StaffType.Value, 0.5f));
		}
	}

	/// <summary>
	/// WorklistEditorComponent class
	/// </summary>
	public class WorklistEditorComponent : NavigatorComponentContainer
	{
		class ProcedureTypeGroupTable : Table<ProcedureTypeGroupSummary>
		{
			public ProcedureTypeGroupTable()
			{
				this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnProcedureTypeGroupName, summary => summary.Name));
			}
		}

		class DepartmentTable : Table<DepartmentSummary>
		{
			public DepartmentTable()
			{
				this.Columns.Add(new TableColumn<DepartmentSummary, string>(SR.ColumnDepartmentName, summary => summary.Name));
				this.Columns.Add(new TableColumn<DepartmentSummary, string>(SR.ColumnFacility, summary => summary.FacilityName));
			}
		}

		class LocationTable : Table<LocationSummary>
		{
			public LocationTable()
			{
				this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnLocationName, summary => summary.Name));
			}
		}

		class StaffGroupTable : Table<StaffGroupSummary>
		{
			public StaffGroupTable()
			{
				this.Columns.Add(new TableColumn<StaffGroupSummary, string>(SR.ColumnStaffGroupName, item => item.Name, 1.0f));
			}
		}

		class ProcedureTypeTable : Table<ProcedureTypeSummary>
		{
			public ProcedureTypeTable()
			{
				this.Columns.Add(new TableColumn<ProcedureTypeSummary, string>(SR.ColumnProcedureTypeName, rpt => string.Format("{0} ({1})", rpt.Name, rpt.Id), 1.0f));

				var invisibleIDColumn = new TableColumn<ProcedureTypeSummary, string>(SR.ColumnID, rpt => rpt.Id, 1.0f)
					{
						Visible = false
					};
				this.Columns.Add(invisibleIDColumn);
				this.Sort(new TableSortParams(invisibleIDColumn, true));
			}
		}

		private readonly WorklistEditorMode _mode;
		private readonly bool _adminMode;
		private readonly string _initialClassName;
		private readonly IList<string> _worklistClassChoices;

		private EntityRef _worklistRef;
		private readonly List<WorklistAdminSummary> _editedWorklistSummaries = new List<WorklistAdminSummary>();
		private WorklistAdminDetail _worklistDetail;

		private WorklistDetailEditorComponentBase _detailComponent;
		private WorklistFilterEditorComponent _filterComponent;
		private StaffSelectorEditorComponent _interpretedByFilterComponent;
		private StaffSelectorEditorComponent _transcribedByFilterComponent;
		private StaffSelectorEditorComponent _verifiedByFilterComponent;
		private StaffSelectorEditorComponent _supervisedByFilterComponent;
		private WorklistTimeWindowEditorComponent _timeWindowComponent;
		private SelectorEditorComponent<ProcedureTypeSummary, ProcedureTypeTable> _procedureTypeFilterComponent;
		private SelectorEditorComponent<ProcedureTypeGroupSummary, ProcedureTypeGroupTable> _procedureTypeGroupFilterComponent;
		private SelectorEditorComponent<DepartmentSummary, DepartmentTable> _departmentFilterComponent;
		private SelectorEditorComponent<LocationSummary, LocationTable> _locationFilterComponent;
		private SelectorEditorComponent<StaffSummary, StaffSelectorTable> _staffSubscribersComponent;
		private SelectorEditorComponent<StaffGroupSummary, StaffGroupTable> _groupSubscribersComponent;
		private WorklistSummaryComponent _summaryComponent;
		private NavigatorPage _departmentComponentPage;
		private NavigatorPage _interpretedByFilterComponentPage;
		private NavigatorPage _transcribedByFilterComponentPage;
		private NavigatorPage _verifiedByFilterComponentPage;
		private NavigatorPage _supervisedByFilterComponentPage;
		private NavigatorPage _timeWindowComponentPage;
		private NavigatorPage _summaryComponentPage;

		private GetWorklistEditFormChoicesResponse _formDataResponse;

		/// <summary>
		/// Constructor to create new worklist(s).
		/// </summary>
		public WorklistEditorComponent(bool adminMode)
			: this(null, adminMode, WorklistEditorMode.Add, null, null)
		{
		}

		/// <summary>
		/// Constructor to create new worklist(s) with limited choices of class.
		/// </summary>
		public WorklistEditorComponent(bool adminMode, IList<string> worklistClassChoices, string selectedClass)
			: this(null, adminMode, WorklistEditorMode.Add, worklistClassChoices, selectedClass)
		{
		}

		/// <summary>
		/// Constructor to edit a worklist.
		/// </summary>
		public WorklistEditorComponent(EntityRef entityRef, bool adminMode)
			: this(entityRef, adminMode, WorklistEditorMode.Edit, null, null)
		{
		}

		/// <summary>
		/// Constructor to duplicate a worklist.
		/// </summary>
		public WorklistEditorComponent(EntityRef entityRef, bool adminMode, IList<string> worklistClassChoices, string selectedClass)
			: this(entityRef, adminMode, WorklistEditorMode.Duplicate, worklistClassChoices, selectedClass)
		{
		}

		/// <summary>
		/// Private constructor.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <param name="adminMode"></param>
		/// <param name="editMode"></param>
		/// <param name="worklistClassChoices"></param>
		/// <param name="selectedClass"></param>
		private WorklistEditorComponent(EntityRef entityRef, bool adminMode, WorklistEditorMode editMode, IList<string> worklistClassChoices, string selectedClass)
		{
			_worklistRef = entityRef;
			_adminMode = adminMode;
			_mode = editMode;
			_worklistClassChoices = worklistClassChoices;
			_initialClassName = selectedClass;

			// start with entire tree expanded
			this.StartFullyExpanded = true;
		}

		public override void Start()
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					var request = new GetWorklistEditFormDataRequest
									{
										GetWorklistEditFormChoicesRequest = new GetWorklistEditFormChoicesRequest(!_adminMode)
									};
					_formDataResponse = service.GetWorklistEditFormData(request).GetWorklistEditFormChoicesResponse;

					// initialize _worklistDetail depending on add vs edit vs duplicate mode
					var procedureTypeGroups = new List<ProcedureTypeGroupSummary>();
					if (_mode == WorklistEditorMode.Add)
					{
						_worklistDetail = new WorklistAdminDetail
							{
								FilterByWorkingFacility = true,
								// establish initial class name
								WorklistClass = CollectionUtils.SelectFirst(_formDataResponse.WorklistClasses, wc => wc.ClassName == _initialClassName)
							};

					}
					else
					{
						// load the existing worklist
						var response = service.LoadWorklistForEdit(new LoadWorklistForEditRequest(_worklistRef));

						_worklistDetail = response.Detail;
						_worklistRef = response.Detail.EntityRef;

						// determine initial set of proc type groups, since worklist class already known
						var groupsResponse = service.ListProcedureTypeGroups(new ListProcedureTypeGroupsRequest(_worklistDetail.WorklistClass.ProcedureTypeGroupClassName));
						procedureTypeGroups = groupsResponse.ProcedureTypeGroups;
					}

					// limit class choices if filter specified
					if (_worklistClassChoices != null)
					{
						_formDataResponse.WorklistClasses =
							CollectionUtils.Select(_formDataResponse.WorklistClasses, wc => _worklistClassChoices.Contains(wc.ClassName));
					}

					// sort worklist classes so they appear alphabetically in editor
					_formDataResponse.WorklistClasses = CollectionUtils.Sort(_formDataResponse.WorklistClasses, (x, y) => x.DisplayName.CompareTo(y.DisplayName));

					// determine which main page to show (multi or single)
					if (_mode == WorklistEditorMode.Add && _adminMode)
					{
						_detailComponent = new WorklistMultiDetailEditorComponent(_formDataResponse.WorklistClasses,
							_formDataResponse.OwnerGroupChoices);
					}
					else
					{
						_detailComponent = new WorklistDetailEditorComponent(
							_worklistDetail,
							_formDataResponse.WorklistClasses,
							_formDataResponse.OwnerGroupChoices,
							_mode,
							_adminMode,
							false);
					}
					_detailComponent.ProcedureTypeGroupClassChanged += ProcedureTypeGroupClassChangedEventHandler;
					_detailComponent.WorklistClassChanged += OnWorklistClassChanged;
					_detailComponent.WorklistCategoryChanged += OnWorklistCategoryChanged;

					// create all other pages
					_filterComponent = new WorklistFilterEditorComponent(_worklistDetail,
						_formDataResponse.FacilityChoices, _formDataResponse.OrderPriorityChoices,
						_formDataResponse.PatientClassChoices);

					_procedureTypeFilterComponent = new SelectorEditorComponent<ProcedureTypeSummary, ProcedureTypeTable>(
						_formDataResponse.ProcedureTypeChoices, _worklistDetail.ProcedureTypes, s => s.ProcedureTypeRef);

					_procedureTypeGroupFilterComponent = new SelectorEditorComponent<ProcedureTypeGroupSummary, ProcedureTypeGroupTable>(
						procedureTypeGroups, _worklistDetail.ProcedureTypeGroups, s => s.ProcedureTypeGroupRef);

					_departmentFilterComponent = new SelectorEditorComponent<DepartmentSummary, DepartmentTable>(
						_formDataResponse.DepartmentChoices, _worklistDetail.Departments, s => s.DepartmentRef);

					_locationFilterComponent = new SelectorEditorComponent<LocationSummary, LocationTable>(
						_formDataResponse.PatientLocationChoices, _worklistDetail.PatientLocations, s => s.LocationRef);

					var maxSpanDays = _formDataResponse.CurrentServerConfigurationRequiresTimeFilter ? _formDataResponse.CurrentServerConfigurationMaxSpanDays : 0;
					_timeWindowComponent = new WorklistTimeWindowEditorComponent(
						_worklistDetail,
						_mode == WorklistEditorMode.Add && _formDataResponse.CurrentServerConfigurationRequiresTimeFilter,
						maxSpanDays);

					_interpretedByFilterComponent = new StaffSelectorEditorComponent(
						_formDataResponse.StaffChoices, _worklistDetail.InterpretedByStaff.Staff, _worklistDetail.InterpretedByStaff.IncludeCurrentUser);

					_transcribedByFilterComponent = new StaffSelectorEditorComponent(
						_formDataResponse.StaffChoices, _worklistDetail.TranscribedByStaff.Staff, _worklistDetail.TranscribedByStaff.IncludeCurrentUser);

					_verifiedByFilterComponent = new StaffSelectorEditorComponent(
						_formDataResponse.StaffChoices, _worklistDetail.VerifiedByStaff.Staff, _worklistDetail.VerifiedByStaff.IncludeCurrentUser);
					_supervisedByFilterComponent = new StaffSelectorEditorComponent(
						_formDataResponse.StaffChoices, _worklistDetail.SupervisedByStaff.Staff, _worklistDetail.SupervisedByStaff.IncludeCurrentUser);

					if (ShowSubscriptionPages)
					{
						_staffSubscribersComponent = new SelectorEditorComponent<StaffSummary, StaffSelectorTable>(
							_formDataResponse.StaffChoices,
							_worklistDetail.StaffSubscribers,
							s => s.StaffRef,
							SubscriptionPagesReadOnly);
						_groupSubscribersComponent = new SelectorEditorComponent<StaffGroupSummary, StaffGroupTable>(
							_formDataResponse.GroupSubscriberChoices,
							_worklistDetail.GroupSubscribers,
							s => s.StaffGroupRef,
							SubscriptionPagesReadOnly);
					}
				});

			// add pages
			this.Pages.Add(new NavigatorPage("NodeWorklist", _detailComponent));
			this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters", _filterComponent));
			this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/FilterProcedureType", _procedureTypeFilterComponent));
			this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/FilterProcedureTypeGroup", _procedureTypeGroupFilterComponent));

			if(new WorkflowConfigurationReader().EnableVisitWorkflow)
			{
				this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/FilterPatientLocation", _locationFilterComponent));
			}
			this.Pages.Add(_departmentComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/FilterDepartment", _departmentFilterComponent));

			_procedureTypeFilterComponent.ItemsAdded += OnProcedureTypeAdded;
			_procedureTypeGroupFilterComponent.ItemsAdded += OnProcedureTypeGroupAdded;

			_interpretedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/FilterInterpretedBy", _interpretedByFilterComponent);

			if (WorklistEditorComponentSettings.Default.ShowTranscribedByPage)
				_transcribedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/FilterTranscribedBy", _transcribedByFilterComponent);

			_verifiedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/FilterVerifiedBy", _verifiedByFilterComponent);
			_supervisedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/FilterSupervisedBy", _supervisedByFilterComponent);

			_timeWindowComponentPage = new NavigatorPage("NodeWorklist/FilterTimeWindow", _timeWindowComponent);

			ShowWorklistCategoryDependantPages();

			if (ShowSubscriptionPages)
			{
				this.Pages.Add(new NavigatorPage("NodeWorklist/NodeSubscribers/NodeGroupSubscribers", _groupSubscribersComponent));
				this.Pages.Add(new NavigatorPage("NodeWorklist/NodeSubscribers/NodeStaffSubscribers", _staffSubscribersComponent));
			}
			this.Pages.Add(_summaryComponentPage = new NavigatorPage("NodeWorklist/NodeWorklistSummary", _summaryComponent = new WorklistSummaryComponent(_worklistDetail, _adminMode)));

			this.CurrentPageChanged += WorklistEditorComponent_CurrentPageChanged;

			this.ValidationStrategy = new AllComponentsValidationStrategy();

			base.Start();

			// Modify EntityRef and add the word "copy" to the worklist name.
			// This is done after the Start() call, so changing the worklist name will trigger a component modify changed.
			if (_mode == WorklistEditorMode.Duplicate)
			{
				_worklistDetail.EntityRef = null;
				((WorklistDetailEditorComponent)_detailComponent).Name = _worklistDetail.Name + " copy";
			}
		}

		private void OnProcedureTypeAdded(object sender, EventArgs e)
		{
			// Only either one of Procedure Type or Procedure Type Group can be used for filtering, but not both.
			// If there are existing procedure type groups, ask user to clear them when a procedure type is added.
			if (_procedureTypeGroupFilterComponent.SelectedItems.Count > 0 &&
				DialogBoxAction.Yes == this.Host.ShowMessageBox(SR.MessageConfirmClearProcedureTypeGroups, MessageBoxActions.YesNo))
			{
				foreach (var item in _procedureTypeGroupFilterComponent.SelectedItemsTable.Items)
					_procedureTypeGroupFilterComponent.AvailableItemsTable.Items.Add(item);
	
				_procedureTypeGroupFilterComponent.SelectedItemsTable.Items.Clear();
			}
		}

		private void OnProcedureTypeGroupAdded(object sender, EventArgs e)
		{
			// Only either one of Procedure Type or Procedure Type Group can be used for filtering, but not both.
			// If there are existing procedure types, ask user to clear them when a procedure type group is added.
			if (_procedureTypeFilterComponent.SelectedItems.Count > 0 &&
				DialogBoxAction.Yes == this.Host.ShowMessageBox(SR.MessageConfirmClearProcedureTypes, MessageBoxActions.YesNo))
			{
				foreach (var item in _procedureTypeFilterComponent.SelectedItemsTable.Items)
					_procedureTypeFilterComponent.AvailableItemsTable.Items.Add(item);

				_procedureTypeFilterComponent.SelectedItemsTable.Items.Clear();
			}
		}

		private void OnWorklistCategoryChanged(object sender, EventArgs e)
		{
			ShowWorklistCategoryDependantPages();
		}

		private void OnWorklistClassChanged(object sender, EventArgs e)
		{
			ShowWorklistCategoryDependantPages();
		}

		private void ShowWorklistCategoryDependantPages()
		{
			var showStaffFilters = ShowStaffRoleFilterPages;
			ShowAfterPage(_interpretedByFilterComponentPage, showStaffFilters, _departmentComponentPage);

			if (WorklistEditorComponentSettings.Default.ShowTranscribedByPage)
			{
				ShowAfterPage(_transcribedByFilterComponentPage, showStaffFilters, _interpretedByFilterComponentPage);
				ShowAfterPage(_verifiedByFilterComponentPage, showStaffFilters, _transcribedByFilterComponentPage);
			}
			else
			{
				ShowAfterPage(_verifiedByFilterComponentPage, showStaffFilters, _interpretedByFilterComponentPage);
			}

			ShowAfterPage(_supervisedByFilterComponentPage, showStaffFilters, _verifiedByFilterComponentPage);

			ShowBeforePage(_timeWindowComponentPage, true, _summaryComponentPage);
		}

		private void ShowAfterPage(NavigatorPage page, bool show, NavigatorPage insertAfterPage)
		{
			if (show)
			{
				if (this.Pages.Contains(page) == false)
				{
					if (insertAfterPage == null)
						this.Pages.Add(page);
					else
					{
						var index = this.Pages.IndexOf(insertAfterPage);
						this.Pages.Insert(index + 1, page);
					}

				}
			}
			else
			{
				if (this.Pages.Contains(page))
					this.Pages.Remove(page);
			}
		}

		private void ShowBeforePage(NavigatorPage page, bool show, NavigatorPage insertBeforePage)
		{
			if (show)
			{
				if (this.Pages.Contains(page) == false)
				{
					if (insertBeforePage == null)
						this.Pages.Add(page);
					else
					{
						var index = this.Pages.IndexOf(insertBeforePage);
						this.Pages.Insert(index, page);
					}
					
				}
			}
			else
			{
				if (this.Pages.Contains(page))
					this.Pages.Remove(page);
			}
		}

		private void WorklistEditorComponent_CurrentPageChanged(object sender, EventArgs e)
		{
			// Update the summary page when it is active
			if (this.CurrentPage.Component == _summaryComponent)
			{
				UpdateWorklistDetail();

				if (_detailComponent is WorklistMultiDetailEditorComponent)
				{
					var detailEditor = (WorklistMultiDetailEditorComponent)_detailComponent;

					var names = new List<string>();
					var descriptions = new List<string>();
					var classes = new List<WorklistClassSummary>();

					CollectionUtils.ForEach(detailEditor.WorklistsToCreate,
							delegate(WorklistMultiDetailEditorComponent.WorklistTableEntry entry)
								{
									names.Add(entry.Name);
									descriptions.Add(entry.Description);
									classes.Add(entry.Class);
								});

					_summaryComponent.SetMultipleWorklistInfo(names, descriptions, classes);
			}

				_summaryComponent.Refresh();
		}
		}

		private void ProcedureTypeGroupClassChangedEventHandler(object sender, EventArgs e)
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					var groupsResponse = service.ListProcedureTypeGroups(
						new ListProcedureTypeGroupsRequest(_detailComponent.ProcedureTypeGroupClass));

					_procedureTypeGroupFilterComponent.AllItems = groupsResponse.ProcedureTypeGroups;
				});
		}

		public List<WorklistAdminSummary> EditedWorklistSummaries
		{
			get { return _editedWorklistSummaries; }
		}

		#region Presentation Model

		public override void Accept()
		{
			UpdateWorklistDetail();
			WarnAboutNonOptimalFilterChoices();

			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
				try
				{
					if (_mode == WorklistEditorMode.Add || _mode == WorklistEditorMode.Duplicate)
					{
						AddWorklists();
					}
					else
					{
						UpdateWorklist();
					}

					// save any modified user settings
					WorklistEditorComponentSettings.Default.Save();

					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionSaveWorklist, this.Host.DesktopWindow,
						() => this.Exit(ApplicationComponentExitCode.Error));
				}
			}
		}

		public void ItemsAddedOrRemoved()
		{
			this.Modified = true;
		}

		#endregion

		private bool ShowSubscriptionPages
		{
			get { return _adminMode && Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Admin.Data.Worklist); }
		}

		private bool SubscriptionPagesReadOnly
		{
			get { return _worklistDetail.IsUserWorklist; }
		}

		private bool ShowStaffRoleFilterPages
		{
			get
			{
				return _worklistDetail != null && _worklistDetail.WorklistClass != null
						? _worklistDetail.WorklistClass.SupportsReportingStaffRoleFilters
						: CollectionUtils.Contains<WorklistClassSummary>(_detailComponent.WorklistClassChoices, wc => wc.SupportsReportingStaffRoleFilters);
			}
		}

		private void UpdateWorklistDetail()
		{
			if (_filterComponent.IsStarted)
				_filterComponent.SaveData();

			if (_timeWindowComponent.IsStarted)
				_timeWindowComponent.SaveData();

			if (_procedureTypeFilterComponent.IsStarted)
				_worklistDetail.ProcedureTypes = new List<ProcedureTypeSummary>(_procedureTypeFilterComponent.SelectedItems);

			if (_procedureTypeGroupFilterComponent.IsStarted)
				_worklistDetail.ProcedureTypeGroups = new List<ProcedureTypeGroupSummary>(_procedureTypeGroupFilterComponent.SelectedItems);

			if (_departmentFilterComponent.IsStarted)
				_worklistDetail.Departments = new List<DepartmentSummary>(_departmentFilterComponent.SelectedItems);

			if (_locationFilterComponent.IsStarted)
				_worklistDetail.PatientLocations = new List<LocationSummary>(_locationFilterComponent.SelectedItems);

			if (ShowSubscriptionPages && _groupSubscribersComponent.IsStarted)
				_worklistDetail.GroupSubscribers = new List<StaffGroupSummary>(_groupSubscribersComponent.SelectedItems);

			if (ShowSubscriptionPages && _staffSubscribersComponent.IsStarted)
				_worklistDetail.StaffSubscribers = new List<StaffSummary>(_staffSubscribersComponent.SelectedItems);

			if (ShowStaffRoleFilterPages && _interpretedByFilterComponent.IsStarted)
			{
				_worklistDetail.InterpretedByStaff.Staff = new List<StaffSummary>(_interpretedByFilterComponent.SelectedItems);
				_worklistDetail.InterpretedByStaff.IncludeCurrentUser = _interpretedByFilterComponent.IncludeCurrentUser;
			}

			if (ShowStaffRoleFilterPages && _transcribedByFilterComponent.IsStarted)
			{
				_worklistDetail.TranscribedByStaff.Staff = new List<StaffSummary>(_transcribedByFilterComponent.SelectedItems);
				_worklistDetail.TranscribedByStaff.IncludeCurrentUser = _transcribedByFilterComponent.IncludeCurrentUser;
			}

			if (ShowStaffRoleFilterPages && _verifiedByFilterComponent.IsStarted)
			{
				_worklistDetail.VerifiedByStaff.Staff = new List<StaffSummary>(_verifiedByFilterComponent.SelectedItems);
				_worklistDetail.VerifiedByStaff.IncludeCurrentUser = _verifiedByFilterComponent.IncludeCurrentUser;
			}

			if (ShowStaffRoleFilterPages && _supervisedByFilterComponent.IsStarted)
			{
				_worklistDetail.SupervisedByStaff.Staff = new List<StaffSummary>(_supervisedByFilterComponent.SelectedItems);
				_worklistDetail.SupervisedByStaff.IncludeCurrentUser = _supervisedByFilterComponent.IncludeCurrentUser;
			}
		}

		private void WarnAboutNonOptimalFilterChoices()
		{
			CheckFilterChoices(SR.FilterProcedureType, _worklistDetail.ProcedureTypes, _formDataResponse.ProcedureTypeChoices);
			CheckFilterChoices(SR.FilterPerformingFacility, _worklistDetail.Facilities, _formDataResponse.FacilityChoices);
			CheckFilterChoices(SR.FilterDepartment, _worklistDetail.Departments, _formDataResponse.DepartmentChoices);
			CheckFilterChoices(SR.FilterPatientClass, _worklistDetail.PatientClasses, _formDataResponse.PatientClassChoices);
			CheckFilterChoices(SR.FilterPatientLocation, _worklistDetail.PatientLocations, _formDataResponse.PatientLocationChoices);
			CheckFilterChoices(SR.FilterOrderPriority, _worklistDetail.OrderPriorities, _formDataResponse.OrderPriorityChoices);
			CheckFilterChoices(SR.FilterPortable, _worklistDetail.Portabilities, new[] { true, false });

			CheckFilterChoices(SR.FilterInterpretedBy, _worklistDetail.InterpretedByStaff.Staff, _formDataResponse.StaffChoices);
			CheckFilterChoices(SR.FilterTranscribedBy, _worklistDetail.TranscribedByStaff.Staff, _formDataResponse.StaffChoices);
			CheckFilterChoices(SR.FilterVerifiedBy, _worklistDetail.VerifiedByStaff.Staff, _formDataResponse.StaffChoices);
			CheckFilterChoices(SR.FilterSupervisedBy, _worklistDetail.SupervisedByStaff.Staff, _formDataResponse.StaffChoices);
		}

		private void CheckFilterChoices<TSummary>(string filterName, IList<TSummary> filterValues, IList<TSummary> allValues)
		{
			// if all possible values have been selected for the filter, inform the user that this isn't usually a good idea
			if (allValues.Count > 0 && CollectionUtils.Equal(filterValues, allValues, false))
			{
				var msg = string.Format(SR.MessageWarnAllFilterValuesShouldNotBeSelected, filterName);
				var action = this.Host.ShowMessageBox(msg, MessageBoxActions.YesNo);
				if (action == DialogBoxAction.Yes)
				{
					filterValues.Clear();
				}
			}
		}

		private void AddWorklists()
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					if (_detailComponent is WorklistMultiDetailEditorComponent)
					{
						// add each worklist in the multi editor
						var detailEditor = (WorklistMultiDetailEditorComponent)_detailComponent;
						foreach (var entry in detailEditor.WorklistsToCreate)
						{
							_worklistDetail.Name = entry.Name;
							_worklistDetail.Description = entry.Description;
							_worklistDetail.WorklistClass = entry.Class;

							var response = service.AddWorklist(new AddWorklistRequest(_worklistDetail, !_adminMode));
							_editedWorklistSummaries.Add(response.WorklistAdminSummary);
						}
					}
					else
					{
						// only 1 worklist to add
						var response = service.AddWorklist(new AddWorklistRequest(_worklistDetail, !_adminMode));
						_editedWorklistSummaries.Add(response.WorklistAdminSummary);
					}

				});
		}

		private void UpdateWorklist()
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					var response = service.UpdateWorklist(new UpdateWorklistRequest(_worklistRef, _worklistDetail));
					_worklistRef = response.WorklistAdminSummary.WorklistRef;
					_editedWorklistSummaries.Add(response.WorklistAdminSummary);
				});
		}
	}
}
