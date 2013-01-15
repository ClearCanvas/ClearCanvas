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

//using ClearCanvas.ImageViewer.Tools;
//using ClearCanvas.ImageViewer.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public class StockWorkspaceTools
    {
		[MenuAction("activate", "MenuFile/MenuFileCloseWorkspace")]
		[ClickHandler("activate", "Activate")]

  		//[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
  		[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
		public class CloseWorkspaceTool : StockTool
        {
            public CloseWorkspaceTool()
            {
            }
			
			/// <summary>
			/// Method OnActivated
			/// </summary>
			/// <param name="sender">An object</param>
			/// <param name="e">An EventArgs</param>
			public void Activate()
			{
				_mainView.CloseActiveWorkspace();
			}
        }
    }
}
