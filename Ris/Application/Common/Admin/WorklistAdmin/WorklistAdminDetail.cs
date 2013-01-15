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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{


	[DataContract]
	public class WorklistAdminDetail : DataContractBase
	{
		[DataContract]
		public class TimePoint
		{
			public TimePoint(DateTime? fixedTime, long resolution)
			{
				this.FixedTime = fixedTime;
				this.Resolution = resolution;
			}

			public TimePoint(TimeSpan? relativeTime, long resolution)
			{
				this.RelativeTime = relativeTime;
				this.Resolution = resolution;
			}

			[DataMember]
			public DateTime? FixedTime;

			[DataMember]
			public TimeSpan? RelativeTime;

			[DataMember]
			public long Resolution;
		}

		[DataContract]
		public class StaffList
		{
			public StaffList()
			{
				this.Staff = new List<StaffSummary>();
			}

			[DataMember]
			public List<StaffSummary> Staff;

			[DataMember]
			public bool IncludeCurrentUser;
		}

		public WorklistAdminDetail()
		{
			this.ProcedureTypes = new List<ProcedureTypeSummary>();
			this.ProcedureTypeGroups = new List<ProcedureTypeGroupSummary>();
			this.Facilities = new List<FacilitySummary>();
			this.Departments = new List<DepartmentSummary>();
			this.PatientClasses = new List<EnumValueInfo>();
			this.PatientLocations = new List<LocationSummary>();
			this.OrderPriorities = new List<EnumValueInfo>();
			this.Portabilities = new List<bool>();

			this.InterpretedByStaff = new StaffList();
			this.TranscribedByStaff = new StaffList();
			this.VerifiedByStaff = new StaffList();
			this.SupervisedByStaff = new StaffList();

			this.StaffSubscribers = new List<StaffSummary>();
			this.GroupSubscribers = new List<StaffGroupSummary>();
		}


		public WorklistAdminDetail(EntityRef entityRef, string name, string description, WorklistClassSummary worklistClass)
			: this()
		{
			EntityRef = entityRef;
			Name = name;
			Description = description;
			WorklistClass = worklistClass;
		}

		public bool IsUserWorklist
		{
			get { return IsStaffOwned || IsGroupOwned; }
		}

		public bool IsStaffOwned
		{
			get { return OwnerStaff != null; }
		}

		public bool IsGroupOwned
		{
			get { return OwnerGroup != null; }
		}

		[DataMember]
		public EntityRef EntityRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

		[DataMember]
		public StaffSummary OwnerStaff;

		[DataMember]
		public StaffGroupSummary OwnerGroup;

		[DataMember]
		public WorklistClassSummary WorklistClass;

		[DataMember]
		public List<ProcedureTypeSummary> ProcedureTypes;

		[DataMember]
		public List<ProcedureTypeGroupSummary> ProcedureTypeGroups;

		[DataMember]
		public List<FacilitySummary> Facilities;

		[DataMember]
		public List<DepartmentSummary> Departments;

		[DataMember]
		public bool FilterByWorkingFacility;

		[DataMember]
		public List<EnumValueInfo> OrderPriorities;

		[DataMember]
		public List<ExternalPractitionerSummary> OrderingPractitioners;

		[DataMember]
		public List<EnumValueInfo> PatientClasses;

		[DataMember]
		public List<LocationSummary> PatientLocations;

		[DataMember]
		public List<bool> Portabilities;

		[DataMember]
		public TimePoint StartTime;

		[DataMember]
		public TimePoint EndTime;

		[DataMember]
		public List<StaffSummary> StaffSubscribers;

		[DataMember]
		public List<StaffGroupSummary> GroupSubscribers;

		[DataMember]
		public StaffList InterpretedByStaff;

		[DataMember]
		public StaffList TranscribedByStaff;

		[DataMember]
		public StaffList VerifiedByStaff;

		[DataMember]
		public StaffList SupervisedByStaff;
	}
}