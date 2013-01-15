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

using System.Collections.Generic;
using NUnit.Framework;
using Iesi.Collections.Generic;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class ExternalPractitionerTests
	{
		internal static class TestHelper
		{
			/// <summary>
			/// Create a simple practitioner with no contact point and properties.
			/// </summary>
			/// <remarks>
			/// The practitioner is not edited, not merged and not verified.
			/// </remarks>
			public static ExternalPractitioner CreatePractitioner(string familyName, string givenName)
			{
				var licenseNumber = familyName;
				var billingNumber = givenName;
				return new ExternalPractitioner(
					new PersonName(familyName, givenName, null, null, null, null),
					licenseNumber, billingNumber,
					false, null,
					null,
					new HashedSet<ExternalPractitionerContactPoint>(),
					new Dictionary<string, string>(),
					null);
			}

			/// <summary>
			/// Perform a simple merge of two practitioners.
			/// </summary>
			/// <remarks>
			/// Destination is the primary practitioner.  The result will have all info inherit from destination.
			/// No contact points are deactivated or replaced.
			/// </remarks>
			public static ExternalPractitioner SimpleMerge(ExternalPractitioner src, ExternalPractitioner dest)
			{
				var deactivatedContactPoints = new List<ExternalPractitionerContactPoint>();
				var contactPointReplacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>();
				return ExternalPractitioner.MergePractitioners(src, dest,
					dest.Name, dest.LicenseNumber, dest.BillingNumber, 
					dest.ExtendedProperties, dest.DefaultContactPoint,
					deactivatedContactPoints, contactPointReplacements);
			}
		}

		#region Basic Sanity Tests

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Same_Practitioner()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);

			TestHelper.SimpleMerge(p1, p1);
		}

		#endregion

		#region Test Activate/Deactivate Merged/NotMerged Practitioner

		[Test]
		public void Test_Deactivate_Merged_Practitioner()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsFalse(p2.Deactivated);

			TestHelper.SimpleMerge(p1, p2);

			Assert.IsTrue(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);
			Assert.IsTrue(p2.IsMerged);
			Assert.IsTrue(p2.Deactivated);

			// Should be a no-op
			p1.MarkDeactivated(true);
			p2.MarkDeactivated(true);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Activate_Merged_Practitioner()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsFalse(p2.Deactivated);

			TestHelper.SimpleMerge(p1, p2);

			Assert.IsTrue(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);
			Assert.IsTrue(p2.IsMerged);
			Assert.IsTrue(p2.Deactivated);

			// Activated merged/deactivated practitioners
			p1.MarkDeactivated(false);
			p2.MarkDeactivated(false);
		}

		[Test]
		public void Test_Deactivate_NotMerged_Practitioner()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);

			p1.MarkDeactivated(true);
			Assert.IsTrue(p1.Deactivated);
		}

		[Test]
		public void Test_Activate_NotMerged_Practitioner()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			p1.MarkDeactivated(true);

			Assert.IsFalse(p1.IsMerged);
			Assert.IsTrue(p1.Deactivated);

			p1.MarkDeactivated(false);
			Assert.IsFalse(p1.Deactivated);
		}

		#endregion

		#region Test Merge Activated/Deactivated Practitioners

		[Test]
		public void Test_Merge_Activated_Practitioners()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsFalse(p2.Deactivated);

			TestHelper.SimpleMerge(p1, p2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Deactivated_Practitioners_Left()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			p1.MarkDeactivated(true);
			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsTrue(p1.Deactivated); // left is deactivated
			Assert.IsFalse(p2.Deactivated);

			TestHelper.SimpleMerge(p1, p2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Deactivated_Practitioners_Right()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			p2.MarkDeactivated(true);
			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.IsMerged);
			Assert.IsTrue(p2.Deactivated); // Right is deactivated

			TestHelper.SimpleMerge(p1, p2);
		}

		#endregion

		#region Test Merge Merged Practitioners

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Merged_Practitioners_Left()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");
			var p3 = TestHelper.CreatePractitioner("C", "3");

			p1.SetMergedInto(p3);
			Assert.IsTrue(p1.IsMerged); // Left is already merged
			Assert.IsFalse(p2.IsMerged);

			TestHelper.SimpleMerge(p1, p2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_Merged_Practitioners_Right()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");
			var p3 = TestHelper.CreatePractitioner("C", "3");

			p2.SetMergedInto(p3);
			Assert.IsFalse(p1.IsMerged);
			Assert.IsTrue(p2.IsMerged); // Right is already merged

			TestHelper.SimpleMerge(p1, p2);
		}

		#endregion

		#region Test Merged Practitioner Properties

		[Test]
		public void Test_Merged_Practitioner_Basic_Properties()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");
			var result = TestHelper.SimpleMerge(p1, p2);

			Assert.AreEqual(p2.Name, result.Name);
			Assert.AreEqual(p2.LicenseNumber, result.LicenseNumber);
			Assert.AreEqual(p2.BillingNumber, result.BillingNumber);
		}

		[Test]
		public void Test_Merged_Practitioner_Extended_Properties()
		{
			const string testKeyP1 = "TestKey1";
			const string testKeyP2 = "TestKey2";
			const string testKeyP3 = "TestKey3";
			var p1 = TestHelper.CreatePractitioner("A", "1");
			p1.ExtendedProperties.Add(testKeyP1, "Ignored");
			p1.ExtendedProperties.Add(testKeyP2, "Ignored");

			var p2 = TestHelper.CreatePractitioner("B", "2");
			p2.ExtendedProperties.Add(testKeyP1, "Test Value 1");
			p2.ExtendedProperties.Add(testKeyP2, "Test Value 2");
			p2.ExtendedProperties.Add(testKeyP3, "Test Value 3");

			var result = TestHelper.SimpleMerge(p1, p2);

			Assert.AreEqual(result.ExtendedProperties.Count, 3);

			// Verify value of these key in p2, which is shared with p1, is propagated to result
			Assert.AreEqual(p2.ExtendedProperties[testKeyP1], result.ExtendedProperties[testKeyP1]);
			Assert.AreEqual(p2.ExtendedProperties[testKeyP2], result.ExtendedProperties[testKeyP2]);

			// Verify value of this key, which does not exist in p1, is propagated to result
			Assert.AreEqual(p2.ExtendedProperties[testKeyP3], result.ExtendedProperties[testKeyP3]);
		}

		[Test]
		public void Test_Merged_Practitioner_IsMerged()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.IsMerged);
			Assert.IsFalse(p2.IsMerged);

			var result = TestHelper.SimpleMerge(p1, p2);

			Assert.IsTrue(p1.IsMerged);
			Assert.IsTrue(p2.IsMerged);
			Assert.IsFalse(result.IsMerged); // Result is not merged
		}

		[Test]
		public void Test_Merged_Practitioner_IsVerified()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			// Verify p1 and p2 first
			p1.MarkVerified();
			p2.MarkVerified();

			Assert.IsTrue(p1.IsVerified);
			Assert.IsTrue(p2.IsVerified);

			var result = TestHelper.SimpleMerge(p1, p2);

			Assert.IsFalse(p1.IsVerified);
			Assert.IsFalse(p2.IsVerified);
			Assert.IsFalse(result.IsVerified); // result is not verified
		}

		[Test]
		public void Test_Merged_Practitioner_Deactivated()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			Assert.IsFalse(p1.Deactivated);
			Assert.IsFalse(p2.Deactivated);
	
			var result = TestHelper.SimpleMerge(p1, p2);

			Assert.IsTrue(p1.Deactivated);
			Assert.IsTrue(p2.Deactivated);
			Assert.IsFalse(result.Deactivated); // result is not deactivated
		}

		[Test]
		public void Test_Merged_Practitioner_LastEditedTime()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			Assert.IsNull(p1.LastEditedTime);
			Assert.IsNull(p2.LastEditedTime);

			var result = TestHelper.SimpleMerge(p1, p2);

			Assert.IsNotNull(p1.LastEditedTime);
			Assert.IsNotNull(p2.LastEditedTime);
			Assert.IsNotNull(result.LastEditedTime);
		}

		#endregion

		#region Test Merge Topology

		[Test]
		public void Test_Topology_Chain_Merge()
		{
			// Setup the basic practitioners, each with one contact point.
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");
			var p3 = TestHelper.CreatePractitioner("C", "3");
			var p4 = TestHelper.CreatePractitioner("D", "4");
			var cpP1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cpP1", "cpP1");
			var cpP2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p2, "cpP2", "cpP2");
			var cpP3 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p3, "cpP3", "cpP3");
			var cpP4 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p4, "cpP4", "cpP4");

			// Perform Merge p1 -> p2 -> p3 -> p4 -> ultimateDest
			var p12 = TestHelper.SimpleMerge(p1, p2);
			var p123 = TestHelper.SimpleMerge(p12, p3);
			var ultimateDest = TestHelper.SimpleMerge(p123, p4);

			// Get a reference to all the contact points
			var p12_cpP1 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP1.Name);
			var p12_cpP2 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP2.Name);
			var p123_cpP1 = CollectionUtils.SelectFirst(p123.ContactPoints, cp => cp.Name == cpP1.Name);
			var p123_cpP2 = CollectionUtils.SelectFirst(p123.ContactPoints, cp => cp.Name == cpP2.Name);
			var p123_cpP3 = CollectionUtils.SelectFirst(p123.ContactPoints, cp => cp.Name == cpP3.Name);
			var dest_cpP1 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP1.Name);
			var dest_cpP2 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP2.Name);
			var dest_cpP3 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP3.Name);
			var dest_cpP4 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP4.Name);

			// Verifying all practitioners are merged into the right one
			Assert.AreEqual(p1.MergedInto, p12);
			Assert.AreEqual(p2.MergedInto, p12);
			Assert.AreEqual(p3.MergedInto, p123);
			Assert.AreEqual(p12.MergedInto, p123);
			Assert.AreEqual(p123.MergedInto, ultimateDest);
			Assert.IsNull(ultimateDest.MergedInto);

			// Verifying all practitioners are ultimately merged into the right one
			Assert.AreEqual(p1.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p2.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p3.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p4.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p12.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p123.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(ultimateDest.GetUltimateMergeDestination(), ultimateDest);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cpP1.MergedInto, p12_cpP1);
			Assert.AreEqual(cpP2.MergedInto, p12_cpP2);
			Assert.AreEqual(p12_cpP1.MergedInto, p123_cpP1);
			Assert.AreEqual(p12_cpP2.MergedInto, p123_cpP2);
			Assert.AreEqual(cpP3.MergedInto, p123_cpP3);
			Assert.AreEqual(p123_cpP1.MergedInto, dest_cpP1);
			Assert.AreEqual(p123_cpP2.MergedInto, dest_cpP2);
			Assert.AreEqual(p123_cpP3.MergedInto, dest_cpP3);
			Assert.AreEqual(cpP4.MergedInto, dest_cpP4);
			Assert.IsNull(dest_cpP1.MergedInto);
			Assert.IsNull(dest_cpP2.MergedInto);
			Assert.IsNull(dest_cpP3.MergedInto);
			Assert.IsNull(dest_cpP4.MergedInto);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(cpP4.GetUltimateMergeDestination(), dest_cpP4);
			Assert.AreEqual(p12_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(p12_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(p123_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(p123_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(p123_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(dest_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(dest_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(dest_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(dest_cpP4.GetUltimateMergeDestination(), dest_cpP4);
		}

		[Test]
		public void Test_Topology_Binary_Merge()
		{
			// Setup the basic practitioners, each with one contact point.
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");
			var p3 = TestHelper.CreatePractitioner("C", "3");
			var p4 = TestHelper.CreatePractitioner("D", "4");
			var cpP1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cpP1", "cpP1");
			var cpP2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p2, "cpP2", "cpP2");
			var cpP3 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p3, "cpP3", "cpP3");
			var cpP4 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p4, "cpP4", "cpP4");

			// Perform Merge p1+p2->p12, p3+p4->p34, p12+p34->ultimateDest
			var p12 = TestHelper.SimpleMerge(p1, p2);
			var p34 = TestHelper.SimpleMerge(p3, p4);
			var ultimateDest = TestHelper.SimpleMerge(p12, p34);

			// Get a reference to all the contact points
			var p12_cpP1 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP1.Name);
			var p12_cpP2 = CollectionUtils.SelectFirst(p12.ContactPoints, cp => cp.Name == cpP2.Name);
			var p34_cpP3 = CollectionUtils.SelectFirst(p34.ContactPoints, cp => cp.Name == cpP3.Name);
			var p34_cpP4 = CollectionUtils.SelectFirst(p34.ContactPoints, cp => cp.Name == cpP4.Name);
			var dest_cpP1 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP1.Name);
			var dest_cpP2 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP2.Name);
			var dest_cpP3 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP3.Name);
			var dest_cpP4 = CollectionUtils.SelectFirst(ultimateDest.ContactPoints, cp => cp.Name == cpP4.Name);

			// Verifying all practitioners are merged into the right one
			Assert.AreEqual(p1.MergedInto, p12);
			Assert.AreEqual(p2.MergedInto, p12);
			Assert.AreEqual(p3.MergedInto, p34);
			Assert.AreEqual(p4.MergedInto, p34);
			Assert.AreEqual(p12.MergedInto, ultimateDest);
			Assert.AreEqual(p34.MergedInto, ultimateDest);
			Assert.IsNull(ultimateDest.MergedInto);

			// Verifying all practitioners are ultimately merged into the right one
			Assert.AreEqual(p1.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p2.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p3.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p4.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p12.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(p34.GetUltimateMergeDestination(), ultimateDest);
			Assert.AreEqual(ultimateDest.GetUltimateMergeDestination(), ultimateDest);

			// Verifying all contact points are merged into the right one
			Assert.AreEqual(cpP1.MergedInto, p12_cpP1);
			Assert.AreEqual(cpP2.MergedInto, p12_cpP2);
			Assert.AreEqual(cpP3.MergedInto, p34_cpP3);
			Assert.AreEqual(cpP4.MergedInto, p34_cpP4);
			Assert.AreEqual(p12_cpP1.MergedInto, dest_cpP1);
			Assert.AreEqual(p12_cpP2.MergedInto, dest_cpP2);
			Assert.AreEqual(p34_cpP3.MergedInto, dest_cpP3);
			Assert.AreEqual(p34_cpP4.MergedInto, dest_cpP4);
			Assert.IsNull(dest_cpP1.MergedInto);
			Assert.IsNull(dest_cpP2.MergedInto);
			Assert.IsNull(dest_cpP3.MergedInto);
			Assert.IsNull(dest_cpP4.MergedInto);

			// Verifying all contact points are ultimately merged into the right one
			Assert.AreEqual(cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(cpP4.GetUltimateMergeDestination(), dest_cpP4);
			Assert.AreEqual(p12_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(p12_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(p34_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(p34_cpP4.GetUltimateMergeDestination(), dest_cpP4);
			Assert.AreEqual(dest_cpP1.GetUltimateMergeDestination(), dest_cpP1);
			Assert.AreEqual(dest_cpP2.GetUltimateMergeDestination(), dest_cpP2);
			Assert.AreEqual(dest_cpP3.GetUltimateMergeDestination(), dest_cpP3);
			Assert.AreEqual(dest_cpP4.GetUltimateMergeDestination(), dest_cpP4);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Circular_Merge()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var p2 = TestHelper.CreatePractitioner("B", "2");

			var p12 = TestHelper.SimpleMerge(p1, p2);
			TestHelper.SimpleMerge(p12, p1); // Merge back with p1
		}

		#endregion

		#region Test Merged Default/Deactivated/Replaced/Merged Contact Points

		[Test]
		public void Test_Merged_Default_Contact_Point()
		{
			var pA = TestHelper.CreatePractitioner("A", "1");
			var cpPA1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pA, "cpPA1", "cpPA1");
			var cpPA2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pA, "cpPA2", "cpPA2");
			Assert.IsTrue(cpPA1.IsDefaultContactPoint); // oroginal default
			Assert.IsFalse(cpPA2.IsDefaultContactPoint);

			var pB = TestHelper.CreatePractitioner("B", "2");
			var cpPB1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pB, "cpPB1", "cpPB1");
			var cpPB2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pB, "cpPB2", "cpPB2");
			Assert.IsTrue(cpPB1.IsDefaultContactPoint);
			Assert.IsFalse(cpPB2.IsDefaultContactPoint);

			var newDefault = cpPB2;
			var deactivated = new List<ExternalPractitionerContactPoint>();  // None
			var replacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>(); // None
			var result = ExternalPractitioner.MergePractitioners(pA, pB,
				pA.Name, pA.LicenseNumber, pA.BillingNumber, pA.ExtendedProperties, 
				newDefault, deactivated, replacements);
			var result_cpPA1 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == newDefault.Name);

			Assert.AreEqual(result.ContactPoints.Count, 4);
			Assert.IsTrue(result_cpPA1.IsDefaultContactPoint); // new default with the same content
		}

		[Test]
		public void Test_Merge_With_No_Default_Contact_Point()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var cp1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp1", "cp1");
			Assert.AreEqual(p1.DefaultContactPoint, cp1);

			var p2 = TestHelper.CreatePractitioner("B", "2");
			Assert.IsNull(p2.DefaultContactPoint);

			var deactivatedContactPoints = new List<ExternalPractitionerContactPoint>();
			var contactPointReplacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>();
			var result = ExternalPractitioner.MergePractitioners(p1, p2,
				p2.Name, p2.LicenseNumber, p2.BillingNumber,
				p2.ExtendedProperties, p2.DefaultContactPoint,
				deactivatedContactPoints, contactPointReplacements);

			Assert.AreEqual(result.ContactPoints.Count, 1);
			Assert.IsNull(result.DefaultContactPoint);
		}

		[Test]
		public void Test_Deactivate_And_Replace_Contact_Point_1()
		{
			var pA = TestHelper.CreatePractitioner("A", "1");
			var cpPA1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pA, "cpPA1", "cpPA1");
			var cpPA2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pA, "cpPA2", "cpPA2");

			var pB = TestHelper.CreatePractitioner("B", "2");
			var cpPB1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pB, "cpPB1", "cpPB1");
			var cpPB2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pB, "cpPB2", "cpPB2");

			// This test does the following:
			// cpPA1 - replaced
			// cpPA2 - deactivated
			// cpPB1 - replaced destination
			// cpPB2 - new default
			var newDefault = cpPB2;
			var deactivated = new List<ExternalPractitionerContactPoint> {cpPA2};
			var replacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>
				{ {cpPA1, cpPB1} };

			var result = ExternalPractitioner.MergePractitioners(pA, pB,
				pA.Name, pA.LicenseNumber, pA.BillingNumber, pA.ExtendedProperties, 
				newDefault, deactivated, replacements);

			var result_cpPA1 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPA1.Name);
			var result_cpPA2 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPA2.Name);
			var result_cpPB1 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPB1.Name);
			var result_cpPB2 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPB2.Name);

			Assert.AreEqual(result.ContactPoints.Count, 3);
			Assert.IsNull(result_cpPA1); // replaced
			Assert.IsNotNull(result_cpPA2);
			Assert.IsNotNull(result_cpPB1);
			Assert.IsNotNull(result_cpPB2);

			// Check default
			Assert.IsTrue(result_cpPB2.IsDefaultContactPoint);

			// Check deactivated status
			Assert.IsTrue(result_cpPA2.Deactivated); // The only one deactivated
			Assert.IsFalse(result_cpPB1.Deactivated);
			Assert.IsFalse(result_cpPB2.Deactivated);

			// Check MergedInto
			Assert.AreEqual(cpPA1.MergedInto, result_cpPB1); // Replaced
			Assert.AreEqual(cpPA2.MergedInto, result_cpPA2);
			Assert.AreEqual(cpPB1.MergedInto, result_cpPB1);
			Assert.AreEqual(cpPB2.MergedInto, result_cpPB2);
		}

		[Test]
		public void Test_Deactivate_And_Replace_Contact_Point_2()
		{
			var pA = TestHelper.CreatePractitioner("A", "1");
			var cpPA1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pA, "cpPA1", "cpPA1");
			var cpPA2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pA, "cpPA2", "cpPA2");

			var pB = TestHelper.CreatePractitioner("B", "2");
			var cpPB1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pB, "cpPB1", "cpPB1");
			var cpPB2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(pB, "cpPB2", "cpPB2");

			// This test does the following:
			// cpPA1 - no-op, simply migrated
			// cpPA2 - deactivated and replaced
			// cpPB1 - no-op, simply migrated
			// cpPB2 - new default and new replacement
			var newDefault = cpPB2;
			var deactivated = new List<ExternalPractitionerContactPoint> { cpPA2 };
			var replacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>
				{ { cpPA2, cpPB2 } };

			var result = ExternalPractitioner.MergePractitioners(pA, pB,
				pA.Name, pA.LicenseNumber, pA.BillingNumber, pA.ExtendedProperties,
				newDefault, deactivated, replacements);

			var result_cpPA1 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPA1.Name);
			var result_cpPA2 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPA2.Name);
			var result_cpPB1 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPB1.Name);
			var result_cpPB2 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cpPB2.Name);

			Assert.AreEqual(result.ContactPoints.Count, 3);
			Assert.IsNotNull(result_cpPA1);
			Assert.IsNull(result_cpPA2); // replaced
			Assert.IsNotNull(result_cpPB1);
			Assert.IsNotNull(result_cpPB2);

			// Check default
			Assert.IsTrue(result_cpPB2.IsDefaultContactPoint);

			// Check deactivated status.  All are active, because the only deactivated cp is replaced.
			Assert.IsFalse(result_cpPA1.Deactivated);
			Assert.IsFalse(result_cpPB1.Deactivated);
			Assert.IsFalse(result_cpPB2.Deactivated);

			// Check MergedInto
			Assert.AreEqual(cpPA1.MergedInto, result_cpPA1);
			Assert.AreEqual(cpPA2.MergedInto, result_cpPB2); // Replaced
			Assert.AreEqual(cpPB1.MergedInto, result_cpPB1);
			Assert.AreEqual(cpPB2.MergedInto, result_cpPB2);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_With_Merged_Replacements()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var cp1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp2", "cp2");
			var cp3 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp3", "cp3");

			// Merge into cp12, change name and description for easy identification.
			var cp12 = ExternalPractitionerContactPointTests.TestHelper.SimpleMerge(cp1, cp2);
			cp12.Name = "cp12";
			cp12.Description = "cp12";
			Assert.IsTrue(cp12.IsDefaultContactPoint);

			// This test does the following:
			// cp1 - a merged cp that is chosen as replaced destination
			// cp2 - merged, deactivated
			// cp3 - replace
			// cp12 - retain as default
			var newDefault = cp12;
			var deactivated = new List<ExternalPractitionerContactPoint> ();
			var replacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint> { { cp3, cp1 } };

			var p2 = TestHelper.CreatePractitioner("B", "2");
			ExternalPractitioner.MergePractitioners(p1, p2,
				p2.Name, p2.LicenseNumber, p2.BillingNumber, p2.ExtendedProperties,
				newDefault, deactivated, replacements);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Merge_With_Merged_Default_ContactPoint()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var cp1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp2", "cp2");

			// Merge into cp12, change name and description for easy identification.
			var cp12 = ExternalPractitionerContactPointTests.TestHelper.SimpleMerge(cp1, cp2);
			cp12.Name = "cp12";
			cp12.Description = "cp12";

			// This test does the following:
			// cp1 - a merged cp that is chosen as new default
			// cp2 - merged, deactivated
			// cp12 - previous default
			var newDefault = cp1;
			var deactivated = new List<ExternalPractitionerContactPoint>();
			var replacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>();

			var p2 = TestHelper.CreatePractitioner("B", "2");
			ExternalPractitioner.MergePractitioners(p1, p2,
				p2.Name, p2.LicenseNumber, p2.BillingNumber, p2.ExtendedProperties,
				newDefault, deactivated, replacements);
		}

		[Test]
		public void Test_Merge_With_Merged_ContactPoints()
		{
			var p1 = TestHelper.CreatePractitioner("A", "1");
			var cp1 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp1", "cp1");
			var cp2 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p1, "cp2", "cp2");

			// Merge into cp12, change name and description for easy identification.
			var cp12 = ExternalPractitionerContactPointTests.TestHelper.SimpleMerge(cp1, cp2);
			cp12.Name = "cp12";
			cp12.Description = "cp12";

			var p2 = TestHelper.CreatePractitioner("B", "2");
			var cp3 = ExternalPractitionerContactPointTests.TestHelper.AddContactPoint(p2, "cp3", "cp3");

			var result = TestHelper.SimpleMerge(p1, p2);
			var result_cp1 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cp1.Name);
			var result_cp2 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cp2.Name);
			var result_cp12 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cp12.Name);
			var result_cp3 = CollectionUtils.SelectFirst(result.ContactPoints, cp => cp.Name == cp3.Name);

			Assert.AreEqual(2, result.ContactPoints.Count);  // should only have result_cp12 and result_cp3
			Assert.IsNull(result_cp1);
			Assert.IsNull(result_cp2);
			Assert.IsNotNull(result_cp12);
			Assert.IsNotNull(result_cp3);
		}

		#endregion
	}
}

#endif