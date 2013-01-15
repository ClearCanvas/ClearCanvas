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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class LocationAssembler
	{
		public LocationSummary CreateLocationSummary(Location location)
		{
			return new LocationSummary(
				location.GetRef(),
				location.Id,
				location.Name,
				new FacilityAssembler().CreateFacilitySummary(location.Facility),
				location.Building,
				location.Floor,
				location.PointOfCare,
				location.Deactivated);
		}

		public LocationDetail CreateLocationDetail(Location location)
		{
			return new LocationDetail(
				location.GetRef(),
				location.Id,
				location.Name,
				location.Description,
				new FacilityAssembler().CreateFacilitySummary(location.Facility),
				location.Building,
				location.Floor,
				location.PointOfCare,
				location.Deactivated);
		}

		public void UpdateLocation(LocationDetail detail, Location location, IPersistenceContext context)
		{
			location.Name = detail.Name;
			location.Id = detail.Id;
			location.Description = detail.Description;

			location.Facility = context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
			location.Building = detail.Building;
			location.Floor = detail.Floor;
			location.PointOfCare = detail.PointOfCare;
			location.Deactivated = detail.Deactivated;
		}

	}
}
