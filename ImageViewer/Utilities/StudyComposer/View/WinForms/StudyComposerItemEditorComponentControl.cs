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

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms
{
	public partial class StudyComposerItemEditorComponentControl : UserControl
	{
		private readonly StudyComposerItemEditorComponent _component;

		public StudyComposerItemEditorComponentControl()
		{
			InitializeComponent();
		}

		public StudyComposerItemEditorComponentControl(StudyComposerItemEditorComponent component) : this()
		{
			_component = component;
			Image icon = _component.Icon;
			if (icon == null)
			{
				picIcon.Visible = false;
			}
			else
			{
				this.IconSize = icon.Size;
				picIcon.Image = icon;
			}

			lblName.DataBindings.Add("Text", _component, "Name");
			lblDescription.DataBindings.Add("Text", _component, "Description");
			pgvProps.SelectedObject = _component.Node;
		}

		public Size IconSize
		{
			get { return picIcon.Size; }
			set
			{
				if (picIcon.Size != value)
				{
					pnlHeader.Size = new Size(pnlHeader.Width, value.Height + pnlHeader.Padding.Vertical);
					picIcon.Size = value;
				}
			}
		}

		private void btnOk_Click(object sender, EventArgs e) {
			_component.Ok();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			_component.Cancel();
		}

		private void btnApply_Click(object sender, EventArgs e) {
			_component.Apply();
		}
	}
}