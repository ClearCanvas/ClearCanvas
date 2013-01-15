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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Represents a static, rectangular region of interest for the purposes of computing statistics on the contained pixels.
	/// </summary>
	public class RectangularRoi : Roi, IRoiAreaProvider, IRoiStatisticsProvider
	{
		private readonly RectangleF _rectangle;
		private Units _units;

		/// <summary>
		/// Constructs a new rectangular region of interest, specifying an <see cref="IBoundableGraphic"/> as the source of the definition and pixel data.
		/// </summary>
		/// <param name="rectangle">The rectangular graphic that represents the region of interest.</param>
		public RectangularRoi(IBoundableGraphic rectangle) : base(rectangle.ParentPresentationImage)
		{
			rectangle.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				// note that this is the rectangle complete with orientation information
				_rectangle = rectangle.Rectangle;
			}
			finally
			{
				rectangle.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Constructs a new rectangular region of interest.
		/// </summary>
		/// <param name="rectangle">The region of interest.</param>
		/// <param name="presentationImage">The image containing the source pixel data.</param>
		public RectangularRoi(RectangleF rectangle, IPresentationImage presentationImage) : base(presentationImage)
		{
			_rectangle = rectangle;
		}

		/// <summary>
		/// Gets the rectangular region of interest.
		/// </summary>
		/// <remarks>
		/// This value maintains the original orientation of the rectangle when the <see cref="RectangularRoi"/> was constructed.
		/// </remarks>
		public RectangleF Rectangle
		{
			get { return _rectangle; }
		}

		/// <summary>
		/// Gets or sets the units of area with which to compute the value of <see cref="IRoiAreaProvider.Area"/>.
		/// </summary>
		public Units Units
		{
			get { return _units; }
			set { _units = value; }
		}

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated with physical dimensions.
		/// </summary>
		public bool IsCalibrated
		{
			get { return !base.NormalizedPixelSpacing.IsNull; }
		}

		/// <summary>
		/// Called by <see cref="Roi.BoundingBox"/> to compute the tightest bounding box of the region of interest.
		/// </summary>
		/// <remarks>
		/// <para>This method is only called once and the result is cached for future accesses.</para>
		/// <para>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </para>
		/// </remarks>
		/// <returns>A rectangle defining the bounding box.</returns>
		protected override RectangleF ComputeBounds()
		{
			return RectangleUtilities.ConvertToPositiveRectangle(_rectangle);
		}

		/// <summary>
		/// Creates a copy of this <see cref="RectangularRoi"/> using the same region of interest shape but using a different image as the source pixel data.
		/// </summary>
		/// <param name="presentationImage">The image upon which to copy this region of interest.</param>
		/// <returns>A new <see cref="RectangularRoi"/> of the same shape as the current region of interest.</returns>
		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new RectangularRoi(_rectangle, presentationImage);
		}

		/// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="point">The point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public override bool Contains(PointF point)
		{
			return this.BoundingBox.Contains(point);
		}

		#region IRoiStatisticsProvider Members

		private RoiStatistics _statistics;

		/// <summary>
		/// Gets the standard deviation of the values over the <see cref="Roi"/>.
		/// </summary>
		public double StandardDeviation
		{
			get
			{
				if (_statistics == null)
				{
					_statistics = RoiStatistics.Calculate(this);
				}
				return _statistics.StandardDeviation;
			}
		}

		/// <summary>
		/// Gets the mean of the values over the <see cref="Roi"/>.
		/// </summary>
		public double Mean
		{
			get
			{
				if (_statistics == null)
				{
					_statistics = RoiStatistics.Calculate(this);
				}
				return _statistics.Mean;
			}
		}

		#endregion

		#region IRoiAreaProvider Members

		private double? _area;

		/// <summary>
		/// Gets the area of the region of interest in the units of area as specified by <see cref="IRoiAreaProvider.Units"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">If <see cref="IRoiAreaProvider.Units"/> is a physical
		/// unit of measurement and the image has no pixel spacing information, nor has it been calibrated.</exception>
		public double Area
		{
			get
			{
				try
				{
					if (!_area.HasValue)
					{
						_area = Math.Abs(_rectangle.Width*_rectangle.Height);
					}
					return ConvertFromSquarePixels(_area.Value, _units, base.NormalizedPixelSpacing);
				}
				catch (ArgumentException ex)
				{
					throw new InvalidOperationException("Pixel spacing must be calibrated in order to compute physical units.", ex);
				}
			}
		}

		#endregion
	}
}