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

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	public partial class IconCheckBox : UserControl
	{
		public IconCheckBox()
		{
			InitializeComponent();
		}

		public bool Checked
		{
			get { return _check.Checked; }
			set { _check.Checked = value; }
		}

		public bool CheckEnabled
		{
			get { return _check.Enabled; }
			set { _check.Enabled = value; }
		}

		public event EventHandler CheckedChanged
		{
			add { _check.CheckedChanged += value; }
			remove { _check.CheckedChanged -= value; }
		}

		public override string Text
		{
			get { return _checkLabel.Text; }
			set { _checkLabel.Text = value; }
		}

		public Image Image
		{
			get { return _icon.Image; }
			set { _icon.Image = value; }
		}
	}
}