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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class OrderNoteAssembler
    {
        #region OrderNoteSynchronizeHelper

        class OrderNoteSynchronizeHelper : CollectionSynchronizeHelper<OrderNote, OrderNoteDetail>
        {
            private readonly Order _order;
            private readonly OrderNoteAssembler _assembler;
            private readonly Staff _newNoteAuthor;
            private readonly IPersistenceContext _context;

            public OrderNoteSynchronizeHelper(OrderNoteAssembler assembler, Order order, Staff newNoteAuthor, IPersistenceContext context)
                : base(false, false)
            {
                _assembler = assembler;
                _order = order;
                _newNoteAuthor = newNoteAuthor;
                _context = context;
            }

            protected override bool CompareItems(OrderNote domainItem, OrderNoteDetail sourceItem)
            {
                return domainItem.GetRef().Equals(sourceItem.OrderNoteRef, true);
            }

            protected override void AddItem(OrderNoteDetail sourceItem, ICollection<OrderNote> notes)
            {
                notes.Add(_assembler.CreateOrderNote(sourceItem, _order, _newNoteAuthor, true, _context));
            }
        }

        #endregion

        /// <summary>
        /// Synchronizes an order's Notes collection, adding any notes that don't already exist in the collection,
        /// using the specified author.  The notes are posted immediately.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="sourceList"></param>
        /// <param name="newNoteAuthor"></param>
        /// <param name="context"></param>
        public void SynchronizeOrderNotes(Order order, IList<OrderNoteDetail> sourceList, Staff newNoteAuthor, IPersistenceContext context)
        {
            List<OrderNote> existingNotes = new List<OrderNote>(OrderNote.GetNotesForOrder(order));
            foreach (OrderNoteDetail detail in sourceList)
            {
                if (!CollectionUtils.Contains(existingNotes,
                    delegate(OrderNote n) { return n.GetRef().Equals(detail.OrderNoteRef); }))
                {
                    CreateOrderNote(detail, order, newNoteAuthor, true, context);
                }
            }
        }

        public OrderNoteDetail CreateOrderNoteDetail(OrderNote orderNote, IPersistenceContext context)
        {
            return CreateOrderNoteDetail(orderNote, null, context);
        }
        /// <summary>
        /// Creates an <see cref="OrderNoteDetail"/> from a <see cref="OrderNote"/>.
        /// </summary>
        /// <param name="orderNote"></param>
        /// <param name="currentUserStaff"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public OrderNoteDetail CreateOrderNoteDetail(OrderNote orderNote, Staff currentUserStaff, IPersistenceContext context)
        {
            List<OrderNoteDetail.StaffRecipientDetail> staffRecipients = new List<OrderNoteDetail.StaffRecipientDetail>();
            List<OrderNoteDetail.GroupRecipientDetail> groupRecipients = new List<OrderNoteDetail.GroupRecipientDetail>();

            // if the note has been posted, construct the recipients list from the postings, so we can get the ACK status
            if (orderNote.IsPosted)
            {
                foreach (NotePosting posting in orderNote.Postings)
                {
                    if (posting is GroupNotePosting)
                    {
                        groupRecipients.Add(
                            CreateGroupRecipientDetail(((GroupNotePosting)posting).Recipient,
                                                       posting.IsAcknowledged,
                                                       posting.AcknowledgedBy, context));
                    }
                    else
                    {
                        staffRecipients.Add(
                            CreateStaffRecipientDetail(((StaffNotePosting)posting).Recipient,
                                                       posting.IsAcknowledged,
                                                       posting.AcknowledgedBy, context));
                    }
                }
            }

            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler groupAssembler = new StaffGroupAssembler();
            return new OrderNoteDetail(
                orderNote.GetRef(),
                orderNote.Category,
                orderNote.CreationTime,
                orderNote.PostTime,
                staffAssembler.CreateStaffSummary(orderNote.Author, context),
                orderNote.OnBehalfOfGroup == null ? null : groupAssembler.CreateSummary(orderNote.OnBehalfOfGroup),
                orderNote.Urgent,
                staffRecipients,
                groupRecipients,
                orderNote.Body,
                currentUserStaff == null ? false : orderNote.CanAcknowledge(currentUserStaff));
        }

        /// <summary>
        /// Creates an <see cref="OrderNoteSummary"/> from a <see cref="OrderNote"/>.
        /// </summary>
        /// <param name="orderNote"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public OrderNoteSummary CreateOrderNoteSummary(OrderNote orderNote, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler groupAssembler = new StaffGroupAssembler();
            return new OrderNoteSummary(
                orderNote.OID == null ? null : orderNote.GetRef(), // may be a virtual order note, which is transient
                orderNote.Category,
                orderNote.CreationTime,
                orderNote.PostTime,
                staffAssembler.CreateStaffSummary(orderNote.Author, context),
                orderNote.OnBehalfOfGroup == null ? null : groupAssembler.CreateSummary(orderNote.OnBehalfOfGroup),
                orderNote.IsFullyAcknowledged,
                orderNote.Urgent,
                orderNote.Body);
        }

        /// <summary>
        /// Creates a new <see cref="OrderNote"/> based on the information provided in the specified detail object.
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="order"></param>
        /// <param name="author"></param>
        /// <param name="post"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public OrderNote CreateOrderNote(OrderNoteDetail detail, Order order, Staff author, bool post, IPersistenceContext context)
        {
            List<Staff> staffRecipients = new List<Staff>();
            staffRecipients.AddRange(
                CollectionUtils.Map<OrderNoteDetail.StaffRecipientDetail, Staff>(detail.StaffRecipients ?? new List<OrderNoteDetail.StaffRecipientDetail>(),
                    delegate(OrderNoteDetail.StaffRecipientDetail item)
                    {
                        return context.Load<Staff>(item.Staff.StaffRef, EntityLoadFlags.Proxy);
                    }));

            List<StaffGroup> groupRecipients = new List<StaffGroup>();
            groupRecipients.AddRange(
                CollectionUtils.Map<OrderNoteDetail.GroupRecipientDetail, StaffGroup>(detail.GroupRecipients ?? new List<OrderNoteDetail.GroupRecipientDetail>(),
                    delegate(OrderNoteDetail.GroupRecipientDetail item)
                    {
                        return context.Load<StaffGroup>(item.Group.StaffGroupRef, EntityLoadFlags.Proxy);
                    }));

            StaffGroup onBehalfOf = detail.OnBehalfOfGroup == null ? null :
                context.Load<StaffGroup>(detail.OnBehalfOfGroup.StaffGroupRef, EntityLoadFlags.Proxy);

            // Bug #3717: If an author is supplied in the OrderNoteDetail use it instead.
            if (detail.Author != null)
            {
                author = context.Load<Staff>(detail.Author.StaffRef, EntityLoadFlags.Proxy);
            }

            OrderNote note = new OrderNote(order, detail.Category, author, onBehalfOf, detail.NoteBody, detail.Urgent);

            if (post)
                note.Post(staffRecipients, groupRecipients);

            // Bug #3717: If a post time is supplied in the OrderNoteDetail assume it and the CreationTime are correct.
            if (detail.PostTime.HasValue)
            {
                note.PostTime = detail.PostTime;
                note.CreationTime = detail.CreationTime;
            }

            // bug #3467: since we removed the Notes collection from Order, need to lock this manually
            context.Lock(note, DirtyState.New);

            return note;
        }

        #region Helpers

        private OrderNoteDetail.GroupRecipientDetail CreateGroupRecipientDetail(StaffGroup group, bool acknowledged,
            NoteAcknowledgement acknowledgement, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler staffGroupAssembler = new StaffGroupAssembler();
            return new OrderNoteDetail.GroupRecipientDetail(
                                staffGroupAssembler.CreateSummary(group),
                                acknowledged,
                                acknowledged ? acknowledgement.Time : null,
                                acknowledged ? staffAssembler.CreateStaffSummary(acknowledgement.Staff, context) : null);
        }

        private OrderNoteDetail.StaffRecipientDetail CreateStaffRecipientDetail(Staff staff, bool acknowledged,
            NoteAcknowledgement acknowledgement, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteDetail.StaffRecipientDetail(
                                staffAssembler.CreateStaffSummary(staff, context),
                                acknowledged,
                                acknowledged ? acknowledgement.Time : null);
        }

        #endregion

    }
}
