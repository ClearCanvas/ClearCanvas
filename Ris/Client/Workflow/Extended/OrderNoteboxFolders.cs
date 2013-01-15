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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to me")]
	[FolderDescription("OrderNotesPersonalInboxFolderDescription")]
	internal class PersonalInboxFolder : OrderNoteboxFolder
	{
		public PersonalInboxFolder(OrderNoteboxFolderSystem folderSystem)
			: base(folderSystem, "OrderNotePersonalInbox")
		{
		}
	}

	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to my groups")]
	[FolderDescription("OrderNotesGroupInboxFolderDescription")]
	internal class GroupInboxFolder : OrderNoteboxFolder
	{
		private readonly EntityRef _groupRef;

		public GroupInboxFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem, StaffGroupSummary staffGroup)
			: base(orderNoteboxFolderSystem, "OrderNoteGroupInbox")
		{
			_groupRef = staffGroup.StaffGroupRef;
			this.FolderPath = this.FolderPath.Append(new PathSegment(staffGroup.Name, this.ResourceResolver));
			this.Tooltip = staffGroup.Name;
			this.IsStatic = false;
		}

		public override string Id
		{
			get { return _groupRef.ToString(false); }
		}

		protected override void PrepareQueryRequest(QueryNoteboxRequest request)
		{
			base.PrepareQueryRequest(request);

			request.StaffGroupRef = _groupRef;
		}
	}

	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted by me")]
	[FolderDescription("OrderNotesSentItemsFolderDescription")]
	internal class SentItemsFolder : OrderNoteboxFolder
	{
		public SentItemsFolder(OrderNoteboxFolderSystem folderSystem)
			: base(folderSystem, "OrderNoteSentItems")
		{
		}
	}
}