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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class LocationDetail : DataContractBase
	{
		public LocationDetail(
			EntityRef locationRef,
			string id,
			string name,
			string description,
			FacilitySummary facility,
			string building,
			string floor,
			string pointOfCare,
			bool deactivated)
		{
			this.LocationRef = locationRef;
			this.Id = id;
			this.Name = name;
			this.Description = description;
			this.Facility = facility;
			this.Building = building;
			this.Floor = floor;
			this.PointOfCare = pointOfCare;
			this.Deactivated = deactivated;
		}

		public LocationDetail()
		{
		}


		[DataMember]
		public EntityRef LocationRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

		[DataMember]
		public FacilitySummary Facility;

		[DataMember]
		public string Building;

		[DataMember]
		public string Floor;

		[DataMember]
		public string PointOfCare;

		[DataMember]
		public bool Deactivated;
	}
}
