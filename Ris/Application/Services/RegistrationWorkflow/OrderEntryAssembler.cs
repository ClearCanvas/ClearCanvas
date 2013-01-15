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

using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	public class OrderEntryAssembler
	{
		public OrderRequisition CreateOrderRequisition(Order order, IPersistenceContext context)
		{
			var patientProfileAssembler = new PatientProfileAssembler();
			var visitAssembler = new VisitAssembler();
			var pracAssembler = new ExternalPractitionerAssembler();
			var facilityAssembler = new FacilityAssembler();
			var dsAssembler = new DiagnosticServiceAssembler();
			var attachmentAssembler = new OrderAttachmentAssembler();
			var noteAssembler = new OrderNoteAssembler();
			var resultRecipientAssembler = new ResultRecipientAssembler();

			var requisition = new OrderRequisition
								{
									OrderRef = order.GetRef(),
									Patient =
										patientProfileAssembler.CreatePatientProfileSummary(
											CollectionUtils.FirstElement(order.Procedures).PatientProfile, context),
									Visit = visitAssembler.CreateVisitSummary(order.Visit, context),
									DiagnosticService = dsAssembler.CreateSummary(order.DiagnosticService),
									SchedulingRequestTime = order.SchedulingRequestTime,
									OrderingPractitioner =
										pracAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context),
									OrderingFacility = facilityAssembler.CreateFacilitySummary(order.OrderingFacility),
									ReasonForStudy = order.ReasonForStudy,
									Priority = EnumUtils.GetEnumValueInfo(order.Priority, context),
									ResultRecipients = CollectionUtils.Map<ResultRecipient, ResultRecipientDetail>(
										order.ResultRecipients,
										r => resultRecipientAssembler.CreateResultRecipientDetail(r, context)),
									Procedures = CollectionUtils.Map<Procedure, ProcedureRequisition>(
										order.Procedures,
										procedure => CreateProcedureRequisition(procedure, context)),
									Attachments = CollectionUtils.Map<OrderAttachment, AttachmentSummary>(
										order.Attachments,
										attachment => attachmentAssembler.CreateOrderAttachmentSummary(attachment, context)),
									Notes = CollectionUtils.Map<OrderNote, OrderNoteDetail>(
										OrderNote.GetNotesForOrder(order),
										note => noteAssembler.CreateOrderNoteDetail(note, context)),
									ExtendedProperties = ExtendedPropertyUtils.Copy(order.ExtendedProperties),
									CanModify = !order.IsTerminated
								};

			return requisition;
		}

		public void UpdateOrderFromRequisition(Order order, OrderRequisition requisition, Staff currentStaff, IPersistenceContext context)
		{
			// only certain properties of an order may be updated from a requisition
			// Patient cannot not be updated
			// DiagnosticService cannot be updated
			// OrderingFacility cannot be updated

			// do not update the individual procedures, as this is done separately - see UpdateProcedureFromRequisition

			// Some properties cannot be updated if the procedure is terminated
			if (!order.IsTerminated)
			{
				order.Visit = context.Load<Visit>(requisition.Visit.VisitRef, EntityLoadFlags.Proxy);

				order.SchedulingRequestTime = requisition.SchedulingRequestTime;
				order.OrderingPractitioner = context.Load<ExternalPractitioner>(
					requisition.OrderingPractitioner.PractitionerRef, EntityLoadFlags.Proxy);
				order.ReasonForStudy = requisition.ReasonForStudy;
				order.Priority = EnumUtils.GetEnumValue<OrderPriority>(requisition.Priority);

				// wipe out and reset the result recipients
				order.ResultRecipients.Clear();

				CollectionUtils.Map<ResultRecipientDetail, ResultRecipient>(
					requisition.ResultRecipients,
					s => new ResultRecipient(
							context.Load<ExternalPractitionerContactPoint>(s.ContactPoint.ContactPointRef, EntityLoadFlags.Proxy),
							EnumUtils.GetEnumValue<ResultCommunicationMode>(s.PreferredCommunicationMode))).ForEach(r => order.ResultRecipients.Add(r));
			}

			// synchronize Order.Attachments from order requisition
			var attachmentAssembler = new OrderAttachmentAssembler();
			attachmentAssembler.Synchronize(order.Attachments, requisition.Attachments, currentStaff, context);

			// synchronize Order.Notes from order requisition
			var noteAssembler = new OrderNoteAssembler();
			noteAssembler.SynchronizeOrderNotes(order, requisition.Notes, currentStaff, context);

			if (requisition.ExtendedProperties != null)
			{
				ExtendedPropertyUtils.Update(order.ExtendedProperties, requisition.ExtendedProperties);
			}
		}

		public ProcedureRequisition CreateProcedureRequisition(Procedure procedure, IPersistenceContext context)
		{
			var modality = procedure.ModalityProcedureSteps.Select(mps => mps.Modality).FirstOrDefault();

			var procedureTypeAssembler = new ProcedureTypeAssembler();
			var facilityAssembler = new FacilityAssembler();
			var departmentAssembler = new DepartmentAssembler();
			var modalityAssembler = new ModalityAssembler();

			// create requisition
			return new ProcedureRequisition(
				procedure.GetRef(),
				procedureTypeAssembler.CreateSummary(procedure.Type),
				procedure.Number,
				procedure.ScheduledStartTime,
				procedure.ScheduledDuration,
				modalityAssembler.CreateModalitySummary(modality),
				EnumUtils.GetEnumValueInfo(procedure.SchedulingCode),
				procedure.PerformingFacility == null ? null : facilityAssembler.CreateFacilitySummary(procedure.PerformingFacility),
				procedure.PerformingDepartment == null ? null : departmentAssembler.CreateSummary(procedure.PerformingDepartment, context),
				EnumUtils.GetEnumValueInfo(procedure.Laterality, context),
				procedure.Portable,
				procedure.IsPreCheckIn == false,
				EnumUtils.GetEnumValueInfo(procedure.Status, context),
				IsProcedureModifiable(procedure),
				procedure.Status == ProcedureStatus.CA || procedure.Status == ProcedureStatus.DC);
		}

		public void UpdateProcedureFromRequisition(Procedure procedure, ProcedureRequisition requisition, Staff currentUserStaff, IPersistenceContext context)
		{
			// check if the procedure was cancelled
			if (requisition.Cancelled)
			{
				if (procedure.Status == ProcedureStatus.SC)
				{
					// if RP is still scheduled, cancel it
					procedure.Cancel();
				}
				else if (procedure.Status == ProcedureStatus.IP)
				{
					// if RP in-progress, discontinue it
					procedure.Discontinue();
				}

				// early exit - nothing else to update
				return;
			}

			// The following properties are appropriate only for procedures in SC status.
			if (!IsProcedureModifiable(procedure))
				return;

			procedure.Schedule(requisition.ScheduledTime, requisition.ScheduledDuration);
			procedure.SchedulingCode = EnumUtils.GetEnumValue<SchedulingCodeEnum>(requisition.SchedulingCode, context);

			procedure.PerformingFacility = context.Load<Facility>(requisition.PerformingFacility.FacilityRef, EntityLoadFlags.Proxy);
			procedure.PerformingDepartment = requisition.PerformingDepartment == null ? null
				: context.Load<Department>(requisition.PerformingDepartment.DepartmentRef, EntityLoadFlags.Proxy);

			// if the requisition explicitly specifies a modality, assign that modality to all MPS
			// (we ignore the fact that the procedure plan can theoretically contain modality procedures steps spanning multiple 
			// DICOM modalities, since in the small clinic use-case, each procedure type generally only has a single MPS)
			if (requisition.Modality != null)
			{
				foreach (var mps in procedure.ModalityProcedureSteps)
				{
					mps.Modality = context.Load<Modality>(requisition.Modality.ModalityRef, EntityLoadFlags.Proxy);
				}
			}

			procedure.Laterality = EnumUtils.GetEnumValue<Laterality>(requisition.Laterality);
			procedure.Portable = requisition.PortableModality;

			if (requisition.CheckedIn && procedure.IsPreCheckIn)
			{
				procedure.CheckIn(currentUserStaff, null);
			}
			else if (!requisition.CheckedIn && procedure.IsCheckedIn)
			{
				procedure.RevertCheckIn();
			}
		}

		// arguably this is a business logic decision that shouldn't go here, but there is really no
		// better place to put it right now
		// note that the notion of "modifiable" here is specific to the idea of a "requisition"
		// The "requisition" is modifiable only as long as the procedure is in the SC status
		private static bool IsProcedureModifiable(Procedure procedure)
		{
			return procedure.Status == ProcedureStatus.SC;
		}
	}
}
