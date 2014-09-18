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

using System.Drawing;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers
{
	internal class ColorBarManager : OverlayManager
	{
		public ColorBarManager()
			: base("ColorBar", "NameColourBarOverlay")
		{
			IconSet = new IconSet("Icons.ColorBarToolSmall.png", "Icons.ColorBarToolMedium.png", "Icons.ColorBarToolLarge.png");
		}

		public override bool IsSelectedByDefault(string modality)
		{
			return false;
		}

		public override void SetOverlayVisible(IPresentationImage image, bool visible)
		{
            var graphic = GetCompositeColorBarGraphic(image, visible);
			if (graphic != null)
				graphic.Visible = visible;
		}

        private static ColorBarCompositeGraphic GetCompositeColorBarGraphic(IPresentationImage image, bool createIfNull)
        {
            var applicationGraphicsProvider = image as IApplicationGraphicsProvider;
            if (image is IColorMapProvider && applicationGraphicsProvider != null)
			{
                var applicationGraphics = applicationGraphicsProvider.ApplicationGraphics;
			    var graphic = applicationGraphics.OfType<ColorBarCompositeGraphic>().FirstOrDefault();
				if (graphic == null && createIfNull)
					applicationGraphics.Add(graphic = new ColorBarCompositeGraphic());

				return graphic;
			}

			return null;
		}

		[Cloneable(false)]
		private class ColorBarCompositeGraphic : CompositeGraphic
		{
			[CloneIgnore]
			private ColorBarGraphic _colorBarGraphic;

			public ColorBarCompositeGraphic()
			{
				base.Graphics.Add(_colorBarGraphic = new ColorBarGraphic());
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ColorBarCompositeGraphic(ColorBarCompositeGraphic source, ICloningContext context)
			{
				context.CloneFields(source, this);
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				_colorBarGraphic = (ColorBarGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ColorBarGraphic);
			}

			public override void OnDrawing()
			{
				// ensure the color bar uses the same color map as the underlying image
				if (base.ParentPresentationImage is IColorMapProvider)
					_colorBarGraphic.ColorMapManager.SetMemento(((IColorMapProvider) base.ParentPresentationImage).ColorMapManager.CreateMemento());
				if (base.ParentPresentationImage != null)
				{
					_colorBarGraphic.CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						_colorBarGraphic.Length = (int) (base.ParentPresentationImage.ClientRectangle.Height*0.3f);
						_colorBarGraphic.Location = new PointF(25, (base.ParentPresentationImage.ClientRectangle.Height - _colorBarGraphic.Length)/2f);
					}
					finally
					{
						_colorBarGraphic.ResetCoordinateSystem();
					}
				}

				base.OnDrawing();
			}
		}
	}
}