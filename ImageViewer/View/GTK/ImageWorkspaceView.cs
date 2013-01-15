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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
//using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.GTK
{
	[ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceViewExtensionPoint))]
	public class ImageWorkspaceView : GtkView, IWorkspaceView
	{
		//private ImageWorkspaceControl _imageWorkspaceControl;
		private ImageWorkspaceDrawingArea _imageWorkspaceControl;
		private ImageWorkspace _imageWorkspace;

		public ImageWorkspaceView()
		{
		}

		public void SetWorkspace(Workspace workspace)
		{
			_imageWorkspace = workspace as ImageWorkspace;
			Platform.CheckForInvalidCast(_imageWorkspace, "workspace", "ImageWorkspace");
		}

		public override object GuiElement
		{
			get 
			{
				if (_imageWorkspaceControl == null)
					//_imageWorkspaceControl = new ImageWorkspaceControl(_imageWorkspace);
					_imageWorkspaceControl = new ImageWorkspaceDrawingArea(_imageWorkspace);
	
				return _imageWorkspaceControl; 
			}
		}
	}
}
