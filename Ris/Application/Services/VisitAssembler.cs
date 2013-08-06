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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class VisitAssembler
	{
		public VisitSummary CreateVisitSummary(Visit visit, IPersistenceContext context)
		{
			var patientProfileAssembler = new PatientProfileAssembler();
			var summary = new VisitSummary
				{
					VisitRef = visit.GetRef(),
					Patient = patientProfileAssembler.CreatePatientProfileSummary(visit.PatientProfile, context),
					VisitNumber = CreateVisitNumberDetail(visit.VisitNumber),
					AdmissionType = EnumUtils.GetEnumValueInfo(visit.AdmissionType),
					PatientClass = EnumUtils.GetEnumValueInfo(visit.PatientClass),
					PatientType = EnumUtils.GetEnumValueInfo(visit.PatientType),
					Status = EnumUtils.GetEnumValueInfo(visit.Status, context),
					AdmitTime = visit.AdmitTime,
					DischargeTime = visit.DischargeTime
				};

			var facilityAssembler = new FacilityAssembler();
			summary.Facility = visit.Facility == null ? null : facilityAssembler.CreateFacilitySummary(visit.Facility);

			var locationAssembler = new LocationAssembler();
			summary.CurrentLocation = visit.CurrentLocation == null ? null : locationAssembler.CreateLocationSummary(visit.CurrentLocation);
			summary.CurrentRoom = visit.CurrentRoom;
			summary.CurrentBed = visit.CurrentBed;

			return summary;
		}

		public VisitDetail CreateVisitDetail(Visit visit, IPersistenceContext context)
		{
			var patientProfileAssembler = new PatientProfileAssembler();
			var detail = new VisitDetail
				{
					VisitRef = visit.GetRef(),
					Patient = patientProfileAssembler.CreatePatientProfileSummary(visit.PatientProfile, context),
					VisitNumber = CreateVisitNumberDetail(visit.VisitNumber),
					AdmissionType = EnumUtils.GetEnumValueInfo(visit.AdmissionType),
					PatientClass = EnumUtils.GetEnumValueInfo(visit.PatientClass),
					PatientType = EnumUtils.GetEnumValueInfo(visit.PatientType),
					Status = EnumUtils.GetEnumValueInfo(visit.Status, context),
					AdmitTime = visit.AdmitTime,
					DischargeTime = visit.DischargeTime,
					DischargeDisposition = visit.DischargeDisposition,
					Facility = new FacilityAssembler().CreateFacilitySummary(visit.Facility),
					CurrentLocation = visit.CurrentLocation == null ? null : new LocationAssembler().CreateLocationSummary(visit.CurrentLocation),
					CurrentRoom = visit.CurrentRoom,
					CurrentBed = visit.CurrentBed,
					Locations = new List<VisitLocationDetail>(),
					PreadmitNumber = visit.PreadmitNumber,
					VipIndicator = visit.VipIndicator,
					ExtendedProperties = ExtendedPropertyUtils.Copy(visit.ExtendedProperties)
				};

			foreach (var vl in visit.Locations)
			{
				detail.Locations.Add(CreateVisitLocationDetail(vl, context));
			}

			detail.Practitioners = new List<VisitPractitionerDetail>();
			foreach (var vp in visit.Practitioners)
			{
				detail.Practitioners.Add(CreateVisitPractitionerDetail(vp, context));
			}

			detail.AmbulatoryStatuses = new List<EnumValueInfo>();
			foreach (var ambulatoryStatus in visit.AmbulatoryStatuses)
			{
				detail.AmbulatoryStatuses.Add(EnumUtils.GetEnumValueInfo(ambulatoryStatus));
			}

			return detail;
		}

		public CompositeIdentifierDetail CreateVisitNumberDetail(VisitNumber vn)
		{
			return new CompositeIdentifierDetail(vn.Id, EnumUtils.GetEnumValueInfo(vn.AssigningAuthority));
		}

		public void UpdateVisit(Visit visit, VisitDetail detail, IPersistenceContext context)
		{
			visit.Patient = context.Load<Patient>(detail.Patient.PatientRef, EntityLoadFlags.Proxy);
			visit.VisitNumber.Id = detail.VisitNumber.Id;
			visit.VisitNumber.AssigningAuthority = EnumUtils.GetEnumValue<InformationAuthorityEnum>(detail.VisitNumber.AssigningAuthority, context);

			visit.AdmissionType = EnumUtils.GetEnumValue<AdmissionTypeEnum>(detail.AdmissionType, context);
			visit.PatientClass = EnumUtils.GetEnumValue<PatientClassEnum>(detail.PatientClass, context);
			visit.PatientType = EnumUtils.GetEnumValue<PatientTypeEnum>(detail.PatientType, context);
			visit.Status = EnumUtils.GetEnumValue<VisitStatus>(detail.Status);

			visit.AdmitTime = detail.AdmitTime;
			visit.DischargeTime = detail.DischargeTime;
			visit.DischargeDisposition = detail.DischargeDisposition;
			visit.VipIndicator = detail.VipIndicator;

			visit.Facility = detail.Facility == null ? null :
				context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
			visit.CurrentLocation = detail.CurrentLocation == null ? null :
				context.Load<Location>(detail.CurrentLocation.LocationRef, EntityLoadFlags.Proxy);
			visit.CurrentRoom = detail.CurrentRoom;
			visit.CurrentBed = detail.CurrentBed;

			visit.Locations.Clear();
			foreach (var vlDetail in detail.Locations)
			{
				visit.Locations.Add(new VisitLocation(
					context.Load<Location>(vlDetail.Location.LocationRef, EntityLoadFlags.Proxy),
					vlDetail.Room,
					vlDetail.Bed,
					EnumUtils.GetEnumValue<VisitLocationRole>(vlDetail.Role),
					vlDetail.StartTime,
					vlDetail.EndTime));
			}

			visit.Practitioners.Clear();
			foreach (var vpDetail in detail.Practitioners)
			{
				visit.Practitioners.Add(new VisitPractitioner(
					context.Load<ExternalPractitioner>(vpDetail.Practitioner.PractitionerRef, EntityLoadFlags.Proxy),
					EnumUtils.GetEnumValue<VisitPractitionerRole>(vpDetail.Role),
					vpDetail.StartTime,
					vpDetail.EndTime));
			}

			visit.AmbulatoryStatuses.Clear();
			foreach (var ambulatoryStatus in detail.AmbulatoryStatuses)
			{
				visit.AmbulatoryStatuses.Add(EnumUtils.GetEnumValue<AmbulatoryStatusEnum>(ambulatoryStatus, context));
			}

			ExtendedPropertyUtils.Update(visit.ExtendedProperties, detail.ExtendedProperties);
		}

		private static VisitLocationDetail CreateVisitLocationDetail(VisitLocation vl, IPersistenceContext context)
		{
			var detail = new VisitLocationDetail
				{
					Location = new LocationAssembler().CreateLocationSummary(vl.Location),
					Room = vl.Room,
					Bed = vl.Bed,
					Role = EnumUtils.GetEnumValueInfo(vl.Role, context),
					StartTime = vl.StartTime,
					EndTime = vl.EndTime
				};

			return detail;
		}

		private static VisitPractitionerDetail CreateVisitPractitionerDetail(VisitPractitioner vp, IPersistenceContext context)
		{
			var detail = new VisitPractitionerDetail
				{
					Practitioner = new ExternalPractitionerAssembler().CreateExternalPractitionerSummary(vp.Practitioner, context),
					Role = EnumUtils.GetEnumValueInfo(vp.Role, context),
					StartTime = vp.StartTime,
					EndTime = vp.EndTime
				};

			return detail;
		}
	}
}
