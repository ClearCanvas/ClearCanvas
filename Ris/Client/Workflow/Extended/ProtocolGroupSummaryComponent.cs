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
using ClearCanvas.Ris.Application.Extended.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    [MenuAction("launch", "global-menus/Admin/Protocol/Groups", "Launch")]
	[ActionPermission("launch", Application.Extended.Common.AuthorityTokens.Admin.Data.ProtocolGroups)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class ProtocolGroupSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    ProtocolGroupSummaryComponent component = new ProtocolGroupSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleProtocolGroups);
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
    /// Extension point for views onto <see cref="ProtocolGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolGroupSummaryComponent class
    /// </summary>
    public class ProtocolGroupSummaryComponent : SummaryComponentBase<ProtocolGroupSummary, ProtocolGroupTable, ListProtocolGroupsRequest>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolGroupSummaryComponent()
        {
        }

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(Application.Extended.Common.AuthorityTokens.Admin.Data.ProtocolGroups);
			model.Edit.SetPermissibility(Application.Extended.Common.AuthorityTokens.Admin.Data.ProtocolGroups);
			model.Delete.SetPermissibility(Application.Extended.Common.AuthorityTokens.Admin.Data.ProtocolGroups);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}
		
		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<ProtocolGroupSummary> ListItems(ListProtocolGroupsRequest request)
		{
			ListProtocolGroupsResponse listResponse = null;
			Platform.GetService<IProtocolAdminService>(
				delegate(IProtocolAdminService service)
				{
					listResponse = service.ListProtocolGroups(request);
				});

			return listResponse.ProtocolGroups;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ProtocolGroupSummary> addedItems)
		{
			addedItems = new List<ProtocolGroupSummary>();
			ProtocolGroupEditorComponent editor = new ProtocolGroupEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddProtocolGroup);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ProtocolGroupSummary);
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
		protected override bool EditItems(IList<ProtocolGroupSummary> items, out IList<ProtocolGroupSummary> editedItems)
		{
			editedItems = new List<ProtocolGroupSummary>();
			ProtocolGroupSummary item = CollectionUtils.FirstElement(items);

			ProtocolGroupEditorComponent editor = new ProtocolGroupEditorComponent(item.ProtocolGroupRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleEditProtocolGroup + " - " + item.Name);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ProtocolGroupSummary);
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
		protected override bool DeleteItems(IList<ProtocolGroupSummary> items, out IList<ProtocolGroupSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<ProtocolGroupSummary>();

			foreach (ProtocolGroupSummary item in items)
			{
				try
				{
					Platform.GetService<IProtocolAdminService>(
						delegate(IProtocolAdminService service)
						{
							service.DeleteProtocolGroup(new DeleteProtocolGroupRequest(item.ProtocolGroupRef));
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
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(ProtocolGroupSummary x, ProtocolGroupSummary y)
		{
			return x.ProtocolGroupRef.Equals(y.ProtocolGroupRef, true);
		}

    }
}
