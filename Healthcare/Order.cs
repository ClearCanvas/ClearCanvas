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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Order entity
	/// </summary>
	[Validation(HighLevelRulesProviderMethod = "GetValidationRules")]
	public partial class Order
	{
		public class MergeResult
		{
			/// <summary>
			/// Gets the set of ghost procedures that were generated.
			/// </summary>
			public List<Procedure> GhostProcedures { get; internal set; }
		}

		public class UnmergeResult
		{
			/// <summary>
			/// Gets the replacement order.
			/// </summary>
			public Order ReplacementOrder { get; internal set; }

			/// <summary>
			/// Gets the set of ghost procedures that were generated.
			/// </summary>
			public List<Procedure> GhostProcedures { get; internal set; }
		}

		#region Static Factory methods

		/// <summary>
		/// Factory method to create a new order.
		/// </summary>
		public static Order NewOrder(OrderCreationArgs args, IProcedureNumberBroker procedureNumberBroker, IDicomUidBroker dicomUidBroker)
		{
			// validate required members are set
			Platform.CheckMemberIsSet(args.Patient, "Patient");
			Platform.CheckMemberIsSet(args.Visit, "Visit");
			Platform.CheckMemberIsSet(args.AccessionNumber, "AccessionNumber");
			Platform.CheckMemberIsSet(args.DiagnosticService, "DiagnosticService");
			Platform.CheckMemberIsSet(args.ReasonForStudy, "ReasonForStudy");
			Platform.CheckMemberIsSet(args.OrderingFacility, "OrderingFacility");
			Platform.CheckMemberIsSet(args.OrderingPractitioner, "OrderingPractitioner");


			// create the order
			var order = new Order
			{
				Patient = args.Patient,
				Visit = args.Visit,
				AccessionNumber = args.AccessionNumber,
				DiagnosticService = args.DiagnosticService,
				ReasonForStudy = args.ReasonForStudy,
				OrderingFacility = args.OrderingFacility,
				OrderingPractitioner = args.OrderingPractitioner,
				Priority = args.Priority,
				SchedulingRequestTime = args.SchedulingRequestTime,
				EnteredTime = args.EnteredTime,
				EnteredBy = args.EnteredBy,
				EnteredComment = args.EnteredComment
			};

			if (args.Procedures == null || args.Procedures.Count == 0)
			{
				// create procedures according to the diagnostic service plan
				args.Procedures = CollectionUtils.Map<ProcedureType, Procedure>(
					args.DiagnosticService.ProcedureTypes,
					type => new Procedure(type, procedureNumberBroker.GetNext(), dicomUidBroker.GetNewUid())
								{
									PerformingFacility = args.PerformingFacility ?? args.OrderingFacility
								});
			}


			// associate all procedures with the order
			foreach (var procedure in args.Procedures)
			{
				order.AddProcedure(procedure);
			}

			// add recipients
			if (args.ResultRecipients != null)
			{
				foreach (var recipient in args.ResultRecipients)
				{
					order.ResultRecipients.Add(recipient);
				}
			}

			var recipientsContainsOrderingPractitioner = CollectionUtils.Contains(
				order.ResultRecipients,
				r => r.PractitionerContactPoint.Practitioner.Equals(args.OrderingPractitioner));

			// if the result recipients collection does not contain the ordering practitioner, add it by force
			if (!recipientsContainsOrderingPractitioner)
			{
				var orderingPractitionerContactPoint =
					// use the contact point associated to the ordering facility's information authority
					CollectionUtils.SelectFirst(args.OrderingPractitioner.ContactPoints,
						cp => args.OrderingFacility.InformationAuthority.Equals(cp.InformationAuthority) && cp.Deactivated == false)
					// or, use the default contact point
					?? CollectionUtils.SelectFirst(args.OrderingPractitioner.ContactPoints, cp => cp.IsDefaultContactPoint)
					// or, if no default, use first available active CP (should never happen)
					?? CollectionUtils.SelectFirst(args.OrderingPractitioner.ContactPoints, cp => !cp.Deactivated)
					// or, if no active CPs, use first in the collection (should never happen)
					?? CollectionUtils.FirstElement(args.OrderingPractitioner.ContactPoints);

				if (orderingPractitionerContactPoint != null)
				{
					order.ResultRecipients.Add(new ResultRecipient(orderingPractitionerContactPoint, ResultCommunicationMode.ANY));
				}
			}

			return order;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Priority
		/// </summary>
		public virtual OrderPriority Priority
		{
			get { return _priority; }
			set
			{
				_priority = value;
				_priorityRank = (int) value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this order is in a terminal state.
		/// </summary>
		public virtual bool IsTerminated
		{
			get
			{
				return _status == OrderStatus.CM || _status == OrderStatus.CA || _status == OrderStatus.DC || _status == OrderStatus.RP || _status == OrderStatus.MG;
			}
		}

		/// <summary>
		/// Gets a value indicating whether all active (i.e. non terminated) procedures in this order are performed.
		/// </summary>
		public virtual bool AreAllActiveProceduresPerformed
		{
			get
			{
				return CollectionUtils.TrueForAll(
					CollectionUtils.Select(this.Procedures, p => !p.IsTerminated),
					p => p.IsPerformed);
			}
		}

		#endregion

		#region Public operations

		public virtual List<Procedure> GetProcedures(Predicate<Procedure> filter)
		{
			return CollectionUtils.Select(_procedures, filter);
		}


		/// <summary>
		/// Adds the specified procedure to this order.
		/// </summary>
		/// <param name="procedure"></param>
		public virtual void AddProcedure(Procedure procedure)
		{
			if (procedure.Status != ProcedureStatus.SC)
				throw new WorkflowException("Only Procedures in the SC status may be added to an order.");
			if(Equals(procedure.Order, this))
				throw new WorkflowException("This procedure is already part of this order.");
			if (this.IsTerminated)
				throw new WorkflowException(string.Format("Cannot add procedure to order with status {0}.", _status));

			if(procedure.Order != null)
			{
				// JR: ideally we want to remove it from its current order collection,
				// but there seems to be a bug in NH that will cause an exception upon save if we do this
				//procedure.Order.RemoveProcedure(procedure);
			}

			procedure.Order = this;

			// add to collection
			_procedures.Add(procedure);

			// update scheduling information
			UpdateScheduling();
		}

		/// <summary>
		/// Removes the specified procedure from this order.
		/// </summary>
		/// <param name="procedure"></param>
		public virtual bool RemoveProcedure(Procedure procedure)
		{
			if (procedure.Status != ProcedureStatus.SC)
				throw new WorkflowException("Only procedures in the SC status can be removed from an order.");

			if(_procedures.Contains(procedure))
			{
				_procedures.Remove(procedure);
				procedure.Order = null;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check to see if merge is possible.
		/// </summary>
		/// <param name="mergeInfo"></param>
		/// <param name="failureReason"></param>
		/// <returns>True if the merge operation is possible, false otherwise.</returns>
		public virtual bool CanMerge(OrderMergeInfo mergeInfo, out string failureReason)
		{
			var destinationOrder = mergeInfo.MergeDestinationOrder;

			failureReason = null;
			if (this.AccessionNumber == destinationOrder.AccessionNumber)
				failureReason = SR.MessageOrderCannotMergeSameAccessionNumber;
			else if (this.Status != OrderStatus.SC || destinationOrder.Status != OrderStatus.SC)
				failureReason = SR.MessageOrderCannotMergeOrderAlreadyStarted;
			else if (!Equals(this.Patient, destinationOrder.Patient))
				failureReason = SR.MessageOrderCannotMergeDifferentPatientOrders;
			else if (!Equals(this.OrderingFacility.InformationAuthority, destinationOrder.OrderingFacility.InformationAuthority))
				failureReason = SR.MessageOrderCannotMergeDifferentOrderingFacilityOrders;
			else if (CollectionUtils.Contains(this.Procedures, p => p.DowntimeRecoveryMode)
				|| CollectionUtils.Contains(destinationOrder.Procedures, p => p.DowntimeRecoveryMode))
				failureReason = SR.MessageOrderCannotMergeDowntimeOrders;

			return string.IsNullOrEmpty(failureReason);
		}

		/// <summary>
		/// Merge the current order into the destination order specified in the mergeInfo.
		/// </summary>
		/// <param name="mergeInfo"></param>
		public virtual MergeResult Merge(OrderMergeInfo mergeInfo)
		{
			var destOrder = mergeInfo.MergeDestinationOrder;

			string failureReason;
			if (!CanMerge(mergeInfo, out failureReason))
				throw new WorkflowException(failureReason);

			_mergeInfo = mergeInfo;

			// copy all the result recipients to destination
			foreach (var rr in _resultRecipients)
			{
				var rrr = rr;
				var recipientAlreadyExist = CollectionUtils.Contains(
					destOrder.ResultRecipients, recipients => recipients.PractitionerContactPoint.Equals(rrr.PractitionerContactPoint));
				if (!recipientAlreadyExist)
					destOrder.ResultRecipients.Add((ResultRecipient)rr.Clone());
			}

			// move all the attachments to destination, and replace with ghosts
			var ghostAttachments = CollectionUtils.Map(_attachments, (OrderAttachment a) => a.CreateGhostCopy());
			foreach (var a in _attachments)
			{
				destOrder.Attachments.Add(a);
			}
			_attachments.Clear();
			foreach (var ghost in ghostAttachments)
			{
				if(PersistenceScope.Current != null)
				{
					PersistenceScope.CurrentContext.Lock(ghost.Document, DirtyState.New);
				}
				_attachments.Add(ghost);
			}

			// Move all the order notes to destination, and create ghosts of notes for this order
			var notes = OrderNote.GetNotesForOrder(this);
			var ghostNotes = CollectionUtils.Map(notes, (OrderNote n) => n.CreateGhostCopy());
			foreach (var n in notes)
			{
				n.Order = destOrder;
			}
			foreach (var n in ghostNotes)
			{
				PersistenceScope.CurrentContext.Lock(n, DirtyState.New);
			}

			// Create ghost copies of the original procedures before it is added to the destinations
			// Theese ghost procedures are required for HL7 messages.
			var ghostProcedures = CollectionUtils.Map(_procedures, (Procedure p) => p.CreateGhostCopy());

			// Move all the procedures to the destination order.
			foreach (var p in _procedures)
			{
				destOrder.AddProcedure(p);
			}

			// update destination scheduling information
			destOrder.UpdateScheduling();

			// Add ghost procedures back to the source order.
			_procedures.AddAll(ghostProcedures);

			// Set source order to merged status
			SetStatus(OrderStatus.MG);

			return new MergeResult {GhostProcedures = ghostProcedures};
		}

		/// <summary>
		/// Gets a value indicating whether this order can be unmerged from its merge destination.
		/// </summary>
		/// <param name="cancelInfo"></param>
		/// <param name="failureReason"></param>
		/// <returns></returns>
		public virtual bool CanUnmerge(OrderCancelInfo cancelInfo, out string failureReason)
		{
			// Bug #12488 - unmerging orders as it exists currently will leave ghost procedures around, which can eventually corrupt the order state after repeated merge-unmerge cycles
			failureReason = "Not Supported";
			return false;

			//var destOrder = _mergeInfo.MergeDestinationOrder;
			//failureReason = null;
			//if (_status != OrderStatus.MG || destOrder == null)
			//    failureReason = "Only orders in the MG status can be unmerged.";
			//else if (destOrder.Status != OrderStatus.SC)
			//    failureReason = "Cannot unmerge because the merge target order has already been started.";
			//else if(CollectionUtils.Contains(destOrder.Procedures, p => p.Status == ProcedureStatus.CA))
			//    failureReason = "Cannot unmerge because the merge target order has cancelled procedures.";
			//else if (cancelInfo.Reason == null)
			//    failureReason = "A reason must be provided to unmerge.";
			//else if (CollectionUtils.Contains(this.Procedures, p => p.DowntimeRecoveryMode)
			//    || CollectionUtils.Contains(destOrder.Procedures, p => p.DowntimeRecoveryMode))
			//    failureReason = "Downtime orders cannot be unmerged.";

			//return string.IsNullOrEmpty(failureReason);
		}

		/// <summary>
		/// Un-merges this order from its merge destination, returning a new order with the specified accession #,
		/// and marking this order as Replaced by the new order.
		/// </summary>
		/// <param name="cancelInfo"></param>
		/// <param name="newAccessionNumber"></param>
		/// <returns></returns>
		public virtual UnmergeResult Unmerge(OrderCancelInfo cancelInfo, string newAccessionNumber)
		{
			string failureReason;
			if (!CanUnmerge(cancelInfo, out failureReason))
				throw new WorkflowException(failureReason);

			var destOrder = _mergeInfo.MergeDestinationOrder;


			// create replacement order
			var newOrder = new Order(
					_patient,
					_visit,
					null,	// do not copy placer-number
					newAccessionNumber,  // assign new acc #
					_diagnosticService,
					_enteredTime,
					_enteredBy,
					_enteredComment,
					_schedulingRequestTime,
					null, // will be set by call to UpdateScheduling()
					null,
					null,
					_orderingPractitioner,
					_orderingFacility,
					new HashedSet<Procedure>(), // will be added later
					new HashedSet<Procedure>(), // ghosts
					CollectionUtils.Map(_resultRecipients, (ResultRecipient rr) => (ResultRecipient)rr.Clone()),
					new List<OrderAttachment>(),
					_reasonForStudy,
					_priority,
					(int)_priority,
					OrderStatus.SC,
					null,
					null,
					new HashedSet<Order>(),
					ExtendedPropertyUtils.Copy(_extendedProperties)
				);


			// reclaim order notes
			var notes = OrderNote.GetNotesForOrder(this);
			var reclaimNotes = CollectionUtils.Map(
				CollectionUtils.Select(notes, n => n.GhostOf != null),
				(OrderNote n) => n.GhostOf.Downcast<OrderNote>());
			foreach (var note in reclaimNotes)
			{
				note.Order = newOrder;
			}

			// reclaim attachments
			var reclaimAttachments = CollectionUtils.Map(_attachments,
				(OrderAttachment a) => 
					CollectionUtils.SelectFirst(destOrder.Attachments, b => Equals(a.Document.GhostOf, b.Document)));
			foreach (var attachment in reclaimAttachments)
			{
				destOrder.Attachments.Remove(attachment);
				newOrder.Attachments.Add(attachment);
			}

			// reclaim procedures
			// need to create new ghost copies on the dest order, so that HL7 can cancel them
			var reclaimProcedures = CollectionUtils.Map(_ghostProcedures, (Procedure p) => p.GhostOf);
			var ghostProcedures = CollectionUtils.Map(reclaimProcedures, (Procedure p) => p.CreateGhostCopy());
			foreach (var procedure in reclaimProcedures)
			{
				newOrder.AddProcedure(procedure);
			}
			destOrder.Procedures.AddAll(ghostProcedures);	// note: procedure Indexes are already set correctly

			// update scheduling/status information
			newOrder.UpdateScheduling();
			newOrder.UpdateStatus();

			// any orders that were merged into this order must be redirected to the new order,
			// in order to support recursive unmerge
			foreach (var sourceOrder in _mergeSourceOrders)
			{
				sourceOrder.MergeInfo.MergeDestinationOrder = newOrder;
				newOrder.MergeSourceOrders.Add(sourceOrder);
			}
			_mergeSourceOrders.Clear();

			// change status of this order to RP, and set cancel info
			_cancelInfo = (OrderCancelInfo)cancelInfo.Clone();
			_cancelInfo.ReplacementOrder = newOrder;

			// clear merge info on this order, since it is no longer considered merged
			_mergeInfo = null;

			// set status of this order to RP, and set end time manually
			SetStatus(OrderStatus.RP);
			_endTime = Platform.Time;

			return new UnmergeResult {ReplacementOrder = newOrder, GhostProcedures = ghostProcedures};
		}

		/// <summary>
		/// Cancels the order.
		/// </summary>
		/// <param name="cancelInfo"></param>
		public virtual void Cancel(OrderCancelInfo cancelInfo)
		{
			if (this.Status != OrderStatus.SC)
				throw new WorkflowException("Only orders in the SC status can be canceled");

			_cancelInfo = cancelInfo;

			// cancel/discontinue all procedures
			foreach (var procedure in _procedures)
			{
				// given that the order is still in SC, all procedures must be either
				// SC or CA - and only those in SC need to be cancelled
				if (procedure.Status == ProcedureStatus.SC)
					procedure.Cancel();
			}

			// if the order was replaced, change the status to RP
			if (cancelInfo.ReplacementOrder != null)
				SetStatus(OrderStatus.RP);

			// need to update the end-time again, after cacnelling procedures
			UpdateEndTime();
		}

		/// <summary>
		/// Discontinues the order.
		/// </summary>
		public virtual void Discontinue(OrderCancelInfo cancelInfo)
		{
			if (this.Status != OrderStatus.IP)
				throw new WorkflowException("Only orders in the IP status can be discontinued");

			_cancelInfo = cancelInfo;

			// update the status prior to cancelling the procedures
			// (otherwise cancelling the procedures will cause them to try and update the order status)
			SetStatus(OrderStatus.DC);

			// cancel or discontinue any non-terminated procedures
			foreach (var procedure in _procedures)
			{
				if (procedure.Status == ProcedureStatus.SC)
					procedure.Cancel();
				else if (procedure.Status == ProcedureStatus.IP)
					procedure.Discontinue();
			}

			// need to update the end-time again, after discontinuing procedures
			UpdateEndTime();
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.  Calling this method on a
		/// <see cref="Order"/> will also shift all child objects (Procedures, ProcedureSteps, 
		/// Reports, ProcedureCheckIn, Protocol).  The <see cref="Visit"/> is not shifted, because
		/// it is not considered a child of the order.
		/// </para>
		/// <para>
		/// Typically this method is called after the order is performed and the report(s) is 
		/// created and published, so that the entire order and all resulting documentation is
		/// shifted in time by the same amount.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_enteredTime = _enteredTime.AddMinutes(minutes);
			_schedulingRequestTime = _schedulingRequestTime.HasValue ? _schedulingRequestTime.Value.AddMinutes(minutes) : _schedulingRequestTime;
			_scheduledStartTime = _scheduledStartTime.HasValue ? _scheduledStartTime.Value.AddMinutes(minutes) : _scheduledStartTime;
			_startTime = _startTime.HasValue ? _startTime.Value.AddMinutes(minutes) : _startTime;
			_endTime = _endTime.HasValue ? _endTime.Value.AddMinutes(minutes) : _endTime;

			foreach (var procedure in _procedures)
			{
				procedure.TimeShift(minutes);
			}
		}

		#endregion

		#region Helper methods

		/// <summary>
		/// Called by a child procedure to tell the order to update its scheduling information.
		/// </summary>
		protected internal virtual void UpdateScheduling()
		{
			// set the scheduled start time to the earliest non-null scheduled start time of any child procedure
			_scheduledStartTime = MinMaxHelper.MinValue(
				_procedures,
				delegate { return true; },
				procedure => procedure.ScheduledStartTime,
				null);
		}

		/// <summary>
		/// Called by a child procedure to tell the order to update its status.  Only
		/// certain status updates can be inferred deterministically from child statuses.  If no
		/// status can be inferred, the status does not change.
		/// </summary>
		protected internal virtual void UpdateStatus()
		{
			// if the order has not yet terminated, it may need to be auto-terminated
			if (!IsTerminated)
			{
				// if all rp are cancelled, the order is cancelled
				if (CollectionUtils.TrueForAll(_procedures, procedure => procedure.Status == ProcedureStatus.CA))
				{
					SetStatus(OrderStatus.CA);
				}
				else
					// if all rp are cancelled or discontinued, the order is discontinued
					if (CollectionUtils.TrueForAll(_procedures, procedure => procedure.Status == ProcedureStatus.CA || procedure.Status == ProcedureStatus.DC))
					{
						SetStatus(OrderStatus.DC);
					}
					else
						// if all rp are cancelled, discontinued or completed, then the order is completed
						if (CollectionUtils.TrueForAll(_procedures, procedure => procedure.IsTerminated))
						{
							SetStatus(OrderStatus.CM);
						}
			}

			// if the order is still scheduled, it may need to be auto-started
			if (_status == OrderStatus.SC)
			{
				if (CollectionUtils.Contains(_procedures, procedure => procedure.Status == ProcedureStatus.IP || procedure.Status == ProcedureStatus.CM))
				{
					SetStatus(OrderStatus.IP);
				}
			}
		}

		private void SetStatus(OrderStatus status)
		{
			if (_status == status)
				return;

			_status = status;

			if (_status == OrderStatus.IP)
				UpdateStartTime();

			if (this.IsTerminated)
				UpdateEndTime();
		}

		private void UpdateStartTime()
		{
			// compute the earliest procedure start time
			_startTime = MinMaxHelper.MinValue(
				_procedures,
				delegate { return true; },
				procedure => procedure.StartTime,
				null);
		}

		private void UpdateEndTime()
		{
			// compute the latest procedure end time
			_endTime = MinMaxHelper.MaxValue(
				_procedures,
				delegate { return true; },
				procedure => procedure.EndTime,
				null);
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		private static IValidationRuleSet GetValidationRules()
		{
			var sameInformationAuthorityRule = new ValidationRule<Order>(
				OrderRules.VisitAndPerformingFacilitiesHaveSameInformationAuthority);

			return new ValidationRuleSet(new[]
			{
				sameInformationAuthorityRule
			});
		}

		#endregion
	}
}
