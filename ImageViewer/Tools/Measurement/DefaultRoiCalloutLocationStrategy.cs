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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// A default implementation of <see cref="IAnnotationCalloutLocationStrategy"/>
	/// suitable for ROI measurements with a tight bounding box (i.e. not a line).
	/// </summary>
	/// <remarks>
	/// <para>
	/// This implementation uses the ROI graphic's <see cref="IGraphic.BoundingBox"/>
	/// compute a callout position that tries to minimize callout obstruction of the
	/// underlying anatomy while keeping the callout within the image tile.
	/// </para>
	/// <para>
	/// The auto computation is disabled if the user manually positions the callout.
	/// </para>
	/// </remarks>
	public class DefaultRoiCalloutLocationStrategy : AnnotationCalloutLocationStrategy
	{
		private bool _manuallyPositionedCallout = false;

		public override void OnCalloutLocationChangedExternally()
		{
			base.OnCalloutLocationChangedExternally();

			_manuallyPositionedCallout = true;
		}

		public override bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			// if the user has manually positioned the callout, we won't override it
			if (_manuallyPositionedCallout)
			{
				location = PointF.Empty;
				coordinateSystem = CoordinateSystem.Destination;
				return false;
			}

			Callout.CoordinateSystem = CoordinateSystem.Destination;
            AnnotationSubject.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
                var roiBoundingBox = AnnotationSubject.BoundingBox;
                var clientRectangle = AnnotationSubject.ParentPresentationImage.ClientRectangle;

				var textSize = Callout.TextBoundingBox.Size;
				if (textSize.IsEmpty)
					textSize = new SizeF(100, 50);

				coordinateSystem = CoordinateSystem.Destination;
				location = new PointF(ComputeCalloutLocationX(textSize, clientRectangle, roiBoundingBox), ComputeCalloutLocationY(textSize, clientRectangle, roiBoundingBox));
			}
			finally
			{
				Callout.ResetCoordinateSystem();
                AnnotationSubject.ResetCoordinateSystem();
			}
			return true;
		}

		private static float ComputeCalloutLocationY(SizeF textSize, RectangleF clientRectangle, RectangleF roiBoundingBox)
		{
			const float roiVOffset = 15;

			var roiY = roiBoundingBox.Top + roiBoundingBox.Height/2;
			var roiHalfHeight = roiBoundingBox.Height/2;
			var textHalfHeight = textSize.Height/2;

			//TODO (CR Sept 2010): can this be written more descriptively?
			// e.g. if (IsBeyondTopEdge(roiY)) MoveInsideTopEdge(RoiY);
			if (roiY < textSize.Height + roiHalfHeight + roiVOffset)
				return roiY + textHalfHeight + roiHalfHeight + roiVOffset;
			else if (roiY < clientRectangle.Height/2)
				return roiY - textHalfHeight - roiHalfHeight - roiVOffset;
			else if (roiY < clientRectangle.Height - textSize.Height - roiHalfHeight - roiVOffset)
				return roiY + textHalfHeight + roiHalfHeight + roiVOffset;
			else
				return roiY - textHalfHeight - roiHalfHeight - roiVOffset;
		}

		private static float ComputeCalloutLocationX(SizeF textSize, RectangleF clientRectangle, RectangleF roiBoundingBox)
		{
			var roiX = roiBoundingBox.Left + roiBoundingBox.Width/2;
			var roiHalfWidth = roiBoundingBox.Width/2;
			var textHalfWidth = textSize.Width/2;

			//TODO (CR Sept 2010): can this be written more descriptively?
			// e.g. if (IsBeyondLeftEdge(roiX)) MoveInsideLeftEdge(RoiX);
			if (roiX < -roiHalfWidth)
				return roiX + roiHalfWidth + textHalfWidth;
			else if (roiX < textHalfWidth)
				return textHalfWidth;
			else if (roiX < clientRectangle.Width - textHalfWidth)
				return roiX;
			else if (roiX < clientRectangle.Width + roiHalfWidth)
				return clientRectangle.Width - textHalfWidth;
			else
				return roiX - roiHalfWidth - textHalfWidth;
		}
	}
}