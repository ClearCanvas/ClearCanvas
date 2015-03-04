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
	/// A Template class providing the base functionality for an <see cref="IRenderer"/>.
	/// </summary>
	/// <remarks>
	/// See the remarks section for <see cref="RendererFactoryBase"/> regarding the 
	/// thread-safety of this object (it is not thread-safe).  For this reason, you should
	/// use this class in tandem with the <see cref="RendererFactoryBase"/>, although it
	/// is not required that you do so.
	/// </remarks>
	/// <seealso cref="RendererBase2{TRenderingSurface,TPresentationImage}"/>
	/// <seealso cref="RendererFactoryBase"/>
	public abstract class RendererBase : IRenderer
	{
		private readonly string _rendererTypeId;
		private DrawMode _drawMode;
		private CompositeGraphic _sceneGraph;
		private IRenderingSurface _surface;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected RendererBase()
		{
			_rendererTypeId = GetType().Name;
		}

		~RendererBase()
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
		/// Gets the <see cref="ClearCanvas.ImageViewer.Rendering.DrawMode"/>.
		/// </summary>
		protected DrawMode DrawMode
		{
			get { return _drawMode; }
			set { _drawMode = value; }
		}

		/// <summary>
		/// Gets the <b>SceneGraph</b> that is to be rendered.
		/// </summary>
		protected CompositeGraphic SceneGraph
		{
			get { return _sceneGraph; }
			set { _sceneGraph = value; }
		}

		/// <summary>
		/// Gets the <see cref="IRenderingSurface"/> that is to be rendered upon.
		/// </summary>
		protected IRenderingSurface Surface
		{
			get { return _surface; }
			set { _surface = value; }
		}

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
			catch (Exception e)
			{
				throw new RenderingException(e, drawArgs);
			}
			finally
			{
				_sceneGraph = null;
				_surface = null;
			}
		}

		/// <summary>
		/// Factory method for an <see cref="IRenderingSurface"/>.
		/// </summary>
		public abstract IRenderingSurface GetRenderingSurface(IntPtr windowId, int width, int height);

		public virtual IRenderingSurface CreateRenderingSurface(IntPtr windowId, int width, int height, RenderingSurfaceType type)
		{
			return GetRenderingSurface(windowId, width, height);
		}

		/// <summary>
		/// Initializes the member variables before calling <see cref="Render"/> or <see cref="Refresh"/>.
		/// </summary>
		protected virtual void Initialize(DrawArgs drawArgs)
		{
			_drawMode = drawArgs.DrawMode;
			_sceneGraph = drawArgs.SceneGraph;
			_surface = drawArgs.RenderingSurface;
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
			DrawSceneGraph(SceneGraph);
			DrawTextOverlay(SceneGraph.ParentPresentationImage);
		}

		/// <summary>
		/// Called when <see cref="DrawArgs.DrawMode"/> is equal to <b>DrawMode.Refresh</b>.
		/// </summary>
		/// <remarks>
		/// Inheritors must implement this method.
		/// </remarks>
		protected abstract void Refresh();

		/// <summary>
		/// Traverses and Renders the Scene Graph.
		/// </summary>
		protected void DrawSceneGraph(CompositeGraphic sceneGraph)
		{
			foreach (Graphic graphic in sceneGraph.Graphics)
			{
				if (graphic.Visible)
				{
					graphic.OnDrawing();

					if (graphic is CompositeGraphic)
						DrawSceneGraph((CompositeGraphic) graphic);
					else if (graphic is ImageGraphic)
						DrawImageGraphic((ImageGraphic) graphic);
					else if (graphic is LinePrimitive)
						DrawLinePrimitive((LinePrimitive) graphic);
					else if (graphic is InvariantLinePrimitive)
						DrawInvariantLinePrimitive((InvariantLinePrimitive) graphic);
					else if (graphic is SplinePrimitive)
						DrawSplinePrimitive((SplinePrimitive) graphic);
					else if (graphic is RectanglePrimitive)
						DrawRectanglePrimitive((RectanglePrimitive) graphic);
					else if (graphic is InvariantRectanglePrimitive)
						DrawInvariantRectanglePrimitive((InvariantRectanglePrimitive) graphic);
					else if (graphic is EllipsePrimitive)
						DrawEllipsePrimitive((EllipsePrimitive) graphic);
					else if (graphic is InvariantEllipsePrimitive)
						DrawInvariantEllipsePrimitive((InvariantEllipsePrimitive) graphic);
					else if (graphic is ArcPrimitive)
						DrawArcPrimitive((IArcGraphic) graphic);
					else if (graphic is InvariantArcPrimitive)
						DrawArcPrimitive((IArcGraphic) graphic);
					else if (graphic is PointPrimitive)
						DrawPointPrimitive((PointPrimitive) graphic);
					else if (graphic is InvariantTextPrimitive)
						DrawTextPrimitive((InvariantTextPrimitive) graphic);
				}
			}
		}

		/// <summary>
		/// Draws the Text Overlay.
		/// </summary>
		protected void DrawTextOverlay(IPresentationImage presentationImage)
		{
#if DEBUG
            CodeClock clock = new CodeClock();
			clock.Start();
#endif
		    var layoutProvider = presentationImage as IAnnotationLayoutProvider;
			if (layoutProvider == null)
				return;

            var layout = layoutProvider.AnnotationLayout;
			if (layout == null || !layout.Visible)
				return;

			foreach (AnnotationBox annotationBox in layout.AnnotationBoxes)
			{
			    if (!annotationBox.Visible) continue;

			    string annotationText = annotationBox.GetAnnotationText(presentationImage);
			    if (!String.IsNullOrEmpty(annotationText))
			        DrawAnnotationBox(annotationText, annotationBox);
			}
#if DEBUG
			clock.Stop();
			PerformanceReportBroker.PublishReport(_rendererTypeId, "DrawTextOverlay", clock.Seconds);
#endif
		}

		/// <summary>
		/// Draws an <see cref="ImageGraphic"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawImageGraphic(ImageGraphic imageGraphic);

		/// <summary>
		/// Draws a <see cref="LinePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawLinePrimitive(LinePrimitive line);

		/// <summary>
		/// Draws a <see cref="InvariantLinePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawInvariantLinePrimitive(InvariantLinePrimitive line);

		/// <summary>
		/// Draws a <see cref="SplinePrimitive"/>. Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawSplinePrimitive(SplinePrimitive spline);

		/// <summary>
		/// Draws a <see cref="RectanglePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawRectanglePrimitive(RectanglePrimitive rectangle);

		/// <summary>
		/// Draws a <see cref="InvariantRectanglePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rectangle);

		/// <summary>
		/// Draws a <see cref="EllipsePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawEllipsePrimitive(EllipsePrimitive ellipse);

		/// <summary>
		/// Draws a <see cref="InvariantEllipsePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawInvariantEllipsePrimitive(InvariantEllipsePrimitive ellipse);

		/// <summary>
		/// Draws a <see cref="ArcPrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawArcPrimitive(IArcGraphic arc);

		/// <summary>
		/// Draws a <see cref="PointPrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawPointPrimitive(PointPrimitive pointPrimitive);

		/// <summary>
		/// Draws an <see cref="InvariantTextPrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawTextPrimitive(InvariantTextPrimitive textPrimitive);

		/// <summary>
		/// Draws an <see cref="AnnotationBox"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox);

		#region Disposal

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected virtual void Dispose(bool disposing) {}

		#region IDisposable Members

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
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		#endregion
	}
}