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
    public class PatientNoteCategorySummary : DataContractBase, ICloneable, IEquatable<PatientNoteCategorySummary>
    {
        public PatientNoteCategorySummary(EntityRef noteCategoryRef, string name, string description, EnumValueInfo severity, bool deactivated)
        {
            this.NoteCategoryRef = noteCategoryRef;
            this.Name = name;
            this.Description = description;
            this.Severity = severity;
        	this.Deactivated = deactivated;
        }

        public PatientNoteCategorySummary()
        {
        }

        [DataMember]
        public EntityRef NoteCategoryRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Severity;

		[DataMember]
		public bool Deactivated;


        #region ICloneable Members

        public object Clone()
        {
			return new PatientNoteCategorySummary(this.NoteCategoryRef, this.Name, this.Description, this.Severity, this.Deactivated);
        }

        #endregion

        public bool Equals(PatientNoteCategorySummary patientNoteCategorySummary)
        {
            if (patientNoteCategorySummary == null) return false;
            return Equals(NoteCategoryRef, patientNoteCategorySummary.NoteCategoryRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as PatientNoteCategorySummary);
        }

        public override int GetHashCode()
        {
            return NoteCategoryRef.GetHashCode();
        }
    }
}
