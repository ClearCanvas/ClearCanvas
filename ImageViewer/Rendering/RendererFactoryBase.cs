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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Provides base implementation for a Render Factory.
	/// </summary>
	/// <remarks>
	/// Inheritors must also derive from <see cref="RendererBase"/> 
	/// in order to use this class (it calls <see cref="GetNewRenderer"/> to allocate
	/// a new <see cref="RendererBase"/> from within the <see cref="GetRenderer"/> factory
	/// method.  Note that only one <see cref="RendererBase"/> object is actually allocated
	/// per thread, and is wrapped in an internal proxy object.  This is because the 
	/// <see cref="RendererBase"/> object is purposely <b>not</b> thread-safe to make it
	/// easier for inheritors to implement.  However, there is nothing stopping a developer
	/// from deriving directly from <see cref="IRenderer"/> and creating their own Singleton
	/// thread-safe renderer.
	/// </remarks>
	public abstract class RendererFactoryBase : IRendererFactory
	{
		#region RendererProxy Class

		private class RendererProxy : IRenderer
		{
			private ReferenceCountedObjectWrapper<RendererBase> _wrapper;

			public RendererProxy(RendererBase realRenderer)
			{
				_wrapper = new ReferenceCountedObjectWrapper<RendererBase>(realRenderer);
			}

			public ReferenceCountedObjectWrapper<RendererBase> Wrapper
			{
				get { return _wrapper; }	
			}

			#region IRenderer Members

			public void Draw(DrawArgs drawArgs)
			{
				Wrapper.Item.Draw(drawArgs);
			}

			public IRenderingSurface CreateRenderingSurface(IntPtr windowId, int width, int height, RenderingSurfaceType type)
			{
				return Wrapper.Item.CreateRenderingSurface(windowId, width, height, type);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					if (_wrapper == null)
						return;
					
					_wrapper.DecrementReferenceCount();
					if (_wrapper.IsReferenceCountAboveZero())
						return;

					_proxy = null;

					_wrapper.Item.Dispose();
					_wrapper = null;

					GC.SuppressFinalize(this);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
		}

		#endregion

		[ThreadStatic]private static RendererProxy _proxy;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected RendererFactoryBase()
		{
		}

		/// <summary>
		/// Allocates a new <see cref="RendererBase"/>.
		/// </summary>
		/// <remarks>
		/// Inheritors must override this method.
		/// </remarks>
		protected abstract RendererBase GetNewRenderer();

		#region IRendererFactory Members

		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// Factory method for <see cref="IRenderer"/>s.
		/// </summary>
		/// <remarks>
		/// See the remarks for <see cref="RendererFactoryBase"/> regarding how
		/// these objects are actually allocated/managed internally.
		/// </remarks>
		public IRenderer GetRenderer()
		{
			if (_proxy == null)
				_proxy = new RendererProxy(GetNewRenderer());

			_proxy.Wrapper.IncrementReferenceCount();
			return _proxy;
		}

		#endregion
	}
}
