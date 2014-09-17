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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="PrintReportComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class PrintReportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PrintReportComponent class.
	/// </summary>
	[AssociateView(typeof(PrintReportComponentViewExtensionPoint))]
	public class PrintReportComponent : ApplicationComponent
	{
		private readonly EntityRef _orderRef;
		private readonly EntityRef _reportRef;

		private ExternalPractitionerLookupHandler _recipientLookupHandler;
		private ExternalPractitionerSummary _selectedRecipient;
		private ExternalPractitionerContactPointDetail _selectedContactPoint;
		private List<ExternalPractitionerContactPointDetail> _recipientContactPointChoices;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PrintReportComponent(EntityRef orderRef, EntityRef reportRef)
		{
			_orderRef = orderRef;
			_reportRef = reportRef;
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_recipientLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

			Platform.GetService(
				delegate(IBrowsePatientDataService service)
				{
					var request = new GetDataRequest { GetOrderDetailRequest = new GetOrderDetailRequest(_orderRef, false, false, false, false, false, true) };
					var order = service.GetData(request).GetOrderDetailResponse.Order;

					_selectedRecipient = order.OrderingPractitioner;

					var recipient = order.ResultRecipients.FirstOrDefault(
						rr => rr.Practitioner.PractitionerRef.Equals(_selectedRecipient.PractitionerRef, true));

					_selectedContactPoint = recipient != null ? recipient.ContactPoint : null;
				});

			UpdateConsultantContactPointChoices();


			base.Start();
		}

		#region Presentation model

		public ILookupHandler RecipientsLookupHandler
		{
			get { return _recipientLookupHandler; }
		}

		[ValidateNotNull]
		public ExternalPractitionerSummary SelectedRecipient
		{
			get { return _selectedRecipient; }
			set
			{
				if (Equals(value, _selectedRecipient))
					return;

				_selectedRecipient = value;
				NotifyPropertyChanged("SelectedRecipient");

				_selectedContactPoint = null;
				UpdateConsultantContactPointChoices();
				NotifyPropertyChanged("RecipientContactPointChoices");
			}
		}

		public IList RecipientContactPointChoices
		{
			get { return _recipientContactPointChoices; }
		}

		public string FormatContactPoint(object cp)
		{
			return ExternalPractitionerContactPointFormat.Format((ExternalPractitionerContactPointDetail)cp);
		}

		[ValidateNotNull]
		public ExternalPractitionerContactPointDetail SelectedContactPoint
		{
			get { return _selectedContactPoint; }
			set
			{
				if (_selectedContactPoint == value)
					return;

				_selectedContactPoint = value;
				NotifyPropertyChanged("SelectedContactPoint");
			}
		}

		public bool CloseOnPrint
		{
			get { return PrintReportComponentSettings.Default.CloseOnPrint; }
			set
			{
				if(PrintReportComponentSettings.Default.CloseOnPrint != value)
				{
					PrintReportComponentSettings.Default.CloseOnPrint = value;
					PrintReportComponentSettings.Default.Save();
				}
			}
		}

		public void Print()
		{
			if(this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				DoPrintRequest();

				if (PrintReportComponentSettings.Default.CloseOnPrint)
				{
					this.Exit(ApplicationComponentExitCode.None);
				}
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

		#region private methods

		private void UpdateConsultantContactPointChoices()
		{
			_recipientContactPointChoices = GetPractitionerContactPoints(_selectedRecipient);
		}

		private static List<ExternalPractitionerContactPointDetail> GetPractitionerContactPoints(ExternalPractitionerSummary prac)
		{
			var choices = new List<ExternalPractitionerContactPointDetail>();
			if (prac != null)
			{
				Platform.GetService(
					delegate(IOrderEntryService service)
					{
						var response = service.GetExternalPractitionerContactPoints(
							new GetExternalPractitionerContactPointsRequest(prac.PractitionerRef));
						choices = response.ContactPoints;
					});
			}

			return choices;
		}

		private void DoPrintRequest()
		{
			string path = null;

			var task = new BackgroundTask(
				delegate(IBackgroundTaskContext taskContext)
					{
						try
						{
							taskContext.ReportProgress(new BackgroundTaskProgress(0, SR.MessageGeneratingPdf));
							Platform.GetService<IReportingWorkflowService>(
								service =>
								{
									var request = new PrintReportRequest(_reportRef) { RecipientContactPointRef = _selectedContactPoint.ContactPointRef };
									var data = service.PrintReport(request).ReportPdfData;

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

			if(path != null)
			{
				Process.Start(path);
			}
		}


		#endregion
	}
}
