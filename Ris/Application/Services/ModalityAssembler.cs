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
	public class ModalityAssembler
	{
		public ModalitySummary CreateModalitySummary(Modality modality)
		{
			var facilityAssember = new FacilityAssembler();
			return new ModalitySummary(
				modality.GetRef(),
				modality.Id,
				modality.Name,
				modality.Facility == null ? null : facilityAssember.CreateFacilitySummary(modality.Facility),
				modality.AETitle,
				EnumUtils.GetEnumValueInfo(modality.DicomModality),
				modality.Deactivated);
		}

		public ModalityDetail CreateModalityDetail(Modality modality)
		{
			var facilityAssember = new FacilityAssembler();
			return new ModalityDetail(
				modality.GetRef(),
				modality.Id,
				modality.Name,
				modality.Facility == null ? null : facilityAssember.CreateFacilitySummary(modality.Facility),
				modality.AETitle,
				EnumUtils.GetEnumValueInfo(modality.DicomModality),
				modality.Deactivated);
		}

		public void UpdateModality(ModalityDetail detail, Modality modality, IPersistenceContext context)
		{
			modality.Id = detail.Id;
			modality.Name = detail.Name;
			modality.Facility = detail.Facility == null ? null : context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
			modality.AETitle = detail.AETitle;
			modality.DicomModality = EnumUtils.GetEnumValue<DicomModalityEnum>(detail.DicomModality, context);
			modality.Deactivated = detail.Deactivated;
		}
	}
}
