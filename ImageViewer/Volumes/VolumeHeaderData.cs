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

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Represents the DICOM data set common to the volume.
	/// </summary>
	public interface IVolumeDataSet : IDicomAttributeProvider
	{
		/// <summary>
		/// Enumerates the DICOM tags in the data set.
		/// </summary>
		IEnumerable<uint> Tags { get; }

		/// <summary>
		/// Enumerates the DICOM attributes in the data set.
		/// </summary>
		IEnumerable<DicomAttribute> Attributes { get; }

		/// <summary>
		/// Creates a deep copy of the contents of the data set.
		/// </summary>
		/// <returns>A new <see cref="DicomAttributeCollection"/> containing the contents of the data set.</returns>
		DicomAttributeCollection Copy();

		/// <summary>
		/// Copies the contents of the data set to another collection.
		/// </summary>
		/// <param name="destinationCollection">The destination collection.</param>
		void CopyTo(IDicomAttributeProvider destinationCollection);
	}

	/// <summary>
	/// Represents the common header data of the <see cref="Volume"/>.
	/// </summary>
	/// <remarks>
	/// This data set contains data from the patient, study and series level IEs that are consistent
	/// throughout the volume, as well as various other volume-specific properties that can be computed
	/// once for the volume.
	/// </remarks>
	internal sealed class VolumeHeaderData : IVolumeDataSet
	{
		private readonly object _syncRoot = new object();

	    private readonly DicomAttributeCollection _collection = new DicomAttributeCollection()
	    {
	        ValidateVrLengths = false,
	        ValidateVrValues = false
	    };

		private readonly Matrix3D _volumeOrientationPatient;

		public VolumeHeaderData(IList<IDicomAttributeProvider> sourceSops,
		                        Size3D arrayDimensions,
		                        Vector3D voxelSpacing,
		                        Vector3D volumePositionPatient,
		                        Matrix3D volumeOrientationPatient,
		                        int bitsAllocated,
		                        int bitsStored,
		                        bool isSigned,
		                        int paddingValue,
		                        double rescaleSlope,
		                        double rescaleIntercept,
		                        RescaleUnits rescaleUnits,
		                        string laterality = null)
		{
			Platform.CheckForNullReference(sourceSops, "sourceSops");
			Platform.CheckTrue(sourceSops.Count > 0, "At least one sourceSop is required");
			Platform.CheckForNullReference(arrayDimensions, "arrayDimensions");
			Platform.CheckForNullReference(voxelSpacing, "voxelSpacing");
			Platform.CheckForNullReference(volumePositionPatient, "originPatient");
			Platform.CheckForNullReference(volumeOrientationPatient, "orientationPatient");

			_volumeOrientationPatient = volumeOrientationPatient;

			var firstSop = sourceSops[0];
			ArrayDimensions = arrayDimensions;
			VoxelSpacing = voxelSpacing;
			VolumePositionPatient = volumePositionPatient;
			VolumeOrientationPatientX = volumeOrientationPatient.GetRow(0);
			VolumeOrientationPatientY = volumeOrientationPatient.GetRow(1);
			VolumeOrientationPatientZ = volumeOrientationPatient.GetRow(2);
			Modality = firstSop[DicomTags.Modality].ToString();
			SourceStudyInstanceUid = firstSop[DicomTags.StudyInstanceUid].ToString();
			SourceSeriesInstanceUid = firstSop[DicomTags.SeriesInstanceUid].ToString();
			FrameOfReferenceUid = firstSop[DicomTags.FrameOfReferenceUid].ToString();
			BitsPerVoxel = bitsAllocated;
			Signed = isSigned;
			PaddingValue = paddingValue;
			RescaleSlope = rescaleSlope;
			RescaleIntercept = rescaleIntercept;
			RescaleUnits = rescaleUnits ?? RescaleUnits.None;
			Laterality = laterality ?? string.Empty;
			VolumeSize = new Vector3D(ArrayDimensions.Width*VoxelSpacing.X, ArrayDimensions.Height*VoxelSpacing.Y, ArrayDimensions.Depth*VoxelSpacing.Z);
			VolumeBounds = new Rectangle3D(new Vector3D(0, 0, 0), VolumeSize);
			VolumeCenter = 0.5f*VolumeBounds.Size;
			VolumeCenterPatient = ConvertToPatient(VolumeCenter);

			// populate the DICOM data set
			FillDataSet(_collection, sourceSops, bitsAllocated, bitsStored, isSigned, rescaleSlope, rescaleIntercept, laterality);
		}

		public readonly Size3D ArrayDimensions;
		public readonly Vector3D VolumeSize;
		public readonly Rectangle3D VolumeBounds;
		public readonly Vector3D VoxelSpacing;
		public readonly Vector3D VolumePositionPatient;
		public readonly Vector3D VolumeOrientationPatientX;
		public readonly Vector3D VolumeOrientationPatientY;
		public readonly Vector3D VolumeOrientationPatientZ;
		public readonly Vector3D VolumeCenter;
		public readonly Vector3D VolumeCenterPatient;
		public readonly int BitsPerVoxel;
		public readonly bool Signed;
		public readonly int PaddingValue;
		public readonly string Modality;
		public readonly string SourceStudyInstanceUid;
		public readonly string SourceSeriesInstanceUid;
		public readonly string FrameOfReferenceUid;
		public readonly double RescaleSlope;
		public readonly double RescaleIntercept;
		public readonly RescaleUnits RescaleUnits;
		public readonly string Laterality;

		#region IVolumeDataSet Members

		public IEnumerable<uint> Tags
		{
			get
			{
				lock (_syncRoot)
				{
					return _collection.Select(a => a.Tag.TagValue).ToList();
				}
			}
		}

		public IEnumerable<DicomAttribute> Attributes
		{
			get
			{
				lock (_syncRoot)
				{
					return _collection.Select(a => a).ToList();
				}
			}
		}

		public DicomAttribute this[DicomTag tag]
		{
			get
			{
				lock (_syncRoot)
				{
					return _collection[tag];
				}
			}
			set
			{
				lock (_syncRoot)
				{
					_collection[tag] = value;
				}
			}
		}

		public DicomAttribute this[uint tag]
		{
			get
			{
				lock (_syncRoot)
				{
					return _collection[tag];
				}
			}
			set
			{
				lock (_syncRoot)
				{
					_collection[tag] = value;
				}
			}
		}

		public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (_syncRoot)
			{
				return _collection.TryGetAttribute(tag, out attribute);
			}
		}

		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock (_syncRoot)
			{
				return _collection.TryGetAttribute(tag, out attribute);
			}
		}

		public DicomAttributeCollection Copy()
		{
		    var collection = new DicomAttributeCollection() {ValidateVrLengths = false, ValidateVrValues = false};
			CopyTo(collection);
			return collection;
		}

		public void CopyTo(IDicomAttributeProvider destinationCollection)
		{
			lock (_syncRoot)
			{
				foreach (var attribute in _collection)
					destinationCollection[attribute.Tag] = attribute.Copy();
			}
		}

		#endregion

		#region Coordinate Transforms

		public Vector3D ConvertToPatient(Vector3D volumePosition)
		{
			Platform.CheckForNullReference(volumePosition, "volumePosition");

			// Set orientation transform
			var volumePatientTransform = _volumeOrientationPatient.Augment();

			// Set origin translation
			volumePatientTransform.SetRow(3, VolumePositionPatient.X, VolumePositionPatient.Y, VolumePositionPatient.Z, 1);

			// Transform volume position to patient position
			var imagePositionMatrix = new Matrix(1, 4);
			imagePositionMatrix.SetRow(0, volumePosition.X, volumePosition.Y, volumePosition.Z, 1F);
			var patientPositionMatrix = imagePositionMatrix*volumePatientTransform;

			var patientPosition = new Vector3D(patientPositionMatrix[0, 0], patientPositionMatrix[0, 1], patientPositionMatrix[0, 2]);
			return patientPosition;
		}

		public Vector3D ConvertToVolume(Vector3D patientPosition)
		{
			Platform.CheckForNullReference(patientPosition, "patientPosition");

			// Set orientation transform
			var patientVolumeTransform = _volumeOrientationPatient.Augment(true);

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

		public Matrix RotateToPatientOrientation(Matrix volumeOrientation)
		{
			Platform.CheckForNullReference(volumeOrientation, "volumeOrientation");

			var size = volumeOrientation.Rows;
			Platform.CheckTrue(volumeOrientation.IsSquare && (size == 3 || size == 4), "volumeOrientation must be a 3x3 orientation matrix or a 4x4 affine transmation matrix");

			var orientationPatient = size == 3 ? volumeOrientation*_volumeOrientationPatient : volumeOrientation*_volumeOrientationPatient.Augment();
			return orientationPatient;
		}

		public Matrix RotateToVolumeOrientation(Matrix patientOrientation)
		{
			Platform.CheckForNullReference(patientOrientation, "patientOrientation");

			var size = patientOrientation.Rows;
			Platform.CheckTrue(patientOrientation.IsSquare && (size == 3 || size == 4), "patientOrientation must be a 3x3 orientation matrix or a 4x4 affine transmation matrix");

			var orientationVolume = size == 3 ? patientOrientation*_volumeOrientationPatient.Transpose() : patientOrientation*_volumeOrientationPatient.Augment(true);
			return orientationVolume;
		}

		public Matrix3D RotateToPatientOrientation(Matrix3D volumeOrientation)
		{
			Platform.CheckForNullReference(volumeOrientation, "volumeOrientation");

			var orientationPatient = volumeOrientation*_volumeOrientationPatient;
			return orientationPatient;
		}

		public Matrix3D RotateToVolumeOrientation(Matrix3D patientOrientation)
		{
			Platform.CheckForNullReference(patientOrientation, "patientOrientation");

			var orientationVolume = patientOrientation*_volumeOrientationPatient.Transpose();
			return orientationVolume;
		}

		public Vector3D RotateToPatientOrientation(Vector3D volumeVector)
		{
			Platform.CheckForNullReference(volumeVector, "volumeVector");

			var patientPos = volumeVector*_volumeOrientationPatient;
			return patientPos;
		}

		public Vector3D RotateToVolumeOrientation(Vector3D patientVector)
		{
			Platform.CheckForNullReference(patientVector, "patientVector");

			var volumePos = patientVector*_volumeOrientationPatient.Transpose();
			return volumePos;
		}

		#endregion

		#region DataSet Helpers

		private static void FillDataSet(IDicomAttributeProvider volumeDataSet, IList<IDicomAttributeProvider> sourceSops, int bitsAllocated, int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept, string laterality)
		{
			const string enumYes = "YES";
			const string enumNo = "NO";
			const string enumLossy = "01";
			const string enumLossless = "00";

			var source = sourceSops[0];

			// perform exact copy on the Patient Module
			foreach (uint tag in PatientModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on the Clinical Trial Subject Module
			foreach (uint tag in ClinicalTrialSubjectModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on the General Study Module
			foreach (uint tag in GeneralStudyModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on the Patient Study Module
			foreach (uint tag in PatientStudyModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on the Clinical Trial Study Module
			foreach (uint tag in ClinicalTrialStudyModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on the General Series Module except for tags that will be overridden as part of reformatting
			foreach (uint tag in GeneralSeriesModuleIod.DefinedTags.Except(new[] {DicomTags.LargestPixelValueInSeries, DicomTags.SmallestPixelValueInSeries, DicomTags.SeriesInstanceUid}))
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on the Clinical Trial Series Module
			foreach (uint tag in ClinicalTrialSeriesModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on additional modality specific modules in the series IE
			foreach (uint tag in ModalitySpecificSeriesModuleTags)
				volumeDataSet[tag] = source[tag].Copy();

			// perform exact copy on the General Equipment Module
			foreach (uint tag in GeneralEquipmentModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// copy volume-consistent values for the Frame of Reference Module
			foreach (uint tag in FrameOfReferenceModuleIod.DefinedTags)
				volumeDataSet[tag] = source[tag].Copy();

			// update computed normalized values for the Modality LUT Module
			volumeDataSet[DicomTags.RescaleSlope].SetFloat64(0, rescaleSlope);
			volumeDataSet[DicomTags.RescaleIntercept].SetFloat64(0, rescaleIntercept);
			volumeDataSet[DicomTags.RescaleType] = source[DicomTags.RescaleType].Copy();
			volumeDataSet[DicomTags.Units] = source[DicomTags.Units].Copy(); // PET series use this attribute to designate rescale units

			// generate volume-consistent values for the General Image Module
			var burnedInAnnotationValues = sourceSops.Select(s => s[DicomTags.BurnedInAnnotation].GetBoolean(0, enumYes, enumNo)).ToList();
			var burnedInAnnotation = burnedInAnnotationValues.Any(v => v.GetValueOrDefault(false)) ? true : (burnedInAnnotationValues.All(v => !v.GetValueOrDefault(true)) ? false : (bool?) null);
			var recognizableVisualFeaturesValues = sourceSops.Select(s => s[DicomTags.RecognizableVisualFeatures].GetBoolean(0, enumYes, enumNo)).ToList();
			var recognizableVisualFeatures = recognizableVisualFeaturesValues.Any(v => v.GetValueOrDefault(false)) ? true : (recognizableVisualFeaturesValues.All(v => !v.GetValueOrDefault(true)) ? false : (bool?) null);
			var lossyImageCompressionValues = sourceSops.Select(s => s[DicomTags.LossyImageCompression].GetBoolean(0, enumLossy, enumLossless)).ToList();
			var lossyImageCompression = lossyImageCompressionValues.Any(v => v.GetValueOrDefault(false)) ? true : (lossyImageCompressionValues.All(v => !v.GetValueOrDefault(true)) ? false : (bool?) null);
			var lossyImageCompressionRatioValues = sourceSops.Select(s => s[DicomTags.LossyImageCompressionRatio].GetFloat32(0, 0)).ToList();
			var lossyImageCompressionRatio = lossyImageCompressionRatioValues.Max();
			volumeDataSet[DicomTags.BurnedInAnnotation].SetBoolean(0, burnedInAnnotation, enumYes, enumNo);
			volumeDataSet[DicomTags.RecognizableVisualFeatures].SetBoolean(0, recognizableVisualFeatures, enumYes, enumNo);
			volumeDataSet[DicomTags.LossyImageCompression].SetBoolean(0, lossyImageCompression, enumLossy, enumLossless);
			if (lossyImageCompressionRatio > 0)
				volumeDataSet[DicomTags.LossyImageCompressionRatio].SetFloat32(0, lossyImageCompressionRatio);

			// record the source images for the General Image Module
			volumeDataSet[DicomTags.SourceImageSequence].Values = CreateSourceImageSequence(sourceSops);

			// generate volume-consistent values for the Image Pixel Module
			volumeDataSet[DicomTags.SamplesPerPixel] = source[DicomTags.SamplesPerPixel].Copy();
			volumeDataSet[DicomTags.PhotometricInterpretation] = source[DicomTags.PhotometricInterpretation].Copy();
			volumeDataSet[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated);
			volumeDataSet[DicomTags.BitsStored].SetInt32(0, bitsStored);
			volumeDataSet[DicomTags.HighBit].SetInt32(0, bitsStored - 1);
			volumeDataSet[DicomTags.PixelRepresentation].SetInt32(0, isSigned ? 1 : 0);

			// fill values for additional modality specific modules in the image IE
			if (!string.IsNullOrEmpty(laterality))
				volumeDataSet[DicomTags.ImageLaterality].SetStringValue(laterality);
		}

		private static DicomSequenceItem[] CreateSourceImageSequence(IEnumerable<IDicomAttributeProvider> sourceSops)
		{
			return sourceSops.GroupBy(s => s[DicomTags.SopInstanceUid].ToString())
				.Select(s => s.First())
				.Select(s => new SourceImageSequence
				             	{
				             		ReferencedSopInstanceUid = s[DicomTags.SopInstanceUid].ToString(),
				             		ReferencedSopClassUid = s[DicomTags.SopClassUid].ToString(),
				             		SpatialLocationsPreserved = SpatialLocationsPreserved.Yes,
				             		PurposeOfReferenceCodeSequence = new CodeSequenceMacro
				             		                                 	{
				             		                                 		CodingSchemeDesignator = "DCM",
				             		                                 		CodeValue = "121322",
				             		                                 		CodeMeaning = "Source image for image processing operation"
				             		                                 	}
				             	})
				.Select(s => s.DicomSequenceItem).ToArray();
		}

		/// <summary>
		/// Gets a list of modality-specific series-level tags that would logically be volume-consistent.
		/// </summary>
		internal static readonly IList<uint> ModalitySpecificSeriesModuleTags
			// only need to list modules for cross sectional modality IODs (e.g. CT, MR, NM, PET)
			// and enhanced/tomography versions of typically projectional modalities (e.g. Enh US, Enh MG)
			= EnhancedSeriesModuleIod.DefinedTags
				.Union(EnhancedPetSeriesModuleIod.DefinedTags)
				.Union(EnhancedUsSeriesModuleIod.DefinedTags)
				.Union(CtSeriesModuleIod.DefinedTags)
				.Union(EnhancedMammographySeriesModuleIod.DefinedTags)
				.Union(OpthalmicTomographySeriesModuleIod.DefinedTags)
				.Union(MrSeriesModuleIod.DefinedTags)
				.Union(NmPetPatientOrientationModuleIod.DefinedTags)
				.Union(PetSeriesModuleIod.DefinedTags)
				.Union(PetIsotopeModuleIod.DefinedTags)
				.Union(PetMultiGatedAcquisitionModuleIod.DefinedTags)
				.Union(XaXrfSeriesModuleIod.DefinedTags)
				.Except(new[] {DicomTags.LargestPixelValueInSeries, DicomTags.SmallestPixelValueInSeries, DicomTags.SeriesInstanceUid})
				.ToList().AsReadOnly();

		#endregion

		#region Unit Test Support

#if UNIT_TESTS

		internal static VolumeHeaderData TestCreate(IList<IDicomAttributeProvider> sourceSops, int bitsAllocated = 16, int bitsStored = 16, bool isSigned = false, string laterality = null)
		{
			return new VolumeHeaderData(sourceSops, new Size3D(0, 0, 0), new Vector3D(0, 0, 0), new Vector3D(0, 0, 0), new Matrix3D(), bitsAllocated, bitsStored, isSigned, DicomPixelData.GetMinPixelValue(bitsAllocated, isSigned), 1, 0, null, laterality);
		}

#endif

		#endregion
	}
}