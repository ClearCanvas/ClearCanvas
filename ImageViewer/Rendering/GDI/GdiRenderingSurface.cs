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
	internal sealed class GdiRenderingSurface : RenderingSurfaceBase
	{
		private BitmapBuffer _imageBuffer;
		private BackBuffer _finalBuffer;

		public GdiRenderingSurface(IntPtr windowId, int width, int height)
			: base(RenderingSurfaceType.Offscreen)
		{
			_imageBuffer = new BitmapBuffer();
			_finalBuffer = new BackBuffer();

			WindowID = windowId;
			ClientRectangle = new Rectangle(0, 0, width, height);
		}

		protected override void OnContextIDChanged(IntPtr value)
		{
			FinalBuffer.ContextId = value;
		}

		protected override void OnClientRectangleChanged(Rectangle value)
		{
			_imageBuffer.Size = new Size(value.Width, value.Height);
			_finalBuffer.ClientRectangle = value;
		}

		public BitmapBuffer ImageBuffer
		{
			get { return _imageBuffer; }
		}

		public BackBuffer FinalBuffer
		{
			get { return _finalBuffer; }
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_imageBuffer != null)
				{
					_imageBuffer.Dispose();
					_imageBuffer = null;
				}

				if (_finalBuffer != null)
				{
					_finalBuffer.Dispose();
					_finalBuffer = null;
				}
			}
		}
	}
}