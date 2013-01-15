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

using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class DepartmentAssembler
	{
		public DepartmentSummary CreateSummary(Department item, IPersistenceContext context)
		{
			return new DepartmentSummary(item.GetRef(),
										 item.Id,
										 item.Name,
										 item.Facility.Code,
										 item.Facility.Name,
										 item.Deactivated);
		}

		public DepartmentDetail CreateDetail(Department item, IPersistenceContext context)
		{
			var facilityAssembler = new FacilityAssembler();
			return new DepartmentDetail(item.GetRef(),
										 item.Id,
										 item.Name,
										 item.Description,
										 facilityAssembler.CreateFacilitySummary(item.Facility),
										 item.Deactivated);
		}

		public void UpdateDepartment(Department item, DepartmentDetail detail, IPersistenceContext context)
		{
			item.Id = detail.Id;
			item.Name = detail.Name;
			item.Description = detail.Description;
			item.Facility = context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
			item.Deactivated = detail.Deactivated;
		}
	}
}