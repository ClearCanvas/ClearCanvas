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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;

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

	partial class Volume
	{
		/// <summary>
		/// The SOP data source prototype object that will be shared between actual SOPs
		/// </summary>
		/// <remarks>
		/// For now, we will have this prototype shared between each SOP (a single slice of any plane)
		/// Ideally, we should have a single multiframe SOP for each plane, and then this prototype is shared between those SOPs
		/// </remarks>
		internal class VolumeSopDataSourcePrototype : IVolumeDataSet
		{
			private readonly DicomAttributeCollection _collection = new DicomAttributeCollection();

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

			public static VolumeSopDataSourcePrototype Create(IList<IDicomAttributeProvider> sourceSops, int bitsAllocated, int bitsStored, bool isSigned)
			{
				const string enumYes = "YES";
				const string enumNo = "NO";
				const string enumLossy = "01";
				const string enumLossless = "00";

				var prototype = new VolumeSopDataSourcePrototype();
				var volumeDataSet = prototype._collection;
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

				return prototype;
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
		}

		#region Unit Test Support

#if UNIT_TESTS

		internal static IDicomAttributeProvider TestCreateSopDataSourcePrototype(IList<IDicomAttributeProvider> sourceSops, int bitsAllocated = 16, int bitsStored = 16, bool isSigned = false)
		{
			return VolumeSopDataSourcePrototype.Create(sourceSops, bitsAllocated, bitsStored, isSigned);
		}

#endif

		#endregion
	}
}