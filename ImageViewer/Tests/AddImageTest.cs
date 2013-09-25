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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class AddImageTest : AbstractTest
	{
		public AddImageTest() {}

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
			GC.Collect();
		}

		[TestFixtureTearDown]
		public void Cleanup() {}

		[Test]
		public void BuildStudyTree()
		{
			IImageViewer viewer = new ImageViewerComponent();
			StudyTree studyTree = viewer.StudyTree;

			string studyUid1 = DicomUid.GenerateUid().UID;
			string studyUid2 = DicomUid.GenerateUid().UID;
			string studyUid3 = DicomUid.GenerateUid().UID;

			string seriesUid1 = DicomUid.GenerateUid().UID;
			string seriesUid2 = DicomUid.GenerateUid().UID;
			string seriesUid3 = DicomUid.GenerateUid().UID;
			string seriesUid4 = DicomUid.GenerateUid().UID;
			string seriesUid5 = DicomUid.GenerateUid().UID;

			string imageUid1 = DicomUid.GenerateUid().UID;
			string imageUid2 = DicomUid.GenerateUid().UID;
			string imageUid3 = DicomUid.GenerateUid().UID;
			string imageUid4 = DicomUid.GenerateUid().UID;
			string imageUid5 = DicomUid.GenerateUid().UID;
			string imageUid6 = DicomUid.GenerateUid().UID;
			string imageUid7 = DicomUid.GenerateUid().UID;
			string imageUid8 = DicomUid.GenerateUid().UID;
			string imageUid9 = DicomUid.GenerateUid().UID;

			ImageSop image1 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid1);
			ImageSop image2 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid2);
			ImageSop image3 = CreateImageSop("patient1", studyUid1, seriesUid2, imageUid3);
			ImageSop image4 = CreateImageSop("patient1", studyUid1, seriesUid2, imageUid4);
			ImageSop image5 = CreateImageSop("patient1", studyUid2, seriesUid3, imageUid5);
			ImageSop image6 = CreateImageSop("patient1", studyUid2, seriesUid3, imageUid6);
			ImageSop image7 = CreateImageSop("patient2", studyUid3, seriesUid4, imageUid7);
			ImageSop image8 = CreateImageSop("patient2", studyUid3, seriesUid4, imageUid8);
			ImageSop image9 = CreateImageSop("patient2", studyUid3, seriesUid5, imageUid9);

			// This is an internal method.  We would never do this from real
			// client code, but we do it here because we just want to test that
			// images are being properly added to the tree. 
			studyTree.AddSop(image1);
			studyTree.AddSop(image2);
			studyTree.AddSop(image3);
			studyTree.AddSop(image4);
			studyTree.AddSop(image5);
			studyTree.AddSop(image6);
			studyTree.AddSop(image7);
			studyTree.AddSop(image8);
			studyTree.AddSop(image9);

			Assert.IsTrue(studyTree.Patients.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid2].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series[seriesUid3].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid4].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid5].Sops.Count == 1);

			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops[imageUid1].SopInstanceUid == image1.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops[imageUid2].SopInstanceUid == image2.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid2].Sops[imageUid3].SopInstanceUid == image3.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid2].Sops[imageUid4].SopInstanceUid == image4.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series[seriesUid3].Sops[imageUid5].SopInstanceUid == image5.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series[seriesUid3].Sops[imageUid6].SopInstanceUid == image6.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid4].Sops[imageUid7].SopInstanceUid == image7.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid4].Sops[imageUid8].SopInstanceUid == image8.SopInstanceUid);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid5].Sops[imageUid9].SopInstanceUid == image9.SopInstanceUid);

			Assert.IsTrue(studyTree.GetSop(imageUid1).SopInstanceUid == image1.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid2).SopInstanceUid == image2.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid3).SopInstanceUid == image3.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid4).SopInstanceUid == image4.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid5).SopInstanceUid == image5.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid6).SopInstanceUid == image6.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid7).SopInstanceUid == image7.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid8).SopInstanceUid == image8.SopInstanceUid);
			Assert.IsTrue(studyTree.GetSop(imageUid9).SopInstanceUid == image9.SopInstanceUid);

			viewer.Dispose();
		}

		[Test]
		public void AddDuplicateImage()
		{
			IImageViewer viewer = new ImageViewerComponent();
			StudyTree studyTree = viewer.StudyTree;

			string studyUid1 = DicomUid.GenerateUid().UID;
			string seriesUid1 = DicomUid.GenerateUid().UID;
			string imageUid1 = DicomUid.GenerateUid().UID;

			ImageSop image1 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid1);
			ImageSop image2 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid1);

			//The sop has already silently disposed the 2nd data source.
			Assert.IsTrue(ReferenceEquals(image1.DataSource, image2.DataSource));
			studyTree.AddSop(image1);
			studyTree.AddSop(image2);

			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops.Count == 1);

			TestDataSource dataSource = (TestDataSource) image1.DataSource;
			viewer.Dispose();

			Assert.IsTrue(dataSource.IsDisposed);
		}

		private ImageSop CreateImageSop(string patientId, string studyUid, string seriesUid, string sopUid)
		{
			DicomAttributeCollection dataSet = new DicomAttributeCollection();
			base.SetupMR(dataSet);
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), dataSet);
			TestDataSource dataSource = new TestDataSource(file);
			file.DataSet[DicomTags.PatientId].SetStringValue(patientId);
			file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyUid);
			file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesUid);
			file.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopUid);

			return new ImageSop(dataSource);
		}
	}
}

#endif