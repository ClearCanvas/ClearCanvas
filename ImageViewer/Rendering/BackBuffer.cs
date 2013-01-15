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

//#define USE_BITMAP

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	#region BufferedGraphics Method

	internal sealed class BackBuffer : IDisposable
	{
		private IntPtr _contextID;
		private Rectangle _clientRectangle;

		private BufferedGraphicsContext _graphicsContext;
		private BufferedGraphics _bufferedGraphics;

		public BackBuffer()
		{
		}

		public IntPtr ContextID
		{
			get { return _contextID; }
			set { _contextID = value; }
		}

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

		public System.Drawing.Graphics Graphics
		{
			get
			{
				if (BufferedGraphics != null)
					return BufferedGraphics.Graphics;

				return null;
			}
		}

		private BufferedGraphics BufferedGraphics
		{
			get
			{
				if (_bufferedGraphics == null && !IsClientRectangleEmpty && _contextID != IntPtr.Zero)
				{
					Context.MaximumBuffer = GetMaximumBufferSize();
					_bufferedGraphics = Context.Allocate(_contextID, _clientRectangle);
				}

				return _bufferedGraphics;
			}
		}

		private BufferedGraphicsContext Context
		{
			get
			{
				if (_graphicsContext == null)
					_graphicsContext = new BufferedGraphicsContext();

				return _graphicsContext;
			}
		}

		private bool IsClientRectangleEmpty
		{
			get { return _clientRectangle.Width == 0 || _clientRectangle.Height == 0; }
		}

		public void RenderImage(ImageBuffer imageBuffer)
		{
			RenderImage(imageBuffer.Bitmap);
		}

		public void RenderImage(Image image)
		{
			Graphics.DrawImageUnscaled(image, 0, 0);
		}

		public void RenderToScreen()
		{
			if (_bufferedGraphics != null && !IsClientRectangleEmpty)
				_bufferedGraphics.Render(_contextID);
		}

		private Size GetMaximumBufferSize()
		{
			return new Size(_clientRectangle.Width + 1, _clientRectangle.Height + 1);
		}

		private void DisposeBuffer()
		{
			if (_graphicsContext != null)
				_graphicsContext.Invalidate();

			if (_bufferedGraphics != null)
			{
				_bufferedGraphics.Dispose();
				_bufferedGraphics = null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			DisposeBuffer();

			if (_graphicsContext != null)
			{
				_graphicsContext.Dispose();
				_graphicsContext = null;
			}
		}

		#endregion
	}

	#endregion
}

