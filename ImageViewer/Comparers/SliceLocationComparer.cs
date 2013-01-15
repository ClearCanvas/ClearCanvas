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
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Frame"/>s based on slice location.
	/// </summary>
	public class SliceLocationComparer : DicomFrameComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SliceLocationComparer"/>.
		/// </summary>
		public SliceLocationComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="SliceLocationComparer"/>.
		/// </summary>
		public SliceLocationComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(Frame frame)
		{
            //Group be common study level attributes
            yield return frame.StudyInstanceUid;

            //Group by common series level attributes
            //This sorts "FOR PRESENTATION" images to the beginning (except in reverse, of course).
            yield return frame.ParentImageSop.PresentationIntentType == "FOR PRESENTATION" ? 0 : 1;
            yield return frame.ParentImageSop.SeriesNumber;
            yield return frame.ParentImageSop.SeriesDescription;
            yield return frame.SeriesInstanceUid;

			yield return frame.FrameOfReferenceUid;

			double? normalX = null, normalY = null, normalZ = null;
			double? zImagePlane = null;

			Vector3D normal = frame.ImagePlaneHelper.GetNormalVector();
			if (normal != null)
			{
				// Return the 3 components of the image normal; if they are all equal
				// then the images are in the same plane.  We are disregarding
				// the rare case where the 2 normals being compared are the negative
				// of each other - technically, they could be considered to be in the
				// same 'plane', but for the purposes of sorting, we won't consider it.
				normalX = Math.Round(normal.X, 3, MidpointRounding.AwayFromZero);
				normalY = Math.Round(normal.Y, 3, MidpointRounding.AwayFromZero);
				normalZ = Math.Round(normal.Z, 3, MidpointRounding.AwayFromZero);
				
				Vector3D positionPatient = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1) / 2F, (frame.Rows - 1) / 2F));
				if (positionPatient != null)
				{
					Vector3D positionImagePlane = frame.ImagePlaneHelper.ConvertToImagePlane(positionPatient, Vector3D.Null);

					//return only the z-component of the image plane position (where the origin remains at the patient origin).
					zImagePlane = Math.Round(positionImagePlane.Z, 3, MidpointRounding.AwayFromZero);
				}
			}

			yield return normalX;
			yield return normalY;
			yield return normalZ;
			yield return zImagePlane;

			//as a last resort.
			yield return frame.ParentImageSop.InstanceNumber;
			yield return frame.FrameNumber;
			yield return frame.AcquisitionNumber;
		}

		/// <summary>
		/// Compares two <see cref="Frame"/>s based on slice location.
		/// </summary>
		public override int Compare(Frame x, Frame y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}
