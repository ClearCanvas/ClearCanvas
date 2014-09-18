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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Base class for a strongly-typed <see cref="IRenderer"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See the remarks section for <see cref="RendererFactoryBase"/> regarding the 
	/// thread-safety of this object (it is not thread-safe).  For this reason, you should
	/// use this class in tandem with the <see cref="RendererFactoryBase"/>, although it
	/// is not required that you do so.
	/// </para>
	/// <para>
	/// This base class differs from <see cref="RendererBase"/> in that it is strongly-typed,
	/// provides more flexibility in implementation (particularly for 3D rendering technologies
	/// where objects need to be instantiated on a per-surface basis, rather than rasterizing
	/// the models from within this class.
	/// </para>
	/// </remarks>
	/// <seealso cref="RendererBase"/>
	/// <seealso cref="RendererFactoryBase"/>
	public abstract class RendererBase2<TRenderingSurface, TPresentationImage> : IRenderer
		where TRenderingSurface : class, IRenderingSurface
		where TPresentationImage : class, IPresentationImage
	{
		private readonly string _rendererTypeId;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected RendererBase2()
		{
			DrawMode = DrawMode.Render;
			Dpi = 96;

			_rendererTypeId = GetType().Name;
		}

		/// <summary>
		/// Finalizer.
		/// </summary>
		~RendererBase2()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}
		}

		/// <summary>
		/// Dispose method.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}
		}

		/// <summary>
		/// Dispose method. Inheritors should override this method to do any additional cleanup.
		/// </summary>
		/// <param name="disposing">True to indicate if being called from <see cref="Dispose"/>; False if being called from Finalizer.</param>
		protected virtual void Dispose(bool disposing) {}

		/// <summary>
		/// Gets the <see cref="ClearCanvas.ImageViewer.Rendering.DrawMode"/>.
		/// </summary>
		protected DrawMode DrawMode { get; private set; }

		/// <summary>
		/// Gets the <b>SceneGraph</b> that is to be rendered.
		/// </summary>
		protected CompositeGraphic SceneGraph { get; private set; }

		/// <summary>
		/// Gets the <see cref="IPresentationImage"/> that is to be rendered.
		/// </summary>
		protected TPresentationImage PresentationImage { get; private set; }

		/// <summary>
		/// Gets the <see cref="IRenderingSurface"/> that is to be rendered upon.
		/// </summary>
		protected TRenderingSurface Surface { get; private set; }

		/// <summary>
		/// Gets the resolution of the intended output device in DPI.
		/// </summary>
		protected float Dpi { get; private set; }

		/// <summary>
		/// Renders the specified scene graph to the graphics surface.
		/// </summary>
		/// <remarks>
		/// Calling code should take care to handle any exceptions in a manner suitable to the context of
		/// the rendering operation. For example, the view control for an
		/// <see cref="ITile"/> may wish to display the error message in the tile's client area <i>without
		/// crashing the control</i>, whereas an image export routine may wish to notify the user via an error
		/// dialog and have the export output <i>fail to be created</i>. Automated routines (such as unit
		/// tests) may even wish that the exception bubble all the way to the top for debugging purposes.
		/// </remarks>
		/// <param name="drawArgs">A <see cref="DrawArgs"/> object that specifies the graphics surface and the scene graph to be rendered.</param>
		/// <exception cref="RenderingException">Thrown if any <see cref="Exception"/> is encountered in the rendering pipeline.</exception>
		public virtual void Draw(DrawArgs drawArgs)
		{
			try
			{
				Initialize(drawArgs);

				if (drawArgs.RenderingSurface.ClientRectangle.Width == 0 || drawArgs.RenderingSurface.ClientRectangle.Height == 0)
					return;

				if (DrawMode == DrawMode.Render)
					Render();
				else
					Refresh();
			}
			catch (RenderingException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new RenderingException(ex, drawArgs);
			}
			finally
			{
				PresentationImage = null;
				Surface = null;
			}
		}

		/// <summary>
		/// Factory method to create an <see cref="IRenderingSurface"/> instance appropriate for this renderer.
		/// </summary>
		public abstract TRenderingSurface CreateRenderingSurface(IntPtr windowId, int width, int height, RenderingSurfaceType type);

		IRenderingSurface IRenderer.CreateRenderingSurface(IntPtr windowId, int width, int height, RenderingSurfaceType type)
		{
			return CreateRenderingSurface(windowId, width, height, type);
		}

		/// <summary>
		/// Initializes the member variables before calling <see cref="Render"/> or <see cref="Refresh"/>.
		/// </summary>
		protected virtual void Initialize(DrawArgs drawArgs)
		{
			DrawMode = drawArgs.DrawMode;
			SceneGraph = drawArgs.SceneGraph;
			PresentationImage = (TPresentationImage) drawArgs.SceneGraph.ParentPresentationImage;
			Surface = (TRenderingSurface) drawArgs.RenderingSurface;
			Dpi = drawArgs.Dpi;
		}

		/// <summary>
		/// Traverses and draws the scene graph.  
		/// </summary>
		/// <remarks>
		/// Inheritors should override this method to do any necessary work before calling the base method.
		/// </remarks>
		protected virtual void Render()
		{
			DrawSceneGraph(PresentationImage);
			DrawTextOverlay(PresentationImage);
		}

		/// <summary>
		/// Called when <see cref="DrawArgs.DrawMode"/> is equal to <b>DrawMode.Refresh</b>.
		/// </summary>
		protected virtual void Refresh()
		{
			Render();
		}

		/// <summary>
		/// Traverses and Renders the Scene Graph.
		/// </summary>
		protected abstract void DrawSceneGraph(TPresentationImage image);

		/// <summary>
		/// Draws the Text Overlay.
		/// </summary>
		protected virtual void DrawTextOverlay(TPresentationImage presentationImage)
		{
#if DEBUG
			var clock = new CodeClock();
			clock.Start();
#endif
			var annotationLayoutProvider = presentationImage as IAnnotationLayoutProvider;
			if (annotationLayoutProvider == null)
				return;

			var layout = annotationLayoutProvider.AnnotationLayout;
			if (layout == null || !layout.Visible)
				return;

			foreach (var annotationBox in layout.AnnotationBoxes)
			{
				if (annotationBox.Visible)
				{
					var annotationText = annotationBox.GetAnnotationText(presentationImage);
					if (!string.IsNullOrEmpty(annotationText))
						DrawAnnotationBox(annotationText, annotationBox);
				}
			}
#if DEBUG
			clock.Stop();
			PerformanceReportBroker.PublishReport(_rendererTypeId, "DrawTextOverlay", clock.Seconds);
#endif
		}

		/// <summary>
		/// Draws an <see cref="AnnotationBox"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox);
	}
}