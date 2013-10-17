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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Represents a 3-dimensional raster volume.
	/// </summary>
	/// <remarks>
	/// The <see cref="Volume"/> class encapsulates 3-dimensional raster data (i.e. a volume defined by a block of voxels).
	/// Typically, an instance of <see cref="Volume"/> is created from a set of DICOM images by calling
	/// <see cref="Create(IDisplaySet)"/> or any of its overloads. Alternatively, the <see cref="VolumeCache"/>
	/// may be used to obtain a wrapper object that allows access to a shared, cached and memory-managed
	/// instance of <see cref="Volume"/>.
	/// </remarks>
	/// <seealso cref="U16Volume"/>
	/// <seealso cref="S16Volume"/>
	/// <seealso cref="U8Volume"/>
	/// <seealso cref="S8Volume"/>
	public abstract partial class Volume : IVolumeHeader, IDisposable
	{
		#region Private fields

		private readonly VolumeHeaderData _volumeHeaderData;

		private int? _minVolumeValue;
		private int? _maxVolumeValue;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="Volume"/>.
		/// </summary>
		/// <param name="volumeHeaderData"></param>
		/// <param name="minVolumeValue"></param>
		/// <param name="maxVolumeValue"></param>
		internal Volume(VolumeHeaderData volumeHeaderData, int? minVolumeValue, int? maxVolumeValue)
		{
			Platform.CheckForNullReference(volumeHeaderData, "volumeHeader");

			_volumeHeaderData = volumeHeaderData;
			_minVolumeValue = minVolumeValue;
			_maxVolumeValue = maxVolumeValue;
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the value in the volume data at the specified array index.
		/// </summary>
		public int this[int i]
		{
			get { return GetArrayValue(i); }
		}

		/// <summary>
		/// Gets the value in the volume data at the specified array indices.
		/// </summary>
		public int this[int x, int y, int z]
		{
			get
			{
				const string message = "Specified {0}-index exceeds array bounds.";
				var arrayDimensions = ArrayDimensions;
				if (!(x >= 0 && x < arrayDimensions.Width))
					throw new ArgumentOutOfRangeException("x", x, string.Format(message, "X"));
				else if (!(y >= 0 && y < arrayDimensions.Height))
					throw new ArgumentOutOfRangeException("y", y, string.Format(message, "Y"));
				else if (!(z >= 0 && z < arrayDimensions.Depth))
					throw new ArgumentOutOfRangeException("z", z, string.Format(message, "Z"));
				return GetArrayValue(x + arrayDimensions.Width*(y + arrayDimensions.Height*z));
			}
		}

		/// <summary>
		/// Called to get the value in the volume data at the specified array index.
		/// </summary>
		protected abstract int GetArrayValue(int i);

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the array containing the volume data.
		/// </summary>
		public abstract Array Array { get; }

		/// <summary>
		/// Gets the total number of voxels in the volume.
		/// </summary>
		public int ArrayLength
		{
			get { return Array.Length; }
		}

		/// <summary>
		/// Gets the dimensions of the volume data.
		/// </summary>
		public Size3D ArrayDimensions
		{
			get { return _volumeHeaderData.ArrayDimensions; }
		}

		/// <summary>
		/// Gets whether or not the values of the volume data are signed.
		/// </summary>
		public abstract bool Signed { get; }

		/// <summary>
		/// Gets the number of bits per voxel of the volume data.
		/// </summary>
		public abstract int BitsPerVoxel { get; }

		/// <summary>
		/// Gets the effective size of the volume (that is, the <see cref="ArrayDimensions"/> multiplied by the <see cref="VoxelSpacing"/> in each respective dimension).
		/// </summary>
		public Vector3D VolumeSize
		{
			get { return _volumeHeaderData.VolumeSize; }
		}

		/// <summary>
		/// Gets the effective bounds of the volume (that is, the 3-dimensional region occupied by the volume after accounting for <see cref="VoxelSpacing"/>).
		/// </summary>
		public Rectangle3D VolumeBounds
		{
			get { return _volumeHeaderData.VolumeBounds; }
		}

		/// <summary>
		/// Gets the spacing between voxels in millimetres (mm).
		/// </summary>
		/// <remarks>
		/// Equivalently, this is the size of a single voxel in millimetres along each dimension, and is the volumetric analogue of an image's Pixel Spacing.
		/// </remarks>
		public Vector3D VoxelSpacing
		{
			get { return _volumeHeaderData.VoxelSpacing; }
		}

		/// <summary>
		/// Gets the origin of the volume in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Position (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumePositionPatient
		{
			get { return _volumeHeaderData.VolumePositionPatient; }
		}

		/// <summary>
		/// Gets the direction of the volume X-axis in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Orientation (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumeOrientationPatientX
		{
			get { return _volumeHeaderData.VolumeOrientationPatientX; }
		}

		/// <summary>
		/// Gets the direction of the volume Y-axis in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Orientation (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumeOrientationPatientY
		{
			get { return _volumeHeaderData.VolumeOrientationPatientY; }
		}

		/// <summary>
		/// Gets the direction of the volume Z-axis in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Orientation (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumeOrientationPatientZ
		{
			get { return _volumeHeaderData.VolumeOrientationPatientZ; }
		}

		/// <summary>
		/// Gets the centre of the volume in the volume coordinate system.
		/// </summary>
		public Vector3D VolumeCenter
		{
			get { return _volumeHeaderData.VolumeCenter; }
		}

		/// <summary>
		/// Gets the centre of the volume in the patient coordinate system.
		/// </summary>
		public Vector3D VolumeCenterPatient
		{
			get { return _volumeHeaderData.VolumeCenterPatient; }
		}

		/// <summary>
		/// Gets the value used for padding empty regions of the volume.
		/// </summary>
		public int PaddingValue
		{
			get { return _volumeHeaderData.PaddingValue; }
		}

		/// <summary>
		/// Gets the Rescale Slope of the linear modality LUT used to transform the values of the volume.
		/// </summary>
		/// <remarks>
		/// If the source frames have different rescale function parameters per frame, the volume data
		/// will be normalized such that they are defined by this one single rescale function.
		/// </remarks>
		public double RescaleSlope
		{
			get { return _volumeHeaderData.RescaleSlope; }
		}

		/// <summary>
		/// Gets the Rescale Intercept of the linear modality LUT used to transform the values of the volume.
		/// </summary>
		/// <remarks>
		/// If the source frames have different rescale function parameters per frame, the volume data
		/// will be normalized such that they are defined by this one single rescale function.
		/// </remarks>
		public double RescaleIntercept
		{
			get { return _volumeHeaderData.RescaleIntercept; }
		}

		/// <summary>
		/// Gets the output units of the linear modality LUT used to transform the values of the volume.
		/// </summary>
		public RescaleUnits RescaleUnits
		{
			get { return _volumeHeaderData.RescaleUnits; }
		}

		/// <summary>
		/// Gets the minimum voxel value in the volume data.
		/// </summary>
		public int MinimumVolumeValue
		{
			get { return _minVolumeValue.HasValue ? _minVolumeValue.Value : DoGetMinMaxVolumeValue(true); }
		}

		/// <summary>
		/// Gets the maximum voxel value in the volume data.
		/// </summary>
		public int MaximumVolumeValue
		{
			get { return _maxVolumeValue.HasValue ? _maxVolumeValue.Value : DoGetMinMaxVolumeValue(false); }
		}

		/// <summary>
		/// Gets the Modality of the source images from which the volume was created.
		/// </summary>
		public string Modality
		{
			get { return _volumeHeaderData.Modality; }
		}

		/// <summary>
		/// Gets the Study Instance UID that identifies the source images from which the volume was created.
		/// </summary>
		public string SourceStudyInstanceUid
		{
			get { return _volumeHeaderData.SourceStudyInstanceUid; }
		}

		/// <summary>
		/// Gets the Series Instance UID that identifies the source images from which the volume was created.
		/// </summary>
		public string SourceSeriesInstanceUid
		{
			get { return _volumeHeaderData.SourceSeriesInstanceUid; }
		}

		/// <summary>
		/// Gets the Frame of Reference UID that correlates the patient coordinate system with other data sources.
		/// </summary>
		public string FrameOfReferenceUid
		{
			get { return _volumeHeaderData.FrameOfReferenceUid; }
		}

		/// <summary>
		/// Gets the DICOM data set containing the common values in the source images from which the volume was created.
		/// </summary>
		/// <remarks>
		/// In general, this data set should be considered read only. This is especially true if you are using the <see cref="Volume"/>
		/// in conjunction with the <see cref="VolumeCache"/>, as any changes are only temporary and will be lost if the volume
		/// instance were unloaded by the memory manager and subsequently reloaded on demand.
		/// </remarks>
		public IVolumeDataSet DataSet
		{
			get { return _volumeHeaderData; }
		}

		#endregion

		#region Other Protected Methods

		private int DoGetMinMaxVolumeValue(bool returnMin)
		{
			int min, max;
			GetMinMaxVolumeValue(out min, out max);
			_minVolumeValue = min;
			_maxVolumeValue = max;
			return returnMin ? min : max;
		}

		/// <summary>
		/// Called to determine the minimum and maximum voxel values in the volume data.
		/// </summary>
		protected abstract void GetMinMaxVolumeValue(out int minValue, out int maxValue);

		#endregion

		#region Coordinate Transforms

		/// <summary>
		/// Converts the specified volume position into the patient coordinate system.
		/// </summary>
		/// <param name="volumePosition">The volume position to be converted, specified as a <see cref="Vector3D"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumePosition"/> is NULL.</exception>
		/// <returns>The specified volume position converted to the patient coordinate system.</returns>
		public Vector3D ConvertToPatient(Vector3D volumePosition)
		{
			return _volumeHeaderData.ConvertToPatient(volumePosition);
		}

		/// <summary>
		/// Converts the specified patient position into the volume coordinate system.
		/// </summary>
		/// <param name="patientPosition">The patient position to be converted, specified as a <see cref="Vector3D"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="patientPosition"/> is NULL.</exception>
		/// <returns>The specified patient position converted to the volume coordinate system.</returns>
		public Vector3D ConvertToVolume(Vector3D patientPosition)
		{
			return _volumeHeaderData.ConvertToVolume(patientPosition);
		}

		/// <summary>
		/// Rotates the specified volume orientation matrix into the patient coordinate system.
		/// </summary>
		/// <param name="volumeOrientation">The volume orientation to be converted, specified as a <see cref="Matrix3D"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeOrientation"/> is NULL.</exception>
		/// <returns>The specified volume orientation converted to the patient coordinate system.</returns>
		public Matrix3D RotateToPatientOrientation(Matrix3D volumeOrientation)
		{
			return _volumeHeaderData.RotateToPatientOrientation(volumeOrientation);
		}

		/// <summary>
		/// Rotates the specified patient orientation matrix into the volume coordinate system.
		/// </summary>
		/// <param name="patientOrientation">The patient orientation to be converted, specified as a <see cref="Matrix3D"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="patientOrientation"/> is NULL.</exception>
		/// <returns>The specified patient orientation converted to the volume coordinate system.</returns>
		public Matrix3D RotateToVolumeOrientation(Matrix3D patientOrientation)
		{
			return _volumeHeaderData.RotateToVolumeOrientation(patientOrientation);
		}

		/// <summary>
		/// Rotates the specified volume orientation or affine transformation matrix into the patient coordinate system.
		/// </summary>
		/// <param name="volumeOrientation">The volume orientation to be converted, specified as a 3x3 (orientation) or 4x4 (affine transformation) instance of <see cref="Matrix"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeOrientation"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="volumeOrientation"/> is not a 3x3 orientation matrix or a 4x4 affine transformation matrix.</exception>
		/// <returns>The specified volume orientation converted to the patient coordinate system.</returns>
		public Matrix RotateToPatientOrientation(Matrix volumeOrientation)
		{
			return _volumeHeaderData.RotateToPatientOrientation(volumeOrientation);
		}

		/// <summary>
		/// Rotates the specified patient orientation or affine transformation matrix into the volume coordinate system.
		/// </summary>
		/// <param name="patientOrientation">The patient orientation to be converted, specified as a 3x3 (orientation) or 4x4 (affine transformation) instance of <see cref="Matrix"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="patientOrientation"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="patientOrientation"/> is not a 3x3 orientation matrix or a 4x4 affine transformation matrix.</exception>
		/// <returns>The specified patient orientation converted to the volume coordinate system.</returns>
		public Matrix RotateToVolumeOrientation(Matrix patientOrientation)
		{
			return _volumeHeaderData.RotateToVolumeOrientation(patientOrientation);
		}

		/// <summary>
		/// Rotates the specified volume vector into the patient coordinate system.
		/// </summary>
		/// <param name="volumeVector">The volume vector to be converted, specified as a <see cref="Vector3D"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeVector"/> is NULL.</exception>
		/// <returns>The specified volume vector converted to the patient coordinate system.</returns>
		public Vector3D RotateToPatientOrientation(Vector3D volumeVector)
		{
			return _volumeHeaderData.RotateToPatientOrientation(volumeVector);
		}

		/// <summary>
		/// Rotates the specified patient vector into the volume coordinate system.
		/// </summary>
		/// <param name="patientVector">The patient vector to be converted, specified as a <see cref="Vector3D"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="patientVector"/> is NULL.</exception>
		/// <returns>The specified patient vector converted to the volume coordinate system.</returns>
		public Vector3D RotateToVolumeOrientation(Vector3D patientVector)
		{
			return _volumeHeaderData.RotateToVolumeOrientation(patientVector);
		}

		#endregion

		#region Finalizer and Disposal

		~Volume()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex);
			}
		}

		/// <summary>
		/// Called to release any resources held by this object.
		/// </summary>
		/// <param name="disposing">True if the object is being disposed; False if the object is being finalized.</param>
		protected virtual void Dispose(bool disposing) {}

		#endregion
	}
}