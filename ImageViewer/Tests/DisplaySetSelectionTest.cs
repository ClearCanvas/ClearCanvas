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

using NUnit.Framework;
using TestPresentationImage = ClearCanvas.ImageViewer.Tests.MockPresentationImage;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class DisplaySetSelectionTest
	{
		public DisplaySetSelectionTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{

		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void SelectDisplaySet()
		{
			IDisplaySet displaySet1 = new DisplaySet();
			displaySet1.PresentationImages.Add(new TestPresentationImage());
			displaySet1.PresentationImages.Add(new TestPresentationImage());
			displaySet1.PresentationImages.Add(new TestPresentationImage());
			displaySet1.PresentationImages.Add(new TestPresentationImage());

			IDisplaySet displaySet2 = new DisplaySet();
			displaySet2.PresentationImages.Add(new TestPresentationImage());
			displaySet2.PresentationImages.Add(new TestPresentationImage());

			IImageViewer viewer = new ImageViewerComponent();
			IImageBox imageBox = new ImageBox();
			viewer.PhysicalWorkspace.ImageBoxes.Add(imageBox);

			imageBox.SetTileGrid(2, 2);
			imageBox.DisplaySet = displaySet1;
			imageBox[0, 0].Select();

			Assert.IsTrue(imageBox[0, 0].Selected);
			Assert.IsFalse(imageBox[0, 1].Selected);

			imageBox[0, 1].Select();
			Assert.IsFalse(imageBox[0, 0].Selected);
			Assert.IsTrue(imageBox[0, 1].Selected);

			imageBox.DisplaySet = displaySet2;
			Assert.IsFalse(imageBox[0, 0].Selected);
			Assert.IsTrue(imageBox[0, 1].Selected);
		}

		[Test]
		public void ReplaceDisplaySet()
		{
			IDisplaySet displaySet1 = new DisplaySet();
			IPresentationImage image1 = new TestPresentationImage();
			displaySet1.PresentationImages.Add(image1);

			IDisplaySet displaySet2 = new DisplaySet();
			IPresentationImage image2 = new TestPresentationImage();
			displaySet2.PresentationImages.Add(image2);

			ImageViewerComponent viewer = new ImageViewerComponent();

			IImageBox imageBox1 = new ImageBox();
			viewer.PhysicalWorkspace.ImageBoxes.Add(imageBox1);

			imageBox1.SetTileGrid(2, 2);
			imageBox1.DisplaySet = displaySet1;
			imageBox1[0,0].Select();

			Assert.IsTrue(displaySet1.Selected);
			Assert.IsTrue(image1.Selected);

			imageBox1.DisplaySet = displaySet2;

			Assert.IsFalse(displaySet1.Selected);
			Assert.IsFalse(image1.Selected);

			Assert.IsTrue(displaySet2.Selected);
			Assert.IsTrue(image2.Selected);

		}
	}
}

#endif