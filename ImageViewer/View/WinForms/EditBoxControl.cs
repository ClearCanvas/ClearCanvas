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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	/// <summary>
	/// A <see cref="TextBox"/> control designed for <see cref="EditBox"/>es.
	/// </summary>
	internal class EditBoxControl : TextBox
	{
		private EditBox _editBox;
		private bool _hasChanges = false;

		public EditBoxControl()
		{
			// we can't do a transparent background unless we user paint it...
			// base.SetStyle(ControlStyles.UserPaint, true);
			// base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			// base.BackColor = Color.FromArgb(128, 128, 128, 128);

			base.AcceptsReturn = false;
			base.AcceptsTab = false;
			base.BackColor = Color.Black;
			base.BorderStyle = BorderStyle.None;
			base.ForeColor = Color.Tomato;
			base.Multiline = true;
			base.Visible = false;
			base.WordWrap = false;
		}

		public EditBox EditBox
		{
			get { return _editBox; }
			set
			{
				if (_editBox != null)
				{
					base.Visible = false;
				}

				_editBox = value;

				if (_editBox != null)
				{
					base.Font = new Font(_editBox.FontName, _editBox.FontSize);
					base.Text = _editBox.Value;
					base.Bounds = ComputeEditBoxControlBounds(this, _editBox);
					base.Visible = true;
					base.Focus();
					base.SelectAll();
				}

				_hasChanges = false;
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
				e.IsInputKey = true;
			else if (e.KeyCode == Keys.Tab)
				e.IsInputKey = false;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			if (_editBox == null)
				return;

			_editBox.Value = base.Text;
			_hasChanges = true;
			base.Bounds = ComputeEditBoxControlBounds(this, _editBox);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			if (_editBox == null)
				return;

			// if the user clicks away, then we infer if the user meant to accept or cancel
			// based on if the user has actually typed into the control
			if (_hasChanges)
				_editBox.Accept();
			else 
				_editBox.Cancel();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (_editBox == null)
				return;

			if (e.KeyCode == Keys.Escape)
			{
				_editBox.Cancel();
				e.Handled = e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Enter && e.Modifiers == 0)
			{
				_editBox.Accept();
				e.Handled = e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Enter && !_editBox.Multiline)
			{
				e.Handled = e.SuppressKeyPress = true;
			}
		}

		private static Rectangle ComputeEditBoxControlBounds(Control control, EditBox editBox)
		{
			Size sz = control.GetPreferredSize(Size.Empty);
			sz = new Size(Math.Max(Math.Max(sz.Width, editBox.Size.Width), 50), Math.Max(Math.Max(sz.Height, editBox.Size.Height), 21));
			return RectangleUtilities.ConvertToRectangle(editBox.Location, sz);
		}
	}
}