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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[ExtensionPoint]
	public class WorklistSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorklistSummaryComponent class
	/// </summary>
	[AssociateView(typeof(WorklistSummaryComponentViewExtensionPoint))]
	public class WorklistSummaryComponent : SummaryComponentBase<WorklistAdminSummary, WorklistAdminSummaryTable, ListWorklistsRequest>
	{
		private readonly WorklistClassSummary _filterNone = new WorklistClassSummary(SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 false);
		private readonly object _duplicateWorklistActionKey = new object();
		private string _name;
		private WorklistClassSummary _worklistClass;
		private readonly ArrayList _worklistClassChoices = new ArrayList();
		private bool _includeUseDefinedWorklists;

		public override void Start()
		{
			Platform.GetService(
					delegate(IWorklistAdminService service)
					{
						var request = new GetWorklistEditFormDataRequest
								{
									GetWorklistEditFormChoicesRequest = new GetWorklistEditFormChoicesRequest(false)
								};
						var response = service.GetWorklistEditFormData(request);
						_worklistClassChoices.Add(_filterNone);
						response.GetWorklistEditFormChoicesResponse.WorklistClasses.Sort(
							(x, y) =>
							x.CategoryName.Equals(y.CategoryName)
								? x.DisplayName.CompareTo(y.DisplayName)
								: x.CategoryName.CompareTo(y.CategoryName));
						_worklistClassChoices.AddRange(response.GetWorklistEditFormChoicesResponse.WorklistClasses);
					});
			base.Start();
			_worklistClass = _filterNone;
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			// add a "duplicate worklist" action 
			this.ActionModel.AddAction(_duplicateWorklistActionKey, SR.TitleDuplicate, "Icons.DuplicateSmall.png", DuplicateWorklist);

			model.Add.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.Worklist);
			model.Edit.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.Worklist);
			model.Delete.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.Worklist);
			model[_duplicateWorklistActionKey].SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.Worklist);
		}

		#region Presentation Model

		public object NullFilter
		{
			get { return _filterNone; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool IncludeUserDefinedWorklists
		{
			get { return _includeUseDefinedWorklists; }
			set { _includeUseDefinedWorklists = value; }
		}

		public object SelectedWorklistClass
		{
			get { return _worklistClass; }
			set
			{
				if (value == _filterNone)
					_worklistClass = null;
				else
					_worklistClass = (WorklistClassSummary)value;

				if (value == _filterNone)
					_worklistClass = _filterNone;
			}
		}

		public IList WorklistClassChoices
		{
			get { return _worklistClassChoices; }
		}

		public string FormatWorklistClassChoicesItem(object item)
		{
			if (item != _filterNone)
			{
				var summary = (WorklistClassSummary)item;
				return string.Format("{0} - {1}", summary.CategoryName, summary.DisplayName);
			}
			return SR.DummyItemNone;
		}

		public void DuplicateWorklist()
		{
			try
			{
				if (this.SelectedItems.Count != 1) return;

				var worklist = CollectionUtils.FirstElement(this.SelectedItems);
				var editor = new WorklistEditorComponent(worklist.WorklistRef, true, null, null);
				var exitCode = LaunchAsDialog(this.Host.DesktopWindow,
					new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Large));

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					this.Table.Items.AddRange(editor.EditedWorklistSummaries);
					this.SummarySelection = new Selection(editor.EditedWorklistSummaries);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();
			this.ActionModel[_duplicateWorklistActionKey].Enabled = this.SelectedItems.Count == 1;
		}

		protected override IList<WorklistAdminSummary> ListItems(ListWorklistsRequest request)
		{
			ListWorklistsResponse listResponse = null;

			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					var classNames = (_worklistClass == null || _worklistClass == _filterNone) ?
						new string[] { } : new[] { _worklistClass.ClassName };
					request.ClassNames = new List<string>(classNames);
					request.WorklistName = _name;
					request.IncludeUserDefinedWorklists = _includeUseDefinedWorklists;

					listResponse = service.ListWorklists(request);
				});

			return listResponse.WorklistSummaries;
		}

		protected override bool AddItems(out IList<WorklistAdminSummary> addedItems)
		{
			var editor = new WorklistEditorComponent(true);
			var exitCode = LaunchAsDialog(this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Large));

			switch (exitCode)
			{
				case ApplicationComponentExitCode.Accepted:
					addedItems = editor.EditedWorklistSummaries;
					return true;
				default:
					addedItems = null;
					return false;
			}
		}

		protected override bool EditItems(IList<WorklistAdminSummary> items, out IList<WorklistAdminSummary> editedItems)
		{
			var worklist = CollectionUtils.FirstElement(items);
			var editor = new WorklistEditorComponent(worklist.WorklistRef, true);
			var exitCode = LaunchAsDialog(this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, string.Format(SR.FormatTitleSubtitle, SR.TitleUpdateWorklist, worklist.DisplayName), null, DialogSizeHint.Large));

			switch (exitCode)
			{
				case ApplicationComponentExitCode.Accepted:
					editedItems = editor.EditedWorklistSummaries;
					return true;
				default:
					editedItems = null;
					return false;
			}
		}

		protected override bool DeleteItems(IList<WorklistAdminSummary> items, out IList<WorklistAdminSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<WorklistAdminSummary>();

			foreach (var item in items)
			{
				try
				{
					var summary = item;
					Platform.GetService<IWorklistAdminService>(
						service => service.DeleteWorklist(new DeleteWorklistRequest(summary.WorklistRef)));

					deletedItems.Add(item);
				}
				catch (Exception e)
				{
					failureMessage = e.Message;
				}
			}

			return deletedItems.Count > 0;
		}

		protected override bool IsSameItem(WorklistAdminSummary x, WorklistAdminSummary y)
		{
			return x.WorklistRef.Equals(y.WorklistRef, true);
		}
	}
}
