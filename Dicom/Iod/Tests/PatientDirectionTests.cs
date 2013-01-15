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

using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Tests
{
	[TestFixture]
	public class PatientDirectionTests
	{
		[Test]
		public void TestConstructors()
		{
			var empty = new PatientDirection("");
			Assert.AreEqual("", empty.Code, "Direction: empty");
			Assert.AreEqual(true, empty.IsEmpty, "Direction: empty");
			Assert.AreEqual(false, empty.IsUnspecified, "Direction: empty");

			var unspec = new PatientDirection("X");
			Assert.AreEqual("X", unspec.Code, "Direction: unspec");
			Assert.AreEqual(false, unspec.IsEmpty, "Direction: unspec");
			Assert.AreEqual(true, unspec.IsUnspecified, "Direction: unspec");

			var front = new PatientDirection("A");
			Assert.AreEqual("A", front.Code, "Direction: front");
			Assert.AreEqual(false, front.IsEmpty, "Direction: front");
			Assert.AreEqual(false, front.IsUnspecified, "Direction: front");

			var rightfoot = new PatientDirection("RF");
			Assert.AreEqual("RF", rightfoot.Code, "Direction: rightfoot");
			Assert.AreEqual(false, rightfoot.IsEmpty, "Direction: rightfoot");
			Assert.AreEqual(false, rightfoot.IsUnspecified, "Direction: rightfoot");

			var headleftback = new PatientDirection("HLP");
			Assert.AreEqual("HLP", headleftback.Code, "Direction: headleftback");
			Assert.AreEqual(false, headleftback.IsEmpty, "Direction: headleftback");
			Assert.AreEqual(false, headleftback.IsUnspecified, "Direction: headleftback");
		}

		[Test]
		public void TestComponents()
		{
			var empty = new PatientDirection("");
			Assert.AreEqual(0, empty.ComponentCount, "Direction: empty");
			Assert.AreEqual("", empty.Primary.Code, "Direction: empty");
			Assert.AreEqual("", empty.Secondary.Code, "Direction: empty");
			Assert.AreEqual("", empty.Tertiary.Code, "Direction: empty");
			Assert.AreEqual("", empty[PatientDirection.Component.Primary].Code, "Direction: empty");
			Assert.AreEqual("", empty[PatientDirection.Component.Secondary].Code, "Direction: empty");
			Assert.AreEqual("", empty[PatientDirection.Component.Tertiary].Code, "Direction: empty");

			var unspec = new PatientDirection("X");
			Assert.AreEqual(1, unspec.ComponentCount, "Direction: unspec");
			Assert.AreEqual("X", unspec.Primary.Code, "Direction: unspec");
			Assert.AreEqual("", unspec.Secondary.Code, "Direction: unspec");
			Assert.AreEqual("", unspec.Tertiary.Code, "Direction: unspec");
			Assert.AreEqual("X", unspec[PatientDirection.Component.Primary].Code, "Direction: unspec");
			Assert.AreEqual("", unspec[PatientDirection.Component.Secondary].Code, "Direction: unspec");
			Assert.AreEqual("", unspec[PatientDirection.Component.Tertiary].Code, "Direction: unspec");

			var front = new PatientDirection("A");
			Assert.AreEqual(1, front.ComponentCount, "Direction: front");
			Assert.AreEqual("A", front.Primary.Code, "Direction: front");
			Assert.AreEqual("", front.Secondary.Code, "Direction: front");
			Assert.AreEqual("", front.Tertiary.Code, "Direction: front");
			Assert.AreEqual("A", front[PatientDirection.Component.Primary].Code, "Direction: front");
			Assert.AreEqual("", front[PatientDirection.Component.Secondary].Code, "Direction: front");
			Assert.AreEqual("", front[PatientDirection.Component.Tertiary].Code, "Direction: front");

			var rightfoot = new PatientDirection("RF");
			Assert.AreEqual(2, rightfoot.ComponentCount, "Direction: rightfoot");
			Assert.AreEqual("R", rightfoot.Primary.Code, "Direction: rightfoot");
			Assert.AreEqual("F", rightfoot.Secondary.Code, "Direction: rightfoot");
			Assert.AreEqual("", rightfoot.Tertiary.Code, "Direction: rightfoot");
			Assert.AreEqual("R", rightfoot[PatientDirection.Component.Primary].Code, "Direction: rightfoot");
			Assert.AreEqual("F", rightfoot[PatientDirection.Component.Secondary].Code, "Direction: rightfoot");
			Assert.AreEqual("", rightfoot[PatientDirection.Component.Tertiary].Code, "Direction: rightfoot");

			var headleftback = new PatientDirection("HLP");
			Assert.AreEqual(3, headleftback.ComponentCount, "Direction: headleftback");
			Assert.AreEqual("H", headleftback.Primary.Code, "Direction: headleftback");
			Assert.AreEqual("L", headleftback.Secondary.Code, "Direction: headleftback");
			Assert.AreEqual("P", headleftback.Tertiary.Code, "Direction: headleftback");
			Assert.AreEqual("H", headleftback[PatientDirection.Component.Primary].Code, "Direction: headleftback");
			Assert.AreEqual("L", headleftback[PatientDirection.Component.Secondary].Code, "Direction: headleftback");
			Assert.AreEqual("P", headleftback[PatientDirection.Component.Tertiary].Code, "Direction: headleftback");
		}

		[Test]
		public void TestIsValid()
		{
			Assert.AreEqual(false, new PatientDirection("").IsValid, "Direction: empty");
			Assert.AreEqual(true, new PatientDirection("X").IsValid, "Direction: unspec");
			Assert.AreEqual(true, new PatientDirection("A").IsValid, "Direction: front");
			Assert.AreEqual(true, new PatientDirection("RF").IsValid, "Direction: rightfoot");
			Assert.AreEqual(true, new PatientDirection("HLP").IsValid, "Direction: headleftback");

			Assert.AreEqual(false, new PatientDirection("LX").IsValid, "Direction: illegal unspec component");
			Assert.AreEqual(false, new PatientDirection("XR").IsValid, "Direction: illegal unspec component");
			Assert.AreEqual(false, new PatientDirection("HLPA").IsValid, "Direction: too many components");
			Assert.AreEqual(false, new PatientDirection("D").IsValid, "Direction: unknown component");
			Assert.AreEqual(false, new PatientDirection("HD").IsValid, "Direction: unknown component");
			Assert.AreEqual(false, new PatientDirection("HF").IsValid, "Direction: components in same direction");
			Assert.AreEqual(false, new PatientDirection("HPF").IsValid, "Direction: components in same direction");
		}

		[Test]
		public void TestEquality()
		{
			Assert.AreEqual(PatientDirection.Empty, new PatientDirection(""), "Direction: empty");
			Assert.AreEqual(PatientDirection.Unspecified, new PatientDirection("X"), "Direction: unspec");
			Assert.AreEqual(PatientDirection.Anterior, new PatientDirection("A"), "Direction: front");
			Assert.AreEqual(new PatientDirection("RF"), new PatientDirection("RF"), "Direction: rightfoot");
			Assert.AreEqual(new PatientDirection("HLP"), new PatientDirection("HLP"), "Direction: headleftback");

			Assert.AreNotEqual(PatientDirection.Empty, new PatientDirection("L"), "inequality");
			Assert.AreNotEqual(PatientDirection.Left, new PatientDirection("LF"), "left vs leftfoot");
			Assert.AreNotEqual(PatientDirection.Left, new PatientDirection("FL"), "left vs footleft");
			Assert.AreNotEqual(new PatientDirection("LF"), new PatientDirection("FL"), "leftfoot vs footleft");
		}

		[Test]
		public void TestOpposingDirection()
		{
			var empty = new PatientDirection("");
			Assert.AreEqual("", empty.OpposingDirection.Code, "Direction: empty");

			var unspec = new PatientDirection("X");
			Assert.AreEqual("X", unspec.OpposingDirection.Code, "Direction: unspec");

			var front = new PatientDirection("A");
			Assert.AreEqual("P", front.OpposingDirection.Code, "Direction: front");

			var rightfoot = new PatientDirection("RF");
			Assert.AreEqual("LH", rightfoot.OpposingDirection.Code, "Direction: rightfoot");

			var headleftback = new PatientDirection("HLP");
			Assert.AreEqual("FRA", headleftback.OpposingDirection.Code, "Direction: headleftback");
		}

		[Test]
		public void TestConcatenator()
		{
			var empty = new PatientDirection("");
			Assert.AreEqual("", (empty + new PatientDirection("")).Code, "Direction: empty+empty");
			Assert.AreEqual("X", (empty + new PatientDirection("X")).Code, "Direction: empty+unspec");
			Assert.AreEqual("X", (new PatientDirection("X") + empty).Code, "Direction: unspec+empty");
			Assert.AreEqual(false, (new PatientDirection("X") + new PatientDirection("A")).IsValid, "Direction: illegal concatenation");
			Assert.AreEqual(false, (new PatientDirection("A") + new PatientDirection("X")).IsValid, "Direction: illegal concatenation");
			Assert.AreEqual("RF", (new PatientDirection("R") + new PatientDirection("F")).Code, "Direction: rightfoot");
			Assert.AreEqual("HLP", (new PatientDirection("H") + new PatientDirection("L") + new PatientDirection("P")).Code, "Direction: headleftback");
		}
	}
}

#endif