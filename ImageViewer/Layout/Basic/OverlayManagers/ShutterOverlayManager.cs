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

using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers
{
	internal class ShutterOverlayManager : OverlayManager
	{
		public ShutterOverlayManager()
			: base("ShutterOverlay", "NameShutterOverlay")
		{
			IconSet = new IconSet("Icons.ShutterOverlayToolSmall.png", "Icons.ShutterOverlayToolMedium.png", "Icons.ShutterOverlayToolLarge.png");
		}

		public override bool IsSelectedByDefault(string modality)
		{
			return true;
		}

		public override void SetOverlayVisible(IPresentationImage image, bool visible)
		{
			var dicomImage = image as IDicomPresentationImage;
			if (dicomImage == null)
				return;

			var dicomGraphics = DicomGraphicsPlane.GetDicomGraphicsPlane(dicomImage, true);
			if (dicomGraphics != null)
				dicomGraphics.Shutters.Enabled = visible;
		}
	}
}