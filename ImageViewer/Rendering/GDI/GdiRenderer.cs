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
using System.Drawing.Imaging;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering.GDI
{
	/// <summary>
	/// A 2D Renderer that uses GDI+.
	/// </summary>
	public class GdiRenderer : RendererBase
	{
		/// <summary>
		/// The minimum font size for rendered text. If the specified font size is less than this value, the text will not be rendered at all.
		/// </summary>
		protected static readonly ushort MinimumFontSizeInPixels = 4;

		private const float _nominalScreenDpi = 96;

		private Pen _pen;
		private SolidBrush _brush;

		private FontFactory _fontFactory;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public GdiRenderer()
		{
			_pen = new Pen(Color.White);
			_brush = new SolidBrush(Color.Black);
			_fontFactory = new FontFactory();
		}

		private new GdiRenderingSurface Surface
		{
			get { return (GdiRenderingSurface) base.Surface; }
		}

		#region Disposal

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

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
		}

		#endregion

		/// <summary>
		/// Traverses and renders the scene graph.  
		/// </summary>
		protected override void Render()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			Surface.FinalBuffer.Graphics.Clear(Color.Black);
			base.Render();

			clock.Stop();
			PerformanceReportBroker.PublishReport("GDIRenderer", "Render", clock.Seconds);
		}

		/// <summary>
		/// Called when <see cref="DrawArgs.DrawMode"/> is equal to <b>DrawMode.Refresh</b>.
		/// </summary>
		protected override void Refresh()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			if (Surface.FinalBuffer != null)
				Surface.FinalBuffer.RenderToScreen();

			clock.Stop();
			PerformanceReportBroker.PublishReport("GDIRenderer", "Refresh", clock.Seconds);
		}

		/// <summary>
		/// Factory method for an <see cref="IRenderingSurface"/>.
		/// </summary>
		public override sealed IRenderingSurface GetRenderingSurface(IntPtr windowId, int width, int height)
		{
			return new GdiRenderingSurface(windowId, width, height);
		}

		/// <summary>
		/// Draws an <see cref="ImageGraphic"/>.
		/// </summary>
		protected override void DrawImageGraphic(ImageGraphic imageGraphic)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			Surface.ImageBuffer.Graphics.Clear(Color.FromArgb(0x0, 0xFF, 0xFF, 0xFF));

			DrawImageGraphic(Surface.ImageBuffer, imageGraphic);

			Surface.FinalBuffer.RenderImage(Surface.ImageBuffer);

			clock.Stop();
			PerformanceReportBroker.PublishReport("GDIRenderer", "DrawImageGraphic", clock.Seconds);
		}

		/// <summary>
		/// Draws a <see cref="LinePrimitive"/>.
		/// </summary>
		protected override void DrawLinePrimitive(LinePrimitive line)
		{
			DrawLinePrimitive(Surface.FinalBuffer, _pen, line, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="InvariantLinePrimitive"/>.
		/// </summary>
		protected override void DrawInvariantLinePrimitive(InvariantLinePrimitive line)
		{
			DrawLinePrimitive(Surface.FinalBuffer, _pen, line, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="SplinePrimitive"/>.
		/// </summary>
		protected override void DrawSplinePrimitive(SplinePrimitive spline)
		{
			DrawSplinePrimitive(Surface.FinalBuffer, _pen, spline, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="RectanglePrimitive"/>.
		/// </summary>
		protected override void DrawRectanglePrimitive(RectanglePrimitive rect)
		{
			DrawRectanglePrimitive(Surface.FinalBuffer, _pen, rect, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="InvariantRectanglePrimitive"/>.
		/// </summary>
		protected override void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
		{
			DrawRectanglePrimitive(Surface.FinalBuffer, _pen, rect, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="EllipsePrimitive"/>.
		/// </summary>
		protected override void DrawEllipsePrimitive(EllipsePrimitive ellipse)
		{
			DrawEllipsePrimitive(Surface.FinalBuffer, _pen, ellipse, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="InvariantEllipsePrimitive"/>.
		/// </summary>
		protected override void DrawInvariantEllipsePrimitive(InvariantEllipsePrimitive ellipse)
		{
			DrawEllipsePrimitive(Surface.FinalBuffer, _pen, ellipse, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="ArcPrimitive"/>.
		/// </summary>
		protected override void DrawArcPrimitive(IArcGraphic arc)
		{
			DrawArcPrimitive(Surface.FinalBuffer, _pen, arc, Dpi);
		}

		/// <summary>
		/// Draws a <see cref="PointPrimitive"/>.
		/// </summary>
		protected override void DrawPointPrimitive(PointPrimitive pointPrimitive)
		{
			DrawPointPrimitive(Surface.FinalBuffer, _brush, pointPrimitive, Dpi);
		}

		/// <summary>
		/// Draws an <see cref="InvariantTextPrimitive"/>.
		/// </summary>
		protected override void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
			DrawTextPrimitive(Surface.FinalBuffer, _brush, _fontFactory, textPrimitive, Dpi);
		}

		/// <summary>
		/// Draws an <see cref="AnnotationBox"/>.
		/// </summary>
		protected override void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox)
		{
			DrawAnnotationBox(Surface.FinalBuffer, _brush, _fontFactory, annotationText, annotationBox, Dpi);
		}

		/// <summary>
		/// Draws an error message in the Scene Graph's client area of the screen.
		/// </summary>
		[Obsolete("Renderer implementations are no longer responsible for handling render pipeline errors.")]
		protected override void ShowErrorMessage(string message)
		{
			using (var format = new StringFormat())
			{
				format.Trimming = StringTrimming.EllipsisCharacter;
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;
				format.FormatFlags = StringFormatFlags.NoClip;

				var font = _fontFactory.GetFont(FontFactory.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Point, FontFactory.GenericSansSerif);
				_brush.Color = Color.WhiteSmoke;
				Surface.FinalBuffer.Graphics.DrawString(message, font, _brush, Surface.ClipRectangle, format);
			}
		}

		#region Static Draw Helpers

		/// <summary>
		/// Draws an image graphic to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="imageGraphic">The image graphic to be drawn.</param>
		public static void DrawImageGraphic(BitmapBuffer buffer, ImageGraphic imageGraphic)
		{
			const int bytesPerPixel = 4;

			var bounds = ((IGdiBuffer) buffer).Bounds;
			var bitmapData = buffer.Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			try
			{
				ImageRenderer.Render(imageGraphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, bounds);
			}
			finally
			{
				buffer.Bitmap.UnlockBits(bitmapData);
			}
		}

		/// <summary>
		/// Draws an annotation box to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="brush">A GDI brush to use for drawing.</param>
		/// <param name="fontFactory">A GDI font factory to use for drawing.</param>
		/// <param name="annotationText">The annotation text to be drawn.</param>
		/// <param name="annotationBox">The annotation box to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawAnnotationBox(IGdiBuffer buffer, SolidBrush brush, FontFactory fontFactory, string annotationText, AnnotationBox annotationBox, float dpi = _nominalScreenDpi)
		{
			// if there's nothing to draw, there's nothing to do. go figure.
			if (string.IsNullOrEmpty(annotationText))
				return;

			var clientRectangle = RectangleUtilities.CalculateSubRectangle(buffer.Bounds, annotationBox.NormalizedRectangle);

			//Deflate the client rectangle by 4 pixels to allow some space 
			//between neighbouring rectangles whose borders coincide.
			Rectangle.Inflate(clientRectangle, -4, -4);

			var fontSize = (clientRectangle.Height/annotationBox.NumberOfLines) - 1;

			//don't draw it if it's too small to read, anyway.
			if (fontSize < MinimumFontSizeInPixels)
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

				var style = FontStyle.Regular;
				if (annotationBox.Bold)
					style |= FontStyle.Bold;
				if (annotationBox.Italics)
					style |= FontStyle.Italic;

				//don't draw it if it's too small to read, anyway.
				if (fontSize < MinimumFontSizeInPixels)
					return;

				var font = fontFactory.GetFont(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel, AnnotationBox.DefaultFont);
				var layoutArea = new SizeF(clientRectangle.Width, clientRectangle.Height);
				var size = buffer.Graphics.MeasureString(annotationText, font, layoutArea, format);
				if (annotationBox.FitWidth && size.Width > clientRectangle.Width)
				{
					fontSize = (int) (Math.Round(fontSize*clientRectangle.Width/(double) size.Width - 0.5));

					//don't draw it if it's too small to read, anyway.
					if (fontSize < MinimumFontSizeInPixels)
						return;

					font = fontFactory.GetFont(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel, AnnotationBox.DefaultFont);
				}

				// Draw drop shadow
				brush.Color = Color.Black;
				clientRectangle.Offset(1, 1);

				buffer.Graphics.DrawString(
					annotationText,
					font,
					brush,
					clientRectangle,
					format);

				brush.Color = Color.FromName(annotationBox.Color);
				clientRectangle.Offset(-1, -1);

				buffer.Graphics.DrawString(
					annotationText,
					font,
					brush,
					clientRectangle,
					format);
			}
		}

		/// <summary>
		/// Draws a text primitive to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="brush">A GDI brush to use for drawing.</param>
		/// <param name="fontFactory">A GDI font factory to use for drawing.</param>
		/// <param name="text">The text primitive to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawTextPrimitive(IGdiBuffer buffer, SolidBrush brush, FontFactory fontFactory, InvariantTextPrimitive text, float dpi = _nominalScreenDpi)
		{
			text.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				// We adjust the font size depending on the scale so that it's the same size
				// irrespective of the zoom
				var fontSize = CalculateScaledFontPoints(text.SizeInPoints, dpi);
				var font = fontFactory.GetFont(text.Font, fontSize, FontStyle.Regular, GraphicsUnit.Point, FontFactory.GenericSansSerif);

				// Calculate how big the text will be so we can set the bounding box
				text.Dimensions = buffer.Graphics.MeasureString(text.Text, font);

				// Draw drop shadow
				brush.Color = Color.Black;

				var dropShadowOffset = new SizeF(1, 1);
				var boundingBoxTopLeft = new PointF(text.BoundingBox.Left, text.BoundingBox.Top);

				buffer.Graphics.DrawString(
					text.Text,
					font,
					brush,
					boundingBoxTopLeft + dropShadowOffset);

				// Draw text
				brush.Color = text.Color;

				buffer.Graphics.DrawString(
					text.Text,
					font,
					brush,
					boundingBoxTopLeft);
			}
			finally
			{
				text.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Draws a point primitive to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="brush">A GDI brush to use for drawing.</param>
		/// <param name="point">The point primitive to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawPointPrimitive(IGdiBuffer buffer, SolidBrush brush, PointPrimitive point, float dpi = _nominalScreenDpi)
		{
			buffer.Graphics.Transform = point.SpatialTransform.CumulativeTransform;
			point.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				var dropShadowOffset = GetDropShadowOffset(point, dpi);
				var width = CalculateScaledPenWidth(point, 1, dpi);

				// Draw drop shadow
				brush.Color = Color.Black;

				buffer.Graphics.FillRectangle(
					brush,
					point.Point.X + dropShadowOffset.Width,
					point.Point.Y + dropShadowOffset.Height,
					width,
					width);

				// Draw point
				brush.Color = point.Color;

				buffer.Graphics.FillRectangle(
					brush,
					point.Point.X,
					point.Point.Y,
					width,
					width);
			}
			finally
			{
				point.ResetCoordinateSystem();
				buffer.Graphics.ResetTransform();
			}
		}

		/// <summary>
		/// Draws an arc primitive to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="pen">A GDI pen to use for drawing.</param>
		/// <param name="arc">The arc primitive to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawArcPrimitive(IGdiBuffer buffer, Pen pen, IArcGraphic arc, float dpi = _nominalScreenDpi)
		{
			buffer.Graphics.Transform = arc.SpatialTransform.CumulativeTransform;
			arc.CoordinateSystem = CoordinateSystem.Source;
			buffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try
			{
				var rectangle = new RectangleF(arc.TopLeft.X, arc.TopLeft.Y, arc.Width, arc.Height);
				rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
				var dropShadowOffset = GetDropShadowOffset(arc, dpi);

				// Draw drop shadow
				pen.Color = Color.Black;
				pen.Width = CalculateScaledPenWidth(arc, 1, dpi);

				SetDashStyle(pen, arc);

				buffer.Graphics.DrawArc(
					pen,
					rectangle.Left + dropShadowOffset.Width,
					rectangle.Top + dropShadowOffset.Height,
					rectangle.Width,
					rectangle.Height,
					arc.StartAngle,
					arc.SweepAngle);

				// Draw rectangle
				pen.Color = arc.Color;

				buffer.Graphics.DrawArc(
					pen,
					rectangle.Left,
					rectangle.Top,
					rectangle.Width,
					rectangle.Height,
					arc.StartAngle,
					arc.SweepAngle);
			}
			finally
			{
				buffer.Graphics.SmoothingMode = SmoothingMode.None;
				arc.ResetCoordinateSystem();
				buffer.Graphics.ResetTransform();
			}
		}

		/// <summary>
		/// Draws a line primitive to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="pen">A GDI pen to use for drawing.</param>
		/// <param name="line">The line primitive to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawLinePrimitive(IGdiBuffer buffer, Pen pen, ILineSegmentGraphic line, float dpi = _nominalScreenDpi)
		{
			buffer.Graphics.Transform = line.SpatialTransform.CumulativeTransform;
			line.CoordinateSystem = CoordinateSystem.Source;
			buffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try
			{
				// Draw drop shadow
				pen.Color = Color.Black;
				pen.Width = CalculateScaledPenWidth(line, 1, dpi);

				SetDashStyle(pen, line);

				var dropShadowOffset = GetDropShadowOffset(line, dpi);
				buffer.Graphics.DrawLine(
					pen,
					line.Point1 + dropShadowOffset,
					line.Point2 + dropShadowOffset);

				// Draw line
				pen.Color = line.Color;

				buffer.Graphics.DrawLine(
					pen,
					line.Point1,
					line.Point2);
			}
			finally
			{
				buffer.Graphics.SmoothingMode = SmoothingMode.None;
				line.ResetCoordinateSystem();
				buffer.Graphics.ResetTransform();
			}
		}

		/// <summary>
		/// Draws a spline primitive to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="pen">A GDI pen to use for drawing.</param>
		/// <param name="spline">The spline primitive to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawSplinePrimitive(IGdiBuffer buffer, Pen pen, SplinePrimitive spline, float dpi = _nominalScreenDpi)
		{
			buffer.Graphics.Transform = spline.SpatialTransform.CumulativeTransform;
			spline.CoordinateSystem = CoordinateSystem.Source;
			buffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try
			{
				// Draw drop shadow
				pen.Color = Color.Black;
				pen.Width = CalculateScaledPenWidth(spline, 1, dpi);

				SetDashStyle(pen, spline);

				var dropShadowOffset = GetDropShadowOffset(spline, dpi);
				var pathPoints = GetCurvePoints(spline.Points, dropShadowOffset);

				if (spline.Points.IsClosed)
					buffer.Graphics.DrawClosedCurve(pen, pathPoints);
				else
					buffer.Graphics.DrawCurve(pen, pathPoints);

				// Draw line
				pen.Color = spline.Color;
				pathPoints = GetCurvePoints(spline.Points, SizeF.Empty);

				if (spline.Points.IsClosed)
					buffer.Graphics.DrawClosedCurve(pen, pathPoints);
				else
					buffer.Graphics.DrawCurve(pen, pathPoints);
			}
			finally
			{
				buffer.Graphics.SmoothingMode = SmoothingMode.None;
				spline.ResetCoordinateSystem();
				buffer.Graphics.ResetTransform();
			}
		}

		/// <summary>
		/// Draws a rectangle primitive to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="pen">A GDI pen to use for drawing.</param>
		/// <param name="rect">The rectangle primitive to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawRectanglePrimitive(IGdiBuffer buffer, Pen pen, IBoundableGraphic rect, float dpi = _nominalScreenDpi)
		{
			buffer.Graphics.Transform = rect.SpatialTransform.CumulativeTransform;
			rect.CoordinateSystem = CoordinateSystem.Source;
			buffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try
			{
				var rectangle = new RectangleF(rect.TopLeft.X, rect.TopLeft.Y, rect.Width, rect.Height);
				rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
				var dropShadowOffset = GetDropShadowOffset(rect, dpi);

				// Draw drop shadow
				pen.Color = Color.Black;
				pen.Width = CalculateScaledPenWidth(rect, 1, dpi);

				SetDashStyle(pen, rect);

				buffer.Graphics.DrawRectangle(
					pen,
					rectangle.Left + dropShadowOffset.Width,
					rectangle.Top + dropShadowOffset.Height,
					rectangle.Width,
					rectangle.Height);

				// Draw rectangle
				pen.Color = rect.Color;

				buffer.Graphics.DrawRectangle(
					pen,
					rectangle.Left,
					rectangle.Top,
					rectangle.Width,
					rectangle.Height);
			}
			finally
			{
				buffer.Graphics.SmoothingMode = SmoothingMode.None;
				rect.ResetCoordinateSystem();
				buffer.Graphics.ResetTransform();
			}
		}

		/// <summary>
		/// Draws an ellipse primitive to the specified destination buffer.
		/// </summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="pen">A GDI pen to use for drawing.</param>
		/// <param name="ellipse">The ellipse primitive to be drawn.</param>
		/// <param name="dpi">The intended output DPI.</param>
		public static void DrawEllipsePrimitive(IGdiBuffer buffer, Pen pen, IBoundableGraphic ellipse, float dpi = _nominalScreenDpi)
		{
			buffer.Graphics.Transform = ellipse.SpatialTransform.CumulativeTransform;
			ellipse.CoordinateSystem = CoordinateSystem.Source;
			buffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try
			{
				var rectangle = new RectangleF(ellipse.TopLeft.X, ellipse.TopLeft.Y, ellipse.Width, ellipse.Height);
				rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
				var dropShadowOffset = GetDropShadowOffset(ellipse, dpi);

				// Draw drop shadow
				pen.Color = Color.Black;
				pen.Width = CalculateScaledPenWidth(ellipse, 1, dpi);

				SetDashStyle(pen, ellipse);

				buffer.Graphics.DrawEllipse(
					pen,
					rectangle.Left + dropShadowOffset.Width,
					rectangle.Top + dropShadowOffset.Height,
					rectangle.Width,
					rectangle.Height);

				// Draw rectangle
				pen.Color = ellipse.Color;

				buffer.Graphics.DrawEllipse(
					pen,
					rectangle.Left,
					rectangle.Top,
					rectangle.Width,
					rectangle.Height);
			}
			finally
			{
				buffer.Graphics.SmoothingMode = SmoothingMode.None;
				ellipse.ResetCoordinateSystem();
				buffer.Graphics.ResetTransform();
			}
		}

		private static void SetDashStyle(Pen pen, IVectorGraphic graphic)
		{
			if (graphic.LineStyle == LineStyle.Solid)
			{
				pen.DashStyle = DashStyle.Solid;
			}
			else
			{
				pen.DashStyle = DashStyle.Custom;

				if (graphic.LineStyle == LineStyle.Dash)
					pen.DashPattern = new[] {4.0F, 4.0F};
				else
					pen.DashPattern = new[] {2.0F, 4.0F};
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
}

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A 2D Renderer that uses GDI+.
	/// </summary>
	/// <remarks>
	/// The default 2D GDI+ <see cref="IRenderer"/> has moved to <see cref="GDI.GdiRenderer"/>.
	/// This class exists solely to provide backward compatibility for code that referenced the GdiRenderer in this namespace.
	/// </remarks>
	[Obsolete("The default GdiRenderer has moved to the ClearCanvas.ImageViewer.Rendering.GDI namespace.")]
	public class GdiRenderer : GDI.GdiRenderer {}
}