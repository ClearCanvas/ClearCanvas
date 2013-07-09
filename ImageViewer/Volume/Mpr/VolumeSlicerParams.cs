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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO (cr Oct 2009): just properties on Slicer?  Might factor itself out when Slicer creates Slice objects.
	/// <summary>
	/// Defines the parameters for a particular slicing of a <see cref="Volume"/> object (i.e. plane boundaries, orientation, thickness, etc.)
	/// </summary>
	public partial class VolumeSlicerParams : IVolumeSlicerParams
	{
		/// <summary>
		/// Gets the identity slice plane orientation, which is (X=0, Y=0, Z=0).
		/// </summary>
		public static readonly IVolumeSlicerParams Identity = new VolumeSlicerParams().AsReadOnly();

		/// <summary>
		/// Gets an orthogonal slice plane orientation along the X axis in the original frame, which is (X=90, Y=0, Z=0).
		/// </summary>
		public static readonly IVolumeSlicerParams OrthogonalX = new VolumeSlicerParams(90, 0, 0, string.Format(SR.FormatSliceOrthogonalX, 90, 0, 0)).AsReadOnly();

		/// <summary>
		/// Gets an orthogonal slice plane orientation along the Y axis in the original frame, which is (X=90, Y=0, Z=90).
		/// </summary>
		public static readonly IVolumeSlicerParams OrthogonalY = new VolumeSlicerParams(90, 0, 90, string.Format(SR.FormatSliceOrthogonalY, 90, 0, 90)).AsReadOnly();

		private const float _radiansPerDegree = (float) (Math.PI/180);
		private const float _degreesPerRadian = (float) (180/Math.PI);

		private Vector3D _sliceThroughPointPatient;
		private float _sliceExtentXmm;
		private float _sliceExtentYmm;
		private float _sliceSpacing;

		private Matrix _slicingPlaneRotation;
		private float _rotateAboutX;
		private float _rotateAboutY;
		private float _rotateAboutZ;

		private string _description;

		private VolumeSlicerInterpolationMode _interpolationMode = VolumeSlicerInterpolationMode.Linear;

		/// <summary>
		/// Initializes a <see cref="VolumeSlicerParams"/> for the identity slice orientation.
		/// </summary>
		public VolumeSlicerParams()
		{
			_slicingPlaneRotation = Matrix.GetIdentity(4);
			_rotateAboutX = _rotateAboutY = _rotateAboutZ = 0;
			_description = string.Format(SR.FormatSliceIdentity, 0, 0, 0);
		}

		/// <summary>
		/// Initializes a <see cref="VolumeSlicerParams"/> for a slice orientation defined as rotations about individual axes.
		/// </summary>
		/// <param name="rotateAboutX">The desired rotation about the X-axis in degrees.</param>
		/// <param name="rotateAboutY">The desired rotation about the Y-axis in degrees.</param>
		/// <param name="rotateAboutZ">The desired rotation about the Z-axis in degrees.</param>
		public VolumeSlicerParams(float rotateAboutX, float rotateAboutY, float rotateAboutZ)
			: this(rotateAboutX, rotateAboutY, rotateAboutZ, string.Format(SR.FormatSliceCustom, rotateAboutX, rotateAboutY, rotateAboutZ)) {}

		/// <summary>
		/// Initializes a <see cref="VolumeSlicerParams"/> for a slice orientation defined as rotations about individual axes.
		/// </summary>
		/// <param name="rotateAboutX">The desired rotation about the X-axis in degrees.</param>
		/// <param name="rotateAboutY">The desired rotation about the Y-axis in degrees.</param>
		/// <param name="rotateAboutZ">The desired rotation about the Z-axis in degrees.</param>
		/// <param name="description">A string value for describing the orientation.</param>
		public VolumeSlicerParams(float rotateAboutX, float rotateAboutY, float rotateAboutZ, string description)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateAboutX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateAboutY);
			Matrix aboutZ = CalcRotateMatrixAboutZ(rotateAboutZ);

			_slicingPlaneRotation = aboutX*aboutY*aboutZ;
			_rotateAboutX = rotateAboutX;
			_rotateAboutY = rotateAboutY;
			_rotateAboutZ = rotateAboutZ;

			_description = description;
		}

		/// <summary>
		/// Initializes a <see cref="VolumeSlicerParams"/> for a slice orientation defined using a set of orthogonal basis vectors.
		/// </summary>
		/// <remarks>
		/// The provided orthogonal basis vectors need not be unit vectors.
		/// </remarks>
		/// <param name="xAxis">The desired X-axis of the coordinate system in the slice orientation.</param>
		/// <param name="yAxis">The desired Y-axis of the coordinate system in the slice orientation.</param>
		/// <param name="zAxis">The desired Z-axis of the coordinate system in the slice orientation.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the provided vectors is null.</exception>
		/// <exception cref="ArgumentException">Thrown if the provided vectors do not form an orthogonal basis.</exception>
		public VolumeSlicerParams(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
			: this(CalcRotateMatrixFromOrthogonalBasis(xAxis, yAxis, zAxis)) {}

		/// <summary>
		/// Initializes a <see cref="VolumeSlicerParams"/> for a slice orientation defined using a set of orthogonal basis vectors.
		/// </summary>
		/// <remarks>
		/// The provided orthogonal basis vectors need not be unit vectors.
		/// </remarks>
		/// <param name="xAxis">The desired X-axis of the coordinate system in the slice orientation.</param>
		/// <param name="yAxis">The desired Y-axis of the coordinate system in the slice orientation.</param>
		/// <param name="zAxis">The desired Z-axis of the coordinate system in the slice orientation.</param>
		/// <param name="description">A string value for describing the orientation.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the provided vectors is null.</exception>
		/// <exception cref="ArgumentException">Thrown if the provided vectors do not form an orthogonal basis.</exception>
		public VolumeSlicerParams(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis, string description)
			: this(CalcRotateMatrixFromOrthogonalBasis(xAxis, yAxis, zAxis))
		{
			_description = description;
		}

		/// <summary>
		/// Initializes a <see cref="VolumeSlicerParams"/> for a slice orientation defined as a 4x4 affine transformation matrix.
		/// </summary>
		/// <param name="sliceRotation">The desired 4x4 affine transformation matrix.</param>
		/// <param name="description">A string value for describing the orientation.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided matrix is null.</exception>
		/// <exception cref="ArgumentException">Thrown if the provided matrix is not an affine transform.</exception>
		public VolumeSlicerParams(Matrix sliceRotation, string description)
			: this(sliceRotation)
		{
			_description = description;
		}

		/// <summary>
		/// Initializes a <see cref="VolumeSlicerParams"/> for a slice orientation defined as a 4x4 affine transformation matrix.
		/// </summary>
		/// <param name="sliceRotation">The desired 4x4 affine transformation matrix.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided matrix is null.</exception>
		/// <exception cref="ArgumentException">Thrown if the provided matrix is not an affine transform.</exception>
		public VolumeSlicerParams(Matrix sliceRotation)
		{
			const string invalidTransformMessage = "sliceRotation must be a 4x4 affine transformation matrix.";

			Platform.CheckForNullReference(sliceRotation, "sliceRotation");
			Platform.CheckTrue(sliceRotation.Columns == 4 && sliceRotation.Rows == 4, invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 0], 0), invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 1], 0), invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 2], 0), invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 3], 1), invalidTransformMessage);

			//is this a "rotation matrix" or a "desired coordinate system"
			_slicingPlaneRotation = sliceRotation;

			//TODO (CR Sept 2010): if the input matrix is a rotation matrix, why are we not just extracting values
			//for _rotateAboutX, etc.  Also, _slicingPlaneRotation seems more or less unused.
			double yRadians = (float) Math.Asin(sliceRotation[0, 2]);
			double cosY = Math.Cos(yRadians);
			_rotateAboutX = (float) Math.Atan2(-sliceRotation[1, 2]/cosY, sliceRotation[2, 2]/cosY)*_degreesPerRadian;
			_rotateAboutY = (float) yRadians*_degreesPerRadian;
			_rotateAboutZ = (float) Math.Atan2(-sliceRotation[0, 1]/cosY, sliceRotation[0, 0]/cosY)*_degreesPerRadian;

			_description = string.Format(SR.FormatSliceCustom, _rotateAboutX, _rotateAboutY, _rotateAboutZ);
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		//TODO (CR Sept 2010): Unclear what this is for.  If you can infer _rotateAboutX, etc. from this, why are they separate?
		public Matrix SlicingPlaneRotation
		{
			get { return _slicingPlaneRotation; }
			//TODO (CR Sept 2010): Shouldn't changing this change _rotateAboutX etc.
			set { _slicingPlaneRotation = value; }
		}

		public Vector3D SliceThroughPointPatient
		{
			get { return _sliceThroughPointPatient; }
			set { _sliceThroughPointPatient = value; }
		}

		public float SliceExtentXMillimeters
		{
			get { return _sliceExtentXmm; }
			set { _sliceExtentXmm = value; }
		}

		public float SliceExtentYMillimeters
		{
			get { return _sliceExtentYmm; }
			set { _sliceExtentYmm = value; }
		}

		public float SliceSpacing
		{
			get { return _sliceSpacing; }
			set { _sliceSpacing = value; }
		}

		public VolumeSlicerInterpolationMode InterpolationMode
		{
			get { return _interpolationMode; }
			set { _interpolationMode = value; }
		}

		bool IVolumeSlicerParams.IsReadOnly
		{
			get { return false; }
		}

		public float RotateAboutX
		{
			get { return _rotateAboutX; }
		}

		public float RotateAboutY
		{
			get { return _rotateAboutY; }
		}

		public float RotateAboutZ
		{
			get { return _rotateAboutZ; }
		}

		public IVolumeSlicerParams AsReadOnly()
		{
			return new ReadOnlyVolumeSlicerParams(this);
		}

		/// <summary>
		/// Allows specification of the slice plane, through point, and extent via two points in patient space
		/// </summary>
		public static VolumeSlicerParams Create(IVolumeHeader volume, Vector3D sourceOrientationColumnPatient, Vector3D sourceOrientationRowPatient,
		                                               Vector3D startPointPatient, Vector3D endPointPatient)
		{
			Vector3D sourceOrientationNormalPatient = sourceOrientationColumnPatient.Cross(sourceOrientationRowPatient);
			Vector3D normalLinePatient = (endPointPatient - startPointPatient).Normalize();
			Vector3D normalPerpLinePatient = sourceOrientationNormalPatient.Cross(normalLinePatient);

			Vector3D slicePlanePatientX = normalLinePatient;
			Vector3D slicePlanePatientY = sourceOrientationNormalPatient;
			Vector3D slicePlanePatientZ = normalPerpLinePatient;

			Matrix slicePlanePatientOrientation = Math3D.OrientationMatrixFromVectors(slicePlanePatientX, slicePlanePatientY, slicePlanePatientZ);

			Matrix _resliceAxes = volume.RotateToVolumeOrientation(slicePlanePatientOrientation);
			Vector3D lineMiddlePointPatient = new Vector3D(
				(startPointPatient.X + endPointPatient.X)/2,
				(startPointPatient.Y + endPointPatient.Y)/2,
				(startPointPatient.Z + endPointPatient.Z)/2);

			VolumeSlicerParams slicerParams = new VolumeSlicerParams(_resliceAxes);

			slicerParams.SliceThroughPointPatient = new Vector3D(lineMiddlePointPatient);
			slicerParams.SliceExtentXMillimeters = (endPointPatient - startPointPatient).Magnitude;

			return slicerParams;
		}

		#region Static Helpers

		private static Matrix CalcRotateMatrixAboutX(float rotateXdegrees)
		{
			Matrix aboutX;

			if (rotateXdegrees != 0)
			{
				float rotateXradians = rotateXdegrees*_radiansPerDegree;
				float sinX = (float) Math.Sin(rotateXradians);
				float cosX = (float) Math.Cos(rotateXradians);
				aboutX = new Matrix(new float[4,4]
				                    	{
				                    		{1, 0, 0, 0},
				                    		{0, cosX, -sinX, 0},
				                    		{0, sinX, cosX, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutX = Matrix.GetIdentity(4);

			return aboutX;
		}

		private static Matrix CalcRotateMatrixAboutY(float rotateYdegrees)
		{
			Matrix aboutY;

			if (rotateYdegrees != 0)
			{
				float rotateYradians = rotateYdegrees*_radiansPerDegree;
				float sinY = (float) Math.Sin(rotateYradians);
				float cosY = (float) Math.Cos(rotateYradians);
				aboutY = new Matrix(new float[4,4]
				                    	{
				                    		{cosY, 0, sinY, 0},
				                    		{0, 1, 0, 0},
				                    		{-sinY, 0, cosY, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutY = Matrix.GetIdentity(4);

			return aboutY;
		}

		private static Matrix CalcRotateMatrixAboutZ(float rotateZdegrees)
		{
			Matrix aboutZ;

			if (rotateZdegrees != 0)
			{
				float rotateZradians = rotateZdegrees*_radiansPerDegree;
				float sinZ = (float) Math.Sin(rotateZradians);
				float cosZ = (float) Math.Cos(rotateZradians);
				aboutZ = new Matrix(new float[4,4]
				                    	{
				                    		{cosZ, -sinZ, 0, 0},
				                    		{sinZ, cosZ, 0, 0},
				                    		{0, 0, 1, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutZ = Matrix.GetIdentity(4);

			return aboutZ;
		}

		private static Matrix CalcRotateMatrixFromOrthogonalBasis(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
		{
			const float zeroTolerance = 1e-4f;
			Platform.CheckForNullReference(xAxis, "xAxis");
			Platform.CheckForNullReference(yAxis, "yAxis");
			Platform.CheckForNullReference(zAxis, "zAxis");
			Platform.CheckFalse(xAxis.IsNull || yAxis.IsNull || zAxis.IsNull, "Input must be an orthogonal set of basis vectors (i.e. non-trivial vectors).");
			Platform.CheckTrue(FloatComparer.AreEqual(xAxis.Dot(yAxis), 0, zeroTolerance)
			                   && FloatComparer.AreEqual(xAxis.Dot(zAxis), 0, zeroTolerance)
			                   && FloatComparer.AreEqual(yAxis.Dot(zAxis), 0, zeroTolerance), "Input must be an orthogonal set of basis vectors (i.e. mutually perpendicular).");

			xAxis = xAxis.Normalize();
			yAxis = yAxis.Normalize();
			zAxis = zAxis.Normalize();

			//TODO (CR Sept 2010): is this a rotation matrix, or the definition of a coordinate system?
			var basis = new Matrix(4, 4);
			basis.SetRow(0, xAxis.X, xAxis.Y, xAxis.Z, 0);
			basis.SetRow(1, yAxis.X, yAxis.Y, yAxis.Z, 0);
			basis.SetRow(2, zAxis.X, zAxis.Y, zAxis.Z, 0);
			basis.SetRow(3, 0, 0, 0, 1);
			return basis;
		}

		#endregion
	}
}