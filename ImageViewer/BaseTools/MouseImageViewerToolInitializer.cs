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
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	internal sealed class MouseImageViewerToolInitializer
	{
		private MouseImageViewerToolInitializer()
		{ 
		}

		public static void Initialize(MouseImageViewerTool mouseTool)
		{
			object[] buttonAssignment = mouseTool.GetType().GetCustomAttributes(typeof(MouseToolButtonAttribute), true);
			if (buttonAssignment == null || buttonAssignment.Length == 0)
			{
				throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolShouldHaveDefault, mouseTool.GetType().FullName));
			}
			else
			{
				MouseToolButtonAttribute attribute = buttonAssignment[0] as MouseToolButtonAttribute;

				if (attribute.MouseButton == XMouseButtons.None)
				{
					throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolShouldHaveDefault, mouseTool.GetType().FullName));
				}
				else
				{
					mouseTool.MouseButton = attribute.MouseButton;
					mouseTool.Active = attribute.InitiallyActive;
				}
			}
		}
	}
}
