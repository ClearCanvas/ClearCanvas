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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers
{
	internal class EllipseGraphicAnnotationSerializer : GraphicAnnotationSerializer<IBoundableGraphic>
	{
		protected override void Serialize(IBoundableGraphic graphic, GraphicAnnotationSequenceItem serializationState)
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

				SizeF halfDims = new SizeF(graphic.Width/2, graphic.Height/2);
				if (FloatComparer.AreEqual(graphic.Width, graphic.Height)) // check if graphic is a circle
				{
					annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Circle;
					annotationElement.NumberOfGraphicPoints = 2;

					PointF[] list = new PointF[2];
					list[0] = graphic.TopLeft + halfDims; // centre of circle
					list[1] = graphic.TopLeft + new SizeF(0, halfDims.Height); // any point on the circle
					annotationElement.GraphicData = list;
				}
				else
				{
					annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Ellipse;
					annotationElement.NumberOfGraphicPoints = 4;

					int offset = graphic.Width < graphic.Height ? 2 : 0; // offset list by 2 if major axis is vertical
					PointF[] list = new PointF[4];
					list[(offset + 0)%4] = graphic.TopLeft + new SizeF(0, halfDims.Height); // left point of horizontal axis
					list[(offset + 1)%4] = graphic.TopLeft + new SizeF(graphic.Width, halfDims.Height); // right point of horizontal axis
					list[(offset + 2)%4] = graphic.TopLeft + new SizeF(halfDims.Width, 0); // top point of vertical axis
					list[(offset + 3)%4] = graphic.TopLeft + new SizeF(halfDims.Width, graphic.Height); // bottom point of vertical axis
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