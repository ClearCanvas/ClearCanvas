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
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers
{
	internal class TextOverlayManager : OverlayManager
	{
		public TextOverlayManager()
			: base("TextOverlay", "NameTextOverlay")
		{
			IconSet = new IconSet("Icons.TextOverlayToolSmall.png", "Icons.TextOverlayToolMedium.png", "Icons.TextOverlayToolLarge.png");
		}

		public override bool IsSelectedByDefault(string modality)
		{
			return modality != "US";
		}

		public override void SetOverlayVisible(IPresentationImage image, bool visible)
		{
			var annotationLayoutProvider = image as IAnnotationLayoutProvider;
			if (annotationLayoutProvider == null)
				return;

			foreach (var box in annotationLayoutProvider.AnnotationLayout.AnnotationBoxes)
				box.Visible = visible;
		}
	}
}