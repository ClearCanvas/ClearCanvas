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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;

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
	public abstract partial class Volume : IDisposable
	{
		#region Private fields

		private readonly Size3D _arrayDimensions;
		private readonly Vector3D _voxelSpacing;
		private readonly Rectangle3D _volumeBounds;
		private readonly Vector3D _volumePositionPatient;
		private readonly Vector3D _volumeOrientationPatientX;
		private readonly Vector3D _volumeOrientationPatientY;
		private readonly Vector3D _volumeOrientationPatientZ;
		private readonly Vector3D _volumeCenter;
		private readonly Vector3D _volumeCenterPatient;
		private readonly Matrix _volumeOrientationPatient;

		private readonly int _paddingValue;
		private readonly int _minVolumeValue;
		private readonly int _maxVolumeValue;

		private readonly string _sourceSeriesInstanceUid;
		private readonly VolumeSopDataSourcePrototype _dataSourcePrototype;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="Volume"/>.
		/// </summary>
		/// <param name="arrayDimensions"></param>
		/// <param name="voxelSpacing"></param>
		/// <param name="volumePositionPatient"></param>
		/// <param name="volumeOrientationPatient"></param>
		/// <param name="dataSourcePrototype"></param>
		/// <param name="paddingValue"></param>
		/// <param name="sourceSeriesInstanceUid"></param>
		/// <param name="minVolumeValue"></param>
		/// <param name="maxVolumeValue"></param>
		internal Volume(Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix volumeOrientationPatient, VolumeSopDataSourcePrototype dataSourcePrototype, int paddingValue, string sourceSeriesInstanceUid, int minVolumeValue, int maxVolumeValue)
		{
			Platform.CheckForNullReference(arrayDimensions, "arrayDimensions");
			Platform.CheckForNullReference(voxelSpacing, "voxelSpacing");
			Platform.CheckForNullReference(volumePositionPatient, "originPatient");
			Platform.CheckForNullReference(volumeOrientationPatient, "orientationPatient");
			Platform.CheckForNullReference(dataSourcePrototype, "sopDataSourcePrototype");

			_arrayDimensions = arrayDimensions;
			_voxelSpacing = voxelSpacing;
			_volumePositionPatient = volumePositionPatient;
			_volumeOrientationPatient = volumeOrientationPatient;
			_volumeOrientationPatientX = volumeOrientationPatient.GetRow(0);
			_volumeOrientationPatientY = volumeOrientationPatient.GetRow(1);
			_volumeOrientationPatientZ = volumeOrientationPatient.GetRow(2);
			_dataSourcePrototype = dataSourcePrototype;
			_paddingValue = paddingValue;
			_sourceSeriesInstanceUid = sourceSeriesInstanceUid ?? string.Empty;
			_minVolumeValue = minVolumeValue;
			_maxVolumeValue = maxVolumeValue;

			_volumeBounds = new Rectangle3D(0, 0, 0, _arrayDimensions.Width*_voxelSpacing.X, _arrayDimensions.Height*_voxelSpacing.Y, _arrayDimensions.Depth*_voxelSpacing.Z);
			_volumeCenter = 0.5f*_volumeBounds.Size;
			_volumeCenterPatient = ConvertToPatient(_volumeCenter);
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
				if (!(x >= 0 && x < _arrayDimensions.Width))
					throw new ArgumentOutOfRangeException("x", x, string.Format(message, "X"));
				else if (!(y >= 0 && y < _arrayDimensions.Height))
					throw new ArgumentOutOfRangeException("y", y, string.Format(message, "Y"));
				else if (!(z >= 0 && z < _arrayDimensions.Depth))
					throw new ArgumentOutOfRangeException("z", z, string.Format(message, "Z"));
				return GetArrayValue(x + _arrayDimensions.Width*(y + _arrayDimensions.Height*z));
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
			get { return _arrayDimensions; }
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
			get { return _volumeBounds.Size; }
		}

		/// <summary>
		/// Gets the effective bounds of the volume (that is, the 3-dimensional region occupied by the volume after accounting for <see cref="VoxelSpacing"/>).
		/// </summary>
		public Rectangle3D VolumeBounds
		{
			get { return _volumeBounds; }
		}

		/// <summary>
		/// Gets the spacing between voxels in millimetres (mm).
		/// </summary>
		/// <remarks>
		/// Equivalently, this is the size of a single voxel in millimetres along each dimension, and is the volumetric analogue of an image's Pixel Spacing.
		/// </remarks>
		public Vector3D VoxelSpacing
		{
			get { return _voxelSpacing; }
		}

		/// <summary>
		/// Gets the origin of the volume in the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Position (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumePositionPatient
		{
			get { return _volumePositionPatient; }
		}

		/// <summary>
		/// Gets the direction cosines describing the orientation of the volume X-axis relative to the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Orientation (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumeOrientationPatientX
		{
			get { return _volumeOrientationPatientX; }
		}

		/// <summary>
		/// Gets the direction cosines describing the orientation of the volume Y-axis relative to the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Orientation (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumeOrientationPatientY
		{
			get { return _volumeOrientationPatientY; }
		}

		/// <summary>
		/// Gets the direction cosines describing the orientation of the volume Z-axis relative to the patient coordinate system.
		/// </summary>
		/// <remarks>
		/// This is the volumetric analogue of the Image Orientation (Patient) concept in DICOM.
		/// </remarks>
		public Vector3D VolumeOrientationPatientZ
		{
			get { return _volumeOrientationPatientZ; }
		}

		/// <summary>
		/// Gets the centre of the volume in the volume coordinate system.
		/// </summary>
		public Vector3D VolumeCenter
		{
			get { return _volumeCenter; }
		}

		/// <summary>
		/// Gets the centre of the volume in the patient coordinate system.
		/// </summary>
		public Vector3D VolumeCenterPatient
		{
			get { return _volumeCenterPatient; }
		}

		/// <summary>
		/// Gets the value used for padding empty regions of the volume.
		/// </summary>
		public int PaddingValue
		{
			get { return _paddingValue; }
		}

		/// <summary>
		/// Gets the minimum voxel value in the volume data.
		/// </summary>
		public int MinimumVolumeValue
		{
			get { return _minVolumeValue; }
		}

		/// <summary>
		/// Gets the maximum voxel value in the volume data.
		/// </summary>
		public int MaximumVolumeValue
		{
			get { return _maxVolumeValue; }
		}

		/// <summary>
		/// Gets the Modality of the source images from which the volume was created.
		/// </summary>
		public string Modality
		{
			get
			{
				DicomAttribute attribute;
				return _dataSourcePrototype.TryGetAttribute(DicomTags.Modality, out attribute) ? attribute.ToString() : string.Empty;
			}
		}

		/// <summary>
		/// Gets the Series Instance UID that identifies the source images from which the volume was created.
		/// </summary>
		public string SourceSeriesInstanceUid
		{
			get { return _sourceSeriesInstanceUid; }
		}

		/// <summary>
		/// Gets the Frame of Reference UID that correlates the patient coordinate system with other data sources.
		/// </summary>
		public string FrameOfReferenceUid
		{
			get
			{
				DicomAttribute attribute;
				return _dataSourcePrototype.TryGetAttribute(DicomTags.FrameOfReferenceUid, out attribute) ? attribute.ToString() : string.Empty;
			}
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
			get { return _dataSourcePrototype; }
		}

		#endregion

		#region Coordinate Transforms

		/// <summary>
		/// Converts the specified volume position into the patient coordinate system.
		/// </summary>
		public Vector3D ConvertToPatient(Vector3D volumePosition)
		{
			// Set orientation transform
			var volumePatientTransform = new Matrix(_volumeOrientationPatient);

			// Set origin translation
			volumePatientTransform.SetRow(3, VolumePositionPatient.X, VolumePositionPatient.Y, VolumePositionPatient.Z, 1);

			// Transform volume position to patient position
			var imagePositionMatrix = new Matrix(1, 4);
			imagePositionMatrix.SetRow(0, volumePosition.X, volumePosition.Y, volumePosition.Z, 1F);
			var patientPositionMatrix = imagePositionMatrix*volumePatientTransform;

			var patientPosition = new Vector3D(patientPositionMatrix[0, 0], patientPositionMatrix[0, 1], patientPositionMatrix[0, 2]);
			return patientPosition;
		}

		/// <summary>
		/// Converts the specified patient position into the volume coordinate system.
		/// </summary>
		public Vector3D ConvertToVolume(Vector3D patientPosition)
		{
			// Set orientation transform
			var patientVolumeTransform = new Matrix(_volumeOrientationPatient.Transpose());

			// Set origin translation
			var rotatedOrigin = RotateToVolumeOrientation(VolumePositionPatient);
			patientVolumeTransform.SetRow(3, -rotatedOrigin.X, -rotatedOrigin.Y, -rotatedOrigin.Z, 1);

			// Transform patient position to volume position
			var patientPositionMatrix = new Matrix(1, 4);
			patientPositionMatrix.SetRow(0, patientPosition.X, patientPosition.Y, patientPosition.Z, 1F);
			var imagePositionMatrix = patientPositionMatrix*patientVolumeTransform;

			var imagePosition = new Vector3D(imagePositionMatrix[0, 0], imagePositionMatrix[0, 1], imagePositionMatrix[0, 2]);
			return imagePosition;
		}

		/// <summary>
		/// Rotates the specified volume orientation matrix into the patient coordinate system.
		/// </summary>
		public Matrix RotateToPatientOrientation(Matrix volumeOrientation)
		{
			var orientationPatient = volumeOrientation*_volumeOrientationPatient;
			return orientationPatient;
		}

		/// <summary>
		/// Rotates the specified patient orientation matrix into the volume coordinate system.
		/// </summary>
		public Matrix RotateToVolumeOrientation(Matrix patientOrientation)
		{
			var orientationVolume = patientOrientation*_volumeOrientationPatient.Transpose();
			return orientationVolume;
		}

		/// <summary>
		/// Rotates the specified volume vector into the patient coordinate system.
		/// </summary>
		public Vector3D RotateToPatientOrientation(Vector3D volumeVector)
		{
			var volumePos = new Matrix(1, 4);
			volumePos.SetRow(0, volumeVector.X, volumeVector.Y, volumeVector.Z, 1F);
			Matrix patientPos = volumePos*_volumeOrientationPatient;
			return new Vector3D(patientPos[0, 0], patientPos[0, 1], patientPos[0, 2]);
		}

		/// <summary>
		/// Rotates the specified patient vector into the volume coordinate system.
		/// </summary>
		public Vector3D RotateToVolumeOrientation(Vector3D patientVector)
		{
			var patientPos = new Matrix(1, 4);
			patientPos.SetRow(0, patientVector.X, patientVector.Y, patientVector.Z, 1F);
			var volumePos = patientPos*_volumeOrientationPatient.Transpose();
			return new Vector3D(volumePos[0, 0], volumePos[0, 1], volumePos[0, 2]);
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
		/// Gets a value indicating whether or not the object has already been disposed.
		/// </summary>
		protected bool Disposed { get; private set; }

		/// <summary>
		/// Called to release any resources held by this object.
		/// </summary>
		/// <param name="disposing">True if the object is being disposed; False if the object is being finalized.</param>
		protected virtual void Dispose(bool disposing)
		{
			Disposed = true;
		}

		#endregion
	}
}