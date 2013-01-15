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

using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Tests
{
	class TestPresentationImage : PresentationImage
	{
		public override IRenderer ImageRenderer
		{
			get { return null; }
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new TestPresentationImage();
		}
	}

	class TestTree
	{
		IImageViewer _viewer;
		IImageBox _imageBox1;
		IImageBox _imageBox2;
		ITile _tile1;
		ITile _tile2;
		ITile _tile3;
		ITile _tile4;
		IImageSet _imageSet1;
		IDisplaySet _displaySet1;
		IDisplaySet _displaySet2;
		IPresentationImage _image1;
		IPresentationImage _image2;
		IPresentationImage _image3;
		IPresentationImage _image4;

		public TestTree()
		{
			_viewer = new ImageViewerComponent();
			
			_imageBox1 = new ImageBox();
			_imageBox2 = new ImageBox();
			
			_tile1 = new Tile();
			_tile2 = new Tile();
			_tile3 = new Tile();
			_tile4 = new Tile();

			_imageSet1 = new ImageSet();

			_displaySet1 = new DisplaySet();
			_displaySet2 = new DisplaySet();

			_image1 = new TestPresentationImage();
			_image2 = new TestPresentationImage();
			_image3 = new TestPresentationImage();
			_image4 = new TestPresentationImage();

			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox1);
			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox2);

			_imageBox1.Tiles.Add(_tile1);
			_imageBox1.Tiles.Add(_tile2);
			_imageBox2.Tiles.Add(_tile3);
			_imageBox2.Tiles.Add(_tile4);

			_viewer.LogicalWorkspace.ImageSets.Add(_imageSet1);

			_imageSet1.DisplaySets.Add(_displaySet1);
			_imageSet1.DisplaySets.Add(_displaySet2);
			
			_displaySet1.PresentationImages.Add(_image1);
			_displaySet1.PresentationImages.Add(_image2);
			_displaySet2.PresentationImages.Add(_image3);
			_displaySet2.PresentationImages.Add(_image4);
			
			_imageBox1.DisplaySet = _displaySet1;
			_imageBox2.DisplaySet = _displaySet2;
		}

		public IImageViewer Viewer { get { return _viewer; } }
		public IImageBox ImageBox1 { get { return _imageBox1; } }
		public IImageBox ImageBox2 { get { return _imageBox2; } }
		public ITile Tile1 { get { return _tile1; } }
		public ITile Tile2 { get { return _tile2; } }
		public ITile Tile3 { get { return _tile3; } }
		public ITile Tile4 { get { return _tile4; } }
		public IImageSet ImageSet1 { get { return _imageSet1; } }
		public IDisplaySet DisplaySet1 { get { return _displaySet1; } }
		public IDisplaySet DisplaySet2 { get { return _displaySet2; } }
		public IPresentationImage Image1 { get { return _image1; } }
		public IPresentationImage Image2 { get { return _image2; } }
		public IPresentationImage Image3 { get { return _image3; } }
		public IPresentationImage Image4 { get { return _image4; } }

	}
}

#endif