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
using ClearCanvas.Common;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(ExceptionDialogFactoryExtensionPoint))]
	public class ExceptionDialogFactory : IExceptionDialogFactory
	{
		#region IExceptionDialogFactory Members

		public IExceptionDialog CreateExceptionDialog()
		{
			return new ExceptionDialog();
		}

		#endregion
	}

	public class ExceptionDialog : Desktop.ExceptionDialog
	{
		private DialogBoxForm _form;

		protected override ExceptionDialogAction Show()
		{
			var control = new ExceptionDialogControl(Exception, Message, Actions, CloseForm, CloseForm);
			_form = new DialogBoxForm(Title, control, Size.Empty, DialogSizeHint.Auto);

			var screen = ScreenFromActiveForm() ?? ScreenFromMousePosition();
			int xdiff = screen.Bounds.Width - _form.Bounds.Width;
			int ydiff = screen.Bounds.Height - _form.Bounds.Height;
			int locationX = screen.WorkingArea.Left + Math.Max(0, (xdiff)/2);
			int locationY = screen.WorkingArea.Top + Math.Max(0, (ydiff)/2);

			_form.StartPosition = FormStartPosition.Manual;
			_form.Location = new Point(locationX, locationY);
			//_form.TopMost = true;
			_form.ShowDialog();

			return control.Result;
		}

		private void CloseForm()
		{
			_form.Close();
		}

		private static System.Windows.Forms.Screen ScreenFromActiveForm()
		{
			try
			{
				var activeForm = Form.ActiveForm;
				if (activeForm != null)
					return System.Windows.Forms.Screen.FromControl(activeForm);
			}
			catch
			{
			}

			return null;
		}

		private static System.Windows.Forms.Screen ScreenFromMousePosition()
		{
			return System.Windows.Forms.Screen.FromPoint(Control.MousePosition);
		}
	}
}
