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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("unlock", "basicgraphic-menu/MenuUnlockAnnotation", "Unlock")]
	[VisibleStateObserver("unlock", "Visible", "VisibleChanged")]
	//
	[ExtensionOf(typeof (GraphicToolExtensionPoint))]
	internal class DicomGraphicAnnotationTool : GraphicTool
	{
		protected new DicomGraphicAnnotation Graphic
		{
			get { return base.Graphic as DicomGraphicAnnotation; }
		}

		public override void Initialize()
		{
			var graphic = Graphic;
			Visible = graphic != null && !graphic.Interactive;
		}

		public void Unlock()
		{
			var graphic = Graphic;
			if (graphic != null && !graphic.Interactive)
			{
				graphic.Interactive = true;
				Visible = false;
			}
		}
	}
}