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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
	/// <summary>
	/// Extension point for views onto <see cref="LocationEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class LocationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// LocationEditorComponent class
	/// </summary>
	[AssociateView(typeof(LocationEditorComponentViewExtensionPoint))]
	public class LocationEditorComponent : ApplicationComponent
	{
		private List<FacilitySummary> _facilityChoices;

		private LocationDetail _locationDetail;
		private EntityRef _locationRef;
		private readonly bool _isNew;

		private LocationSummary _locationSummary;

		/// <summary>
		/// Constructor
		/// </summary>
		public LocationEditorComponent()
		{
			_isNew = true;
		}

		public LocationEditorComponent(EntityRef locationRef)
		{
			_isNew = false;
			_locationRef = locationRef;
		}

		public LocationSummary LocationSummary
		{
			get { return _locationSummary; }
		}

		public override void Start()
		{
			if (_isNew)
			{
				_locationDetail = new LocationDetail();
			}
			else
			{
				Platform.GetService(
					delegate(ILocationAdminService service)
					{
							var response = service.LoadLocationForEdit(new LoadLocationForEditRequest(_locationRef));
							_locationRef = response.LocationDetail.LocationRef;
							_locationDetail = response.LocationDetail;
					});
			}
		
			Platform.GetService(
				delegate(IFacilityAdminService service)
				{
					var response = service.ListAllFacilities(new ListAllFacilitiesRequest());
					_facilityChoices = response.Facilities;

					if (_isNew && _locationDetail.Facility == null && response.Facilities.Count > 0)
					{
						_locationDetail.Facility = response.Facilities[0];
					}
				});
				

			base.Start();
		}

		public LocationDetail LocationDetail
		{
			get { return _locationDetail; }
			set { _locationDetail = value; }
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Id
		{
			get { return _locationDetail.Id; }
			set
			{
				_locationDetail.Id = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _locationDetail.Name; }
			set
			{
				_locationDetail.Name = value;
				this.Modified = true;
			}
		}

		public string Description
		{
			get { return _locationDetail.Description; }
			set
			{
				_locationDetail.Description = value;
				this.Modified = true;
			}
		}

		public IList FacilityChoices
		{
			get { return _facilityChoices; }
		}

		[ValidateNotNull]
		public FacilitySummary Facility
		{
			get { return _locationDetail.Facility; }
			set
			{
				_locationDetail.Facility = value;
				this.Modified = true;
			}
		}

		public string FormatFacility(object item)
		{
			var f = (FacilitySummary) item;
			return f.Name;
		}

		public string Building
		{
			get { return _locationDetail.Building; }
			set 
			{ 
				_locationDetail.Building = value;
				this.Modified = true;
			}
		}

		public string Floor
		{
			get { return _locationDetail.Floor; }
			set 
			{ 
				_locationDetail.Floor = value;
				this.Modified = true;
			}
		}

		public string PointOfCare
		{
			get { return _locationDetail.PointOfCare; }
			set 
			{ 
				_locationDetail.PointOfCare = value;
				this.Modified = true;
			}
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
				try
				{
					SaveChanges();

					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionSaveLocation, this.Host.DesktopWindow,
						delegate
						{
							this.ExitCode = ApplicationComponentExitCode.Error;
							this.Host.Exit();
						});
				}
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		#endregion

		private void SaveChanges()
		{
			if (_isNew)
			{
				Platform.GetService(
					delegate(ILocationAdminService service)
					{
						var response = service.AddLocation(new AddLocationRequest(_locationDetail));
						_locationRef = response.Location.LocationRef;
						_locationSummary = response.Location;
					});
			}
			else
			{
				Platform.GetService(
					delegate(ILocationAdminService service)
					{
						var response = service.UpdateLocation(new UpdateLocationRequest(_locationDetail));
						_locationRef = response.Location.LocationRef;
						_locationSummary = response.Location;
					});
			}
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}
	}
}
