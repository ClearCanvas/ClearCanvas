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
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Extended.Common.OrderNotes
{
    [DataContract]
	public class QueryNoteboxRequest : PagedDataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteboxClass"></param>
        /// <param name="queryCount"></param>
        /// <param name="queryItems"></param>
        public QueryNoteboxRequest(string noteboxClass, bool queryCount, bool queryItems)
        {
            NoteboxClass = noteboxClass;
            QueryCount = queryCount;
            QueryItems = queryItems;
        }

        /// <summary>
        /// Specified the notebox class to query.
        /// </summary>
        [DataMember]
        public string NoteboxClass;

		/// <summary>
		/// Identifies the staff group notebox to query, in the case where <see cref="NoteboxClass"/> 
		/// refers to a group notebox.
		/// </summary>
		[DataMember]
    	public EntityRef StaffGroupRef;

        /// <summary>
        /// Specifies whether to return a count of the total number of items in the notebox.
        /// </summary>
        [DataMember]
        public bool QueryCount;

        /// <summary>
        /// Specifies whether to return the list of items in the notebox.
        /// </summary>
        [DataMember]
        public bool QueryItems;
    }
}
