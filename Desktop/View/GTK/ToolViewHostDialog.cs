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

using System;
using Gtk;
//using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Desktop.Tools;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
	public class ToolViewHostDialog : Dialog
	{
		private ToolViewProxy _view;
		
		public ToolViewHostDialog(ToolViewProxy view, Window parent)
			:base(view.Title, parent, Gtk.DialogFlags.DestroyWithParent)
		{
			_view = view;
			this.VBox.PackStart((Widget)_view.View.GuiElement, false, false, 0);
		}
		
		
		protected override bool OnDeleteEvent(Gdk.Event e)
		{
			_view.Active = false;
			this.Hide();
			return true;	// don't destroy the dialog
		}
	}
}
