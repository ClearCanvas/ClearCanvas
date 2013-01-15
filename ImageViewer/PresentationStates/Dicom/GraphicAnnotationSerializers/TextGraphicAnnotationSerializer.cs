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
	internal class TextGraphicAnnotationSerializer : GraphicAnnotationSerializer<ITextGraphic>
	{
		protected override void Serialize(ITextGraphic textGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			// if the callout is not visible, don't serialize it!
			if (!textGraphic.Visible)
				return;

			if (string.IsNullOrEmpty(textGraphic.Text))
				return;

			GraphicAnnotationSequenceItem.TextObjectSequenceItem text = new GraphicAnnotationSequenceItem.TextObjectSequenceItem();

			textGraphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(textGraphic.BoundingBox);
				text.BoundingBoxAnnotationUnits = GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Pixel;
				text.BoundingBoxTextHorizontalJustification = GraphicAnnotationSequenceItem.BoundingBoxTextHorizontalJustification.Left;
				text.BoundingBoxTopLeftHandCorner = boundingBox.Location;
				text.BoundingBoxBottomRightHandCorner = boundingBox.Location + boundingBox.Size;
				text.UnformattedTextValue = textGraphic.Text;
			}
			finally
			{
				textGraphic.ResetCoordinateSystem();
			}

			serializationState.AppendTextObjectSequence(text);
		}
	}
}