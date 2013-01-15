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

namespace ClearCanvas.ImageViewer.Imaging.Tests
{

	[TestFixture]
	public class MiscellaneousLutTests
	{
		public MiscellaneousLutTests()
		{
		}

		[Test]
		public void TestSimpleLut()
		{
			int bitsStored = 8;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			SimpleDataModalityLut simpleLut = 
				new SimpleDataModalityLut(modalityLUT.MinInputValue, modalityLUT.Data, modalityLUT.MinOutputValue, modalityLUT.MaxOutputValue, modalityLUT.GetKey(), modalityLUT.GetDescription()); 

			Assert.AreEqual(modalityLUT.MinInputValue, simpleLut.MinInputValue);
			Assert.AreEqual(modalityLUT.MaxInputValue, simpleLut.MaxInputValue);
			Assert.AreEqual(modalityLUT.MinOutputValue, simpleLut.MinOutputValue);
			Assert.AreEqual(modalityLUT.MaxOutputValue, simpleLut.MaxOutputValue);
			Assert.AreEqual(modalityLUT.GetKey(), simpleLut.GetKey());
			Assert.AreEqual(modalityLUT.GetDescription(), simpleLut.GetDescription());
			Assert.AreEqual(modalityLUT.Length, simpleLut.Length);
		}
	}
}

#endif