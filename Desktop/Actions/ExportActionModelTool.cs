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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using System.IO;

namespace ClearCanvas.Desktop.Actions
{
#if DEBUG   // only include this tool in debug builds

	/// <summary>
	/// Exports the in-memory action model to a file.
	/// </summary>
	[MenuAction("apply", "global-menus/MenuTools/MenuUtilities/Export Action Model", "Apply")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ExportActionModelTool : Tool<IDesktopToolContext>
	{
		internal void Apply()
		{
			FileDialogResult result = this.Context.DesktopWindow.ShowSaveFileDialogBox(new FileDialogCreationArgs("actionmodel.xml"));
			if(result.Action == DialogBoxAction.Ok)
			{
				try
				{
					using (StreamWriter sw = File.CreateText(result.FileName))
					{
						using (XmlTextWriter writer = new XmlTextWriter(sw))
						{
							writer.Formatting = Formatting.Indented;
                            ActionModelSettings.DefaultInstance.Export(writer);
						}
					}
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
		}
	}

#endif
}
