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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// This class provide common functionality for a edit report tool that either opens the report with or without images opening.
	/// </summary>
	/// <remarks>
	/// "With image" and "Without image" behaviour are specified by <see cref="EditReportWithImagesTool"/> and <see cref="EditReportWithoutImagesTool"/>
	/// respectively.
	/// </remarks>
	public abstract class EditReportToolBase : ReportingWorkflowItemTool
	{
		private readonly bool _loadImages;
		private readonly string _createReportTitle;
		private readonly string _editReportTitle;
		private readonly IconSet _createReportIcons;
		private readonly IconSet _editReportIcons;

		protected EditReportToolBase(bool loadImages, string createReportTitle, string editReportTitle, IconSet createReportIcons, IconSet editReportIcons)
			: base("EditReport")
		{
			_loadImages = loadImages;
			_createReportTitle = createReportTitle;
			_editReportTitle = editReportTitle;
			_createReportIcons = createReportIcons;
			_editReportIcons = editReportIcons;
		}

		public string Label
		{
			get
			{
				var item = GetSelectedItem();
				if (item == null)
					return _editReportTitle;

				if (item.ProcedureStepName == StepType.Interpretation && item.ActivityStatus.Code == StepState.Scheduled && item.ReportRef == null)
					return _createReportTitle;

				return _editReportTitle;
			}
		}

		public IconSet CurrentIconSet
		{
			get
			{
				var item = GetSelectedItem();
				if (item != null && item.ProcedureStepName == StepType.Interpretation && item.ActivityStatus.Code == StepState.Scheduled)
					return _createReportIcons;

				return _editReportIcons;
			}
		}

		public event EventHandler LabelChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		public override bool Enabled
		{
			get
			{
				ReportingWorklistItemSummary item = GetSelectedItem();

				if (this.Context.SelectedItems.Count != 1)
					return false;

				return
					this.Context.GetOperationEnablement("StartInterpretation") ||
					this.Context.GetOperationEnablement("StartTranscriptionReview") ||
					this.Context.GetOperationEnablement("StartVerification") ||

					// there is no specific workflow operation for editing a previously created draft,
					// so we enable the tool if it looks like a draft and SaveReport is enabled
					(this.Context.GetOperationEnablement("SaveReport") && item != null && item.ActivityStatus.Code == StepState.InProgress);
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItemSummary> items)
		{
			// this tool is only registered as a drop handler for the Drafts folder
			// and the only operation that would make sense in this context is StartInterpretation
			return this.Context.GetOperationEnablement("StartInterpretation");
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
			{
				// if we want to load images, and the document was already open, ensure the images are visible
				if (_loadImages)
				{
					var document = DocumentManager.Get<ReportDocument>(item.ProcedureStepRef);
					var component = document != null ? document.GetComponent() as ReportingComponent : null;
					if (component != null) component.EnsureImagesAreVisible();
				}
				return true;
			}

			// open the report editor
			OpenReportEditor(item, _loadImages);

			return true;
		}
	}

	/// <summary>
	/// An extension of <see cref="EditReportToolBase"/> that does not open images with the report.
	/// </summary>
	/// <remarks>
	/// This tool is not added by the extension mechanism, but instead indirectly added by the <see cref="EditReportTool"/>.
	/// </remarks>
	public class EditReportWithoutImagesTool : EditReportToolBase
	{
		public EditReportWithoutImagesTool()
			: base(
			false,
			SR.TitleCreateReport,
			SR.TitleEditReport,
			new IconSet("Icons.CreateReportSmall.png", "Icons.CreateReportMedium.png", "Icons.CreateReportMedium.png"),
			new IconSet("Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png"))
		{
		}
	}

	/// <summary>
	/// An extension of <see cref="EditReportToolBase"/> that does open images with the report.
	/// </summary>
	/// <remarks>
	/// This tool is not added by the extension mechanism, but instead indirectly added by the <see cref="EditReportTool"/>.
	/// </remarks>
	public class EditReportWithImagesTool : EditReportToolBase
	{
		public EditReportWithImagesTool()
			: base(
			true,
			SR.TitleCreateReportWithImages,
			SR.TitleEditReportWithImages,
			new IconSet("Icons.CreateReportWithImagesSmall.png", "Icons.CreateReportWithImagesMedium.png", "Icons.CreateReportWithImagesMedium.png"),
			new IconSet("Icons.EditReportWithImagesToolSmall.png", "Icons.EditReportWithImagesToolMedium.png", "Icons.EditReportWithImagesToolLarge.png"))
		{
		}
	}

	/// <summary>
	/// Adds "Edit/Create Report" and "Edit/Create Report and Open Images" tools to the reporting folder system.
	/// </summary>
	/// <remarks>
	/// This class adds both tools to the folder system context menu.  Additionally, it adds a drop-down button to the folder system tool bar with 
	/// drop down options for each tool;  selecting either drop down option will invoke the tool and also cause the tool bar button to perform the same 
	/// action, as well as change the folder system's double-click and drag-drop functions to invoke the same tool.  The selected drop-down tool 
	/// will also be saved in the <see cref="ReportingSettings.ShouldOpenImages"/> setting for each user, preserving the toolbar button, double-click,
	/// and drag-drop behaviour between sessions.
	/// 
	/// This class does not open the report itself; clickHandlers for the three actions are proxied to an instance of either a 
	/// <see cref="EditReportWithImagesTool"/> or <see cref="EditReportWithoutImagesTool"/> which take care of opening the report.
	/// </remarks>
	[DropDownButtonAction("group", "folderexplorer-items-toolbar/ToolbarEditReport", "ApplySelected", "EditReportMenuModel")]
	[EnabledStateObserver("group", "Enabled", "EnabledChanged")]
	[IconSetObserver("group", "GroupCurrentIconSet", "LabelChanged")]
	[LabelValueObserver("group", "GroupLabel", "LabelChanged")]
	[TooltipValueObserver("group", "GroupLabel", "LabelChanged")]

	[MenuAction("withImagesContext", "folderexplorer-items-contextmenu/MenuEditReportAndOpenImages", "ApplyWithImages")]
	[EnabledStateObserver("withImagesContext", "Enabled", "EnabledChanged")]
	[IconSetObserver("withImagesContext", "WithImagesIconSet", "LabelChanged")]
	[LabelValueObserver("withImagesContext", "WithImagesLabel", "LabelChanged")]

	[ButtonAction("withImagesToolbar", "editreport-toolbar-dropdown/MenuEditReportAndOpenImages", "ApplyWithImagesAndSetDefault")]
	[EnabledStateObserver("withImagesToolbar", "Enabled", "EnabledChanged")]
	[IconSetObserver("withImagesToolbar", "WithImagesIconSet", "LabelChanged")]
	[LabelValueObserver("withImagesToolbar", "WithImagesLabel", "LabelChanged")]
	[CheckedStateObserver("withImagesToolbar", "WithImagesChecked", "ActiveToolChanged")]

	[MenuAction("withoutImagesContext", "folderexplorer-items-contextmenu/MenuEditReport", "ApplyWithoutImages")]
	[EnabledStateObserver("withoutImagesContext", "Enabled", "EnabledChanged")]
	[IconSetObserver("withoutImagesContext", "WithoutImagesIconSet", "LabelChanged")]
	[LabelValueObserver("withoutImagesContext", "WithoutImagesLabel", "LabelChanged")]

	[ButtonAction("withoutImagesToolbar", "editreport-toolbar-dropdown/MenuEditReport", "ApplyWithoutImagesAndSetDefault")]
	[EnabledStateObserver("withoutImagesToolbar", "Enabled", "EnabledChanged")]
	[IconSetObserver("withoutImagesToolbar", "WithoutImagesIconSet", "LabelChanged")]
	[LabelValueObserver("withoutImagesToolbar", "WithoutImagesLabel", "LabelChanged")]
	[CheckedStateObserver("withoutImagesToolbar", "WithoutImagesChecked", "ActiveToolChanged")]

	[ActionPermission("group", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ActionPermission("withImagesToolbar",
		ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create,
		ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Images.View)]
	[ActionPermission("withImagesContext",
		ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create,
		ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Images.View)]
	[ActionPermission("withoutImagesToolbar", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ActionPermission("withoutImagesContext", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class EditReportTool : ReportingWorkflowItemTool
	{
		private readonly EditReportWithImagesTool _editReportWithImagesTool;
		private readonly EditReportWithoutImagesTool _editReportWithoutImagesTool;
		private EditReportToolBase _selectedTool;

		public EditReportTool()
			: base("EditReport")
		{
			_editReportWithImagesTool = new EditReportWithImagesTool();
			_editReportWithoutImagesTool = new EditReportWithoutImagesTool();
		}

		public override void Initialize()
		{
			base.Initialize();

			// manually add the two child tools.
			_editReportWithImagesTool.SetContext(this.Context);
			_editReportWithImagesTool.Initialize();

			_editReportWithoutImagesTool.SetContext(this.Context);
			_editReportWithoutImagesTool.Initialize();

			if (ReportingSettings.Default != null
				&& ReportingSettings.Default.ShouldOpenImages
				&& Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Images.View))
				SetActiveTool(_editReportWithImagesTool);
			else
				SetActiveTool(_editReportWithoutImagesTool);
		}

		/// Execute need not be implemented since no actions delegate to the Apply method on the <see cref="WorkflowItemTool{TItem,TContext}"/> 
		/// base class.
		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool Enabled
		{
			get
			{
				ReportingWorklistItemSummary item = GetSelectedItem();

				if (this.Context.SelectedItems.Count != 1)
					return false;

				return
					this.Context.GetOperationEnablement("StartInterpretation") ||
					this.Context.GetOperationEnablement("StartTranscriptionReview") ||
					this.Context.GetOperationEnablement("StartVerification") ||

					// there is no specific workflow operation for editing a previously created draft,
					// so we enable the tool if it looks like a draft and SaveReport is enabled
					(this.Context.GetOperationEnablement("SaveReport") && item != null && item.ActivityStatus.Code == StepState.InProgress);
			}
		}

		public event EventHandler LabelChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		public bool WithImagesChecked
		{
			get { return _selectedTool == _editReportWithImagesTool; }
		}

		public bool WithoutImagesChecked
		{
			get { return _selectedTool == _editReportWithoutImagesTool; }
		}

		#region Action delegates for top-level group button

		public void ApplySelected()
		{
			_selectedTool.Apply();
		}

		public ActionModelNode EditReportMenuModel
		{
			get { return ActionModelRoot.CreateModel(typeof(EditReportTool).FullName, "editreport-toolbar-dropdown", base.Actions); }
		}

		public string GroupLabel
		{
			get { return _selectedTool.Label; }
		}

		public IconSet GroupCurrentIconSet
		{
			get { return _selectedTool.CurrentIconSet; }
		}

		#endregion

		#region Action delegates for context menu and group sub-menu buttons

		public void ApplyWithImages()
		{
			_editReportWithImagesTool.Apply();
		}

		public void ApplyWithImagesAndSetDefault()
		{
			SetActiveTool(_editReportWithImagesTool);
			ApplyWithImages();
		}

		public void ApplyWithoutImages()
		{
			_editReportWithoutImagesTool.Apply();
		}

		public void ApplyWithoutImagesAndSetDefault()
		{
			SetActiveTool(_editReportWithoutImagesTool);
			ApplyWithoutImages();
		}

		public string WithImagesLabel
		{
			get { return _editReportWithImagesTool.Label; }
		}

		public IconSet WithImagesIconSet
		{
			get { return _editReportWithImagesTool.CurrentIconSet; }
		}

		public string WithoutImagesLabel
		{
			get { return _editReportWithoutImagesTool.Label; }
		}

		public IconSet WithoutImagesIconSet
		{
			get { return _editReportWithoutImagesTool.CurrentIconSet; }
		}

		#endregion

		public event EventHandler ActiveToolChanged;

		private void SetActiveTool(EditReportToolBase tool)
		{
			if (_selectedTool == tool)
				return;

			if (ReportingSettings.Default == null)
				return;

			_selectedTool = tool;

			if (_selectedTool == _editReportWithImagesTool)
			{
				ReportingSettings.Default.ShouldOpenImages = true;

				this.Context.UnregisterDropHandler(typeof(Folders.Reporting.DraftFolder), _editReportWithoutImagesTool);
				this.Context.RegisterDropHandler(typeof(Folders.Reporting.DraftFolder), _editReportWithImagesTool);

				this.Context.UnregisterDoubleClickHandler(
					(IClickAction)CollectionUtils.SelectFirst(
						this.Actions,
						a => a is IClickAction && a.ActionID.EndsWith("withoutImagesContext")));
				this.Context.RegisterDoubleClickHandler(
					(IClickAction)CollectionUtils.SelectFirst(
						this.Actions,
						a => a is IClickAction && a.ActionID.EndsWith("withImagesContext")));
			}
			else
			{
				ReportingSettings.Default.ShouldOpenImages = false;

				this.Context.UnregisterDropHandler(typeof(Folders.Reporting.DraftFolder), _editReportWithImagesTool);
				this.Context.RegisterDropHandler(typeof(Folders.Reporting.DraftFolder), _editReportWithoutImagesTool);

				this.Context.UnregisterDoubleClickHandler(
					(IClickAction)CollectionUtils.SelectFirst(
						this.Actions,
						a => a is IClickAction && a.ActionID.EndsWith("withImagesContext")));
				this.Context.RegisterDoubleClickHandler(
					(IClickAction)CollectionUtils.SelectFirst(
						this.Actions,
						a => a is IClickAction && a.ActionID.EndsWith("withoutImagesContext")));
			}

			ReportingSettings.Default.Save();

			EventsHelper.Fire(ActiveToolChanged, this, EventArgs.Empty);
		}
	}
}

