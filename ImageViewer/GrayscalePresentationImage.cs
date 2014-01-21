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

using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A grayscale <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class GrayscalePresentationImage 
		: BasicPresentationImage, 
		IModalityLutProvider,
		IVoiLutProvider,
		ILutPipelineProvider,
		IColorMapProvider
	{
		#region Private fields

		private int _rows;
		private int _columns;
		private int _bitsAllocated;
		private int _bitsStored;
		private int _highBit;
		private bool _isSigned;
		private bool _inverted;
		private double _rescaleSlope;
		private double _rescaleIntercept;
		private double _pixelSpacingX;
		private double _pixelSpacingY;
		private double _pixelAspectRatioX;
		private double _pixelAspectRatioY;
		
		[CloneCopyReference]
		private PixelDataGetter _pixelDataGetter;
		private int _constructor;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// This simple constructor will automatically create grayscale pixel data with the specified
		/// number of rows and columns.
		/// </remarks>
		public GrayscalePresentationImage(int rows, int columns)
			: base(new GrayscaleImageGraphic(rows, columns))
		{
			_constructor = 0;
			_rows = rows;
			_columns = columns;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="inverted"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <param name="pixelAspectRatioX"></param>
		/// <param name="pixelAspectRatioY"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// This more flexible constructor allows for the pixel data
		/// to be retrieved from and external source via a <see cref="PixelDataGetter"/>.
		/// </remarks>
		public GrayscalePresentationImage(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY,
			PixelDataGetter pixelDataGetter)
			: base(new GrayscaleImageGraphic(
			       	rows,
			       	columns,
			       	bitsAllocated,
			       	bitsStored,
			       	highBit,
			       	isSigned,
			       	inverted,
			       	rescaleSlope,
			       	rescaleIntercept,
			       	pixelDataGetter),
			       pixelSpacingX,
			       pixelSpacingY,
			       pixelAspectRatioX,
			       pixelAspectRatioY)
		{
			_constructor = 1;
			_rows = rows;
			_columns = columns;
			_bitsAllocated = bitsAllocated;
			_bitsStored = bitsStored;
			_highBit = highBit;
			_isSigned = isSigned;
			_inverted = inverted;
			_rescaleSlope = rescaleSlope;
			_rescaleIntercept = rescaleIntercept;
			_pixelSpacingX = pixelSpacingX;
			_pixelSpacingY = pixelSpacingY;
			_pixelAspectRatioX = pixelAspectRatioX;
			_pixelAspectRatioY = pixelAspectRatioY;
			_pixelDataGetter = pixelDataGetter;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected GrayscalePresentationImage(GrayscalePresentationImage source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets the <see cref="GrayscaleImageGraphic"/> associated with this <see cref="GrayscalePresentationImage"/>.
		/// </summary>
		public new GrayscaleImageGraphic ImageGraphic
		{
			get { return (GrayscaleImageGraphic)base.ImageGraphic; }
		}

		#region IModalityLutProvider Members

		/// <summary>
		/// Gets this image's modality lut.
		/// </summary>
		public IModalityLut ModalityLut
		{
			get
			{
				return this.ImageGraphic.ModalityLut;
			}
		}

		#endregion

		#region IVoiLutProvider Members

		/// <summary>
		/// Gets this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get 
			{
				return this.ImageGraphic.VoiLutManager;
			}
		}

		#endregion

		#region ILutPipelineProvider Members

		IVoiLut ILutPipelineProvider.VoiLut
		{
			get { return ImageGraphic.VoiLut; }
		}

		public double LookupPixelValue(int rawPixelValue, LutPipelineStage outStage)
		{
			return ImageGraphic.LookupPixelValue(rawPixelValue, outStage);
		}

		#endregion

		#region IColorMapProvider Members

		/// <summary>
		/// Gets this image's <see cref="IColorMapManager"/>.
		/// </summary>
		public IColorMapManager ColorMapManager
		{
			get
			{
				return this.ImageGraphic.ColorMapManager;
			}
		}

		/// <summary>
		/// Creates a clone of the <see cref="GrayscalePresentationImage"/>.
		/// </summary>
		public override IPresentationImage CreateFreshCopy()
		{
			if (_constructor == 0)
				return new GrayscalePresentationImage(_rows, _columns);
			else
				return new GrayscalePresentationImage(
					_rows,
					_columns,
					_bitsAllocated,
					_bitsStored,
					_highBit,
					_isSigned,
					_inverted,
					_rescaleSlope,
					_rescaleIntercept,
					_pixelSpacingX,
					_pixelSpacingY,
					_pixelAspectRatioX,
					_pixelAspectRatioY,
					_pixelDataGetter);
		}

		#endregion
	}
}
