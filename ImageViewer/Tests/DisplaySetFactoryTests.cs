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
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class DisplaySetFactoryTests : AbstractTest
	{
		public DisplaySetFactoryTests()
		{ }

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
			MemoryManager.Enabled = false;
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{ }

		private List<TestDataSource> CreateMRStudyDataSources(int numberOfSeries, int instancesPerSeries, string studyInstanceUid)
		{
			IList<DicomAttributeCollection> dataSets = SetupMRSeries(numberOfSeries, instancesPerSeries, studyInstanceUid);
			List<TestDataSource> dataSources = new List<TestDataSource>();

			foreach (DicomAttributeCollection dataSet in dataSets)
			{
				DicomFile file = new DicomFile(null, new DicomAttributeCollection(), dataSet);
				TestDataSource dataSource = new TestDataSource(file);

				//because of an exception that gets thrown from the DateTimeParser
				dataSource._file.DataSet[DicomTags.StudyDate].SetNullValue();
				dataSource._file.DataSet[DicomTags.StudyTime].SetNullValue();
				dataSource._file.DataSet[DicomTags.SeriesDate].SetNullValue();
				dataSource._file.DataSet[DicomTags.SeriesTime].SetNullValue();
				dataSources.Add(dataSource);
			}

			return dataSources;
		}

		private List<Sop> ConvertToSops(List<TestDataSource> dataSources)
		{
			return CollectionUtils.Map<TestDataSource, Sop>(dataSources, Sop.Create);
		}

		private List<TestDataSource> ConvertToDataSources(List<DicomFile> files)
		{
			return CollectionUtils.Map<DicomFile, TestDataSource>(files, file => new TestDataSource(file));
		}

		public StudyTree CreateStudyTree(List<Sop> sops)
		{
			StudyTree studyTree = new StudyTree();
			foreach (Sop sop in sops)
				studyTree.AddSop(sop);
			return studyTree;
		}

		[Test]
		public void TestNoSeriesSplitting()
		{
			const int numberOfSeries = 5;
			const int instancesPerSeries = 10;
			List<TestDataSource> dataSources = CreateMRStudyDataSources(numberOfSeries, instancesPerSeries, "1.2.3");
			StudyTree studyTree = CreateStudyTree(ConvertToSops(dataSources));

			BasicDisplaySetFactory factory = new BasicDisplaySetFactory();
			factory.SetStudyTree(studyTree);

			List<IDisplaySet> allDisplaySets = new List<IDisplaySet>();

			try
			{
				Patient patient = studyTree.Patients[0];
				Study study = patient.Studies[0];

				Assert.AreEqual(numberOfSeries, study.Series.Count, "There should be exactly {0} series", numberOfSeries);

				foreach (Series series in study.Series)
				{
					Assert.AreEqual(instancesPerSeries, series.Sops.Count, "There should be exactly {0} sops", instancesPerSeries);

					List<IDisplaySet> displaySets = factory.CreateDisplaySets(series);
					allDisplaySets.AddRange(displaySets);

					Assert.AreEqual(1, displaySets.Count, "There should be exactly {0} display sets", 1);

					IDisplaySet displaySet = displaySets[0];

					Assert.AreEqual(series.Sops.Count, displaySet.PresentationImages.Count, "#Sops should match #presentation images");
					Assert.AreEqual(series.SeriesInstanceUid, displaySet.Descriptor.Uid, "Series UID and Display Set UID don't match");
					Assert.AreEqual(typeof(SeriesDisplaySetDescriptor), displaySet.Descriptor.GetType(), "Wrong display set descriptor type");
				}
			}
			finally
			{
				foreach (IDisplaySet displaySet in allDisplaySets)
					displaySet.Dispose();

				studyTree.Dispose();
			}
		}

		[Test]
		public void TestNoSeriesSplitting_KeyImageSeries()
		{
			TestKeyImages(5, 2, 6, false);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_MultiImageSeries_NoMultiframes()
		{
			TestSeriesSplittingSingleImage_MixedMultiframeSeries(0, 10);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_MultiImageSeries_AllMultiframes()
		{
			TestSeriesSplittingSingleImage_MixedMultiframeSeries(10, 0);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_MixedMultiframeSeries()
		{
			TestSeriesSplittingSingleImage_MixedMultiframeSeries(3, 4);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_SingleImageSeries()
		{
			TestSeriesSplittingSingleImage_MixedMultiframeSeries(0, 1);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_SingleMultframeImageSeries()
		{
			TestSeriesSplittingSingleImage_MixedMultiframeSeries(1, 0);
		}

		private void TestSeriesSplittingSingleImage_MixedMultiframeSeries(int numberOfMultiframes, int numberOfSingleframes)
		{
			const int numberOfSeries = 1;
			int instancesPerSeries = numberOfMultiframes + numberOfSingleframes;
			const int multiFrameNumberOfFrames = 5;
			List<TestDataSource> dataSources = CreateMRStudyDataSources(numberOfSeries, instancesPerSeries, "1.2.3");
			for (int i = 0; i < numberOfMultiframes; ++i)
			{
				TestDataSource multiFrameDataSource = dataSources[i];
				multiFrameDataSource._file.DataSet[DicomTags.NumberOfFrames].SetInt32(0, multiFrameNumberOfFrames);
			}

			StudyTree studyTree = CreateStudyTree(ConvertToSops(dataSources));
			BasicDisplaySetFactory factory = new BasicDisplaySetFactory { CreateSingleImageDisplaySets = instancesPerSeries > 1 };

		    List<IDisplaySet> allDisplaySets = new List<IDisplaySet>();

			int numberOfMultiframesFound = 0;
			int numberOfSingleframesFound = 0;

			try
			{
				Patient patient = studyTree.Patients[0];
				Study study = patient.Studies[0];

				Assert.AreEqual(numberOfSeries, study.Series.Count, "There should be exactly {0} series", numberOfSeries);

				foreach (Series series in study.Series)
				{
					Assert.AreEqual(instancesPerSeries, series.Sops.Count, "There should be exactly {0} sops", instancesPerSeries);

					List<IDisplaySet> displaySets = factory.CreateDisplaySets(series);
					allDisplaySets.AddRange(displaySets);

					foreach (IDisplaySet displaySet in displaySets)
					{
						ImageSop imageSop = ((IImageSopProvider)displaySet.PresentationImages[0]).ImageSop;
						if (imageSop.NumberOfFrames > 1)
						{
							++numberOfMultiframesFound;
							Assert.AreEqual(multiFrameNumberOfFrames, displaySet.PresentationImages.Count, "There should be {0} presentation image per display set", multiFrameNumberOfFrames);
							if (instancesPerSeries > 1)
							{
								Assert.AreEqual(typeof(MultiframeDisplaySetDescriptor), displaySet.Descriptor.GetType(), "Wrong display set descriptor type");
								Assert.IsTrue(displaySet.Name.Contains("Multiframe #"), "display set name doesn't contain \"Multiframe #\"");
							}
						}
						else
						{
							++numberOfSingleframesFound;
							Assert.AreEqual(1, displaySet.PresentationImages.Count, "There should be only one presentation image per display set");
							if (instancesPerSeries > 1)
							{
								Assert.AreEqual(typeof(SingleImageDisplaySetDescriptor), displaySet.Descriptor.GetType(), "Wrong display set descriptor type");
								Assert.IsTrue(displaySet.Name.Contains("Image #"), "display set name doesn't contain \"Image #\"");
							}
						}

						if (instancesPerSeries == 1)
						{
							if (numberOfMultiframes == 1)
								Assert.AreEqual(multiFrameNumberOfFrames, displaySet.PresentationImages.Count, "There should be one presentation image per frame");
							else
								Assert.AreEqual(1, displaySet.PresentationImages.Count, "There should be only one presentation image per display set");

							Assert.AreEqual(typeof(SeriesDisplaySetDescriptor), displaySet.Descriptor.GetType(), "Wrong display set descriptor type");
						}
					}
				}
			}
			finally
			{
				foreach (IDisplaySet displaySet in allDisplaySets)
					displaySet.Dispose();

				studyTree.Dispose();
			}

			Assert.AreEqual(numberOfMultiframes, numberOfMultiframesFound, "Incorrect number of multiframes");
			Assert.AreEqual(numberOfSingleframes, numberOfSingleframesFound, "Incorrect number of singleframes");
		}

		[Test]
		public void TestSeriesSplittingSingleImage_KeyImageSeries_SingleImage()
		{
			TestKeyImages(0, 0, 1, true);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_KeyImageSeries_SingleFrame()
		{
			TestKeyImages(5, 1, 0, true);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_KeyImageSeries_MultipleFramesAndImages()
		{
			TestKeyImages(5, 2, 6, true);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_KeyImageSeries_MultipleImages()
		{
			TestKeyImages(0, 0, 6, true);
		}

		[Test]
		public void TestSeriesSplittingSingleImage_KeyImageSeries_MultipleFrames()
		{
			TestKeyImages(5, 2, 0, true);
		}

		public void TestKeyImages(int numberOfFrames, int numberOfMultiframeKeyImages, int numberOfSingleFrameKeyImages, bool doSplitting)
		{
			Assert.IsTrue(numberOfFrames == 0 || numberOfFrames > 1);
			Assert.IsTrue(numberOfMultiframeKeyImages <= numberOfFrames);

			const int numberOfSeries = 1;
			int instancesPerSeries = numberOfSingleFrameKeyImages + ((numberOfFrames > 0) ? 1:0);

			Assert.IsTrue(instancesPerSeries > 0);

			List<TestDataSource> dataSources = CreateMRStudyDataSources(numberOfSeries, instancesPerSeries, "1.2.3");
			if (numberOfFrames > 0)
			{
				TestDataSource multiFrameDataSource = dataSources[0];
				DicomAttributeCollection oldDataSet = multiFrameDataSource._file.DataSet;
				DicomAttributeCollection newDataSet = new DicomAttributeCollection();
				DicomFile newFile = new DicomFile("", new DicomAttributeCollection(), newDataSet);
				//Yes this is the world's crappiest hack.
				base.SetupMultiframeXA(newDataSet, 512,512, (uint)numberOfFrames);
				//because of an exception that gets thrown from the DateTimeParser
				newDataSet[DicomTags.StudyDate].SetNullValue();
				newDataSet[DicomTags.StudyTime].SetNullValue();
				newDataSet[DicomTags.SeriesDate].SetNullValue();
				newDataSet[DicomTags.SeriesTime].SetNullValue();
				newDataSet[DicomTags.ReferencedStudySequence].SetEmptyValue();
				newDataSet[DicomTags.Modality].SetStringValue("MR");
				newDataSet[DicomTags.StudyInstanceUid].SetStringValue(oldDataSet[DicomTags.StudyInstanceUid].ToString());
				newDataSet[DicomTags.SeriesInstanceUid].SetStringValue(oldDataSet[DicomTags.SeriesInstanceUid].ToString());
				dataSources[0] = new TestDataSource(newFile);
			}

			StudyTree studyTree = CreateStudyTree(ConvertToSops(dataSources));
			KeyImageSerializer serializer = new KeyImageSerializer();

			Patient patient = studyTree.Patients[0];
			Study study = patient.Studies[0];
			Series sourceSeries = study.Series[0];

			List<IDisplaySet> allDisplaySets = new List<IDisplaySet>();

			BasicDisplaySetFactory factory = new BasicDisplaySetFactory();
			factory.SetStudyTree(studyTree);

			List<IDisplaySet> displaySets = factory.CreateDisplaySets(sourceSeries);
			allDisplaySets.AddRange(displaySets);

			List<DicomFile> presentationStates = new List<DicomFile>();
			int numberOfMultiframeKeyImagesCreated = 0;
			foreach (IDisplaySet displaySet in displaySets)
			{
				foreach (IPresentationImage image in displaySet.PresentationImages)
				{
					Frame frame = ((IImageSopProvider)image).Frame;
					if (frame.ParentImageSop.NumberOfFrames > 1)
					{
						if(numberOfMultiframeKeyImagesCreated >= numberOfMultiframeKeyImages)
							continue;

						++numberOfMultiframeKeyImagesCreated;
					}

					DicomSoftcopyPresentationState presentationState = DicomSoftcopyPresentationState.Create(image);
					//because of an exception that gets thrown from the DateTimeParser
					presentationState.DicomFile.DataSet[DicomTags.StudyDate].SetNullValue();
					presentationState.DicomFile.DataSet[DicomTags.StudyTime].SetNullValue();
					presentationState.DicomFile.DataSet[DicomTags.SeriesDate].SetNullValue();
					presentationState.DicomFile.DataSet[DicomTags.SeriesTime].SetNullValue();

					presentationStates.Add(presentationState.DicomFile);
					serializer.AddImage(frame, presentationState);
				}
			}

			List<DicomFile> files = serializer.Serialize();
			List<TestDataSource> keyImageDataSources = ConvertToDataSources(files);
			List<Sop> keyImageSops = ConvertToSops(keyImageDataSources);
			keyImageSops.AddRange(ConvertToSops(ConvertToDataSources(presentationStates)));

			foreach (Sop sop in keyImageSops)
				studyTree.AddSop(sop);

			try
			{
				foreach (Series series in study.Series)
				{
					if (series.Modality != "KO")
						continue;

				    List<IDisplaySet> keyImageDisplaySets;
                    if (doSplitting)
                    {
                        factory.CreateSingleImageDisplaySets = true;
                        keyImageDisplaySets = factory.CreateDisplaySets(series);
                        if (keyImageDisplaySets.Count == 0)
                        {
                            factory.CreateSingleImageDisplaySets = false;
                            keyImageDisplaySets = factory.CreateDisplaySets(series);
                        }
                    }
                    else
                    {
                        keyImageDisplaySets = factory.CreateDisplaySets(series);
                    }

					allDisplaySets.AddRange(keyImageDisplaySets);

					int numberOfKeyImages = numberOfMultiframeKeyImages + numberOfSingleFrameKeyImages;
					if (!doSplitting)
					{
						Assert.AreEqual(1, keyImageDisplaySets.Count, "There should be only one display set");
						IDisplaySet keyImageDisplaySet = keyImageDisplaySets[0];
						Assert.AreEqual(numberOfKeyImages, keyImageDisplaySet.PresentationImages.Count, "Expected {0} images", numberOfKeyImages);
						Assert.AreEqual(typeof(SeriesDisplaySetDescriptor), keyImageDisplaySet.Descriptor.GetType(), "Wrong display set descriptor type");
					}
					else
					{
						Assert.AreEqual(numberOfKeyImages, keyImageDisplaySets.Count, "Expected {0} display sets", numberOfKeyImages);

						foreach (IDisplaySet keyImageDisplaySet in keyImageDisplaySets)
						{
							Assert.AreEqual(1, keyImageDisplaySet.PresentationImages.Count, "There should be only one presentation image");
							IPresentationImage keyImage = keyImageDisplaySet.PresentationImages[0];
							ImageSop sop = ((IImageSopProvider)keyImage).ImageSop;

							Assert.AreEqual(sourceSeries.SeriesInstanceUid, sop.SeriesInstanceUid, "Series Instance Uid is not that of the source series");
							if (numberOfKeyImages == 1)
								Assert.AreEqual(typeof(SeriesDisplaySetDescriptor), keyImageDisplaySet.Descriptor.GetType(), "Wrong display set descriptor type");
							else if (sop.NumberOfFrames > 1)
								Assert.AreEqual(typeof(SingleFrameDisplaySetDescriptor), keyImageDisplaySet.Descriptor.GetType(), "Wrong display set descriptor type");
							else
								Assert.AreEqual(typeof(SingleImageDisplaySetDescriptor), keyImageDisplaySet.Descriptor.GetType(), "Wrong display set descriptor type");
						}
					}
				}
			}
			finally
			{
				foreach (IDisplaySet displaySet in allDisplaySets)
					displaySet.Dispose();

				studyTree.Dispose();
			}
		}


		[Test]
		public void TestSeriesSplittingMREcho()
		{
			const int numberOfSeries = 5;
			const int instancesPerSeries = 10;
			List<TestDataSource> dataSources = CreateMRStudyDataSources(numberOfSeries, instancesPerSeries, "1.2.3");
			StudyTree studyTree = CreateStudyTree(ConvertToSops(dataSources));

			MREchoDisplaySetFactory factory = new MREchoDisplaySetFactory();
			factory.SetStudyTree(studyTree);
			List<IDisplaySet> allDisplaySets = new List<IDisplaySet>();

			try
			{
				Patient patient = studyTree.Patients[0];
				Study study = patient.Studies[0];

				Assert.AreEqual(numberOfSeries, study.Series.Count, "There should be exactly {0} series", numberOfSeries);

				Series series2 = study.Series[1];
				Series series3 = study.Series[2];

				MakeEchoSeries(series2);
				MakeEchoSeries(series3);

				foreach (Series series in study.Series)
				{
					Assert.AreEqual(instancesPerSeries, series.Sops.Count, "There should be exactly {0} sops", instancesPerSeries);
					List<IDisplaySet> displaySets = factory.CreateDisplaySets(series);
					allDisplaySets.AddRange(displaySets);

					if (series == series2 || series == series3)
					{
						IDisplaySet displaySet = displaySets[0];
						Assert.AreEqual(2, displaySets.Count, "There should be exactly 2 display sets");
						Assert.AreEqual(series.Sops.Count / 2, displaySet.PresentationImages.Count, "#presentation images should be #Sops/2");
						Assert.AreNotEqual(series.SeriesInstanceUid, displaySet.Descriptor.Uid, "Series UID and Display Set UID don't match");
						Assert.AreEqual(typeof(MREchoDisplaySetDescriptor), displaySet.Descriptor.GetType(), "Wrong display set descriptor type");
						Assert.IsTrue(displaySet.Name.Contains("Echo"), "Display Set name not correct");

						ValidateEchoDisplaySet(displaySets[0], 1);
						ValidateEchoDisplaySet(displaySets[1], 2);
					}
					else
					{
						Assert.AreEqual(0, displaySets.Count, "There should be no display sets");
					}
				}
			}
			finally
			{
				foreach (IDisplaySet displaySet in allDisplaySets)
					displaySet.Dispose();

				studyTree.Dispose();
			}
		}

		private void MakeEchoSeries(Series series)
		{
			int i = 0;
			foreach (Sop sop in series.Sops)
			{
				TestDataSource dataSource = (TestDataSource)sop.DataSource;
				int echoNumber = (++i <= 5) ? 1 : 2;
				dataSource._file.DataSet[DicomTags.EchoNumbers].SetInt32(0, echoNumber);
			}
		}

		private void ValidateEchoDisplaySet(IDisplaySet displaySet, int echoNumber)
		{
			string seriesInstanceUid = null;
			foreach (IPresentationImage presentationImage in displaySet.PresentationImages)
			{
				ImageSop sop = ((IImageSopProvider)presentationImage).ImageSop;
				seriesInstanceUid = sop.SeriesInstanceUid;
				Assert.AreEqual(echoNumber, sop[DicomTags.EchoNumbers].GetUInt32(0, 0), "Echo number must be {0} for each image in the series", echoNumber);
			}

			Assert.IsNotNull(seriesInstanceUid);
			Assert.AreEqual(String.Format("{0}:Echo{1}", seriesInstanceUid, echoNumber), displaySet.Uid);
		}

		[Test]
		public void TestSplitMixedMultiframeSeries_OneMultiframeOnly()
		{
			TestMixedMultiframes(0, new[] { 5 });
		}

		[Test]
		public void TestSplitMixedMultiframeSeries_OneSingleImageOnly()
		{
			TestMixedMultiframes(1, new int[0]);
		}

		[Test]
		public void TestSplitMixedMultiframeSeries_OneMultiframeOneSingleFrame()
		{
			TestMixedMultiframes(1, new[] { 5 });
		}

		[Test]
		public void TestSplitMixedMultiframeSeries_AllMultiframes()
		{
			TestMixedMultiframes(0, new[] { 5, 10, 15 });
		}

		[Test]
		public void TestSplitMixedMultiframeSeries_OneMultiframeSomeSingleFrames()
		{
			TestMixedMultiframes(8, new[] { 5 });
		}

		[Test]
		public void TestSplitMixedMultiframeSeries_SomeMultiframesSomeSingleFrames()
		{
			TestMixedMultiframes(8, new[] { 5, 10, 15 });
		}
		
		public void TestMixedMultiframes(int numberOfSingleFrames, int[] multiFramesNumberOfFrames)
		{
			const int numberOfSeries = 1;
			int instancesPerSeries = numberOfSingleFrames + multiFramesNumberOfFrames.Length;
			int numberOfMultiframes = multiFramesNumberOfFrames.Length;

			List<TestDataSource> dataSources = CreateMRStudyDataSources(numberOfSeries, instancesPerSeries, "1.2.3");
			for (int i = 0; i < numberOfMultiframes; i++)
				dataSources[i]._file.DataSet[DicomTags.NumberOfFrames].SetInt32(0, multiFramesNumberOfFrames[i]);

			StudyTree studyTree = CreateStudyTree(ConvertToSops(dataSources));

			MixedMultiFrameDisplaySetFactory factory = new MixedMultiFrameDisplaySetFactory();
			factory.SetStudyTree(studyTree);
			List<IDisplaySet> allDisplaySets = new List<IDisplaySet>();
			
			Patient patient = studyTree.Patients[0];
			Study study = patient.Studies[0];
			Series series = study.Series[0];

			if (instancesPerSeries == 1)
			{
				try
				{
					Assert.AreEqual(instancesPerSeries, series.Sops.Count, "There should be exactly {0} sops", instancesPerSeries);

					List<IDisplaySet> displaySets = factory.CreateDisplaySets(series);
					allDisplaySets.AddRange(displaySets);

					Assert.AreEqual(0, displaySets.Count, "This scenario should produce no display sets");
				}
				finally
				{
					foreach (IDisplaySet displaySet in allDisplaySets)
						displaySet.Dispose();

					studyTree.Dispose();
				}
			}
			else
			{
				try
				{
					Assert.AreEqual(instancesPerSeries, series.Sops.Count, "There should be exactly {0} sops", instancesPerSeries);

					List<IDisplaySet> displaySets = factory.CreateDisplaySets(series);
					allDisplaySets.AddRange(displaySets);

					if (series.SeriesInstanceUid == dataSources[0].SeriesInstanceUid)
					{
						int numberOfDisplaySets = numberOfSingleFrames > 0 ? 1 : 0;
						numberOfDisplaySets += numberOfMultiframes;
						Assert.AreEqual(numberOfDisplaySets, displaySets.Count, "There should be exactly {0} display sets", numberOfDisplaySets);

						if (numberOfSingleFrames > 0)
						{
							IDisplaySet singleFramesDisplaySet = displaySets[0];
							Assert.AreEqual(numberOfSingleFrames, singleFramesDisplaySet.PresentationImages.Count, "#Presentation images should match #singe frames in series");
							Assert.AreEqual(typeof(SingleImagesDisplaySetDescriptor), singleFramesDisplaySet.Descriptor.GetType(), "Wrong display set descriptor type");
							Assert.AreEqual(String.Format("{0}:SingleImages", series.SeriesInstanceUid), singleFramesDisplaySet.Uid);
						}

						int multiFramesStartIndex = numberOfSingleFrames > 0 ? 1 : 0;
						for (int i = multiFramesStartIndex; i < displaySets.Count; i++)
						{
							IDisplaySet multiFrameDisplaySet = displaySets[i];
							int numberOfFrames = multiFramesNumberOfFrames[i - multiFramesStartIndex];
							ImageSop multiFrame = ((IImageSopProvider)multiFrameDisplaySet.PresentationImages[0]).ImageSop;
							Assert.AreEqual(numberOfFrames, multiFrameDisplaySet.PresentationImages.Count, "#Presentation images should match #frames in multiframe");
							Assert.AreEqual(typeof(MultiframeDisplaySetDescriptor), multiFrameDisplaySet.Descriptor.GetType(), "Wrong display set descriptor type");
							Assert.AreEqual(multiFrame.SopInstanceUid, multiFrameDisplaySet.Uid);
						}
					}
					else
					{
						Assert.AreEqual(0, displaySets.Count, "There should be no display sets");
					}
				}
				finally
				{
					foreach (IDisplaySet displaySet in allDisplaySets)
						displaySet.Dispose();

					studyTree.Dispose();
				}
			}
		}
	}
}

#endif