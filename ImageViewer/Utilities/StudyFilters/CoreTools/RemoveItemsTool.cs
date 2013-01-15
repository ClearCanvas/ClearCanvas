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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("remove", DefaultToolbarActionSite + "/ToolbarRemoveItems", "RemoveItems")]
	[MenuAction("remove", DefaultContextMenuActionSite + "/MenuRemoveItems", "RemoveItems")]
	[EnabledStateObserver("remove", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("remove", "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
	//
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class RemoveItemsTool : StudyFilterTool
	{
		public void RemoveItems()
		{
			List<IStudyItem> selected = new List<IStudyItem>(base.SelectedItems);
			base.Context.BulkOperationsMode = selected.Count > 50;
			try
			{
				foreach (IStudyItem item in selected)
				{
					base.Items.Remove(item);
					item.Dispose();
				}
				base.RefreshTable();
			}
			finally
			{
				base.Context.BulkOperationsMode = false;
			}
		}
	}
}