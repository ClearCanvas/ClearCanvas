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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("delete", "dicomgraphic-menu/MenuDeleteAnnotation", "Delete")]
	[IconSet("delete", "DeleteAnnotationToolSmall.png", "DeleteAnnotationToolMedium.png", "DeleteAnnotationToolLarge.png")]
	[VisibleStateObserver("delete", "IsInteractive", "IsInteractiveChanged")]
	[GroupHint("delete", "Tools.Annotations.Delete")]
	//
	[MenuAction("unlock", "dicomgraphic-menu/MenuUnlockAnnotation", "Unlock")]
	[VisibleStateObserver("unlock", "Visible", "VisibleChanged")]
	[GroupHint("delete", "Tools.Annotations.Unlock")]
	//
	[ExtensionOf(typeof (GraphicToolExtensionPoint))]
	internal class DicomGraphicAnnotationTool : DeleteAnnotationsTool
	{
		protected new DicomGraphicAnnotation Graphic
		{
			get { return base.Graphic as DicomGraphicAnnotation; }
		}

		public bool IsInteractive
		{
			get { return !Visible; }
		}

		public event EventHandler IsInteractiveChanged
		{
			add { VisibleChanged += value; }
			remove { VisibleChanged -= value; }
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