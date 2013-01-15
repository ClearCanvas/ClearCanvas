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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	/// <summary>
	/// Extension point for views onto <see cref="PathProfileComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PathProfileComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PathProfileComponent class
	/// </summary>
	[AssociateView(typeof(PathProfileComponentViewExtensionPoint))]
	public class PathProfileComponent : RoiAnalysisComponent
	{
		private int[] _pixelIndices;
		private double[] _pixelValues;

		/// <summary>
		/// Constructor
		/// </summary>
		public PathProfileComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext)
		{
		}

		public int[] PixelIndices
		{
			get { return _pixelIndices; }
		}

		public double[] PixelValues
		{
			get { return _pixelValues; }
		}

		public override void Start()
		{
			// TODO prepare the component for its live phase
			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public bool ComputeProfile()
		{
			IPointsGraphic line = GetSelectedPolyline();

			// For now, make sure the ROI is a polyline
			if (line == null || line.Points.Count != 2)
			{
				this.Enabled = false;
				return false;
			}

			IImageGraphicProvider imageGraphicProvider =
				line.ParentPresentationImage as IImageGraphicProvider;

			if (imageGraphicProvider == null)
			{
				this.Enabled = false;
				return false;
			}

			// For now, only allow ROIs of grayscale images
			GrayscaleImageGraphic image = imageGraphicProvider.ImageGraphic as GrayscaleImageGraphic;

			if (image == null)
			{
				this.Enabled = false;
				return false;
			}

			line.CoordinateSystem = CoordinateSystem.Source;
			Point pt1 = new Point((int)line.Points[0].X, (int)line.Points[0].Y);
			Point pt2 = new Point((int)line.Points[1].X, (int)line.Points[1].Y);

			if (pt1.X < 0 || pt1.X > image.Columns - 1 ||
				pt2.X < 0 || pt2.X > image.Columns - 1 ||
				pt1.Y < 0 || pt1.Y > image.Rows - 1 ||
				pt2.Y < 0 || pt2.Y > image.Rows - 1)
			{
				this.Enabled = false;
				return false;
			}


			List<Point> pixels = BresenhamLine(pt1, pt2);

			_pixelIndices = new int[pixels.Count];
			_pixelValues = new double[pixels.Count];

			int i = 0;

			foreach (Point pixel in pixels)
			{
				int rawPixelValue = image.PixelData.GetPixel(pixel.X, pixel.Y);
				_pixelIndices[i] = i;
				_pixelValues[i] = (int) image.ModalityLut[rawPixelValue];
				i++;
			}

			this.Enabled = true;
			return true;
		}

		protected override bool CanAnalyzeSelectedRoi()
		{
			return GetSelectedPolyline() == null ? false : true;
		}

		private IPointsGraphic GetSelectedPolyline()
		{
			RoiGraphic graphic = GetSelectedRoi();

			if (graphic == null)
				return null;

			IPointsGraphic line = graphic.Subject as IPointsGraphic;

			if (line == null)
				return null;

			return line;
		}


		// Swap the values of A and B
		private void Swap<T>(ref T a, ref T b)
		{
			T c = a;
			a = b;
			b = c;
		}

		// Returns the list of points from p0 to p1 
		private List<Point> BresenhamLine(Point p0, Point p1)
		{
			return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
		}

		// Returns the list of points from (x0, y0) to (x1, y1)
		private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
		{
			// Optimization: it would be preferable to calculate in
			// advance the size of "result" and to use a fixed-size array
			// instead of a list.
			List<Point> result = new List<Point>();

			bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (steep)
			{
				Swap(ref x0, ref y0);
				Swap(ref x1, ref y1);
			}
			if (x0 > x1)
			{
				Swap(ref x0, ref x1);
				Swap(ref y0, ref y1);
			}

			int deltax = x1 - x0;
			int deltay = Math.Abs(y1 - y0);
			int error = 0;
			int ystep;
			int y = y0;
			if (y0 < y1) ystep = 1; else ystep = -1;
			for (int x = x0; x <= x1; x++)
			{
				if (steep) result.Add(new Point(y, x));
				else result.Add(new Point(x, y));
				error += deltay;
				if (2 * error >= deltax)
				{
					y += ystep;
					error -= deltax;
				}
			}

			return result;
		}
	}
}
