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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("show", DefaultToolbarActionSite + "/ToolbarAddRemoveColumns", "Show")]
	[IconSet("show", "Icons.AddRemoveColumnsToolSmall.png", "Icons.AddRemoveColumnsToolMedium.png", "Icons.AddRemoveColumnsToolLarge.png")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class AddRemoveColumnsTool : StudyFilterTool
	{
		public void Show()
		{
			ColumnPickerComponent component = new ColumnPickerComponent(base.Columns);
			SimpleComponentContainer container = new SimpleComponentContainer(component);
			DialogBoxAction action = base.DesktopWindow.ShowDialogBox(container, SR.AddRemoveColumns);
			if (action == DialogBoxAction.Ok)
			{
				base.Columns.Clear();
				foreach (StudyFilterColumn.ColumnDefinition column in component.Columns)
				{
					base.Columns.Add(column.Create());
				}

				base.RefreshTable();
			}
		}
	}
}