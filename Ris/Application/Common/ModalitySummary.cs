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
	public class ModalitySummary : DataContractBase, IEquatable<ModalitySummary>
	{
		public ModalitySummary(EntityRef modalityRef, string id, string name, FacilitySummary facility, string aeTitle, EnumValueInfo dicomModality, bool deactivated)
		{
			this.ModalityRef = modalityRef;
			this.Id = id;
			this.Name = name;
			this.Facility = facility;
			this.AETitle = aeTitle;
			this.DicomModality = dicomModality;
			this.Deactivated = deactivated;
		}

		public ModalitySummary()
		{
		}

		[DataMember]
		public EntityRef ModalityRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public FacilitySummary Facility;

		[DataMember]
		public string AETitle;

		[DataMember]
		public EnumValueInfo DicomModality;

		[DataMember]
		public bool Deactivated;

		public bool Equals(ModalitySummary other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ModalityRef, ModalityRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ModalitySummary)) return false;
			return Equals((ModalitySummary) obj);
		}

		public override int GetHashCode()
		{
			return (ModalityRef != null ? ModalityRef.GetHashCode() : 0);
		}
	}
}
