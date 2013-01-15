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
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.OrderNotes
{
	[DataContract]
	public class GetConversationEditorFormDataResponse : DataContractBase
	{
		public GetConversationEditorFormDataResponse(List<StaffGroupSummary> onBehalfOfGroupChoices)
		{
			OnBehalfOfGroupChoices = onBehalfOfGroupChoices;
			RecipientStaffs = new List<StaffSummary>();
			RecipientStaffGroups = new List<StaffGroupSummary>();
		}

		/// <summary>
		/// The on-behalf-of group choices for the current user.
		/// </summary>
		[DataMember]
		public List<StaffGroupSummary> OnBehalfOfGroupChoices;

		/// <summary>
		/// Staff summaries for recipient staff, specified in the request.
		/// </summary>
		[DataMember]
		public List<StaffSummary> RecipientStaffs;

		/// <summary>
		/// Group summaries for recipient groups, specified in the request.
		/// </summary>
		[DataMember]
		public List<StaffGroupSummary> RecipientStaffGroups;
	}
}
