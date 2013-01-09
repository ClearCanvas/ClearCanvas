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

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	partial class Volume
	{
		/// <summary>
		/// The SOP data source prototype object that will be shared between actual SOPs
		/// </summary>
		/// <remarks>
		/// For now, we will have this prototype shared between each SOP (a single slice of any plane)
		/// Ideally, we should have a single multiframe SOP for each plane, and then this prototype is shared between those SOPs
		/// </remarks>
		private class VolumeSopDataSourcePrototype : IDicomAttributeProvider
		{
			private readonly DicomAttributeCollection _collection = new DicomAttributeCollection();

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

			public static VolumeSopDataSourcePrototype Create(IDicomAttributeProvider source, int bitsAllocated, int bitsStored, bool isSigned)
			{
				VolumeSopDataSourcePrototype prototype = new VolumeSopDataSourcePrototype();
				DicomAttributeCollection volumeDataSet = prototype._collection;

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

				// generate SC Equipment Module
				var scEquipment = new ScEquipmentModuleIod(volumeDataSet);
				scEquipment.ConversionType = @"WSD";
				scEquipment.SecondaryCaptureDeviceManufacturer = @"ClearCanvas Inc.";
				scEquipment.SecondaryCaptureDeviceManufacturersModelName = ProductInformation.GetName(true, false);
				scEquipment.SecondaryCaptureDeviceSoftwareVersions = new[] {ProductInformation.GetVersion(true, true, true)};

				// generate common values for the General Image Module
				volumeDataSet[DicomTags.ImageType].SetStringValue(@"DERIVED\SECONDARY");
				volumeDataSet[DicomTags.PixelSpacing] = source[DicomTags.PixelSpacing].Copy();
				volumeDataSet[DicomTags.FrameOfReferenceUid] = source[DicomTags.FrameOfReferenceUid].Copy();

				// generate common values for the Image Pixel Module
				volumeDataSet[DicomTags.SamplesPerPixel] = source[DicomTags.SamplesPerPixel].Copy();
				volumeDataSet[DicomTags.PhotometricInterpretation] = source[DicomTags.PhotometricInterpretation].Copy();
				volumeDataSet[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated);
				volumeDataSet[DicomTags.BitsStored].SetInt32(0, bitsStored);
				volumeDataSet[DicomTags.HighBit].SetInt32(0, bitsStored - 1);
				volumeDataSet[DicomTags.PixelRepresentation].SetInt32(0, isSigned ? 1 : 0);

				// generate common values for the SOP Common Module
				volumeDataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);

				return prototype;
			}

			private static IList<uint> _seriesModuleTags;

			private static IEnumerable<uint> ModalitySpecificSeriesModuleTags
			{
				get
				{
					if (_seriesModuleTags == null)
					{
						// only need to list modules for logically reformattable IODs (e.g. CT, MR, NM, PET)
						// and enhanced/tomography versions of nominally non-reformattable modalities (e.g. Enh US, Enh MG)
						_seriesModuleTags = EnhancedSeriesModuleIod.DefinedTags
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
							.Distinct()
							.Except(new[] {DicomTags.LargestPixelValueInSeries, DicomTags.SmallestPixelValueInSeries, DicomTags.SeriesInstanceUid})
							.ToList().AsReadOnly();
					}
					return _seriesModuleTags;
				}
			}
		}
	}
}