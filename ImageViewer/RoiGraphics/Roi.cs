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
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Represents a static region of interest for the purposes of computing statistics on the contained pixels.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Roi"/> objects are static definitions of a region of interest on a particular image. Its
	/// shape definition and the underlying pixel values are considered fixed upon construction, and hence its
	/// various properties and statistics are non-changing.
	/// </para>
	/// <para>
	/// New instances of a <see cref="Roi"/> should be constructed everytime the definition of the region of
	/// interest or the underlying image pixel data has changed. The <see cref="IGraphic.GetRoi"/>
	/// method allows client code to quickly construct a new instance of a <see cref="Roi"/> based on the current
	/// definition of the graphic and the image it currently belongs to.
	/// </para>
	/// </remarks>
	public abstract class Roi
	{
		private readonly int _imageRows;
		private readonly int _imageColumns;
		private readonly string _modality;
		private readonly bool _subnormalModalityLut;
		private readonly PixelData _pixelData;
		private readonly PixelAspectRatio _pixelAspectRatio;
		private readonly PixelSpacing _normalizedPixelSpacing;
		private readonly RescaleUnits _modalityLutUnits;
		private readonly IModalityLut _modalityLut;
		private readonly IPresentationImage _presentationImage;

		private RectangleF _boundingBox;

		/// <summary>
		/// Constructs a new region of interest, specifying an <see cref="IPresentationImage"/> as the source of the pixel data.
		/// </summary>
		/// <param name="presentationImage">The image containing the source pixel data.</param>
		protected Roi(IPresentationImage presentationImage)
		{
			_presentationImage = presentationImage;

			IImageGraphicProvider provider = presentationImage as IImageGraphicProvider;
			if (provider != null)
			{
				_imageRows = provider.ImageGraphic.Rows;
				_imageColumns = provider.ImageGraphic.Columns;
				_pixelData = provider.ImageGraphic.PixelData;
			}

			var modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				_modalityLut = (modalityLutProvider).ModalityLut;

			if (presentationImage is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider) presentationImage).Frame;
				_normalizedPixelSpacing = frame.NormalizedPixelSpacing;
				_pixelAspectRatio = frame.PixelAspectRatio;
				_modality = frame.ParentImageSop.Modality;
				_modalityLutUnits = frame.RescaleUnits;
				_subnormalModalityLut = frame.IsSubnormalRescale;
			}
			else
			{
				_normalizedPixelSpacing = new PixelSpacing(0, 0);
				_pixelAspectRatio = new PixelAspectRatio(0, 0);
				_modalityLutUnits = RescaleUnits.None;
				_subnormalModalityLut = false;
			}
		}

		///<summary>
		/// Gets the presentation image from which the ROI was created.
		///</summary>
		public IPresentationImage PresentationImage
		{
			get { return _presentationImage; }
		}

		/// <summary>
		/// Gets the number of rows in the entire image.
		/// </summary>
		public int ImageRows
		{
			get { return _imageRows; }
		}

		/// <summary>
		/// Gets the number of columns in the entire image.
		/// </summary>
		public int ImageColumns
		{
			get { return _imageColumns; }
		}

		/// <summary>
		/// Gets the pixel dimensions of the image.
		/// </summary>
		public Size ImageSize
		{
			get { return new Size(_imageColumns, _imageRows); }
		}

		/// <summary>
		/// Gets the pixel data of the image.
		/// </summary>
		public PixelData PixelData
		{
			get { return _pixelData; }
		}

		/// <summary>
		/// Gets the pixel aspect ratio of the image.
		/// </summary>
		public PixelAspectRatio PixelAspectRatio
		{
			get { return _pixelAspectRatio; }
		}

		/// <summary>
		/// Gets the normalized pixel spacing of the image.
		/// </summary>
		public PixelSpacing NormalizedPixelSpacing
		{
			get { return _normalizedPixelSpacing; }
		}

		/// <summary>
		/// Gets the modality LUT of the image, if one exists.
		/// </summary>
		public IModalityLut ModalityLut
		{
			get { return _modalityLut; }
		}

		/// <summary>
		/// Gets the units of the modality LUT output of the image.
		/// </summary>
		public RescaleUnits ModalityLutUnits
		{
			get { return _modalityLutUnits; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the modality LUT is subnormal (i.e. the modality LUT output all map to a single distinct value).
		/// </summary>
		public bool SubnormalModalityLut
		{
			get { return _subnormalModalityLut; }
		}

		/// <summary>
		/// Gets the modality of the image.
		/// </summary>
		public string Modality
		{
			get { return _modality; }
		}

		/// <summary>
		/// Gets the tightest bounding box containing the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		public RectangleF BoundingBox
		{
			get
			{
				if (_boundingBox.IsEmpty)
					_boundingBox = RectangleUtilities.ConvertToPositiveRectangle(ComputeBounds());

				return _boundingBox;
			}
		}

		/// <summary>
		/// Returns a bounding box that has been rounded to the nearest whole pixel(s).
		/// </summary>
		/// <remarks>
		/// Uses <see cref="RectangleUtilities.RoundInflate"/> to ensure the bounding box
		/// returned will encompass every pixel that is inside the ROI.
		/// </remarks>
		/// <param name="constrainToImage">Whether or not the returned rectangle should also be constrained to the image bounds.</param>
		public Rectangle GetBoundingBoxRounded(bool constrainToImage)
		{
			Rectangle bounds = RectangleUtilities.RoundInflate(BoundingBox);
			if (constrainToImage)
			{
				bounds.Intersect(new Rectangle(0, 0, ImageSize.Width - 1, ImageSize.Height - 1));
			}

			return bounds;
		}

		/// <summary>
		/// Called by <see cref="BoundingBox"/> to compute the tightest bounding box of the region of interest.
		/// </summary>
		/// <remarks>
		/// <para>This method is only called once and the result is cached for future accesses.</para>
		/// <para>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </para>
		/// </remarks>
		/// <returns>A rectangle defining the bounding box.</returns>
		protected abstract RectangleF ComputeBounds();

		/// <summary>
		/// Creates a copy of this <see cref="Roi"/> using the same region of interest shape but using a different image as the source pixel data.
		/// </summary>
		/// <param name="presentationImage">The image upon which to copy this region of interest.</param>
		/// <returns>A new <see cref="Roi"/> of the same type and shape as the current region of interest.</returns>
		public abstract Roi CopyTo(IPresentationImage presentationImage);

		/// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="point">The point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public abstract bool Contains(PointF point);

		/// <summary>
		/// Tests to see whether or not the region of interest actual contains any pixel data.
		/// </summary>
		/// <returns>True if the region of interest contains pixel data; False otherwise..</returns>
		public virtual bool ContainsPixelData
		{
			get { return IsBoundingBoxInImage(); }
		}

		/// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="x">The X-coordinate of the point to test.</param>
		/// <param name="y">The Y-coordinate of the point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public bool Contains(float x, float y)
		{
			return Contains(new PointF(x, y));
		}

		/// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="x">The X-coordinate of the point to test.</param>
		/// <param name="y">The Y-coordinate of the point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public bool Contains(int x, int y)
		{
			return Contains(new PointF(x, y));
		}

		/// <summary>
		/// Gets an enumeration of the coordinates of points contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <returns>An enumeration of points.</returns>
		public IEnumerable<PointF> GetPixelCoordinates()
		{
			Rectangle bounds = GetBoundingBoxRounded(true);
			for (int x = bounds.Left; x <= bounds.Right; x++)
			{
				for (int y = bounds.Top; y <= bounds.Bottom; y++)
				{
					PointF p = new PointF(x, y);
					if (Contains(p))
						yield return p;
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of the raw pixel values contained within the region of interest.
		/// </summary>
		/// <returns>An enumeration of raw pixel values.</returns>
		public IEnumerable<int> GetRawPixelValues()
		{
			if (PixelData == null)
				yield break;

			Rectangle bounds = GetBoundingBoxRounded(true);
			for (int x = bounds.Left; x <= bounds.Right; x++)
			{
				for (int y = bounds.Top; y <= bounds.Bottom; y++)
				{
					PointF p = new PointF(x, y);
					if (Contains(p))
						yield return PixelData.GetPixel(x, y);
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of the modality LUT transformed pixel values contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// If the <see cref="ModalityLut"/> is null, then this method enumerates the same values as <see cref="GetRawPixelValues"/>.
		/// </remarks>
		/// <returns>An enumeration of modality LUT transformed pixel values.</returns>
		public IEnumerable<double> GetPixelValues()
		{
			if (PixelData == null)
				yield break;

			LutFunction lut = v => v;
			if (ModalityLut != null)
				lut = v => ModalityLut[v];

			Rectangle bounds = GetBoundingBoxRounded(true);
			for (int x = bounds.Left; x <= bounds.Right; x++)
			{
				for (int y = bounds.Top; y <= bounds.Bottom; y++)
				{
					PointF p = new PointF(x, y);
					if (Contains(p))
						yield return lut(PixelData.GetPixel(x, y));
				}
			}
		}

		/// <summary>
		/// Checks if operations in the given <paramref name="units"/> are possible with the <paramref name="pixelSpacing"/> information available.
		/// </summary>
		/// <returns>True if such operations are valid; False if no such operation is possible.</returns>
		protected static bool ValidateUnits(Units units, PixelSpacing pixelSpacing)
		{
			return (units == Units.Pixels || !pixelSpacing.IsNull);
		}

		/// <summary>
		/// Converts an area in pixels into the given units given some particular pixel spacing.
		/// </summary>
		/// <param name="area">The area of pixels to be converted.</param>
		/// <param name="units">The units into which the area should be converted.</param>
		/// <param name="pixelSpacing">The pixel spacing information available.</param>
		/// <returns>The equivalent area in the units of <paramref name="units"/>.</returns>
		/// <exception cref="ArgumentException">Thrown if <paramref name="units"/> is a physical unit of measurement and <paramref name="pixelSpacing"/> is not calibrated.</exception>
		protected static double ConvertFromSquarePixels(double area, Units units, PixelSpacing pixelSpacing)
		{
			if (!ValidateUnits(units, pixelSpacing))
			{
				const string msg = "Pixel spacing must be calibrated in order to compute physical units.";
				throw new ArgumentException(msg, "units");
			}

			double factor;

			switch (units)
			{
				case Units.Pixels:
					factor = 1;
					break;
				case Units.Centimeters:
					factor = pixelSpacing.Column*pixelSpacing.Row/100;
					break;
				case Units.Millimeters:
				default:
					factor = pixelSpacing.Column*pixelSpacing.Row;
					break;
			}

			return area*factor;
		}

		/// <summary>
		/// Checks whether or not the region of interest's bounding box intersects the image.
		/// </summary>
		/// <returns>True if the bounding box intersects the image; False otherwise.</returns>
		private bool IsBoundingBoxInImage()
		{
			RectangleF boundingBox = BoundingBox;

			if (FloatComparer.AreEqual(0, boundingBox.Width) || FloatComparer.AreEqual(0, boundingBox.Height))
				return false;

			if (boundingBox.Left < 0 ||
			    boundingBox.Top < 0 ||
			    boundingBox.Right > (ImageColumns - 1) ||
			    boundingBox.Bottom > (ImageRows - 1))
				return false;

			return true;
		}

		private delegate double LutFunction(int v);
	}
}