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
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PatientNoteDetail : DataContractBase, ICloneable
    {
        public PatientNoteDetail(
			EntityRef patientNoteRef,
			string comment, 
            PatientNoteCategorySummary category, 
            StaffSummary createdBy, 
            DateTime? creationTime,
            DateTime? validRangeFrom,
            DateTime? validRangeUntil,
            bool isExpired)
        {
        	this.PatientNoteRef = patientNoteRef;
            this.Comment = comment;
            this.Category = category;
            this.Author = createdBy;
            this.CreationTime = creationTime;
            this.ValidRangeFrom = validRangeFrom;
            this.ValidRangeUntil = validRangeUntil;
            this.IsExpired = isExpired;
        }

        public PatientNoteDetail()
        {
        }

		[DataMember]
    	public EntityRef PatientNoteRef;

        [DataMember]
        public string Comment;

        [DataMember]
        public PatientNoteCategorySummary Category;

        [DataMember]
        public StaffSummary Author;

        [DataMember]
        public DateTime? CreationTime;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;

        [DataMember]
        public bool IsExpired;

        #region ICloneable Members

        public object Clone()
        {
            return new PatientNoteDetail(this.PatientNoteRef,this.Comment, this.Category, this.Author, this.CreationTime,
				this.ValidRangeFrom, this.ValidRangeUntil, this.IsExpired);
        }

        #endregion    
    }
}
