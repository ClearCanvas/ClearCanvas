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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Graphics3D;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class DFOVAnnotationItem : AnnotationItem
	{
		public DFOVAnnotationItem()
			: base("Presentation.DFOV", new AnnotationResourceResolver(typeof (DFOVAnnotationItem).Assembly)) {}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return String.Empty;

			IImageSopProvider imageSopProvider = presentationImage as IImageSopProvider;
			if (imageSopProvider == null)
				return String.Empty;

			ISpatialTransformProvider spatialTransformProvider = presentationImage as ISpatialTransformProvider;
			if (spatialTransformProvider == null)
				return String.Empty;

			// 2D DFOV value doesn't make a lot of sense when the "image" is 3D, so we're blanking it out until we define what DFOV means in this context
			if (presentationImage is ISpatialTransform3DProvider)
				return String.Empty;

			IImageSpatialTransform transform = spatialTransformProvider.SpatialTransform as IImageSpatialTransform;
			if (transform == null)
				return String.Empty;

			if (transform.RotationXY%90 != 0)
				return SR.ValueNotApplicable;

			Frame frame = imageSopProvider.Frame;
			PixelSpacing normalizedPixelSpacing = frame.NormalizedPixelSpacing;
			if (normalizedPixelSpacing.IsNull)
				return String.Empty;

			RectangleF sourceRectangle = new RectangleF(0, 0, frame.Columns, frame.Rows);
			RectangleF destinationRectangle = transform.ConvertToDestination(sourceRectangle);
			destinationRectangle = RectangleUtilities.Intersect(destinationRectangle, presentationImage.ClientRectangle);

			//Convert the displayed width and height to source dimensions
			SizeF widthInSource = transform.ConvertToSource(new SizeF(destinationRectangle.Width, 0));
			SizeF heightInSource = transform.ConvertToSource(new SizeF(0, destinationRectangle.Height));

			//The displayed FOV is given by the magnitude of each line in source coordinates, but
			//for each of the 2 lines, one of x or y will be zero, so we can optimize.

			float x1 = Math.Abs(widthInSource.Width);
			float y1 = Math.Abs(widthInSource.Height);
			float x2 = Math.Abs(heightInSource.Width);
			float y2 = Math.Abs(heightInSource.Height);

			double displayedFieldOfViewX, displayedFieldOfViewY;

			if (x1 > y1) //the image is not rotated
			{
				displayedFieldOfViewX = x1*normalizedPixelSpacing.Column/10;
				displayedFieldOfViewY = y2*normalizedPixelSpacing.Row/10;
			}
			else //the image is rotated by 90 or 270 degrees
			{
				displayedFieldOfViewX = x2*normalizedPixelSpacing.Column/10;
				displayedFieldOfViewY = y1*normalizedPixelSpacing.Row/10;
			}

			return String.Format(SR.FormatCentimeters, String.Format(SR.Format2Dimensions, displayedFieldOfViewX.ToString("F1"), displayedFieldOfViewY.ToString("F1")));
		}
	}
}