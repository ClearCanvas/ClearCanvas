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
	public class ExternalPractitionerSummary : DataContractBase, ICloneable, IEquatable<ExternalPractitionerSummary>
	{
		public ExternalPractitionerSummary(
			EntityRef pracRef,
			PersonNameDetail personNameDetail,
			string licenseNumber,
			string billingNumber,
			bool isVerified,
			DateTime? lastVerifiedTime,
			DateTime? lastEditedTime,
			bool isMerged,
			bool deactivated)
		{
			this.PractitionerRef = pracRef;
			this.Name = personNameDetail;
			this.LicenseNumber = licenseNumber;
			this.BillingNumber = billingNumber;
			this.IsVerified = isVerified;
			this.LastVerifiedTime = lastVerifiedTime;
			this.LastEditedTime = lastEditedTime;
			this.IsMerged = isMerged;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerSummary()
		{
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
		public bool IsMerged;

		[DataMember]
		public bool Deactivated;

		public bool Equals(ExternalPractitionerSummary externalPractitionerSummary)
		{
			return externalPractitionerSummary != null && Equals(PractitionerRef, externalPractitionerSummary.PractitionerRef);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as ExternalPractitionerSummary);
		}

		public override int GetHashCode()
		{
			return PractitionerRef.GetHashCode();
		}

		#region ICloneable Members

		public object Clone()
		{
			return new ExternalPractitionerSummary(
				this.PractitionerRef,
				(PersonNameDetail)this.Name.Clone(),
				this.LicenseNumber,
				this.BillingNumber,
				this.IsVerified,
				this.LastVerifiedTime,
				this.LastEditedTime,
				this.IsMerged,
				this.Deactivated);
		}

		#endregion
	}
}
