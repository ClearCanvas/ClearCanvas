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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Tests
{
	/// <summary>
	/// This fixture tests the behaviour of the <see cref="Report"/> and <see cref="ReportPart"/> classes.
	/// It does not test reporting workflow.
	/// </summary>
	[TestFixture]
	public class ReportTests
	{
		public ReportTests()
        {
            // set the extension factory to special test factory
            Platform.SetExtensionFactory(new TestExtensionFactory());
        }

		private Report CreateReport()
		{
			Order order = TestOrderFactory.CreateOrder(1, 1, true, true);
			return new Report(CollectionUtils.FirstElement(order.Procedures));
		}

        [Test]
        public void Test_CreateNewReport()
        {
        	Report report = CreateReport();

            // check basics
			Assert.AreEqual(ReportStatus.D, report.Status);
			Assert.IsNull(report.PreliminaryTime);
			Assert.IsNull(report.CancelledTime);
			Assert.IsNull(report.CompletedTime);
			Assert.AreEqual(1, report.Parts.Count);	// main report part is created by default
			Assert.AreEqual(ReportPartStatus.D, report.Parts[0].Status);
        }

		[Test]
        public void Test_DraftToPreliminary()
		{
			Report report = CreateReport();
			report.ActivePart.MarkPreliminary();

			// check basics
			Assert.AreEqual(ReportStatus.P, report.Status);
			Assert.IsNotNull(report.PreliminaryTime);
			Assert.IsNull(report.CancelledTime);
			Assert.IsNull(report.CompletedTime);
			Assert.AreEqual(1, report.Parts.Count);	// main report part is created by default
			Assert.AreEqual(ReportPartStatus.P, report.Parts[0].Status);
		}

		[Test]
        public void Test_DraftToFinal()
		{
			Report report = CreateReport();
			report.ActivePart.Complete();

			// check basics
			Assert.AreEqual(ReportStatus.F, report.Status);
			Assert.IsNull(report.PreliminaryTime);
			Assert.IsNull(report.CancelledTime);
			Assert.IsNotNull(report.CompletedTime);
			Assert.AreEqual(1, report.Parts.Count);	// main report part is created by default
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
		}

		[Test]
        public void Test_PreliminaryToFinal()
		{
			Report report = CreateReport();
			report.ActivePart.MarkPreliminary();
			report.ActivePart.Complete();

			// check basics
			Assert.AreEqual(ReportStatus.F, report.Status);
			Assert.IsNotNull(report.PreliminaryTime);
			Assert.IsNull(report.CancelledTime);
			Assert.IsNotNull(report.CompletedTime);
			Assert.AreEqual(1, report.Parts.Count);	// main report part is created by default
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
		}

		[Test]
        public void Test_DraftToCancelled()
		{
			Report report = CreateReport();
			report.ActivePart.Cancel();

			// check basics
			Assert.AreEqual(ReportStatus.X, report.Status);
			Assert.IsNull(report.PreliminaryTime);
			Assert.IsNotNull(report.CancelledTime);
			Assert.IsNull(report.CompletedTime);
			Assert.AreEqual(1, report.Parts.Count);	// main report part is created by default
			Assert.AreEqual(ReportPartStatus.X, report.Parts[0].Status);
		}

		[Test]
        public void Test_PreliminaryToCancelled()
		{
			Report report = CreateReport();
			report.ActivePart.MarkPreliminary();
			report.ActivePart.Cancel();

			// check basics
			Assert.AreEqual(ReportStatus.X, report.Status);
			Assert.IsNotNull(report.PreliminaryTime);
			Assert.IsNotNull(report.CancelledTime);
			Assert.IsNull(report.CompletedTime);
			Assert.AreEqual(1, report.Parts.Count);	// main report part is created by default
			Assert.AreEqual(ReportPartStatus.X, report.Parts[0].Status);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_FinalToPreliminary()
		{
			Report report = CreateReport();
			report.Parts[0].Complete();
			report.Parts[0].MarkPreliminary();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_CancelledToPreliminary()
		{
			Report report = CreateReport();
			report.Parts[0].Cancel();
			report.Parts[0].MarkPreliminary();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_FinalToCancelled()
		{
			Report report = CreateReport();
			report.Parts[0].Complete();
			report.Parts[0].Cancel();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_CancelledToFinal()
		{
			Report report = CreateReport();
			report.Parts[0].Cancel();
			report.Parts[0].Complete();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_FinalToFinal()
		{
			Report report = CreateReport();
			report.Parts[0].Complete();
			report.Parts[0].Complete();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_CancelledToCancelled()
		{
			Report report = CreateReport();
			report.Parts[0].Cancel();
			report.Parts[0].Cancel();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_AddAddendumToDraft()
		{
			Report report = CreateReport();
			report.AddAddendum();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_AddAddendumToPreliminary()
		{
			Report report = CreateReport();
			report.ActivePart.MarkPreliminary();
			report.AddAddendum();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
        public void Test_AddAddendumToCancelled()
		{
			Report report = CreateReport();
			report.ActivePart.Cancel();
			report.AddAddendum();
		}

		[Test]
        public void Test_AddAddendumToFinal()
		{
			Report report = CreateReport();
			report.ActivePart.Complete();

			report.AddAddendum();

			// check basics
			Assert.AreEqual(ReportStatus.F, report.Status);		// report is still considered Final (not Corrected until addendum is complete)
			Assert.AreEqual(2, report.Parts.Count);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
			Assert.AreEqual(ReportPartStatus.D, report.Parts[1].Status);
		}

		[Test]
        public void Test_AddendumDraftToPreliminary()
		{
			Report report = CreateReport();
			report.ActivePart.Complete();

			ReportPart addendum = report.AddAddendum();
			addendum.MarkPreliminary();

			// check basics
			Assert.AreEqual(ReportStatus.F, report.Status);		// report is still considered Final (not Corrected until addendum is complete)
			Assert.AreEqual(2, report.Parts.Count);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
			Assert.AreEqual(ReportPartStatus.P, report.Parts[1].Status);
		}

		[Test]
        public void Test_AddendumDraftToComplete()
		{
			Report report = CreateReport();
			report.ActivePart.Complete();

			ReportPart addendum = report.AddAddendum();
			addendum.Complete();

			// check basics
			Assert.AreEqual(ReportStatus.C, report.Status); // report is Corrected
			Assert.IsNotNull(report.CorrectedTime);
			Assert.AreEqual(2, report.Parts.Count);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[1].Status);
		}

		[Test]
        public void Test_AddendumCancel()
		{
			Report report = CreateReport();
			report.ActivePart.Complete();

			ReportPart addendum = report.AddAddendum();
			addendum.Cancel();

			// check basics
			Assert.AreEqual(ReportStatus.F, report.Status); // report is still considered final, since only the addendum was cancelled
			Assert.IsNull(report.CorrectedTime);	// no corrected time
			Assert.AreEqual(2, report.Parts.Count);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
			Assert.AreEqual(ReportPartStatus.X, report.Parts[1].Status);
		}

		[Test]
        public void Test_MultipleAddenda()
		{
			Report report = CreateReport();
			report.ActivePart.Complete();

			ReportPart addendum1 = report.AddAddendum();
			addendum1.Complete();

			Assert.AreEqual(ReportStatus.C, report.Status); // report is Corrected
			Assert.IsNotNull(report.CorrectedTime);
			Assert.AreEqual(2, report.Parts.Count);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[1].Status);

			DateTime? correctedTime = report.CorrectedTime;

			ReportPart addendum2 = report.AddAddendum();
			addendum2.Complete();

			// check basics
			Assert.AreEqual(ReportStatus.C, report.Status); // report is Corrected
			Assert.AreEqual(correctedTime, report.CorrectedTime);	// corrected time was not affected by the additonal addendum
			Assert.AreEqual(3, report.Parts.Count);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[0].Status);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[1].Status);
			Assert.AreEqual(ReportPartStatus.F, report.Parts[2].Status);
		}
	}
}

#endif
