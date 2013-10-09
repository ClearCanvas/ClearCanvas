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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents the spatial plane of an image and its relationship with the patient coordinate system as defined by the frame's DICOM Image Plane Module.
	/// </summary>
	public sealed class ImagePlaneHelper
	{
		private const float _defaultToleranceRadians = 100*float.Epsilon;

		private readonly ImagePositionPatient _imagePositionPatient;
		private readonly ImageOrientationPatient _imageOrientationPatient;
		private readonly PixelSpacing _pixelSpacing;
		private readonly int _rows;
		private readonly int _columns;

		private Matrix3D _rotationMatrix;
		private Matrix _pixelToPatientTransform;
		private Vector3D _imageRowOrientationPatient;
		private Vector3D _imageColumnOrientationPatient;
		private Vector3D _imageNormalPatient;
		private Vector3D _imageTopLeftPatient;
		private Vector3D _imageTopRightPatient;
		private Vector3D _imageBottomLeftPatient;
		private Vector3D _imageBottomRightPatient;
		private Vector3D _imageCenterPatient;
		private Vector3D _topLeftImagePlane;

		/// <summary>
		/// Constructor (for use by <see cref="Frame"/> class only).
		/// </summary>
		internal ImagePlaneHelper(Frame frame)
			: this(frame.ImagePositionPatient, frame.ImageOrientationPatient, frame.PixelSpacing, frame.Rows, frame.Columns)
		{
			// this constructor is internal because it keeps references to the source frame's position, orientation and spacing properties
			// if the frame is changed or disposed, the calculations made by this instance would be affected
			// thus, only the Frame class should use this constructor, since it will thus have the same lifetime as the referenced objects
			// if you need to create an instance separated from a frame, use the other constructor with appropriate clones of the position, orientation and spacing
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImagePlaneHelper"/> from the specified image plane details.
		/// </summary>
		public ImagePlaneHelper(ImagePositionPatient imagePositionPatient, ImageOrientationPatient imageOrientationPatient, PixelSpacing pixelSpacing, int rows, int columns)
		{
			Platform.CheckForNullReference(imagePositionPatient, "imagePositionPatient");
			Platform.CheckForNullReference(imageOrientationPatient, "imageOrientationPatient");
			Platform.CheckForNullReference(pixelSpacing, "pixelSpacing");
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			_rows = rows;
			_columns = columns;
			_pixelSpacing = pixelSpacing;
			_imageOrientationPatient = imageOrientationPatient;
			_imagePositionPatient = imagePositionPatient;
		}

		/// <summary>
		/// Gets the number of rows in the frame.
		/// </summary>
		public int ImageRows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns in the frame.
		/// </summary>
		public int ImageColumns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets the <see cref="ImagePositionPatient"/> for the frame (the values of the Image Position (Patient) tag in the headers).
		/// </summary>
		public ImagePositionPatient ImagePositionPatient
		{
			get { return _imagePositionPatient; }
		}

		/// <summary>
		/// Gets the <see cref="ImageOrientationPatient"/> for the frame (the values of the Image Orientation (Patient) tag in the headers).
		/// </summary>
		public ImageOrientationPatient ImageOrientationPatient
		{
			get { return _imageOrientationPatient; }
		}

		/// <summary>
		/// Gets the <see cref="PixelSpacing"/> for the frame (the values of the Pixel Spacing tag in the headers).
		/// </summary>
		public PixelSpacing PixelSpacing
		{
			get { return _pixelSpacing; }
		}

		/// <summary>
		/// Gets the origin of the image plane in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid,
		/// and yields the same value as <see cref="ImageTopLeftPatient"/>.
		/// </remarks>
		public Vector3D ImagePositionPatientVector
		{
			get { return _imageTopLeftPatient ?? (_imageTopLeftPatient = !_imagePositionPatient.IsNull ? new Vector3D((float) _imagePositionPatient.X, (float) _imagePositionPatient.Y, (float) _imagePositionPatient.Z) : Vector3D.Null); }
		}

		/// <summary>
		/// Gets the orientation vector of the first row in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D ImageRowOrientationPatientVector
		{
			get { return _imageRowOrientationPatient ?? (_imageRowOrientationPatient = !_imageOrientationPatient.IsNull ? new Vector3D((float) _imageOrientationPatient.RowX, (float) _imageOrientationPatient.RowY, (float) _imageOrientationPatient.RowZ).Normalize() : Vector3D.Null); }
		}

		/// <summary>
		/// Gets the orientation vector of the first column in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D ImageColumnOrientationPatientVector
		{
			get { return _imageColumnOrientationPatient ?? (_imageColumnOrientationPatient = !_imageOrientationPatient.IsNull ? new Vector3D((float) _imageOrientationPatient.ColumnX, (float) _imageOrientationPatient.ColumnY, (float) _imageOrientationPatient.ColumnZ).Normalize() : Vector3D.Null); }
		}

		/// <summary>
		/// Gets the unit normal to the image plane in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D ImageNormalPatient
		{
			get { return _imageNormalPatient ?? (_imageNormalPatient = GetNormalVector(_imageOrientationPatient) ?? Vector3D.Null); }
		}

		/// <summary>
		/// Gets the top-left corner of the image in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid,
		/// and yields the same value as <see cref="ImagePositionPatientVector"/>.
		/// </remarks>
		public Vector3D ImageTopLeftPatient
		{
			get { return ImagePositionPatientVector; }
		}

		/// <summary>
		/// Gets the top-right corner of the image in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D ImageTopRightPatient
		{
			get { return _imageTopRightPatient ?? (_imageTopRightPatient = ConvertToPatient(new PointF(_columns, 0)) ?? Vector3D.Null); }
		}

		/// <summary>
		/// Gets the bottom-left corner of the image in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D ImageBottomLeftPatient
		{
			get { return _imageBottomLeftPatient ?? (_imageBottomLeftPatient = ConvertToPatient(new PointF(0, _rows)) ?? Vector3D.Null); }
		}

		/// <summary>
		/// Gets the bottom-right corner of the image in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D ImageBottomRightPatient
		{
			get { return _imageBottomRightPatient ?? (_imageBottomRightPatient = ConvertToPatient(new PointF(_columns, _rows)) ?? Vector3D.Null); }
		}

		/// <summary>
		/// Gets the centre of the image in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D ImageCenterPatient
		{
			get { return _imageCenterPatient ?? (_imageCenterPatient = ConvertToPatient(new PointF(_columns/2f, _rows/2f)) ?? Vector3D.Null); }
		}

		/// <summary>
		/// Gets the top-left corner of the image plane relative to the patient origin in the image coordinate system.
		/// </summary>
		/// <remarks>
		/// This property returns a zero vector if the image plane information is invalid.
		/// </remarks>
		public Vector3D TopLeftImagePlane
		{
			get { return _topLeftImagePlane ?? (_topLeftImagePlane = ConvertToImagePlane(ImageTopLeftPatient, Vector3D.Null) ?? Vector3D.Null); }
		}

		/// <summary>
		/// Indicates whether or not the image plane information is valid and can be used to transform between patient and image coordinate systems.
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

			// A shortcut for when the pixel position is exactly (0, 0).
			if (positionPixels.IsEmpty)
				return ImagePositionPatientVector;

			// Calculation of position in patient coordinates using 
			// the matrix method described in Dicom PS 3.3 C.7.6.2.1.1.
			var imageToPatientTransform = GetImageToPatientTransform();

			Matrix columnMatrix = new Matrix(4, 1);
			columnMatrix.SetColumn(0, positionPixels.X, positionPixels.Y, 0F, 1F);
			Matrix result = imageToPatientTransform*columnMatrix;

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

			var translated = positionPatient;
			if (originPatient != null)
				translated -= originPatient;

			var rotationMatrix = GetRotationMatrix();
			if (rotationMatrix == null)
				return null;

			// Rotate coordinate system to match that of the image plane.
			return rotationMatrix*translated;
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
			return ConvertToImagePlane(positionPatient, ImagePositionPatientVector);
		}

		/// <summary>
		/// Converts a point in the image plane expressed in millimetres (mm) into a point expressed in pixels.
		/// </summary>
		/// <returns>The corresponding pixel coordinate, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public PointF? ConvertToImage(PointF positionMillimetres)
		{
			if (PixelSpacing.IsNull)
				return null;

			return new PointF(positionMillimetres.X/(float) PixelSpacing.Column, positionMillimetres.Y/(float) PixelSpacing.Row);
		}

		/// <summary>
		/// Converts a point in the image expressed in pixels to a point expressed in mm.
		/// </summary>
		public PointF? ConvertFromImage(PointF positionPixels)
		{
			if (PixelSpacing.IsNull)
				return null;

			return new PointF(positionPixels.X*(float) PixelSpacing.Column, positionPixels.Y*(float) PixelSpacing.Row);
		}

		/// <summary>
		/// Converts a point in the image plane expressed in millimetres (mm) into a point expressed in pixels.
		/// </summary>
		/// <returns>The corresponding pixel coordinate.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the pixel spacing information was invalid.</exception>
		public PointF ConvertToImage2(PointF positionMillimetres)
		{
			var result = ConvertToImage(positionMillimetres);
			if (!result.HasValue) throw new InvalidOperationException("Operation not valid when pixel spacing information is invalid.");
			return result.Value;
		}

		/// <summary>
		/// Converts a point in the image expressed in pixels to a point expressed in mm.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the pixel spacing information was invalid.</exception>
		public PointF ConvertFromImage2(PointF positionPixels)
		{
			var result = ConvertFromImage(positionPixels);
			if (!result.HasValue) throw new InvalidOperationException("Operation not valid when pixel spacing information is invalid.");
			return result.Value;
		}

		/// <summary>
		/// Gets whether or not this image plane is parallel to the specified other image plane.
		/// </summary>
		/// <param name="other">The other <see cref="ImagePlaneHelper"/>.</param>
		/// <returns>True if the planes are mutually parallel; False otherwise.</returns>
		public bool IsParallelTo(ImagePlaneHelper other)
		{
			return IsParallelTo(other, _defaultToleranceRadians);
		}

		/// <summary>
		/// Gets whether or not this image plane is parallel to the specified other image plane.
		/// </summary>
		/// <param name="other">The other <see cref="ImagePlaneHelper"/>.</param>
		/// <param name="angleToleranceRadians">The angle of tolerance, in radians, for the planes to be considered parallel.</param>
		/// <returns>True if the planes are mutually parallel; False otherwise.</returns>
		public bool IsParallelTo(ImagePlaneHelper other, float angleToleranceRadians)
		{
			Platform.CheckForNullReference(other, "other");
			return ImageNormalPatient.IsParallelTo(other.ImageNormalPatient, angleToleranceRadians);
		}

		/// <summary>
		/// Gets whether or not this image plane is orthogonal to the specified other image plane.
		/// </summary>
		/// <param name="other">The other <see cref="ImagePlaneHelper"/>.</param>
		/// <returns>True if the planes are mutually orthogonal; False otherwise.</returns>
		public bool IsOrthogonalTo(ImagePlaneHelper other)
		{
			return IsOrthogonalTo(other, _defaultToleranceRadians);
		}

		/// <summary>
		/// Gets whether or not this image plane is orthogonal to the specified other image plane.
		/// </summary>
		/// <param name="other">The other <see cref="ImagePlaneHelper"/>.</param>
		/// <param name="angleToleranceRadians">The angle of tolerance, in radians, for the planes to be considered orthogonal.</param>
		/// <returns>True if the planes are mutually orthogonal; False otherwise.</returns>
		public bool IsOrthogonalTo(ImagePlaneHelper other, float angleToleranceRadians)
		{
			Platform.CheckForNullReference(other, "other");
			return ImageNormalPatient.IsOrthogonalTo(other.ImageNormalPatient, angleToleranceRadians);
		}

		/// <summary>
		/// Gets the angle between the normals of this image plane and the specified other image plane.
		/// </summary>
		/// <param name="other">The other <see cref="ImagePlaneHelper"/>.</param>
		/// <returns>The angle between the normals of the planes, in radians.</returns>
		public float GetAngleBetween(ImagePlaneHelper other)
		{
			Platform.CheckForNullReference(other, "other");
			return ImageNormalPatient.GetAngleBetween(other.ImageNormalPatient);
		}

		/// <summary>
		/// Gets whether or not this image plane intersects with the specified other image plane.
		/// </summary>
		/// <param name="other">The other <see cref="ImagePlaneHelper"/> with which to intersect.</param>
		/// <returns>True if an intersection between the planes exists; False otherwise.</returns>
		public bool IntersectsWith(ImagePlaneHelper other)
		{
			Vector3D p1, p2;
			return IntersectsWith(other, out p1, out p2);
		}

		/// <summary>
		/// Gets whether or not this image plane intersects with the specified other image plane.
		/// </summary>
		/// <param name="intersectionPointPatient1">Returns one end point of the line segment of intersection between the two planes, if it exists.</param>
		/// <param name="intersectionPointPatient2">Returns the other end point of the line segment of intersection between the two planes, if it exists.</param>
		/// <param name="other">The other <see cref="ImagePlaneHelper"/> with which to intersect.</param>
		/// <returns>True if an intersection between the planes exists; False otherwise.</returns>
		public bool IntersectsWith(ImagePlaneHelper other, out Vector3D intersectionPointPatient1, out Vector3D intersectionPointPatient2)
		{
			Platform.CheckForNullReference(other, "other");
			intersectionPointPatient1 = intersectionPointPatient2 = null;

			// Bounding line segments of this (reference) image plane
			var lineSegmentsImagePlaneBounds = new[,]
			                                   	{
			                                   		{ImageTopLeftPatient, ImageTopRightPatient},
			                                   		{ImageTopLeftPatient, ImageBottomLeftPatient},
			                                   		{ImageBottomRightPatient, ImageTopRightPatient},
			                                   		{ImageBottomRightPatient, ImageBottomLeftPatient}
			                                   	};

			var planeIntersectionPoints = new List<Vector3D>();
			for (var i = 0; i < 4; ++i)
			{
				// Intersect the bounding line segments of the reference plane of the target plane
				var intersectionPoint = Vector3D.GetLinePlaneIntersection(other.ImageNormalPatient, other.ImageCenterPatient,
				                                                          lineSegmentsImagePlaneBounds[i, 0],
				                                                          lineSegmentsImagePlaneBounds[i, 1], true);
				if (intersectionPoint != null) planeIntersectionPoints.Add(intersectionPoint);
			}

			if (planeIntersectionPoints.Count < 2) return false;

			intersectionPointPatient1 = planeIntersectionPoints[0];
			intersectionPointPatient2 = planeIntersectionPoints.Find(point => !planeIntersectionPoints[0].Equals(point));
			return intersectionPointPatient1 != null && intersectionPointPatient2 != null;
		}

		/// <summary>
		/// Gets the unit normal vector describing the plane of the image in patient coordinates.
		/// </summary>
		/// <remarks>
		/// This method yields the same results as <see cref="ImageNormalPatient"/>, except that it returns null if the image plane information is invalid.
		/// </remarks>
		/// <returns>The unit normal vector, or null if the image plane information is invalid.</returns>
		public Vector3D GetNormalVector()
		{
			var normal = ImageNormalPatient;
			return normal.IsNull ? null : normal;
		}

		/// <summary>
		/// Gets the unit normal vector describing the specified image plane in patient coordinates.
		/// </summary>
		/// <returns>The unit normal vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public static Vector3D GetNormalVector(ImageOrientationPatient imageOrientationPatient)
		{
			if (imageOrientationPatient.IsNull)
				return null;

			var left = new Vector3D((float) imageOrientationPatient.RowX, (float) imageOrientationPatient.RowY, (float) imageOrientationPatient.RowZ);
			var normal = left.Cross(new Vector3D((float) imageOrientationPatient.ColumnX, (float) imageOrientationPatient.ColumnY, (float) imageOrientationPatient.ColumnZ));
			return FloatComparer.AreEqual(normal.Magnitude, 0) ? Vector3D.Null : normal.Normalize();
		}

		/// <summary>
		/// Gets a rotation matrix that, when multiplied by a column matrix representing a
		/// position vector in patient coordinates, will rotate the position vector
		/// into a coordinate system matching that of the image plane.
		/// </summary>
		/// <returns>The rotation matrix, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		private Matrix3D GetRotationMatrix()
		{
			if (_rotationMatrix == null)
			{
				if (ImageOrientationPatient.IsNull)
					return null;

				var normal = GetNormalVector();
				if (normal == null || normal.IsNull)
					return null;

				_rotationMatrix = new Matrix3D();
				_rotationMatrix.SetRow(0, (float) ImageOrientationPatient.RowX, (float) ImageOrientationPatient.RowY, (float) ImageOrientationPatient.RowZ);
				_rotationMatrix.SetRow(1, (float) ImageOrientationPatient.ColumnX, (float) ImageOrientationPatient.ColumnY, (float) ImageOrientationPatient.ColumnZ);
				_rotationMatrix.SetRow(2, normal);
			}

			return _rotationMatrix;
		}

		private Matrix GetImageToPatientTransform()
		{
			if (_pixelToPatientTransform == null)
			{
				_pixelToPatientTransform = new Matrix(4, 4);

				_pixelToPatientTransform.SetColumn(0, (float) (ImageOrientationPatient.RowX*PixelSpacing.Column),
				                                   (float) (ImageOrientationPatient.RowY*PixelSpacing.Column),
				                                   (float) (ImageOrientationPatient.RowZ*PixelSpacing.Column), 0F);

				_pixelToPatientTransform.SetColumn(1, (float) (ImageOrientationPatient.ColumnX*PixelSpacing.Row),
				                                   (float) (ImageOrientationPatient.ColumnY*PixelSpacing.Row),
				                                   (float) (ImageOrientationPatient.ColumnZ*PixelSpacing.Row), 0F);

				_pixelToPatientTransform.SetColumn(3, ImagePositionPatientVector.X, ImagePositionPatientVector.Y, ImagePositionPatientVector.Z, 1F);
			}
			return _pixelToPatientTransform;
		}
	}
}