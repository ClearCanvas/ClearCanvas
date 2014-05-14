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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using Action = ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuExternalPractitioners", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class ExternalPractitionerSummaryTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						new ExternalPractitionerSummaryComponent(),
						SR.TitleExternalPractitioner);
					_workspace.Closed += delegate { _workspace = null; };

				}
				catch (Exception e)
				{
					// failed to launch component
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
			else
			{
				_workspace.Activate();
			}
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ExternalPractitionerSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerSummaryComponent class
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerSummaryComponentViewExtensionPoint))]
	public class ExternalPractitionerSummaryComponent : SummaryComponentBase<ExternalPractitionerSummary, ExternalPractitionerAdminTable, ListExternalPractitionersRequest>
	{
		private string _firstName;
		private string _lastName;

		private Action _mergePractitionerAction;
		private Action _mergeContactPointAction;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerSummaryComponent()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
		public ExternalPractitionerSummaryComponent(bool dialogMode)
			: base(dialogMode)
		{
		}

		#region Presentation Model

		public string FirstName
		{
			get { return _firstName; }
			set
			{
				_firstName = value;
				NotifyPropertyChanged("FirstName");
			}
		}

		public string LastName
		{
			get { return _lastName; }
			set
			{
				_lastName = value;
				NotifyPropertyChanged("LastName");
			}
		}

		public void Clear()
		{
			this.FirstName = null;
			this.LastName = null;
			Search();
		}

		#endregion

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(
				OrPermissions(
					Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner,
					Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Create));
			model.Edit.SetPermissibility(
				OrPermissions(
					Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner,
					Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Update));

			// these actions are only available to admins
			model.Delete.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner);
			model.ToggleActivation.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner);

			_mergePractitionerAction = model.AddAction("mergePractitioner", SR.TitleMergePractitioner, "Icons.MergePersonToolMedium.png",
				SR.TitleMergePractitioner, MergePractitioners);
			_mergePractitionerAction.Enabled = false;
			_mergePractitionerAction.SetPermissibility(
				OrPermissions(
					Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner,
					Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Merge));

			_mergeContactPointAction = model.AddAction("mergeContactPoint", SR.TitleMergeContactPoints, "Icons.MergeContactPointToolMedium.png",
				SR.TitleMergeContactPoints, MergeContactPoint);
			_mergeContactPointAction.Enabled = false;
			_mergeContactPointAction.SetPermissibility(
				OrPermissions(
					Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner,
					Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Merge));

			// #10231: hide these tools for the time being, since the functionality is not well tested
			_mergePractitionerAction.Visible = false;
			_mergeContactPointAction.Visible = false;
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<ExternalPractitionerSummary> ListItems(ListExternalPractitionersRequest request)
		{
			ListExternalPractitionersResponse listResponse = null;
			Platform.GetService<IExternalPractitionerAdminService>(service =>
				{
					request.QueryItems = true;
					request.FirstName = _firstName;
					request.LastName = _lastName;
					listResponse = service.ListExternalPractitioners(request);
				});

			return listResponse.Practitioners;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ExternalPractitionerSummary> addedItems)
		{
			addedItems = new List<ExternalPractitionerSummary>();
			var editor = new ExternalPractitionerEditorComponent();
			var exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddExternalPractitioner);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ExternalPractitionerSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "edit" action.
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool EditItems(IList<ExternalPractitionerSummary> items, out IList<ExternalPractitionerSummary> editedItems)
		{
			editedItems = new List<ExternalPractitionerSummary>();
			var item = CollectionUtils.FirstElement(items);

			var editor = new ExternalPractitionerEditorComponent(item.PractitionerRef);
			var exitCode = LaunchAsDialog(
				this.Host.DesktopWindow,
				editor,
				SR.TitleUpdateExternalPractitioner + " - " + Formatting.PersonNameFormat.Format(item.Name));

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ExternalPractitionerSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<ExternalPractitionerSummary> items, out IList<ExternalPractitionerSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<ExternalPractitionerSummary>();

			foreach (var item in items)
			{
				try
				{
					Platform.GetService<IExternalPractitionerAdminService>(service =>
					{
						service.DeleteExternalPractitioner(new DeleteExternalPractitionerRequest(item.PractitionerRef));
					});

					deletedItems.Add(item);
				}
				catch (Exception e)
				{
					failureMessage = e.Message;
				}
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Called to handle the "toggle activation" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool UpdateItemsActivation(IList<ExternalPractitionerSummary> items, out IList<ExternalPractitionerSummary> editedItems)
		{
			if (CollectionUtils.Contains(items, item => item.Deactivated && item.IsMerged))
			{
				this.Host.ShowMessageBox(SR.MessageCannotActivateMergedPractitioners, MessageBoxActions.Ok);
				editedItems = new List<ExternalPractitionerSummary>();
				return false;
			}

			var results = new List<ExternalPractitionerSummary>();
			foreach (var item in items)
			{
				Platform.GetService<IExternalPractitionerAdminService>(service =>
					{
						var forEditResponse = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(item.PractitionerRef));
						var detail = forEditResponse.PractitionerDetail;
						detail.Deactivated = !detail.Deactivated;

						var updateExternalPractitionerResponse = service.UpdateExternalPractitioner(new UpdateExternalPractitionerRequest(detail, false));
						var summary = updateExternalPractitionerResponse.Practitioner;
						results.Add(summary);
					});
			}

			editedItems = results;
			return true;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(ExternalPractitionerSummary x, ExternalPractitionerSummary y)
		{
			return x.PractitionerRef.Equals(y.PractitionerRef, true);
		}

		/// <summary>
		/// Called when the user changes the selected items in the table.
		/// </summary>
		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			_mergePractitionerAction.Enabled =
				(this.SelectedItems.Count == 1 ||
				this.SelectedItems.Count == 2);

			_mergeContactPointAction.Enabled = this.SelectedItems.Count == 1;
		}

		private static ISpecification OrPermissions(string token1, string token2)
		{
			var or = new OrSpecification();
			or.Add(new PrincipalPermissionSpecification(token1));
			or.Add(new PrincipalPermissionSpecification(token2));
			return or;
		}

		private void MergePractitioners()
		{
			try
			{
				var firstItem = CollectionUtils.FirstElement(this.SelectedItems);
				var secondItem = this.SelectedItems.Count > 1 ? CollectionUtils.LastElement(this.SelectedItems) : null;

				if (firstItem != null && !CanMerge(firstItem) || 
					secondItem != null && !CanMerge(secondItem))
				{
					this.Host.ShowMessageBox(SR.MessageCannotMergeSelectedPractitioners, MessageBoxActions.Ok);
					return;
				}

				var editor = new ExternalPractitionerMergeNavigatorComponent(firstItem.PractitionerRef, secondItem == null ? null : secondItem.PractitionerRef);

				var title = SR.TitleMergePractitioner + " - " + Formatting.PersonNameFormat.Format(firstItem.Name);
				var creationArg = new DialogBoxCreationArgs(editor, title, null, DialogSizeHint.Large);

				LaunchAsDialog(this.Host.DesktopWindow, creationArg);
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void MergeContactPoint()
		{
			try
			{
				if(this.SelectedItems.Count > 1)
					return;

				var detail = GetSelectedExternalPractitionerDetail();
				var activeContactPoints = CollectionUtils.Select(detail.ContactPoints, contactPoint => !contactPoint.Deactivated);
				var exitCode = LaunchAsDialog(
					this.Host.DesktopWindow,
					new ExternalPractitionerContactPointMergeComponent(detail.PractitionerRef, activeContactPoints),
					SR.TitleMergeContactPoints);
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private ExternalPractitionerDetail GetSelectedExternalPractitionerDetail()
		{
			ExternalPractitionerDetail detail = null;
			var selectedPractitioner = CollectionUtils.FirstElement(this.SelectedItems);
			Platform.GetService<IExternalPractitionerAdminService>(service =>
			{
				var forEditResponse = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(selectedPractitioner.PractitionerRef));
				detail = forEditResponse.PractitionerDetail;
			});
			return detail;
		}

		private static bool CanMerge(ExternalPractitionerSummary practitioner)
		{
			return !practitioner.Deactivated && !practitioner.IsMerged;
		}
	}
}
