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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Represents a 3-dimensional volume.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="Volume"/> class encapsulates 3 dimensional voxel data and currently supports short and unsigned short types.
	/// You will typically use a <see cref="VolumeBuilder"/> object to create a Volume.
	/// A volume has two coordinate spaces of interest: volume and patient. Naming convention is to use Patient
	/// suffix if in patient space, Volume suffix in volume space, if not specified then in volume space.
	/// </para>
	/// <para>
	/// The spaces currently only differ (potentially) by origin and orientation. The volume origin is 
	/// fixed to 0,0,0 and the patient origin is derived from the first image's DICOM image position (patient).
	/// The volume orientation is consistently defined by the input images and is irrespective of the actual
	/// patient orientation (i.e. axial, sagittal, coronal captured images are all normalized in volume space).
	/// The patient orientation is derived from the DICOM image orientation.
	/// </para>
	/// </remarks>
	public partial class Volume : IDisposable
	{
		#region Private fields

		// CR (Oct 2009): Ideally, Volume should be a base class with specialized signed and unsigned 16-bit volume subclasses
		// The volume arrays that contain the voxel values. Constructors ensure that only one of these 
		//	arrays is set, which defines whether the data for this volume is signed or unsigned.
		private short[] _volumeDataInt16;
		private ushort[] _volumeDataUInt16;

		// Size of each of the volume array dimensions
		private readonly Size3D _arrayDimensions;

		// The spacing between pixels (X,Y) and slices (Z) as defined by the DICOM images for this volume
		private readonly Vector3D _voxelSpacing;
		// The DICOM image position (patient) of the first slice in the volume, used to convert between Volume and Patient spaces
		private readonly Vector3D _originPatient;
		// The DICOM image orientation (patient) of all slices, used to convert between Volume and Patient spaces
		private readonly Matrix _orientationPatientMatrix;
		// Used as pixel value for any data not derived from voxel values (e.g. when slice extends beyond volume)
		private readonly int _paddingValue;
		// Decided to keep the volume origin at 0,0,0 and translate to patient coordinates
		//	when needed. This makes dealing with non axial datasets easier. It also mimics 
		//	the typical image to patient coordintate transform.
		private readonly Vector3D _origin = new Vector3D(0, 0, 0);

		// As pertinent volume fields are currently readonly, we only have to create the
		//	VTK volume wrapper once for each volume.
		private vtkImageData _cachedVtkVolume;

		// This handle is used to pin the volume array. Whenever VTK operates on the volume,
		//	the volume array must be pinned.
		private GCHandle? _volumeArrayPinnedHandle;

		private bool _disposed = false;

		private readonly string _description;
		private readonly string _sourceSeriesInstanceUid;
		private readonly int _minVolumeValue;
		private readonly int _maxVolumeValue;
		private readonly VolumeSopDataSourcePrototype _modelDicom;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a <see cref="Volume"/> using a volume data array of signed 16-bit words.
		/// </summary>
		/// <remarks>
		/// Consider using one of the static helpers such as <see cref="Create(ClearCanvas.ImageViewer.IDisplaySet)"/> to construct and automatically fill a <see cref="Volume"/>.
		/// </remarks>
		public Volume(short[] data, Size3D dimensions, Vector3D spacing, Vector3D originPatient,
					  Matrix orientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, string sourceSeriesInstanceUid)
			: this(data, null, dimensions, spacing, originPatient, orientationPatient,
			       VolumeSopDataSourcePrototype.Create(dicomAttributeModel, 16, 16, true), paddingValue, sourceSeriesInstanceUid, 0, 0) {}

		/// <summary>
		/// Constructs a <see cref="Volume"/> using a volume data array of unsigned 16-bit words.
		/// </summary>
		/// <remarks>
		/// Consider using one of the static helpers such as <see cref="Create(ClearCanvas.ImageViewer.IDisplaySet)"/> to construct and automatically fill a <see cref="Volume"/>.
		/// </remarks>
		public Volume(ushort[] data, Size3D dimensions, Vector3D spacing, Vector3D originPatient,
					  Matrix orientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, string sourceSeriesInstanceUid)
			: this(null, data, dimensions, spacing, originPatient, orientationPatient,
			       VolumeSopDataSourcePrototype.Create(dicomAttributeModel, 16, 16, false), paddingValue, sourceSeriesInstanceUid, 0, 0) {}

		private Volume(short[] dataInt16, ushort[] dataUInt16, Size3D dimensions, Vector3D spacing, Vector3D originPatient, Matrix orientationPatient, VolumeSopDataSourcePrototype sopDataSourcePrototype, int paddingValue, string sourceSeriesInstanceUid, int minVolumeValue, int maxVolumeValue)
		{
			Platform.CheckTrue(dataInt16 != null ^ dataUInt16 != null, "Exactly one of dataInt16 and dataUInt16 must be non-null.");
			_volumeDataInt16 = dataInt16;
			_volumeDataUInt16 = dataUInt16;
			_sourceSeriesInstanceUid = sourceSeriesInstanceUid;
			_minVolumeValue = minVolumeValue;
			_maxVolumeValue = maxVolumeValue;
			_arrayDimensions = dimensions;
			_voxelSpacing = spacing;
			_originPatient = originPatient;
			_orientationPatientMatrix = orientationPatient;
			_modelDicom = sopDataSourcePrototype;
			_paddingValue = paddingValue;

			// Generate a descriptive name for the volume
			PersonName patientName = new PersonName(sopDataSourcePrototype[DicomTags.PatientsName].ToString());
			string patientId = sopDataSourcePrototype[DicomTags.PatientId].ToString();
			string seriesDescription = sopDataSourcePrototype[DicomTags.SeriesDescription].ToString();
			if (string.IsNullOrEmpty(seriesDescription))
				_description = string.Format(SR.FormatVolumeLabel, patientName.FormattedName, patientId, seriesDescription);
			else
				_description = string.Format(SR.FormatVolumeLabelWithSeries, patientName.FormattedName, patientId, seriesDescription);
		}

		#endregion

		#region Public properties

		public string Description
		{
			get { return _description; }
		}

		public string Modality
		{
			get
			{
				DicomAttribute attribute;
				return _modelDicom.TryGetAttribute(DicomTags.Modality, out attribute) ? attribute.ToString() : string.Empty;
			}
		}

		/// <summary>
		/// Gets the Series Instance UID of that identifies the source images from which the volume was created.
		/// </summary>
		public string SourceSeriesInstanceUid
		{
			get { return _sourceSeriesInstanceUid; }
		}

		/// <summary>
		/// Gets the Frame of Reference UID (0020,0052) that identifies the volume's coordinate system.
		/// </summary>
		public string FrameOfReferenceUid
		{
			get
			{
				DicomAttribute attribute;
				if (_modelDicom.TryGetAttribute(DicomTags.FrameOfReferenceUid, out attribute))
					return attribute.ToString();
				return string.Empty;
			}
		}

		/// <summary>
		/// The effective volume dimensions in Volume space.
		/// </summary>
		public Vector3D Dimensions
		{
			get
			{
				return new Vector3D(ArrayDimensions.Width*VoxelSpacing.X, ArrayDimensions.Height*VoxelSpacing.Y,
				                    ArrayDimensions.Depth*VoxelSpacing.Z);
			}
		}

		/// <summary>
		/// Spacing in millimeters along respective axes in Volume space.
		/// </summary>
		public Vector3D VoxelSpacing
		{
			get { return _voxelSpacing; }
		}

		/// <summary>
		/// The origin of the volume in Patient space.
		/// </summary>
		public Vector3D OriginPatient
		{
			get { return _originPatient; }
		}

		/// <summary>
		/// The orientation of the volume in Patient space.
		/// </summary>
		public Matrix OrientationPatientMatrix
		{
			get { return _orientationPatientMatrix; }
		}

		/// <summary>
		/// Gets a value indicating whether this volume contains signed or unsigned data.
		/// </summary>
		public bool Signed
		{
			get { return _volumeDataInt16 != null; }
		}

		/// <summary>
		/// Gets the minimum value in the volume data.
		/// </summary>
		public int MinimumVolumeValue
		{
			get { return _minVolumeValue; }
		}

		/// <summary>
		/// Gets the maximum value in the volume data.
		/// </summary>
		public int MaximumVolumeValue
		{
			get { return _maxVolumeValue; }
		}

		/// <summary>
		/// The number of voxels represented by this volume (and in the volume array).
		/// </summary>
		public int SizeInVoxels
		{
			get { return ArrayDimensions.Volume; }
		}

		public float MinimumXCoodinate
		{
			get { return Origin.X; }
		}

		public float MaximumXCoordinate
		{
			get { return (Origin.X + VoxelSpacing.X*ArrayDimensions.Width); }
		}

		public float MinimumYCoordinate
		{
			get { return Origin.Y; }
		}

		public float MaximumYCoordinate
		{
			get { return (Origin.Y + VoxelSpacing.Y*ArrayDimensions.Height); }
		}

		public float MinimumZCoordinate
		{
			get { return Origin.Z; }
		}

		public float MaximumZCoordinate
		{
			get { return (Origin.Z + VoxelSpacing.Z*ArrayDimensions.Depth); }
		}

		public float MinimumSpacing
		{
			get { return Math.Min(Math.Min(VoxelSpacing.X, VoxelSpacing.Y), VoxelSpacing.Z); }
		}

		public float MaximumSpacing
		{
			get { return Math.Max(Math.Max(VoxelSpacing.X, VoxelSpacing.Y), VoxelSpacing.Z); }
		}

		public Vector3D Origin
		{
			get { return _origin; }
		}

		public int PaddingValue
		{
			get { return _paddingValue; }
		}

		public float LongAxisMagnitude
		{
			get { return Math.Max(Math.Max(Dimensions.X, Dimensions.Y), Dimensions.Z); }
		}

		public float ShortAxisMagnitude
		{
			get { return Math.Min(Math.Min(Dimensions.X, Dimensions.Y), Dimensions.Z); }
		}

		public float DiagonalMagnitude
		{
			get
			{
				return (float) Math.Sqrt(Dimensions.X*Dimensions.X +
				                         Dimensions.Y*Dimensions.Y +
				                         Dimensions.Z*Dimensions.Z);
			}
		}

		/// <summary>
		/// Volume center point in volume coordinates
		/// </summary>
		public Vector3D CenterPoint
		{
			get
			{
				Vector3D center = new Vector3D(Origin.X + VoxelSpacing.X*0.5f*ArrayDimensions.Width,
				                               Origin.Y + VoxelSpacing.Y*0.5f*ArrayDimensions.Height,
				                               Origin.Z + VoxelSpacing.Z*0.5f*ArrayDimensions.Depth);
				return center;
			}
		}

		/// <summary>
		/// Volume center point in patient coordinates
		/// </summary>
		public Vector3D CenterPointPatient
		{
			get { return ConvertToPatient(CenterPoint); }
		}

		public bool Contains(Vector3D point)
		{
			return this.Contains(point.X, point.Y, point.Z);
		}

		public bool Contains(float x, float y, float z)
		{
			return x >= MinimumXCoodinate && x <= MaximumXCoordinate &&
			       y >= MinimumYCoordinate && y <= MaximumYCoordinate &&
			       z >= MinimumZCoordinate && z <= MaximumZCoordinate;
		}

		#endregion

		#region Coordinate Transforms

		public Vector3D ConvertToPatient(Vector3D volumePosition)
		{
			// Set orientation transform
			Matrix volumePatientTransform = new Matrix(OrientationPatientMatrix);
			// Set origin translation
			volumePatientTransform.SetRow(3, OriginPatient.X, OriginPatient.Y, OriginPatient.Z, 1);

			// Transform volume position to patient position
			Matrix imagePositionMatrix = new Matrix(1, 4);
			imagePositionMatrix.SetRow(0, volumePosition.X, volumePosition.Y, volumePosition.Z, 1F);
			Matrix patientPositionMatrix = imagePositionMatrix*volumePatientTransform;

			Vector3D patientPosition = new Vector3D(patientPositionMatrix[0, 0], patientPositionMatrix[0, 1],
			                                        patientPositionMatrix[0, 2]);
			return patientPosition;
		}

		public Vector3D ConvertToVolume(Vector3D patientPosition)
		{
			// Set orientation transform
			Matrix patientVolumeTransform = new Matrix(OrientationPatientMatrix.Transpose());
			// Set origin translation
			Vector3D rotatedOrigin = RotateToVolumeOrientation(OriginPatient);
			patientVolumeTransform.SetRow(3, -rotatedOrigin.X, -rotatedOrigin.Y, -rotatedOrigin.Z, 1);

			// Transform patient position to volume position
			Matrix patientPositionMatrix = new Matrix(1, 4);
			patientPositionMatrix.SetRow(0, patientPosition.X, patientPosition.Y, patientPosition.Z, 1F);
			Matrix imagePositionMatrix = patientPositionMatrix*patientVolumeTransform;

			Vector3D imagePosition = new Vector3D(imagePositionMatrix[0, 0], imagePositionMatrix[0, 1],
			                                      imagePositionMatrix[0, 2]);
			return imagePosition;
		}

		public Matrix RotateToPatientOrientation(Matrix orientationVolume)
		{
			Matrix orientationPatient = orientationVolume*OrientationPatientMatrix;
			return orientationPatient;
		}

		public Matrix RotateToVolumeOrientation(Matrix orientationPatient)
		{
			Matrix orientationVolume = orientationPatient*OrientationPatientMatrix.Transpose();
			return orientationVolume;
		}

		public Vector3D RotateToPatientOrientation(Vector3D volumeVec)
		{
			Matrix volumePos = new Matrix(1, 4);
			volumePos.SetRow(0, volumeVec.X, volumeVec.Y, volumeVec.Z, 1F);
			Matrix patientPos = volumePos*OrientationPatientMatrix;
			return new Vector3D(patientPos[0, 0], patientPos[0, 1], patientPos[0, 2]);
		}

		public Vector3D RotateToVolumeOrientation(Vector3D patientVec)
		{
			Matrix patientPos = new Matrix(1, 4);
			patientPos.SetRow(0, patientVec.X, patientVec.Y, patientVec.Z, 1F);
			Matrix volumePos = patientPos*OrientationPatientMatrix.Transpose();
			return new Vector3D(volumePos[0, 0], volumePos[0, 1], volumePos[0, 2]);
		}

		#endregion

		#region Implementation

		internal IDicomAttributeProvider DataSet
		{
			get { return _modelDicom; }
		}

		// Decided to keep private for now, shouldn't be interesting to the outside world, and helps 
		//	avoid confusion with dimensions that take spacing into account (which is useful to the outside world)
		private Size3D ArrayDimensions
		{
			get { return _arrayDimensions; }
		}

		#region VTK volume wrapper

		private vtkImageData CreateVtkVolume()
		{
			vtkImageData vtkVolume = new vtkImageData();

			VtkHelper.RegisterVtkErrorEvents(vtkVolume);

			vtkVolume.SetDimensions(ArrayDimensions.Width, ArrayDimensions.Height, ArrayDimensions.Depth);
			vtkVolume.SetOrigin(Origin.X, Origin.Y, Origin.Z);
			vtkVolume.SetSpacing(VoxelSpacing.X, VoxelSpacing.Y, VoxelSpacing.Z);

			if (!this.Signed)
			{
				vtkVolume.SetScalarTypeToUnsignedShort();
				vtkVolume.GetPointData().SetScalars(
					VtkHelper.ConvertToVtkUnsignedShortArray(_volumeDataUInt16));
			}
			else
			{
				vtkVolume.SetScalarTypeToShort();
				vtkVolume.GetPointData().SetScalars(
					VtkHelper.ConvertToVtkShortArray(_volumeDataInt16));
			}

			// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
			vtkVolume.UpdateInformation();

			return vtkVolume;
		}

		/// <summary>
		/// Call to obtain a VTK volume structure that is safe for VTK to operate on. When done
		/// operating on the volume, call <see cref="ReleasePinnedVtkVolume"/>.
		/// </summary>
		/// <returns></returns>
		internal vtkImageData ObtainPinnedVtkVolume()
		{
			//TODO: Wrap with disposable object and have released on Dispose

			// Create the VTK volume wrapper if it doesn't exist
			if (_cachedVtkVolume == null)
				_cachedVtkVolume = CreateVtkVolume();

			// Pin the managed volume array. If not null, then already pinned so we do not re-pin.
			if (_volumeArrayPinnedHandle == null)
			{
				if (!this.Signed)
					_volumeArrayPinnedHandle = GCHandle.Alloc(_volumeDataUInt16, GCHandleType.Pinned);
				else
					_volumeArrayPinnedHandle = GCHandle.Alloc(_volumeDataInt16, GCHandleType.Pinned);
			}

			return _cachedVtkVolume;
		}

		internal void ReleasePinnedVtkVolume()
		{
			if (_volumeArrayPinnedHandle != null)
			{
				// Check for null avoids calling this twice
				_volumeArrayPinnedHandle.Value.Free();
				_volumeArrayPinnedHandle = null;
			}
		}

		#endregion

		#endregion

		#region Unit Test Accessors

#if UNIT_TESTS

		public Volume(short[] data, Size3D dimensions, Vector3D spacing, Vector3D originPatient, Matrix orientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue)
			: this(data, dimensions, spacing, originPatient, orientationPatient, new[] {attributeProvider}, paddingValue, null) {}

		public Volume(ushort[] data, Size3D dimensions, Vector3D spacing, Vector3D originPatient, Matrix orientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue)
			: this(data, dimensions, spacing, originPatient, orientationPatient, new[] {attributeProvider}, paddingValue, null) {}

		internal int this[int x, int y, int z]
		{
			get
			{
				if (!this.Contains(x,y,z))
					throw new ArgumentOutOfRangeException();
				if (this.Signed)
					return _volumeDataInt16[x + _arrayDimensions.Width*(y + _arrayDimensions.Height*z)];
				else
					return _volumeDataUInt16[x + _arrayDimensions.Width * (y + _arrayDimensions.Height * z)];
			}
		}

#endif

		#endregion

		#region Destructor and Disposal

		~Volume()
		{
			this.Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_cachedVtkVolume != null)
					{
						_cachedVtkVolume.GetPointData().Dispose();
						_cachedVtkVolume.Dispose();
						_cachedVtkVolume = null;
					}
				}

				// This should have been taken care of by caller of Obtain, release here just to be safe.
				ReleasePinnedVtkVolume();

				_volumeDataInt16 = null;
				_volumeDataUInt16 = null;

				_disposed = true;
			}
		}

		#endregion
	}
}