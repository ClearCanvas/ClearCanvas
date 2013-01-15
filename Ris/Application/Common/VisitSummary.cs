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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class VisitSummary : DataContractBase, IEquatable<VisitSummary>
	{
		public VisitSummary()
		{
		}

		public VisitSummary(
			EntityRef visitRef,
			PatientProfileSummary patient,
			CompositeIdentifierDetail visitNumber,
			EnumValueInfo patientClass,
			EnumValueInfo patientType,
			EnumValueInfo admissionType,
			EnumValueInfo status,
			DateTime? admitTime,
			DateTime? dischargeTime,
			FacilitySummary facility,
			LocationSummary currentLocation,
			string currentRoom,
			string currentBed)
		{
			this.VisitRef = visitRef;
			this.Patient = patient;
			this.VisitNumber = visitNumber;
			this.PatientClass = patientClass;
			this.PatientType = patientType;
			this.AdmissionType = admissionType;
			this.Status = status;
			this.AdmitTime = admitTime;
			this.DischargeTime = dischargeTime;
			this.Facility = facility;
			this.CurrentLocation = currentLocation;
			this.CurrentRoom = currentRoom;
			this.CurrentBed = currentBed;
		}

		[DataMember]
		public EntityRef VisitRef;

		[DataMember]
		public PatientProfileSummary Patient;

		[DataMember]
		public CompositeIdentifierDetail VisitNumber;

		[DataMember]
		public EnumValueInfo PatientClass;

		[DataMember]
		public EnumValueInfo PatientType;

		[DataMember]
		public EnumValueInfo AdmissionType;

		[DataMember]
		public EnumValueInfo Status;

		[DataMember]
		public DateTime? AdmitTime;

		[DataMember]
		public DateTime? DischargeTime;

		[DataMember]
		public FacilitySummary Facility;

		[DataMember]
		public LocationSummary CurrentLocation;

		[DataMember]
		public string CurrentRoom;

		[DataMember]
		public string CurrentBed;

		public bool Equals(VisitSummary visitSummary)
		{
			if (visitSummary == null) return false;
			return Equals(VisitRef, visitSummary.VisitRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as VisitSummary);
		}

		public override int GetHashCode()
		{
			return VisitRef.GetHashCode();
		}
	}
}
