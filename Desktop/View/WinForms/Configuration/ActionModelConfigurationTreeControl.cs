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
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	[Obsolete("See JY", true)]
	public class ActionModelConfigurationTreeControl : BindingTreeView
	{
		public ActionModelConfigurationTreeControl() : base()
		{
			base.TreeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
			base.TreeView.DrawNode += OnTreeViewDrawNode;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.TreeView.DrawNode -= OnTreeViewDrawNode;
			}
			base.Dispose(disposing);
		}

		private void OnTreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (e.Node.IsEditing)
				return;

			Font font = e.Node.NodeFont ?? base.TreeView.Font;
			Color fontColor = !e.Node.ForeColor.IsEmpty ? e.Node.ForeColor : base.TreeView.ForeColor;
			if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected && (e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused)
				fontColor = SystemColors.HighlightText;

			using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
			{
				sf.HotkeyPrefix = HotkeyPrefix.Show;
				sf.Alignment = StringAlignment.Near;
				sf.LineAlignment = StringAlignment.Center;

				Rectangle newNodeBounds = new Rectangle(e.Bounds.X + 3, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				using (Brush b = new SolidBrush(fontColor))
				{
					e.Graphics.DrawString(e.Node.Text, font, b, newNodeBounds, sf);
				}
			}
		}
	}
}