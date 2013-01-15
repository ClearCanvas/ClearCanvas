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

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderNoteSummary : DataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderNoteRef"></param>
        /// <param name="category"></param>
        /// <param name="creationTime"></param>
        /// <param name="postTime"></param>
        /// <param name="author"></param>
        /// <param name="onBehalfOfGroup"></param>
        /// <param name="isAcknowledged"></param>
		/// <param name="urgent"></param>
		/// <param name="noteBody"></param>
        public OrderNoteSummary(EntityRef orderNoteRef, string category, DateTime? creationTime, DateTime? postTime,
			StaffSummary author, StaffGroupSummary onBehalfOfGroup, bool isAcknowledged, bool urgent, string noteBody)
        {
            OrderNoteRef = orderNoteRef;
            Category = category;
            CreationTime = creationTime;
            PostTime = postTime;
            Author = author;
        	OnBehalfOfGroup = onBehalfOfGroup;
            IsAcknowledged = isAcknowledged;
        	Urgent = urgent;
            NoteBody = noteBody;
        }

        [DataMember]
        public EntityRef OrderNoteRef;

        [DataMember]
        public string Category;

        [DataMember]
        public DateTime? CreationTime;

        [DataMember]
        public DateTime? PostTime;

        [DataMember]
        public StaffSummary Author;

		[DataMember]
		public StaffGroupSummary OnBehalfOfGroup;

        /// <summary>
        /// Gets a value indicating whether the note has been acknowledged by all recipients.
        /// </summary>
        [DataMember]
        public bool IsAcknowledged;

        [DataMember]
        public bool Urgent;

        [DataMember]
        public string NoteBody;
    }
}
