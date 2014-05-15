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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Images.View)]
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuViewImages", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuViewImages", "Apply")]
	[ButtonAction("apply", "biography-reports-toolbar/MenuViewImages", "Apply")]
	[IconSet("apply", "Icons.ViewImagesSmall.png", "Icons.ViewImagesMedium.png", "Icons.ViewImagesLarge.png")]
	[Tooltip("apply", "TooltipViewImages")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BiographyOrderReportsToolExtensionPoint))]
	public class ViewImagesTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// A no-args constructor is required by the framework.  Do not remove.
		/// </remarks>
		public ViewImagesTool()
		{
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				((IReportingWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IPerformingWorkflowItemToolContext)
			{
				((IPerformingWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IBiographyOrderReportsToolContext)
			{
				((IBiographyOrderReportsToolContext)this.ContextBase).ContextChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

		public bool Visible
		{
			get { return ViewImagesHelper.IsSupported; }
		}

		/// <summary>
		/// Notifies that the <see cref="Enabled"/> state of this tool has changed.
		/// </summary>
		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				return (((IReportingWorkflowItemToolContext)this.ContextBase).SelectedItems != null
					&& ((IReportingWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}

			if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				return (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
						&& ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}

			if (this.ContextBase is IPerformingWorkflowItemToolContext)
			{
				return (((IPerformingWorkflowItemToolContext)this.ContextBase).SelectedItems != null
						&& ((IPerformingWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}

			if (this.ContextBase is IBiographyOrderReportsToolContext)
			{
				var context = (IBiographyOrderReportsToolContext)this.ContextBase;
				return !string.IsNullOrEmpty(context.AccessionNumber);
			}

			return false;
		}

		/// <summary>
		/// Gets whether this tool is enabled/disabled in the UI.
		/// </summary>
		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Notifies that the <see cref="Enabled"/> state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void Apply()
		{
			if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				var context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
				OpenViewer((WorklistItemSummaryBase)context.Selection.Item, context.DesktopWindow);
			}
			else if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				var context = (IReportingWorkflowItemToolContext)this.ContextBase;
				OpenViewer((WorklistItemSummaryBase)context.Selection.Item, context.DesktopWindow);
			}
			else if (this.ContextBase is IPerformingWorkflowItemToolContext)
			{
				var context = (IPerformingWorkflowItemToolContext)this.ContextBase;
				OpenViewer((WorklistItemSummaryBase)context.Selection.Item, context.DesktopWindow);
			}
			else if (this.ContextBase is IBiographyOrderReportsToolContext)
			{
				var context = (IBiographyOrderReportsToolContext)this.ContextBase;
				OpenViewer(context.OrderRef, context.DesktopWindow);
			}
		}

		private static void OpenViewer(WorklistItemSummaryBase item, IDesktopWindow desktopWindow)
		{
			if (item == null)
				return;
			if (!CheckSupported(desktopWindow))
				return;

			try
			{
				// first try opening a viewer for this specific procedure
				ViewImagesHelper.ViewStudies(item.OrderRef, new[]{item.ProcedureRef},
					new ViewImagesHelper.ViewStudiesOptions{FallbackToOrder = true, Interactive = true, DesktopWindow = desktopWindow});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}

		private static void OpenViewer(EntityRef orderRef, IDesktopWindow desktopWindow)
		{
			if (!CheckSupported(desktopWindow))
				return;

			try
			{
				ViewImagesHelper.ViewStudies(orderRef,
					new ViewImagesHelper.ViewStudiesOptions { Interactive = true, DesktopWindow = desktopWindow });
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}

		private static bool CheckSupported(IDesktopWindow desktopWindow)
		{
			if (!ViewImagesHelper.IsSupported)
			{
				// this should not happen because the tool will be invisible
				desktopWindow.ShowMessageBox(SR.MessageNoViewerSupport, MessageBoxActions.Ok);
				return false;
			}
			return true;
		}
	}
}
