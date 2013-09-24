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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod.FunctionalGroups;
using ClearCanvas.Dicom.Iod.Macros;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Tests
{
	[TestFixture]
	internal class FunctionalGroupMacroTests
	{
		private static readonly string[] _multiframeSopClassUids = new[]
		                                                           	{
		                                                           		SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid,
		                                                           		SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid,
		                                                           		SopClass.MultiFrameTrueColorSecondaryCaptureImageStorageUid,
		                                                           		SopClass.VlWholeSlideMicroscopyImageStorageUid,
		                                                           		SopClass.EnhancedMrImageStorageUid,
		                                                           		SopClass.EnhancedMrColorImageStorageUid,
		                                                           		SopClass.MrSpectroscopyStorageUid,
		                                                           		SopClass.EnhancedCtImageStorageUid,
		                                                           		SopClass.EnhancedXaImageStorageUid,
		                                                           		SopClass.EnhancedXrfImageStorageUid,
		                                                           		SopClass.SegmentationStorageUid,
		                                                           		SopClass.OphthalmicTomographyImageStorageUid,
		                                                           		SopClass.XRay3dAngiographicImageStorageUid,
		                                                           		SopClass.XRay3dCraniofacialImageStorageUid,
		                                                           		SopClass.BreastTomosynthesisImageStorageUid,
		                                                           		SopClass.EnhancedPetImageStorageUid,
		                                                           		SopClass.EnhancedUsVolumeStorageUid,
		                                                           		SopClass.IntravascularOpticalCoherenceTomographyImageStorageForPresentationUid,
		                                                           		SopClass.IntravascularOpticalCoherenceTomographyImageStorageForProcessingUid
		                                                           	};

		[Test]
		public void ListFunctionalGroups()
		{
			const string s = "> {0}";
			foreach (var functionalGroupType in GetFunctionalGroupTypes())
			{
				Console.WriteLine(functionalGroupType.Name);

				var functionalGroup = (FunctionalGroupMacro) Activator.CreateInstance(functionalGroupType);
				foreach (var tag in functionalGroup.DefinedTags.Select(DicomTagDictionary.GetDicomTag))
				{
					Console.WriteLine(tag.ToString());

					foreach (var nestedTag in functionalGroup.NestedTags.Select(DicomTagDictionary.GetDicomTag))
					{
						Console.WriteLine(s, nestedTag);
					}
				}

				Console.WriteLine();
			}
		}

		[Test]
		public void ListAmbiguousTagsBySopClass()
		{
			foreach (var uid in _multiframeSopClassUids)
			{
				const string msg = "SOP Class: {0}";
				Console.WriteLine(msg, SopClass.GetSopClass(uid).Name);
				ListTagsByParentSequence(FunctionalGroupDescriptor.GetApplicableFunctionalGroups(uid).Select(f => f.Create()), true);

				Console.WriteLine(new string('=', 32));
				Console.WriteLine();
			}
		}

		[Test]
		public void ListAmbiguousTagsByParentSequence()
		{
			ListTagsByParentSequence(CreateFunctionalGroups(), true);
		}

		[Test]
		public void ListTagsByParentSequence()
		{
			ListTagsByParentSequence(CreateFunctionalGroups(), false);
		}

		private static void ListTagsByParentSequence(IEnumerable<FunctionalGroupMacro> functionalGroups, bool excludeMultiItemSequences)
		{
			var tagFunctionalGroupMap = functionalGroups.SelectMany(f => f.NestedTags.Select(g => new {Tag = g, ParentTag = f.DefinedTags.Single(), FunctionalGroup = f}));
			if (excludeMultiItemSequences) tagFunctionalGroupMap = tagFunctionalGroupMap.Where(t => !t.FunctionalGroup.CanHaveMultipleItems);
			foreach (var groupResults in tagFunctionalGroupMap.GroupBy(f => f.Tag).OrderByDescending(t => t.Distinct((a, b) => a.ParentTag == b.ParentTag).Count()))
			{
				var subGrouped = groupResults.GroupBy(g => g.ParentTag).ToList();
				if (subGrouped.Count() == 1 && excludeMultiItemSequences) continue;

				Console.WriteLine(DicomTagDictionary.GetDicomTag(groupResults.Key));
				foreach (var pair in subGrouped)
				{
					const string format = "- {0} ({1})";
					Console.WriteLine(format, DicomTagDictionary.GetDicomTag(pair.Key), string.Join(", ", pair.Select(g => g.FunctionalGroup.GetType().Name).ToArray()));
				}
				Console.WriteLine();
			}
		}

		[Test]
		public void TestFunctionalGroupSequenceTags()
		{
			// asserts some basic stuff about the functional groups
			foreach (var functionalGroup in CreateFunctionalGroups())
			{
				var fgName = functionalGroup.GetType().Name;

				var tags = functionalGroup.DefinedTags.Select(DicomTagDictionary.GetDicomTag).ToList();
				Assert.AreEqual(1, tags.Count(), "Root Tag in functional group should be the only one: {0}", fgName);

				var rootTag = tags.Single();
				Assert.AreEqual(DicomVr.SQvr, rootTag.VR, "Root Tag in functional group should be SQ: {0} in {1}", rootTag, fgName);

				var sqProperty = functionalGroup.GetType().GetProperty(rootTag.VariableName.Replace("3d", "3D"));
				if (functionalGroup.CanHaveMultipleItems)
				{
					Assert.IsTrue(sqProperty.PropertyType.IsArray, "Sequence property for multiple item functional group should be an array: {0}", fgName);

					var elementType = sqProperty.PropertyType.GetElementType();
					Assert.IsTrue(typeof (IodBase).IsAssignableFrom(elementType) || typeof (IIodMacro).IsAssignableFrom(elementType),
					              "Sequence property array element type should be some subclass of IodBase: {0}", fgName);
				}
				else
				{
					Assert.IsFalse(sqProperty.PropertyType.IsArray, "Sequence property for single item functional group should NOT be an array: {0}", fgName);

					var propertyType = sqProperty.PropertyType;
					Assert.IsTrue(typeof (IodBase).IsAssignableFrom(propertyType) || typeof (IIodMacro).IsAssignableFrom(propertyType),
					              "Sequence property type should be some subclass of IodBase: {0}", fgName);
				}

				var nestedTags = functionalGroup.NestedTags.ToList();
				Assert.IsNotEmpty(nestedTags, "Nested tags in sequence should be defined: {0}", fgName);
			}
		}

		[Test]
		public void TestFunctionalGroupApplicabilityBySopClass()
		{
			var commonGroups = new[]
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
			                   	};

			// asserts that a set exists for the empty string, and that it's only the 'common' functional groups defined in PS 3.3 C.7.6.16
			Assert.AreEqual(commonGroups.Select(t => new FunctionalGroupDescriptor(t)).OrderBy(t => t.Name).ToList(),
			                FunctionalGroupDescriptor.GetApplicableFunctionalGroups(string.Empty).OrderBy(t => t.Name).ToList(),
			                "A functional group set should exist that consists only of common (non-modality specific) functional groups");

			// asserts that GetApplicableFunctionalGroups returns common functional groups for unrecognized SOP classes
			Assert.AreEqual(FunctionalGroupDescriptor.GetApplicableFunctionalGroups(string.Empty).OrderBy(t => t.Name).ToList(),
			                FunctionalGroupDescriptor.GetApplicableFunctionalGroups("1.2.3.4").OrderBy(t => t.Name).ToList(),
			                "Unrecognized SOP classes should only return the common functional groups");

			// asserts that GetApplicableFunctionalGroups returns distinct functional groups
			foreach (var uid in _multiframeSopClassUids)
			{
				var results = FunctionalGroupDescriptor.GetApplicableFunctionalGroups(uid).ToList();
				Assert.IsTrue(results.Count == results.Distinct().Count(), "Non unique functional groups defined for {0}", SopClass.GetSopClass(uid).Name);
			}
		}

		[Test]
		public void TestTagMappingBySopClass()
		{
			foreach (var uid in _multiframeSopClassUids)
			{
				var sopClass = SopClass.GetSopClass(uid).Name;
				var functionalGroups = FunctionalGroupDescriptor.GetApplicableFunctionalGroups(uid).ToList();
				var tagGroups = functionalGroups.Select(f => f.Create())
					.SelectMany(f => f.NestedTags.Select(t => new {TagValue = t, FunctionalGroup = f}))
					.GroupBy(u => u.TagValue).OrderBy(u => u.Key).ToList();

				foreach (var group in tagGroups)
				{
					var dcmTag = DicomTagDictionary.GetDicomTag(group.Key);

					// asserts that any tag defined in 'singleton' functional groups (those whose sequence can have at most 1 item) should have at least some mapping
					var fgType = FunctionalGroupDescriptor.GetFunctionalGroupByTag(uid, group.Key);
					if (fgType == null)
					{
						foreach (var entry in group)
						{
							Assert.IsTrue(entry.FunctionalGroup.CanHaveMultipleItems, "At least one singleton functional group defines tag {0} for SOP class {1}", dcmTag, sopClass);
						}
					}

					// explicitly assert the mapping for any tag defined by multiple 'singleton' functional groups - so that if new tags are introduced later, we would explicitly consider what is the correct mapping
					if (group.Count(g => !g.FunctionalGroup.CanHaveMultipleItems) > 1)
					{
						const string wrongMapMsg = "SOP Class {0} maps tag {1} to the wrong functional group";

						if (uid == SopClass.EnhancedXaImageStorageUid)
						{
							switch (group.Key)
							{
								case DicomTags.TableHorizontalRotationAngle:
								case DicomTags.TableHeadTiltAngle:
								case DicomTags.TableCradleTiltAngle:
									Assert.AreEqual(new FunctionalGroupDescriptor(typeof (XRayTablePositionFunctionalGroup)), fgType, wrongMapMsg, sopClass, dcmTag);
									break;
								default:
									Assert.Fail("SOP Class {0} shouldn't have an ambiguous mapping for tag {1} - if new tags were added, please explicitly update the expected mapping in this unit test", sopClass, dcmTag);
									break;
							}
						}
						else
						{
							Assert.Fail("SOP Class {0} shouldn't have any ambiguous mappings - if new tags were added, please explicitly update the expected mapping in this unit test", sopClass);
						}
					}
				}
			}
		}

		private static IEnumerable<FunctionalGroupMacro> CreateFunctionalGroups()
		{
			return GetFunctionalGroupTypes().Select(t => (FunctionalGroupMacro) Activator.CreateInstance(t));
		}

		private static IEnumerable<Type> GetFunctionalGroupTypes()
		{
			var baseType = typeof (FunctionalGroupMacro);
			return baseType.Assembly.GetTypes().Where(baseType.IsAssignableFrom).Where(t => !t.IsAbstract).OrderBy(t => t.Name);
		}
	}
}

#endif