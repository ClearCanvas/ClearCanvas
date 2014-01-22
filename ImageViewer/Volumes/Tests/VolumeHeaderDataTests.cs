#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

using System.Collections.Generic;
using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volumes.Tests
{
	[TestFixture]
	public class VolumeHeaderDataTests
	{
		[Test]
		public void TestBasicTags()
		{
			var sourceSops = CreateSourceSops();

			var prototype =  VolumeHeaderData.TestCreate(sourceSops);

			AssertAreEqual(sourceSops[0], prototype, DicomTags.PatientId);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.PatientsName);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.ClinicalTrialSubjectId);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.StudyInstanceUid);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.StudyId);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.ClinicalTrialTimePointId);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.PatientsAge);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.PatientsSize);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.PatientsWeight);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.SeriesDescription);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.Modality);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.SeriesNumber);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.ClinicalTrialSeriesId);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.Manufacturer);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.ManufacturersModelName);
			AssertAreEqual(sourceSops[0], prototype, DicomTags.FrameOfReferenceUid);
		}

		[Test]
		public void TestAggregatedPatientAnonymityTags()
		{
			// The Rules for aggregating Burned in Annotation and Recognizable Visual Features:
			// 1. If any one SOP says YES, the result is always YES
			// 2. Otherwise, if any one SOP does not include the tag, or the value is unparseable, the result is the empty value (undefined)
			// 3. Otherwise, all SOPs consistently say NO, so the result is simply NO
			const string enumYes = "YES";
			const string enumNo = "NO";

			// All Undefined
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.BurnedInAnnotation].SetEmptyValue();
					x[DicomTags.RecognizableVisualFeatures].SetEmptyValue();
				}

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual("", prototype, DicomTags.BurnedInAnnotation, "All Undefined");
				AssertAreEqual("", prototype, DicomTags.RecognizableVisualFeatures, "All Undefined");
			}

			// All YES
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.BurnedInAnnotation].SetStringValue(enumYes);
					x[DicomTags.RecognizableVisualFeatures].SetStringValue(enumYes);
				}

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual(enumYes, prototype, DicomTags.BurnedInAnnotation, "All YES");
				AssertAreEqual(enumYes, prototype, DicomTags.RecognizableVisualFeatures, "All YES");
			}

			// All NO
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.BurnedInAnnotation].SetStringValue(enumNo);
					x[DicomTags.RecognizableVisualFeatures].SetStringValue(enumNo);
				}

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual(enumNo, prototype, DicomTags.BurnedInAnnotation, "All NO");
				AssertAreEqual(enumNo, prototype, DicomTags.RecognizableVisualFeatures, "All NO");
			}

			// At Least One YES
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.BurnedInAnnotation].SetEmptyValue();
					x[DicomTags.RecognizableVisualFeatures].SetEmptyValue();
				}
				sourceSops[3][DicomTags.BurnedInAnnotation].SetStringValue("ASDF");
				sourceSops[4][DicomTags.BurnedInAnnotation].SetStringValue(enumYes);
				sourceSops[5][DicomTags.BurnedInAnnotation].SetStringValue(enumNo);
				sourceSops[6][DicomTags.RecognizableVisualFeatures].SetStringValue(enumNo);
				sourceSops[7][DicomTags.RecognizableVisualFeatures].SetStringValue(enumYes);
				sourceSops[8][DicomTags.RecognizableVisualFeatures].SetStringValue("");

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual(enumYes, prototype, DicomTags.BurnedInAnnotation, "At Least One YES");
				AssertAreEqual(enumYes, prototype, DicomTags.RecognizableVisualFeatures, "At Least One YES");
			}

			// At Least One Undefined
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.BurnedInAnnotation].SetStringValue(enumNo);
					x[DicomTags.RecognizableVisualFeatures].SetStringValue(enumNo);
				}
				sourceSops[3][DicomTags.BurnedInAnnotation].SetStringValue("ASDF");
				sourceSops[8][DicomTags.RecognizableVisualFeatures].SetStringValue("");

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual("", prototype, DicomTags.BurnedInAnnotation, "At Least One Undefined");
				AssertAreEqual("", prototype, DicomTags.RecognizableVisualFeatures, "At Least One Undefined");
			}
		}

		[Test]
		public void TestAggregatedLossyCompressionTags()
		{
			// The Rules for aggregating Lossy Image Compression:
			// 1. If any one SOP says 01 (Lossy), the result is always 01 (Lossy)
			// 2. Otherwise, if any one SOP does not include the tag, or the value is unparseable, the result is the empty value (undefined)
			// 3. Otherwise, all SOPs consistently say 00 (Lossless), so the result is simply 00 (Lossless)
			const string enumLossy = "01";
			const string enumLossless = "00";

			// All Undefined
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.LossyImageCompression].SetEmptyValue();
				}

				var prototype = VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual("", prototype, DicomTags.LossyImageCompression, "All Undefined");
			}

			// All LOSSY
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.LossyImageCompression].SetStringValue(enumLossy);
				}

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual(enumLossy, prototype, DicomTags.LossyImageCompression, "All LOSSY");
			}

			// All LOSSLESS
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.LossyImageCompression].SetStringValue(enumLossless);
				}

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual(enumLossless, prototype, DicomTags.LossyImageCompression, "All LOSSLESS");
			}

			// At Least One LOSSY
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.LossyImageCompression].SetEmptyValue();
				}
				sourceSops[3][DicomTags.LossyImageCompression].SetStringValue("ASDF");
				sourceSops[4][DicomTags.LossyImageCompression].SetStringValue(enumLossy);
				sourceSops[5][DicomTags.LossyImageCompression].SetStringValue(enumLossless);

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual(enumLossy, prototype, DicomTags.LossyImageCompression, "At Least One LOSSY");
			}

			// At Least One Undefined
			{
				var sourceSops = CreateSourceSops();
				foreach (var x in sourceSops)
				{
					x[DicomTags.LossyImageCompression].SetStringValue(enumLossless);
				}
				sourceSops[3][DicomTags.LossyImageCompression].SetStringValue("ASDF");

				var prototype =  VolumeHeaderData.TestCreate(sourceSops);
				AssertAreEqual("", prototype, DicomTags.LossyImageCompression, "At Least One Undefined");
			}
		}

		private static void AssertAreEqual(string value, IDicomAttributeProvider actualDataSource, uint dicomTag, string message = "", params object[] args)
		{
			var tag = DicomTagDictionary.GetDicomTag(dicomTag);
			Assert.AreEqual(value, actualDataSource[dicomTag].ToString(), "@{1}: {0}", string.Format(message, args), tag != null ? tag.Name : dicomTag.ToString("X8"));
		}

		private static void AssertAreEqual(IDicomAttributeProvider expectedDataSource, IDicomAttributeProvider actualDataSource, uint dicomTag, string message = "", params object[] args)
		{
			var tag = DicomTagDictionary.GetDicomTag(dicomTag);
			Assert.AreEqual(expectedDataSource[dicomTag], actualDataSource[dicomTag], "@{1}: {0}", string.Format(message, args), tag != null ? tag.Name : dicomTag.ToString("X8"));
		}

		private static IList<IDicomAttributeProvider> CreateSourceSops(int count = 10)
		{
			var studyInstanceUid = DicomUid.GenerateUid().UID;
			var seriesInstanceUid = DicomUid.GenerateUid().UID;
			var frameOfReferenceUid = DicomUid.GenerateUid().UID;

			var list = new List<IDicomAttributeProvider>();
			for (var n = 0; n < count; ++n)
			{
				var dataset = new DicomAttributeCollection();
				list.Add(dataset);

				// patient module
				dataset[DicomTags.PatientId].SetStringValue("PATIENT_ID");
				dataset[DicomTags.PatientsName].SetStringValue("PATIENT_NAME");

				// clinical trial subject module
				dataset[DicomTags.ClinicalTrialSubjectId].SetStringValue("TRIAL_SUBJECT_ID");

				// general study module
				dataset[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				dataset[DicomTags.StudyId].SetStringValue("STUDY_ID");
				dataset[DicomTags.ClinicalTrialTimePointId].SetStringValue("TRIAL_TIMEPOINT_ID");

				// patient study module
				dataset[DicomTags.PatientsAge].SetStringValue("938Y");
				dataset[DicomTags.PatientsSize].SetStringValue("2.0");
				dataset[DicomTags.PatientsWeight].SetStringValue("198.0");

				// general series module
				dataset[DicomTags.SeriesDescription].SetStringValue("SERIES_DESU");
				dataset[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				dataset[DicomTags.Modality].SetStringValue("SC");
				dataset[DicomTags.SeriesNumber].SetInt32(0, 1);

				// clinical trial series module
				dataset[DicomTags.ClinicalTrialSeriesId].SetStringValue("TRIAL_SERIES_ID");

				// general equipment module
				dataset[DicomTags.Manufacturer].SetStringValue("MANUFACTURER");
				dataset[DicomTags.ManufacturersModelName].SetStringValue("MODEL_NAME");

				// frame of reference module
				dataset[DicomTags.FrameOfReferenceUid].SetStringValue(frameOfReferenceUid);

				// sop common module
				dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				dataset[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);

				// general image module and others
				dataset[DicomTags.InstanceNumber].SetInt32(0, n + 1);
				dataset[DicomTags.PixelSpacing].SetStringValue(@"0.01\0.01");
				dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
				dataset[DicomTags.BitsStored].SetInt32(0, 16);
				dataset[DicomTags.BitsAllocated].SetInt32(0, 16);
				dataset[DicomTags.HighBit].SetInt32(0, 15);
				dataset[DicomTags.PixelRepresentation].SetInt32(0, 0);
				dataset[DicomTags.Rows].SetInt32(0, 512);
				dataset[DicomTags.Columns].SetInt32(0, 256);
				dataset[DicomTags.WindowCenter].SetInt32(0, 32768);
				dataset[DicomTags.WindowWidth].SetInt32(0, 65536);
				dataset[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");
			}
			return list;
		}
	}
}

#endif