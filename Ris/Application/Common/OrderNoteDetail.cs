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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderNoteDetail : DataContractBase
    {
        [DataContract]
        public abstract class RecipientDetail : DataContractBase
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="isAcknowledged"></param>
            /// <param name="acknowledgedTime"></param>
            protected RecipientDetail(bool isAcknowledged, DateTime? acknowledgedTime)
            {
                IsAcknowledged = isAcknowledged;
                AcknowledgedTime = acknowledgedTime;
            }

            /// <summary>
            /// Constructor for deserialization
            /// </summary>
            public RecipientDetail()
            {
            }

            /// <summary>
            /// Gets a value indicating whether the note has been acknowledged by the recipient.
            /// (For a group recipient, indicates whether a member of the group has acknowledged the note on behalf of the group).
            /// This field is ignored when creating a new note.
            /// </summary>
            [DataMember]
            public bool IsAcknowledged;

            /// <summary>
            /// Gets the time that the note was acknowledged, or null if it was not acknowledged.
            /// (For a group recipient, gets the time when a member of the group acknowledged the note on behalf of the group).
            /// This field is ignored when creating a new note.
            /// </summary>
            [DataMember]
            public DateTime? AcknowledgedTime;
        }

        [DataContract]
        public class StaffRecipientDetail : RecipientDetail
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="staff"></param>
            /// <param name="isAcknowledged"></param>
            /// <param name="acknowledgedTime"></param>
            public StaffRecipientDetail(StaffSummary staff, bool isAcknowledged, DateTime? acknowledgedTime)
                : base(isAcknowledged, acknowledgedTime)
            {
                Staff = staff;
            }

            /// <summary>
            /// Constructor for deserialization
            /// </summary>
            public StaffRecipientDetail()
            {
            }

            /// <summary>
            /// Gets the staff recipient.
            /// </summary>
            [DataMember]
            public StaffSummary Staff;
        }

        [DataContract]
        public class GroupRecipientDetail : RecipientDetail
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="isAcknowledged"></param>
            /// <param name="acknowledgedTime"></param>
            /// <param name="group"></param>
            /// <param name="acknowledgedBy"></param>
            public GroupRecipientDetail(StaffGroupSummary group, bool isAcknowledged, DateTime? acknowledgedTime, StaffSummary acknowledgedBy)
                : base(isAcknowledged, acknowledgedTime)
            {
                Group = group;
                AcknowledgedByStaff = acknowledgedBy;
            }

            /// <summary>
            /// Constructor for deserialization
            /// </summary>
            public GroupRecipientDetail()
            {
            }

            /// <summary>
            /// Gets the group recipient.
            /// </summary>
            [DataMember]
            public StaffGroupSummary Group;

            /// <summary>
            /// Gets the staff that acknowledged on behalf of the group, if <see cref="RecipientDetail.IsAcknowledged"/> is true.
            /// </summary>
            [DataMember]
            public StaffSummary AcknowledgedByStaff;
        }

        /// <summary>
        /// Constructor for creating detail for an existing order note.
        /// </summary>
        /// <param name="orderNoteRef"></param>
        /// <param name="category"></param>
        /// <param name="creationTime"></param>
        /// <param name="postTime"></param>
        /// <param name="author"></param>
        /// <param name="onBehalfOfGroup"></param>
        /// <param name="urgent"></param>
        /// <param name="staffRecipients"></param>
        /// <param name="groupRecipients"></param>
        /// <param name="noteBody"></param>
        /// <param name="canAcknowledge"></param>
        public OrderNoteDetail(EntityRef orderNoteRef, string category, DateTime creationTime,
            DateTime? postTime, StaffSummary author, StaffGroupSummary onBehalfOfGroup, bool urgent,
            List<StaffRecipientDetail> staffRecipients,
            List<GroupRecipientDetail> groupRecipients, string noteBody, bool canAcknowledge)
        {
            OrderNoteRef = orderNoteRef;
            Category = category;
            CreationTime = creationTime;
            PostTime = postTime;
            Author = author;
            OnBehalfOfGroup = onBehalfOfGroup;
            Urgent = urgent;
            StaffRecipients = staffRecipients;
            GroupRecipients = groupRecipients;
            NoteBody = noteBody;
            CanAcknowledge = canAcknowledge;
        }

        /// <summary>
        /// Constructor for generating a new order note.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="noteBody"></param>
        /// <param name="onBehalfOfGroup"></param>
        /// <param name="urgent"></param>
        /// <param name="staffRecipients"></param>
        /// <param name="groupRecipients"></param>
        public OrderNoteDetail(string category, string noteBody, StaffGroupSummary onBehalfOfGroup, bool urgent, List<StaffSummary> staffRecipients, List<StaffGroupSummary> groupRecipients)
        {
            Category = category;
            NoteBody = noteBody;
            OnBehalfOfGroup = onBehalfOfGroup;
            Urgent = urgent;

            StaffRecipients = staffRecipients == null ? null :
                CollectionUtils.Map<StaffSummary, StaffRecipientDetail>(staffRecipients,
                    delegate(StaffSummary staff)
                    {
                        return new StaffRecipientDetail(staff, false, null);
                    });


            GroupRecipients = groupRecipients == null ? null :
                CollectionUtils.Map<StaffGroupSummary, GroupRecipientDetail>(groupRecipients,
                    delegate(StaffGroupSummary group)
                    {
                        return new GroupRecipientDetail(group, false, null, null);
                    });
        }

        /// <summary>
        /// Constructor for deserialization
        /// </summary>
        public OrderNoteDetail()
        {
        }

        /// <summary>
        /// Gets a reference to the order note.
        /// This field is ignored when creating a new note.
        /// </summary>
        [DataMember]
        public EntityRef OrderNoteRef;

        /// <summary>
        /// Gets the category of the note.
        /// </summary>
        [DataMember]
        public string Category;

        /// <summary>
        /// Gets the time the note was created.
        /// This field is ignored when creating a new note if <see cref="PostTime"/> is null.
        /// </summary>
        [DataMember]
        public DateTime CreationTime;

        /// <summary>
        /// Gets the time the note was posted.
        /// </summary>
        [DataMember]
        public DateTime? PostTime;

        /// <summary>
        /// Gets the note author.
        /// </summary>
        [DataMember]
        public StaffSummary Author;

        /// <summary>
        /// Gets the staff group on behalf of which the note is posted, or null if it was not posted on behalf of any group.
        /// </summary>
        [DataMember]
        public StaffGroupSummary OnBehalfOfGroup;

        /// <summary>
        /// Gets the note body text.
        /// </summary>
        [DataMember]
        public string NoteBody;

        /// <summary>
        /// Gets a value indicating whether the note is urgent or not.
        /// </summary>
        [DataMember]
        public bool Urgent;

        /// <summary>
        /// Gets the list of staff recipients.
        /// </summary>
        [DataMember]
        public List<StaffRecipientDetail> StaffRecipients;

        /// <summary>
        /// Gets the list of group recipients.
        /// </summary>
        [DataMember]
        public List<GroupRecipientDetail> GroupRecipients;

        /// <summary>
        /// Gets a value indicating whether this note can be acknowledged by the current user.
        /// </summary>
        [DataMember]
        public bool CanAcknowledge;

    }
}
