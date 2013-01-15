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
using System.Drawing;
using System;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class InvariantArcPrimitiveTest
	{
		public InvariantArcPrimitiveTest()
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
		public void CoordinateChange()
		{
			InvariantArcPrimitive arc = new InvariantArcPrimitive();

			for (int angle = 0; angle <= 360; angle += 90)
			{
				arc.SpatialTransform.RotationXY = angle;
				arc.SpatialTransform.FlipX = true;
				arc.SpatialTransform.FlipY = true;
				arc.CoordinateSystem = CoordinateSystem.Source;
				arc.StartAngle = 30;
				arc.CoordinateSystem = CoordinateSystem.Destination;
				float dstAngle = arc.StartAngle;
				arc.StartAngle = dstAngle;
				arc.CoordinateSystem = CoordinateSystem.Source;
				Assert.AreEqual(30, arc.StartAngle);
			}
		}
	}
}

#endif