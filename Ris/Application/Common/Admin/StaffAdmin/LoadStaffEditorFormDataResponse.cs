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

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
	[DataContract]
	public class LoadStaffEditorFormDataResponse : DataContractBase
	{
		public LoadStaffEditorFormDataResponse(
			List<EnumValueInfo> staffTypeChoices,
			List<EnumValueInfo> sexChoices,
			List<EnumValueInfo> phoneTypeChoices,
			List<EnumValueInfo> addressTypeChoices,
			List<StaffGroupSummary> groupChoices)
		{
			this.StaffTypeChoices = staffTypeChoices;
			this.SexChoices = sexChoices;
			this.PhoneTypeChoices = phoneTypeChoices;
			this.AddressTypeChoices = addressTypeChoices;
			this.StaffGroupChoices = groupChoices;
		}

		[DataMember]
		public List<EnumValueInfo> StaffTypeChoices;

		[DataMember]
		public List<EnumValueInfo> SexChoices;

		[DataMember]
		public List<EnumValueInfo> PhoneTypeChoices;

		[DataMember]
		public List<EnumValueInfo> AddressTypeChoices;

		[DataMember]
		public List<StaffGroupSummary> StaffGroupChoices;
	}
}
