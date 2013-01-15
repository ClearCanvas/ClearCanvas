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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using GraphicObject = ClearCanvas.Dicom.Iod.Sequences.GraphicAnnotationSequenceItem.GraphicObjectSequenceItem;
using TextObject = ClearCanvas.Dicom.Iod.Sequences.GraphicAnnotationSequenceItem.TextObjectSequenceItem;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	partial class ShowAnglesTool
	{
		[DicomSerializableGraphicAnnotation(typeof (CompositeGraphicSerializer))]
		partial class ShowAnglesToolCompositeGraphic {}

		[DicomSerializableGraphicAnnotation(typeof (ShowAnglesToolGraphicSerializer))]
		partial class ShowAnglesToolGraphic {}

		private class ShowAnglesToolGraphicSerializer : GraphicAnnotationSerializer<ShowAnglesToolGraphic>
		{
			protected override void Serialize(ShowAnglesToolGraphic showAnglesToolGraphic, GraphicAnnotationSequenceItem serializationState)
			{
				if (!showAnglesToolGraphic.Visible)
					return;

				foreach (IGraphic graphic in showAnglesToolGraphic.Graphics)
				{
					if (!graphic.Visible)
						continue;

					if (graphic is ILineSegmentGraphic)
						SerializeDashedLine((ILineSegmentGraphic) graphic, serializationState);
					else if (graphic is ICalloutGraphic)
						SerializeCallout((ICalloutGraphic) graphic, serializationState);
				}
			}

			private static void SerializeDashedLine(ILineSegmentGraphic lineSegmentGraphic, GraphicAnnotationSequenceItem serializationState)
			{
				lineSegmentGraphic.CoordinateSystem = CoordinateSystem.Source;
				try
				{
					SerializeDashedLine(lineSegmentGraphic.Point1, lineSegmentGraphic.Point2, lineSegmentGraphic.SpatialTransform, serializationState, true);
				}
				finally
				{
					lineSegmentGraphic.ResetCoordinateSystem();
				}
			}

			private static void SerializeDashedLine(PointF point1, PointF point2, SpatialTransform spatialTransform, GraphicAnnotationSequenceItem serializationState, bool showHashes)
			{
				// these control parameters are in screen pixels at the nominal presentation zoom level
				const float period = 8;
				const float amplitude = 0.5f;

				SizeF normalVector;
				SizeF dashVector;
				float periods;

				// compute the dash vector and cross hash vector to be sized relative to screen pixels at nominal presentation zoom level
				{
					PointF dstLineVector = spatialTransform.ConvertToDestination(new SizeF(point2) - new SizeF(point1)).ToPointF();
					float dstMagnitude = (float) Math.Sqrt(dstLineVector.X*dstLineVector.X + dstLineVector.Y*dstLineVector.Y);
					periods = dstMagnitude/period;
					dstLineVector.X /= dstMagnitude;
					dstLineVector.Y /= dstMagnitude;
					dashVector = spatialTransform.ConvertToSource(new SizeF(dstLineVector.X*period/2, dstLineVector.Y*period/2));
					normalVector = spatialTransform.ConvertToSource(new SizeF(-dstLineVector.Y*amplitude, dstLineVector.X*amplitude));
				}

				PointF start = point1;
				int limit = (int) periods;
				for (int n = 0; n < limit; n++)
				{
					PointF midPeriod = start + dashVector;
					start = midPeriod + dashVector;

					GraphicObject dash = new GraphicObject();
					dash.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
					dash.GraphicData = new PointF[] {midPeriod, start};
					dash.GraphicDimensions = 2;
					dash.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;
					dash.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Polyline;
					dash.NumberOfGraphicPoints = 2;
					serializationState.AppendGraphicObjectSequence(dash);
				}

				// the first half of each period has no line, so this is only necessary if the residual is over half a period
				if (periods - limit > 0.5f)
				{
					GraphicObject dash = new GraphicObject();
					dash.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
					dash.GraphicData = new PointF[] {start + dashVector, point2};
					dash.GraphicDimensions = 2;
					dash.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;
					dash.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Polyline;
					dash.NumberOfGraphicPoints = 2;
					serializationState.AppendGraphicObjectSequence(dash);
				}

				if (showHashes)
				{
					GraphicObject hash1 = new GraphicObject();
					hash1.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
					hash1.GraphicData = new PointF[] {point1 - normalVector, point1 + normalVector};
					hash1.GraphicDimensions = 2;
					hash1.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;
					hash1.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Polyline;
					hash1.NumberOfGraphicPoints = 2;
					serializationState.AppendGraphicObjectSequence(hash1);

					GraphicObject hash2 = new GraphicObject();
					hash2.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
					hash2.GraphicData = new PointF[] {point2 - normalVector, point2 + normalVector};
					hash2.GraphicDimensions = 2;
					hash2.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;
					hash2.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Polyline;
					hash2.NumberOfGraphicPoints = 2;
					serializationState.AppendGraphicObjectSequence(hash2);
				}
			}

			private static void SerializeCallout(ICalloutGraphic calloutGraphic, GraphicAnnotationSequenceItem serializationState)
			{
				calloutGraphic.CoordinateSystem = CoordinateSystem.Source;
				try
				{
					RectangleF textBoundingBox = RectangleUtilities.ConvertToPositiveRectangle(calloutGraphic.TextBoundingBox);

					TextObject text = new TextObject();
					text.BoundingBoxAnnotationUnits = GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Pixel;
					text.BoundingBoxBottomRightHandCorner = new PointF(textBoundingBox.Right, textBoundingBox.Bottom);
					text.BoundingBoxTextHorizontalJustification = GraphicAnnotationSequenceItem.BoundingBoxTextHorizontalJustification.Center;
					text.BoundingBoxTopLeftHandCorner = textBoundingBox.Location;
					text.UnformattedTextValue = calloutGraphic.Text;
					serializationState.AppendTextObjectSequence(text);

					// draw the callout line manually instead of anchoring the text,
					// since we do not want the text to be moveable as permitted if we have an anchor point
					SerializeDashedLine(calloutGraphic.TextLocation, calloutGraphic.AnchorPoint, calloutGraphic.SpatialTransform, serializationState, false);
				}
				finally
				{
					calloutGraphic.ResetCoordinateSystem();
				}
			}
		}

		private class CompositeGraphicSerializer : GraphicAnnotationSerializer<CompositeGraphic>
		{
			protected override void Serialize(CompositeGraphic graphic, GraphicAnnotationSequenceItem serializationState)
			{
				if (!graphic.Visible)
					return;

				foreach (IGraphic subgraphic in graphic.Graphics)
					SerializeGraphic(subgraphic, serializationState);
			}
		}
	}
}