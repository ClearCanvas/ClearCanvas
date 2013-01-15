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
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using System.Collections;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ProtocolEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ProtocolEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProtocolEditorComponent class
	/// </summary>
	[AssociateView(typeof(ProtocolEditorComponentViewExtensionPoint))]
	public class ProtocolEditorComponent : ApplicationComponent
	{
		internal interface IDefaultProtocolGroupSettingsProvider
		{
			string this[string procedureName] { get; set; }
			string GetSuggestedDefault();
		}

		private class DefaultProtocolGroupSettingsProvider : IDefaultProtocolGroupSettingsProvider
		{
			#region IDefaultProtocolGroupSettingsProvider Members

			public string this[string procedureName]
			{
				get
				{
					return ProtocollingSettings.Default.GetDefaultProtocolGroup(procedureName);
				}
				set
				{
					if (String.IsNullOrEmpty(value)) return;

					ProtocollingSettings.Default.SetDefaultProtocolGroup(value, procedureName);
				}
			}

			public string GetSuggestedDefault()
			{
				return ProtocollingSettings.Default.LastDefaultProtocolGroup;
			}

			#endregion
		}

		#region Private Fields

		private ReportingWorklistItemSummary _worklistItem;

		private List<EnumValueInfo> _protocolUrgencyChoices;

		private ProtocolDetail _protocolDetail;
		private string _proceduresText;

		private List<ProtocolGroupSummary> _protocolGroupChoices;
		private ProtocolGroupSummary _protocolGroup;
		private string _defaultProtocolGroupName;
		private readonly IDefaultProtocolGroupSettingsProvider _defaultProtocolGroupProvider = new DefaultProtocolGroupSettingsProvider();

		private readonly ProtocolCodeTable _availableProtocolCodes;
		private readonly ProtocolCodeTable _selectedProtocolCodes;
		private ProtocolCodeSummary _selectedProtocolCodesSelection;
		private bool _canEdit;

		private ILookupHandler _supervisorLookupHandler;
		private bool _rememberSupervisor;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public ProtocolEditorComponent(ReportingWorklistItemSummary worklistItem)
		{
			_worklistItem = worklistItem;

			_availableProtocolCodes = new ProtocolCodeTable();
			_selectedProtocolCodes = new ProtocolCodeTable();
			_selectedProtocolCodes.Items.ItemsChanged += SelectedProtocolCodesChanged;
			_protocolGroupChoices = new List<ProtocolGroupSummary>();
		}

		#region ApplicationComponent overrides

		public override void Start()
		{
			// create supervisor lookup handler, using filters supplied in application settings
			var filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
			var staffTypes = string.IsNullOrEmpty(filters)
				? new string[] { }
				: CollectionUtils.Map<string, string>(filters.Split(','), s => s.Trim()).ToArray();
			_supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

			_rememberSupervisor = ProtocollingSettings.Default.ShouldApplyDefaultSupervisor;

			Platform.GetService<IProtocollingWorkflowService>(service =>
				{
					var response = service.GetProtocolFormData(new GetProtocolFormDataRequest());
					_protocolUrgencyChoices = response.ProtocolUrgencyChoices;

					LoadWorklistItem(service);
				});

			base.Start();
		}

		#endregion

		public ReportingWorklistItemSummary WorklistItem
		{
			get { return _worklistItem; }
			set
			{
				_worklistItem = value;

				Platform.GetService<IProtocollingWorkflowService>(
					service => LoadWorklistItem(service));
			}
		}

		public ProtocolDetail ProtocolDetail
		{
			get { return _protocolDetail; }
		}

		private void LoadWorklistItem(IProtocollingWorkflowService service)
		{
			if (_worklistItem == null)
				return;

			var protocolRequest = new GetProcedureProtocolRequest(_worklistItem.ProcedureRef);
			var protocolResponse = service.GetProcedureProtocol(protocolRequest);

			_protocolDetail = protocolResponse.ProtocolDetail;

			var sb = new StringBuilder();
			foreach (var procedure in _protocolDetail.Procedures)
			{
				sb.Append(ProcedureFormat.Format(procedure) + ", ");
			}

			_proceduresText = sb.ToString().TrimEnd(", ".ToCharArray());

			// Load available protocol groups
			var request = new ListProtocolGroupsForProcedureRequest(_worklistItem.ProcedureRef);
			var response = service.ListProtocolGroupsForProcedure(request);

			_protocolGroupChoices = response.ProtocolGroups;
			_protocolGroup = GetInitialProtocolGroup();

			RefreshAvailableProtocolCodes(_protocolDetail.Codes, service);

			// fill out selected item codes
			_selectedProtocolCodes.Items.Clear();
			_selectedProtocolCodes.Items.AddRange(_protocolDetail.Codes);

			if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview)
				&& _protocolDetail.Supervisor == null)
			{
				// if this user has a default supervisor, retreive it, otherwise leave supervisor as null
				if (_rememberSupervisor && !String.IsNullOrEmpty(ProtocollingSettings.Default.SupervisorID))
				{
					_protocolDetail.Supervisor = GetStaffByID(ProtocollingSettings.Default.SupervisorID);
				}
			}

			NotifyPropertyChanged("ProtocolGroupChoices");
			NotifyPropertyChanged("ProtocolGroup");
			NotifyPropertyChanged("SetDefaultProtocolGroupEnabled");
			NotifyPropertyChanged("Urgency");
		}

		private StaffSummary GetStaffByID(string id)
		{
			StaffSummary staff = null;
			Platform.GetService<IStaffAdminService>(service =>
				{
					ListStaffResponse response = service.ListStaff(
						new ListStaffRequest(id, null, null, null, null, true));
					staff = CollectionUtils.FirstElement(response.Staffs);
				});
			return staff;
		}

		#region Presentation Model

		#region Supervisor

		public StaffSummary Supervisor
		{
			get
			{
				return _protocolDetail != null ? _protocolDetail.Supervisor : null;
			}
			set
			{
				SetSupervisor(value);
				NotifyPropertyChanged("Supervisor");
			}
		}

		private void SetSupervisor(StaffSummary supervisor)
		{
			if (_protocolDetail != null)
			{
				_protocolDetail.Supervisor = supervisor;
				ProtocollingSettings.Default.SupervisorID = supervisor == null ? "" : supervisor.StaffId;
				ProtocollingSettings.Default.Save();
			}
		}

		public ILookupHandler SupervisorLookupHandler
		{
			get { return _supervisorLookupHandler; }
		}

		public bool SupervisorVisible
		{
			get
			{
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview)
					|| (_canEdit == false && _protocolDetail != null && _protocolDetail.Supervisor != null);
			}
		}

		public bool RememberSupervisor
		{
			get { return _rememberSupervisor; }
			set
			{
				if (!Equals(value, _rememberSupervisor))
				{
					_rememberSupervisor = value;
					ProtocollingSettings.Default.ShouldApplyDefaultSupervisor = _rememberSupervisor;
					ProtocollingSettings.Default.Save();
					NotifyPropertyChanged("RememberSupervisor");
				}
			}
		}

		public bool RememberSupervisorVisible
		{
			get
			{
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview)
					&& !Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.OmitSupervisor);
			}
		}

		#endregion

		public string Author
		{
			get
			{
				return _protocolDetail != null
					? PersonNameFormat.Format(_protocolDetail.Author.Name)
					: string.Empty;
			}
		}

		public bool ShowAuthor
		{
			get
			{
				if (_protocolDetail != null)
				{
					return !String.Equals(LoginSession.Current.Staff.StaffId, _protocolDetail.Author.StaffId);
				}

				return false;
			}
		}

		public EnumValueInfo Urgency
		{
			get { return _protocolDetail == null ? null : _protocolDetail.Urgency; }
			set
			{
				_protocolDetail.Urgency = value;
				this.Modified = true;
			}
		}

		public IList UrgencyChoices
		{
			get { return _protocolUrgencyChoices; }
		}

		public IList<string> ProtocolGroupChoices
		{
			get
			{
				return CollectionUtils.Map<ProtocolGroupSummary, string>(
					_protocolGroupChoices,
					summary => AppendDefaultText(summary.Name));
			}
		}

		public string ProtocolGroup
		{
			get { return _protocolGroup == null ? "" : AppendDefaultText(_protocolGroup.Name); }
			set
			{
				_protocolGroup = (value == null)
					? null
					: CollectionUtils.SelectFirst(_protocolGroupChoices, summary => summary.Name == RemoveDefaultText(value));

				ProtocolGroupSelectionChanged();
			}
		}

		public bool SetDefaultProtocolGroupEnabled
		{
			get
			{
				if (_protocolGroup == null || !this.CanEdit)
					return false;

				return _defaultProtocolGroupName != _protocolGroup.Name;
			}
		}

		public void SetDefaultProtocolGroup()
		{
			_defaultProtocolGroupProvider[_worklistItem.ProcedureName] = _defaultProtocolGroupName = _protocolGroup.Name;

			NotifyPropertyChanged("ProtocolGroupChoices");
			NotifyPropertyChanged("ProtocolGroup");
			NotifyPropertyChanged("SetDefaultProtocolGroupEnabled");
		}

		public ITable AvailableProtocolCodesTable
		{
			get { return _availableProtocolCodes; }
		}

		public ITable SelectedProtocolCodesTable
		{
			get { return _selectedProtocolCodes; }
		}

		// this isn't actually used by the component, but we need it for validation
		// so that we can show an error icon next to the "selected items" table
		public ISelection SelectedProtocolCodesSelection
		{
			get { return new Selection(_selectedProtocolCodesSelection); }
			set
			{
				_selectedProtocolCodesSelection = (ProtocolCodeSummary)value.Item;
			}
		}

		public bool CanEdit
		{
			get { return _canEdit; }
			internal set { _canEdit = value; }
		}

		public string ProceduresText
		{
			get { return _proceduresText; }
		}

		#endregion

		#region Private Methods

		private void RefreshAvailableProtocolCodes(IEnumerable<ProtocolCodeSummary> existingSelectedCodes, IProtocollingWorkflowService service)
		{
			_availableProtocolCodes.Items.Clear();

			if (_protocolGroup == null)
				return;

			var protocolCodesDetailResponse = service.GetProtocolGroupDetail(new GetProtocolGroupDetailRequest(_protocolGroup));

			_availableProtocolCodes.Items.AddRange(protocolCodesDetailResponse.ProtocolGroup.Codes);

			// Make existing code selections unavailable
			foreach (var code in existingSelectedCodes)
			{
				_availableProtocolCodes.Items.Remove(code);
			}
		}

		private void SelectedProtocolCodesChanged(object sender, ItemChangedEventArgs e)
		{
			var code = (ProtocolCodeSummary)e.Item;
			switch (e.ChangeType)
			{
				case ItemChangeType.ItemAdded:
					_protocolDetail.Codes.Add(code);
					break;
				case ItemChangeType.ItemRemoved:
					_protocolDetail.Codes.Remove(code);
					break;
				default:
					return;
			}

			this.Modified = true;
		}

		private ProtocolGroupSummary GetInitialProtocolGroup()
		{
			ProtocolGroupSummary defaultProtocolGroup = null;

			// Use the default if one exists for this procedure.
			// Otherwise, get a suggested initial group and set the default to the suggested value
			_defaultProtocolGroupName = _defaultProtocolGroupProvider[_worklistItem.ProcedureName]
									?? (_defaultProtocolGroupProvider[_worklistItem.ProcedureName] = GetSuggestedDefault());

			if (string.IsNullOrEmpty(_defaultProtocolGroupName) == false)
			{
				defaultProtocolGroup = CollectionUtils.SelectFirst(
					_protocolGroupChoices,
					summary => summary.Name == _defaultProtocolGroupName);
			}

			defaultProtocolGroup = defaultProtocolGroup ?? CollectionUtils.FirstElement(_protocolGroupChoices);

			return defaultProtocolGroup;
		}

		private string GetSuggestedDefault()
		{
			var suggestedDefault = _defaultProtocolGroupProvider.GetSuggestedDefault();
			return CollectionUtils.Contains(_protocolGroupChoices, pgs => pgs.Name == suggestedDefault)
				? suggestedDefault
				: null;
		}

		private void ProtocolGroupSelectionChanged()
		{
			try
			{
				Platform.GetService<IProtocollingWorkflowService>(
					service => RefreshAvailableProtocolCodes(_protocolDetail.Codes, service));
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		// Refresh the list of available protocol codes when the list of protocol groups is initially loaded 
		// and whenever the protocol group selection changes
		private string AppendDefaultText(string value)
		{
			return value == _defaultProtocolGroupName ? value + " (Default)" : value;
		}

		private static string RemoveDefaultText(string value)
		{
			return value.EndsWith(" (Default)") ? value.Replace(" (Default)", "") : value;
		}

		#endregion
	}
}
