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

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers
{
	internal class RectangleGraphicAnnotationSerializer : GraphicAnnotationSerializer<IBoundableGraphic>
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
				annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Polyline;
				annotationElement.NumberOfGraphicPoints = 5;

				RectangleF bounds = graphic.BoundingBox;

				// add shape vertices
				PointF[] list = new PointF[5];
				list[0] = bounds.Location;
				list[1] = bounds.Location + new SizeF(bounds.Width, 0);
				list[2] = bounds.Location + bounds.Size;
				list[3] = bounds.Location + new SizeF(0, bounds.Height);
				list[4] = bounds.Location;
				annotationElement.GraphicData = list;

				annotationElement.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;
			}
			finally
			{
				graphic.ResetCoordinateSystem();
			}

			serializationState.AppendGraphicObjectSequence(annotationElement);
		}
	}
}