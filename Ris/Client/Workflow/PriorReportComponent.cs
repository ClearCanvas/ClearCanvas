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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using System;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="PriorReportComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PriorReportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PriorReportComponent class
	/// </summary>
	[AssociateView(typeof(PriorReportComponentViewExtensionPoint))]
	public class PriorReportComponent : ApplicationComponent
	{
		class ReportViewComponent : DHtmlComponent
		{
			private readonly PriorReportComponent _owner;

			public ReportViewComponent(PriorReportComponent owner)
			{
				_owner = owner;
			}

			public override void Start()
			{
				SetUrl(WebResourcesSettings.Default.PriorReportPreviewPageUrl);
				base.Start();
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner._selectedPrior;
			}
		}

		private ReportingWorklistItemSummary _worklistItem;
		private EntityRef _reportRef;

		private readonly PriorSummaryTable _reportList;
		private PriorProcedureSummary _selectedPrior;

		private bool _relevantPriorsOnly;

		private List<PriorProcedureSummary> _relevantPriors;
		private List<PriorProcedureSummary> _allPriors;

		private ChildComponentHost _reportViewComponentHost;

		/// <summary>
		/// Constructor for showing priors based on a reporting step.
		/// </summary>
		public PriorReportComponent(ReportingWorklistItemSummary worklistItem)
			: this(worklistItem, null)
		{
		}

		/// <summary>
		/// Constructor for showing priors based on a reporting step when the report ref in the reporting step may be stale.
		/// </summary>
		public PriorReportComponent(ReportingWorklistItemSummary worklistItem, EntityRef reportRef)
			: this()
		{
			_worklistItem = worklistItem;
			_reportRef = reportRef;
		}

		private PriorReportComponent()
		{
			_relevantPriorsOnly = PriorReportComponentSettings.Default.ShowRelevantPriorsOnly;
			_reportList = new PriorSummaryTable();
		}

		public override void Start()
		{
			_reportViewComponentHost = new ChildComponentHost(this.Host, new ReportViewComponent(this));
			_reportViewComponentHost.StartComponent();

			UpdateReportList();

			base.Start();
		}

		public override void Stop()
		{
			if (_reportViewComponentHost != null)
			{
				_reportViewComponentHost.StopComponent();
				_reportViewComponentHost = null;
			}

			base.Stop();
		}

		#region Presentation Model

		public ApplicationComponentHost ReportViewComponentHost
		{
			get { return _reportViewComponentHost; }
		}

		public bool RelevantPriorsOnly
		{
			get { return _relevantPriorsOnly; }
			set
			{
				if (value != _relevantPriorsOnly)
				{
					_relevantPriorsOnly = value;
					PriorReportComponentSettings.Default.ShowRelevantPriorsOnly = value;
					PriorReportComponentSettings.Default.Save();
					UpdateReportList();
				}
			}
		}

		public bool AllPriors
		{
			get { return !this.RelevantPriorsOnly; }
		}

		public ITable Reports
		{
			get { return _reportList; }
		}

		public ISelection SelectedReport
		{
			get { return new Selection(_selectedPrior); }
			set
			{
				var newSelection = (PriorProcedureSummary)value.Item;
				if (_selectedPrior != newSelection)
				{
					_selectedPrior = newSelection;
					((ReportViewComponent)_reportViewComponentHost.Component).Refresh();
				}
			}
		}

		#endregion

		private void UpdateReportList()
		{
			// cancel any pending requests
			Async.CancelPending(this);

			// clear table
			_reportList.Items.Clear();

			if (_relevantPriorsOnly)
			{
				Async.Invoke(this,
					delegate
					{
						if (_relevantPriors == null)
							_relevantPriors = LoadPriors(true);
					},
					delegate
					{
						_reportList.Items.AddRange(_relevantPriors);
					});

			}
			else
			{
				Async.Invoke(this,
					delegate
					{
						if (_allPriors == null)
							_allPriors = LoadPriors(false);
					},
					delegate
					{
						_reportList.Items.AddRange(_allPriors);
					});
			}
		}

		private List<PriorProcedureSummary> LoadPriors(bool relevantOnly)
		{
			GetPriorsResponse response = null;
			Platform.GetService<IReportingWorkflowService>(service =>
				{
					var request = new GetPriorsRequest {RelevantOnly = relevantOnly};

					// ReportRef will be null when used from Protocolling component
					if (this.ReportRef != null)
						request.ReportRef = this.ReportRef;
					else
						request.OrderRef = _worklistItem.OrderRef;

					response = service.GetPriors(request);
				});
			return response.Reports;
		}

		private EntityRef ReportRef
		{
			get
			{
				return _reportRef 
					?? (_worklistItem != null ? _worklistItem.ReportRef : null);
			}
		}

		public void SetContext(ReportingWorklistItemSummary worklistItem, EntityRef reportRef)
		{
			_reportRef = reportRef;
			_worklistItem = worklistItem;

			if (this.IsStarted)
			{
				_relevantPriors = null;
				_allPriors = null;
				UpdateReportList();
				((ReportViewComponent)_reportViewComponentHost.Component).Refresh();
			}
		}
	}
}
