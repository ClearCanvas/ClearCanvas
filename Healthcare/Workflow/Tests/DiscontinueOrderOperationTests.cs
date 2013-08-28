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
using NUnit.Framework;
using ClearCanvas.Workflow;

// disable warnings about DiscontinueOrderOperation being obsolete
#pragma warning disable 618

namespace ClearCanvas.Healthcare.Workflow.Tests
{
    [TestFixture]
    public class DiscontinueOrderOperationTests
    {
        class ConcreteReportingProcedureStep : ReportingProcedureStep
        {

            public ConcreteReportingProcedureStep(Procedure procedure)
                : base(procedure, new ReportPart())
            {

            }

            public override string Name
            {
                get { return "Concrete Reporting Procedure Step"; }
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [Test]
        public void Test_Execute()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            Assert.AreEqual(OrderStatus.IP, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Scheduling()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            Assert.AreEqual(OrderStatus.SC, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Complete()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            procedure.Complete(DateTime.Now);
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CM, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Discontinue()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            order.Discontinue(new OrderCancelInfo());
            Assert.AreEqual(OrderStatus.DC, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Cancel()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            order.Cancel(new OrderCancelInfo());
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CA, order.Status);

            op.Execute(order, info);
        }

        [Test]
        public void Test_CanExecute_InProgress()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            Assert.AreEqual(OrderStatus.IP, order.Status);

            Assert.IsTrue(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            Assert.AreEqual(OrderStatus.SC, order.Status);
            Assert.IsFalse(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_Complete()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            procedure.Complete(DateTime.Now);
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CM, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_Discontinue()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            order.Discontinue(new OrderCancelInfo());
            Assert.AreEqual(OrderStatus.DC, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_Cancel()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            order.Cancel(new OrderCancelInfo());
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CA, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }
    }
}

#endif
