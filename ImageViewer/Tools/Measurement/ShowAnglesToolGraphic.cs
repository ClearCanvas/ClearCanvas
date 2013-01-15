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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using Matrix2D=System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	//TODO (CR Mar 2010): general note: variable names should be more explanatory
	partial class ShowAnglesTool
	{
		[Cloneable]
		internal partial class ShowAnglesToolGraphic : CompositeGraphic, IVectorGraphic
		{
			private const int _minLineLength = 10;
			private const int _minLength = 36;

			[CloneIgnore]
			private readonly PointsList _endPoints;

			[CloneIgnore]
			private AngleCalloutGraphic _angleCalloutGraphic1;

			[CloneIgnore]
			private AngleCalloutGraphic _angleCalloutGraphic2;

			[CloneIgnore]
			private ILineSegmentGraphic _extenderLine1;

			[CloneIgnore]
			private ILineSegmentGraphic _extenderLine2;

			[CloneIgnore]
			private ILineSegmentGraphic _riserLine1;

			[CloneIgnore]
			private ILineSegmentGraphic _riserLine2;

			public ShowAnglesToolGraphic()
			{
				base.Graphics.Add(_angleCalloutGraphic1 = new AngleCalloutGraphic());
				base.Graphics.Add(_angleCalloutGraphic2 = new AngleCalloutGraphic());
				base.Graphics.Add(_extenderLine1 = new LinePrimitive());
				base.Graphics.Add(_extenderLine2 = new LinePrimitive());
				base.Graphics.Add(_riserLine1 = new LinePrimitive());
				base.Graphics.Add(_riserLine2 = new LinePrimitive());

				_angleCalloutGraphic1.ShowArrowhead = _angleCalloutGraphic2.ShowArrowhead = false;
				_angleCalloutGraphic1.LineStyle = _angleCalloutGraphic2.LineStyle = LineStyle.Dash;
				_angleCalloutGraphic1.Name = "callout1";
				_angleCalloutGraphic2.Name = "callout2";
				_extenderLine1.Name = "extender1";
				_extenderLine2.Name = "extender2";
				_riserLine1.Name = "riser1";
				_riserLine2.Name = "riser2";

				_endPoints = new PointsList(new PointF[] {PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty}, this);

				this.Color = Color.Coral;
				this.LineStyle = LineStyle.Dot;
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ShowAnglesToolGraphic(ShowAnglesToolGraphic source, ICloningContext context) : base()
			{
				context.CloneFields(source, this);

				_endPoints = new PointsList(source._endPoints, this);
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				_angleCalloutGraphic1 = (AngleCalloutGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g.Name == "callout1");
				_angleCalloutGraphic2 = (AngleCalloutGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g.Name == "callout2");
				_extenderLine1 = (ILineSegmentGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g.Name == "extender1");
				_extenderLine2 = (ILineSegmentGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g.Name == "extender2");
				_riserLine1 = (ILineSegmentGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g.Name == "riser1");
				_riserLine2 = (ILineSegmentGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g.Name == "riser2");
			}

			public Color Color
			{
				get { return _angleCalloutGraphic1.Color; }
				set
				{
					_angleCalloutGraphic1.Color = _angleCalloutGraphic2.Color = value;
					_extenderLine1.Color = _extenderLine2.Color = value;
					_riserLine1.Color = _riserLine2.Color = value;
				}
			}

			public LineStyle LineStyle
			{
				get { return _angleCalloutGraphic1.LineStyle; }
				set
				{
					_extenderLine1.LineStyle = _extenderLine2.LineStyle = value;
					_riserLine1.LineStyle = _riserLine2.LineStyle = value;
				}
			}

			public void SetEndpoints(PointF p1, PointF p2, PointF q1, PointF q2)
			{
				_endPoints[0] = p1;
				_endPoints[1] = p2;
				_endPoints[2] = q1;
				_endPoints[3] = q2;

				this.Update();
			}

			private void Update()
			{
				const float calloutOffset = 36;
				const float largerCalloutOffset = 64;

				_angleCalloutGraphic1.Visible = _angleCalloutGraphic2.Visible = false;
				_extenderLine1.Visible = _extenderLine2.Visible = false;
				_riserLine1.Visible = _riserLine2.Visible = false;

				if (this.ParentPresentationImage == null)
					return;

				this.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					PointF p1 = _endPoints[0];
					PointF p2 = _endPoints[1];
					PointF q1 = _endPoints[2];
					PointF q2 = _endPoints[3];
					RectangleF bounds = base.ParentPresentationImage.ClientRectangle;

					// show nothing if either line is completely outside the visible client rectangle
					if (!RestrictLine(ref p1, ref p2, bounds) || !RestrictLine(ref q1, ref q2, bounds))
						return;

					// show nothing if the two lines appear as one (i.e. they are completely coincident)
					if (FloatComparer.AreEqual(p1, q1) && FloatComparer.AreEqual(p2, q2))
						return;

					// show nothing if either line is extremely small so as to make comparing the lines' angle difficult
					if (Vector.Distance(p1, p2) < _minLineLength || Vector.Distance(q1, q2) < _minLineLength)
						return;

					PointF intersection;
					if (Vector.AreColinear(p1, p2, q1, q2))
					{
						// line segments are colinear - do nothing
						return;
					}
					else if (!Vector.IntersectLineSegments(p1, p2, q1, q2, out intersection))
					{
						// the line segments do not intersect onscreen, so figure out where they would have intersected and decide from there
						bool areParallel = !Vector.IntersectLines(p1, p2, q1, q2, out intersection);

						bool drawRisers = false;
						if (areParallel || !base.ParentPresentationImage.ClientRectangle.Contains(Point.Round(intersection)))
						{
							// the virtual intersection isn't onscreen either - average the points to find a suitable onscreen intersection
							intersection = new PointF((p1.X + p2.X + q1.X + q2.X)/4, (p1.Y + p2.Y + q1.Y + q2.Y)/4);
							drawRisers = true;
						}

						PointF pV = DrawExtenderLine(_extenderLine1, ref p1, ref p2, intersection);
						PointF qV = DrawExtenderLine(_extenderLine2, ref q1, ref q2, intersection);

						if (drawRisers)
						{
							DrawRiserLine(_riserLine1, ref p1, ref p2, pV, intersection);
							DrawRiserLine(_riserLine2, ref q1, ref q2, qV, intersection);
						}
					}

					// draw the callouts around the actual intersection

					double angle = MeasureAngle();
					string textAngle = string.Format(SR.ToolsMeasurementFormatDegrees, angle);
					string textComplementaryAngle = string.Format(SR.ToolsMeasurementFormatDegrees, 180 - angle);

					bool p1MeetsThreshold = Vector.Distance(p1, intersection) >= _minLength;
					bool p2MeetsThreshold = Vector.Distance(p2, intersection) >= _minLength;
					bool q1MeetsThreshold = Vector.Distance(q1, intersection) >= _minLength;
					bool q2MeetsThreshold = Vector.Distance(q2, intersection) >= _minLength;

					if (p2MeetsThreshold && q2MeetsThreshold)
						DrawAngleCallout(_angleCalloutGraphic1, textAngle, intersection, BisectAngle(p2, intersection, q2, angle > 30 ? calloutOffset : largerCalloutOffset));
					else if (p1MeetsThreshold && q1MeetsThreshold)
						DrawAngleCallout(_angleCalloutGraphic1, textAngle, intersection, BisectAngle(p1, intersection, q1, angle > 30 ? calloutOffset : largerCalloutOffset));

					if (p1MeetsThreshold && q2MeetsThreshold)
						DrawAngleCallout(_angleCalloutGraphic2, textComplementaryAngle, intersection, BisectAngle(q2, intersection, p1, angle < 150 ? calloutOffset : largerCalloutOffset));
					else if (p2MeetsThreshold && q1MeetsThreshold)
						DrawAngleCallout(_angleCalloutGraphic2, textComplementaryAngle, intersection, BisectAngle(q1, intersection, p2, angle < 150 ? calloutOffset : largerCalloutOffset));
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}

			private double MeasureAngle()
			{
				float aspectRatio = 1f;

				IImageSopProvider imageSopProvider = this.ParentPresentationImage as IImageSopProvider;
				if (imageSopProvider != null)
				{
					if (!imageSopProvider.Frame.PixelAspectRatio.IsNull)
						aspectRatio = imageSopProvider.Frame.PixelAspectRatio.Value;
					else if (!imageSopProvider.Frame.NormalizedPixelSpacing.IsNull)
						aspectRatio = (float) imageSopProvider.Frame.NormalizedPixelSpacing.AspectRatio;
				}

				this.CoordinateSystem = CoordinateSystem.Source;
				try
				{
					PointF p1 = _endPoints[1];
					PointF p2 = _endPoints[0];
					PointF p3 = _endPoints[3] + new SizeF(_endPoints[0]) - new SizeF(_endPoints[2]);
					p1.Y *= aspectRatio;
					p2.Y *= aspectRatio;
					p3.Y *= aspectRatio;
					return Math.Abs(Vector.SubtendedAngle(p1, p2, p3));
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}

			/// <summary>
			/// Draws <paramref name="riser"/> to rise from the point <paramref name="pV"/> on the vector P (defined by <paramref name="p1"/> <paramref name="p2"/>)
			/// to the point <paramref name="intersection"/>. The vector P is redefined to be the vector between <paramref name="pV"/> and
			/// <paramref name="intersection"/>, with direction depending on original orientation of vector P.
			/// </summary>
			/// <param name="p1"></param>
			/// <param name="p2"></param>
			/// <param name="pV"></param>
			/// <param name="intersection"></param>
			private static void DrawRiserLine(ILineSegmentGraphic riser, ref PointF p1, ref PointF p2, PointF pV, PointF intersection)
			{
				// extend riser past intersection by a bit
				PointF pW = new PointF(intersection.X - pV.X, intersection.Y - pV.Y);
				float pF = 1 + (float) ((_minLength + 1)/Math.Sqrt((pW.X*pW.X + pW.Y*pW.Y)));
				pW = new PointF(pV.X + pW.X*pF, pV.Y + pW.Y*pF);

				//TODO (CR Mar 2010): this kind of calculation in a function (CrossProduct)
				if ((pW.X - pV.X)*(p2.Y - p1.Y) - (pW.Y - pV.Y)*(p2.X - p1.X) < 0)
				{
					PointF temp = pW;
					pW = pV;
					pV = temp;
				}
				riser.Visible = true;
				riser.Point1 = p1 = pV;
				riser.Point2 = p2 = pW;
			}

			/// <summary>
			/// Draws <paramref name="extender"/> to extend the vector P (defined by <paramref name="p1"/> <paramref name="p2"/>)
			/// to the point where <paramref name="p1"/> <paramref name="pX"/> projects onto P. The projected point is returned.
			/// Vector P is redefined to be the vector including the project point.
			/// </summary>
			private static PointF DrawExtenderLine(ILineSegmentGraphic extender, ref PointF p1, ref PointF p2, PointF pX)
			{
				float pF = DotProduct(p1, pX, p1, p2)/DotProduct(p1, p2, p1, p2);
				PointF pC = new PointF(p1.X + pF*(p2.X - p1.X), p1.Y + pF*(p2.Y - p1.Y));

				if (pF > 1)
				{
					extender.Visible = true;
					extender.Point1 = p2;
					extender.Point2 = p2 = pC;
				}
				else if (pF < 0)
				{
					extender.Visible = true;
					extender.Point2 = p1;
					extender.Point1 = p1 = pC;
				}

				return pC;
			}

			/// <summary>
			/// Draws <paramref name="callout"/> with the specified <paramref name="text"/> at <paramref name="location"/> with the callout
			/// line originating from <paramref name="anchor"/>.
			/// </summary>
			private static void DrawAngleCallout(AngleCalloutGraphic callout, string text, PointF anchor, PointF location)
			{
				callout.Text = text;
				callout.Visible = !string.IsNullOrEmpty(text);
				callout.AnchorPoint = anchor;
				callout.TextLocation = location;
			}

			private static bool RestrictLine(ref PointF p1, ref PointF p2, RectangleF bounds)
			{
				bool p1Inside = bounds.Contains(p1);
				bool p2Inside = bounds.Contains(p2);

				if (p1Inside && p2Inside)
					return true;
				//TODO (CR Mar 2010): supposed to push enpoints inside bounds, but doesn't in case where both endpoints
				//are outside, but the line segment intersects a corner of the rectangle.

				PointF[] sides = new PointF[] {new PointF(bounds.Left, bounds.Top), new PointF(bounds.Right, bounds.Top), new PointF(bounds.Right, bounds.Bottom), new PointF(bounds.Left, bounds.Bottom)};

				for (int n = 0; n < 4; n++)
				{
					PointF intersection;
					if (Vector.IntersectLineSegments(p1, p2, sides[n], sides[(n + 1)%4], out intersection))
					{
						if (p1Inside)
							p2 = intersection;
						else if (p2Inside)
							p1 = intersection;
						return true;
					}
				}
				return false;
			}

			private static float DotProduct(PointF p1, PointF p2, PointF q1, PointF q2)
			{
				return (p2.X - p1.X)*(q2.X - q1.X) + (p2.Y - p1.Y)*(q2.Y - q1.Y);
			}

			/// <summary>
			/// Bisects the inner angle formed by <paramref name="point1"/> <paramref name="point2"/> <paramref name="point3"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Based largely on <see cref="ProtractorRoiCalloutLocationStrategy"/>.
			/// </para>
			/// <para>
			/// The return value is a point within the angle such that the line segment
			/// from this point to <paramref name="point2"/> has the specified <paramref name="magnitude"/> and bisects the angle
			/// <paramref name="point1"/> <paramref name="point2"/> <paramref name="point3"/>.
			/// </para>
			/// </remarks>
			private static PointF BisectAngle(PointF point1, PointF point2, PointF point3, float magnitude)
			{
				PointF[] points = new PointF[] {point1, point3};
				using (Matrix2D rotation = new Matrix2D())
				{
					rotation.Rotate((float) (-Vector.SubtendedAngle(point1, point2, point3)/2 + 180));
					rotation.Translate(-point2.X, -point2.Y);
					rotation.TransformPoints(points);
				}

				Vector3D result = new Vector3D(points[0].X, points[0].Y, 0);
				if (FloatComparer.AreEqual(result.Magnitude, 0F, 0.01F))
					result = new Vector3D(-1, 0, 0);
				result = result/result.Magnitude*magnitude;

				return new PointF(point2.X - result.X, point2.Y - result.Y);
			}
		}

		[Cloneable]
		private class AngleCalloutGraphic : CalloutGraphic
		{
			public AngleCalloutGraphic() {}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected AngleCalloutGraphic(AngleCalloutGraphic source, ICloningContext context) : base(source, context)
			{
				context.CloneFields(source, this);
			}

			public new string Text
			{
				get { return base.Text; }
				set { base.Text = value; }
			}

			protected override IControlGraphic InitializePointControlGraphic(IPointGraphic pointGraphic)
			{
				return new ControlGraphic(pointGraphic);
			}

			protected override IControlGraphic InitializeTextControlGraphic(ITextGraphic textGraphic)
			{
				return new ControlGraphic(textGraphic);
			}
		}
	}
}