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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class SelectionTest
	{
		public SelectionTest()
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
		public void SelectTilesInSameImageBox()
		{
			TestTree testTree = new TestTree();

			testTree.Tile1.Select();
			Assert.AreEqual(testTree.Tile1, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image1.Selected);
			Assert.IsTrue(testTree.Tile1.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);

			testTree.Tile2.Select();

			Assert.AreEqual(testTree.Tile2, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image2.Selected);
			Assert.IsTrue(testTree.Tile2.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image1.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile1.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);
		}

		[Test]
		public void SelectTilesInDiffferentImageBoxes()
		{
			TestTree testTree = new TestTree();

			testTree.Tile1.Select();
			Assert.AreEqual(testTree.Tile1, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image1.Selected);
			Assert.IsTrue(testTree.Tile1.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);

			testTree.Tile3.Select();

			Assert.AreEqual(testTree.Tile3, testTree.ImageBox2.SelectedTile);
			Assert.AreEqual(testTree.ImageBox2, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image3.Selected);
			Assert.IsTrue(testTree.Tile3.Selected);
			Assert.IsTrue(testTree.DisplaySet2.Selected);
			Assert.IsTrue(testTree.ImageBox2.Selected);

			Assert.IsFalse(testTree.Image1.Selected);
			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile1.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet1.Selected);
			Assert.IsFalse(testTree.ImageBox1.Selected);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SelectTileBeforeAddingToTree()
		{
			ITile tile = new Tile();
			tile.Select();
		}
	}
}

#endif