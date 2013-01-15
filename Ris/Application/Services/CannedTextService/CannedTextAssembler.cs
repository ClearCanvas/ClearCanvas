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
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	public class CannedTextAssembler
	{
		public CannedTextSummary GetCannedTextSummary(CannedText cannedText, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextSummary(
				cannedText.GetRef(),
				cannedText.Name,
				cannedText.Category,
				cannedText.Staff == null ? null : staffAssembler.CreateStaffSummary(cannedText.Staff, context),
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup),
				cannedText.Text);
		}

		public CannedTextDetail GetCannedTextDetail(CannedText cannedText, IPersistenceContext context)
		{
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextDetail(
				cannedText.Name,
				cannedText.Category,
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup),
				cannedText.Text);
		}

		public CannedText CreateCannedText(CannedTextDetail detail, Staff owner, IPersistenceContext context)
		{
			CannedText newCannedText = new CannedText();
			UpdateCannedText(newCannedText, detail, owner, context);
			return newCannedText;
		}

		public void UpdateCannedText(CannedText cannedText, CannedTextDetail detail, Staff owner, IPersistenceContext context)
		{
			cannedText.Name = detail.Name;
			cannedText.Category = detail.Category;
			cannedText.Staff = detail.IsPersonal ? owner : null;
			cannedText.StaffGroup = detail.IsGroup ? context.Load<StaffGroup>(detail.StaffGroup.StaffGroupRef, EntityLoadFlags.Proxy) : null;
			cannedText.Text = detail.Text;
		}
	}
}
