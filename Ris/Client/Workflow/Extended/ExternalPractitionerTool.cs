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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public abstract class ExternalPractitionerTool :  Tool<IExternalPractitionerItemToolContext>
	{
		public virtual bool Enabled
		{
			get { return this.Context.SelectedItems.Count == 1; }
		}

		public event EventHandler EnabledChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Verify", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Verify", "Apply")]
	[Tooltip("apply", "Verify External Practitioner Information")]
	[IconSet("apply", "Icons.VerifyPractitionerToolSmall.png", "Icons.VerifyPractitionerToolMedium.png", "Icons.VerifyPractitionerToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	// Bug #7342: disabling this tool because it has been deemed too accessible and error prone, but leaving the code here just in case
	//[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerVerifyTool : ExternalPractitionerTool
	{
		public override bool Enabled
		{
			get
			{
				var item = (ExternalPractitionerSummary)this.Context.Selection.Item;
				return base.Enabled && item.IsVerified == false;
			}
		}

		public void Apply()
		{
			try
			{
				var item = (ExternalPractitionerSummary)this.Context.Selection.Item;

				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
						{
							var editRequest = new LoadExternalPractitionerForEditRequest(item.PractitionerRef);
							var editResponse = service.LoadExternalPractitionerForEdit(editRequest);

							var updateRequest = new UpdateExternalPractitionerRequest(editResponse.PractitionerDetail, true);
							service.UpdateExternalPractitioner(updateRequest);
						});

				DocumentManager.InvalidateFolder(typeof(UnverifiedFolder));
				DocumentManager.InvalidateFolder(typeof(VerifiedTodayFolder));
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Edit External Practitioner", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Edit External Practitioner", "Apply")]
	[Tooltip("apply", "Edit External Practitioner Information")]
	[IconSet("apply", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Update)]
	[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerEditTool : ExternalPractitionerTool
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(
					this.Actions,
					a => a is IClickAction && a.ActionID.EndsWith("apply")));
		}

		public void Apply()
		{
			var item = (ExternalPractitionerSummary)this.Context.Selection.Item;

			var editor = new ExternalPractitionerEditorComponent(item.PractitionerRef);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow, editor, SR.TitleUpdateExternalPractitioner + " - " + Formatting.PersonNameFormat.Format(item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				DocumentManager.InvalidateFolder(typeof(UnverifiedFolder));
				DocumentManager.InvalidateFolder(typeof(VerifiedTodayFolder));
			}
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Merge External Practitioners", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Merge External Practitioners", "Apply")]
	[Tooltip("apply", "Merge Duplicate External Practitioners")]
	[IconSet("apply", "Icons.MergePersonToolSmall.png", "Icons.MergePersonToolSmall.png", "Icons.MergePersonToolMedium.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
	[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerMergeTool : ExternalPractitionerTool
	{
		public override bool Enabled
		{
			get { return this.Context.SelectedItems.Count == 1 || this.Context.SelectedItems.Count == 2; }
		}

		public void Apply()
		{
			var firstItem = CollectionUtils.FirstElement(this.Context.SelectedItems);
			var secondItem = this.Context.SelectedItems.Count > 1 ? CollectionUtils.LastElement(this.Context.SelectedItems) : null;
			var editor = new ExternalPractitionerMergeNavigatorComponent(firstItem.PractitionerRef, secondItem == null ? null : secondItem.PractitionerRef);

			var title = SR.TitleMergePractitioner + " - " + Formatting.PersonNameFormat.Format(firstItem.Name);
			var creationArg = new DialogBoxCreationArgs(editor, title, null, DialogSizeHint.Large);

			var exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, creationArg);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				DocumentManager.InvalidateFolder(typeof(UnverifiedFolder));
			}
		}
	}
}
