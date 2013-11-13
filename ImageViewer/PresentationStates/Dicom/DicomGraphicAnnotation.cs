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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// An <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/> whose contents represent those of a DICOM Graphic Annotation Sequence (PS 3.3 C.10.5).
	/// </summary>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (DicomGraphicAnnotationSerializer))]
	public partial class DicomGraphicAnnotation : StandardStatefulGraphic, IContextMenuProvider
	{
		[CloneIgnore]
		private ToolSet _toolSet;

		[CloneIgnore]
		private bool _interactive = false;

		/// <summary>
		/// Constructs a new <see cref="IGraphic"/> whose contents are constructed based on a <see cref="GraphicAnnotationSequenceItem">DICOM Graphic Annotation Sequence Item</see>.
		/// </summary>
		/// <param name="graphicAnnotationSequenceItem">The DICOM graphic annotation sequence item to render.</param>
		/// <param name="displayedArea">The image's displayed area with which to </param>
		public static DicomGraphicAnnotation Create(GraphicAnnotationSequenceItem graphicAnnotationSequenceItem, RectangleF displayedArea)
		{
			var subjectGraphic = new SubjectGraphic();
			var dataPoints = new List<PointF>();
			if (graphicAnnotationSequenceItem.GraphicObjectSequence != null)
			{
				foreach (var graphicItem in graphicAnnotationSequenceItem.GraphicObjectSequence)
				{
					try
					{
						var points = GetGraphicDataAsSourceCoordinates(displayedArea, graphicItem);
						var graphic = CreateGraphic(graphicItem.GraphicType, points, true);
						if (graphic != null) subjectGraphic.Graphics.Add(new ElementGraphic(graphic));
						dataPoints.AddRange(points);
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Warn, ex, "DICOM Softcopy Presentation State Deserialization Fault (Graphic Object Type {0}). Reprocess with log level DEBUG to see DICOM data dump.", graphicItem.GraphicType);
						Platform.Log(LogLevel.Debug, graphicItem.DicomSequenceItem.Dump());
					}
				}
			}

			var annotations = new List<IGraphic>();
			var annotationBounds = RectangleF.Empty;
			if (dataPoints.Count > 0) annotationBounds = RectangleUtilities.ComputeBoundingRectangle(dataPoints.ToArray());
			if (graphicAnnotationSequenceItem.TextObjectSequence != null)
			{
				foreach (var textItem in graphicAnnotationSequenceItem.TextObjectSequence)
				{
					try
					{
						annotations.Add(CreateCalloutText(annotationBounds, displayedArea, textItem));
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Warn, ex, "DICOM Softcopy Presentation State Deserialization Fault (Text Object). Reprocess with log level DEBUG to see DICOM data dump.");
						Platform.Log(LogLevel.Debug, textItem.DicomSequenceItem.Dump());
					}
				}
			}

			var calloutGraphic = annotations.FirstOrDefault() as ICalloutGraphic;
			if (subjectGraphic.Graphics.Count == 1 && annotations.Count == 1 && calloutGraphic != null)
			{
				var subjectElement = (ElementGraphic) subjectGraphic.Graphics.Single();
				subjectElement.Graphics.Add(calloutGraphic);
				subjectElement.Callout = calloutGraphic;
			}
			else
			{
				subjectGraphic.Graphics.AddRange(annotations.Where(g => !(g is ICalloutGraphic)).Select(g => new TextEditControlGraphic(g)));
				subjectGraphic.Graphics.AddRange(annotations.OfType<ICalloutGraphic>().Select(g => new UserCalloutGraphic
				                                                                                   	{
				                                                                                   		AnchorPoint = g.AnchorPoint,
				                                                                                   		TextLocation = g.TextLocation,
				                                                                                   		Text = g.Text,
				                                                                                   		ShowArrowhead = !(g is CalloutGraphic) || ((CalloutGraphic) g).ShowArrowhead
				                                                                                   	}));
			}

			subjectGraphic.SetEnabled(false);
			subjectGraphic.SetColor(Color.LemonChiffon);
			return new DicomGraphicAnnotation(subjectGraphic);
		}

		private DicomGraphicAnnotation(IGraphic subjectGraphic)
			: base(subjectGraphic)
		{
			InactiveColor = Color.LemonChiffon;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomGraphicAnnotation(DicomGraphicAnnotation source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public bool Interactive
		{
			get { return _interactive; }
			set
			{
				if (_interactive != value)
				{
					_interactive = value;
					OnInteractiveChanged();
				}
			}
		}

		protected virtual string ContextMenuNamespace
		{
			get { return typeof (DicomGraphicAnnotation).FullName; }
		}

		/// <summary>
		/// Gets or sets the <see cref="CompositeGraphic.CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// Setting the <see cref="CompositeGraphic.CoordinateSystem"/> property will recursively set the 
		/// <see cref="CompositeGraphic.CoordinateSystem"/> property for <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </remarks>
		public override sealed CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set { base.CoordinateSystem = value; }
		}

		/// <summary>
		/// Resets the <see cref="CompositeGraphic.CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="CompositeGraphic.ResetCoordinateSystem"/> will reset the <see cref="CompositeGraphic.CoordinateSystem"/>
		/// to what it was before the <see cref="CompositeGraphic.CoordinateSystem"/> was last set.
		/// </para>
		/// <para>
		/// Calling <see cref="CompositeGraphic.ResetCoordinateSystem"/> will recursively call
		/// <see cref="CompositeGraphic.ResetCoordinateSystem"/> on <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </para>
		/// </remarks>
		public override sealed void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();
		}

		/// <summary>
		/// Called when the <see cref="Interactive"/> property changes.
		/// </summary>
		protected virtual void OnInteractiveChanged()
		{
			foreach (var graphic in Graphics.OfType<SubjectGraphic>())
			{
				graphic.SetEnabled(Interactive);
			}
		}

		protected override bool Start(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ActiveButton == XMouseButtons.Right)
			{
				CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					if (HitTest(mouseInformation.Location)) return true;
				}
				finally
				{
					ResetCoordinateSystem();
				}
			}
			return base.Start(mouseInformation);
		}

		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			const string actionSite = "dicomgraphic-menu";
			var actions = GetExportedActions(actionSite, mouseInformation);
			if (actions == null || actions.Count == 0)
				return null;
			return ActionModelRoot.CreateModel(ContextMenuNamespace, actionSite, actions);
		}

		public override IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			if (!HitTest(mouseInformation.Location))
				return new ActionSet();

			if (_toolSet == null)
				_toolSet = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(this));

			return base.GetExportedActions(site, mouseInformation).Union(_toolSet.Actions);
		}

		#region Static Graphic Creation Helpers

		private static IList<PointF> GetGraphicDataAsSourceCoordinates(RectangleF displayedArea, GraphicAnnotationSequenceItem.GraphicObjectSequenceItem graphicItem)
		{
			if (graphicItem.GraphicAnnotationUnits == GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Display)
			{
				return graphicItem.GraphicData.Select(point => GetPointInSourceCoordinates(displayedArea, point)).ToList().AsReadOnly();
			}
			else
			{
				return graphicItem.GraphicData.Select(point => new PointF(point.X, point.Y)).ToList().AsReadOnly();
			}
		}

		private static PointF GetPointInSourceCoordinates(RectangleF displayedArea, PointF point)
		{
			return displayedArea.Location + new SizeF(displayedArea.Width*point.X, displayedArea.Height*point.Y);
		}

		private static IGraphic CreateGraphic(GraphicAnnotationSequenceItem.GraphicType graphicType, IList<PointF> points, bool editable = false)
		{
			switch (graphicType)
			{
				case GraphicAnnotationSequenceItem.GraphicType.Interpolated:
					Platform.CheckTrue(points.Count >= 2, "Graphic Type INTERPOLATED requires at least 2 coordinates");
					return editable ? CreateInteractiveInterpolated(points) : CreateInterpolated(points);
				case GraphicAnnotationSequenceItem.GraphicType.Polyline:
					Platform.CheckTrue(points.Count >= 2, "Graphic Type POLYLINE requires at least 2 coordinates");
					return editable ? CreateInteractivePolyline(points) : CreatePolyline(points);
				case GraphicAnnotationSequenceItem.GraphicType.Point:
					Platform.CheckTrue(points.Count >= 1, "Graphic Type POINT requires 1 coordinate");
					return editable ? CreateInteractivePoint(points[0]) : CreatePoint(points[0]);
				case GraphicAnnotationSequenceItem.GraphicType.Circle:
					Platform.CheckTrue(points.Count >= 2, "Graphic Type CIRCLE requires 2 coordinates");
					return editable ? CreateInteractiveCircle(points[0], (float) Vector.Distance(points[0], points[1])) : CreateCircle(points[0], (float) Vector.Distance(points[0], points[1]));
				case GraphicAnnotationSequenceItem.GraphicType.Ellipse:
					Platform.CheckTrue(points.Count >= 4, "Graphic Type INTERPOLATED requires 4 coordinates");
					return editable ? CreateInteractiveEllipse(points[0], points[1], points[2], points[3]) : CreateEllipse(points[0], points[1], points[2], points[3]);
				default:
					Platform.Log(LogLevel.Debug, "Unrecognized Graphic Type");
					return null;
			}
		}

		private static IGraphic CreateInteractiveInterpolated(IList<PointF> dataPoints)
		{
			var closed = FloatComparer.AreEqual(dataPoints[0], dataPoints[dataPoints.Count - 1]);
			var curve = CreateInterpolated(dataPoints);
			return closed ? new PolygonControlGraphic(true, new MoveControlGraphic(curve)) : new VerticesControlGraphic(true, new MoveControlGraphic(curve));
		}

		private static IGraphic CreateInterpolated(IList<PointF> dataPoints)
		{
			var curve = new SplinePrimitive();
			curve.Points.AddRange(dataPoints);
			return curve;
		}

		private static IGraphic CreateInteractivePolyline(IList<PointF> vertices)
		{
			var closed = FloatComparer.AreEqual(vertices[0], vertices[vertices.Count - 1]);

			// use a standard rectangle primitive if the axes defines an axis-aligned rectangle
			if (closed && vertices.Count == 5 && IsAxisAligned(vertices[0], vertices[1]) && IsAxisAligned(vertices[1], vertices[2]) && IsAxisAligned(vertices[2], vertices[3]) && IsAxisAligned(vertices[3], vertices[4]))
			{
				var bounds = RectangleUtilities.ConvertToPositiveRectangle(RectangleUtilities.ComputeBoundingRectangle(vertices[0], vertices[1], vertices[2], vertices[3]));
				var rectangle = new RectanglePrimitive {TopLeft = bounds.Location, BottomRight = bounds.Location + bounds.Size};
				return new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(rectangle)));
			}
			else if (!closed && vertices.Count == 3)
			{
				var protractor = new ProtractorGraphic {Points = {vertices[0], vertices[1], vertices[2]}};
				return new VerticesControlGraphic(new MoveControlGraphic(protractor));
			}
			else if (!closed && vertices.Count == 2)
			{
				var line = new PolylineGraphic {Points = {vertices[0], vertices[1]}};
				return new VerticesControlGraphic(new MoveControlGraphic(line));
			}

			var polyline = new PolylineGraphic(closed);
			polyline.Points.AddRange(vertices);
			return closed ? new PolygonControlGraphic(true, new MoveControlGraphic(polyline)) : new VerticesControlGraphic(true, new MoveControlGraphic(polyline));
		}

		private static IGraphic CreatePolyline(IList<PointF> vertices)
		{
			var closed = FloatComparer.AreEqual(vertices[0], vertices[vertices.Count - 1]);
			var polyline = new PolylineGraphic(closed);
			polyline.Points.AddRange(vertices);
			return polyline;
		}

		private static IGraphic CreateInteractiveCircle(PointF center, float radius)
		{
			return new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(CreateCircle(center, radius))));
		}

		private static IGraphic CreateCircle(PointF center, float radius)
		{
			var circle = new EllipsePrimitive();
			var radial = new SizeF(radius, radius);
			circle.TopLeft = center - radial;
			circle.BottomRight = center + radial;
			return circle;
		}

		private static IGraphic CreateInteractivePoint(PointF location)
		{
			return new MoveControlGraphic(CreatePoint(location));
		}

		private static IGraphic CreatePoint(PointF location)
		{
			const float radius = 4;
			var point = new InvariantEllipsePrimitive();
			point.Location = location;
			point.InvariantTopLeft = new PointF(-radius, -radius);
			point.InvariantBottomRight = new PointF(radius, radius);
			return point;
		}

		private static IGraphic CreateInteractiveEllipse(PointF majorAxisEnd1, PointF majorAxisEnd2, PointF minorAxisEnd1, PointF minorAxisEnd2)
		{
			if (IsAxisAligned(majorAxisEnd1, majorAxisEnd2) && IsAxisAligned(minorAxisEnd1, minorAxisEnd2))
			{
				// use a standard ellipse primitive if the axes defines an axis-aligned ellipse
				var bounds = RectangleUtilities.ConvertToPositiveRectangle(RectangleUtilities.ComputeBoundingRectangle(majorAxisEnd1, majorAxisEnd2, minorAxisEnd1, minorAxisEnd2));
				var ellipse = new EllipsePrimitive {TopLeft = bounds.Location, BottomRight = bounds.Location + bounds.Size};
				return new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(ellipse)));
			}
			return new MoveControlGraphic(CreateEllipse(majorAxisEnd1, majorAxisEnd2, minorAxisEnd1, minorAxisEnd2));
		}

		private static IGraphic CreateEllipse(PointF majorAxisEnd1, PointF majorAxisEnd2, PointF minorAxisEnd1, PointF minorAxisEnd2)
		{
			var dicomEllipseGraphic = new DicomEllipseGraphic();
			dicomEllipseGraphic.MajorAxisPoint1 = majorAxisEnd1;
			dicomEllipseGraphic.MajorAxisPoint2 = majorAxisEnd2;
			dicomEllipseGraphic.MinorAxisPoint1 = minorAxisEnd1;
			dicomEllipseGraphic.MinorAxisPoint2 = minorAxisEnd2;
			return dicomEllipseGraphic;
		}

		private static IGraphic CreateCalloutText(RectangleF annotationBounds, RectangleF displayedArea, GraphicAnnotationSequenceItem.TextObjectSequenceItem textItem)
		{
			if (textItem.AnchorPoint.HasValue)
			{
				CalloutGraphic callout = new CalloutGraphic(textItem.UnformattedTextValue);

				PointF anchor = textItem.AnchorPoint.Value;
				if (textItem.AnchorPointAnnotationUnits == GraphicAnnotationSequenceItem.AnchorPointAnnotationUnits.Display)
					anchor = GetPointInSourceCoordinates(displayedArea, anchor);

				callout.AnchorPoint = anchor;
				callout.ShowArrowhead = annotationBounds.IsEmpty; // show arrowhead if graphic annotation bounds are empty

				if (textItem.BoundingBoxTopLeftHandCorner.HasValue && textItem.BoundingBoxBottomRightHandCorner.HasValue)
				{
					PointF topLeft = textItem.BoundingBoxTopLeftHandCorner.Value;
					PointF bottomRight = textItem.BoundingBoxBottomRightHandCorner.Value;

					if (textItem.BoundingBoxAnnotationUnits == GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Display)
					{
						topLeft = GetPointInSourceCoordinates(displayedArea, topLeft);
						bottomRight = GetPointInSourceCoordinates(displayedArea, bottomRight);
					}

					callout.TextLocation = Vector.Midpoint(topLeft, bottomRight);
				}
				else
				{
					if (!annotationBounds.IsEmpty)
						callout.TextLocation = annotationBounds.Location - new SizeF(30, 30);
					else
						callout.TextLocation = anchor - new SizeF(30, 30);
				}
				return callout;
			}
			else if (textItem.BoundingBoxTopLeftHandCorner.HasValue && textItem.BoundingBoxBottomRightHandCorner.HasValue)
			{
				InvariantTextPrimitive text = new InvariantTextPrimitive(textItem.UnformattedTextValue);
				PointF topLeft = textItem.BoundingBoxTopLeftHandCorner.Value;
				PointF bottomRight = textItem.BoundingBoxBottomRightHandCorner.Value;

				if (textItem.BoundingBoxAnnotationUnits == GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Display)
				{
					topLeft = GetPointInSourceCoordinates(displayedArea, topLeft);
					bottomRight = GetPointInSourceCoordinates(displayedArea, bottomRight);
				}

				// TODO: make the text variant - rotated as specified by bounding area - as well as justified as requested
				// RectangleF boundingBox = RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
				// boundingBox = RectangleUtilities.ConvertToPositiveRectangle(boundingBox);
				// boundingBox.Location = boundingBox.Location - new SizeF(1, 1);
				text.Location = Vector.Midpoint(topLeft, bottomRight);

				return new MoveControlGraphic(text);
			}
			else
			{
				throw new InvalidDataException("The GraphicAnnotationSequenceItem must define either an anchor point or a bounding box.");
			}
		}

		private static bool IsAxisAligned(PointF pt1, PointF pt2)
		{
			var v = pt2 - new SizeF(pt1);
			return FloatComparer.AreEqual(0, v.X) || FloatComparer.AreEqual(0, v.Y);
		}

		#endregion

		#region DicomGraphicAnnotationSerializer Class

		private class DicomGraphicAnnotationSerializer : GraphicAnnotationSerializer<CompositeGraphic>
		{
			protected override void Serialize(CompositeGraphic graphic, GraphicAnnotationSequenceItem serializationState)
			{
				foreach (IGraphic subgraphic in graphic.Graphics)
					SerializeGraphic(subgraphic, serializationState);
			}
		}

		#endregion
	}
}