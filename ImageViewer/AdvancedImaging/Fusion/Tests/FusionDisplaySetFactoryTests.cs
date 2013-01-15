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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[TestFixture]
	public class FusionDisplaySetFactoryTests
	{
		[Test]
		public void TestNullCreation()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var displaySets = CreateDisplaySets(factory, Combine<ISopDataSource>());

			try
			{
				Assert.IsEmpty(displaySets, "Display set factories should not throw an exception if there is nothing to create.");
			}
			finally
			{
				Dispose(displaySets);
			}
		}

		[Test]
		public void TestPETAndCTFusion()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET));

			try
			{
				Assert.IsNotEmpty(displaySets, "Fusion display sets should be created for trivially simple PET-CT data.");
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesCT, seriesPET);
			}
		}

		[Test]
		public void TestNoFusingWrongModality()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesMR = CreateSopSeries(25, "PatientA", "StudyA", "SeriesMR", 1, "FrameA", Modality.MR);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesMR, seriesPET));

			try
			{
				Assert.IsEmpty(displaySets, "Fusion display sets should not be created for PET-MR using a PET-CT factory.");
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesMR, seriesPET);
			}
		}

		[Test]
		public void TestNoFusingInvalidModalities()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesMR = CreateSopSeries(25, "PatientA", "StudyA", "SeriesMR", 1, "FrameA", Modality.MR);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesMR));

			try
			{
				Assert.IsEmpty(displaySets, "Fusion display sets should not be created between invalid modalities (e.g. CT and MR).");
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesCT, seriesMR);
			}
		}

		[Test]
		public void TestNoFusingDifferentStudies()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyB", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET));

			try
			{
				Assert.IsEmpty(displaySets, "Fusion display sets should not be created for series in different studies.");
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesCT, seriesPET);
			}
		}

		[Test]
		public void TestNoFusingDifferentPatients()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientB", "StudyB", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET));

			try
			{
				Assert.IsEmpty(displaySets, "Fusion display sets should not be created for series from different patients.");
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesCT, seriesPET);
			}
		}

		[Test]
		public void TestFusingDifferentFramesOfReference()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPET", 2, "FrameB", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET));

			try
			{
				Assert.IsNotEmpty(displaySets, "Fusion display sets should still be created even with different frames of reference.");
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesCT, seriesPET);
			}
		}

		[Test]
		public void TestPETFusionDisplaySetDescriptor()
		{
			StudyTree studyTree;
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPET", 2, "FrameA", Modality.PT);
			var seriesPETCor = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPETCor", 3, "FrameA", Modality.PT, true, false);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET, seriesPETCor), out studyTree);

			try
			{
				Assert.AreEqual(2, displaySets.Count, "There is only one valid combination of fuseable series.");

				// Verify identity of fused series
				var displaySet = CollectionUtils.SelectFirst(displaySets, d => ValidateFusionDisplaySetDescriptor(d.Descriptor, HashUid("SeriesCT"), HashUid("SeriesPET")));
				Assert.IsNotNull(displaySet, "Could not find the uncorrected fusion series.");
				Assert.IsFalse(((PETFusionDisplaySetDescriptor) displaySet.Descriptor).AttenuationCorrection, "Uncorrected fusion series is incorrectly flagged as corrected.");

				var displaySetCor = CollectionUtils.SelectFirst(displaySets, d => ValidateFusionDisplaySetDescriptor(d.Descriptor, HashUid("SeriesCT"), HashUid("SeriesPETCor")));
				Assert.IsNotNull(displaySetCor, "Could not find the corrected fusion series.");
				Assert.IsTrue(((PETFusionDisplaySetDescriptor) displaySetCor.Descriptor).AttenuationCorrection, "Corrected fusion series is incorrectly flagged as uncorrected.");

				var seriesCollection = CollectionUtils.FirstElement(CollectionUtils.FirstElement(studyTree.Patients).Studies).Series;
				var ctSeries = CollectionUtils.SelectFirst(seriesCollection, s => s.SeriesDescription == "SeriesCT");
				var petSeries = CollectionUtils.SelectFirst(seriesCollection, s => s.SeriesDescription == "SeriesPET");
				var petCorSeries = CollectionUtils.SelectFirst(seriesCollection, s => s.SeriesDescription == "SeriesPETCor");

				var expectedDescriptor = GetFusedDisplaySetName(ctSeries, petSeries, false);
				var expectedDescriptorCor = GetFusedDisplaySetName(ctSeries, petCorSeries, true);

				// Validate the names of the display sets (this is what is shown on the context menu)
				Assert.AreEqual(expectedDescriptor, displaySet.Name, "Name string differs for uncorrected display set.");
				Assert.AreEqual(expectedDescriptorCor, displaySetCor.Name, "Name string differs for corrected display set.");
			}
			catch (Exception)
			{
				if (displaySets.Count > 0)
				{
					Console.WriteLine("Generated Display Sets ({0})", displaySets.Count);
					foreach (var displaySet in displaySets)
						Console.WriteLine(" > {0}", displaySet.Descriptor.Description);
				}
				throw;
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesCT, seriesPET, seriesPETCor);
			}
		}

		[Test]
		public void TestFusionMatchMaker()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesCTAx = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCTAx", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPET", 2, "FrameA", Modality.PT);
			var seriesPETCor = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPETCor", 3, "FrameA", Modality.PT, true, false);
			var unrelatedSeries = CreateSopSeries(25, "PatientA", "StudyA", "UnrelatedSeries", 1, "FrameA", Modality.MR);
			var unrelatedStudy = CreateSopSeries(25, "PatientA", "UnrelatedStudy", "UnrelatedSeries", 1, "FrameA", Modality.PT);
			var unrelatedPatient = CreateSopSeries(25, "UnrelatedPatient", "UnrelatedStudy", "UnrelatedSeries", 1, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET, seriesCTAx, seriesPETCor, unrelatedSeries, unrelatedStudy, unrelatedPatient));

			try
			{
				// There are 4 lights, obviously
				Assert.AreEqual(4, displaySets.Count, "There are four valid combinations of fuseable series.");

				// Verify identities of each fused series
				Assert.IsTrue(CollectionUtils.Contains(displaySets, d => ValidateFusionDisplaySetDescriptor(d.Descriptor, HashUid("SeriesCT"), HashUid("SeriesPET"))));
				Assert.IsTrue(CollectionUtils.Contains(displaySets, d => ValidateFusionDisplaySetDescriptor(d.Descriptor, HashUid("SeriesCT"), HashUid("SeriesPETCor"))));
				Assert.IsTrue(CollectionUtils.Contains(displaySets, d => ValidateFusionDisplaySetDescriptor(d.Descriptor, HashUid("SeriesCTAx"), HashUid("SeriesPET"))));
				Assert.IsTrue(CollectionUtils.Contains(displaySets, d => ValidateFusionDisplaySetDescriptor(d.Descriptor, HashUid("SeriesCTAx"), HashUid("SeriesPETCor"))));
			}
			catch (Exception)
			{
				if (displaySets.Count > 0)
				{
					Console.WriteLine("Generated Display Sets ({0})", displaySets.Count);
					foreach (var displaySet in displaySets)
						Console.WriteLine(" > {0}", displaySet.Descriptor.Description);
				}
				throw;
			}
			finally
			{
				Dispose(displaySets);
				Dispose(seriesCT, seriesCTAx, seriesPET, seriesPETCor, unrelatedSeries, unrelatedStudy, unrelatedPatient);
			}
		}

		private static bool ValidateFusionDisplaySetDescriptor(IDisplaySetDescriptor descriptor, string baseSeriesInstanceUid, string overlaySeriesInstanceUid)
		{
			if (descriptor is IFusionDisplaySetDescriptor)
			{
				var fusionDisplaySetDescriptor = (IFusionDisplaySetDescriptor) descriptor;
				return fusionDisplaySetDescriptor.SourceSeries.SeriesInstanceUid == baseSeriesInstanceUid
				       && fusionDisplaySetDescriptor.OverlaySeries.SeriesInstanceUid == overlaySeriesInstanceUid;
			}
			return false;
		}

		private static List<IDisplaySet> CreateDisplaySets(IDisplaySetFactory displaySetFactory, IEnumerable<ISopDataSource> sopDataSources)
		{
			StudyTree studyTree;
			return CreateDisplaySets(displaySetFactory, sopDataSources, out studyTree);
		}

		private static List<IDisplaySet> CreateDisplaySets(IDisplaySetFactory displaySetFactory, IEnumerable<ISopDataSource> sopDataSources, out StudyTree studyTree)
		{
			studyTree = new StudyTree();
			foreach (var sopDataSource in sopDataSources)
			{
				studyTree.AddSop(new ImageSop(sopDataSource));
			}
			displaySetFactory.SetStudyTree(studyTree);

			var displaySets = new List<IDisplaySet>();
			foreach (var patient in studyTree.Patients)
			{
				foreach (var study in patient.Studies)
                    displaySets.AddRange(displaySetFactory.CreateDisplaySets(study));
			}
			return displaySets;
		}

		private static IEnumerable<ISopDataSource> CreateSopSeries(int sopCount, string patientId, string studyId, string seriesDesc, int seriesNumber, string frameOfReferenceId, Modality modality)
		{
			return CreateSopSeries(sopCount, patientId, patientId, studyId, HashUid(studyId), seriesDesc, seriesNumber, HashUid(seriesDesc), HashUid(frameOfReferenceId), modality, false, false);
		}

		private static IEnumerable<ISopDataSource> CreateSopSeries(int sopCount, string patientId, string studyId, string seriesDesc, int seriesNumber, string frameOfReferenceId, Modality modality, bool attnCorrected, bool lossyCompressed)
		{
			return CreateSopSeries(sopCount, patientId, patientId, studyId, HashUid(studyId), seriesDesc, seriesNumber, HashUid(seriesDesc), HashUid(frameOfReferenceId), modality, attnCorrected, lossyCompressed);
		}

		private static IEnumerable<ISopDataSource> CreateSopSeries(int sopCount,
		                                                           string patientId, string patientName,
		                                                           string studyId, string studyInstanceUid,
		                                                           string seriesDesc, int seriesNumber, string seriesInstanceUid,
		                                                           string frameOfReferenceUid, Modality modality,
		                                                           bool attnCorrected, bool lossyCompressed)
		{
			for (int n = 0; n < sopCount; n++)
			{
				var dicomFile = new DicomFile();
				var dataset = dicomFile.DataSet;
				dataset[DicomTags.PatientId].SetStringValue(patientId);
				dataset[DicomTags.PatientsName].SetStringValue(patientName);
				dataset[DicomTags.StudyId].SetStringValue(studyId);
				dataset[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				dataset[DicomTags.SeriesDescription].SetStringValue(seriesDesc);
				dataset[DicomTags.SeriesNumber].SetInt32(0, seriesNumber);
				dataset[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				dataset[DicomTags.SopClassUid].SetStringValue(ModalityConverter.ToSopClassUid(modality));
				dataset[DicomTags.Modality].SetStringValue(modality.ToString());
				dataset[DicomTags.LossyImageCompression].SetStringValue(lossyCompressed ? "01" : "00");
				dataset[DicomTags.LossyImageCompressionRatio].SetFloat32(0, lossyCompressed ? 9999 : 1);
				dataset[DicomTags.LossyImageCompressionMethod].SetStringValue(lossyCompressed ? "IDUNNO" : string.Empty);
				dataset[DicomTags.CorrectedImage].SetStringValue(attnCorrected ? "ATTN" : string.Empty);
				dataset[DicomTags.FrameOfReferenceUid].SetStringValue(frameOfReferenceUid);
				dataset[DicomTags.ImageOrientationPatient].SetStringValue(string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", 1, 0, 0, 0, 1, 0));
				dataset[DicomTags.ImagePositionPatient].SetStringValue(string.Format(@"{0}\{1}\{2}", 0, 0, n));
				dataset[DicomTags.PixelSpacing].SetStringValue(string.Format(@"{0}\{1}", 0.5, 0.5));
				dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
				dataset[DicomTags.SamplesPerPixel].SetInt32(0, 1);
				dataset[DicomTags.BitsStored].SetInt32(0, 16);
				dataset[DicomTags.BitsAllocated].SetInt32(0, 16);
				dataset[DicomTags.HighBit].SetInt32(0, 15);
				dataset[DicomTags.PixelRepresentation].SetInt32(0, 1);
				dataset[DicomTags.Rows].SetInt32(0, 100);
				dataset[DicomTags.Columns].SetInt32(0, 100);
				dataset[DicomTags.WindowCenter].SetInt32(0, 0);
				dataset[DicomTags.WindowWidth].SetInt32(0, 65536);
				dataset[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");
				dataset[DicomTags.PixelData].Values = new byte[2*100*100];
				dicomFile.MediaStorageSopClassUid = dataset[DicomTags.SopClassUid];
				dicomFile.MediaStorageSopInstanceUid = dataset[DicomTags.SopInstanceUid];
				yield return new XSopDataSource(dicomFile);
			}
		}

		private static string GetFusedDisplaySetName(ISeriesData baseSeries, ISeriesData petSeries, bool attenuationCorrection)
		{
			return string.Format(SR.FormatPETFusionDisplaySet,
			                     string.Format("{0} - {1}", baseSeries.SeriesNumber, baseSeries.SeriesDescription),
			                     string.Format("{0} - {1}", petSeries.SeriesNumber, petSeries.SeriesDescription),
			                     attenuationCorrection ? SR.LabelAttenuationCorrection : SR.LabelNoAttentuationCorrection
				);
		}

		private static string HashUid(string id)
		{
			return string.Format("411.12.8453.12.83109.70.5.{0}", BitConverter.ToUInt32(BitConverter.GetBytes(id.GetHashCode()), 0));
		}

		private static IEnumerable<T> Combine<T>(params IEnumerable<T>[] enumerables)
		{
			if (enumerables == null)
				yield break;

			foreach (var enumerable in enumerables)
				foreach (var item in enumerable)
					yield return item;
		}

		private static void Dispose<T>(params IEnumerable<T>[] disposableses) where T : IDisposable
		{
			if (disposableses != null)
				foreach (var disposables in disposableses)
					if (disposables != null)
						foreach (var disposable in disposables)
							disposable.Dispose();
		}

		private class XSopDataSource : DicomMessageSopDataSource
		{
			public XSopDataSource(DicomMessageBase sourceMessage) : base(sourceMessage) {}
		}
	}
}

#endif