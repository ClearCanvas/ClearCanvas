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
using System.Drawing;
using System.Threading;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal static class InteractiveShutterGraphicBuilders
	{
		private static readonly Color _normalColor = Color.LightSteelBlue;
		private static readonly Color _invalidColor = Color.Red;
		private static readonly int _flashDelay = 100;

		public static InteractiveCircleGraphicBuilder CreateCircularShutterBuilder(IBoundableGraphic graphic)
		{
			return new InteractiveCircularShutterBuilder(graphic);
		}

		public static InteractiveBoundableGraphicBuilder CreateRectangularShutterBuilder(IBoundableGraphic graphic)
		{
			return new InteractiveRectangularShutterBuilder(graphic);
		}

		public static InteractivePolygonGraphicBuilder CreatePolygonalShutterBuilder(IPointsGraphic graphic)
		{
			return new InteractivePolygonalShutterBuilder(graphic);
		}

		private static bool IsShutterTooSmall(IGraphic shutterGraphic)
		{
			shutterGraphic.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				RectangleF boundingBox = shutterGraphic.BoundingBox;
				return (boundingBox.Width < 50 || boundingBox.Height < 50);
			}
			finally
			{
				shutterGraphic.ResetCoordinateSystem();
			}
		}

		private delegate void DoFlashDelegate(IVectorGraphic graphic, SynchronizationContext context);

		private static void DoFlash(IVectorGraphic graphic, SynchronizationContext context)
		{
			context.Send(delegate
			             	{
			             		if (graphic.ImageViewer != null && graphic.ImageViewer.DesktopWindow != null)
			             		{
			             			graphic.Color = _invalidColor;
			             			graphic.Draw();
			             		}
			             	}, null);
			Thread.Sleep(_flashDelay);
			context.Send(delegate
			             	{
			             		if (graphic.ImageViewer != null && graphic.ImageViewer.DesktopWindow != null)
			             		{
			             			graphic.Color = _normalColor;
			             			graphic.Draw();
			             		}
			             	}, null);
		}

		private static void Flash(IGraphic graphic)
		{
			if (graphic is IVectorGraphic)
			{
				DoFlashDelegate doFlashDelegate = DoFlash;
				doFlashDelegate.BeginInvoke((IVectorGraphic) graphic, SynchronizationContext.Current, delegate(IAsyncResult result) { doFlashDelegate.EndInvoke(result); }, null);
			}
		}

		private class InteractiveCircularShutterBuilder : InteractiveCircleGraphicBuilder
		{
			public InteractiveCircularShutterBuilder(IBoundableGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void OnGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					Flash(base.Graphic);
					base.Rollback();
					return;
				}
				base.OnGraphicComplete();
			}
		}

		private class InteractiveRectangularShutterBuilder : InteractiveBoundableGraphicBuilder
		{
			public InteractiveRectangularShutterBuilder(IBoundableGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void OnGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					Flash(base.Graphic);
					base.Rollback();
					return;
				}
				base.OnGraphicComplete();
			}
		}

		private class InteractivePolygonalShutterBuilder : InteractivePolygonGraphicBuilder
		{
			public InteractivePolygonalShutterBuilder(IPointsGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void OnGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					Flash(base.Graphic);
					base.Rollback();
					return;
				}
				base.OnGraphicComplete();
			}
		}
	}
}