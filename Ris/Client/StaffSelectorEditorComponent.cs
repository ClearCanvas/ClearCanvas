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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// A specialize Staff SelectorEditComponent that includes the current user.
	/// </summary>
	public class StaffSelectorEditorComponent : SelectorEditorComponent<StaffSummary, StaffSelectorTable>
	{
		public class DummyItem : StaffSummary
		{
			public DummyItem()
			{
				this.Name = new PersonNameDetail();
				this.Name.FamilyName = SR.DummyItemUser;
				this.StaffId = "";
				this.StaffType = new EnumValueInfo("", "");
				this.StaffRef = new EntityRef(typeof(DummyItem), new object(), 0);
			}
		}

		private static readonly StaffSummary _currentUserItem = new DummyItem();

		private static IEnumerable<StaffSummary> CollectionAndCurrentUser(IEnumerable<StaffSummary> items)
		{
			List<StaffSummary> a = new List<StaffSummary>();
			a.Add(_currentUserItem);
			a.AddRange(items);
			return a;
		}

		public StaffSelectorEditorComponent(IEnumerable<StaffSummary> allItems, IEnumerable<StaffSummary> selectedItems, bool includeCurrentUser)
			: base(
				CollectionAndCurrentUser(allItems),
				includeCurrentUser ? CollectionAndCurrentUser(selectedItems) : selectedItems, 
				delegate(StaffSummary s) { return s.StaffRef; })
		{
		}

		public bool IncludeCurrentUser
		{
			get { return base.SelectedItems.Contains(_currentUserItem); }
		}

		public override IList<StaffSummary> SelectedItems
		{
			get
			{
				return CollectionUtils.Select(base.SelectedItems, delegate(StaffSummary staff) { return staff != _currentUserItem; });
			}
		}
	}
}