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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("neworder", "folderexplorer-items-contextmenu/MenuNewOrder", "NewOrder")]
	[ButtonAction("neworder", "folderexplorer-items-toolbar/MenuNewOrder", "NewOrder")]
	[ButtonAction("neworder", "patientsearch-items-toolbar/MenuNewOrder", "NewOrder")]
	[MenuAction("neworder", "patientsearch-items-contextmenu/MenuNewOrder", "NewOrder")]
	[IconSet("neworder", "NewOrderSmall.png", "NewOrderMedium.png", "NewOrderLarge.png")]
	[ActionPermission("neworder", Application.Common.AuthorityTokens.Workflow.Order.Create)]

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	public class NewOrderTool : Tool<IToolContext>
	{
		public void NewOrder()
		{
			if (this.Context is IRegistrationWorkflowItemToolContext)
			{
				var context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
				var item = CollectionUtils.FirstElement(context.SelectedItems);
				NewOrder(item, context.DesktopWindow);
			}
			else if (this.Context is IPatientSearchToolContext)
			{
				var context = (IPatientSearchToolContext)this.ContextBase;
				NewOrder(context.SelectedProfile, context.DesktopWindow);
			}
			else if (this.Context is IPatientBiographyToolContext)
			{
				var context = (IPatientBiographyToolContext)this.ContextBase;
				NewOrder(context.PatientProfile.GetSummary(), context.DesktopWindow);
			}
		}

		private static void NewOrder(WorklistItemSummaryBase worklistItem, IDesktopWindow desktopWindow)
		{
			if(worklistItem == null)
			{
				NewOrder(null, "New Order", desktopWindow);
				return;
			}

			PatientProfileSummary summary = null;
			Platform.GetService<IBrowsePatientDataService>(
				service =>
				{
					var response = service.GetData(new GetDataRequest
													{
														GetPatientProfileDetailRequest = new GetPatientProfileDetailRequest
																							{
																								PatientProfileRef = worklistItem.PatientProfileRef
																							}
													});
					summary = response.GetPatientProfileDetailResponse.PatientProfile.GetSummary();
				});

			NewOrder(summary, desktopWindow);
		}

		private static void NewOrder(PatientProfileSummary patientProfile, IDesktopWindow desktopWindow)
		{
			if(patientProfile == null)
			{
				NewOrder(null, "New Order", desktopWindow);
				return;
			}

			var title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(patientProfile.Name), MrnFormat.Format(patientProfile.Mrn));
			NewOrder(patientProfile, title, desktopWindow);
		}

		private static void NewOrder(PatientProfileSummary patientProfile, string title, IDesktopWindow desktopWindow)
		{
			try
			{
				var component = new OrderEditorComponent(new OrderEditorComponent.NewOrderOperatingContext {PatientProfile = patientProfile});
				var result = ApplicationComponent.LaunchAsDialog(
					desktopWindow,
					component,
					title);

				if (result == ApplicationComponentExitCode.Accepted)
				{
					DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}
	}
}
