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
using System.Drawing.Imaging;

namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	public enum GraphicsSource
	{
		Default = 0,
		Hdc = 1
	}

	public class TestImageRenderer
	{
		private Size _size;
		private bool _customBackBuffer;
		private PixelFormat _pixelFormat;
		private GraphicsSource _graphicsSource;
		private bool _useBufferedGraphics;

		private Bitmap _customImage;

		private Bitmap _image;
		private Bitmap _buffer;
		private BufferedGraphicsContext _context;
		private BufferedGraphics _bufferedGraphics;

		private TimeSpan _renderTime;
		private TimeSpan _blitTime;

		public TestImageRenderer()
		{
			_customBackBuffer = true;

			Format = PixelFormat.Format24bppRgb;
			Source = GraphicsSource.Default;
		}

		public IList<PixelFormat> GetPixelFormats()
		{
			List<PixelFormat> formats = new List<PixelFormat>();
			formats.Add(PixelFormat.Format24bppRgb);
			formats.Add(PixelFormat.Format32bppPArgb);
			return formats;
		}

		public IList<GraphicsSource> GetGraphicsSources()
		{
			List<GraphicsSource> sources = new List<GraphicsSource>();
			Array values = Enum.GetValues(typeof(GraphicsSource));
			foreach (GraphicsSource value in values)
				sources.Add(value);
			return sources;
		}

		public Bitmap CustomImage
		{
			get { return _customImage; }
			set 
			{ 
				if (_customImage != null)
					_customImage.Dispose();

				_customImage = value;
				DisposeBuffer();
			}
		}

		public bool CustomBackBuffer
		{
			get { return _customBackBuffer; }	
			set
			{
				_customBackBuffer = value;
				DisposeBuffer();
			}
		}

		public bool UseBufferedGraphics
		{
			get { return _useBufferedGraphics; }
			set
			{
				_useBufferedGraphics = value;
				DisposeBuffer();
			}
		}

		public PixelFormat Format
		{
			get { return _pixelFormat; }
			set
			{
				_pixelFormat = value;
				DisposeBuffer();
			}
		}

		public GraphicsSource Source
		{
			get { return _graphicsSource; }
			set
			{
				_graphicsSource = value;
				DisposeBuffer();
			}
		}

		public TimeSpan RenderTime
		{
			get { return _renderTime; }	
		}

		public TimeSpan BlitTime
		{
			get { return _blitTime; }	
		}

		public void ResetStats()
		{
			_renderTime = TimeSpan.Zero;
			_blitTime = TimeSpan.Zero;
		}

		public void RenderTo(Graphics graphics, Size size)
		{
			Graphics target = graphics;
			IntPtr hdc = IntPtr.Zero;
			if (_graphicsSource == GraphicsSource.Hdc)
			{
				hdc = graphics.GetHdc();
				target = Graphics.FromHdc(hdc);
			}

			if (_customBackBuffer)
			{
				RenderImage(size);
				AllocateBuffer(target, size);
				if (_buffer != null)
				{
					using (Graphics buffer = Graphics.FromImage(_buffer))
					{
						Render(buffer, size);
					}

					DateTime start = DateTime.Now;
					target.DrawImage(_buffer, 0, 0);
					DateTime end = DateTime.Now;
					_blitTime = _blitTime.Add(end.Subtract(start));
					
				}
				else
				{
					Render(_bufferedGraphics.Graphics, size);

					DateTime start = DateTime.Now;
					_bufferedGraphics.Render(target);
					DateTime end = DateTime.Now;
					_blitTime = _blitTime.Add(end.Subtract(start));
				}
			}
			else
			{
				Render(target, size);
			}

			if (hdc != IntPtr.Zero)
			{
				graphics.ReleaseHdc(hdc);
				target.Dispose();
			}

			_size = size;
		}

		private void DisposeBuffer()
		{
			if (_buffer != null)
			{
				_buffer.Dispose();
				_buffer = null;
			}
			
			if (_bufferedGraphics != null)
			{
				_bufferedGraphics.Dispose();
				_bufferedGraphics = null;
			}
		}

		private void AllocateBuffer(Graphics target, Size size)
		{
			if (!_useBufferedGraphics)
			{
				if (_buffer != null && size != _size)
					DisposeBuffer();

				if (_buffer == null)
					_buffer = new Bitmap(size.Width, size.Height, _pixelFormat);
			}
			else
			{
				if (_bufferedGraphics != null && size != _size)
					DisposeBuffer();

				if (_context == null)
					_context = BufferedGraphicsManager.Current;

				if (_bufferedGraphics == null)
				{
					_context.MaximumBuffer = new Size(size.Width + 1, size.Height + 1);
					_bufferedGraphics = _context.Allocate(target, new Rectangle(Point.Empty, size));
				}
			}
		}

		private void Render(Graphics graphics, Size size)
		{
			DateTime start = DateTime.Now;

			graphics.Clear(Color.Black);
			if (_customImage != null)
				graphics.DrawImage(_customImage, 0, 0, size.Width, size.Height);
			else
				graphics.DrawImage(_image, 0, 0, size.Width, size.Height);

			DateTime end = DateTime.Now;
			_renderTime = _renderTime.Add(end.Subtract(start));
		}

		private void RenderImage(Size size)
		{
			if (_image != null && _image.Size != size)
			{
				_image.Dispose();
				_image = null;
			}

			if (_image == null)
				_image = new Bitmap(size.Width, size.Height, _pixelFormat);
			else
				return;

			int rectw = size.Width/3 - 11;
			int recth = size.Height/3 - 11;

			Graphics graphics = Graphics.FromImage(_image);
			Pen pen = new Pen(Brushes.Red, 10);
			graphics.DrawRectangle(pen, 0, 0, rectw, recth);
			pen.Dispose();

			pen = new Pen(Brushes.Green, 10);
			graphics.DrawRectangle(pen, rectw, recth, rectw, recth);
			pen.Dispose();

			pen = new Pen(Brushes.Blue, 10);
			graphics.DrawRectangle(pen, 2 * rectw, 2 * recth, rectw, recth);
			pen.Dispose();
			graphics.Dispose();
		}
	}
}
