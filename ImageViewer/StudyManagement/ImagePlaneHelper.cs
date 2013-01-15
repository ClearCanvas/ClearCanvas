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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Contains helper methods for converting between image coordinates
	/// and the patient coordinate system defined by the Dicom Image Plane Module.
	/// </summary>
	public class ImagePlaneHelper
	{
		// No sense recalculating these things since they never change.
		private Matrix _rotationMatrix;
		private Matrix _pixelToPatientTransform;
	    private Vector3D _normalVector;

        internal ImagePlaneHelper(Frame frame)
            : this(frame.ImagePositionPatient, frame.ImageOrientationPatient, frame.PixelSpacing)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
	    public ImagePlaneHelper(ImagePositionPatient imagePositionPatient, ImageOrientationPatient imageOrientationPatient, PixelSpacing pixelSpacing)
		{
            Platform.CheckForNullReference(imagePositionPatient, "imagePositionPatient");
            Platform.CheckForNullReference(imageOrientationPatient, "imageOrientationPatient");
            Platform.CheckForNullReference(pixelSpacing, "pixelSpacing");

            PixelSpacing = pixelSpacing;
		    ImageOrientationPatient = imageOrientationPatient;
	        ImagePositionPatient = imagePositionPatient;
            ImagePositionPatientVector = new Vector3D((float)ImagePositionPatient.X, (float)ImagePositionPatient.Y, (float)ImagePositionPatient.Z);
		}

        public ImagePositionPatient ImagePositionPatient { get; private set; }
        public ImageOrientationPatient ImageOrientationPatient { get; private set; }
        public PixelSpacing PixelSpacing { get; private set; }
        public Vector3D ImagePositionPatientVector { get; private set; }

        /// <summary>
        /// Gets whether or not the patient orientation and pixel spacing values are valid and
        /// can therefore be used to transform between patient and image coordinates.
        /// </summary>
	    public bool IsValid
	    {
	        get
	        {
	            if (ImageOrientationPatient.IsNull)
                    return false;
                
                if (PixelSpacing.IsNull)
                    return false;

	            var normal = GetNormalVector();
	            return normal != null && !normal.IsNull;
	        }
	    }

        /// <summary>
		/// Converts the input image position (expressed in pixels) to the patient coordinate system.
		/// </summary>
		/// <returns>A position vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public Vector3D ConvertToPatient(PointF positionPixels)
		{
            if (ImageOrientationPatient.IsNull || PixelSpacing.IsNull)
				return null;

			// A shortcut for when the pixel position is (0, 0).
            if (positionPixels.X == 0F && positionPixels.Y == 0F)
                return ImagePositionPatientVector;

			// Calculation of position in patient coordinates using 
			// the matrix method described in Dicom PS 3.3 C.7.6.2.1.1.

			if (_pixelToPatientTransform == null)
			{
				_pixelToPatientTransform = new Matrix(4, 4);

                _pixelToPatientTransform.SetColumn(0, (float)(ImageOrientationPatient.RowX * PixelSpacing.Column),
                                     (float)(ImageOrientationPatient.RowY * PixelSpacing.Column),
                                     (float)(ImageOrientationPatient.RowZ * PixelSpacing.Column), 0F);

                _pixelToPatientTransform.SetColumn(1, (float)(ImageOrientationPatient.ColumnX * PixelSpacing.Row),
                                     (float)(ImageOrientationPatient.ColumnY * PixelSpacing.Row),
                                     (float)(ImageOrientationPatient.ColumnZ * PixelSpacing.Row), 0F);

                _pixelToPatientTransform.SetColumn(3, ImagePositionPatientVector.X, ImagePositionPatientVector.Y, ImagePositionPatientVector.Z, 1F);
			}

			Matrix columnMatrix = new Matrix(4, 1);
			columnMatrix.SetColumn(0, positionPixels.X, positionPixels.Y, 0F, 1F);
			Matrix result = _pixelToPatientTransform * columnMatrix;

			return new Vector3D(result[0, 0], result[1, 0], result[2, 0]);
		}

		/// <summary>
		/// Converts the input <paramref name="positionPatient">position vector</paramref> to the coordinate
		/// system of the image plane, moving the origin to <paramref name="originPatient"/>.
		/// </summary>
		/// <remarks>
		/// Note that the resultant position vector remains in units of mm and the z-coordinate is valid.
		/// </remarks>
		/// <param name="positionPatient">The position vector, in patient coordinates,
		/// to be converted to the coordinate system of the image plane.</param>
		/// <param name="originPatient">The new origin, in patient coordinates.</param>
		/// <returns>A position vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public Vector3D ConvertToImagePlane(Vector3D positionPatient, Vector3D originPatient)
		{
			Platform.CheckForNullReference(positionPatient, "positionPatient");

			Vector3D translated = positionPatient;
			if (originPatient != null)
				translated -= originPatient;

			Matrix rotationMatrix = GetRotationMatrix();
			if (rotationMatrix == null)
				return null;

			Matrix translatedMatrix = new Matrix(3, 1);
			translatedMatrix.SetColumn(0, translated.X, translated.Y, translated.Z);

			// Rotate coordinate system to match that of the image plane.
			Matrix rotated = rotationMatrix * translatedMatrix;
			return new Vector3D(rotated[0, 0], rotated[1, 0], rotated[2, 0]);
		}

		/// <summary>
		/// Converts the input <paramref name="positionPatient">position vector</paramref> to the coordinate
		/// system of the image plane, moving the origin to the top left corner of the image.
		/// </summary>
		/// <remarks>
		/// Note that the resultant position vector remains in units of mm and the z-coordinate is valid.
		/// </remarks>
		/// <param name="positionPatient">The position vector, in patient coordinates,
		/// to be converted to the coordinate system of the image plane.</param>
		/// <returns>A position vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public Vector3D ConvertToImagePlane(Vector3D positionPatient)
		{
			Platform.CheckForNullReference(positionPatient, "positionPatient");
            return ConvertToImagePlane(positionPatient, this.ImagePositionPatientVector);
		}

		/// <summary>
		/// Converts a point in the image plane expressed in millimetres (mm) into a point expressed in pixels.
		/// </summary>
		/// <returns>The corresponding pixel coordinate, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public PointF? ConvertToImage(PointF positionMillimetres)
		{
			if (PixelSpacing.IsNull)
				return null;

            return new PointF(positionMillimetres.X / (float)PixelSpacing.Column, positionMillimetres.Y / (float)PixelSpacing.Row);
		}

        /// <summary>
        /// Converts a point in the image expressed in pixels to a point expressed in mm.
        /// </summary>
        public PointF? ConvertFromImage(PointF positionPixels)
        {
            if (PixelSpacing.IsNull)
                return null;

            return new PointF(positionPixels.X * (float)PixelSpacing.Column, positionPixels.Y * (float)PixelSpacing.Row);
        }

        /// <summary>
        /// Gets the normal vector describing the plane of the image in patient coordinates.
        /// </summary>
        /// <returns>The normal vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
        public Vector3D GetNormalVector()
        {
            if (_normalVector == null)
                _normalVector = GetNormalVector(ImageOrientationPatient);
            return _normalVector;
        }

        /// <summary>
        /// Gets the normal vector describing the plane of the image in patient coordinates.
        /// </summary>
        /// <returns>The normal vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
        public static Vector3D GetNormalVector(ImageOrientationPatient imageOrientationPatient)
		{
            if (imageOrientationPatient.IsNull)
				return null;

            var left = new Vector3D((float)imageOrientationPatient.RowX, (float)imageOrientationPatient.RowY, (float)imageOrientationPatient.RowZ);
            var normal = left.Cross(new Vector3D((float)imageOrientationPatient.ColumnX, (float)imageOrientationPatient.ColumnY, (float)imageOrientationPatient.ColumnZ));
            return FloatComparer.AreEqual(normal.Magnitude, 0) ? Vector3D.Null : normal.Normalize();
		}

		/// <summary>
		/// Gets a rotation matrix that, when multiplied by a column matrix representing a
		/// position vector in patient coordinates, will rotate the position vector
		/// into a coordinate system matching that of the image plane.
		/// </summary>
		/// <returns>The rotation matrix, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		private Matrix GetRotationMatrix()
		{
			if (_rotationMatrix == null)
			{
                if (ImageOrientationPatient.IsNull)
					return null;

				Vector3D normal = GetNormalVector();
                if (normal == null || normal.IsNull)
                    return null;

				_rotationMatrix = new Matrix(3, 3);
                _rotationMatrix.SetRow(0, (float)ImageOrientationPatient.RowX, (float)ImageOrientationPatient.RowY, (float)ImageOrientationPatient.RowZ);
                _rotationMatrix.SetRow(1, (float)ImageOrientationPatient.ColumnX, (float)ImageOrientationPatient.ColumnY, (float)ImageOrientationPatient.ColumnZ);
				_rotationMatrix.SetRow(2, normal.X, normal.Y, normal.Z);
			}

			return _rotationMatrix;
		}
	}
}
