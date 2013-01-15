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

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (DicomEllipseGraphicAnnotationSerializer))]
	internal class DicomEllipseGraphic : CompositeGraphic, IVectorGraphic
	{
		private const int _majorAxisPoint1 = 0;
		private const int _majorAxisPoint2 = 1;
		private const int _minorAxisPoint1 = 2;
		private const int _minorAxisPoint2 = 3;
		private readonly PointsList _points;
		private EllipsePrimitive _ellipse;

		public DicomEllipseGraphic()
		{
			base.Graphics.Add(_ellipse = new EllipsePrimitive());
			_points = new PointsList(this);
			_points.Add(PointF.Empty);
			_points.Add(PointF.Empty);
			_points.Add(PointF.Empty);
			_points.Add(PointF.Empty);
			_points.PointChanged += OnPointChanged;
		}

		protected DicomEllipseGraphic(DicomEllipseGraphic source, ICloningContext context) : base()
		{
			context.CloneFields(source, this);
			_points = new PointsList(source._points, this);
			_points.PointChanged += OnPointChanged;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_ellipse = CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic test) { return test is EllipsePrimitive; }) as EllipsePrimitive;
			Platform.CheckForNullReference(_ellipse, "Cloned Ellipse");
		}

		public bool IsRegular
		{
			get
			{
				return FloatComparer.AreEqual((float) Vector.Distance(_points[_majorAxisPoint1], _points[_majorAxisPoint2]),
				                              (float) Vector.Distance(_points[_minorAxisPoint1], _points[_minorAxisPoint2]));
			}
		}

		public Color Color
		{
			get { return _ellipse.Color; }
			set { _ellipse.Color = value; }
		}

		public LineStyle LineStyle
		{
			get { return _ellipse.LineStyle; }
			set { _ellipse.LineStyle = value; }
		}

		public PointF MajorAxisPoint1
		{
			get { return _points[_majorAxisPoint1]; }
			set { _points[_majorAxisPoint1] = value; }
		}

		public PointF MajorAxisPoint2
		{
			get { return _points[_majorAxisPoint2]; }
			set { _points[_majorAxisPoint2] = value; }
		}

		public PointF MinorAxisPoint1
		{
			get { return _points[_minorAxisPoint1]; }
			set { _points[_minorAxisPoint1] = value; }
		}

		public PointF MinorAxisPoint2
		{
			get { return _points[_minorAxisPoint2]; }
			set { _points[_minorAxisPoint2] = value; }
		}

		private void OnPointChanged(object sender, IndexEventArgs e)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				PointF centroid = Vector.Midpoint(this.MajorAxisPoint1, this.MajorAxisPoint2);
				SizeF radial = new SizeF((float) (Vector.Distance(MajorAxisPoint1, MajorAxisPoint2)/2), (float) (Vector.Distance(MinorAxisPoint1, MinorAxisPoint2)/2));
				_ellipse.TopLeft = centroid - radial;
				_ellipse.BottomRight = centroid + radial;
				_ellipse.SpatialTransform.CenterOfRotationXY = centroid;
				_ellipse.SpatialTransform.RotationXY = (int) Vector.SubtendedAngle(MajorAxisPoint1, MajorAxisPoint2, MajorAxisPoint2 + new SizeF(1, 0))%180;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}
	}

	internal class DicomEllipseGraphicAnnotationSerializer : GraphicAnnotationSerializer<DicomEllipseGraphic>
	{
		protected override void Serialize(DicomEllipseGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			if (!graphic.Visible)
				return; // if the graphic is not visible, don't serialize it!

			GraphicAnnotationSequenceItem.GraphicObjectSequenceItem annotationElement = new GraphicAnnotationSequenceItem.GraphicObjectSequenceItem();

			graphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				annotationElement.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
				annotationElement.GraphicDimensions = 2;
				annotationElement.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;

				if (graphic.IsRegular) // check if graphic is a circle
				{
					annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Circle;
					annotationElement.NumberOfGraphicPoints = 2;

					PointF[] list = new PointF[2];
					list[0] = Vector.Midpoint(graphic.MajorAxisPoint1, graphic.MajorAxisPoint2); // centre of circle
					list[1] = graphic.MajorAxisPoint1; // any point on the circle
					annotationElement.GraphicData = list;
				}
				else
				{
					annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Ellipse;
					annotationElement.NumberOfGraphicPoints = 4;

					PointF[] list = new PointF[4];
					list[0] = graphic.MajorAxisPoint1; // left point of major axis
					list[1] = graphic.MajorAxisPoint2; // right point of major axis
					list[2] = graphic.MinorAxisPoint1; // top point of minor axis
					list[3] = graphic.MinorAxisPoint2; // bottom point of minor axis
					annotationElement.GraphicData = list;
				}
			}
			finally
			{
				graphic.ResetCoordinateSystem();
			}

			serializationState.AppendGraphicObjectSequence(annotationElement);
		}
	}
}