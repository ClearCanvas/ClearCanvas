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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyVisitHistoryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class BiographyVisitHistoryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PatientVisitHistoryComponent class
	/// </summary>
	[AssociateView(typeof(BiographyVisitHistoryComponentViewExtensionPoint))]
	public class BiographyVisitHistoryComponent : ApplicationComponent
	{
		private EntityRef _patientRef;
		private readonly VisitListTable _visitList;
		private VisitListItem _selectedVisit;

		private VisitDetailViewComponent _visitDetailComponent;
		private ChildComponentHost _visitDetailComponentHost;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyVisitHistoryComponent()
		{
			_visitList = new VisitListTable();
		}

		public override void Start()
		{
			_visitDetailComponent = new BiographyVisitDetailViewComponent();
			_visitDetailComponentHost = new ChildComponentHost(this.Host, _visitDetailComponent);
			_visitDetailComponentHost.StartComponent();

			LoadVisits();

			base.Start();
		}

		public override void Stop()
		{
			if (_visitDetailComponentHost != null)
			{
				_visitDetailComponentHost.StopComponent();
				_visitDetailComponentHost = null;
			}

			base.Stop();
		}

		public EntityRef PatientRef
		{
			get { return _patientRef; }
			set
			{
				_patientRef = value;

				if (this.IsStarted)
					LoadVisits();
			}
		}

		#region Presentation Model

		public ITable Visits
		{
			get { return _visitList; }
		}

		public ISelection SelectedVisit
		{
			get { return new Selection(_selectedVisit); }
			set
			{
				var newSelection = (VisitListItem)value.Item;
				if (_selectedVisit == newSelection)
					return;

				_selectedVisit = newSelection;

				UpdatePages();
				NotifyAllPropertiesChanged();
			}
		}

		public ApplicationComponentHost VisitDetailComponentHost
		{
			get { return _visitDetailComponentHost; }
		}

		#endregion

		private void UpdatePages()
		{
			_visitDetailComponent.Context = _selectedVisit == null 
				? null 
				: new VisitDetailViewComponent.VisitContext(_selectedVisit.VisitRef);
		}

		private void LoadVisits()
		{
			Async.CancelPending(this);

			if (_patientRef == null)
				return;

			Async.Request(
				this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
					{
						ListVisitsRequest = new ListVisitsRequest(_patientRef)
					};

					return service.GetData(request);
				},
				response =>
				{
					_visitList.Items.Clear();
					_visitList.Items.AddRange(response.ListVisitsResponse.Visits);
				});
		}
	}
}