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
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Images.View)]
	[MenuAction("apply", "folderexplorer-items-contextmenu/View Images", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Verify", "Apply")]
	[ButtonAction("apply", "biography-reports-toolbar/View Images", "Apply")]
	[IconSet("apply", "Icons.ViewImagesSmall.png", "Icons.ViewImagesMedium.png", "Icons.ViewImagesLarge.png")]
	[Tooltip("apply", "View Images")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
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

			if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IOrderNoteboxItemToolContext)
			{
				((IOrderNoteboxItemToolContext)this.ContextBase).SelectionChanged += delegate
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
			if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				return (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
						&& ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}

			if (this.ContextBase is IOrderNoteboxItemToolContext)
			{
				return (((IOrderNoteboxItemToolContext)this.ContextBase).SelectedItems != null
						&& ((IOrderNoteboxItemToolContext)this.ContextBase).SelectedItems.Count == 1);
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
			else if (this.ContextBase is IOrderNoteboxItemToolContext)
			{
				var context = (IOrderNoteboxItemToolContext)this.ContextBase;
				OpenViewer((OrderNoteboxItemSummary)context.Selection.Item, context.DesktopWindow);
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
				ViewImagesHelper.ViewStudies(item.OrderRef, new[] { item.ProcedureRef },
					new ViewImagesHelper.ViewStudiesOptions { FallbackToOrder = true, Interactive = true, DesktopWindow = desktopWindow });
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}

		private static void OpenViewer(OrderNoteboxItemSummary item, IDesktopWindow desktopWindow)
		{
			if (item == null)
				return;
			if (!CheckSupported(desktopWindow))
				return;

			try
			{
				OpenViewer(item.OrderRef, desktopWindow);
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
				desktopWindow.ShowMessageBox("No image viewing support.", MessageBoxActions.Ok);
				return false;
			}
			return true;
		}
	}

	[ButtonAction("view", "folderexplorer-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/Patient Biography", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/Patient Biography", "View")]
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "Open patient biography")]
	[IconSet("view", "PatientDetailsToolSmall.png", "PatientDetailsToolMedium.png", "PatientDetailsToolLarge.png")]
	[ActionPermission("view", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientBiography.View)]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	public class PatientBiographyTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			if (this.ContextBase is IWorkflowItemToolContext)
			{
				((IWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IWorkflowItemToolContext)
			{
				var ctx = (IWorkflowItemToolContext)this.ContextBase;
				return ctx.Selection != null && ctx.Selection.Items.Length == 1;
			}

			return false;
		}

		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void View()
		{
			if (this.ContextBase is IWorkflowItemToolContext)
			{
				var context = (IWorkflowItemToolContext)ContextBase;
				if (this.Context is IOrderNoteboxItemToolContext)
				{
					var item = (OrderNoteboxItemSummary)context.Selection.Item;

					// this is super-hokey, but it should work as long as we fill out
					// the key properties
					var worklistItem = new WorklistItemSummaryBase
										{
											AccessionNumber = item.AccessionNumber,
											DiagnosticServiceName = item.DiagnosticServiceName,
											Mrn = item.Mrn,
											OrderRef = item.OrderRef,
											PatientName = item.PatientName,
											PatientProfileRef = item.PatientProfileRef,
											PatientRef = item.PatientRef,

										};

					OpenPatient(worklistItem, context.DesktopWindow);
				}
				else
				{
					var item = (WorklistItemSummaryBase)context.Selection.Item;
					OpenPatient(item, context.DesktopWindow);
				}
			}
		}

		protected static void OpenPatient(WorklistItemSummaryBase worklistItem, IDesktopWindow window)
		{
			try
			{
				var document = DocumentManager.Get<PatientBiographyDocument>(worklistItem.PatientRef);
				if (document == null)
				{
					document = new PatientBiographyDocument(worklistItem, window);
					document.Open();
				}
				else
				{
					document.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, window);
			}
		}
	}

}
