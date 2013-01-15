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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextDetail : DataContractBase
	{
		public CannedTextDetail()
		{
		}

		public CannedTextDetail(string name, string category, StaffGroupSummary staffGroup, string text)
		{
			this.Name = name;
			this.Category = category;
			this.StaffGroup = staffGroup;
			this.Text = text;
		}

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public StaffGroupSummary StaffGroup;

		[DataMember]
		public string Text;

		public bool IsPersonal
		{
			get { return !this.IsGroup; }
		}

		public bool IsGroup
		{
			get { return this.StaffGroup != null; }
		}
	}
}
