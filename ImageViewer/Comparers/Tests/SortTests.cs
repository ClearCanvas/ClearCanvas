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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Comparers.Tests
{
	[TestFixture]
	public class SortTests : AbstractTest
	{
		private delegate void TraceDelegate(IEnumerable<IPresentationImage> imagesx);

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{ 
		}

		[Test]
		public void TestSortingDicomImagesByInstanceNumber()
		{
			TestSortingDicomImagesByInstanceAndFrameNumber(false);
		}

		[Test]
		public void TestSortingDicomImagesByInstanceNumberReverse()
		{
			TestSortingDicomImagesByInstanceAndFrameNumber(true);
		}

		[Test]
		public void TestSortingDicomImagesByAcquisitionTime()
		{
			TestSortingDicomImagesByAcquisitionTime(false);
		}

		[Test]
		public void TestSortingDicomImagesByAcquisitionTimeReverse()
		{
			TestSortingDicomImagesByAcquisitionTime(true);
		}

		[Test]
		public void TestSortingDicomImagesBySliceLocation()
		{	
			TestSortingDicomImagesBySliceLocation(false);
		}

		[Test]
		public void TestSortingDicomImagesBySliceLocationReverse()
		{
			TestSortingDicomImagesBySliceLocation(true);
		}

		[Test]
		public void TestSortingDisplaySetsBySeriesNumber()
		{
			TestSortingDisplaySetsBySeriesNumber(false);
		}

		[Test]
		public void TestSortingDisplaySetsBySeriesNumberReverse()
		{
			TestSortingDisplaySetsBySeriesNumber(true);
		}

		[Test]
		public void TestSortingImageSetsByStudyDateOld()
		{
			TestSortingImageSetsByStudyDate(false, true);
		}

		[Test]
		public void TestSortingImageSetsByStudyDateReverseOld()
		{
			TestSortingImageSetsByStudyDate(true, true);
		}

		[Test]
		public void TestSortingImageSetsByStudyDate()
		{
			TestSortingImageSetsByStudyDate(false, true);
		}

		[Test]
		public void TestSortingImageSetsByStudyDateReverse()
		{
			TestSortingImageSetsByStudyDate(true, true);
		}

		[Test]
		public void TestLayoutManagerSortImageSetsByStudyDate()
		{
			TestSortingImageSetsByStudyDate(false, true, true);
		}

		[Test]
		public void TestLayoutManagerSortImageSetsByStudyDateReverse()
		{
			TestSortingImageSetsByStudyDate(true, true, true);
		}

		private void TestSortingImageSetsByStudyDate(bool reverse, bool useSops)
		{
			TestSortingImageSetsByStudyDate(reverse, useSops, false);
		}

		private void TestSortingImageSetsByStudyDate(bool reverse, bool useSops, bool testLayoutManagerSort)
		{
			ImageSetCollection orderedCollection = new ImageSetCollection();
			ImageSetCollection nonOrderedCollection = new ImageSetCollection();
			StudyTree studyTree = new StudyTree();

			for (int i = 0; i <= 20; ++i)
			{
				string id = i.ToString();
				ImageSet imageSet = new ImageSet();
				imageSet.Name = id;

				string studyDate;
				if (i == 0)
					studyDate = "";
				else
					studyDate = String.Format("200801{0}", i.ToString("00"));

				if (useSops)
				{
					DisplaySet displaySet = new DisplaySet(id, id);
					ImageSop sop = NewImageSop(id, id, i);
					imageSet.Uid = sop.StudyInstanceUid;
					studyTree.AddSop(sop);

					IPresentationImage image = new DicomGrayscalePresentationImage(sop.Frames[1]);
					IImageSopProvider sopProvider = (IImageSopProvider)image;

					DicomMessageSopDataSource dataSource = ((DicomMessageSopDataSource)sopProvider.ImageSop.DataSource);
					dataSource.SourceMessage.DataSet[DicomTags.StudyDate].SetString(0, studyDate);
					imageSet.DisplaySets.Add(displaySet);
					displaySet.PresentationImages.Add(image);
				}
				else
				{
					StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
					identifier.StudyDate = studyDate;
					identifier.StudyInstanceUid = id;
					ImageSetDescriptor descriptor = new DicomImageSetDescriptor(identifier);
					imageSet.Descriptor = descriptor;
				}

				orderedCollection.Add(imageSet);
			}

			if (reverse)
			{
				List<IImageSet> temp = new List<IImageSet>();
				temp.AddRange(orderedCollection);
				temp.Reverse();
				orderedCollection.Clear();
				foreach (IImageSet imageSet in temp)
					orderedCollection.Add(imageSet);
			}

			Randomize(orderedCollection, nonOrderedCollection);

			Debug.WriteLine("Before Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection,
			                        imageSet => Debug.WriteLine(String.Format("name: {0}, date: {1}", imageSet.Name,
																	((IImageSopProvider)(imageSet.DisplaySets[0].PresentationImages[0])).
																		ImageSop.StudyDate)));

			if (testLayoutManagerSort)
			{
				LayoutManager.SortImageSets(nonOrderedCollection, GetStudies(orderedCollection, studyTree));
			}
			else
			{
				nonOrderedCollection.Sort(new StudyDateComparer(reverse));
			}

			Debug.WriteLine("\nAfter Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection,
			                        imageSet => Debug.WriteLine(String.Format("name: {0}, date: {1}", imageSet.Name,
																  ((IImageSopProvider)(imageSet.DisplaySets[0].PresentationImages[0])).
                                                      				ImageSop.StudyDate)));

			if (reverse)
				nonOrderedCollection.RemoveAt(20);
			else 
				nonOrderedCollection.RemoveAt(0);

			int j = reverse ? 20 : 1;
			foreach (IImageSet set in nonOrderedCollection)
			{
				Assert.AreEqual(j.ToString(), set.Name);
				j += reverse ? -1 : 1;
			}

			foreach (IImageSet set in nonOrderedCollection)
				set.Dispose();
			foreach (IImageSet set in orderedCollection)
				set.Dispose();

			studyTree.Dispose();
		}

		private List<Study> GetStudies(IEnumerable<IImageSet> imageSets, StudyTree studyTree)
		{
			List<Study> studies = new List<Study>();
			foreach (IImageSet imageSet in imageSets)
				studies.Add(studyTree.GetStudy(imageSet.Uid));

			return studies;
		}

		private void TestSortingDisplaySetsBySeriesNumber(bool reverse)
		{
			DisplaySetCollection orderedCollection = new DisplaySetCollection();
			DisplaySetCollection nonOrderedCollection = new DisplaySetCollection();

			for (int i = 1; i <= 20; ++i)
			{
				string id = i.ToString();
				DisplaySet displaySet = new DisplaySet(id, id);
				ImageSop sop = NewImageSop(id, id, i);
				IPresentationImage image = new DicomGrayscalePresentationImage(sop.Frames[1]);
				sop.Dispose();
				IImageSopProvider sopProvider = (IImageSopProvider)image;
				DicomMessageSopDataSource dataSource = ((DicomMessageSopDataSource)sopProvider.ImageSop.DataSource);
				dataSource.SourceMessage.DataSet[DicomTags.SeriesNumber].SetInt32(0, i);

				displaySet.PresentationImages.Add(image);
				orderedCollection.Add(displaySet);
			}

			Randomize(orderedCollection, nonOrderedCollection);

			Debug.WriteLine("Before Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection, delegate(IDisplaySet displaySet) { Debug.WriteLine(String.Format("name: {0}", displaySet.Name)); });

			nonOrderedCollection.Sort(new SeriesNumberComparer(reverse));

			Debug.WriteLine("\nAfter Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection, delegate(IDisplaySet displaySet) { Debug.WriteLine(String.Format("name: {0}", displaySet.Name)); });

			int j = reverse ? 20 : 1;
			foreach (IDisplaySet set in nonOrderedCollection)
			{
				Assert.AreEqual(j.ToString(), set.Name);
				j += reverse ? -1 : 1;
			}

			foreach (DisplaySet set in nonOrderedCollection)
				set.Dispose();
			foreach (DisplaySet set in orderedCollection)
				set.Dispose();
		}

		private void TestSortingDicomImagesBySliceLocation(bool reverse)
		{
			PresentationImageCollection orderedCollection = new PresentationImageCollection();
			PresentationImageCollection nonOrderedCollection = new PresentationImageCollection();

			foreach (IPresentationImage image in GetSliceLocationTestImages())
				orderedCollection.Add(image);

			SortImagesAndValidate(orderedCollection, nonOrderedCollection, reverse,
				new SliceLocationComparer(reverse), TraceSliceLocation);

			foreach (PresentationImage image in nonOrderedCollection)
				image.Dispose();
			foreach (PresentationImage image in orderedCollection)
				image.Dispose();
		}

		private void TestSortingDicomImagesByAcquisitionTime(bool reverse)
		{
			PresentationImageCollection orderedCollection = new PresentationImageCollection();
			PresentationImageCollection nonOrderedCollection = new PresentationImageCollection();

			foreach (IPresentationImage image in GetAcquisitionTimeTestImages())
				orderedCollection.Add(image);

			SortImagesAndValidate(orderedCollection, nonOrderedCollection, reverse,
				new SliceLocationComparer(reverse), TraceAcquisitionTime);

			foreach (PresentationImage image in nonOrderedCollection)
				image.Dispose();
			foreach (PresentationImage image in orderedCollection)
				image.Dispose();
		}

		private void TestSortingDicomImagesByInstanceAndFrameNumber(bool reverse)
		{ 
			PresentationImageCollection orderedCollection = new PresentationImageCollection();
			PresentationImageCollection nonOrderedCollection = new PresentationImageCollection();

			AppendCollection(orderedCollection, NewDicomSeries("123", "1", 1, 25));
			
			AppendCollection(orderedCollection, NewDicomSeries("123", "10", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("123", "111", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("123", "456", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("123", "789", 1, 25));

			//Note that the seriesUID are *not* in numerical order.  This is because
			//it is a string comparison that is being done.
			AppendCollection(orderedCollection, NewDicomSeries("a", "1", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("a", "11", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("a", "12", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("a", "6", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("a", "7", 1, 25));

			AppendCollection(orderedCollection, NewDicomSeries("b", "20", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("b", "21", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("b", "33", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("b", "34", 1, 25));
			AppendCollection(orderedCollection, NewDicomSeries("b", "40", 1, 25));

			//just put one of these at the end, it's enough.  We just want to see
			// that non-Dicom images get pushed to one end (depending on forward/reverse).
			orderedCollection.Add(new MockPresentationImage());

			SortImagesAndValidate(orderedCollection, nonOrderedCollection, reverse,
			                      new InstanceAndFrameNumberComparer(reverse), TraceInstanceAndFrameNumbers);

			foreach (PresentationImage image in nonOrderedCollection)
				image.Dispose();
			foreach (PresentationImage image in orderedCollection)
				image.Dispose();
		}

		private IEnumerable<IPresentationImage> GetAcquisitionTimeTestImages()
		{
			foreach (Frame frame in GetAcquisitionTimeTestFrames())
			{
				yield return new DicomGrayscalePresentationImage(frame);
			}
		}

		private IEnumerable<Frame> GetAcquisitionTimeTestFrames()
		{
			int i = 0;
			foreach (DateTime dateTime in GetAquisitionDateTimes())
			{
				ImageSop sop = NewImageSop("123", "123", i++);
				Frame frame = sop.Frames[1];
				DicomMessageSopDataSource dataSource = ((DicomMessageSopDataSource)frame.ParentImageSop.DataSource);

				if (i%2 == 0)
				{
					dataSource.SourceMessage.DataSet[DicomTags.AcquisitionDate].SetStringValue(dateTime.Date.ToString(DateParser.DicomDateFormat));
					dataSource.SourceMessage.DataSet[DicomTags.AcquisitionTime].SetStringValue(dateTime.ToString(TimeParser.DicomFullTimeFormat));
				}
				else
				{
					dataSource.SourceMessage.DataSet[DicomTags.AcquisitionDatetime].SetStringValue(dateTime.ToString(DateTimeParser.DicomFullDateTimeFormat));
				}

				yield return frame;
				sop.Dispose();
			}
		}

		private IEnumerable<DateTime> GetAquisitionDateTimes()
		{
			DateTime theTime = new DateTime(2008, 6, 10, 1, 30, 40, 345);
			for (int i = 0; i < 20; ++i)
			{
				yield return theTime;
				theTime = theTime.AddMilliseconds(10);
			}

			theTime = new DateTime(2008, 6, 11, 2, 20, 45, 345);
			for (int i = 0; i < 20; ++i)
			{
				yield return theTime;
				theTime = theTime.AddHours(5);
			}
		}

		private void TraceAcquisitionTime(IEnumerable<IPresentationImage> images)
		{
			foreach (IPresentationImage image in images)
			{
				if (image is DicomGrayscalePresentationImage)
				{
					DicomGrayscalePresentationImage dicomImage = (DicomGrayscalePresentationImage)image;

					DateTime? acqDate = DateParser.Parse(dicomImage.ImageSop.Frames[1].AcquisitionDate);
					DateTime? acqTime = TimeParser.Parse(dicomImage.ImageSop.Frames[1].AcquisitionTime);
					if (acqDate != null)
						acqDate = acqDate.Value.AddTicks(acqTime.Value.Ticks);
					else
						acqDate = DateTimeParser.Parse(dicomImage.ImageSop.Frames[1].AcquisitionDateTime);

					string line = string.Format("StudyUID: {0}, Series: {1}, Acq.Date/Time: {2}, Instance: {3}, Frame: {4}",
												dicomImage.ImageSop.StudyInstanceUid,
												dicomImage.ImageSop.SeriesInstanceUid,
												acqDate.Value.ToString(DateTimeParser.DicomFullDateTimeFormat),
												dicomImage.ImageSop.InstanceNumber,
												dicomImage.Frame.FrameNumber);
					Debug.WriteLine(line);
				}
				else
				{
					Debug.WriteLine("** Non-Dicom Image **");
				}
			}
		}

		private void TraceSliceLocation(IEnumerable<IPresentationImage> images)
		{
			foreach (IPresentationImage image in images)
			{
				if (image is DicomGrayscalePresentationImage)
				{
					DicomGrayscalePresentationImage dicomImage = (DicomGrayscalePresentationImage)image;
					
					Vector3D normal = dicomImage.Frame.ImagePlaneHelper.GetNormalVector();
					Vector3D positionPatient = dicomImage.Frame.ImagePlaneHelper.ConvertToPatient(
						new PointF((dicomImage.Frame.Columns - 1) / 2F, (dicomImage.Frame.Rows - 1) / 2F));

					Vector3D positionImagePlane = dicomImage.Frame.ImagePlaneHelper.ConvertToImagePlane(positionPatient, Vector3D.Null);
					double zImagePlane = Math.Round(positionImagePlane.Z, 3, MidpointRounding.AwayFromZero);

					string line = string.Format("StudyUID: {0}, Series: {1}, Normal: {2}, zPosition: {3}, Instance: {4}, Frame: {5}", 
						dicomImage.ImageSop.StudyInstanceUid,
						dicomImage.ImageSop.SeriesInstanceUid,
						normal,
						zImagePlane,
						dicomImage.ImageSop.InstanceNumber,
						dicomImage.Frame.FrameNumber);
					Debug.WriteLine(line);
				}
				else
				{
					Debug.WriteLine("** Non-Dicom Image **");
				}
			}
		}

		private IEnumerable<IPresentationImage> GetSliceLocationTestImages()
		{
			foreach (ImageSop sop in GetSliceLocationTestImageSops())
			{
				yield return new DicomGrayscalePresentationImage(sop.Frames[1]);
				sop.Dispose();
			}
		}

		private IEnumerable<ImageSop> GetSliceLocationTestImageSops()
		{
			int i = 0;
			int instanceNumber = 1;
			foreach (ImageOrientationPatient orientation in GetSliceLocationOrientations())
			{
				for (int j = -3; j <= 3; ++j)
				{
					//we're not testing the study, series etc grouping because the 'instance and frame number one does that'
					ImageSop sop = NewImageSop("123", "1", instanceNumber++);

					DicomMessageSopDataSource dataSource = ((DicomMessageSopDataSource)sop.Frames[1].ParentImageSop.DataSource);
					dataSource.SourceMessage.DataSet[DicomTags.ImagePositionPatient].SetStringValue(GetSliceLocationPosition(i, j).ToString());
					dataSource.SourceMessage.DataSet[DicomTags.ImageOrientationPatient].SetStringValue(orientation.ToString());
					yield return sop;
				}

				++i;
			}
		}

		private ImagePositionPatient GetSliceLocationPosition(int i, int j)
		{
			if (i == 0 || i == 5)
			{
				//Sagittal (normal along +-x)
				if (i == 0)
					j *= -1;

				return new ImagePositionPatient(j, 0, 0);
			}
			else if (i == 1 || i == 4)
			{
				//Coronal(normal along +-y)
				if (i == 1)
					j *= -1;

				return new ImagePositionPatient(0, j, 0);
			}
			else
			{
				if (i == 2)
					j *= -1;

				//Axial (normal along +-z)
				return new ImagePositionPatient(0, 0, j);
			}
		}

		private IEnumerable<ImageOrientationPatient> GetSliceLocationOrientations()
		{
			//Sagittal (normal along -x)
			yield return new ImageOrientationPatient(0, -1, 0, 0, 0, 1);
			//Coronal(normal along -y)
			yield return new ImageOrientationPatient(1, 0, 0, 0, 0, 1);
			//Axial (normal along -z)
			yield return new ImageOrientationPatient(-1, 0, 0, 0, 1, 0);
			//Axial (normal along +z)
			yield return new ImageOrientationPatient(1, 0, 0, 0, 1, 0);
			//Coronal(normal along +y)
			yield return new ImageOrientationPatient(-1, 0, 0, 0, 0, 1);
			//Sagittal (normal along +x)
			yield return new ImageOrientationPatient(0, 1, 0, 0, 0, 1);
		}

		private void AppendCollection(PresentationImageCollection collection, IEnumerable<PresentationImage> listImages)
		{
			foreach (PresentationImage image in listImages)
				collection.Add(image);
		}

		private void SortImagesAndValidate(
			PresentationImageCollection orderedCollection, 
			PresentationImageCollection nonOrderedCollection, 
			bool reverse, 
			IComparer<IPresentationImage> comparer,
			TraceDelegate trace)
		{
			if (reverse)
			{
				PresentationImageCollection reversedCollection = new PresentationImageCollection();
				for (int i = orderedCollection.Count - 1; i >= 0; --i)
					reversedCollection.Add(orderedCollection[i]);

				orderedCollection = reversedCollection;
			}

			Randomize(orderedCollection, nonOrderedCollection);

			Debug.WriteLine("NON-ORDERED COLLECTION (PRE-SORT)\n");
			trace(nonOrderedCollection);

			//Be certain it is currently *not* in order.
			Assert.IsFalse(VerifyOrdered(orderedCollection, nonOrderedCollection));

			//Sort it.
			nonOrderedCollection.Sort(comparer);

			Debug.WriteLine("NON-ORDERED COLLECTION (POST-SORT)");
			trace(nonOrderedCollection);

			//It should now be in the proper order.
			Assert.IsTrue(VerifyOrdered(orderedCollection, nonOrderedCollection));
		}

		private void TraceInstanceAndFrameNumbers(IEnumerable<IPresentationImage> collection)
		{
			foreach (IPresentationImage image in collection)
			{
				if (image is DicomGrayscalePresentationImage)
				{
					DicomGrayscalePresentationImage dicomImage = (DicomGrayscalePresentationImage)image;
					string line = string.Format("StudyUID: {0}, Series: {1}, Instance: {2}, Frame: {3}", dicomImage.ImageSop.StudyInstanceUid,
																			dicomImage.ImageSop.SeriesInstanceUid,
																			dicomImage.ImageSop.InstanceNumber, 
																			dicomImage.Frame.FrameNumber);
					Debug.WriteLine(line);
				}
				else
				{
					Debug.WriteLine("** Non-Dicom Image **");
				}
			}
		}

		private bool VerifyOrdered(
			PresentationImageCollection orderedCollection,
			PresentationImageCollection nonOrderedCollection)
		{
			Assert.AreEqual(orderedCollection.Count, nonOrderedCollection.Count);

			int index = 0;
			foreach (PresentationImage orderedImage in orderedCollection)
			{
				IPresentationImage nonOrderedImage = nonOrderedCollection[index];

				if (!(orderedImage is DicomGrayscalePresentationImage) && !(nonOrderedImage is DicomGrayscalePresentationImage))
				{
					++index;
					continue;
				}

				if (!(orderedImage is DicomGrayscalePresentationImage) && (nonOrderedImage is DicomGrayscalePresentationImage))
					return false;

				if ((orderedImage is DicomGrayscalePresentationImage) && !(nonOrderedImage is DicomGrayscalePresentationImage))
					return false;

				DicomGrayscalePresentationImage dicomOrdered = orderedImage as DicomGrayscalePresentationImage;
				DicomGrayscalePresentationImage dicomNonOrdered = nonOrderedImage as DicomGrayscalePresentationImage;

				if (dicomOrdered.ImageSop.StudyInstanceUid != dicomNonOrdered.ImageSop.StudyInstanceUid ||
				    dicomOrdered.ImageSop.SeriesInstanceUid != dicomNonOrdered.ImageSop.SeriesInstanceUid ||
				    dicomOrdered.ImageSop.InstanceNumber != dicomNonOrdered.ImageSop.InstanceNumber)
					return false;

				++index;
			}

			return true;
		}

		private void Randomize<T>(ICollection<T> orderedCollection, ICollection<T> nonOrderedCollection)
		{ 
			ArrayList tempCollection = new ArrayList();
			foreach (T obj in orderedCollection)
				tempCollection.Add(obj);

			Random random = new Random();
			while (tempCollection.Count > 0)
			{
				int index = random.Next(tempCollection.Count);
				T obj = (T)tempCollection[index];
				nonOrderedCollection.Add(obj);
				tempCollection.Remove(obj);
			}
		}

		private List<PresentationImage> NewDicomSeries(string studyUID, string seriesUID, int startInstanceNumber, uint numberInstances)
		{
			Random rand = new Random();

			List<PresentationImage> listImages = new List<PresentationImage>();
			int instanceNumber = (int)startInstanceNumber;
			while (instanceNumber < (startInstanceNumber + numberInstances))
			{
				listImages.AddRange(NewDicomImages(studyUID, seriesUID, instanceNumber, rand.Next(3, 5)));
				++instanceNumber;
			}

			return listImages;
		}

		private IEnumerable<PresentationImage> NewDicomImages(string studyUID, string seriesUID, int instanceNumber, int numberOfFrames)
		{
			ImageSop sop = NewImageSop(studyUID, seriesUID, instanceNumber, numberOfFrames);
			for (int i = 1; i <= numberOfFrames; ++i)
				yield return new DicomGrayscalePresentationImage(sop.Frames[i]);

			sop.Dispose();
		}

		private ImageSop NewImageSop(string studyUID, string seriesUID, int instanceNumber)
		{
			return NewImageSop(studyUID, seriesUID, instanceNumber, 1);
		}

		private ImageSop NewImageSop(string studyUID, string seriesUID, int instanceNumber, int numberOfFrames)
		{
			DicomAttributeCollection dataSet = new DicomAttributeCollection();

			base.SetupMultiframeXA(dataSet, 512, 512, (uint)numberOfFrames);
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), dataSet);
			TestDataSource dataSource = new TestDataSource(file);
			file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyUID);
			file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesUID);
			file.DataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			file.DataSet[DicomTags.InstanceNumber].SetInt32(0, instanceNumber);
			file.DataSet[DicomTags.PixelSpacing].SetStringValue(new PixelSpacing(1, 1).ToString());

			return new ImageSop(dataSource);
		}
	}
}

#endif