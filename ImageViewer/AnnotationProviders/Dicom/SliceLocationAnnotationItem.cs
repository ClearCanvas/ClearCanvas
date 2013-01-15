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
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class SliceLocationAnnotationItem : AnnotationItem
	{
		public SliceLocationAnnotationItem()
			: base("Dicom.ImagePlane.SliceLocation", new AnnotationResourceResolver(typeof(SliceLocationAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider) presentationImage).Frame;
				Vector3D normal = frame.ImagePlaneHelper.GetNormalVector();
				Vector3D positionCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1) / 2F, (frame.Rows - 1) / 2F));

				if (normal != null && positionCenterOfImage != null)
				{
					// Try to be a bit more specific when we have spatial information
					// by showing directional information (L, R, H, F, A, P) as well as
					// the slice location.
					float absX = Math.Abs(normal.X);
					float absY = Math.Abs(normal.Y);
					float absZ = Math.Abs(normal.Z);

					// Get the primary direction based on the largest component of the normal.
					if (absZ >= absY && absZ >= absX)
					{
						//mostly axial because Z >= X and Y
						string directionString = (positionCenterOfImage.Z >= 0F) ? SR.ValueDirectionalMarkersHead : SR.ValueDirectionalMarkersFoot;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.Z));
					}
					else if (absY >= absX && absY >= absZ)
					{
						//mostly coronal because Y >= X and Z
						string directionString = (positionCenterOfImage.Y >= 0F) ? SR.ValueDirectionalMarkersPosterior : SR.ValueDirectionalMarkersAnterior;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.Y));
					}
					else
					{
						//mostly sagittal because X >= Y and Z
						string directionString = (positionCenterOfImage.X >= 0F) ? SR.ValueDirectionalMarkersLeft : SR.ValueDirectionalMarkersRight;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.X));
					}
				}
			}

			return "";
		}
	}
}
