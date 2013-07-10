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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using TestPresentationImage = ClearCanvas.ImageViewer.Tests.MockPresentationImage;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class BuildTreeTest
	{
		IImageViewer _viewer;
		IImageBox _imageBox;
		ITile _tile1;
		ITile _tile2;
		IImageSet _imageSet;
		IDisplaySet _displaySet;
		IPresentationImage _image1;
		IPresentationImage _image2;
	
		private void CreateObjects()
		{
			_viewer = new ImageViewerComponent();
			_imageBox = new ImageBox();
			_tile1 = new Tile();
			_tile2 = new Tile();

			_imageSet = new ImageSet();
			_displaySet = new DisplaySet();
			_image1 = new TestPresentationImage();
			_image2 = new TestPresentationImage();
		}

		public BuildTreeTest()
		{
		}

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
		public void BuildDownward()
		{
			CreateObjects();
			Assert.AreEqual(_viewer, _viewer.PhysicalWorkspace.ImageViewer);

			// Add image box to physical workspace
			Assert.IsNull(_imageBox.ImageViewer);
			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox);
			Assert.AreEqual(_viewer, _imageBox.ImageViewer);
			Assert.AreEqual(_viewer.PhysicalWorkspace, _imageBox.ParentPhysicalWorkspace);

			// Add tiles to image box
			Assert.IsNull(_tile1.ImageViewer);
			_imageBox.Tiles.Add(_tile1);
			Assert.AreEqual(_viewer, _tile1.ImageViewer);
			Assert.AreEqual(_imageBox, _tile1.ParentImageBox);

			Assert.IsNull(_tile2.ImageViewer);
			_imageBox.Tiles.Add(_tile2);
			Assert.AreEqual(_viewer, _tile2.ImageViewer);
			Assert.AreEqual(_imageBox, _tile2.ParentImageBox);

			// Add image set to logical workspace
			Assert.IsNull(_imageSet.ImageViewer);
			_viewer.LogicalWorkspace.ImageSets.Add(_imageSet);
			Assert.AreEqual(_viewer, _imageSet.ImageViewer);
			Assert.AreEqual(_viewer.LogicalWorkspace, _imageSet.ParentLogicalWorkspace);

			// Add display set to image set
			Assert.IsNull(_displaySet.ImageViewer);
			_imageSet.DisplaySets.Add(_displaySet);
			Assert.AreEqual(_viewer, _displaySet.ImageViewer);
			Assert.AreEqual(_imageSet, _displaySet.ParentImageSet);

			// Add presentation images to display set;
			Assert.IsNull(_image1.ImageViewer);
			_displaySet.PresentationImages.Add(_image1);
			Assert.AreEqual(_viewer, _image1.ImageViewer);
			Assert.AreEqual(_displaySet, _image1.ParentDisplaySet);

			Assert.IsNull(_image2.ImageViewer);
			_displaySet.PresentationImages.Add(_image2);
			Assert.AreEqual(_viewer, _image2.ImageViewer);
			Assert.AreEqual(_displaySet, _image2.ParentDisplaySet);

			// Associate display set with image box
			_imageBox.DisplaySet = _displaySet;
			Assert.AreEqual(_image1, _tile1.PresentationImage);
			Assert.AreEqual(_image2, _tile2.PresentationImage);
		}

		[Test]
		public void BuildUpward()
		{
			CreateObjects();

			// Add tiles to image box
			_imageBox.Tiles.Add(_tile1);
			Assert.IsNull(_tile1.ImageViewer);
			Assert.AreEqual(_imageBox, _tile1.ParentImageBox);

			_imageBox.Tiles.Add(_tile2);
			Assert.IsNull(_tile2.ImageViewer);
			Assert.AreEqual(_imageBox, _tile2.ParentImageBox);

			// Add presentation images to display set
			_displaySet.PresentationImages.Add(_image1);
			Assert.IsNull(_image1.ImageViewer);
			Assert.AreEqual(_displaySet, _image1.ParentDisplaySet);

			_displaySet.PresentationImages.Add(_image2);
			Assert.IsNull(_image2.ImageViewer);
			Assert.AreEqual(_displaySet, _image2.ParentDisplaySet);

			// Associate image box with display set
			_imageBox.DisplaySet = _displaySet;
			Assert.AreEqual(_image1, _tile1.PresentationImage);
			Assert.AreEqual(_image2, _tile2.PresentationImage);

			// Add image box the physical workspace
			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox);

			Assert.AreEqual(_viewer.PhysicalWorkspace, _imageBox.ParentPhysicalWorkspace);

			Assert.AreEqual(_viewer, _imageBox.ImageViewer);
			Assert.AreEqual(_viewer, _tile1.ImageViewer);
			Assert.AreEqual(_viewer, _tile2.ImageViewer);
			Assert.AreEqual(_viewer, _displaySet.ImageViewer);
			Assert.AreEqual(_viewer, _image1.ImageViewer);
			Assert.AreEqual(_viewer, _image2.ImageViewer);
		}

	}
}

#endif