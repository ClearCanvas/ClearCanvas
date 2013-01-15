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
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;
using ClearCanvas.Ris.Application.Services;

namespace ClearCanvas.Ris.Application.Extended.Services.OrderNotes
{
	class OrderNoteboxItemAssembler
	{
		public OrderNoteboxItemSummary CreateSummary(OrderNoteboxItem item, IPersistenceContext context)
		{
			var mrnAssembler = new MrnAssembler();
			var nameAssembler = new PersonNameAssembler();
			var staffAssembler = new StaffAssembler();
			var groupAssembler = new StaffGroupAssembler();

			var staffRecipients = new List<StaffSummary>();
			var groupRecipients = new List<StaffGroupSummary>();

			foreach (var recipient in item.Recipients)
			{
				if (recipient is Staff)
					staffRecipients.Add(staffAssembler.CreateStaffSummary((Staff)recipient, context));
				if (recipient is StaffGroup)
					groupRecipients.Add(groupAssembler.CreateSummary((StaffGroup)recipient));
			}

			return new OrderNoteboxItemSummary(
				item.NoteRef,
				item.OrderRef,
				item.PatientRef,
				item.PatientProfileRef,
				mrnAssembler.CreateMrnDetail(item.Mrn),
				nameAssembler.CreatePersonNameDetail(item.PatientName),
				item.DateOfBirth,
				item.AccessionNumber,
				item.DiagnosticServiceName,
				item.Category,
				item.Urgent,
				item.PostTime,
				staffAssembler.CreateStaffSummary(item.Author, context),
				item.OnBehalfOfGroup == null ? null : groupAssembler.CreateSummary(item.OnBehalfOfGroup),
				item.IsAcknowledged,
				staffRecipients,
				groupRecipients);
		}
	}
}
