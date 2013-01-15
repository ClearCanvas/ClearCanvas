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
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;
using ClearCanvas.Dicom.Tests;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class ImageSetGroupsTests : AbstractTest
	{
		public ImageSetGroupsTests()
		{
		}

		[Test]
		public void Test()
		{
			IImageSet imageSet1 = CreateImageSet("Patient1", "Patient1");
			IImageSet imageSet2 = CreateImageSet("Patient1", "Patient1");
			ImageSetCollection collection = new ImageSetCollection();
			collection.Add(imageSet1);
			collection.Add(imageSet2);

			ImageSetGroups groups = new ImageSetGroups(collection);
			Assert.IsTrue(groups.Root.Items.Count == 0);
			Assert.IsTrue(groups.Root.ChildGroups.Count == 1);
			Assert.IsTrue(groups.Root.ChildGroups[0].Items.Count == 2);

			imageSet1.Dispose();
			imageSet2.Dispose();
		}

		private IImageSet CreateImageSet(string patientId, string description)
		{
			string studyInstanceUid = DicomUid.GenerateUid().UID;
			string seriesInstanceUid = DicomUid.GenerateUid().UID;
			string sopInstanceUid = DicomUid.GenerateUid().UID;

			ImageSop sop = CreateImageSop(patientId, studyInstanceUid, seriesInstanceUid, sopInstanceUid);
			DicomGrayscalePresentationImage img = new DicomGrayscalePresentationImage(sop.Frames[1]);
			sop.Dispose();

			DisplaySet displaySet = new DisplaySet(patientId, seriesInstanceUid);
			displaySet.PresentationImages.Add(img);
			ImageSet imageSet = new ImageSet();
			imageSet.PatientInfo = description;
			imageSet.DisplaySets.Add(displaySet);

			return imageSet;
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