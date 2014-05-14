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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuEnumerations", "Launch")]
	[ActionPermission("launch", Application.Common.AuthorityTokens.Admin.Data.Enumeration)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class EnumerationAdminTool : Tool<IDesktopToolContext>
	{
		private Workspace _workspace;

		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					var component = new EnumerationSummaryComponent();

					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						component,
						SR.TitleEnumerationAdmin);
					_workspace.Closed += delegate { _workspace = null; };

				}
				catch (Exception e)
				{
					// could not launch component
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
	/// Extension point for views onto <see cref="EnumerationSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class EnumerationSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// EnumerationSummaryComponent class
	/// </summary>
	[AssociateView(typeof(EnumerationSummaryComponentViewExtensionPoint))]
	public class EnumerationSummaryComponent : SummaryComponentBase<EnumValueAdminInfo, EnumValueAdminInfoTable>
	{
		private List<EnumerationSummary> _enumerations;
		private EnumerationSummary _selectedEnumeration;

		/// <summary>
		/// Constructor
		/// </summary>
		public EnumerationSummaryComponent()
		{
			_enumerations = new List<EnumerationSummary>();
		}

		public override void Start()
		{
			Platform.GetService(
				delegate(IEnumerationAdminService service)
				{
					var response = service.ListEnumerations(new ListEnumerationsRequest());
					_enumerations = response.Enumerations;
					_enumerations.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
				});

			_selectedEnumeration = CollectionUtils.FirstElement(_enumerations);

			base.Start();
		}

		#region Presentation Model

		public List<string> EnumerationChoices
		{
			get { return CollectionUtils.Map<EnumerationSummary, string, List<string>>(_enumerations, s => s.DisplayName); }
		}

		public string SelectedEnumeration
		{
			get { return _selectedEnumeration.DisplayName; }
			set
			{
				var summary = CollectionUtils.SelectFirst(_enumerations, s => s.DisplayName == value);

				if (_selectedEnumeration == summary)
					return;

				_selectedEnumeration = summary;

				LoadEnumerationValues();

				UpdateOperationEnablement();
				NotifyPropertyChanged("SelectedEnumeration");
				NotifyPropertyChanged("SelectedEnumerationClassName");
			}
		}

		public string SelectedEnumerationClassName
		{
			get { return _selectedEnumeration == null ? null : _selectedEnumeration.AssemblyQualifiedClassName; }
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Enumeration);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Enumeration);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Enumeration);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Enumeration);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		protected override bool SupportsPaging
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<EnumValueAdminInfo> ListItems(int firstItem, int maxItems)
		{
			var listResponse = new ListEnumerationValuesResponse();
			if (_selectedEnumeration != null)
			{
				Platform.GetService(
					delegate(IEnumerationAdminService service)
					{
						var request = new ListEnumerationValuesRequest(_selectedEnumeration.AssemblyQualifiedClassName) { IncludeDeactivated = true };
						listResponse = service.ListEnumerationValues(request);
					});
			}

			return listResponse.Values;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<EnumValueAdminInfo> addedItems)
		{
			// Assign value to addedItems, but we actually don't use this
			// because the entire table need to be refreshed after changes to any enumValueInfo item
			addedItems = new List<EnumValueAdminInfo>();

			var component = new EnumerationEditorComponent(_selectedEnumeration.AssemblyQualifiedClassName, this.Table.Items);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleEnumAddValue))
			{
				// refresh entire table
				LoadEnumerationValues();
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
		protected override bool EditItems(IList<EnumValueAdminInfo> items, out IList<EnumValueAdminInfo> editedItems)
		{
			// Assign value to addedItems, but we actually don't use this
			// because the entire table need to be refreshed after changes to any enumValueInfo item
			editedItems = new List<EnumValueAdminInfo>();

			var item = CollectionUtils.FirstElement(items);
			var title = string.Format("{0} - {1}", SR.TitleEnumEditValue, item.Code);
			var component = new EnumerationEditorComponent(
				_selectedEnumeration.AssemblyQualifiedClassName,
				(EnumValueAdminInfo)item.Clone(),
				this.Table.Items);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, component, title))
			{
				// refresh entire table
				LoadEnumerationValues();
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
		protected override bool DeleteItems(IList<EnumValueAdminInfo> items, out IList<EnumValueAdminInfo> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<EnumValueAdminInfo>();

			foreach (var item in items)
			{
				try
				{
					var enumValue = item;
					Platform.GetService<IEnumerationAdminService>(
						service => service.RemoveValue(new RemoveValueRequest(_selectedEnumeration.AssemblyQualifiedClassName, enumValue)));

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
		protected override bool UpdateItemsActivation(IList<EnumValueAdminInfo> items, out IList<EnumValueAdminInfo> editedItems)
		{
			var results = new List<EnumValueAdminInfo>();
			foreach (var item in items)
			{
				var enumValue = item;
				Platform.GetService(
					delegate(IEnumerationAdminService service)
					{
						enumValue.Deactivated = !enumValue.Deactivated;

						// this is kind of annoying, but the way the service interface is designed, we need to know
						// who to insert after in order to update the value
						var index = this.Table.Items.IndexOf(enumValue);
						var insertAfter = index > 0 ? this.Table.Items[index - 1] : null;
						service.EditValue(new EditValueRequest(_selectedEnumeration.AssemblyQualifiedClassName, enumValue, insertAfter));

						results.Add(enumValue);
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
		protected override bool IsSameItem(EnumValueAdminInfo x, EnumValueAdminInfo y)
		{
			return Equals(x.Code, y.Code);
		}

		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			UpdateOperationEnablement();
		}

		#endregion

		private void LoadEnumerationValues()
		{
			this.Table.Items.Clear();
			this.Table.Items.AddRange(ListItems(0, -1));
		}

		private void UpdateOperationEnablement()
		{
			// overriding base behaviour
			if (_selectedEnumeration == null)
			{
				this.ActionModel.Add.Enabled = false;
				this.ActionModel.Edit.Enabled = false;
				this.ActionModel.Delete.Enabled = false;
				this.ActionModel.ToggleActivation.Enabled = false;
			}
			else
			{
				this.ActionModel.Add.Enabled = _selectedEnumeration.CanAddRemoveValues;
				this.ActionModel.Edit.Enabled = this.SelectedItems.Count == 1;
				this.ActionModel.Delete.Enabled = this.SelectedItems.Count > 0 && _selectedEnumeration.CanAddRemoveValues;
				this.ActionModel.ToggleActivation.Enabled = this.SelectedItems.Count > 0 && _selectedEnumeration.CanAddRemoveValues;
			}
		}
	}
}
