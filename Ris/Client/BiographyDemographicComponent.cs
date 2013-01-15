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
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyDemographicComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class BiographyDemographicComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// BiographyDemographicComponent class
	/// </summary>
	[AssociateView(typeof(BiographyDemographicComponentViewExtensionPoint))]
	public class BiographyDemographicComponent : ApplicationComponent
	{
		class ProfileViewComponent : DHtmlComponent
		{
			private PatientProfileSummary _patientProfile;

			public PatientProfileSummary PatientProfile
			{
				get { return _patientProfile; }
				set
				{
					_patientProfile = value;
					this.SetUrl(WebResourcesSettings.Default.BiographyPatientProfilePageUrl);
				}
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return PatientProfile;
			}
		}

		private EntityRef _defaultProfileRef;
		private EntityRef _patientRef;

		private PatientProfileSummary _selectedProfile;
		private List<PatientProfileSummary> _profileChoices;

		private ProfileViewComponent _profileViewComponent;
		private ChildComponentHost _profileViewComponentHost;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyDemographicComponent()
		{
			_profileChoices = new List<PatientProfileSummary>();
		}

		public override void Start()
		{
			_profileViewComponent = new ProfileViewComponent();
			_profileViewComponentHost = new ChildComponentHost(this.Host, _profileViewComponent);
			_profileViewComponentHost.StartComponent();

			LoadPatientProfile();

			base.Start();
		}

		public override void Stop()
		{
			if (_profileViewComponentHost != null)
			{
				_profileViewComponentHost.StopComponent();
				_profileViewComponentHost = null;
			}

			base.Stop();
		}

		public EntityRef DefaultProfileRef
		{
			get { return _defaultProfileRef; }
			set { _defaultProfileRef = value; }
		}

		public EntityRef PatientRef
		{
			get { return _patientRef; }
			set
			{
				_patientRef = value;

				if (this.IsStarted)
					LoadPatientProfile();
			}
		}

		#region Presentation Model

		public ApplicationComponentHost ProfileViewComponentHost
		{
			get { return _profileViewComponentHost; }
		}

		public IList ProfileChoices
		{
			get { return _profileChoices; }
		}

		public PatientProfileSummary SelectedProfile
		{
			get { return _selectedProfile; }
			set
			{
				if (Equals(value, _selectedProfile))
					return;

				_selectedProfile = value;
				_profileViewComponent.PatientProfile = _selectedProfile;
			}
		}

		public string FormatPatientProfile(object item)
		{
			var summary = (PatientProfileSummary)item;
			return String.Format("{0} - {1}", MrnFormat.Format(summary.Mrn), PersonNameFormat.Format(summary.Name));
		}

		#endregion

		private void LoadPatientProfile()
		{
			Async.CancelPending(this);

			if (_patientRef == null)
				return;

			Async.Request(this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
						{
							ListPatientProfilesRequest = new ListPatientProfilesRequest(_patientRef)
						};
					return service.GetData(request);
				},
				response =>
				{
					_profileChoices = response.ListPatientProfilesResponse.Profiles;

					_selectedProfile = _defaultProfileRef != null 
						? CollectionUtils.SelectFirst(_profileChoices, pp => pp.PatientProfileRef.Equals(_defaultProfileRef, true)) 
						: CollectionUtils.FirstElement(_profileChoices);

					_profileViewComponent.PatientProfile = _selectedProfile;

					NotifyPropertyChanged("SelectedProfile");
					NotifyAllPropertiesChanged();
				},
				exception =>
				{
					_profileChoices = new List<PatientProfileSummary>();
					_selectedProfile = null;
					_profileViewComponent.PatientProfile = null;
					ExceptionHandler.Report(exception, this.Host.DesktopWindow);

					NotifyPropertyChanged("SelectedProfile");
					NotifyAllPropertiesChanged();
				});
		}
		
	}
}
