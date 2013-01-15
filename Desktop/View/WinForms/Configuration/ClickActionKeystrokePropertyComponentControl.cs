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
using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration.ActionModel;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class ClickActionKeystrokePropertyComponentControl : UserControl
	{
		private static readonly IList<XKeys> _invalidKeyStrokes;
		private readonly ClickActionKeystrokePropertyComponent _component;

		public ClickActionKeystrokePropertyComponentControl(ClickActionKeystrokePropertyComponent component)
		{
			InitializeComponent();

			_component = component;

			_keyStrokeCaptureBox.DataBindings.Add("KeyStroke", component, "KeyStroke", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _keyStrokeCaptureBox_ValidateKeyStroke(object sender, ValidateKeyStrokeEventArgs e)
		{
			e.IsValid = e.IsValid && !_invalidKeyStrokes.Contains(e.KeyStroke) && _component.IsValidKeyStroke(e.KeyStroke);
		}

		static ClickActionKeystrokePropertyComponentControl()
		{
			// these invalid key strokes are specific to the way we listen for keyboard events using the WinForms toolkit
			// they may be different depending on platform and toolkit, which is why this logic is not implemented model-side.
			var invalidKeyStrokes = new List<XKeys>();
			invalidKeyStrokes.Add(XKeys.Control | XKeys.Alt | XKeys.Delete);
			invalidKeyStrokes.Add(XKeys.Control | XKeys.Shift | XKeys.Escape);
			invalidKeyStrokes.Add(XKeys.Control | XKeys.Escape);
			invalidKeyStrokes.Add(XKeys.Alt | XKeys.PrintScreen);
			invalidKeyStrokes.Add(XKeys.Alt | XKeys.Tab);
			invalidKeyStrokes.Add(XKeys.PrintScreen);
			invalidKeyStrokes.Add(XKeys.LeftWinKey);
			invalidKeyStrokes.Add(XKeys.RightWinKey);
			_invalidKeyStrokes = invalidKeyStrokes.AsReadOnly();
		}
	}
}