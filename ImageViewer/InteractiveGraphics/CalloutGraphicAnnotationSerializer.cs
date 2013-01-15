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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class CalloutGraphicAnnotationSerializer : GraphicAnnotationSerializer<ICalloutGraphic>
	{
		protected override void Serialize(ICalloutGraphic calloutGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			// if the callout is not visible, don't serialize it!
			if (!calloutGraphic.Visible)
				return;

			GraphicAnnotationSequenceItem.TextObjectSequenceItem text = new GraphicAnnotationSequenceItem.TextObjectSequenceItem();

			calloutGraphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(calloutGraphic.TextBoundingBox);
				text.BoundingBoxAnnotationUnits = GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Pixel;
				text.BoundingBoxTextHorizontalJustification = GraphicAnnotationSequenceItem.BoundingBoxTextHorizontalJustification.Left;
				text.BoundingBoxTopLeftHandCorner = boundingBox.Location;
				text.BoundingBoxBottomRightHandCorner = boundingBox.Location + boundingBox.Size;

				text.AnchorPoint = calloutGraphic.AnchorPoint;
				text.AnchorPointAnnotationUnits = GraphicAnnotationSequenceItem.AnchorPointAnnotationUnits.Pixel;
				text.AnchorPointVisibility = GraphicAnnotationSequenceItem.AnchorPointVisibility.Y;

				if (string.IsNullOrEmpty(calloutGraphic.Text))
					text.UnformattedTextValue = " ";
				else
					text.UnformattedTextValue = calloutGraphic.Text;
			}
			finally
			{
				calloutGraphic.ResetCoordinateSystem();
			}

			serializationState.AppendTextObjectSequence(text);
		}
	}
}