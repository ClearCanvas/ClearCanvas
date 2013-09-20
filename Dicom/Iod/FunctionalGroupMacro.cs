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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.FunctionalGroups;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Functional Group Macro
	/// </summary>
	public abstract class FunctionalGroupMacro : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalGroupMacro"/> class.
		/// </summary>
		protected FunctionalGroupMacro() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalGroupMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		protected FunctionalGroupMacro(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Enumerates the <see cref="DicomTag"/>s used by this functional group macro.
		/// </summary>
		public abstract IEnumerable<uint> DefinedTags { get; }

		/// <summary>
		/// Enumerates the nested <see cref="DicomTag"/>s used in the sequence attribute defined by this functional group macro.
		/// </summary>
		public abstract IEnumerable<uint> NestedTags { get; }

		/// <summary>
		/// Gets a value indicating whether or not the sequence attribute used by this functional group macro can potentially have multiple (more than one) sequence item.
		/// </summary>
		public virtual bool CanHaveMultipleItems
		{
			get { return false; }
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public abstract void InitializeAttributes();

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public virtual bool HasValues()
		{
			return DefinedTags.Any(t =>
			                       	{
			                       		DicomAttribute a;
			                       		return DicomAttributeProvider.TryGetAttribute(t, out a) && !a.IsEmpty;
			                       	});
		}

		/// <summary>
		/// Gets the singleton sequence item in the functional group sequence if the functional group cannot have multiple items. Returns NULL otherwise.
		/// </summary>
		public DicomSequenceItem SingleItem
		{
			get
			{
				DicomAttribute a;
				return !CanHaveMultipleItems && DicomAttributeProvider.TryGetAttribute(DefinedTags.Single(), out a) && a.Count > 0 ? ((DicomAttributeSQ) a)[0] : null;
			}
		}

		#region Functional Group by SOP Class/Tag

		/// <summary>
		/// The tag to functional group mapping by SOP class.
		/// </summary>
		private static readonly Dictionary<string, IDictionary<uint, Type>> _functionalGroupTagMap = new Dictionary<string, IDictionary<uint, Type>>();

		/// <summary>
		/// Gets the functional group in which the specified DICOM tag is uniquely used for a given SOP class.
		/// </summary>
		/// <remarks>
		/// This method will return only common, non-modality-specific functional groups if the SOP class is not multi-frame class by the standard that uses functional groups defined by the standard.
		/// </remarks>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="dicomTag">The DICOM tag.</param>
		/// <returns></returns>
		public static Type GetFunctionalGroupByTag(string sopClassUid, uint dicomTag)
		{
			// lookup the requested tag in the tag map, and return the type of the functional group that defines it - or NULL if it's not defined at all
			Type functionalGroupType;
			return GetFunctionalGroupMap(sopClassUid).TryGetValue(dicomTag, out functionalGroupType) ? functionalGroupType : null;
		}

		/// <summary>
		/// Gets a dictionary mapping tags to functional groups in which they are uniquely used for a given SOP class.
		/// </summary>
		/// <remarks>
		/// This method will return only common, non-modality-specific functional groups if the SOP class is not multi-frame class by the standard that uses functional groups defined by the standard.
		/// </remarks>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <returns></returns>
		public static IDictionary<uint, Type> GetFunctionalGroupMap(string sopClassUid)
		{
			// normalize the SOP class UID
			if (sopClassUid == null || !_functionalGroupUsage.ContainsKey(sopClassUid)) sopClassUid = string.Empty;

			// get the tag map for the indicated SOP class - building the tag map now if necessary
			IDictionary<uint, Type> tagMap;
			if (!_functionalGroupTagMap.TryGetValue(sopClassUid, out tagMap))
			{
				_functionalGroupTagMap[sopClassUid] = tagMap = GetApplicableFunctionalGroups(sopClassUid)
				                                               	.Select(t => (FunctionalGroupMacro) Activator.CreateInstance(t))
				                                               	.Where(f => !f.CanHaveMultipleItems) // if the sequence can have multiple items, then you can't have a 1-1 mapping with a frame
				                                               	.SelectMany(f => f.NestedTags.Select(g => new KeyValuePair<uint, Type>(g, f.GetType())))
				                                               	.GroupBy(k => k.Key).Select(g => g.First()) // group by tag, and prioritize the first functional group that defines it
				                                               	.ToDictionary(k => k.Key, k => k.Value).AsReadOnly();
			}
			return tagMap;
		}

		#endregion

		#region Functional Group Usage by SOP Class

		/// <summary>
		/// Enumerates the functional groups that are applicable to the specified SOP class.
		/// </summary>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetApplicableFunctionalGroups(string sopClassUid)
		{
			Type[] functionalGroups;
			return _functionalGroupUsage.TryGetValue(sopClassUid ?? string.Empty, out functionalGroups) ? functionalGroups : _functionalGroupUsage[string.Empty];
		}

		/// <summary>
		/// List of functional groups by SOP class. Each list of functional groups should be in order of descending priority in the event of tags that appear in multiple groups.
		/// </summary>
		private static readonly Dictionary<string, Type[]> _functionalGroupUsage = new Dictionary<string, Type[]>
		                                                                           	{
		                                                                           		#region General Multi-Frame Fallback
		                                                                           		{
		                                                                           			string.Empty,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (PixelValueTransformationFunctionalGroup),
		                                                                           					// IdentityPixelValueTransformationFunctionalGroup is just PixelValueTransformationFunctionalGroup plus restrictions
		                                                                           					// FrameVoiLutFunctionalGroup is just a subset of FrameVoiLutWithLutFunctionalGroup
		                                                                           					typeof (FrameVoiLutWithLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (PixelIntensityRelationshipLutFunctionalGroup),
		                                                                           					typeof (FramePixelShiftFunctionalGroup),
		                                                                           					typeof (PatientOrientationInFrameFunctionalGroup),
		                                                                           					typeof (FrameDisplayShutterFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (IrradiationEventIdentificationFunctionalGroup),
		                                                                           					typeof (RadiopharmaceuticalUsageFunctionalGroup),
		                                                                           					typeof (PatientPhysiologicalStateFunctionalGroup),
		                                                                           					typeof (PlanePositionVolumeFunctionalGroup),
		                                                                           					typeof (PlaneOrientationVolumeFunctionalGroup),
		                                                                           					typeof (TemporalPositionFunctionalGroup),
		                                                                           					typeof (ImageDataTypeFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region VL Whole Slide Microscopy
		                                                                           		{
		                                                                           			SopClass.VlWholeSlideMicroscopyImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (PlanePositionSlideFunctionalGroup),
		                                                                           					typeof (OpticalPathIdentificationFunctionalGroup),
		                                                                           					typeof (SpecimenReferenceFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Enhanced MR
		                                                                           		{
		                                                                           			SopClass.EnhancedMrImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (PixelValueTransformationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (MrImageFrameTypeFunctionalGroup),
		                                                                           					typeof (MrTimingAndRelatedParametersFunctionalGroup),
		                                                                           					typeof (MrFovGeometryFunctionalGroup),
		                                                                           					typeof (MrEchoFunctionalGroup),
		                                                                           					typeof (MrModifierFunctionalGroup),
		                                                                           					typeof (MrImagingModifierFunctionalGroup),
		                                                                           					typeof (MrReceiveCoilFunctionalGroup),
		                                                                           					typeof (MrTransmitCoilFunctionalGroup),
		                                                                           					typeof (MrDiffusionFunctionalGroup),
		                                                                           					typeof (MrAveragesFunctionalGroup),
		                                                                           					typeof (MrSpatialSaturationFunctionalGroup),
		                                                                           					typeof (MrMetaboliteMapFunctionalGroup),
		                                                                           					typeof (MrVelocityEncodingFunctionalGroup),
		                                                                           					typeof (MrArterialSpinLabelingFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Enhanced MR Color
		                                                                           		{
		                                                                           			SopClass.EnhancedMrColorImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (PixelValueTransformationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (MrImageFrameTypeFunctionalGroup),
		                                                                           					typeof (MrTimingAndRelatedParametersFunctionalGroup),
		                                                                           					typeof (MrFovGeometryFunctionalGroup),
		                                                                           					typeof (MrEchoFunctionalGroup),
		                                                                           					typeof (MrModifierFunctionalGroup),
		                                                                           					typeof (MrImagingModifierFunctionalGroup),
		                                                                           					typeof (MrReceiveCoilFunctionalGroup),
		                                                                           					typeof (MrTransmitCoilFunctionalGroup),
		                                                                           					typeof (MrDiffusionFunctionalGroup),
		                                                                           					typeof (MrAveragesFunctionalGroup),
		                                                                           					typeof (MrSpatialSaturationFunctionalGroup),
		                                                                           					typeof (MrMetaboliteMapFunctionalGroup),
		                                                                           					typeof (MrVelocityEncodingFunctionalGroup),
		                                                                           					typeof (MrArterialSpinLabelingFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region MR Spectroscopy
		                                                                           		{
		                                                                           			SopClass.MrSpectroscopyStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (MrSpectroscopyFrameTypeFunctionalGroup),
		                                                                           					typeof (MrTimingAndRelatedParametersFunctionalGroup),
		                                                                           					typeof (MrSpectroscopyFovGeometryFunctionalGroup),
		                                                                           					typeof (MrEchoFunctionalGroup),
		                                                                           					typeof (MrModifierFunctionalGroup),
		                                                                           					typeof (MrReceiveCoilFunctionalGroup),
		                                                                           					typeof (MrTransmitCoilFunctionalGroup),
		                                                                           					typeof (MrDiffusionFunctionalGroup),
		                                                                           					typeof (MrAveragesFunctionalGroup),
		                                                                           					typeof (MrSpatialSaturationFunctionalGroup),
		                                                                           					typeof (MrVelocityEncodingFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Enhanced CT
		                                                                           		{
		                                                                           			SopClass.EnhancedCtImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (IrradiationEventIdentificationFunctionalGroup),
		                                                                           					typeof (CtImageFrameTypeFunctionalGroup),
		                                                                           					typeof (CtAcquisitionTypeFunctionalGroup),
		                                                                           					typeof (CtAcquisitionDetailsFunctionalGroup),
		                                                                           					typeof (CtTableDynamicsFunctionalGroup),
		                                                                           					typeof (CtPositionFunctionalGroup),
		                                                                           					typeof (CtGeometryFunctionalGroup),
		                                                                           					typeof (CtReconstructionFunctionalGroup),
		                                                                           					typeof (CtExposureFunctionalGroup),
		                                                                           					typeof (CtXRayDetailsFunctionalGroup),
		                                                                           					typeof (CtPixelValueTransformationFunctionalGroup),
		                                                                           					typeof (CtAdditionalXRaySourceFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Enhanced XA
		                                                                           		{
		                                                                           			SopClass.EnhancedXaImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (PixelIntensityRelationshipLutFunctionalGroup),
		                                                                           					typeof (FramePixelShiftFunctionalGroup),
		                                                                           					typeof (PatientOrientationInFrameFunctionalGroup),
		                                                                           					typeof (FrameDisplayShutterFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (IrradiationEventIdentificationFunctionalGroup),
		                                                                           					typeof (XaXrfFrameCharacteristicsFunctionalGroup),
		                                                                           					typeof (XRayFieldOfViewFunctionalGroup),
		                                                                           					typeof (XRayExposureControlSensingRegionsFunctionalGroup),
		                                                                           					typeof (XaXrfFramePixelDataPropertiesFunctionalGroup),
		                                                                           					typeof (XRayFrameDetectorParametersFunctionalGroup),
		                                                                           					typeof (XRayCalibrationDeviceUsageFunctionalGroup),
		                                                                           					typeof (XRayObjectThicknessFunctionalGroup),
		                                                                           					typeof (XRayFrameAcquisitionFunctionalGroup),
		                                                                           					typeof (XRayProjectionPixelCalibrationFunctionalGroup),
		                                                                           					typeof (XRayPositionerFunctionalGroup),
		                                                                           					typeof (XRayTablePositionFunctionalGroup),
		                                                                           					typeof (XRayCollimatorFunctionalGroup),
		                                                                           					typeof (XRayIsocenterReferenceSystemFunctionalGroup),
		                                                                           					typeof (XRayGeometryFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Enhanced XRF
		                                                                           		{
		                                                                           			SopClass.EnhancedXrfImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (PixelIntensityRelationshipLutFunctionalGroup),
		                                                                           					typeof (FramePixelShiftFunctionalGroup),
		                                                                           					typeof (PatientOrientationInFrameFunctionalGroup),
		                                                                           					typeof (FrameDisplayShutterFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (IrradiationEventIdentificationFunctionalGroup),
		                                                                           					typeof (XaXrfFrameCharacteristicsFunctionalGroup),
		                                                                           					typeof (XRayFieldOfViewFunctionalGroup),
		                                                                           					typeof (XRayExposureControlSensingRegionsFunctionalGroup),
		                                                                           					typeof (XaXrfFramePixelDataPropertiesFunctionalGroup),
		                                                                           					typeof (XRayFrameDetectorParametersFunctionalGroup),
		                                                                           					typeof (XRayCalibrationDeviceUsageFunctionalGroup),
		                                                                           					typeof (XRayObjectThicknessFunctionalGroup),
		                                                                           					typeof (XRayFrameAcquisitionFunctionalGroup),
		                                                                           					typeof (XRayPositionerFunctionalGroup),
		                                                                           					typeof (XRayTablePositionFunctionalGroup),
		                                                                           					typeof (XRayCollimatorFunctionalGroup),
		                                                                           					typeof (XRayGeometryFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Segmentation
		                                                                           		{
		                                                                           			SopClass.SegmentationStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (SegmentationFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Ophthalmic Tomography
		                                                                           		{
		                                                                           			SopClass.OphthalmicTomographyImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (OphthalmicFrameLocationFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region X-Ray 3D Angiograph
		                                                                           		{
		                                                                           			SopClass.XRay3dAngiographicImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (PixelValueTransformationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (XRay3DFrameTypeFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region X-Ray 3D Craniofacial
		                                                                           		{
		                                                                           			SopClass.XRay3dCraniofacialImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (PixelValueTransformationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (XRay3DFrameTypeFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Breast Tomosynthesis
		                                                                           		{
		                                                                           			SopClass.BreastTomosynthesisImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (IdentityPixelValueTransformationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutWithLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (XRay3DFrameTypeFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Enhanced PET
		                                                                           		{
		                                                                           			SopClass.EnhancedPetImageStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (PixelValueTransformationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (RadiopharmaceuticalUsageFunctionalGroup),
		                                                                           					typeof (PatientPhysiologicalStateFunctionalGroup),
		                                                                           					typeof (PetFrameTypeFunctionalGroup),
		                                                                           					typeof (PetFrameAcquisitionFunctionalGroup),
		                                                                           					typeof (PetDetectorMotionDetailsFunctionalGroup),
		                                                                           					typeof (PetPositionFunctionalGroup),
		                                                                           					typeof (PetFrameCorrectionFactorsFunctionalGroup),
		                                                                           					typeof (PetReconstructionFunctionalGroup),
		                                                                           					typeof (PetTableDynamicsFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Enhanced US
		                                                                           		{
		                                                                           			SopClass.EnhancedUsVolumeStorageUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (PlanePositionPatientFunctionalGroup),
		                                                                           					typeof (PlaneOrientationPatientFunctionalGroup),
		                                                                           					typeof (ReferencedImageFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (RealWorldValueMappingFunctionalGroup),
		                                                                           					typeof (ContrastBolusUsageFunctionalGroup),
		                                                                           					typeof (PatientOrientationInFrameFunctionalGroup),
		                                                                           					typeof (FrameDisplayShutterFunctionalGroup),
		                                                                           					typeof (RespiratorySynchronizationFunctionalGroup),
		                                                                           					typeof (PlanePositionVolumeFunctionalGroup),
		                                                                           					typeof (PlaneOrientationVolumeFunctionalGroup),
		                                                                           					typeof (TemporalPositionFunctionalGroup),
		                                                                           					typeof (ImageDataTypeFunctionalGroup),
		                                                                           					typeof (UsImageDescriptionFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           		#region Intravascular OCT
		                                                                           		{
		                                                                           			SopClass.IntravascularOpticalCoherenceTomographyImageStorageForPresentationUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (PixelIntensityRelationshipLutFunctionalGroup),
		                                                                           					typeof (IntravascularOctFrameTypeFunctionalGroup),
		                                                                           					typeof (IntravascularFrameContentFunctionalGroup),
		                                                                           					typeof (IntravascularOctFrameContentFunctionalGroup)
		                                                                           				}
		                                                                           			},
		                                                                           		{
		                                                                           			SopClass.IntravascularOpticalCoherenceTomographyImageStorageForProcessingUid,
		                                                                           			new[]
		                                                                           				{
		                                                                           					typeof (PixelMeasuresFunctionalGroup),
		                                                                           					typeof (FrameContentFunctionalGroup),
		                                                                           					typeof (DerivationImageFunctionalGroup),
		                                                                           					typeof (FrameAnatomyFunctionalGroup),
		                                                                           					typeof (CardiacSynchronizationFunctionalGroup),
		                                                                           					typeof (FrameVoiLutFunctionalGroup),
		                                                                           					typeof (PixelIntensityRelationshipLutFunctionalGroup),
		                                                                           					typeof (IntravascularOctFrameTypeFunctionalGroup),
		                                                                           					typeof (IntravascularFrameContentFunctionalGroup),
		                                                                           					typeof (IntravascularOctFrameContentFunctionalGroup)
		                                                                           				}
		                                                                           			},

		                                                                           		#endregion
		                                                                           	};

		#endregion
	}
}