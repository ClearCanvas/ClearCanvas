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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tests
{
	/// <summary>
	/// Creates various test pattern <see cref="ImageGraphic"/>s.
	/// </summary>
	internal static class TestPattern
	{
		/// <summary>
		/// Creates a test pattern of <paramref name="size"/> consisting of red, blue, green and black in the NW, NE, SW, SE corners respectively.
		/// </summary>
		public static ColorImageGraphic CreateRGBKCorners(Size size)
		{
			int width = size.Width;
			int height = size.Height;
			int halfWidth = width/2;
			int halfHeight = height/2;

			ColorImageGraphic imageGraphic = new ColorImageGraphic(height, width);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Color c;
					if (x < halfWidth)
						c = y < halfHeight ? Color.Red : Color.LimeGreen;
					else
						c = y < halfHeight ? Color.Blue : Color.Black;

					imageGraphic.PixelData.SetPixel(x, y, c);
				}
			}
			return imageGraphic;
		}

		/// <summary>
		/// Creates a test pattern of <paramref name="size"/> consisting of a NW to SE black to white gradient.
		/// </summary>
		public static GrayscaleImageGraphic CreateGraydient(Size size)
		{
			int width = size.Width;
			int height = size.Height;

			GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(height, width);

			int range = (1 << imageGraphic.BitsStored) - 1;
			int offset = imageGraphic.IsSigned ? -(1 << (imageGraphic.BitsStored - 1)) : 0;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int v = (int) (1f*(x+y)/(width+height)*range + offset);
					imageGraphic.PixelData.SetPixel(x, y, v);
				}
			}
			return imageGraphic;
		}

		/// <summary>
		/// Creates a test pattern of <paramref name="size"/> consisting of a black and white checkboard.
		/// </summary>
		public static GrayscaleImageGraphic CreateCheckerboard(Size size)
		{
			int width = size.Width;
			int height = size.Height;

			GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(height, width);

			int minValue = imageGraphic.IsSigned ? -(1 << (imageGraphic.BitsStored - 1)) : 0;
			int maxValue = (1 << imageGraphic.BitsStored) + minValue - 1;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int file = (int) (8f*x/width);
					int rank = (int) (8f*y/height);
					int v = (file%2 == 0) ^ (rank%2 == 0) ? minValue : maxValue;
					imageGraphic.PixelData.SetPixel(x, y, v);
				}
			}

			return imageGraphic;
		}
	}
}

#endif