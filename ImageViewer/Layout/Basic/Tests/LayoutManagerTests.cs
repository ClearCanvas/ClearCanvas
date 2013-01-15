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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.Common.StudyManagement.Tests;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Layout.Basic.Tests
{
	[TestFixture]
	public class LayoutManagerTests
	{
		[TestFixtureSetUp]
		public void Init()
		{
            DicomServerTestServiceProvider.Reset();

			Platform.SetExtensionFactory(new UnitTestExtensionFactory()
			                             	{
                                                {typeof(ServiceProviderExtensionPoint), typeof(DicomServerTestServiceProvider)},
                                                {typeof(ServiceNodeServiceProviderExtensionPoint), typeof(TestServiceNodeServiceProvider)},
                                                {typeof (StudyLoaderExtensionPoint), typeof (UnitTestStudyLoader)},
			                             		{typeof (ExpressionFactoryExtensionPoint), typeof (JScriptExpressionFactory)},
			                             		{typeof (ScreenInfoProviderExtensionPoint), typeof (MockScreenInfoProvider)},
                                                {typeof (ServiceProviderExtensionPoint),typeof (StudyStoreTestServiceProvider)},
                                                {typeof (ServiceProviderExtensionPoint),typeof (TestSystemConfigurationServiceProvider)}      
			                             	});
		}

		[Test]
		public void TestDisplaySetSortByPresentationIntent()
		{
			var dxForPresentation1 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series1ForPres", 1, true);
			var dxForProcessing1 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series2ForProc", 2, false);
			var dxForOther1 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series3ForShaiHulud", 3, null);
			var dxForPresentation2 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series4ForPres", 4, true);
			var dxForProcessing2 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series5ForProc", 5, false);
			var dxForOther2 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series6ForTheHorde", 6, null);
			var dxForPresentation3 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series7ForPres", 7, true);
			var dxForProcessing3 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series8ForProc", 8, false);
			var dxForOther3 = CreateDXSopSeries(1, "PatientPeon", "StudyA", "Series9ForAiur", 9, null);

			var oldSynchronizationContext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext ?? new SynchronizationContext());
			try
			{
				using (var studyProviderContext = UnitTestStudyLoader.RegisterStudies(Combine(dxForOther1, dxForOther2, dxForOther3, dxForPresentation1, dxForPresentation2, dxForPresentation3, dxForProcessing1, dxForProcessing2, dxForProcessing3)))
				{
					using (var viewer = new ImageViewerComponent(new LayoutManager()))
					{
						viewer.LoadStudy(new LoadStudyArgs(HashUid("StudyA"), studyProviderContext.Server));
						viewer.Layout();

						Assert.AreEqual(9, viewer.LogicalWorkspace.ImageSets[0].DisplaySets.Count, "There should be 9 display sets here");

						// FOR PRESENTATION series should be sorted to the front.
						Assert.AreEqual("Series1ForPres", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0].Description, "FOR PRESENTATION and indeterminate series should be sorted to the front, subsorted by series number");
						Assert.AreEqual("Series4ForPres", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1].Description, "FOR PRESENTATION and indeterminate series should be sorted to the front, subsorted by series number");
						Assert.AreEqual("Series7ForPres", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[2].Description, "FOR PRESENTATION and indeterminate series should be sorted to the front, subsorted by series number");

						// FOR PROCESSING and indeterminate series are equivalent and are sorted to the end.
                        Assert.AreEqual("Series2ForProc", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[3].Description, "FOR PROCESSING series should be sorted to the back, subsorted by series number");
                        Assert.AreEqual("Series3ForShaiHulud", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[4].Description, "FOR PRESENTATION and indeterminate series should be sorted to the front, subsorted by series number");
                        Assert.AreEqual("Series5ForProc", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[5].Description, "FOR PROCESSING series should be sorted to the back, subsorted by series number");
                        Assert.AreEqual("Series6ForTheHorde", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[6].Description, "FOR PRESENTATION and indeterminate series should be sorted to the front, subsorted by series number");
                        Assert.AreEqual("Series8ForProc", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[7].Description, "FOR PROCESSING series should be sorted to the back, subsorted by series number");
                        Assert.AreEqual("Series9ForAiur", viewer.LogicalWorkspace.ImageSets[0].DisplaySets[8].Description, "FOR PRESENTATION and indeterminate series should be sorted to the front, subsorted by series number");
                    }
				}
			}
			finally
			{
				SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
				Dispose(dxForOther1, dxForOther2, dxForOther3, dxForPresentation1, dxForPresentation2, dxForPresentation3, dxForProcessing1, dxForProcessing2, dxForProcessing3);
			}
		}

		private delegate void DicomDataSetInitializer(IDicomAttributeProvider dicomAttributeProvider);

		private static IEnumerable<ISopDataSource> CreateDXSopSeries(int sopCount, string patientId, string studyId, string seriesDesc, int seriesNumber, bool? forPresentation)
		{
			return CreateSopSeries(sopCount, patientId, studyId, seriesDesc, seriesNumber, "ShowMeShoyuKikkoman", "DX", !forPresentation.HasValue || forPresentation.Value ? SopClass.DigitalXRayImageStorageForPresentation : SopClass.DigitalXRayImageStorageForProcessing, s => s[DicomTags.PresentationIntentType].SetStringValue(forPresentation.HasValue ? (forPresentation.Value ? "FOR PRESENTATION" : "FOR PROCESSING") : string.Empty));
		}

		private static IEnumerable<ISopDataSource> CreateSopSeries(int sopCount, string patientId, string studyId, string seriesDesc, int seriesNumber, string frameOfReferenceId, string modality, SopClass sopClass, DicomDataSetInitializer initializer)
		{
			return CreateSopSeries(sopCount, patientId, patientId, studyId, HashUid(studyId), seriesDesc, seriesNumber, HashUid(seriesDesc), HashUid(frameOfReferenceId), modality, sopClass, initializer);
		}

		private static IEnumerable<ISopDataSource> CreateSopSeries(int sopCount,
		                                                           string patientId, string patientName,
		                                                           string studyId, string studyInstanceUid,
		                                                           string seriesDesc, int seriesNumber, string seriesInstanceUid,
		                                                           string frameOfReferenceUid, string modality,
		                                                           SopClass sopClass, DicomDataSetInitializer initializer)
		{
			for (int n = 0; n < sopCount; n++)
			{
				var dicomFile = new DicomFile();
				var dataset = dicomFile.DataSet;
				if (initializer != null)
					initializer.Invoke(dataset);
				dataset[DicomTags.PatientId].SetStringValue(patientId);
				dataset[DicomTags.PatientsName].SetStringValue(patientName);
				dataset[DicomTags.StudyId].SetStringValue(studyId);
				dataset[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				dataset[DicomTags.SeriesDescription].SetStringValue(seriesDesc);
				dataset[DicomTags.SeriesNumber].SetInt32(0, seriesNumber);
				dataset[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				dataset[DicomTags.SopClassUid].SetStringValue(sopClass.Uid);
				dataset[DicomTags.Modality].SetStringValue(modality.ToString());
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

		private static string HashUid(string id)
		{
			return string.Format("1337.411.12.8453.12.83109.70.5.{0}", BitConverter.ToUInt32(BitConverter.GetBytes(id.GetHashCode()), 0));
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