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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ReportingMppsDocumentationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ReportingMppsDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ReportingMppsDocumentationComponent class responsible for making the MPPS performing documentation
	/// comments visible to the radiologist in the Reporting workspace.
	/// </summary>
	[AssociateView(typeof (ReportingMppsDocumentationComponentViewExtensionPoint))]
	public class ReportingMppsDocumentationComponent : ApplicationComponent, IReportingPage
	{
		[ExtensionOf(typeof(ReportingPageProviderExtensionPoint))]
		public class PageProvider : IReportingPageProvider
		{
			public IReportingPage[] GetPages(IReportingContext context)
			{
				return new IReportingPage[] { new ReportingMppsDocumentationComponent(context) };
			}
		}

		private readonly IReportingContext _reportingContext;
		private readonly PerformingDocumentationMppsSummaryTable _mppsTable = new PerformingDocumentationMppsSummaryTable();
		private ModalityPerformedProcedureStepDetail _selectedMpps;

		public ReportingMppsDocumentationComponent(IReportingContext context)
		{
			_reportingContext = context;
		}

		Path IExtensionPage.Path
		{
			get { return new Path("TitlePerformingDocumentation", new ResourceResolver(GetType().Assembly)); }
		}

		IApplicationComponent IExtensionPage.GetComponent()
		{
			return this;
		}

		public override void Start()
		{
			base.Start();

			LoadMppsForOrder();

			_reportingContext.WorklistItemChanged += WorklistItemChanged;
		}

		public ITable MppsTable
		{
			get { return _mppsTable; }
		}

		public ISelection SelectedMpps
		{
			get { return new Selection(_selectedMpps); }
			set
			{
				var selectedMpps = (ModalityPerformedProcedureStepDetail)value.Item;
				if (selectedMpps != _selectedMpps)
				{
					_selectedMpps = selectedMpps;
					NotifyPropertyChanged("SelectedMpps");
					NotifyPropertyChanged("MppsComments");
				}
			}
		}

		public string MppsComments
		{
			get
			{
				string comments;
				return _selectedMpps != null 
						&& _selectedMpps.ExtendedProperties != null
						&& _selectedMpps.ExtendedProperties.TryGetValue("Comments", out comments)
						? comments
						: null;
			}
		}

		private void WorklistItemChanged(object sender, EventArgs e)
		{
			try
			{
				LoadMppsForOrder();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Host.DesktopWindow);
			}
		}

		private void LoadMppsForOrder()
		{
			_mppsTable.Items.Clear();
			var order = _reportingContext.Order;
			if (order == null || order.OrderRef == null)
				return;

			Platform.GetService<IModalityWorkflowService>(
				service =>
				{
					var mppsRequest = new ListPerformedProcedureStepsRequest(order.OrderRef);
					var mppsResponse = service.ListPerformedProcedureSteps(mppsRequest);

					_mppsTable.Items.AddRange(mppsResponse.PerformedProcedureSteps);
					_mppsTable.Sort();
				});
		}

	}
}
