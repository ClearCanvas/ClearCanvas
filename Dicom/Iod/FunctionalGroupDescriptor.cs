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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.FunctionalGroups;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Describes the usage of a functional group.
	/// </summary>
	public sealed class FunctionalGroupDescriptor : IEquatable<FunctionalGroupDescriptor>
	{
		private readonly Type _type;
		private readonly string _name;

		/// <summary>
		/// Initializes a new instance of <see cref="FunctionalGroupDescriptor"/>.
		/// </summary>
		/// <param name="type"></param>
		public FunctionalGroupDescriptor(Type type)
		{
			Platform.CheckForNullReference(type, "type");
			Platform.CheckTrue(typeof (FunctionalGroupMacro).IsAssignableFrom(type), "type must derive from FunctionalGroupMacro");
			Platform.CheckTrue(!type.IsAbstract, "type cannot be abstract");

			_type = type;
			_name = type.Name;
		}

		/// <summary>
		/// Gets the name of the functional group.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Creates an instance of the functional group macro.
		/// </summary>
		/// <returns></returns>
		public FunctionalGroupMacro Create()
		{
			return (FunctionalGroupMacro) Activator.CreateInstance(_type);
		}

		/// <summary>
		/// Creates an instance of the functional group macro.
		/// </summary>
		/// <returns></returns>
		public FunctionalGroupMacro Create(DicomSequenceItem dicomSequenceItem)
		{
			var instance = Create();
			instance.DicomSequenceItem = dicomSequenceItem;
			return instance;
		}

		public override string ToString()
		{
			return _name;
		}

		public override int GetHashCode()
		{
			return _type.GetHashCode();
		}

		public bool Equals(FunctionalGroupDescriptor other)
		{
			return !ReferenceEquals(other, null) && _type == other._type;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as FunctionalGroupDescriptor);
		}

		#region Functional Group by SOP Class/Tag

		/// <summary>
		/// The tag to functional group mapping by SOP class.
		/// </summary>
		private static readonly Dictionary<string, IDictionary<uint, FunctionalGroupDescriptor>> _functionalGroupTagMap = new Dictionary<string, IDictionary<uint, FunctionalGroupDescriptor>>();

		/// <summary>
		/// Gets the functional group in which the specified DICOM tag is uniquely used for a given SOP class.
		/// </summary>
		/// <remarks>
		/// This method will return only common, non-modality-specific functional groups if the SOP class is not multi-frame class by the standard that uses functional groups defined by the standard.
		/// </remarks>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="dicomTag">The DICOM tag.</param>
		/// <returns></returns>
		public static FunctionalGroupDescriptor GetFunctionalGroupByTag(string sopClassUid, uint dicomTag)
		{
			// lookup the requested tag in the tag map, and return the type of the functional group that defines it - or NULL if it's not defined at all
			FunctionalGroupDescriptor functionalGroupType;
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
		public static IDictionary<uint, FunctionalGroupDescriptor> GetFunctionalGroupMap(string sopClassUid)
		{
			// normalize the SOP class UID
			if (sopClassUid == null || !_functionalGroupUsage.ContainsKey(sopClassUid)) sopClassUid = String.Empty;

			// get the tag map for the indicated SOP class - building the tag map now if necessary
			IDictionary<uint, FunctionalGroupDescriptor> tagMap;
			if (!_functionalGroupTagMap.TryGetValue(sopClassUid, out tagMap))
			{
				_functionalGroupTagMap[sopClassUid] = tagMap = GetApplicableFunctionalGroups(sopClassUid).Select(t => t.Create())
				                                               	.Where(f => !f.CanHaveMultipleItems) // if the sequence can have multiple items, then you can't have a 1-1 mapping with a frame
				                                               	.SelectMany(f => f.NestedTags.Select(g => new KeyValuePair<uint, FunctionalGroupDescriptor>(g, _functionalGroupDescriptors[f.GetType()])))
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
		public static IEnumerable<FunctionalGroupDescriptor> GetApplicableFunctionalGroups(string sopClassUid)
		{
			FunctionalGroupDescriptor[] functionalGroups;
			return _functionalGroupUsage.TryGetValue(sopClassUid ?? String.Empty, out functionalGroups) ? functionalGroups : _functionalGroupUsage[String.Empty];
		}

		/// <summary>
		/// Maps core functional group types to their descriptors. Cannot just statically define them on the classes themselves, otherwise there is a circular class initialization dependency.
		/// </summary>
		private static readonly Dictionary<Type, FunctionalGroupDescriptor> _functionalGroupDescriptors = typeof (FunctionalGroupDescriptor).Assembly
			.GetTypes().Where(t => typeof (FunctionalGroupMacro).IsAssignableFrom(t) && !t.IsAbstract)
			.ToDictionary(t => t, t => new FunctionalGroupDescriptor(t));

		/// <summary>
		/// List of functional groups by SOP class. Each list of functional groups should be in order of descending priority in the event of tags that appear in multiple groups.
		/// </summary>
		private static readonly Dictionary<string, FunctionalGroupDescriptor[]> _functionalGroupUsage = new Dictionary<string, FunctionalGroupDescriptor[]>
		                                                                                                	{
		                                                                                                		#region General Multi-Frame Fallback
		                                                                                                		{
		                                                                                                			string.Empty,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelValueTransformationFunctionalGroup)],
		                                                                                                					// IdentityPixelValueTransformationFunctionalGroup is just PixelValueTransformationFunctionalGroup plus restrictions
		                                                                                                					// FrameVoiLutFunctionalGroup is just a subset of FrameVoiLutWithLutFunctionalGroup
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutWithLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelIntensityRelationshipLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FramePixelShiftFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PatientOrientationInFrameFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameDisplayShutterFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IrradiationEventIdentificationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RadiopharmaceuticalUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PatientPhysiologicalStateFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionVolumeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationVolumeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (TemporalPositionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ImageDataTypeFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region VL Whole Slide Microscopy
		                                                                                                		{
		                                                                                                			SopClass.VlWholeSlideMicroscopyImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionSlideFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (OpticalPathIdentificationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (SpecimenReferenceFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Enhanced MR
		                                                                                                		{
		                                                                                                			SopClass.EnhancedMrImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelValueTransformationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrImageFrameTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrTimingAndRelatedParametersFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrFovGeometryFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrEchoFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrModifierFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrImagingModifierFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrReceiveCoilFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrTransmitCoilFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrDiffusionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrAveragesFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrSpatialSaturationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrMetaboliteMapFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrVelocityEncodingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrArterialSpinLabelingFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Enhanced MR Color
		                                                                                                		{
		                                                                                                			SopClass.EnhancedMrColorImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelValueTransformationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrImageFrameTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrTimingAndRelatedParametersFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrFovGeometryFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrEchoFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrModifierFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrImagingModifierFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrReceiveCoilFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrTransmitCoilFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrDiffusionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrAveragesFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrSpatialSaturationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrMetaboliteMapFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrVelocityEncodingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrArterialSpinLabelingFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region MR Spectroscopy
		                                                                                                		{
		                                                                                                			SopClass.MrSpectroscopyStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrSpectroscopyFrameTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrTimingAndRelatedParametersFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrSpectroscopyFovGeometryFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrEchoFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrModifierFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrReceiveCoilFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrTransmitCoilFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrDiffusionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrAveragesFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrSpatialSaturationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (MrVelocityEncodingFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Enhanced CT
		                                                                                                		{
		                                                                                                			SopClass.EnhancedCtImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IrradiationEventIdentificationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtImageFrameTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtAcquisitionTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtAcquisitionDetailsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtTableDynamicsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtPositionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtGeometryFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtReconstructionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtExposureFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtXRayDetailsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtPixelValueTransformationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CtAdditionalXRaySourceFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Enhanced XA
		                                                                                                		{
		                                                                                                			SopClass.EnhancedXaImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelIntensityRelationshipLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FramePixelShiftFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PatientOrientationInFrameFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameDisplayShutterFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IrradiationEventIdentificationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XaXrfFrameCharacteristicsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayFieldOfViewFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayExposureControlSensingRegionsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XaXrfFramePixelDataPropertiesFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayFrameDetectorParametersFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayCalibrationDeviceUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayObjectThicknessFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayFrameAcquisitionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayProjectionPixelCalibrationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayPositionerFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayTablePositionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayCollimatorFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayIsocenterReferenceSystemFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayGeometryFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Enhanced XRF
		                                                                                                		{
		                                                                                                			SopClass.EnhancedXrfImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelIntensityRelationshipLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FramePixelShiftFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PatientOrientationInFrameFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameDisplayShutterFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IrradiationEventIdentificationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XaXrfFrameCharacteristicsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayFieldOfViewFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayExposureControlSensingRegionsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XaXrfFramePixelDataPropertiesFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayFrameDetectorParametersFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayCalibrationDeviceUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayObjectThicknessFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayFrameAcquisitionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayPositionerFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayTablePositionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayCollimatorFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRayGeometryFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Segmentation
		                                                                                                		{
		                                                                                                			SopClass.SegmentationStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (SegmentationFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Ophthalmic Tomography
		                                                                                                		{
		                                                                                                			SopClass.OphthalmicTomographyImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (OphthalmicFrameLocationFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region X-Ray 3D Angiograph
		                                                                                                		{
		                                                                                                			SopClass.XRay3dAngiographicImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelValueTransformationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRay3DFrameTypeFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region X-Ray 3D Craniofacial
		                                                                                                		{
		                                                                                                			SopClass.XRay3dCraniofacialImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelValueTransformationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRay3DFrameTypeFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Breast Tomosynthesis
		                                                                                                		{
		                                                                                                			SopClass.BreastTomosynthesisImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IdentityPixelValueTransformationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutWithLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (XRay3DFrameTypeFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Enhanced PET
		                                                                                                		{
		                                                                                                			SopClass.EnhancedPetImageStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelValueTransformationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RadiopharmaceuticalUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PatientPhysiologicalStateFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PetFrameTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PetFrameAcquisitionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PetDetectorMotionDetailsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PetPositionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PetFrameCorrectionFactorsFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PetReconstructionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PetTableDynamicsFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Enhanced US
		                                                                                                		{
		                                                                                                			SopClass.EnhancedUsVolumeStorageUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationPatientFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ReferencedImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RealWorldValueMappingFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ContrastBolusUsageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PatientOrientationInFrameFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameDisplayShutterFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (RespiratorySynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlanePositionVolumeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PlaneOrientationVolumeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (TemporalPositionFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (ImageDataTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (UsImageDescriptionFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                		#region Intravascular OCT
		                                                                                                		{
		                                                                                                			SopClass.IntravascularOpticalCoherenceTomographyImageStorageForPresentationUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelIntensityRelationshipLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IntravascularOctFrameTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IntravascularFrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IntravascularOctFrameContentFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},
		                                                                                                		{
		                                                                                                			SopClass.IntravascularOpticalCoherenceTomographyImageStorageForProcessingUid,
		                                                                                                			new[]
		                                                                                                				{
		                                                                                                					_functionalGroupDescriptors[typeof (PixelMeasuresFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (DerivationImageFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameAnatomyFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (CardiacSynchronizationFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (FrameVoiLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (PixelIntensityRelationshipLutFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IntravascularOctFrameTypeFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IntravascularFrameContentFunctionalGroup)],
		                                                                                                					_functionalGroupDescriptors[typeof (IntravascularOctFrameContentFunctionalGroup)]
		                                                                                                				}
		                                                                                                			},

		                                                                                                		#endregion
		                                                                                                	};

		#endregion
	}
}