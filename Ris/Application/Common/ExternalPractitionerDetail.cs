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
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ExternalPractitionerDetail : DataContractBase, IEquatable<ExternalPractitionerDetail>
	{
		public ExternalPractitionerDetail(
			EntityRef practitionerRef,
			PersonNameDetail personNameDetail,
			string licenseNumber,
			string billingNumber,
			bool isVerified,
			DateTime? lastVerifiedTime,
			DateTime? lastEditedTime,
			List<ExternalPractitionerContactPointDetail> contactPoints,
			Dictionary<string, string> extendedProperties,
			ExternalPractitionerSummary mergeDestination,
			bool isMerged,
			bool deactivated)
		{
			this.PractitionerRef = practitionerRef;
			this.Name = personNameDetail;
			this.LicenseNumber = licenseNumber;
			this.BillingNumber = billingNumber;
			this.IsVerified = isVerified;
			this.LastVerifiedTime = lastVerifiedTime;
			this.LastEditedTime = lastEditedTime;
			this.ContactPoints = contactPoints;
			this.ExtendedProperties = extendedProperties;
			this.MergeDestination = mergeDestination;
			this.IsMerged = isMerged;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerDetail()
		{
			this.Name = new PersonNameDetail();
			this.ContactPoints = new List<ExternalPractitionerContactPointDetail>();
			this.ExtendedProperties = new Dictionary<string, string>();
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public string LicenseNumber;

		[DataMember]
		public string BillingNumber;

		[DataMember]
		public bool IsVerified { get; private set; }

		[DataMember]
		public DateTime? LastVerifiedTime { get; private set; }

		[DataMember]
		public DateTime? LastEditedTime { get; private set; }

		[DataMember]
		public List<ExternalPractitionerContactPointDetail> ContactPoints;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

		[DataMember]
		public ExternalPractitionerSummary MergeDestination;

		[DataMember]
		public bool IsMerged;

		[DataMember]
		public bool Deactivated;

		public ExternalPractitionerSummary CreateSummary()
		{
			return new ExternalPractitionerSummary(
				this.PractitionerRef,
				this.Name,
				this.LicenseNumber,
				this.BillingNumber,
				this.IsVerified,
				this.LastVerifiedTime,
				this.LastEditedTime,
				this.IsMerged,
				this.Deactivated);
		}

		public bool Equals(ExternalPractitionerDetail detail)
		{
			if (detail == null)
				return false;

			return Equals(this.PractitionerRef, detail.PractitionerRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) 
				return true;

			return Equals(obj as ExternalPractitionerDetail);
		}

		public override int GetHashCode()
		{
			return this.PractitionerRef != null ? this.PractitionerRef.GetHashCode() : 0;
		}
	}
}
