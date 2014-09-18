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
using System.Drawing.Imaging;

namespace ClearCanvas.ImageViewer.Rendering.GDI
{
	/// <summary>
	/// Represents a GDI memory bitmap buffer.
	/// </summary>
	public sealed class BitmapBuffer : IGdiBuffer, IDisposable
	{
		private readonly PixelFormat _pixelFormat;
		private Bitmap _bitmap;
		private System.Drawing.Graphics _graphics;
		private Size _size;

		/// <summary>
		/// Initializes a new instance of <see cref="BitmapBuffer"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="PixelFormat"/> will default to <see cref="System.Drawing.Imaging.PixelFormat.Format32bppArgb"/>.
		/// </remarks>
		public BitmapBuffer()
			: this(PixelFormat.Format32bppArgb, new Size()) {}

		/// <summary>
		/// Initializes a new instance of <see cref="BitmapBuffer"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="PixelFormat"/> will default to <see cref="System.Drawing.Imaging.PixelFormat.Format32bppArgb"/>.
		/// </remarks>
		/// <param name="size">The dimensions of the memory bitmap.</param>
		public BitmapBuffer(Size size)
			: this(PixelFormat.Format32bppArgb, size) {}

		/// <summary>
		/// Initializes a new instance of <see cref="BitmapBuffer"/>.
		/// </summary>
		/// <param name="pixelFormat">The <see cref="System.Drawing.Imaging.PixelFormat"/> of the bitmap.</param>
		public BitmapBuffer(PixelFormat pixelFormat)
			: this(pixelFormat, new Size()) {}

		/// <summary>
		/// Initializes a new instance of <see cref="BitmapBuffer"/>.
		/// </summary>
		/// <param name="pixelFormat">The <see cref="System.Drawing.Imaging.PixelFormat"/> of the bitmap.</param>
		/// <param name="size">The dimensions of the memory bitmap.</param>
		public BitmapBuffer(PixelFormat pixelFormat, Size size)
		{
			_pixelFormat = pixelFormat;
			_size = size;
		}

		public void Dispose()
		{
			DisposeBuffer();
		}

		/// <summary>
		/// Gets the <see cref="System.Drawing.Graphics"/> object that outputs to the memory bitmap.
		/// </summary>
		public System.Drawing.Graphics Graphics
		{
			get { return _graphics ?? (_graphics = System.Drawing.Graphics.FromImage(Bitmap)); }
		}

		/// <summary>
		/// Gets or sets the dimensions of the memory bitmap.
		/// </summary>
		public Size Size
		{
			get { return _size; }
			set
			{
				if (_size == value)
					return;

				_size = value;
				DisposeBuffer();
			}
		}

		public Rectangle Bounds
		{
			get { return new Rectangle(new Point(), _size); }
		}

		/// <summary>
		/// Gets the pixel format of the memory bitmap.
		/// </summary>
		public PixelFormat PixelFormat
		{
			get { return _pixelFormat; }
		}

		/// <summary>
		/// Gets the width of the memory bitmap.
		/// </summary>
		public int Width
		{
			get { return _size.Width; }
		}

		/// <summary>
		/// Gets the height of the memory bitmap.
		/// </summary>
		public int Height
		{
			get { return _size.Height; }
		}

		/// <summary>
		/// Gets the <see cref="System.Drawing.Bitmap"/> instance of the memory bitmap.
		/// </summary>
		public Bitmap Bitmap
		{
			get
			{
				if (_bitmap == null && !_size.IsEmpty)
					_bitmap = new Bitmap(_size.Width, _size.Height, _pixelFormat);
				return _bitmap;
			}
		}

		private void DisposeBuffer()
		{
			if (_graphics != null)
			{
				_graphics.Flush();
				_graphics.Dispose();

				// MUST set bitmaps and graphics to null after disposal, 
				// or app will occasionally crash on exit
				_graphics = null;
			}

			if (_bitmap != null)
			{
				_bitmap.Dispose();
				_bitmap = null;
			}
		}
	}
}