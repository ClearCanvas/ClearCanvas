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
using System.Drawing.Drawing2D;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Rendering.GDI;

namespace ClearCanvas.ImageViewer.Vtk.Rendering
{
	/// <summary>
	/// An implementation of <see cref="IRenderer"/> for <see cref="IVtkPresentationImage"/>s.
	/// </summary>
	public class VtkPresentationImageRenderer : RendererBase2<IVtkRenderingSurface, IVtkPresentationImage>
	{
		private const ushort _minimumFontSizeInPixels = 4;
		private const float _nominalScreenDpi = 96;
		private const string _defaultFont = "Arial";

		private Pen _pen;
		private SolidBrush _brush;
		private FontFactory _fontFactory;

		public VtkPresentationImageRenderer()
		{
			_pen = new Pen(Color.White);
			_brush = new SolidBrush(Color.Black);
			_fontFactory = new FontFactory();
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

				if (_brush != null)
				{
					_brush.Dispose();
					_brush = null;
				}

				if (_fontFactory != null)
				{
					_fontFactory.Dispose();
					_fontFactory = null;
				}
			}

			base.Dispose(disposing);
		}

		public override IVtkRenderingSurface CreateRenderingSurface(IntPtr windowId, int width, int height)
		{
			var clientRectangle = new Rectangle(0, 0, width, height);
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
					return new Win32VtkRenderingSurface(windowId) {ClientRectangle = clientRectangle};
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
			// normally we draw 2D overlay over the image, but since they're rendered to separate buffers, we actually need to render the overlay first
			DrawOverlay2D(PresentationImage);

			DrawSceneGraph(PresentationImage);
		}

		protected override void Refresh()
		{
			Surface.Refresh();
		}

