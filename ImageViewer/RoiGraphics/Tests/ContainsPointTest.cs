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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests;
using ClearCanvas.ImageViewer.Tests;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	internal static class ContainsPointTest
	{
		public static bool CheckImageIsBlackWhite(Bitmap image, double tolerance)
		{
			List<double> list = new List<double>();
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					Color c = image.GetPixel(x, y);
					double v = (c.R + c.G + c.B) / 3.0;
					list.Add(Math.Min(255 - v, v));
				}
			}
			Statistics stats = new Statistics(list);
			Trace.WriteLine(string.Format("CheckImageIsBlackWhite: {0}", stats.ToString()));
			return stats.IsEqualTo(0, tolerance);
		}
	}
}

#endif