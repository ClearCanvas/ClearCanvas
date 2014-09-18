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

using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers
{
	internal class ScaleOverlayManager : OverlayManager
	{
		public ScaleOverlayManager()
			: base("ScaleOverlay", "NameScaleOverlay")
		{
			IconSet = new IconSet("Icons.ScaleOverlayToolSmall.png", "Icons.ScaleOverlayToolMedium.png", "Icons.ScaleOverlayToolLarge.png");
		}

		public override bool IsSelectedByDefault(string modality)
		{
			return modality != "MG";
		}

		public override void SetOverlayVisible(IPresentationImage image, bool visible)
		{
			CompositeScaleGraphic scale = GetCompositeScaleGraphic(image, visible);
			if (scale != null)
				scale.Visible = visible;
		}

		private static CompositeScaleGraphic GetCompositeScaleGraphic(IPresentationImage image, bool createIfNull)
		{
			var applicationGraphicsProvider = image as IApplicationGraphicsProvider;
		    if (applicationGraphicsProvider == null) return null;

		    var overlayGraphics = applicationGraphicsProvider.ApplicationGraphics;
		    var scale = overlayGraphics.OfType<CompositeScaleGraphic>().FirstOrDefault();
		    if (scale == null && createIfNull)
		        overlayGraphics.Insert(0, scale = new CompositeScaleGraphic());

		    return scale;
		}
	}
}