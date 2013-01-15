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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Form that implements the workspace dialog.
	/// </summary>
	public partial class WorkspaceDialogBoxForm : Form
	{
		private readonly Control _content;
		private readonly Size _idealSize;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dialogBox"></param>
		/// <param name="content"></param>
		internal WorkspaceDialogBoxForm(WorkspaceDialogBox dialogBox, Control content)
			: this(dialogBox.Title, content, dialogBox.Size, dialogBox.SizeHint)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="content"></param>
		/// <param name="exactSize"></param>
		/// <param name="sizeHint"></param>
		private WorkspaceDialogBoxForm(string title, Control content, Size exactSize, DialogSizeHint sizeHint)
		{
			InitializeComponent();
			this.Text = title;

			_content = content;

			// important - if we do not set a minimum size, the full content may not be displayed
			_content.MinimumSize = _content.Size;
			_content.Dock = DockStyle.Fill;

			// adjust size of client area to its ideal size
			var contentSize = exactSize != Size.Empty ? exactSize : SizeHintHelper.TranslateHint(sizeHint, _content.Size);
			this.ClientSize = contentSize;// +new Size(0, 4);

			// record the ideal size for future reference
			_idealSize = this.Size;

			_contentPanel.Controls.Add(_content);
		}

		/// <summary>
		/// Position this form in the centre of the specified control.
		/// </summary>
		internal void CentreInControl(Control control)
		{
			// max size of this form is the size of the specified control
			var maxSize = control.Size;

			// computer centre of host control in screen coordinates
			var centre = control.PointToScreen(new Point(0, 0));
			centre.Offset(control.Width / 2, control.Height / 2);

			// compute size of form
			var w = Math.Min(_idealSize.Width, maxSize.Width);
			var h = Math.Min(_idealSize.Height, maxSize.Height);

			// compute upper left corner location
			var x = centre.X - (w/2);
			var y = centre.Y - (h/2);
			
			// update position
			this.Bounds = new Rectangle(x, y, w, h);
		}
	}
}