		protected override void DrawSceneGraph(IVtkPresentationImage image)
		{
			Surface.SetSceneRoot(image.CreateSceneGraph());
			Surface.Render();
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

		#endregion

		#region GDI+ Rendering Implementation

		// TODO: pull this stuff out into a GDI rendering core

		protected void DrawSceneGraph(CompositeGraphic sceneGraph)
		{
			foreach (Graphic graphic in sceneGraph.Graphics)
			{
				if (graphic.Visible)
				{
					graphic.OnDrawing();

					if (graphic is CompositeGraphic)
						DrawSceneGraph((CompositeGraphic) graphic);
						//else if (graphic is ImageGraphic)
						//    DrawImageGraphic((ImageGraphic)graphic);
					else if (graphic is LinePrimitive)
						DrawLinePrimitive((LinePrimitive) graphic);
					else if (graphic is InvariantLinePrimitive)
						DrawInvariantLinePrimitive((InvariantLinePrimitive) graphic);
					else if (graphic is CurvePrimitive)
						DrawCurvePrimitive((CurvePrimitive) graphic);
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
		/// Draws a <see cref="LinePrimitive"/>.
		/// </summary>
		protected void DrawLinePrimitive(LinePrimitive line)
		{
			InternalDrawLinePrimitive(line);
		}

		/// <summary>
		/// Draws a <see cref="InvariantLinePrimitive"/>.
		/// </summary>
		protected void DrawInvariantLinePrimitive(InvariantLinePrimitive line)
		{
			InternalDrawLinePrimitive(line);
		}

		/// <summary>
		/// Draws a <see cref="CurvePrimitive"/>.
		/// </summary>
		protected void DrawCurvePrimitive(CurvePrimitive curve)
		{
			Surface.OverlayBuffer.Graphics.Transform = curve.SpatialTransform.CumulativeTransform;
			curve.CoordinateSystem = CoordinateSystem.Source;

			Surface.OverlayBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(curve, 1, Dpi);

			SetDashStyle(curve);

			SizeF dropShadowOffset = GetDropShadowOffset(curve, Dpi);
			PointF[] pathPoints = GetCurvePoints(curve.Points, dropShadowOffset);

			if (curve.Points.IsClosed)
				Surface.OverlayBuffer.Graphics.DrawClosedCurve(_pen, pathPoints);
			else
				Surface.OverlayBuffer.Graphics.DrawCurve(_pen, pathPoints);

			// Draw line
			_pen.Color = curve.Color;
			pathPoints = GetCurvePoints(curve.Points, SizeF.Empty);

			if (curve.Points.IsClosed)
				Surface.OverlayBuffer.Graphics.DrawClosedCurve(_pen, pathPoints);
			else
				Surface.OverlayBuffer.Graphics.DrawCurve(_pen, pathPoints);

			Surface.OverlayBuffer.Graphics.SmoothingMode = SmoothingMode.None;

			curve.ResetCoordinateSystem();
			Surface.OverlayBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws a <see cref="RectanglePrimitive"/>.
		/// </summary>
		protected void DrawRectanglePrimitive(RectanglePrimitive rect)
		{
			InternalDrawRectanglePrimitive(rect);
		}

		/// <summary>
		/// Draws a <see cref="InvariantRectanglePrimitive"/>.
		/// </summary>
		protected void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
		{
			InternalDrawRectanglePrimitive(rect);
		}

		/// <summary>
		/// Draws a <see cref="EllipsePrimitive"/>.
		/// </summary>
		protected void DrawEllipsePrimitive(EllipsePrimitive ellipse)
		{
			InternalDrawEllipsePrimitive(ellipse);
		}

		/// <summary>
		/// Draws a <see cref="InvariantEllipsePrimitive"/>.
		/// </summary>
		protected void DrawInvariantEllipsePrimitive(InvariantEllipsePrimitive ellipse)
		{
			InternalDrawEllipsePrimitive(ellipse);
		}

		/// <summary>
		/// Draws a <see cref="ArcPrimitive"/>.
		/// </summary>
		protected void DrawArcPrimitive(IArcGraphic arc)
		{
			Surface.OverlayBuffer.Graphics.Transform = arc.SpatialTransform.CumulativeTransform;
			arc.CoordinateSystem = CoordinateSystem.Source;

			Surface.OverlayBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			RectangleF rectangle = new RectangleF(arc.TopLeft.X, arc.TopLeft.Y, arc.Width, arc.Height);
			rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
			SizeF dropShadowOffset = GetDropShadowOffset(arc, Dpi);

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(arc, 1, Dpi);

			SetDashStyle(arc);

			Surface.OverlayBuffer.Graphics.DrawArc(
				_pen,
				rectangle.Left + dropShadowOffset.Width,
				rectangle.Top + dropShadowOffset.Height,
				rectangle.Width,
				rectangle.Height,
				arc.StartAngle,
				arc.SweepAngle);

			// Draw rectangle
			_pen.Color = arc.Color;

			Surface.OverlayBuffer.Graphics.DrawArc(
				_pen,
				rectangle.Left,
				rectangle.Top,
				rectangle.Width,
				rectangle.Height,
				arc.StartAngle,
				arc.SweepAngle);

			arc.ResetCoordinateSystem();
			Surface.OverlayBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws a <see cref="PointPrimitive"/>.
		/// </summary>
		protected void DrawPointPrimitive(PointPrimitive pointPrimitive)
		{
			Surface.OverlayBuffer.Graphics.Transform = pointPrimitive.SpatialTransform.CumulativeTransform;
			pointPrimitive.CoordinateSystem = CoordinateSystem.Source;

			SizeF dropShadowOffset = GetDropShadowOffset(pointPrimitive, Dpi);
			var width = CalculateScaledPenWidth(pointPrimitive, 1, Dpi);

			// Draw drop shadow
			_brush.Color = Color.Black;

			Surface.OverlayBuffer.Graphics.FillRectangle(
				_brush,
				pointPrimitive.Point.X + dropShadowOffset.Width,
				pointPrimitive.Point.Y + dropShadowOffset.Height,
				width,
				width);

			// Draw point
			_brush.Color = pointPrimitive.Color;

			Surface.OverlayBuffer.Graphics.FillRectangle(
				_brush,
				pointPrimitive.Point.X,
				pointPrimitive.Point.Y,
				width,
				width);

			pointPrimitive.ResetCoordinateSystem();
			Surface.OverlayBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws an <see cref="InvariantTextPrimitive"/>.
		/// </summary>
		protected void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
			textPrimitive.CoordinateSystem = CoordinateSystem.Destination;

			// We adjust the font size depending on the scale so that it's the same size
			// irrespective of the zoom
			var fontSize = CalculateScaledFontPoints(textPrimitive.SizeInPoints, Dpi);
			Font font = _fontFactory.GetFont(textPrimitive.Font, fontSize, FontStyle.Regular, GraphicsUnit.Point, _defaultFont);

			// Calculate how big the text will be so we can set the bounding box
			textPrimitive.Dimensions = Surface.OverlayBuffer.Graphics.MeasureString(textPrimitive.Text, font);

			// Draw drop shadow
			_brush.Color = Color.Black;

			SizeF dropShadowOffset = new SizeF(1, 1);
			PointF boundingBoxTopLeft = new PointF(textPrimitive.BoundingBox.Left, textPrimitive.BoundingBox.Top);

			Surface.OverlayBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				boundingBoxTopLeft + dropShadowOffset);

			// Draw text
			_brush.Color = textPrimitive.Color;

			Surface.OverlayBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				boundingBoxTopLeft);

			textPrimitive.ResetCoordinateSystem();
		}

		protected override void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox)
		{
			// if there's nothing to draw, there's nothing to do. go figure.
			if (string.IsNullOrEmpty(annotationText))
				return;

			Rectangle clientRectangle = RectangleUtilities.CalculateSubRectangle(Surface.ClientRectangle, annotationBox.NormalizedRectangle);
			//Deflate the client rectangle by 4 pixels to allow some space 
			//between neighbouring rectangles whose borders coincide.
			Rectangle.Inflate(clientRectangle, -4, -4);

			int fontSize = (clientRectangle.Height/annotationBox.NumberOfLines) - 1;

			//don't draw it if it's too small to read, anyway.
			if (fontSize < _minimumFontSizeInPixels)
				return;

			using (var format = new StringFormat())
			{
				if (annotationBox.Truncation == AnnotationBox.TruncationBehaviour.Truncate)
					format.Trimming = StringTrimming.Character;
				else
					format.Trimming = StringTrimming.EllipsisCharacter;

				if (annotationBox.FitWidth)
					format.Trimming = StringTrimming.None;

				if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Right)
					format.Alignment = StringAlignment.Far;
				else if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Center)
					format.Alignment = StringAlignment.Center;
				else
					format.Alignment = StringAlignment.Near;

				if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Top)
					format.LineAlignment = StringAlignment.Near;
				else if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Center)
					format.LineAlignment = StringAlignment.Center;
				else
					format.LineAlignment = StringAlignment.Far;

				//allow p's and q's, etc to extend slightly beyond the bounding rectangle.  Only completely visible lines are shown.
				format.FormatFlags = StringFormatFlags.NoClip;

				if (annotationBox.NumberOfLines == 1)
					format.FormatFlags |= StringFormatFlags.NoWrap;

				FontStyle style = FontStyle.Regular;
				if (annotationBox.Bold)
					style |= FontStyle.Bold;
				if (annotationBox.Italics)
					style |= FontStyle.Italic;

				//don't draw it if it's too small to read, anyway.
				if (fontSize < _minimumFontSizeInPixels)
					return;

				Font font = _fontFactory.GetFont(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel, AnnotationBox.DefaultFont);
				SizeF layoutArea = new SizeF(clientRectangle.Width, clientRectangle.Height);
				SizeF size = Surface.OverlayBuffer.Graphics.MeasureString(annotationText, font, layoutArea, format);
				if (annotationBox.FitWidth && size.Width > clientRectangle.Width)
				{
					fontSize = (int) (Math.Round(fontSize*clientRectangle.Width/(double) size.Width - 0.5));

					//don't draw it if it's too small to read, anyway.
					if (fontSize < _minimumFontSizeInPixels)
						return;

					font = _fontFactory.GetFont(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel, AnnotationBox.DefaultFont);
				}

				// Draw drop shadow
				_brush.Color = Color.Black;
				clientRectangle.Offset(1, 1);

				Surface.OverlayBuffer.Graphics.DrawString(
					annotationText,
					font,
					_brush,
					clientRectangle,
					format);

				_brush.Color = Color.FromName(annotationBox.Color);
				clientRectangle.Offset(-1, -1);

				Surface.OverlayBuffer.Graphics.DrawString(
					annotationText,
					font,
					_brush,
					clientRectangle,
					format);
			}
		}

		private void InternalDrawLinePrimitive(ILineSegmentGraphic line)
		{
			Surface.OverlayBuffer.Graphics.Transform = line.SpatialTransform.CumulativeTransform;
			line.CoordinateSystem = CoordinateSystem.Source;

			Surface.OverlayBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(line, 1, Dpi);

			SetDashStyle(line);

			SizeF dropShadowOffset = GetDropShadowOffset(line, Dpi);
			Surface.OverlayBuffer.Graphics.DrawLine(
				_pen,
				line.Point1 + dropShadowOffset,
				line.Point2 + dropShadowOffset);

			// Draw line
			_pen.Color = line.Color;

			Surface.OverlayBuffer.Graphics.DrawLine(
				_pen,
				line.Point1,
				line.Point2);

			Surface.OverlayBuffer.Graphics.SmoothingMode = SmoothingMode.None;

			line.ResetCoordinateSystem();
			Surface.OverlayBuffer.Graphics.ResetTransform();
		}

		private void InternalDrawRectanglePrimitive(IBoundableGraphic rect)
		{
			Surface.OverlayBuffer.Graphics.Transform = rect.SpatialTransform.CumulativeTransform;
			rect.CoordinateSystem = CoordinateSystem.Source;

			Surface.OverlayBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			RectangleF rectangle = new RectangleF(rect.TopLeft.X, rect.TopLeft.Y, rect.Width, rect.Height);
			rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
			SizeF dropShadowOffset = GetDropShadowOffset(rect, Dpi);

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(rect, 1, Dpi);

			SetDashStyle(rect);

			Surface.OverlayBuffer.Graphics.DrawRectangle(
				_pen,
				rectangle.Left + dropShadowOffset.Width,
				rectangle.Top + dropShadowOffset.Height,
				rectangle.Width,
				rectangle.Height);

			// Draw rectangle
			_pen.Color = rect.Color;

			Surface.OverlayBuffer.Graphics.DrawRectangle(
				_pen,
				rectangle.Left,
				rectangle.Top,
				rectangle.Width,
				rectangle.Height);

			rect.ResetCoordinateSystem();
			Surface.OverlayBuffer.Graphics.ResetTransform();
		}

		private void InternalDrawEllipsePrimitive(IBoundableGraphic ellipse)
		{
			Surface.OverlayBuffer.Graphics.Transform = ellipse.SpatialTransform.CumulativeTransform;
			ellipse.CoordinateSystem = CoordinateSystem.Source;

			Surface.OverlayBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			RectangleF rectangle = new RectangleF(ellipse.TopLeft.X, ellipse.TopLeft.Y, ellipse.Width, ellipse.Height);
			rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
			SizeF dropShadowOffset = GetDropShadowOffset(ellipse, Dpi);

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(ellipse, 1, Dpi);

			SetDashStyle(ellipse);

			Surface.OverlayBuffer.Graphics.DrawEllipse(
				_pen,
				rectangle.Left + dropShadowOffset.Width,
				rectangle.Top + dropShadowOffset.Height,
				rectangle.Width,
				rectangle.Height);

			// Draw rectangle
			_pen.Color = ellipse.Color;

			Surface.OverlayBuffer.Graphics.DrawEllipse(
				_pen,
				rectangle.Left,
				rectangle.Top,
				rectangle.Width,
				rectangle.Height);

			ellipse.ResetCoordinateSystem();
			Surface.OverlayBuffer.Graphics.ResetTransform();
		}

		private void SetDashStyle(IVectorGraphic graphic)
		{
			if (graphic.LineStyle == LineStyle.Solid)
			{
				_pen.DashStyle = DashStyle.Solid;
			}
			else
			{
				_pen.DashStyle = DashStyle.Custom;

				if (graphic.LineStyle == LineStyle.Dash)
					_pen.DashPattern = new[] {4.0F, 4.0F};
				else
					_pen.DashPattern = new[] {2.0F, 4.0F};
			}
		}

		private static float CalculateScaledFontPoints(float fontPoints, float dpi)
		{
			return fontPoints*dpi/_nominalScreenDpi;
		}

		private static float CalculateScaledPenWidth(IGraphic graphic, int penWidth, float dpi)
		{
			return penWidth/graphic.SpatialTransform.CumulativeScale*dpi/_nominalScreenDpi;
		}

		private static SizeF GetDropShadowOffset(IGraphic graphic, float dpi)
		{
			float offset = CalculateScaledPenWidth(graphic, 1, dpi);
			return new SizeF(offset, offset);
		}

		private static PointF[] GetCurvePoints(IPointsList points, SizeF offset)
		{
			PointF[] result = new PointF[points.Count - (points.IsClosed ? 1 : 0)];
			for (int n = 0; n < result.Length; n++)
				result[n] = points[n] + offset;
			return result;
		}

		#endregion
	}

	public interface IVtkRenderingSurface : IRenderingSurface
	{
		BitmapBuffer OverlayBuffer { get; }

		void SetSceneRoot(VtkSceneGraph sceneGraphRoot);
		void Refresh();
		void Render();
	}
}