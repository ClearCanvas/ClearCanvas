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
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Mathematics;

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
		private readonly DicomAttributeCollection _collection = new DicomAttributeCollection();
		private readonly Matrix _volumeOrientationPatient;

		public VolumeHeaderData(IList<IDicomAttributeProvider> sourceSops,
		                    Size3D arrayDimensions,
		                    Vector3D voxelSpacing,
		                    Vector3D volumePositionPatient,
		                    Matrix volumeOrientationPatient,
		                    int bitsAllocated,
		                    int bitsStored,
		                    bool isSigned,
		                    int paddingValue,
		                    double rescaleSlope,
		                    double rescaleIntercept)
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
			SourceSeriesInstanceUid = firstSop[DicomTags.SeriesInstanceUid].ToString();
			FrameOfReferenceUid = firstSop[DicomTags.FrameOfReferenceUid].ToString();
			BitsPerVoxel = bitsAllocated;
			Signed = isSigned;
			PaddingValue = paddingValue;
			RescaleSlope = rescaleSlope;
			RescaleIntercept = rescaleIntercept;
			VolumeSize = new Vector3D(ArrayDimensions.Width*VoxelSpacing.X, ArrayDimensions.Height*VoxelSpacing.Y, ArrayDimensions.Depth*VoxelSpacing.Z);
			VolumeBounds = new Rectangle3D(new Vector3D(0, 0, 0), VolumeSize);
			VolumeCenter = 0.5f*VolumeBounds.Size;
			VolumeCenterPatient = ConvertToPatient(VolumeCenter);

			// populate the DICOM data set
			FillDataSet(_collection, sourceSops, bitsAllocated, bitsStored, isSigned, rescaleSlope, rescaleIntercept);
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
		public readonly string SourceSeriesInstanceUid;
		public readonly string FrameOfReferenceUid;
		public readonly double RescaleSlope;
		public readonly double RescaleIntercept;

		#region IVolumeDataSet Members

		public IEnumerable<uint> Tags
		{
			get { return _collection.Select(a => a.Tag.TagValue); }
		}

		public IEnumerable<DicomAttribute> Attributes
		{
			get { return _collection.Select(a => a); }
		}

		public DicomAttribute this[DicomTag tag]
		{
			get { return _collection[tag]; }
			set { _collection[tag] = value; }
		}

		public DicomAttribute this[uint tag]
		{
			get { return _collection[tag]; }
			set { _collection[tag] = value; }
		}

		public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			return _collection.TryGetAttribute(tag, out attribute);
		}

		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return _collection.TryGetAttribute(tag, out attribute);
		}

		public DicomAttributeCollection Copy()
		{
			var collection = new DicomAttributeCollection();
			CopyTo(collection);
			return collection;
		}

		public void CopyTo(IDicomAttributeProvider destinationCollection)
		{
			foreach (var attribute in _collection)
				destinationCollection[attribute.Tag] = attribute.Copy();
		}

		#endregion

		#region Coordinate Transforms

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

		public Matrix RotateToPatientOrientation(Matrix volumeOrientation)
		{
			var orientationPatient = volumeOrientation*_volumeOrientationPatient;
			return orientationPatient;
		}

		public Matrix RotateToVolumeOrientation(Matrix patientOrientation)
		{
			var orientationVolume = patientOrientation*_volumeOrientationPatient.Transpose();
			return orientationVolume;
		}

		public Vector3D RotateToPatientOrientation(Vector3D volumeVector)
		{
			var volumePos = new Matrix(1, 4);
			volumePos.SetRow(0, volumeVector.X, volumeVector.Y, volumeVector.Z, 1F);
			Matrix patientPos = volumePos*_volumeOrientationPatient;
			return new Vector3D(patientPos[0, 0], patientPos[0, 1], patientPos[0, 2]);
		}

		public Vector3D RotateToVolumeOrientation(Vector3D patientVector)
		{
			var patientPos = new Matrix(1, 4);
			patientPos.SetRow(0, patientVector.X, patientVector.Y, patientVector.Z, 1F);
			var volumePos = patientPos*_volumeOrientationPatient.Transpose();
			return new Vector3D(volumePos[0, 0], volumePos[0, 1], volumePos[0, 2]);
		}

		#endregion

		#region DataSet Helpers

		private static void FillDataSet(IDicomAttributeProvider volumeDataSet, IList<IDicomAttributeProvider> sourceSops, int bitsAllocated, int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
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
			volumeDataSet[DicomTags.FrameOfReferenceUid] = source[DicomTags.FrameOfReferenceUid].Copy();

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
			// TODO: there's a SourceImageSequence here that we should probably fill out 

			// generate volume-consistent values for the Image Pixel Module
			volumeDataSet[DicomTags.SamplesPerPixel] = source[DicomTags.SamplesPerPixel].Copy();
			volumeDataSet[DicomTags.PhotometricInterpretation] = source[DicomTags.PhotometricInterpretation].Copy();
			volumeDataSet[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated);
			volumeDataSet[DicomTags.BitsStored].SetInt32(0, bitsStored);
			volumeDataSet[DicomTags.HighBit].SetInt32(0, bitsStored - 1);
			volumeDataSet[DicomTags.PixelRepresentation].SetInt32(0, isSigned ? 1 : 0);
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

		internal static VolumeHeaderData TestCreate(IList<IDicomAttributeProvider> sourceSops, int bitsAllocated = 16, int bitsStored = 16, bool isSigned = false)
		{
			return new VolumeHeaderData(sourceSops, new Size3D(0, 0, 0), new Vector3D(0, 0, 0), new Vector3D(0, 0, 0), new Matrix(4, 4), bitsAllocated, bitsStored, isSigned, DicomPixelData.GetMinPixelValue(bitsAllocated, isSigned), 1, 0);
		}

#endif

		#endregion
	}
}