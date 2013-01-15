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

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal class ImageBuffer : IDisposable
    {
        private Bitmap _bitmap;
		private System.Drawing.Graphics _graphics;
		private Size _size;

        public ImageBuffer()
        {
        }

		public System.Drawing.Graphics Graphics
		{
			get
			{
				if (_graphics == null)
				{
					_graphics = System.Drawing.Graphics.FromImage(this.Bitmap);
					//_graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				}

				return _graphics;
			}
		}

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

		public int Width
		{
			get { return _size.Width; }	
		}

		public int Height
		{
			get { return _size.Height; }	
		}

		public Bitmap Bitmap
		{
			get
			{
				if (_bitmap == null && !_size.IsEmpty)
					_bitmap = new Bitmap(_size.Width, _size.Height);

				return _bitmap;
			}
		}

		public void Dispose()
        {
			DisposeBuffer();
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
