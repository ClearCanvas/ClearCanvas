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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("launch", "global-menus/MenuTools/MenuDowntime/MenuPrintDowntimeForms", "Launch")]
	[IconSet("launch", "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Downtime.PrintForms)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class DowntimePrintFormsTool : Tool<IDesktopToolContext>
	{
		public void Launch()
		{
			try
			{
				var component = new DowntimePrintFormsComponent();

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitlePrintDowntimeForms);
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="DowntimePrintFormsComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DowntimePrintFormsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DowntimePrintFormsComponent class
	/// </summary>
	[AssociateView(typeof(DowntimePrintFormsComponentViewExtensionPoint))]
	public class DowntimePrintFormsComponent : ApplicationComponent
	{
		public DowntimePrintFormsComponent()
		{
			NumberOfFormsToPrint = 1;
		}

		#region Presentation Model

		[ValidateGreaterThan(0)]
		[ValidateLessThan(50, Inclusive = true)]
		public int NumberOfFormsToPrint { get; set; }

		public void Print()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				DoPrintRequest();
				this.Exit(ApplicationComponentExitCode.None);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Close()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private void DoPrintRequest()
		{
			string path = null;

			var task = new BackgroundTask(
				delegate(IBackgroundTaskContext taskContext)
				{
					try
					{
						taskContext.ReportProgress(new BackgroundTaskProgress(0, SR.MessageGeneratingPdf));
						Platform.GetService<IOrderEntryService>(
							service =>
							{
								var data = service.PrintDowntimeForms(new PrintDowntimeFormsRequest{NumberOfForms = this.NumberOfFormsToPrint}).DowntimeFormPdfData;

								// we don't really care about the "key" here, or the time-to-live, since we won't be accesing this file ever again
								path = TempFileManager.Instance.CreateFile(new object(), "pdf", data, TimeSpan.FromMinutes(1));
							});

						taskContext.Complete();
					}
					catch (Exception e)
					{
						taskContext.Error(e);
					}
				}, false) { ThreadUICulture = Desktop.Application.CurrentUICulture };

			ProgressDialog.Show(task, this.Host.DesktopWindow, true, ProgressBarStyle.Marquee);

			if (path != null)
			{
				Process.Start(path);
			}
		}

	}
}
