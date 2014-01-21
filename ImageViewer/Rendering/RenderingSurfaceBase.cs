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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A base implementation of <see cref="IRenderingSurface"/>.
	/// </summary>
	public abstract class RenderingSurfaceBase : IRenderingSurface
	{
		private readonly RenderingSurfaceType _type;
		private event EventHandler _invalidated;
		private IntPtr _windowId;
		private IntPtr _contextId;
		private Rectangle _clientRectangle;
		private Rectangle _clipRectangle;

		/// <summary>
		/// Initializes a <see cref="RenderingSurfaceBase"/>.
		/// </summary>
		/// <param name="type">The type of rendering surface.</param>
		protected RenderingSurfaceBase(RenderingSurfaceType type)
		{
			_type = type;
		}

		~RenderingSurfaceBase()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown during Finalize");
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown during Dispose");
			}
		}

		/// <summary>
		/// Called to release any resources held by this object.
		/// </summary>
		/// <param name="disposing">True if being called from <see cref="IDisposable.Dispose"/>; False if being called from the finalizer.</param>
		protected virtual void Dispose(bool disposing) {}

		public RenderingSurfaceType Type
		{
			get { return _type; }
		}

		public IntPtr WindowID
		{
			get { return _windowId; }
			set
			{
				if (_windowId == value) return;
				_windowId = value;
				OnWindowIDChanged(value);
			}
		}

		public IntPtr ContextID
		{
			get { return _contextId; }
			set
			{
				if (_contextId == value) return;
				_contextId = value;
				OnContextIDChanged(value);
			}
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (_clientRectangle == value) return;
				_clientRectangle = value;
				OnClientRectangleChanged(value);
			}
		}

		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
			set
			{
				if (_clipRectangle == value) return;
				_clipRectangle = value;
				OnClipRectangleChanged(value);
			}
		}

		public event EventHandler Invalidated
		{
			add { _invalidated += value; }
			remove { _invalidated -= value; }
		}

		// ReSharper disable InconsistentNaming

		/// <summary>
		/// Called when the value of <see cref="WindowID"/> has changed.
		/// </summary>
		/// <param name="value">The new value of <see cref="WindowID"/>.</param>
		protected virtual void OnWindowIDChanged(IntPtr value) {}

		/// <summary>
		/// Called when the value of <see cref="ContextID"/> has changed.
		/// </summary>
		/// <param name="value">The new value of <see cref="ContextID"/>.</param>
		protected virtual void OnContextIDChanged(IntPtr value) {}

		// ReSharper restore InconsistentNaming

		/// <summary>
		/// Called when the value of <see cref="ClientRectangle"/> has changed.
		/// </summary>
		/// <param name="value">The new value of <see cref="ClientRectangle"/>.</param>
		protected virtual void OnClientRectangleChanged(Rectangle value) {}

		/// <summary>
		/// Called when the value of <see cref="ClipRectangle"/> has changed.
		/// </summary>
		/// <param name="value">The new value of <see cref="ClipRectangle"/>.</param>
		protected virtual void OnClipRectangleChanged(Rectangle value) {}

		/// <summary>
		/// Fires the <see cref="Invalidated"/> event, notifying listeners that the contents of the surface have updated independently of a draw request.
		/// </summary>
		protected void NotifyInvalidated()
		{
			if (_invalidated != null)
				_invalidated.Invoke(this, new EventArgs());
		}
	}
}