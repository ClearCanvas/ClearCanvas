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
	public class QuadrupedPatientDirectionTests
	{
		[Test]
		public void TestConstructors()
		{
			var empty = new PatientDirection("", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("", empty.Code, "Direction: empty");
			Assert.AreEqual(true, empty.IsEmpty, "Direction: empty");
			Assert.AreEqual(false, empty.IsUnspecified, "Direction: empty");

			var unspec = new PatientDirection("X", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("X", unspec.Code, "Direction: unspec");
			Assert.AreEqual(false, unspec.IsEmpty, "Direction: unspec");
			Assert.AreEqual(true, unspec.IsUnspecified, "Direction: unspec");

			var lateral = new PatientDirection("L", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("L", lateral.Code, "Direction: lateral");
			Assert.AreEqual(false, lateral.IsEmpty, "Direction: lateral");
			Assert.AreEqual(false, lateral.IsUnspecified, "Direction: lateral");

			var plantar = new PatientDirection("PL", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("PL", plantar.Code, "Direction: plantar");
			Assert.AreEqual(false, plantar.IsEmpty, "Direction: plantar");
			Assert.AreEqual(false, plantar.IsUnspecified, "Direction: plantar");

			var parlmarrostral = new PatientDirection("PAR", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("PAR", parlmarrostral.Code, "Direction: parlmarrostral");
			Assert.AreEqual(false, parlmarrostral.IsEmpty, "Direction: parlmarrostral");
			Assert.AreEqual(false, parlmarrostral.IsUnspecified, "Direction: parlmarrostral");

			var dorsaldistal = new PatientDirection("DDI", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("DDI", dorsaldistal.Code, "Direction: dorsaldistal");
			Assert.AreEqual(false, dorsaldistal.IsEmpty, "Direction: dorsaldistal");
			Assert.AreEqual(false, dorsaldistal.IsUnspecified, "Direction: dorsaldistal");

			var plantarright = new PatientDirection("PLRT", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("PLRT", plantarright.Code, "Direction: plantarright");
			Assert.AreEqual(false, plantarright.IsEmpty, "Direction: plantarright");
			Assert.AreEqual(false, plantarright.IsUnspecified, "Direction: plantarright");

			var medialventralcaudal = new PatientDirection("MVCD", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("MVCD", medialventralcaudal.Code, "Direction: medialventralcaudal");
			Assert.AreEqual(false, medialventralcaudal.IsEmpty, "Direction: medialventralcaudal");
			Assert.AreEqual(false, medialventralcaudal.IsUnspecified, "Direction: medialventralcaudal");

			var proximalleftcranial = new PatientDirection("PRLECR", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("PRLECR", proximalleftcranial.Code, "Direction: proximalleftcranial");
			Assert.AreEqual(false, proximalleftcranial.IsEmpty, "Direction: proximalleftcranial");
			Assert.AreEqual(false, proximalleftcranial.IsUnspecified, "Direction: proximalleftcranial");
		}

		[Test]
		public void TestComponents()
		{
			var empty = new PatientDirection("", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(0, empty.ComponentCount, "Direction: empty");
			Assert.AreEqual("", empty.Primary.Code, "Direction: empty");
			Assert.AreEqual("", empty.Secondary.Code, "Direction: empty");
			Assert.AreEqual("", empty.Tertiary.Code, "Direction: empty");
			Assert.AreEqual("", empty[PatientDirection.Component.Primary].Code, "Direction: empty");
			Assert.AreEqual("", empty[PatientDirection.Component.Secondary].Code, "Direction: empty");
			Assert.AreEqual("", empty[PatientDirection.Component.Tertiary].Code, "Direction: empty");

			var unspec = new PatientDirection("X", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(1, unspec.ComponentCount, "Direction: unspec");
			Assert.AreEqual("X", unspec.Primary.Code, "Direction: unspec");
			Assert.AreEqual("", unspec.Secondary.Code, "Direction: unspec");
			Assert.AreEqual("", unspec.Tertiary.Code, "Direction: unspec");
			Assert.AreEqual("X", unspec[PatientDirection.Component.Primary].Code, "Direction: unspec");
			Assert.AreEqual("", unspec[PatientDirection.Component.Secondary].Code, "Direction: unspec");
			Assert.AreEqual("", unspec[PatientDirection.Component.Tertiary].Code, "Direction: unspec");

			var lateral = new PatientDirection("L", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(1, lateral.ComponentCount, "Direction: lateral");
			Assert.AreEqual("L", lateral.Primary.Code, "Direction: lateral");
			Assert.AreEqual("", lateral.Secondary.Code, "Direction: lateral");
			Assert.AreEqual("", lateral.Tertiary.Code, "Direction: lateral");
			Assert.AreEqual("L", lateral[PatientDirection.Component.Primary].Code, "Direction: lateral");
			Assert.AreEqual("", lateral[PatientDirection.Component.Secondary].Code, "Direction: lateral");
			Assert.AreEqual("", lateral[PatientDirection.Component.Tertiary].Code, "Direction: lateral");

			var plantar = new PatientDirection("PL", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(1, plantar.ComponentCount, "Direction: plantar");
			Assert.AreEqual("PL", plantar.Primary.Code, "Direction: plantar");
			Assert.AreEqual("", plantar.Secondary.Code, "Direction: plantar");
			Assert.AreEqual("", plantar.Tertiary.Code, "Direction: plantar");
			Assert.AreEqual("PL", plantar[PatientDirection.Component.Primary].Code, "Direction: plantar");
			Assert.AreEqual("", plantar[PatientDirection.Component.Secondary].Code, "Direction: plantar");
			Assert.AreEqual("", plantar[PatientDirection.Component.Tertiary].Code, "Direction: plantar");

			var palmarrostral = new PatientDirection("PAR", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(2, palmarrostral.ComponentCount, "Direction: palmarrostral");
			Assert.AreEqual("PA", palmarrostral.Primary.Code, "Direction: palmarrostral");
			Assert.AreEqual("R", palmarrostral.Secondary.Code, "Direction: palmarrostral");
			Assert.AreEqual("", palmarrostral.Tertiary.Code, "Direction: palmarrostral");
			Assert.AreEqual("PA", palmarrostral[PatientDirection.Component.Primary].Code, "Direction: palmarrostral");
			Assert.AreEqual("R", palmarrostral[PatientDirection.Component.Secondary].Code, "Direction: palmarrostral");
			Assert.AreEqual("", palmarrostral[PatientDirection.Component.Tertiary].Code, "Direction: palmarrostral");

			var dorsaldistal = new PatientDirection("DDI", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(2, dorsaldistal.ComponentCount, "Direction: dorsaldistal");
			Assert.AreEqual("D", dorsaldistal.Primary.Code, "Direction: dorsaldistal");
			Assert.AreEqual("DI", dorsaldistal.Secondary.Code, "Direction: dorsaldistal");
			Assert.AreEqual("", dorsaldistal.Tertiary.Code, "Direction: dorsaldistal");
			Assert.AreEqual("D", dorsaldistal[PatientDirection.Component.Primary].Code, "Direction: dorsaldistal");
			Assert.AreEqual("DI", dorsaldistal[PatientDirection.Component.Secondary].Code, "Direction: dorsaldistal");
			Assert.AreEqual("", dorsaldistal[PatientDirection.Component.Tertiary].Code, "Direction: dorsaldistal");

			var plantarright = new PatientDirection("PLRT", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(2, plantarright.ComponentCount, "Direction: plantarright");
			Assert.AreEqual("PL", plantarright.Primary.Code, "Direction: plantarright");
			Assert.AreEqual("RT", plantarright.Secondary.Code, "Direction: plantarright");
			Assert.AreEqual("", plantarright.Tertiary.Code, "Direction: plantarright");
			Assert.AreEqual("PL", plantarright[PatientDirection.Component.Primary].Code, "Direction: plantarright");
			Assert.AreEqual("RT", plantarright[PatientDirection.Component.Secondary].Code, "Direction: plantarright");
			Assert.AreEqual("", plantarright[PatientDirection.Component.Tertiary].Code, "Direction: plantarright");

			var medialventralcaudal = new PatientDirection("MVCD", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(3, medialventralcaudal.ComponentCount, "Direction: medialventralcaudal");
			Assert.AreEqual("M", medialventralcaudal.Primary.Code, "Direction: medialventralcaudal");
			Assert.AreEqual("V", medialventralcaudal.Secondary.Code, "Direction: medialventralcaudal");
			Assert.AreEqual("CD", medialventralcaudal.Tertiary.Code, "Direction: medialventralcaudal");
			Assert.AreEqual("M", medialventralcaudal[PatientDirection.Component.Primary].Code, "Direction: medialventralcaudal");
			Assert.AreEqual("V", medialventralcaudal[PatientDirection.Component.Secondary].Code, "Direction: medialventralcaudal");
			Assert.AreEqual("CD", medialventralcaudal[PatientDirection.Component.Tertiary].Code, "Direction: medialventralcaudal");

			var proximalleftcranial = new PatientDirection("PRLECR", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual(3, proximalleftcranial.ComponentCount, "Direction: proximalleftcranial");
			Assert.AreEqual("PR", proximalleftcranial.Primary.Code, "Direction: proximalleftcranial");
			Assert.AreEqual("LE", proximalleftcranial.Secondary.Code, "Direction: proximalleftcranial");
			Assert.AreEqual("CR", proximalleftcranial.Tertiary.Code, "Direction: proximalleftcranial");
			Assert.AreEqual("PR", proximalleftcranial[PatientDirection.Component.Primary].Code, "Direction: proximalleftcranial");
			Assert.AreEqual("LE", proximalleftcranial[PatientDirection.Component.Secondary].Code, "Direction: proximalleftcranial");
			Assert.AreEqual("CR", proximalleftcranial[PatientDirection.Component.Tertiary].Code, "Direction: proximalleftcranial");
		}

		[Test]
		public void TestIsValid()
		{
			Assert.AreEqual(false, new PatientDirection("", AnatomicalOrientationType.Quadruped).IsValid, "Direction: empty");
			Assert.AreEqual(true, new PatientDirection("X", AnatomicalOrientationType.Quadruped).IsValid, "Direction: unspec");
			Assert.AreEqual(true, new PatientDirection("L", AnatomicalOrientationType.Quadruped).IsValid, "Direction: lateral");
			Assert.AreEqual(true, new PatientDirection("PL", AnatomicalOrientationType.Quadruped).IsValid, "Direction: plantar");
			Assert.AreEqual(true, new PatientDirection("PAR", AnatomicalOrientationType.Quadruped).IsValid, "Direction: palmarrostral");
			Assert.AreEqual(true, new PatientDirection("DDI", AnatomicalOrientationType.Quadruped).IsValid, "Direction: dorsaldistal");
			Assert.AreEqual(true, new PatientDirection("PLRT", AnatomicalOrientationType.Quadruped).IsValid, "Direction: plantarright");
			Assert.AreEqual(true, new PatientDirection("MVCD", AnatomicalOrientationType.Quadruped).IsValid, "Direction: medialventralcaudal");
			Assert.AreEqual(true, new PatientDirection("PRLECR", AnatomicalOrientationType.Quadruped).IsValid, "Direction: proximalleftcranial");

			Assert.AreEqual(false, new PatientDirection("LX", AnatomicalOrientationType.Quadruped).IsValid, "Direction: illegal unspec component");
			Assert.AreEqual(false, new PatientDirection("XR", AnatomicalOrientationType.Quadruped).IsValid, "Direction: illegal unspec component");
			Assert.AreEqual(false, new PatientDirection("LRCDPR", AnatomicalOrientationType.Quadruped).IsValid, "Direction: too many components");
			Assert.AreEqual(false, new PatientDirection("ID", AnatomicalOrientationType.Quadruped).IsValid, "Direction: unknown component");
			Assert.AreEqual(false, new PatientDirection("PRPD", AnatomicalOrientationType.Quadruped).IsValid, "Direction: unknown component");
		}

		[Test]
		public void TestEquality()
		{
			Assert.AreEqual(PatientDirection.Empty, new PatientDirection("", AnatomicalOrientationType.Quadruped), "Direction: empty");
			Assert.AreEqual(PatientDirection.Unspecified, new PatientDirection("X", AnatomicalOrientationType.Quadruped), "Direction: unspec");
			Assert.AreEqual(PatientDirection.QuadrupedLateral, new PatientDirection("L", AnatomicalOrientationType.Quadruped), "Direction: lateral");
			Assert.AreEqual(PatientDirection.QuadrupedPlantar, new PatientDirection("PL", AnatomicalOrientationType.Quadruped), "Direction: plantar");
			Assert.AreEqual(new PatientDirection("PAR", AnatomicalOrientationType.Quadruped), new PatientDirection("PAR", AnatomicalOrientationType.Quadruped), "Direction: palmarrostral");
			Assert.AreEqual(new PatientDirection("DDI", AnatomicalOrientationType.Quadruped), new PatientDirection("DDI", AnatomicalOrientationType.Quadruped), "Direction: dorsaldistal");
			Assert.AreEqual(new PatientDirection("PLRT", AnatomicalOrientationType.Quadruped), new PatientDirection("PLRT", AnatomicalOrientationType.Quadruped), "Direction: plantarright");
			Assert.AreEqual(new PatientDirection("MVCD", AnatomicalOrientationType.Quadruped), new PatientDirection("MVCD", AnatomicalOrientationType.Quadruped), "Direction: medialventralcaudal");
			Assert.AreEqual(new PatientDirection("PRLECR", AnatomicalOrientationType.Quadruped), new PatientDirection("PRLECR", AnatomicalOrientationType.Quadruped), "Direction: proximalleftcranial");

			Assert.AreNotEqual(PatientDirection.Empty, new PatientDirection("L", AnatomicalOrientationType.Quadruped), "inequality");
			Assert.AreNotEqual(PatientDirection.Left, new PatientDirection("L", AnatomicalOrientationType.Quadruped), "left vs lateral");
			Assert.AreNotEqual(PatientDirection.Left, new PatientDirection("LEL", AnatomicalOrientationType.Quadruped), "left vs leftlateral");
			Assert.AreNotEqual(new PatientDirection("LEL", AnatomicalOrientationType.Quadruped), new PatientDirection("LLE", AnatomicalOrientationType.Quadruped), "leftlateral vs lateralleft");
		}

		[Test]
		public void TestOpposingDirection()
		{
			var empty = new PatientDirection("", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("", empty.OpposingDirection.Code, "Direction: empty");

			var unspec = new PatientDirection("X", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("X", unspec.OpposingDirection.Code, "Direction: unspec");

			var lateral = new PatientDirection("L", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("X", lateral.OpposingDirection.Code, "Direction: lateral");

			var left = new PatientDirection("LE", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("X", left.OpposingDirection.Code, "Direction: left");

			var proximalleftcranial = new PatientDirection("PRLECR", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("X", proximalleftcranial.OpposingDirection.Code, "Direction: proximalleftcranial");
		}

		[Test]
		public void TestConcatenator()
		{
			var empty = new PatientDirection("", AnatomicalOrientationType.Quadruped);
			Assert.AreEqual("", (empty + new PatientDirection("", AnatomicalOrientationType.Quadruped)).Code, "Direction: empty+empty");
			Assert.AreEqual("X", (empty + new PatientDirection("X", AnatomicalOrientationType.Quadruped)).Code, "Direction: empty+unspec");
			Assert.AreEqual("X", (new PatientDirection("X", AnatomicalOrientationType.Quadruped) + empty).Code, "Direction: unspec+empty");
			Assert.AreEqual(false, (new PatientDirection("X", AnatomicalOrientationType.Quadruped) + new PatientDirection("PA", AnatomicalOrientationType.Quadruped)).IsValid, "Direction: illegal concatenation");
			Assert.AreEqual(false, (new PatientDirection("PL", AnatomicalOrientationType.Quadruped) + new PatientDirection("X", AnatomicalOrientationType.Quadruped)).IsValid, "Direction: illegal concatenation");
			Assert.AreEqual("RPL", (new PatientDirection("R", AnatomicalOrientationType.Quadruped) + new PatientDirection("PL", AnatomicalOrientationType.Quadruped)).Code, "Direction: rightplantar");
			Assert.AreEqual("DIDCD", (new PatientDirection("DI", AnatomicalOrientationType.Quadruped) + new PatientDirection("D", AnatomicalOrientationType.Quadruped) + new PatientDirection("CD", AnatomicalOrientationType.Quadruped)).Code, "Direction: distaldorsalcaudal");
		}
	}
}

#endif