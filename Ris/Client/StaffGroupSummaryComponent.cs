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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuStaffGroups", "Launch")]
	[ActionPermission("launch", Application.Common.AuthorityTokens.Admin.Data.StaffGroup)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class StaffGroupAdminTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    var component = new StaffGroupSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleStaffGroups);
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
    
    public class StaffGroupSummaryTable : Table<StaffGroupSummary>
    {
        public StaffGroupSummaryTable()
        {
            this.Columns.Add(new TableColumn<StaffGroupSummary, string>(SR.ColumnStaffGroupName, item => item.Name, 0.5f));
            this.Columns.Add(new TableColumn<StaffGroupSummary, string>(SR.ColumnDescription, item => item.Description, 1.0f));
			this.Columns.Add(new TableColumn<StaffGroupSummary, bool>(SR.ColumnElective, item => item.IsElective, 0.2f));
		}
    }

    /// <summary>
    /// StaffGroupSummaryComponent class
    /// </summary>
    public class StaffGroupSummaryComponent : SummaryComponentBase<StaffGroupSummary, StaffGroupSummaryTable, ListStaffGroupsRequest>
    {
    	private readonly string _initialFilterText;
    	private readonly bool _electiveGroupsOnly;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffGroupSummaryComponent()
        {
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dialogMode"></param>
		/// <param name="intialFilterText"></param>
		/// <param name="electiveGroupsOnly"></param>
		public StaffGroupSummaryComponent(bool dialogMode, string intialFilterText, bool electiveGroupsOnly)
			:base(dialogMode)
		{
			_initialFilterText = intialFilterText;
			_electiveGroupsOnly = electiveGroupsOnly;
		}

    	/// <summary>
    	/// Initializes the table.  Override this method to perform custom initialization of the table,
    	/// such as adding columns or setting other properties that control the table behaviour.
    	/// </summary>
    	/// <param name="table"></param>
    	protected override void InitializeTable(StaffGroupSummaryTable table)
		{
			base.InitializeTable(table);

			if (!string.IsNullOrEmpty(_initialFilterText))
			{
				table.Filter(new TableFilterParams(null, _initialFilterText));
			}
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
			model.Edit.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
			model.Delete.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
			model.ToggleActivation.SetPermissibility(Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
        /// Gets the list of items to show in the table, according to the specifed first and max items.
        /// </summary>
        /// <returns></returns>
        protected override IList<StaffGroupSummary> ListItems(ListStaffGroupsRequest request)
        {
			request.ElectiveGroupsOnly = _electiveGroupsOnly;

            ListStaffGroupsResponse listResponse = null;
            Platform.GetService<IStaffGroupAdminService>(
            	service => listResponse = service.ListStaffGroups(request));

            return listResponse.StaffGroups;
        }

        /// <summary>
        /// Called to handle the "add" action.
        /// </summary>
        /// <param name="addedItems"></param>
        /// <returns>True if items were added, false otherwise.</returns>
        protected override bool AddItems(out IList<StaffGroupSummary> addedItems)
        {
            addedItems = new List<StaffGroupSummary>();
            var editor = new StaffGroupEditorComponent();
            var exitCode = LaunchAsDialog(
                this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, SR.TitleAddStaffGroup, null, DialogSizeHint.Large));
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                addedItems.Add(editor.StaffGroupSummary);
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
        protected override bool EditItems(IList<StaffGroupSummary> items, out IList<StaffGroupSummary> editedItems)
        {
            editedItems = new List<StaffGroupSummary>();
            var item = CollectionUtils.FirstElement(items);

            var editor = new StaffGroupEditorComponent(item.StaffGroupRef);
            var exitCode = LaunchAsDialog(
                this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, SR.TitleEditStaffGroup + item.Name, null, DialogSizeHint.Large));
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                editedItems.Add(editor.StaffGroupSummary);
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
		protected override bool DeleteItems(IList<StaffGroupSummary> items, out IList<StaffGroupSummary> deletedItems, out string failureMessage)
        {
			failureMessage = null;
			deletedItems = new List<StaffGroupSummary>();

			foreach (var item in items)
			{
				try
				{
					Platform.GetService<IStaffGroupAdminService>(
						service => service.DeleteStaffGroup(new DeleteStaffGroupRequest(item.StaffGroupRef)));

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
		protected override bool UpdateItemsActivation(IList<StaffGroupSummary> items, out IList<StaffGroupSummary> editedItems)
		{
			var results = new List<StaffGroupSummary>();
			foreach (var item in items)
			{
				Platform.GetService<IStaffGroupAdminService>(
					service =>
					{
						var detail = service.LoadStaffGroupForEdit(
							new LoadStaffGroupForEditRequest(item.StaffGroupRef)).StaffGroup;
						detail.Deactivated = !detail.Deactivated;
						var summary = service.UpdateStaffGroup(
							new UpdateStaffGroupRequest(detail)).StaffGroup;

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
        protected override bool IsSameItem(StaffGroupSummary x, StaffGroupSummary y)
        {
            return x.StaffGroupRef.Equals(y.StaffGroupRef, true);
        }
    }
}
