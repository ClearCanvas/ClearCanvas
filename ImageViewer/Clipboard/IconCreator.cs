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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations.Utilities;
using ClearCanvas.ImageViewer.Graphics.Utilities;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard
{
	internal static class IconCreator
	{
		private static readonly int _iconWidth = 100;
		private static readonly int _iconHeight = 100;

		public static Bitmap CreatePresentationImageIcon(IPresentationImage image, Rectangle? clientRectangle = null)
		{
			float aspectRatio = _iconHeight/(float) _iconWidth;

			var clientRectangle2 = clientRectangle.HasValue && !clientRectangle.Value.IsEmpty ? clientRectangle.Value : image.ClientRectangle;
			int dimension = Math.Max(clientRectangle2.Width, clientRectangle2.Height);

			// rendered twice to rasterize the invariant text first and then downsample to icon size
			Bitmap img = DrawToIcon(image, dimension, (int) (dimension*aspectRatio));
			Bitmap bmp = new Bitmap(img, _iconWidth, _iconHeight);

			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
			img.Dispose();

			Pen pen = new Pen(Color.DarkGray);

			g.DrawRectangle(pen, 0, 0, _iconWidth - 1, _iconHeight - 1);

			pen.Dispose();
			g.Dispose();

			return bmp;
		}

		public static Bitmap CreateDisplaySetIcon(IDisplaySet displaySet, Rectangle displayedClientRectangle)
		{
			Bitmap bmp = new Bitmap(_iconWidth, _iconHeight);
			int numImages = 3;
			float aspectRatio = _iconHeight/(float) _iconWidth;
			int offsetX = 10;
			int offsetY = (int) (aspectRatio*offsetX);
			int subIconWidth = _iconWidth - ((numImages - 1)*offsetX);
			int subIconHeight = (int) (aspectRatio*subIconWidth);

			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);

			int dimension = Math.Max(displayedClientRectangle.Width, displayedClientRectangle.Height);

			IPresentationImage image = GetMiddlePresentationImage(displaySet);

			// rendered twice to rasterize the invariant text first and then downsample to icon size
			Bitmap img = DrawToIcon(image, dimension, (int) (dimension*aspectRatio));
			Bitmap iconBmp = new Bitmap(img, subIconWidth, subIconHeight);
			img.Dispose();

			Pen pen = new Pen(Color.DarkGray);

			g.Clear(Color.Black);

			for (int i = 0; i < numImages; i++)
			{
				int x = offsetX*((numImages - 1) - i);
				int y = offsetY*i;

				g.DrawImage(iconBmp, new Point(x, y));
				g.DrawRectangle(pen, x, y, subIconWidth - 1, subIconHeight - 1);
			}

			pen.Dispose();
			iconBmp.Dispose();
			g.Dispose();

			return bmp;
		}

		private static Bitmap DrawToIcon(IPresentationImage image, int width, int height)
		{
			//We just hide the text overlay and application graphics because it creates ugly icons.
			var textOverlayHider = new TextOverlayVisibilityHelper(image);
			var applicationGraphicsHider = GraphicsVisibilityHelper.CreateForApplicationGraphics(image);
			textOverlayHider.Hide();
			applicationGraphicsHider.HideAll();

			try
			{
				return image.DrawToBitmap(width, height);
			}
			catch (Exception ex)
			{
				// rendering the error text to a 100x100 icon is useless, so we'll just paint a placeholder error icon and log the icon error
				Platform.Log(LogLevel.Warn, ex, "Failed to render icon with dimensions {0}x{1}", width, height);
				var bitmap = new Bitmap(width, height);
				using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
				{
					graphics.FillRectangle(Brushes.Black, 0, 0, width, height);
					graphics.DrawLine(Pens.WhiteSmoke, 0, 0, width, height);
					graphics.DrawLine(Pens.WhiteSmoke, 0, height, width, 0);
				}
				return bitmap;
			}
			finally
			{
				textOverlayHider.Restore();
				applicationGraphicsHider.RestoreAll();
			}
		}

		private static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
		{
			if (displaySet.PresentationImages.Count == 0)
				throw new ArgumentException("The display set must contain at least one image.");

			if (displaySet.PresentationImages.Count <= 2)
				return displaySet.PresentationImages[0];

			return displaySet.PresentationImages[displaySet.PresentationImages.Count/2];
		}
	}
}