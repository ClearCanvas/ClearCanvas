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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Allows users to add/edit/delete user-defined worklists directly in the folder explorer.
    /// </summary>
	[ButtonAction("add", "folderexplorer-folders-toolbar/MenuNewWorklist", "Add")]
	[MenuAction("add", "folderexplorer-folders-contextmenu/MenuNewWorklist", "Add")]
	[IconSet("add", CrudActionModel.IconAddResource)]
	[Tooltip("add", "TooltipNewWorklist")]
	[EnabledStateObserver("add", "AddEnabled", "EnablementChanged")]
    [VisibleStateObserver("add", "Visible", "VisibleChanged")]

	[ButtonAction("edit", "folderexplorer-folders-toolbar/MenuEditWorklist", "Edit")]
	[MenuAction("edit", "folderexplorer-folders-contextmenu/MenuEditWorklist", "Edit")]
	[IconSet("edit", CrudActionModel.IconEditResource)]
	[Tooltip("edit", "TooltipEditWorklist")]
	[EnabledStateObserver("edit", "EditEnabled", "EnablementChanged")]
    [VisibleStateObserver("edit", "Visible", "VisibleChanged")]

	[ButtonAction("duplicate", "folderexplorer-folders-toolbar/MenuDuplicateWorklist", "Duplicate")]
	[MenuAction("duplicate", "folderexplorer-folders-contextmenu/MenuDuplicateWorklist", "Duplicate")]
	[IconSet("duplicate", "Icons.DuplicateSmall.png")]
	[Tooltip("duplicate", "TooltipDuplicateWorklist")]
    [EnabledStateObserver("duplicate", "DuplicateEnabled", "EnablementChanged")]
    [VisibleStateObserver("duplicate", "Visible", "VisibleChanged")]

	[ButtonAction("delete", "folderexplorer-folders-toolbar/MenuDeleteWorklist", "Delete")]
	[MenuAction("delete", "folderexplorer-folders-contextmenu/MenuDeleteWorklist", "Delete")]
	[IconSet("delete", CrudActionModel.IconDeleteResource)]
	[Tooltip("delete", "TooltipDeleteWorklist")]
	[EnabledStateObserver("delete", "DeleteEnabled", "EnablementChanged")]
    [VisibleStateObserver("delete", "Visible", "VisibleChanged")]

	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class UserWorklistTool : Tool<IFolderExplorerGroupToolContext>
    {
        #region Public API

        public bool Visible
        {
            get { return IsWorklistFolderSystem && (HasGroupAdminAuthority || HasPersonalAdminAuthority); }
        }

        public event EventHandler VisibleChanged
        {
            add { this.Context.SelectedFolderSystemChanged += value; }
            remove { this.Context.SelectedFolderSystemChanged -= value; }
        }


		public event EventHandler EnablementChanged
		{
			add
            {
                this.Context.SelectedFolderChanged += value;
                this.Context.SelectedFolderSystemChanged += value;
            }
			remove
            {
                this.Context.SelectedFolderChanged -= value;
                this.Context.SelectedFolderSystemChanged -= value;
            }
		}

		public bool AddEnabled
		{
			get { return CanAdd(); }
		}

		public bool EditEnabled
		{
			get { return CanEdit(this.Context.SelectedFolder); }
		}

        public bool DuplicateEnabled
        {
            get { return CanDuplicate(this.Context.SelectedFolder); }
        }

		public bool DeleteEnabled
		{
			get { return CanDelete(this.Context.SelectedFolder); }
		}

		public void Add()
		{
			if (!CanAdd())
				return;

			try
			{
                var fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;
                var folder = this.Context.SelectedFolder as IWorklistFolder;
                var initialWorklistClassName = folder == null ? null : folder.WorklistClassName;


                var editor = new WorklistEditorComponent(false, fs.SupportedWorklistClasses, initialWorklistClassName);
                var exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Context.DesktopWindow,
                    new DialogBoxCreationArgs(editor, SR.TitleNewWorklist, null, DialogSizeHint.Large));
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					AddNewWorklistsToFolderSystem(editor.EditedWorklistSummaries, fs);
				}

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		public void Edit()
		{
            var folder = (IWorklistFolder)this.Context.SelectedFolder;
            if (!CanEdit(folder))
                return;

			try
			{
				var editor = new WorklistEditorComponent(folder.WorklistRef, false);
				var exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Context.DesktopWindow,
					new DialogBoxCreationArgs(editor, string.Format("{0} - {1}", SR.TitleEditWorklist, folder.Name), null, DialogSizeHint.Large));
                if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					var worklist = CollectionUtils.FirstElement(editor.EditedWorklistSummaries);
					var fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;

					// update folder properties
					fs.UpdateWorklistFolder(folder, worklist);

					// refresh folder content
					fs.InvalidateFolder(folder);
				}

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

        public void Duplicate()
        {
            var folder = (IWorklistFolder)this.Context.SelectedFolder;
            if (!CanDuplicate(folder))
                return;

			try
			{
				var fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;
				var initialWorklistClassName = folder.WorklistClassName;

				var editor = new WorklistEditorComponent(folder.WorklistRef, false, fs.SupportedWorklistClasses, initialWorklistClassName);
				var exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Context.DesktopWindow,
					new DialogBoxCreationArgs(editor, SR.TitleNewWorklist, null, DialogSizeHint.Large));
                if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					AddNewWorklistsToFolderSystem(editor.EditedWorklistSummaries, fs);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		public void Delete()
		{
            var folder = (IWorklistFolder)this.Context.SelectedFolder;
            if (!CanDelete(folder))
                return;

            // confirm deletion
            if (this.Context.DesktopWindow.ShowMessageBox(
                SR.MessageConfirmDeleteSelectedWorklist, MessageBoxActions.OkCancel)
                != DialogBoxAction.Ok)
                return;

            try
            {
                // delete worklist
                Platform.GetService<IWorklistAdminService>(
                	service => service.DeleteWorklist(new DeleteWorklistRequest(folder.WorklistRef)));

                // if successful, remove folder from folder system
                var fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;
                fs.Folders.Remove(folder);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }

        #endregion

        #region Helpers

        private static void AddNewWorklistsToFolderSystem(IEnumerable<WorklistAdminSummary> worklists, IWorklistFolderSystem fs)
        {
			foreach (var worklist in worklists)
            {
                // try to add worklist to this folder system
                var folder = fs.AddWorklistFolder(worklist);

                // if add was successful, refresh the folder
                if (folder != null)
                {
                    fs.InvalidateFolder(folder);
                }
            }
        }

        private bool CanAdd()
		{
            return IsWorklistFolderSystem && (HasGroupAdminAuthority || HasPersonalAdminAuthority);
		}

		private static bool CanEdit(IFolder folder)
		{
            return CheckAccess(folder);
		}

        private bool CanDuplicate(IFolder folder)
        {
            // must be able to add, folder must be a worklist folder, and must not be static
            return CanAdd() && (folder is IWorklistFolder) && !folder.IsStatic;
        }

        private static bool CanDelete(IFolder folder)
		{
            return CheckAccess(folder);
        }

        /// <summary>
        /// Checks if the current user can modify/delete this folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool CheckAccess(IFolder folder)
        {
            var wf = folder as IWorklistFolder;

            // if not a worklist folder, or not user defined, can't edit it
            if (wf == null || wf.Ownership == WorklistOwnership.Admin)
                return false;

            // if staff owned, must have personal authority
            if (wf.Ownership == WorklistOwnership.Staff && !HasPersonalAdminAuthority)
                return false;

            // if group owned, must have group authority
            if (wf.Ownership == WorklistOwnership.Group && !HasGroupAdminAuthority)
                return false;

            return true;
        }

        private bool IsWorklistFolderSystem
        {
            get { return this.Context.SelectedFolderSystem is IWorklistFolderSystem; }
        }

        private static bool HasGroupAdminAuthority
        {
            get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Worklist.Group); }
        }

        private static bool HasPersonalAdminAuthority
        {
            get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Worklist.Personal); }
        }

        #endregion
    }
}
