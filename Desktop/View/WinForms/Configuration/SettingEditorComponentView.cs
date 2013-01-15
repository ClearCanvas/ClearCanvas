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
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="SettingEditorComponent"/>
	/// </summary>
	[ExtensionOf(typeof(SettingEditorComponentViewExtensionPoint))]
	public class SettingEditorComponentView : WinFormsView, IApplicationComponentView
	{
		private SettingEditorComponent _component;
		private SettingEditorComponentControl _control;


		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (SettingEditorComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new SettingEditorComponentControl(_component);
				}
				return _control;
			}
		}
	}
}