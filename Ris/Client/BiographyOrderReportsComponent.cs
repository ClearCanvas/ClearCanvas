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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyOrderReportsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class BiographyOrderReportsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[ExtensionPoint]
	public class BiographyOrderReportsToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IBiographyOrderReportsToolContext : IToolContext
	{
		EntityRef PatientRef { get; }
		EntityRef PatientProfileRef { get; }
		EntityRef OrderRef { get; }
		EntityRef ProcedureRef { get; }
		EntityRef ReportRef { get; }
		EnumValueInfo ReportStatus { get; }
		string AccessionNumber { get; }
		IDesktopWindow DesktopWindow { get; }
		event EventHandler ContextChanged;
	}

	/// <summary>
	/// BiographyOrderReportsComponent class.
	/// </summary>
	[AssociateView(typeof(BiographyOrderReportsComponentViewExtensionPoint))]
	public class BiographyOrderReportsComponent : ApplicationComponent
	{
		public class BiographyOrderReportsToolContext : ToolContext, IBiographyOrderReportsToolContext
		{
			private readonly BiographyOrderReportsComponent _component;

			internal BiographyOrderReportsToolContext(BiographyOrderReportsComponent component)
			{
				_component = component;
			}

			#region IBiographyOrderReportsToolContext Members

			public EntityRef PatientRef
			{
				get { return _component.PatientRef; }
			}

			public EntityRef PatientProfileRef
			{
				get { return _component.PatientProfileRef; }
			}

			public EntityRef OrderRef
			{
				get { return _component.OrderRef; }
			}

			public EntityRef ProcedureRef
			{
				get { return _component.ProcedureRef; }
			}

			public EntityRef ReportRef
			{
				get { return _component.ReportRef; }
			}

			public EnumValueInfo ReportStatus
			{
				get { return _component.ReportStatus; }
			}

			public string AccessionNumber
			{
				get { return _component.AccessionNumber; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public event EventHandler ContextChanged
			{
				add { _component.ReportSelectionChanged += value; }
				remove { _component.ReportSelectionChanged -= value; }
			}

			#endregion
		}

		class ReportPreviewComponent : DHtmlComponent
		{
			// Internal data contract used for jscript deserialization
			[DataContract]
			public class ReportPreviewContext : DataContractBase
			{
				public ReportPreviewContext(EntityRef reportRef)
				{
					this.ReportRef = reportRef;
				}

				[DataMember]
				public EntityRef ReportRef;
			}

			private ReportPreviewContext _previewContext;

			public ReportPreviewComponent(EntityRef reportRef)
			{
				_previewContext = reportRef != null ? new ReportPreviewContext(reportRef) : null;
			}

			public override void Start()
			{
				SetUrl(WebResourcesSettings.Default.BiographyReportDetailPageUrl);
				base.Start();
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _previewContext;
			}

			public ReportPreviewContext PreviewContext
			{
				get { return _previewContext; }
				set
				{
					_previewContext = value;
					Refresh();
				}
			}
		}

		public class ReportsContext
		{
			public ReportsContext(EntityRef orderRef, EntityRef patientRef, string accessionNumber)
			{
				OrderRef = orderRef;
				PatientRef = patientRef;
				AccessionNumber = accessionNumber;
			}

			public EntityRef OrderRef;
			public EntityRef PatientRef;
			public string AccessionNumber;
		}

		/// <summary>
		/// Represents a collection of <see cref="ReportListItem"/> which share a common report which should be presented to the user
		/// as a single entity.
		/// </summary>
		public class CommonReportListItem
		{
			private readonly EntityRef _reportRef;
			private readonly EntityRef _procedureRef;
			private readonly IList<ReportListItem> _reportListItems;

			public CommonReportListItem(EntityRef procedureRef, EntityRef reportRef, ReportListItem firstItem)
			{
				_procedureRef = procedureRef;
				_reportRef = reportRef;
				_reportListItems = new List<ReportListItem>();
				_reportListItems.Add(firstItem);
			}

			public EntityRef ReportRef
			{
				get { return _reportRef; }
			}

			public EntityRef ProcedureRef
			{
				get { return _procedureRef; }
			}

			public EnumValueInfo ReportStatus
			{
				get { return CollectionUtils.FirstElement(_reportListItems).ReportStatus; }
			}

			public void AddReportListItem(ReportListItem item)
			{
				if(item != null)
					_reportListItems.Add(item);
			}

			public override string ToString()
			{
				if (_reportListItems.Count == 0)
					return string.Empty;
				
				var s = string.Join(" / ",
					CollectionUtils.Map<ReportListItem, string>(
						_reportListItems, 
						ProcedureFormat.Format).ToArray());

				s += " : " + _reportListItems[0].ReportStatus.Value;

				return s;
			}
		}

		private ReportsContext _context;

		private List<CommonReportListItem> _reports;
		private CommonReportListItem _selectedReport;

		private ToolSet _toolSet;

		private ReportPreviewComponent _reportPreviewComponent;
		private ChildComponentHost _reportPreviewComponentHost;
		private EntityRef _patientProfileRef;

		private event EventHandler _reportSelectionChanged;

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public BiographyOrderReportsComponent()
		{
			_context = null;
			_reports = new List<CommonReportListItem>();
		}

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_reportPreviewComponent = new ReportPreviewComponent(null);
			_reportPreviewComponentHost = new ChildComponentHost(this.Host, _reportPreviewComponent);
			_reportPreviewComponentHost.StartComponent();

			RefreshComponent();

			_toolSet = new ToolSet(new BiographyOrderReportsToolExtensionPoint(), new BiographyOrderReportsToolContext(this));

			base.Start();
		}

		public override void Stop()
		{
			if (_reportPreviewComponentHost != null)
			{
				_reportPreviewComponentHost.StopComponent();
				_reportPreviewComponentHost = null;
			}

			base.Stop();
		}

		#endregion

		#region Presentation model

		public IList Reports
		{
			get { return _reports; }
		}

		public CommonReportListItem SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				if (Equals(_selectedReport, value))
					return;

				_selectedReport = value;
				OnReportSelectionChanged();
			}
		}

		public event EventHandler ReportSelectionChanged
		{
			add { _reportSelectionChanged += value; }
			remove { _reportSelectionChanged -= value; }
		}

		public ActionModelNode ActionModel
		{
			get 
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "biography-reports-toolbar", _toolSet.Actions);
			}
		}

		public ApplicationComponentHost ReportPreviewComponentHost
		{
			get { return _reportPreviewComponentHost; }
		}

		public string FormatReportListItem(object item)
		{
			var reportListItem = (CommonReportListItem)item;
			return reportListItem.ToString();
		}

		#endregion

		public ReportsContext Context
		{
			get { return _context; }
			set
			{
				_context = value;
				if(this.IsStarted)
				{
					RefreshComponent();
				}
			}
		}

		public EntityRef PatientRef
		{
			get { return this.Context != null ? this.Context.PatientRef : null; }
		}

		public EntityRef PatientProfileRef
		{
			get { return _patientProfileRef; }
		}

		public EntityRef OrderRef
		{
			get { return this.Context != null ? this.Context.OrderRef : null; }
		}

		public EntityRef ProcedureRef
		{
			get { return this._selectedReport != null ? this._selectedReport.ProcedureRef : null; }
		}

		public EntityRef ReportRef
		{
			get { return this._selectedReport != null ? this._selectedReport.ReportRef : null; }
		}

		public EnumValueInfo ReportStatus
		{
			get { return this._selectedReport != null ? this._selectedReport.ReportStatus : null; }
		}

		public string AccessionNumber
		{
			get { return this.Context != null ? this.Context.AccessionNumber : null; }
		}

		private void RefreshComponent()
		{
			if (_context == null)
			{
				_reports = new List<CommonReportListItem>();
				_selectedReport = null;

				OnReportSelectionChanged();
				NotifyAllPropertiesChanged();
				return;
			}

			LoadReports();
		}

		private void OnReportSelectionChanged()
		{
			_reportPreviewComponent.PreviewContext = _selectedReport != null ? new ReportPreviewComponent.ReportPreviewContext(_selectedReport.ReportRef) : null;
			EventsHelper.Fire(_reportSelectionChanged, this, EventArgs.Empty);
		}

		private void LoadReports()
		{
			Async.CancelPending(this);

			if (_context == null)
				return;

			Async.Request(
				this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
						{
							ListReportsRequest = new ListReportsRequest(null, _context.OrderRef),
							ListPatientProfilesRequest = new ListPatientProfilesRequest(_context.PatientRef),
							GetOrderDetailRequest = new GetOrderDetailRequest(_context.OrderRef, false, true, false, false, false, false)
						};
					return service.GetData(request);
				},
				response =>
				{
					var procedure = CollectionUtils.FirstElement(response.GetOrderDetailResponse.Order.Procedures);
					if (procedure != null)
					{
						var facilityCode = procedure.PerformingFacility.InformationAuthority.Code;
						var matchingProfile = CollectionUtils.SelectFirst(
							response.ListPatientProfilesResponse.Profiles,
							summary => summary.Mrn.AssigningAuthority.Code == facilityCode);
						_patientProfileRef = matchingProfile != null ? matchingProfile.PatientProfileRef : null;
					}

					var reports = new List<CommonReportListItem>();

					CollectionUtils.ForEach(response.ListReportsResponse.Reports,
						delegate(ReportListItem item)
						{
							var existingItem = CollectionUtils.SelectFirst( reports, crli => Equals(crli.ReportRef, item.ReportRef));

							if (existingItem != null)
								existingItem.AddReportListItem(item);
							else
								reports.Add(new CommonReportListItem(item.ProcedureRef, item.ReportRef, item));
						});

					_reports = reports;
					_selectedReport = CollectionUtils.FirstElement(_reports);

					OnReportSelectionChanged();
					NotifyAllPropertiesChanged();
				},
				exception =>
				{
					_reports = new List<CommonReportListItem>();
					_selectedReport = null;
					ExceptionHandler.Report(exception, this.Host.DesktopWindow);

					OnReportSelectionChanged();
					NotifyAllPropertiesChanged();
				});
		}
	}
}