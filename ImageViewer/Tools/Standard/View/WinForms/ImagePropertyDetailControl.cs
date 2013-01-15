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

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using Application=ClearCanvas.Desktop.Application;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	internal partial class ImagePropertyDetailControl : UserControl
	{
		public ImagePropertyDetailControl(string name, string description, string value)
		{
			InitializeComponent();

			_richText.Text = value;
			_name.Text = name;
			_description.Text = description;
		}
	}

	internal class DummyComponent : ApplicationComponent
	{
		public DummyComponent()
		{
		}
	}

	internal class CancelController : IButtonControl
	{
		readonly Form _parent;

		public CancelController(Form parent)
		{
			_parent = parent;
		}

		#region IButtonControl Members

		public DialogResult DialogResult
		{
			get
			{
				return System.Windows.Forms.DialogResult.Cancel;
			}
			set
			{
			}
		}

		public void NotifyDefault(bool value)
		{
		}

		public void PerformClick()
		{
			_parent.Close();
		}

		#endregion
	}

	internal class ShowValueDialog : DialogBox
	{
		private ShowValueDialog(string text)
			: base(CreateArgs(), Application.ActiveDesktopWindow)
        {
        }

		public static void Show(string name, string description, string text)
		{
			ShowValueDialog dialog = new ShowValueDialog(text);
			DialogBoxForm form = new DialogBoxForm(dialog, new ImagePropertyDetailControl(name, description, text));
			form.Text = SR.TitleDetails;
			form.CancelButton = new CancelController(form);
			form.StartPosition = FormStartPosition.Manual;
			form.DesktopLocation = Cursor.Position - new Size(form.DesktopBounds.Width/2, form.DesktopBounds.Height/2);
			form.ShowDialog();
			form.Dispose();
		}

		private static DialogBoxCreationArgs CreateArgs()
		{
			DialogBoxCreationArgs args = new DialogBoxCreationArgs();
			args.Component = new DummyComponent();
			args.SizeHint = DialogSizeHint.Auto;
			return args;
		}
	}
}
