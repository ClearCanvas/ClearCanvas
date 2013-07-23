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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A factory for <see cref="GdiRenderer"/>s.
	/// </summary>
	internal sealed class GdiRendererFactory : RendererFactoryBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public GdiRendererFactory()
		{
		}

		/// <summary>
		/// Allocates a new <see cref="GdiRenderer"/>.
		/// </summary>
		protected override RendererBase GetNewRenderer()
		{
			return new GdiRenderer();
		}
	}

	/// <summary>
	/// A 2D Renderer that uses GDI.
	/// </summary>
	public class GdiRenderer : RendererBase
	{
#pragma warning disable 1591

		protected static readonly ushort MinimumFontSizeInPixels = 4;

#pragma warning restore 1591

		private const float _nominalScreenDpi = 96;
		private const string _defaultFont = "Arial";

		private Pen _pen;
		private SolidBrush _brush;

	    private FontFactory _fontFactory;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public GdiRenderer()
			: base()
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
		/// Draws an <see cref="ImageGraphic"/>.
		/// </summary>
		protected override void DrawImageGraphic(ImageGraphic imageGraphic)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			const int bytesPerPixel = 4;

			Surface.ImageBuffer.Graphics.Clear(Color.FromArgb(0x0, 0xFF, 0xFF, 0xFF));

			BitmapData bitmapData = Surface.ImageBuffer.Bitmap.LockBits(
				new Rectangle(0, 0, Surface.ImageBuffer.Bitmap.Width, Surface.ImageBuffer.Bitmap.Height),
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			try
			{
				ImageRenderer.Render(imageGraphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, Surface.ClientRectangle);
			}
			finally
			{
				Surface.ImageBuffer.Bitmap.UnlockBits(bitmapData);
			}

			Surface.FinalBuffer.RenderImage(Surface.ImageBuffer);

			clock.Stop();
			PerformanceReportBroker.PublishReport("GDIRenderer", "DrawImageGraphic", clock.Seconds);
		}

		/// <summary>
		/// Draws a <see cref="LinePrimitive"/>.
		/// </summary>
		protected override void DrawLinePrimitive(LinePrimitive line)
		{
			InternalDrawLinePrimitive(line);
		}

		/// <summary>
		/// Draws a <see cref="InvariantLinePrimitive"/>.
		/// </summary>
		protected override void DrawInvariantLinePrimitive(InvariantLinePrimitive line)
		{
			InternalDrawLinePrimitive(line);
		}

		/// <summary>
		/// Draws a <see cref="CurvePrimitive"/>.
		/// </summary>
		protected override void DrawCurvePrimitive(CurvePrimitive curve)
		{
			Surface.FinalBuffer.Graphics.Transform = curve.SpatialTransform.CumulativeTransform;
			curve.CoordinateSystem = CoordinateSystem.Source;

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(curve, 1, Dpi);

			SetDashStyle(curve);

			SizeF dropShadowOffset = GetDropShadowOffset(curve, Dpi);
			PointF[] pathPoints = GetCurvePoints(curve.Points, dropShadowOffset);

			if (curve.Points.IsClosed)
				Surface.FinalBuffer.Graphics.DrawClosedCurve(_pen, pathPoints);
			else
				Surface.FinalBuffer.Graphics.DrawCurve(_pen, pathPoints);

			// Draw line
			_pen.Color = curve.Color;
			pathPoints = GetCurvePoints(curve.Points, SizeF.Empty);

			if (curve.Points.IsClosed)
				Surface.FinalBuffer.Graphics.DrawClosedCurve(_pen, pathPoints);
			else
				Surface.FinalBuffer.Graphics.DrawCurve(_pen, pathPoints);

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.None;

			curve.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws a <see cref="RectanglePrimitive"/>.
		/// </summary>
		protected override void DrawRectanglePrimitive(RectanglePrimitive rect)
		{
			InternalDrawRectanglePrimitive(rect);
		}

		/// <summary>
		/// Draws a <see cref="InvariantRectanglePrimitive"/>.
		/// </summary>
		protected override void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
		{
			InternalDrawRectanglePrimitive(rect);
		}

		/// <summary>
		/// Draws a <see cref="EllipsePrimitive"/>.
		/// </summary>
		protected override void DrawEllipsePrimitive(EllipsePrimitive ellipse)
		{
			InternalDrawEllipsePrimitive(ellipse);
		}

		/// <summary>
		/// Draws a <see cref="InvariantEllipsePrimitive"/>.
		/// </summary>
		protected override void DrawInvariantEllipsePrimitive(InvariantEllipsePrimitive ellipse)
		{
			InternalDrawEllipsePrimitive(ellipse);
		}

		/// <summary>
		/// Draws a <see cref="ArcPrimitive"/>.
		/// </summary>
		protected override void DrawArcPrimitive(IArcGraphic arc)
		{
			Surface.FinalBuffer.Graphics.Transform = arc.SpatialTransform.CumulativeTransform;
			arc.CoordinateSystem = CoordinateSystem.Source;

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			RectangleF rectangle = new RectangleF(arc.TopLeft.X, arc.TopLeft.Y, arc.Width, arc.Height);
			rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
			SizeF dropShadowOffset = GetDropShadowOffset(arc, Dpi);

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(arc, 1, Dpi);

			SetDashStyle(arc);

			
			Surface.FinalBuffer.Graphics.DrawArc(
				_pen,
				rectangle.Left + dropShadowOffset.Width,
				rectangle.Top + dropShadowOffset.Height,
				rectangle.Width,
				rectangle.Height,
				arc.StartAngle,
				arc.SweepAngle);

			// Draw rectangle
			_pen.Color = arc.Color;

			Surface.FinalBuffer.Graphics.DrawArc(
				_pen,
				rectangle.Left,
				rectangle.Top,
				rectangle.Width,
				rectangle.Height,
				arc.StartAngle,
				arc.SweepAngle);

			arc.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws a <see cref="PointPrimitive"/>.
		/// </summary>
		protected override void DrawPointPrimitive(PointPrimitive pointPrimitive)
		{
			Surface.FinalBuffer.Graphics.Transform = pointPrimitive.SpatialTransform.CumulativeTransform;
			pointPrimitive.CoordinateSystem = CoordinateSystem.Source;

			SizeF dropShadowOffset = GetDropShadowOffset(pointPrimitive, Dpi);
			var width = CalculateScaledPenWidth(pointPrimitive, 1, Dpi);

			// Draw drop shadow
			_brush.Color = Color.Black;

			Surface.FinalBuffer.Graphics.FillRectangle(
				_brush,
				pointPrimitive.Point.X + dropShadowOffset.Width,
				pointPrimitive.Point.Y + dropShadowOffset.Height,
				width,
				width);

			// Draw point
			_brush.Color = pointPrimitive.Color;

			Surface.FinalBuffer.Graphics.FillRectangle(
				_brush,
				pointPrimitive.Point.X,
				pointPrimitive.Point.Y,
				width,
				width);

			pointPrimitive.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws an <see cref="InvariantTextPrimitive"/>.
		/// </summary>
		protected override void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
			textPrimitive.CoordinateSystem = CoordinateSystem.Destination;

			// We adjust the font size depending on the scale so that it's the same size
			// irrespective of the zoom
			var fontSize = CalculateScaledFontPoints(textPrimitive.SizeInPoints, Dpi);
			Font font = _fontFactory.CreateFont(textPrimitive.Font, fontSize, FontStyle.Regular, GraphicsUnit.Point, _defaultFont);

			// Calculate how big the text will be so we can set the bounding box
			textPrimitive.Dimensions = Surface.FinalBuffer.Graphics.MeasureString(textPrimitive.Text, font);

			// Draw drop shadow
			_brush.Color = Color.Black;

			SizeF dropShadowOffset = GetScaledFontDropShadowOffset(Dpi);
			PointF boundingBoxTopLeft = new PointF(textPrimitive.BoundingBox.Left, textPrimitive.BoundingBox.Top);

			Surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				boundingBoxTopLeft + dropShadowOffset);

			// Draw text
			_brush.Color = textPrimitive.Color;

			Surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				boundingBoxTopLeft);

			textPrimitive.ResetCoordinateSystem();
		}

		/// <summary>
		/// Draws an <see cref="AnnotationBox"/>.
		/// </summary>
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
		    if (fontSize < MinimumFontSizeInPixels)
		        return;

		    StringFormat format = new StringFormat();

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
		    if (fontSize < MinimumFontSizeInPixels)
		        return;

		    Font font = _fontFactory.CreateFont(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel,
		                                        AnnotationBox.DefaultFont);
		    SizeF layoutArea = new SizeF(clientRectangle.Width, clientRectangle.Height);
		    SizeF size = Surface.FinalBuffer.Graphics.MeasureString(annotationText, font, layoutArea, format);
		    if (annotationBox.FitWidth && size.Width > clientRectangle.Width)
		    {
		        fontSize = (int) (Math.Round(fontSize*clientRectangle.Width/(double) size.Width - 0.5));

		        //don't draw it if it's too small to read, anyway.
		        if (fontSize < MinimumFontSizeInPixels)
		            return;

		        font = _fontFactory.CreateFont(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel,
		                                       AnnotationBox.DefaultFont);
		    }

		    // Draw drop shadow
		    _brush.Color = Color.Black;
		    clientRectangle.Offset(1, 1);

		    Surface.FinalBuffer.Graphics.DrawString(
		        annotationText,
		        font,
		        _brush,
		        clientRectangle,
		        format);

		    _brush.Color = Color.FromName(annotationBox.Color);
		    clientRectangle.Offset(-1, -1);

		    Surface.FinalBuffer.Graphics.DrawString(
		        annotationText,
		        font,
		        _brush,
		        clientRectangle,
		        format);
		}

	    /// <summary>
		/// Draws an error message in the Scene Graph's client area of the screen.
		/// </summary>
		[Obsolete("Renderer implementations are no longer responsible for handling render pipeline errors.")]
		protected override void ShowErrorMessage(string message)
		{
			Font font = new Font(_defaultFont, 12.0f);

			StringFormat format = new StringFormat();
			format.Trimming = StringTrimming.EllipsisCharacter;
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			format.FormatFlags = StringFormatFlags.NoClip;

			_brush.Color = Color.WhiteSmoke;
			Surface.FinalBuffer.Graphics.DrawString(message, font, _brush, Surface.ClipRectangle, format);
		}

		private void InternalDrawLinePrimitive(ILineSegmentGraphic line) {
			Surface.FinalBuffer.Graphics.Transform = line.SpatialTransform.CumulativeTransform;
			line.CoordinateSystem = CoordinateSystem.Source;

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(line, 1, Dpi);

			SetDashStyle(line);

			SizeF dropShadowOffset = GetDropShadowOffset(line, Dpi);
			Surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Point1 + dropShadowOffset,
				line.Point2 + dropShadowOffset);

			// Draw line
			_pen.Color = line.Color;

			Surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Point1,
				line.Point2);

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.None;

			line.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		private void InternalDrawRectanglePrimitive(IBoundableGraphic rect)
		{
			Surface.FinalBuffer.Graphics.Transform = rect.SpatialTransform.CumulativeTransform;
			rect.CoordinateSystem = CoordinateSystem.Source;

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			RectangleF rectangle = new RectangleF(rect.TopLeft.X, rect.TopLeft.Y, rect.Width, rect.Height);
			rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
			SizeF dropShadowOffset = GetDropShadowOffset(rect, Dpi);

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(rect, 1, Dpi);

			SetDashStyle(rect);

			Surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rectangle.Left + dropShadowOffset.Width,
				rectangle.Top + dropShadowOffset.Height,
				rectangle.Width,
				rectangle.Height);

			// Draw rectangle
			_pen.Color = rect.Color;

			Surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rectangle.Left,
				rectangle.Top,
				rectangle.Width,
				rectangle.Height);

			rect.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		private void InternalDrawEllipsePrimitive(IBoundableGraphic ellipse)
		{
			Surface.FinalBuffer.Graphics.Transform = ellipse.SpatialTransform.CumulativeTransform;
			ellipse.CoordinateSystem = CoordinateSystem.Source;

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			RectangleF rectangle = new RectangleF(ellipse.TopLeft.X, ellipse.TopLeft.Y, ellipse.Width, ellipse.Height);
			rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
			SizeF dropShadowOffset = GetDropShadowOffset(ellipse, Dpi);

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(ellipse, 1, Dpi);

			SetDashStyle(ellipse);

			Surface.FinalBuffer.Graphics.DrawEllipse(
				_pen,
				rectangle.Left + dropShadowOffset.Width,
				rectangle.Top + dropShadowOffset.Height,
				rectangle.Width,
				rectangle.Height);

			// Draw rectangle
			_pen.Color = ellipse.Color;

			Surface.FinalBuffer.Graphics.DrawEllipse(
				_pen,
				rectangle.Left,
				rectangle.Top,
				rectangle.Width,
				rectangle.Height);

			ellipse.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
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
					_pen.DashPattern = new float[] { 4.0F, 4.0F };
				else
					_pen.DashPattern = new float[] { 2.0F, 4.0F };
			}
		}

		private static float CalculateScaledFontPoints(float fontPoints, float dpi)
		{
			return fontPoints*dpi/_nominalScreenDpi;
		}

		private static SizeF GetScaledFontDropShadowOffset(float dpi)
		{
			float offset = CalculateScaledFontPoints(1, dpi);
			return new SizeF(offset, offset);
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

		/// <summary>
		/// Factory method for an <see cref="IRenderingSurface"/>.
		/// </summary>
		public sealed override IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			return new GdiRenderingSurface(windowID, width, height);
		}
	}
}
