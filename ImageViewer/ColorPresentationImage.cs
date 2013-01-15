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
	/// A colour <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class ColorPresentationImage
		: BasicPresentationImage,
		IVoiLutProvider
	{
		#region Private fields

		// We only bother having these private fields so
		// we can easily implement CreateFreshCopy()
		private int _rows;
		private int _columns;
		private double _pixelSpacingX;
		private double _pixelSpacingY;
		private double _pixelAspectRatioX;
		private double _pixelAspectRatioY;

		[CloneCopyReference]
		private PixelDataGetter _pixelDataGetter;

		private int _constructor;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ColorPresentationImage"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// This simple constructor will automatically create RGB pixel data with the specified
		/// number of rows and columns.
		/// </remarks>
		public ColorPresentationImage(int rows, int columns) 
			: base(new ColorImageGraphic(rows, columns))
		{
			_constructor = 0;
			_rows = rows;
			_columns = columns;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorPresentationImage"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <param name="pixelAspectRatioX"></param>
		/// <param name="pixelAspectRatioY"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// This more flexible constructor allows for the pixel data
		/// to be retrieved from and external source via a <see cref="PixelDataGetter"/>.
		/// </remarks>
		public ColorPresentationImage(
			int rows,
			int columns,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY,
			PixelDataGetter pixelDataGetter)
			: base(new ColorImageGraphic(rows, columns, pixelDataGetter),
			       pixelSpacingX,
			       pixelSpacingY,
			       pixelAspectRatioX,
			       pixelAspectRatioY)
		{
			_constructor = 1;
			_rows = rows;
			_columns = columns;
			_pixelSpacingX = pixelSpacingX;
			_pixelSpacingY = pixelSpacingY;
			_pixelAspectRatioX = pixelAspectRatioX;
			_pixelAspectRatioY = pixelAspectRatioY;
			_pixelDataGetter = pixelDataGetter;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ColorPresentationImage(ColorPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Creates a fresh copy of the <see cref="ColorPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="ColorPresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			if (_constructor == 0)
				return new ColorPresentationImage(_rows, _columns);
			else
				return new ColorPresentationImage(
					_rows,
					_columns,
					_pixelSpacingX,
					_pixelSpacingY,
					_pixelAspectRatioX,
					_pixelAspectRatioY,
					_pixelDataGetter);
		}

		/// <summary>
		/// Gets this image's <see cref="ColorImageGraphic"/>.
		/// </summary>
		public new ColorImageGraphic ImageGraphic
		{
			get { return (ColorImageGraphic)base.ImageGraphic;  }	
		}

		/// <summary>
		/// Gets or sets a value indicating whether VOI LUTs should be used in rendering this image.
		/// </summary>
		public bool VoiLutsEnabled
		{
			get { return this.VoiLutManager.Enabled; }
			set { this.VoiLutManager.Enabled = value; }
		}

		#region IVoiLutProvider Members

		/// <summary>
		/// Gets this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get { return this.ImageGraphic.VoiLutManager; }
		}

		#endregion
	}
}
