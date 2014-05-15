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
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/MenuAdmin/MenuPatientNoteCategories", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory)]

	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class NoteCategorySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    NoteCategorySummaryComponent component = new NoteCategorySummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleNoteCategories);
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
    /// Extension point for views onto <see cref="NoteCategorySummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class NoteCategorySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// NoteCategorySummaryComponent class
    /// </summary>
	public class NoteCategorySummaryComponent : SummaryComponentBase<PatientNoteCategorySummary, NoteCategoryTable, ListAllNoteCategoriesRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategorySummaryComponent()
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

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}
		
		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<PatientNoteCategorySummary> ListItems(ListAllNoteCategoriesRequest request)
		{
			ListAllNoteCategoriesResponse listResponse = null;
			Platform.GetService<INoteCategoryAdminService>(
				delegate(INoteCategoryAdminService service)
				{
					listResponse = service.ListAllNoteCategories(request);
				});

			return listResponse.NoteCategories;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<PatientNoteCategorySummary> addedItems)
		{
			addedItems = new List<PatientNoteCategorySummary>();
			NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddNoteCategory);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.NoteCategorySummary);
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
		protected override bool EditItems(IList<PatientNoteCategorySummary> items, out IList<PatientNoteCategorySummary> editedItems)
		{
			editedItems = new List<PatientNoteCategorySummary>();
			PatientNoteCategorySummary item = CollectionUtils.FirstElement(items);

			NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent(item.NoteCategoryRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format(SR.FormatTitleSubtitle, SR.TitleUpdateNoteCategory, item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.NoteCategorySummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<PatientNoteCategorySummary> items, out IList<PatientNoteCategorySummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<PatientNoteCategorySummary>();

			foreach (PatientNoteCategorySummary item in items)
			{
				try
				{
					Platform.GetService<INoteCategoryAdminService>(
						delegate(INoteCategoryAdminService service)
						{
							service.DeleteNoteCategory(new DeleteNoteCategoryRequest(item.NoteCategoryRef));
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
		protected override bool UpdateItemsActivation(IList<PatientNoteCategorySummary> items, out IList<PatientNoteCategorySummary> editedItems)
		{
			List<PatientNoteCategorySummary> results = new List<PatientNoteCategorySummary>();
			foreach (PatientNoteCategorySummary item in items)
			{
				Platform.GetService<INoteCategoryAdminService>(
					delegate(INoteCategoryAdminService service)
					{
						PatientNoteCategoryDetail detail = service.LoadNoteCategoryForEdit(
							new LoadNoteCategoryForEditRequest(item.NoteCategoryRef)).NoteCategoryDetail;
						detail.Deactivated = !detail.Deactivated;
						PatientNoteCategorySummary summary = service.UpdateNoteCategory(
							new UpdateNoteCategoryRequest(detail)).NoteCategory;

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
		protected override bool IsSameItem(PatientNoteCategorySummary x, PatientNoteCategorySummary y)
		{
			return x.NoteCategoryRef.Equals(y.NoteCategoryRef, true);
		}
	}
}
