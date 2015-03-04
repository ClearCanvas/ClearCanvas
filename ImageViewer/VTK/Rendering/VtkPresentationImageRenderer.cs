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
using System.IO;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Rendering.GDI;
using GdiRenderer = ClearCanvas.ImageViewer.Rendering.GDI.GdiRenderer;

namespace ClearCanvas.ImageViewer.Vtk.Rendering
{
	/// <summary>
	/// An implementation of <see cref="IRenderer"/> for <see cref="IVtkPresentationImage"/>s.
	/// </summary>
	public class VtkPresentationImageRenderer : RendererBase2<IVtkRenderingSurface, IVtkPresentationImage>
	{
		private Pen _pen;
		private GdiObjectFactory _gdiObjectFactory;

		public VtkPresentationImageRenderer()
		{
			_pen = new Pen(Color.White);
            _gdiObjectFactory = new GdiObjectFactory();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_pen != null)
				{
					_pen.Dispose();
					_pen = null;
				}

				if (_gdiObjectFactory != null)
				{
					_gdiObjectFactory.Dispose();
					_gdiObjectFactory = null;
				}
			}

			base.Dispose(disposing);
		}

		public override IVtkRenderingSurface CreateRenderingSurface(IntPtr windowId, int width, int height, RenderingSurfaceType type)
		{
			var clientRectangle = new Rectangle(0, 0, width, height);
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
					return new Win32VtkRenderingSurface(windowId, type == RenderingSurfaceType.Offscreen) {ClientRectangle = clientRectangle};
				case PlatformID.Unix:
				case PlatformID.Xbox:
				case PlatformID.MacOSX:
					throw new NotSupportedException();
				case PlatformID.WinCE:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
					// legacy platforms that will likely never be supported
					throw new NotSupportedException();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void Render()
		{
			DrawSceneGraph(PresentationImage);
		}

		protected override void Refresh()
		{
			Surface.Refresh();
		}

		protected override void DrawSceneGraph(IVtkPresentationImage image)
		{
			Surface.SetSceneRoot(image.CreateSceneGraph());
			Surface.Render(DrawOverlay2D);
		}

		private void DrawOverlay2D()
		{
			DrawOverlay2D(PresentationImage);
		}

		protected void DrawOverlay2D(IVtkPresentationImage image)
		{
			Surface.OverlayBuffer.Graphics.Clear(Color.Transparent);

			DrawSceneGraph(SceneGraph);
			DrawTextOverlay(image);
		}

		#region Static Members

		/// <summary>
		/// Gets or sets a value controlling whether or not the VTK rendering frame rate should be displayed onscreen.
		/// </summary>
		public static bool ShowFps { get; set; }

		/// <summary>
		/// Gets diagnostic information regarding the system graphics support.
		/// </summary>
		public static string GetDiagnosticInfo()
		{
			var sb = new StringBuilder();
			using (var sr = new StringReader(Win32VtkRenderingSurface.ReportCapabilities()))
			{
				// we do this to normalize the line endings
				string line;
				while ((line = sr.ReadLine()) != null)
					sb.AppendLine(line);
			}
			return sb.ToString();
		}

		#endregion

		#region GDI+ Rendering Implementation

		protected virtual void DrawSceneGraph(CompositeGraphic sceneGraph)
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

		protected virtual void DrawImageGraphic(ImageGraphic imageGraphic)
		{
			GdiRenderer.DrawImageGraphic(Surface.OverlayBuffer, imageGraphic);
		}

		protected virtual void DrawLinePrimitive(LinePrimitive line)
		{
			GdiRenderer.DrawLinePrimitive(Surface.OverlayBuffer, _pen, line, Dpi);
		}

		protected virtual void DrawInvariantLinePrimitive(InvariantLinePrimitive line)
		{
			GdiRenderer.DrawLinePrimitive(Surface.OverlayBuffer, _pen, line, Dpi);
		}

		protected virtual void DrawSplinePrimitive(SplinePrimitive spline)
		{
			GdiRenderer.DrawSplinePrimitive(Surface.OverlayBuffer, _pen, spline, Dpi);
		}

		protected virtual void DrawRectanglePrimitive(RectanglePrimitive rect)
		{
			GdiRenderer.DrawRectanglePrimitive(Surface.OverlayBuffer, _pen, rect, Dpi);
		}

		protected virtual void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
		{
			GdiRenderer.DrawRectanglePrimitive(Surface.OverlayBuffer, _pen, rect, Dpi);
		}

		protected virtual void DrawEllipsePrimitive(EllipsePrimitive ellipse)
		{
			GdiRenderer.DrawEllipsePrimitive(Surface.OverlayBuffer, _pen, ellipse, Dpi);
		}

		protected virtual void DrawInvariantEllipsePrimitive(InvariantEllipsePrimitive ellipse)
		{
			GdiRenderer.DrawEllipsePrimitive(Surface.OverlayBuffer, _pen, ellipse, Dpi);
		}

		protected virtual void DrawArcPrimitive(IArcGraphic arc)
		{
			GdiRenderer.DrawArcPrimitive(Surface.OverlayBuffer, _pen, arc, Dpi);
		}

		protected virtual void DrawPointPrimitive(PointPrimitive pointPrimitive)
		{
			GdiRenderer.DrawPointPrimitive(Surface.OverlayBuffer, _gdiObjectFactory, pointPrimitive, Dpi);
		}

		protected virtual void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
            GdiRenderer.DrawTextPrimitive(Surface.OverlayBuffer, _gdiObjectFactory, textPrimitive, Dpi);
		}

		protected override void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox)
		{
            GdiRenderer.DrawAnnotationBox(Surface.OverlayBuffer, _gdiObjectFactory, annotationText, annotationBox, Dpi);
		}

		#endregion
	}
}