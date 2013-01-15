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
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
	internal static class TestOrderNoteFactory
	{
		internal static IList<OrderNote> CreateOrderNotes()
		{
			IList<OrderNote> notes = new List<OrderNote>();

			notes.Add(CreateOrderNote("Test note"));

			return notes;
		}

		internal static OrderNote CreateOrderNote(string comment)
		{
			//return new OrderNote(Platform.Time, TestStaffFactory.CreateStaff(StaffType.STEC), comment);
			OrderNote note = new OrderNote();
			note.CreationTime = Platform.Time;
			note.Author = TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null));
			note.Body = comment;
			return note;
		}
	}
}
