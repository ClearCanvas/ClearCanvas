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

using System;
using System.Diagnostics;
using System.Linq;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Layout.Basic.Tests
{
	[TestFixture]
	public class ImageSetFillerTests
	{
		[Test]
		public void TestCT_Typical()
		{
			TestCT(2);
		}

		[Test]
		public void TestMR_Typical()
		{
			//Split, Show Original Echo Series, Show Original Series
			TestMR(true, true, true, 2);
			TestMR(true, false, true, 2);

			TestMR(false, true, true, 2);
			TestMR(false, false, true, 2);
		}

		[Test]
		[Ignore("See #9464. These aren't *really* valid tests because the ShowOriginalSeries flag is never false in these cases.")]
		public void TestMR_Typical_KnownFailures()
		{
			//Not really valid because the ShowOriginalSeries flag is never false in these cases

			//Split, Show Original Echo Series, Show Original Series
			TestMR(true, true, false, 2);
			TestMR(true, false, false, 2);
			TestMR(false, true, false, 2);
			TestMR(false, false, false, 2);
		}

		[Test]
		public void TestMREchos_NoSplitting()
		{
			const int numSeries = 2;

			//Split, Show Original Echo Series, Show Original Series
			TestMREcho(false, false, true, numSeries);
			TestMREcho(false, true, true, numSeries);
		}

		[Test]
		[Ignore("See #9464. These aren't *really* valid tests because the ShowOriginalSeries flag is never false in these cases.")]
		public void TestMREchos_NoSplitting_KnownFailures()
		{
			const int numSeries = 2;

			//Not really valid because the ShowOriginalSeries flag is never false in these cases

			//Split, Show Original Echo Series, Show Original Series
			TestMREcho(false, true, false, numSeries);
			TestMREcho(false, false, false, numSeries);
		}

		[Test]
		public void TestMREchos_WithSplitting()
		{
			const int numSeries = 2;
			const int numEchoDisplaySets = 2*numSeries;

			//Split, Show Original Echo Series, Show Original Series
			TestMREcho(true, false, true, numEchoDisplaySets);
			TestMREcho(true, false, false, numEchoDisplaySets);
		}

		[Test]
		[Ignore("See #9464.")]
		public void TestMREchos_WithSplitting_KnownFailures()
		{
			const int numSeries = 2;
			const int numEchoDisplaySets = 2*numSeries;

			//Split, Show Original Echo Series, Show Original Series
			TestMREcho(true, true, false, numEchoDisplaySets + numSeries);
			TestMREcho(true, true, true, numEchoDisplaySets + numSeries);
		}

		[Test]
		public void TestMixedMultiframe_NoSplitting()
		{
			const int numSingleImages = 2;
			const int numMultiframeImages = 2;
			const int numSeries = 1;

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, true, true, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(false, false, true, numSingleImages, numMultiframeImages, numSeries);
		}

		[Test]
		[Ignore("See #9464. These aren't *really* valid tests because the ShowOriginalSeries flag is never false in these cases.")]
		public void TestMixedMultiframe_NoSplitting_KnownFailures()
		{
			const int numSingleImages = 2;
			const int numMultiframeImages = 2;
			const int numSeries = 1;

			//Not really valid because the ShowOriginalSeries flag is never false in these cases

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, true, false, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(false, false, false, numSingleImages, numMultiframeImages, numSeries);
		}

		[Test]
		public void TestMixedMultiframe_NoSplitting_NoSingleImages()
		{
			const int numSingleImages = 0;
			const int numMultiframeImages = 2;
			const int numSeries = 1;

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, true, true, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(false, false, true, numSingleImages, numMultiframeImages, numSeries);
		}

		[Test]
		[Ignore("See #9464. These aren't *really* valid tests because the ShowOriginalSeries flag is never false in these cases.")]
		public void TestMixedMultiframe_NoSplitting_NoSingleImages_KnownFailures()
		{
			const int numSingleImages = 0;
			const int numMultiframeImages = 2;
			const int numSeries = 1;

			//Not really valid because the ShowOriginalSeries flag is never false in these cases

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, true, false, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(false, false, false, numSingleImages, numMultiframeImages, numSeries);
		}

		[Test]
		public void TestMixedMultiframe_WithSplitting()
		{
			const int numSingleImages = 2;
			const int numMultiframeImages = 2;
			//1 for the single images, 1 each for multiframes
			const int expectedDisplaySets = 1 + numMultiframeImages;

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(true, false, true, numSingleImages, numMultiframeImages, expectedDisplaySets);
			TestMixedMultiframe(true, false, false, numSingleImages, numMultiframeImages, expectedDisplaySets);
		}

		[Test]
		[Ignore("See #9464.")]
		public void TestMixedMultiframe_WithSplitting_KnownFailures()
		{
			const int numSingleImages = 2;
			const int numMultiframeImages = 2;
			//1 for the single images, 1 for the original series, 1 each for multiframes
			const int expectedDisplaySets = 1 + 1 + numMultiframeImages;

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(true, true, false, numSingleImages, numMultiframeImages, expectedDisplaySets);
			TestMixedMultiframe(true, true, true, numSingleImages, numMultiframeImages, expectedDisplaySets);
		}

		[Test]
		public void TestMixedMultiframe_WithSplitting_NoSingleImages()
		{
			const int numSingleImages = 0;
			const int numMultiframeImages = 2;
			const int numSeries = 1;

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(true, false, true, numSingleImages, numMultiframeImages, numMultiframeImages);
			TestMixedMultiframe(true, true, true, numSingleImages, numMultiframeImages, numMultiframeImages + numSeries);
		}

		[Test]
		[Ignore("See #9464. These aren't *really* valid tests because the ShowOriginalSeries flag is never false in these cases.")]
		public void TestMixedMultiframe_WithSplitting_NoSingleImages_KnownFailures()
		{
			const int numSingleImages = 0;
			const int numMultiframeImages = 2;
			//1 for the original series, 1 each for multiframes
			const int expectedDisplaySets = numMultiframeImages + 1;

			//Not really valid because the ShowOriginalSeries flag is never false in these cases

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(true, false, false, numSingleImages, numMultiframeImages, expectedDisplaySets);
			TestMixedMultiframe(true, true, false, numSingleImages, numMultiframeImages, expectedDisplaySets);
		}

		[Test]
		public void TestUltrasound_SingleImages()
		{
			const int numSingleImages = 2;
			const int numMultiframeImages = 0;
			const int numSeries = 1;

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, false, true, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(false, true, true, numSingleImages, numMultiframeImages, numSeries);

			TestMixedMultiframe(true, false, true, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(true, true, true, numSingleImages, numMultiframeImages, numSeries);
		}

		[Test]
		[Ignore("See #9464. These aren't *really* valid tests because the ShowOriginalSeries flag is never false in these cases.")]
		public void TestUltrasound_SingleImages_KnownFailures()
		{
			const int numSingleImages = 2;
			const int numMultiframeImages = 0;
			const int numSeries = 1;

			//Not really valid because the ShowOriginalSeries flag is never false in these cases

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, true, false, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(false, false, false, numSingleImages, numMultiframeImages, numSeries);

			TestMixedMultiframe(true, false, false, numSingleImages, numMultiframeImages, numSeries);
			TestMixedMultiframe(true, true, false, numSingleImages, numMultiframeImages, numSeries);
		}

		[Test]
		public void TestUltrasound_Multiframe()
		{
			const int numSingleImages = 0;
			const int numMultiframeImages = 1;

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, false, true, numSingleImages, numMultiframeImages, numMultiframeImages);
			TestMixedMultiframe(false, true, true, numSingleImages, numMultiframeImages, numMultiframeImages);

			TestMixedMultiframe(true, false, true, numSingleImages, numMultiframeImages, numMultiframeImages);
			TestMixedMultiframe(true, true, true, numSingleImages, numMultiframeImages, numMultiframeImages);
		}

		[Test]
		[Ignore("See #9464. These aren't *really* valid tests because the ShowOriginalSeries flag is never false in these cases.")]
		public void TestUltrasound_Multiframe_KnownFailures()
		{
			const int numSingleImages = 0;
			const int numMultiframeImages = 1;

			//Not really valid because the ShowOriginalSeries flag is never false in these cases

			//Split, Show Original MMF Series, Show Original Series
			TestMixedMultiframe(false, true, false, numSingleImages, numMultiframeImages, numMultiframeImages);
			TestMixedMultiframe(false, false, false, numSingleImages, numMultiframeImages, numMultiframeImages);

			TestMixedMultiframe(true, false, false, numSingleImages, numMultiframeImages, numMultiframeImages);
			TestMixedMultiframe(true, true, false, numSingleImages, numMultiframeImages, numMultiframeImages);
		}

		[Test]
		public void TestDX_OneSeries_OneImage()
		{
			//One Series, One Image Per Series | Create Single, Create All, Show Original
			TestDX(true, true, true, true, true, 1);
			//TestDX(true, true, false, false, false, 1);

			TestDX(true, true, true, false, false, 1);
			TestDX(true, true, false, true, false, 1);
			TestDX(true, true, false, false, true, 1);

			TestDX(true, true, true, true, false, 1);
			TestDX(true, true, false, true, true, 1);
			TestDX(true, true, true, false, true, 1);
		}

		[Test]
		public void TestDX_TwoSeries_OneImageEach()
		{
			//One Series, One Image Per Series | Create Single, Create All, Show Original
			TestDX(false, true, true, true, true, 3);
			//TestDX(false, true, false, false, false, 1);

			TestDX(false, true, true, false, false, 2);
			TestDX(false, true, false, true, false, 1);
			TestDX(false, true, false, false, true, 2);

			TestDX(false, true, true, true, false, 3);
			TestDX(false, true, false, true, true, 3);
			TestDX(false, true, true, false, true, 2);
		}

		[Test]
		public void TestDX_OneSeries_TwoImages()
		{
			//One Series, One Image Per Series | Create Single, Create All, Show Original
			TestDX(true, false, true, true, true, 3);
			//TestDX(true, false, false, false, false, 1);

			TestDX(true, false, true, false, false, 2);
			TestDX(true, false, false, true, false, 1);
			TestDX(true, false, false, false, true, 1);

			TestDX(true, false, true, true, false, 3);
			TestDX(true, false, false, true, true, 1);
			TestDX(true, false, true, false, true, 3);
		}

		[Test]
		public void TestMG_TwoSeries_FourImagesEach()
		{
			//Create Single, Create All, Show Original
			TestMG(true, true, true, 11);
			//TestMG(false, false, false, 2);

			TestMG(true, false, false, 8);
			TestMG(false, true, false, 1);
			TestMG(false, false, true, 2);

			TestMG(true, true, false, 9);
			TestMG(false, true, true, 3);
			TestMG(true, false, true, 10);
		}

		public void TestMG(bool createSingleImages, bool createAllImages, bool showOriginal, int expectedDisplaySetCount)
		{
			Trace.WriteLine(String.Format("MG: createSingleImages={0}, createAllImages={1}, showOriginal={2}"
			                              , createSingleImages, createAllImages, showOriginal));

			var studyTree = new StudyTree();
			var builder = StudyBuilderFactory.CreateDigitalMammoBuilder("Patient1", "Test^Patient", "Breast", null, "Bilateral Mammo", 33, false);
			Assert.AreEqual(2, builder.Series.Count);
			foreach (var series in builder.Series)
				Assert.AreEqual(4, series.Images.Count);

			builder.AddStudy(studyTree);

			var options = new DisplaySetCreationOptions();
			var o = (StoredDisplaySetCreationSetting) options["MG"];
			o.CreateAllImagesDisplaySet = createAllImages;
			o.CreateSingleImageDisplaySets = createSingleImages;
			o.ShowOriginalSeries = showOriginal;

			Test(studyTree, builder, options, createAllImages, expectedDisplaySetCount);
		}

		public void TestDX(bool oneSeries, bool oneImagePerSeries, bool createSingleImages, bool createAllImages, bool showOriginal, int expectedDisplaySetCount)
		{
			Trace.WriteLine(String.Format("DX: oneSeries={0}, oneImagePerSeries={1}, createSingleImages={2}, createAllImages={3}, showOriginal={4}"
			                              , oneSeries, oneImagePerSeries, createSingleImages, createAllImages, showOriginal));

			var studyTree = new StudyTree();
			var builder = StudyBuilderFactory.CreateDigitalXRayBuilder("Patient1", "Test^Patient", "Chest", null,
			                                                           "Chest", 3, oneSeries, oneImagePerSeries, false);
			var seriesCount = oneSeries ? 1 : 2;
			var imageCount = oneImagePerSeries ? 1 : 2;
			Assert.AreEqual(seriesCount, builder.Series.Count);
			foreach (var series in builder.Series)
				Assert.AreEqual(imageCount, series.Images.Count);

			builder.AddStudy(studyTree);

			var options = new DisplaySetCreationOptions();
			var o = (StoredDisplaySetCreationSetting) options["DX"];
			o.CreateAllImagesDisplaySet = createAllImages;
			o.CreateSingleImageDisplaySets = createSingleImages;
			o.ShowOriginalSeries = showOriginal;

			Test(studyTree, builder, options, createAllImages && seriesCount > 1, expectedDisplaySetCount);
		}

		public void TestCT(int expectedDisplaySetCount)
		{
			var studyTree = new StudyTree();
			var builder = StudyBuilderFactory.CreateCTChestBuilder("Patient1", "Test^Patient", "Chest", null, "Scout", "Axial", 22);
			Assert.AreEqual(2, builder.Series.Count);
			foreach (var series in builder.Series)
				Assert.IsTrue(series.Images.Count > 1);

			builder.AddStudy(studyTree);

			var options = new DisplaySetCreationOptions();
			var o = (StoredDisplaySetCreationSetting) options["CT"];
			Assert.IsTrue(o.ShowOriginalSeries);

			Test(studyTree, builder, options, false, expectedDisplaySetCount);
		}

		public void TestMR(bool split, bool showOriginal, bool showOriginalSeries, int expectedDisplaySetCount)
		{
			//NOTE: we still accept the "split" parameter even though we know it won't work because that is still a valid use case.
			TestMR(false, split, showOriginal, showOriginalSeries, expectedDisplaySetCount);
		}

		public void TestMREcho(bool split, bool showOriginal, bool showOriginalSeries, int expectedDisplaySetCount)
		{
			TestMR(true, split, showOriginal, showOriginalSeries, expectedDisplaySetCount);
		}

		public void TestMR(bool createEchos, bool split, bool showOriginal, bool showOriginalSeries, int expectedDisplaySetCount)
		{
			Trace.WriteLine(String.Format("MR Echo: split={0}, showOriginal={1}, showOriginalSeries={2}", split, showOriginal, showOriginalSeries));

			var studyTree = new StudyTree();
			var builder = StudyBuilderFactory.CreateMRBuilder("Patient1", "Test^Patient", "MR Echo", null, "Series1", "Series2", 1, createEchos, false);
			builder.AddStudy(studyTree);

			var options = new DisplaySetCreationOptions();
			var o = (StoredDisplaySetCreationSetting) options["MR"];
			Assert.IsTrue(o.SplitMultiEchoSeries);
			Assert.IsFalse(o.ShowOriginalMultiEchoSeries);
			Assert.IsTrue(o.ShowOriginalSeries);

			o.SplitMultiEchoSeries = split;
			o.ShowOriginalMultiEchoSeries = showOriginal;
			o.ShowOriginalSeries = showOriginalSeries;

			Test(studyTree, builder, options, false, expectedDisplaySetCount);
		}

		public void TestMixedMultiframe(bool split, bool showOriginal, bool showOriginalSeries, int numberSingleImages, int numberMultiframes, int expectedDisplaySetCount)
		{
			Trace.WriteLine(String.Format("Mixed Multiframe: split={0}, showOriginal={1}, showOriginalSeries={2}, "
			                              + "singleImages={3}, multiFrames={4}"
			                              , split, showOriginal, showOriginalSeries, numberSingleImages, numberMultiframes));

			var studyTree = new StudyTree();
			var builder = StudyBuilderFactory.CreateMixedMultiframeUltrasound("Patient1", "Test^Patient", "Mixed Multiframe", null,
			                                                                  3, "Series3", numberSingleImages, numberMultiframes, false, false);
			builder.AddStudy(studyTree);

			var options = new DisplaySetCreationOptions();
			var o = (StoredDisplaySetCreationSetting) options["US"];
			Assert.IsTrue(o.SplitMixedMultiframes);
			Assert.IsFalse(o.ShowOriginalMixedMultiframeSeries);
			Assert.IsTrue(o.ShowOriginalSeries);

			o.SplitMixedMultiframes = split;
			o.ShowOriginalMixedMultiframeSeries = showOriginal;
			o.ShowOriginalSeries = showOriginalSeries;

			Test(studyTree, builder, options, false, expectedDisplaySetCount);
		}

		private void Test(StudyTree studyTree, StudyBuilder builder, DisplaySetCreationOptions options, bool expectModalityDisplaySet, int expectedDisplaySetCount)
		{
			var filler = new LayoutManager.ImageSetFiller(studyTree, options);
			var imageSet = new ImageSet {Uid = builder.StudyInstanceUid};

			var study = studyTree.Studies.First();

			try
			{
				//NOTE: follow the same pattern as the layout manager where we add the "all images" display sets
				//first, then we add ones for the individual series after.
				filler.AddMultiSeriesDisplaySets(imageSet, study);
				Assert.AreEqual(expectModalityDisplaySet ? 1 : 0, imageSet.DisplaySets.Count, "Empty Image Set");

				foreach (var series in study.Series)
					filler.AddSeriesDisplaySets(imageSet, series);

				Assert.AreEqual(expectedDisplaySetCount, imageSet.DisplaySets.Count, "Display Set Counts");
			}
			finally
			{
				studyTree.Dispose();
				imageSet.Dispose();
			}
		}
	}
}

#endif