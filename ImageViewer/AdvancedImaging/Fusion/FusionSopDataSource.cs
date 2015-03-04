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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	internal class FusionSopDataSource : SopDataSource
	{
		private readonly ISopDataSource _realSopDataSource;
		private readonly DicomAttributeCollection _fusionHeaders;

		public FusionSopDataSource(ISopDataSource realSopDataSource, PETFusionType type, IEnumerable<IDicomAttributeProvider> overlayFrames)
		{
			_realSopDataSource = realSopDataSource;
		    _fusionHeaders = new DicomAttributeCollection() {ValidateVrLengths = false, ValidateVrValues = false};

			var scEquipment = new ScEquipmentModuleIod(_fusionHeaders);
			scEquipment.ConversionType = @"WSD";
			scEquipment.SecondaryCaptureDeviceManufacturer = @"ClearCanvas Inc.";
			scEquipment.SecondaryCaptureDeviceManufacturersModelName = ProductInformation.GetName(false, false);
			scEquipment.SecondaryCaptureDeviceSoftwareVersions = new[] {ProductInformation.GetVersion(true, true, true, true)};

			// generate values for the General Image Module
			_fusionHeaders[DicomTags.ImageType].SetStringValue(@"DERIVED\SECONDARY");
			_fusionHeaders[DicomTags.SourceImageSequence].Values = UpdateSourceImageSequence(realSopDataSource, overlayFrames);
			UpdateDerivationType(type);
		}

		public override bool IsStored
		{
			get { return false; }
		}

		public override bool IsImage
		{
			get { return _realSopDataSource.IsImage; }
		}

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				DicomAttribute dicomAttribute;
				return _fusionHeaders.TryGetAttribute(tag, out dicomAttribute) ? dicomAttribute : _realSopDataSource[tag];
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute dicomAttribute;
				return _fusionHeaders.TryGetAttribute(tag, out dicomAttribute) ? dicomAttribute : _realSopDataSource[tag];
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return _fusionHeaders.TryGetAttribute(tag, out attribute) || _realSopDataSource.TryGetAttribute(tag, out attribute);
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			return _fusionHeaders.TryGetAttribute(tag, out attribute) || _realSopDataSource.TryGetAttribute(tag, out attribute);
		}

		protected override ISopFrameData GetFrameData(int frameNumber)
		{
			return new FrameData(_realSopDataSource.GetFrameData(frameNumber), this);
		}

		private static DicomSequenceItem[] UpdateSourceImageSequence(IDicomAttributeProvider baseSop, IEnumerable<IDicomAttributeProvider> overlaySops)
		{
			var baseItem = new SourceImageSequence
			               	{
			               		ReferencedSopInstanceUid = baseSop[DicomTags.SopInstanceUid].ToString(),
			               		ReferencedSopClassUid = baseSop[DicomTags.SopClassUid].ToString(),
			               		SpatialLocationsPreserved = SpatialLocationsPreserved.Yes,
			               		PurposeOfReferenceCodeSequence = SourceImagePurposesOfReferenceContextGroup.SourceImageForImageProcessingOperation
			               	};
			var maskItems = overlaySops != null ? overlaySops.GroupBy(s => s[DicomTags.SopInstanceUid].ToString())
			                                      	.Select(s => s.First())
			                                      	.Select(s => new SourceImageSequence
			                                      	             	{
			                                      	             		ReferencedSopInstanceUid = s[DicomTags.SopInstanceUid].ToString(),
			                                      	             		ReferencedSopClassUid = s[DicomTags.SopClassUid].ToString(),
			                                      	             		SpatialLocationsPreserved = SpatialLocationsPreserved.Yes,
			                                      	             		PurposeOfReferenceCodeSequence = SourceImagePurposesOfReferenceContextGroup.SourceImageForImageProcessingOperation
			                                      	             	}) : Enumerable.Empty<SourceImageSequence>();
			return new[] {baseItem}.Concat(maskItems).Select(s => s.DicomSequenceItem).ToArray();
		}

		private void UpdateDerivationType(PETFusionType type)
		{
			switch (type)
			{
				case PETFusionType.CT:
					const string ctPetFusion = "PET/CT Fusion (original CT image with a PET overlay frame extracted from a volume)";
					_fusionHeaders[DicomTags.ImageComments].SetStringValue(ctPetFusion);
					_fusionHeaders[DicomTags.DerivationDescription].SetStringValue(ctPetFusion);
					_fusionHeaders[DicomTags.DerivationCodeSequence].Values = new[] {ImageDerivationContextGroup.SpatiallyRelatedFramesExtractedFromTheVolume.AsDicomSequenceItem()};
					break;
				case PETFusionType.MR:
					const string mrPetFusion = "PET/MR Fusion (original MR image with a PET overlay frame extracted from a volume)";
					_fusionHeaders[DicomTags.ImageComments].SetStringValue(mrPetFusion);
					_fusionHeaders[DicomTags.DerivationDescription].SetStringValue(mrPetFusion);
					_fusionHeaders[DicomTags.DerivationCodeSequence].Values = new[] {ImageDerivationContextGroup.SpatiallyRelatedFramesExtractedFromTheVolume.AsDicomSequenceItem()};
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		private class FrameData : SopFrameData
		{
			private readonly ISopFrameData _realFrameData;

			public FrameData(ISopFrameData realFrameData, SopDataSource parent)
				: base(realFrameData.FrameNumber, parent)
			{
				_realFrameData = realFrameData;
			}

			public override byte[] GetNormalizedPixelData()
			{
				return _realFrameData.GetNormalizedPixelData();
			}

			public override byte[] GetNormalizedOverlayData(int overlayNumber)
			{
				return _realFrameData.GetNormalizedOverlayData(overlayNumber);
			}

			public override void Unload()
			{
				_realFrameData.Unload();
			}
		}
	}
}