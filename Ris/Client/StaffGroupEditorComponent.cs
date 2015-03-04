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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Ris.Client.Formatting;
using System.Threading;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the staff group editor.
	/// </summary>
	public interface IStaffGroupEditorPageProvider : IExtensionPageProvider<IStaffGroupEditorPage, IStaffGroupEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor context.
	/// </summary>
	public interface IStaffGroupEditorContext
	{
		EntityRef StaffGroupRef { get; }
	}

	/// <summary>
	/// Defines an interface to a custom staff group editor page.
	/// </summary>
	public interface IStaffGroupEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the staff group editor.
	/// </summary>
	public class StaffGroupEditorPageProviderExtensionPoint : ExtensionPoint<IStaffGroupEditorPageProvider>
	{
	}

	/// <summary>
	/// Allows editing of staff group information.
	/// </summary>
	public class StaffGroupEditorComponent : NavigatorComponentContainer
	{
		#region StaffGroupEditorContext

		class EditorContext : IStaffGroupEditorContext
		{
			private readonly StaffGroupEditorComponent _owner;

			public EditorContext(StaffGroupEditorComponent owner)
			{
				_owner = owner;
			}

			public EntityRef StaffGroupRef
			{
				get { return _owner._staffGroupRef; }
			}
		}

		#endregion

		class StaffTable : Table<StaffSummary>
		{
			public StaffTable()
			{
				this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffName, item => PersonNameFormat.Format(item.Name), 1.0f));
				this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffRole, item => item.StaffType.Value, 0.5f));
			}
		}

		class WorklistTable : Table<WorklistSummary>
		{
			public WorklistTable()
			{
				this.Columns.Add(new TableColumn<WorklistSummary, string>(SR.ColumnWorklistName, summary => summary.DisplayName, 0.5f));
				this.Columns.Add(new TableColumn<WorklistSummary, string>(SR.ColumnClass,
					summary => string.Concat(summary.ClassCategoryName, " - ", summary.ClassDisplayName),
					0.5f));
			}
		}

		private EntityRef _staffGroupRef;
		private StaffGroupDetail _staffGroupDetail;

		// return value
		private StaffGroupSummary _staffGroupSummary;

		private StaffGroupDetailsEditorComponent _detailsEditor;
		private SelectorEditorComponent<StaffSummary, StaffTable> _staffEditor;
		private SelectorEditorComponent<WorklistSummary, WorklistTable> _worklistEditor;

		private List<IStaffGroupEditorPage> _extensionPages;

		/// <summary>
		/// Constructs an editor to edit a new staff
		/// </summary>
		public StaffGroupEditorComponent()
		{
		}

		/// <summary>
		/// Constructs an editor to edit an existing staff profile
		/// </summary>
		public StaffGroupEditorComponent(EntityRef staffGroupRef)
		{
			_staffGroupRef = staffGroupRef;
		}

		/// <summary>
		/// Gets summary of staff group that was added or edited
		/// </summary>
		public StaffGroupSummary StaffGroupSummary
		{
			get { return _staffGroupSummary; }
		}

		public override void Start()
		{
			LoadStaffGroupEditorFormDataResponse formDataResponse = null;

			Platform.GetService<IStaffGroupAdminService>(service =>
				{
					formDataResponse = service.LoadStaffGroupEditorFormData(
						new LoadStaffGroupEditorFormDataRequest());

					if (_staffGroupRef == null)
					{
						_staffGroupDetail = new StaffGroupDetail();
					}
					else
					{
						var response = service.LoadStaffGroupForEdit(new LoadStaffGroupForEditRequest(_staffGroupRef));
						_staffGroupRef = response.StaffGroup.StaffGroupRef;
						_staffGroupDetail = response.StaffGroup;
					}
				});

			_detailsEditor = new StaffGroupDetailsEditorComponent { StaffGroupDetail = _staffGroupDetail };

			_staffEditor = new SelectorEditorComponent<StaffSummary, StaffTable>(
				formDataResponse.AllStaff,
				_staffGroupDetail.Members,
				staff => staff.StaffRef);

			var isWorklistEditorReadOnly = Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Admin.Data.Worklist) == false;
			_worklistEditor = new SelectorEditorComponent<WorklistSummary, WorklistTable>(
				formDataResponse.AllAdminWorklists,
				_staffGroupDetail.Worklists,
				worklist => worklist.WorklistRef,
				isWorklistEditorReadOnly);

			this.Pages.Add(new NavigatorPage("NodeStaffGroup", _detailsEditor));
			this.Pages.Add(new NavigatorPage("NodeStaffGroup/NodeMembers", _staffEditor));
			this.Pages.Add(new NavigatorPage(
				isWorklistEditorReadOnly
					? "NodeStaffGroup/NodeSubscribedWorklistsReadOnly"
					: "NodeStaffGroup/NodeSubscribedWorklists", 
				_worklistEditor));

			// instantiate all extension pages
			_extensionPages = new List<IStaffGroupEditorPage>();
			foreach (IStaffGroupEditorPageProvider pageProvider in new StaffGroupEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new EditorContext(this)));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (var page in _extensionPages)
			{
				this.Pages.Add(new NavigatorPage(page.Path, page.GetComponent()));
			}

			base.Start();
		}

		public override void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				// give extension pages a chance to save data prior to commit
				_extensionPages.ForEach(page => page.Save());

				// Update staffs
				_staffGroupDetail.Members = new List<StaffSummary>(_staffEditor.SelectedItems);

				if (!_worklistEditor.IsReadOnly)
					_staffGroupDetail.Worklists = new List<WorklistSummary>(_worklistEditor.SelectedItems);

				Platform.GetService<IStaffGroupAdminService>(service =>
					{
						if (_staffGroupRef == null)
						{
							var response = service.AddStaffGroup(new AddStaffGroupRequest(_staffGroupDetail));
							_staffGroupRef = response.StaffGroup.StaffGroupRef;
							_staffGroupSummary = response.StaffGroup;
						}
						else
						{
							var response = service.UpdateStaffGroup(new UpdateStaffGroupRequest(_staffGroupDetail));
							_staffGroupRef = response.StaffGroup.StaffGroupRef;
							_staffGroupSummary = response.StaffGroup;
						}
					});

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ErrorUnableToSaveStaffGroup, this.Host.DesktopWindow,
				                        () => Exit(ApplicationComponentExitCode.Error));
			}
		}
	}
}
