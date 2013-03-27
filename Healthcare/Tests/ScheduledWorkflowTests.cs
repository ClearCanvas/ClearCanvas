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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;
using System.Collections;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ScheduledWorkflowTests
    {
        private OrderCancelReasonEnum _defaultCancelReason;

        public ScheduledWorkflowTests()
        {
            _defaultCancelReason = new OrderCancelReasonEnum("x", "x", "x");

            // set the extension factory to special test factory
            Platform.SetExtensionFactory(new TestExtensionFactory());
        }

        /// <summary>
        /// Verify that when a new order is created, all properties are set correctly, and the diagnostic
        /// service plan is correctly applied to the order.
        /// </summary>
        [Test]
        public void Test_CreateNewOrderScheduled()
        {
            DateTime scheduleTime = DateTime.Now;

            Patient patient = TestPatientFactory.CreatePatient();
            Visit visit = TestVisitFactory.CreateVisit(patient);
            DiagnosticService ds = TestDiagnosticServiceFactory.CreateDiagnosticService();
            string accession = "10000001";
            string reasonForStudy = "Test";
            ExternalPractitioner orderingPrac = TestExternalPractitionerFactory.CreatePractitioner();
            Facility facility = TestFacilityFactory.CreateFacility();

            Order order = Order.NewOrder(new OrderCreationArgs(Platform.Time, TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)), null,
                accession, patient, visit, ds, reasonForStudy, OrderPriority.R, facility, facility,
                scheduleTime, orderingPrac, new List<ResultRecipient>()), new TestProcedureNumberBroker(), new TestDicomUidBroker());

            // check basics
            Assert.AreEqual(accession, order.AccessionNumber);
            Assert.AreEqual(reasonForStudy, order.ReasonForStudy);
            Assert.AreEqual(patient, order.Patient);
            Assert.AreEqual(visit, order.Visit);
            Assert.AreEqual(ds, order.DiagnosticService);
            Assert.AreEqual(scheduleTime, order.SchedulingRequestTime);
            Assert.AreEqual(null, order.ScheduledStartTime);    // because the order has not been scheduled
            Assert.AreEqual(null, order.StartTime);    // because the order has not been started
            Assert.AreEqual(null, order.EndTime);    // because the order has not been completed
            Assert.AreEqual(orderingPrac, order.OrderingPractitioner);
            Assert.AreEqual(facility, order.OrderingFacility);
            Assert.AreEqual(OrderPriority.R, order.Priority);
            CheckStatus(OrderStatus.SC, order);

            // check that diagnostic service plan was copied properly
            Assert.AreEqual(ds.ProcedureTypes.Count, order.Procedures.Count);
            foreach (Procedure rp in order.Procedures)
            {
                CheckStatus(ProcedureStatus.SC, rp);

                ProcedureType rpType = CollectionUtils.SelectFirst(ds.ProcedureTypes,
                    delegate(ProcedureType rpt) { return rpt.Equals(rp.Type); });

                Assert.IsNotNull(rpType, "diagnostic service plan not copied correctly");
                foreach (ModalityProcedureStep mps in rp.ModalityProcedureSteps)
                {
                    CheckStatus(ActivityStatus.SC, mps);
                }
            }
        }

        /// <summary>
        /// Schedule an unscheduled procedure step, verify that order and procedure scheduling
        /// are updated to reflect the scheduled time.
        /// </summary>
        [Test]
		public void Test_ScheduleProcedureStep()
        {
            // create an unscheduled order
            Order order = TestOrderFactory.CreateOrder(1, 1, true);

            DateTime scheduleTime = DateTime.Now;

            Procedure rp = CollectionUtils.FirstElement(order.Procedures);
            ProcedureStep step = CollectionUtils.FirstElement(rp.ProcedureSteps);
            step.Schedule(scheduleTime);

            Assert.AreEqual(scheduleTime, step.Scheduling.StartTime);

            // verify order and rp scheduled time are updated to reflect earliest time
            Assert.AreEqual(scheduleTime, rp.ScheduledStartTime);
            Assert.AreEqual(scheduleTime, order.ScheduledStartTime);
        }

        /// <summary>
        /// Verify that when all procedure steps are unscheduled, the procedure and order are
        /// also unscheduled.
        /// </summary>
        [Test]
		public void Test_UnscheduleProcedureStep()
        {
            // create a scheduled order
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // unschedule all steps
            foreach (Procedure rp in order.Procedures)
            {
                foreach (ProcedureStep step in rp.ProcedureSteps)
                {
                    step.Schedule(null);
                }
                // verify rp start time is null
                Assert.IsNull(rp.ScheduledStartTime);
            }

            // verify order start time is null
            Assert.IsNull(order.ScheduledStartTime);
        }

        /// <summary>
        /// Verify that when a procedure step is rescheduled, procedure and order scheduling information
        /// is updated to reflect the earlist procedure step.
        /// </summary>
        [Test]
		public void Test_RescheduleEarlier()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true, true);

            DateTime originalTime = (DateTime)order.ScheduledStartTime;
            DateTime newTime = originalTime - TimeSpan.FromDays(1);

            Procedure rp = CollectionUtils.FirstElement(order.Procedures);
            ProcedureStep step = CollectionUtils.FirstElement(rp.ProcedureSteps);
            step.Schedule(newTime);

            Assert.AreEqual(newTime, step.Scheduling.StartTime);

            // verify order and rp scheduled time are updated to reflect earliest time
            Assert.AreEqual(newTime, rp.ScheduledStartTime);
            Assert.AreEqual(newTime, order.ScheduledStartTime);
        }

        /// <summary>
        /// Verify that when a procedure step is rescheduled later, the order and procedure
        /// scheduling information still reflects the earliest start time.
        /// </summary>
        [Test]
		public void Test_RescheduleLater()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true, true);

            DateTime originalTime = (DateTime)order.ScheduledStartTime;
            DateTime newTime = originalTime + TimeSpan.FromDays(1);

            Procedure rp = CollectionUtils.FirstElement(order.Procedures);
            ProcedureStep step = CollectionUtils.FirstElement(rp.ProcedureSteps);
            step.Schedule(newTime);

            Assert.AreEqual(newTime, step.Scheduling.StartTime);

            // verify order and rp scheduled time still reflect earliest time
            Assert.AreEqual(originalTime, rp.ScheduledStartTime);
            Assert.AreEqual(originalTime, order.ScheduledStartTime);
        }

        /// <summary>
        /// When an order is cancelled, verify that all procedures are cancelled, and all
        /// procedure steps are discontinued.
        /// </summary>
        [Test]
		public void Test_CancelOrderFromScheduled()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);
            CheckStatus(OrderStatus.SC, order);

			order.Cancel(new OrderCancelInfo(_defaultCancelReason, TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)), "", null));

            CheckStatus(OrderStatus.CA, order);
            Assert.IsNull(order.StartTime);
            Assert.IsNotNull(order.EndTime);

            foreach (Procedure rp in order.Procedures)
            {
                CheckStatus(ProcedureStatus.CA, rp);
                Assert.IsNull(rp.StartTime);
                Assert.IsNotNull(rp.EndTime);
                foreach (ProcedureStep step in rp.ProcedureSteps)
                {
                    CheckStatus(ActivityStatus.DC, step);
                }
            }
        }

        /// <summary>
        /// Verify that an order with cancelled procedures can be cancelled.
        /// (bug #3440)
        /// </summary>
        [Test]
		public void Test_CancelOrderWithCancelledProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);
            CheckStatus(OrderStatus.SC, order);

            // cancel one of the procedures - this will not cancel the order
            Procedure p1 = CollectionUtils.FirstElement(order.Procedures);
            p1.Cancel();

            // order is still scheduled
            CheckStatus(OrderStatus.SC, order);

            // now cancel order
            order.Cancel(new OrderCancelInfo(_defaultCancelReason, TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)), "", null));

            CheckStatus(OrderStatus.CA, order);
            Assert.IsNull(order.StartTime);
            Assert.IsNotNull(order.EndTime);

            foreach (Procedure rp in order.Procedures)
            {
                CheckStatus(ProcedureStatus.CA, rp);
                Assert.IsNull(rp.StartTime);
                Assert.IsNotNull(rp.EndTime);
                foreach (ProcedureStep step in rp.ProcedureSteps)
                {
                    CheckStatus(ActivityStatus.DC, step);
                }
            }
        }

		/// <summary>
		/// When an order is replaced, verify that all procedures are cancelled, and all
		/// procedure steps are discontinued.
		/// </summary>
		[Test]
		public void Test_ReplaceOrderFromScheduled()
		{
			Order order = TestOrderFactory.CreateOrder(2, 2, true);
			CheckStatus(OrderStatus.SC, order);

			Order replacement = TestOrderFactory.CreateOrder(2, 2, true);

			order.Cancel(new OrderCancelInfo(_defaultCancelReason, TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)), "", replacement));

			CheckStatus(OrderStatus.RP, order);
			Assert.IsNull(order.StartTime);
			Assert.IsNotNull(order.EndTime);

			foreach (Procedure rp in order.Procedures)
			{
				CheckStatus(ProcedureStatus.CA, rp);
				Assert.IsNull(rp.StartTime);
				Assert.IsNotNull(rp.EndTime);
				foreach (ProcedureStep step in rp.ProcedureSteps)
				{
					CheckStatus(ActivityStatus.DC, step);
				}
			}
		}

        /// <summary>
        /// Verify that an order cannot be cancelled after it is already in progress.
        /// </summary>
        [Test]
		public void Test_CancelOrderFromInProgress()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // put the order in progress
            Procedure rp = CollectionUtils.FirstElement(order.Procedures);
            ProcedureStep step = CollectionUtils.FirstElement(rp.ProcedureSteps);
			step.Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

            try
            {
				order.Cancel(new OrderCancelInfo(_defaultCancelReason, TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)), "", null));

                Assert.Fail("expected exception when trying to cancel non-scheduled order");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf(typeof(WorkflowException), e);
            }
        }

        /// <summary>
        /// When an order is discontinued, verify that scheduled procedures are discontinued
        /// but in progress procedures are allowed to complete.
        /// </summary>
        [Test]
		public void Test_DiscontinueOrder()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // copy req procs to a list so we can access them by index
            List<Procedure> reqProcs = new List<Procedure>(
                new TypeSafeEnumerableWrapper<Procedure>(order.Procedures));
            Procedure rp1 = reqProcs[0];
            Procedure rp2 = reqProcs[1];

            // start rp 1
			rp1.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)));

			order.Discontinue(new OrderCancelInfo(_defaultCancelReason, TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)), "", null));

            // rp 2 is canceled
            CheckStatus(ProcedureStatus.CA, rp2);
            Assert.IsNull(rp2.StartTime);   // rp2 was never started
            Assert.IsNotNull(rp2.EndTime);   

            // rp 1 is discontinued
			CheckStatus(ProcedureStatus.DC, rp1);
            Assert.IsNotNull(rp1.StartTime);
			Assert.IsNotNull(rp1.EndTime);

            // order is discontinued
            CheckStatus(OrderStatus.DC, order);
            Assert.IsNotNull(order.StartTime);
            Assert.IsNotNull(order.EndTime);   // end-time is set because order is discontinued, even though rp1 is still in progress

        }

        /// <summary>
        /// Verify that order is auto-discontinued when all procedures are cancelled.
        /// </summary>
        [Test]
		public void Test_AutoDiscontinueOrder()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // copy req procs to a list so we can access them by index
            List<Procedure> reqProcs = new List<Procedure>(
                new TypeSafeEnumerableWrapper<Procedure>(order.Procedures));
            Procedure rp1 = reqProcs[0];
            Procedure rp2 = reqProcs[1];

            // start and discontinue rp1
			rp1.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));
            rp1.Discontinue();
            Assert.IsNotNull(rp1.StartTime);
            Assert.IsNotNull(rp1.EndTime);

            // cancel rp2
            rp2.Cancel();
            Assert.IsNull(rp2.StartTime);
            Assert.IsNotNull(rp2.EndTime);

            // order should be discontinued
            CheckStatus(OrderStatus.DC, order);
            Assert.IsNotNull(order.StartTime);
            Assert.IsNotNull(order.EndTime);
        }

        /// <summary>
        /// Verify that order is auto-cancelled when all procedures are cancelled.
        /// </summary>
        [Test]
		public void Test_AutoCancelOrder()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            foreach (Procedure rp in order.Procedures)
            {
                rp.Cancel();
                Assert.IsNull(rp.StartTime);
                Assert.IsNotNull(rp.EndTime);
            }

            CheckStatus(OrderStatus.CA, order);
            Assert.IsNull(order.StartTime);
            Assert.IsNotNull(order.EndTime);
        }

        /// <summary>
        /// Verify that an order and procedure automatically move to the IP status when one of its procedure steps
        /// is started.
        /// </summary>
        [Test]
		public void Test_AutoStartOrderProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // put the order in progress
            Procedure rp = CollectionUtils.FirstElement(order.Procedures);
            ProcedureStep step = CollectionUtils.FirstElement(rp.ProcedureSteps);
			step.Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

            // procedure is in progress
            CheckStatus(ProcedureStatus.IP, rp);
            Assert.IsNotNull(rp.StartTime);

            // order is in progress
            CheckStatus(OrderStatus.IP, order);
            Assert.IsNotNull(order.StartTime);
        }

        /// <summary>
        /// Verify that an order and procedure automatically move to the CM status when a <see cref="PublicationStep"/>
        /// is completed, and that the order is still completed even if another procedure was discontinued.
        /// </summary>
        [Test]
		public void Test_Test_AutoCompleteOrderProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(2, 1, true);
            // copy req procs to a list so we can access them by index
            List<Procedure> reqProcs = new List<Procedure>(
                new TypeSafeEnumerableWrapper<Procedure>(order.Procedures));
            Procedure rp1 = reqProcs[0];
            Procedure rp2 = reqProcs[1];


            // cancel rp2
            rp2.Cancel();

            // complete rp1 and publish it
			rp1.ModalityProcedureSteps[0].Complete(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

            PublicationStep pub1 = new PublicationStep();
            pub1.ReportPart = new ReportPart(new Report(), 0); // must have a report part or we get null-ref exception
            rp1.AddProcedureStep(pub1);

			pub1.Complete(TestStaffFactory.CreateStaff(new StaffTypeEnum("PRAD", null, null)));

            CheckStatus(ProcedureStatus.CA, rp2);
            Assert.IsNull(rp2.StartTime);
            Assert.IsNotNull(rp2.EndTime);

            CheckStatus(ProcedureStatus.CM, rp1);
            Assert.IsNotNull(rp1.StartTime);
            Assert.IsNotNull(rp1.EndTime);

            CheckStatus(OrderStatus.CM, order);
            Assert.IsNotNull(rp1.StartTime);
            Assert.IsNotNull(rp1.EndTime);
        }


        /// <summary>
        /// Verify that when a procedure is cancelled, all steps are discontinued.
        /// </summary>
        [Test]
		public void Test_CancelProcedureFromScheduled()
        {
            Order order = TestOrderFactory.CreateOrder(1, 2, true);

            // copy req procs to a list so we can access them by index
            List<Procedure> reqProcs = new List<Procedure>(
                new TypeSafeEnumerableWrapper<Procedure>(order.Procedures));
            Procedure rp1 = reqProcs[0];

            rp1.Cancel();
            CheckStatus(ProcedureStatus.CA, rp1);
            Assert.IsNull(rp1.StartTime);
            Assert.IsNotNull(rp1.EndTime);

            foreach (ProcedureStep step in rp1.ProcedureSteps)
            {
                // all steps were scheduled, so they should all be discontinued
                CheckStatus(ActivityStatus.DC, step);
                Assert.IsNull(step.StartTime);
                Assert.IsNotNull(step.EndTime);
            }
        }

        /// <summary>
        /// Verify that an in-progress procedure cannot be cancelled.
        /// </summary>
        [Test]
		public void Test_CancelProcedureFromInProgress()
        {
            try
            {
                Order order = TestOrderFactory.CreateOrder(1, 1, true);

                Procedure rp = CollectionUtils.FirstElement(order.Procedures);
				rp.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

                CheckStatus(ProcedureStatus.IP, rp);

                rp.Cancel();

                Assert.Fail("expected exception when trying to cancel in progress procedure");

            }
            catch (WorkflowException e)
            {
                Assert.IsInstanceOf(typeof(WorkflowException), e);
            }
        }

        /// <summary>
        /// Verify that when a procedure is discontinued:
        /// a) SC, IP steps are discontinued
        /// b) CM, DC steps are unchanged.
        /// </summary>
        [Test]
		public void Test_DiscontinueProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(1, 3, true);

            // copy req procs to a list so we can access them by index
            List<Procedure> reqProcs = new List<Procedure>(
                new TypeSafeEnumerableWrapper<Procedure>(order.Procedures));
            Procedure rp1 = reqProcs[0];

            // put one mps in progress and the other completed, leaving the third scheduled
			rp1.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));
			rp1.ModalityProcedureSteps[1].Complete(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

            // discontinue rp1
            rp1.Discontinue();

            CheckStatus(ProcedureStatus.DC, rp1);
            Assert.IsNotNull(rp1.StartTime);
            Assert.IsNotNull(rp1.EndTime);

            // expect scheduled step was discontinued
            CheckStatus(ActivityStatus.DC, rp1.ModalityProcedureSteps[2]);
            Assert.IsNull(rp1.ModalityProcedureSteps[2].StartTime);
            Assert.IsNotNull(rp1.ModalityProcedureSteps[2].EndTime);

			// expect in-progress step was discontinued
			CheckStatus(ActivityStatus.DC, rp1.ModalityProcedureSteps[0]);
			Assert.IsNotNull(rp1.ModalityProcedureSteps[0].StartTime);
			Assert.IsNotNull(rp1.ModalityProcedureSteps[0].EndTime);
			
			// expect completed steps unchanged
            CheckStatus(ActivityStatus.CM, rp1.ModalityProcedureSteps[1]);
            Assert.IsNotNull(rp1.ModalityProcedureSteps[1].StartTime);
            Assert.IsNotNull(rp1.ModalityProcedureSteps[1].EndTime);

        }

        /// <summary>
        /// Verify that a procedure step can be started, and that the order/procedure move to IP status.
        /// </summary>
        [Test]
		public void Test_StartProcedureStep()
        {
            Order order = TestOrderFactory.CreateOrder(1, 1, true);

            Procedure rp1 = CollectionUtils.FirstElement(order.Procedures);
            ModalityProcedureStep mps1 = rp1.ModalityProcedureSteps[0];
            CheckStatus(ActivityStatus.SC, mps1);

			mps1.Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

            CheckStatus(ActivityStatus.IP, mps1);
            Assert.IsNotNull(mps1.StartTime);

            CheckStatus(ProcedureStatus.IP, rp1);
            Assert.IsNotNull(rp1.StartTime);

            CheckStatus(OrderStatus.IP, order);
            Assert.IsNotNull(order.StartTime);
        }

        /// <summary>
        /// Verify that a procedure step can be completed directly from the SC status,
        /// and that the order/procedure status are unchanged.
        /// </summary>
        [Test]
		public void Test_CompleteProcedureStepFromScheduled()
        {
            Order order = TestOrderFactory.CreateOrder(1, 1, true);

            Procedure rp1 = CollectionUtils.FirstElement(order.Procedures);
            ModalityProcedureStep mps1 = rp1.ModalityProcedureSteps[0];
            CheckStatus(ActivityStatus.SC, mps1);

			mps1.Complete(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

            CheckStatus(ActivityStatus.CM, mps1);
            Assert.IsNotNull(mps1.StartTime);
            Assert.IsNotNull(mps1.EndTime);

            CheckStatus(ProcedureStatus.IP, rp1);
            Assert.IsNotNull(rp1.StartTime);
            Assert.IsNull(rp1.EndTime);

            CheckStatus(OrderStatus.IP, order);
            Assert.IsNotNull(order.StartTime);
            Assert.IsNull(order.EndTime);
        }

        /// <summary>
        /// Verify that a procedure step can be completed from the IP status,
        /// and that the order/procedure status are unchanged.
        /// </summary>
        [Test]
		public void Test_CompleteProcedureStepFromInProgress()
        {
            Order order = TestOrderFactory.CreateOrder(1, 1, true);
            Procedure rp1 = CollectionUtils.FirstElement(order.Procedures);
            ModalityProcedureStep mps1 = rp1.ModalityProcedureSteps[0];
            CheckStatus(ActivityStatus.SC, mps1);

			mps1.Start(TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null)));

            CheckStatus(ActivityStatus.IP, mps1);

            mps1.Complete();

            CheckStatus(ActivityStatus.CM, mps1);
            Assert.IsNotNull(mps1.StartTime);
            Assert.IsNotNull(mps1.EndTime);

            CheckStatus(ProcedureStatus.IP, rp1);
            Assert.IsNotNull(rp1.StartTime);
            Assert.IsNull(rp1.EndTime);

            CheckStatus(OrderStatus.IP, order);
            Assert.IsNotNull(order.StartTime);
            Assert.IsNull(order.EndTime);
        }

        /// <summary>
        /// Verify that a procedure step can be discontinued,
        /// and that the parent procedure order status are unchanged
        /// (assuming it is not the only procedure step).
        /// </summary>
        [Test]
		public void Test_DiscontinueProcedureStep()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            CheckStatus(OrderStatus.SC, order);

            Procedure rp1 = CollectionUtils.FirstElement(order.Procedures);
            CheckStatus(ProcedureStatus.SC, rp1);

            rp1.ModalityProcedureSteps[0].Discontinue();
            CheckStatus(ActivityStatus.DC, rp1.ModalityProcedureSteps[0]);
            Assert.IsNull(rp1.ModalityProcedureSteps[0].StartTime);
            Assert.IsNotNull(rp1.ModalityProcedureSteps[0].EndTime);

            // rp and order status unchanged
            CheckStatus(ProcedureStatus.SC, rp1);
            Assert.IsNull(rp1.StartTime);
            Assert.IsNull(rp1.EndTime);

            CheckStatus(OrderStatus.SC, order);
            Assert.IsNull(order.StartTime);
            Assert.IsNull(order.EndTime);
        }

        /// <summary>
        /// Verify that when all procedure steps are discontinued, a procedure is automatically 
        /// discontinued.
        /// </summary>
        [Test]
		public void Test_AutoDiscontinueProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            CheckStatus(OrderStatus.SC, order);

            foreach (Procedure rp in order.Procedures)
            {
                foreach (ProcedureStep step in rp.ProcedureSteps)
                {
                    step.Discontinue();
                    CheckStatus(ActivityStatus.DC, step);
                }
                CheckStatus(ProcedureStatus.DC, rp);
                Assert.IsNull(rp.StartTime);
                Assert.IsNotNull(rp.EndTime);
            }

            CheckStatus(OrderStatus.DC, order);
            Assert.IsNull(order.StartTime);
            Assert.IsNotNull(order.EndTime);
        }

        #region Helper methods

        private void CheckStatus(OrderStatus status, Order o)
        {
            Assert.AreEqual(status, o.Status, string.Format("Exptected {0} status {1}", o.GetClass().Name, status.ToString()));
        }
        private void CheckStatus(ProcedureStatus status, Procedure o)
        {
            Assert.AreEqual(status, o.Status, string.Format("Exptected {0} status {1}", o.GetClass().Name, status.ToString()));
        }
        private void CheckStatus(ActivityStatus status, ProcedureStep o)
        {
            Assert.AreEqual(status, o.State, string.Format("Exptected {0} status {1}", o.GetClass().Name, status.ToString()));
        }

        #endregion
    }
}

#endif
