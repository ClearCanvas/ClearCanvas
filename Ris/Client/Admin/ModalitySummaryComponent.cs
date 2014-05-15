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
using ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/MenuAdmin/MenuModalities", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Modality)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class ModalitySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    ModalitySummaryComponent component = new ModalitySummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleModalities);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch editor
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
    /// Extension point for views onto <see cref="ModalitySummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ModalitySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ModalitySummaryComponent class
    /// </summary>
    public class ModalitySummaryComponent : SummaryComponentBase<ModalitySummary, ModalityTable, ListAllModalitiesRequest>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalitySummaryComponent()
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

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Modality);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Modality);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Modality);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Modality);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}
		
		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<ModalitySummary> ListItems(ListAllModalitiesRequest request)
		{
			ListAllModalitiesResponse listResponse = null;
			Platform.GetService<IModalityAdminService>(
				delegate(IModalityAdminService service)
				{
					listResponse = service.ListAllModalities(request);
				});

			return listResponse.Modalities;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ModalitySummary> addedItems)
		{
			addedItems = new List<ModalitySummary>();
			ModalityEditorComponent editor = new ModalityEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddModality);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ModalitySummary);
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
		protected override bool EditItems(IList<ModalitySummary> items, out IList<ModalitySummary> editedItems)
		{
			editedItems = new List<ModalitySummary>();
			ModalitySummary item = CollectionUtils.FirstElement(items);

			ModalityEditorComponent editor = new ModalityEditorComponent(item.ModalityRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format(SR.FormatTitleCodeSubtitle, SR.TitleUpdateModality, item.Id, item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ModalitySummary);
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
		protected override bool DeleteItems(IList<ModalitySummary> items, out IList<ModalitySummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<ModalitySummary>();

			foreach (ModalitySummary item in items)
			{
				try
				{
					Platform.GetService<IModalityAdminService>(
						delegate(IModalityAdminService service)
						{
							service.DeleteModality(new DeleteModalityRequest(item.ModalityRef));
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
		protected override bool UpdateItemsActivation(IList<ModalitySummary> items, out IList<ModalitySummary> editedItems)
		{
			List<ModalitySummary> results = new List<ModalitySummary>();
			foreach (ModalitySummary item in items)
			{
				Platform.GetService<IModalityAdminService>(
					delegate(IModalityAdminService service)
					{
						ModalityDetail detail = service.LoadModalityForEdit(
							new LoadModalityForEditRequest(item.ModalityRef)).ModalityDetail;
						detail.Deactivated = !detail.Deactivated;
						ModalitySummary summary = service.UpdateModality(
							new UpdateModalityRequest(detail)).Modality;

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
		protected override bool IsSameItem(ModalitySummary x, ModalitySummary y)
		{
			return x.ModalityRef.Equals(y.ModalityRef, true);
		}
	}
}
