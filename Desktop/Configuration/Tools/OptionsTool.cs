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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration.Tools
{
	/// <summary>
	/// A tool that, when executed, will show a configuration dialog.
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/MenuOptions", "Show", KeyStroke = XKeys.Control | XKeys.O)]
	[Tooltip("show", "MenuOptions")]
	[IconSet("show", "Icons.OptionsToolSmall.png", "Icons.OptionsToolMedium.png", "Icons.OptionsToolLarge.png")]
	[GroupHint("show", "Application.Options")]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class OptionsTool : Tool<IDesktopToolContext>
	{
		/// <summary>
		/// Called by the framework when the user clicks the "Options" menu item.
		/// </summary>
		public void Show()
		{
			try
			{
				ConfigurationDialog.Show(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
