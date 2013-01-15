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

using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	[ExtensionOf(typeof(ConfigurationDialogComponentViewExtensionPoint))]
	public class ConfigurationDialogComponentView : WinFormsApplicationComponentView<ConfigurationDialogComponent>
	{
		protected override object CreateGuiElement()
		{
			// NOTE: Yeah, this is a bit weird, but the ConfigurationDialogComponent
			// cannot be a container because the NavigatorComponentContainer cannot be hosted - it
			// has to be at the top level because it has cancel and accept buttons.

			var view = (IApplicationComponentView) ViewFactory.CreateAssociatedView(typeof (ConfigurationDialogComponent).BaseType);
			view.SetComponent(Component);
			var navigatorControl = (Control) view.GuiElement;

			return new ConfigurationDialogComponentControl(Component, navigatorControl);
		}
	}
}
