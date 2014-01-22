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

namespace ClearCanvas.ImageViewer.Rendering.GDI
{
	/// <summary>
	/// Represents the GDI back buffer used for double buffered rendering.
	/// </summary>
	public sealed class BackBuffer : IGdiBuffer, IDisposable
	{
		private Rectangle _clientRectangle;
		private BufferedGraphicsContext _graphicsContext;
		private BufferedGraphics _bufferedGraphics;

		/// <summary>
		/// Initializes a new instance of <see cref="BackBuffer"/>.
		/// </summary>
		public BackBuffer() {}

		/// <summary>
		/// Initializes a new instance of <see cref="BackBuffer"/>.
		/// </summary>
		/// <param name="contextId">The GDI device context of the destination surface.</param>
		/// <param name="clientRectangle">The client rectangle of the destination surface.</param>
		public BackBuffer(IntPtr contextId, Rectangle clientRectangle)
		{
			ContextId = contextId;
			ClientRectangle = clientRectangle;
		}

		public void Dispose()
		{
			DisposeBuffer();

			if (_graphicsContext != null)
			{
				_graphicsContext.Dispose();
				_graphicsContext = null;
			}
		}

		/// <summary>
		/// Gets or sets the GDI device context of the destination surface.
		/// </summary>
		public IntPtr ContextId { get; set; }

		/// <summary>
		/// Gets or sets the client rectangle of the destination surface.
		/// </summary>
		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (_clientRectangle == value)
					return;

				_clientRectangle = value;
				DisposeBuffer();
			}
		}

		Rectangle IGdiBuffer.Bounds
		{
			get { return _clientRectangle; }
		}

		/// <summary>
		/// Gets the <see cref="System.Drawing.Graphics"/> object that outputs to the back buffer.
		/// </summary>
		public System.Drawing.Graphics Graphics
		{
			get { return BufferedGraphics != null ? BufferedGraphics.Graphics : null; }
		}

		private BufferedGraphics BufferedGraphics
		{
			get
			{
				if (_bufferedGraphics == null && !IsClientRectangleEmpty && ContextId != IntPtr.Zero)
				{
					Context.MaximumBuffer = GetMaximumBufferSize();
					_bufferedGraphics = Context.Allocate(ContextId, _clientRectangle);
				}
				return _bufferedGraphics;
			}
		}

		private BufferedGraphicsContext Context
		{
			get { return _graphicsContext ?? (_graphicsContext = new BufferedGraphicsContext()); }
		}

		private bool IsClientRectangleEmpty
		{
			get { return _clientRectangle.Width == 0 || _clientRectangle.Height == 0; }
		}

		/// <summary>
		/// Renders the given <see cref="BitmapBuffer"/> to the back buffer.
		/// </summary>
		/// <param name="imageBuffer"></param>
		public void RenderImage(BitmapBuffer imageBuffer)
		{
			RenderImage(imageBuffer.Bitmap);
		}

		/// <summary>
		/// Renders the given <see cref="Image"/> to the back buffer.
		/// </summary>
		/// <param name="image"></param>
		public void RenderImage(Image image)
		{
			RenderImage(image, new Point());
		}

		/// <summary>
		/// Renders the given <see cref="Image"/> to the back buffer.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="location"></param>
		public void RenderImage(Image image, Point location)
		{
			Graphics.DrawImageUnscaled(image, location);
		}

		/// <summary>
		/// Flips the back buffer to the destination surface.
		/// </summary>
		public void RenderToScreen()
		{
			if (_bufferedGraphics != null && !IsClientRectangleEmpty)
				_bufferedGraphics.Render(ContextId);
		}

		private Size GetMaximumBufferSize()
		{
			return new Size(_clientRectangle.Width + 1, _clientRectangle.Height + 1);
		}

		private void DisposeBuffer()
		{
			if (_graphicsContext != null)
			{
				_graphicsContext.Invalidate();
				_graphicsContext = null;
			}

			if (_bufferedGraphics != null)
			{
				_bufferedGraphics.Dispose();
				_bufferedGraphics = null;
			}
		}
	}
}