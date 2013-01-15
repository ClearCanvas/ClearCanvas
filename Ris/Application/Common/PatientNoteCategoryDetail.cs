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
    public class PatientNoteCategoryDetail : DataContractBase, ICloneable
    {
        public PatientNoteCategoryDetail(EntityRef categoryRef, string category, string description, EnumValueInfo severity, bool deactivated)
        {
        	this.NoteCategoryRef = categoryRef;
            this.Category = category;
            this.Description = description;
            this.Severity = severity;
        	this.Deactivated = deactivated;
        }

        public PatientNoteCategoryDetail()
        {
        }

		[DataMember]
		public EntityRef NoteCategoryRef;

		[DataMember]
        public string Category;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Severity;

		[DataMember]
		public bool Deactivated;

        #region ICloneable Members

        public object Clone()
        {
        	return new PatientNoteCategoryDetail(this.NoteCategoryRef, this.Category, this.Description, this.Severity, this.Deactivated);
        }

        #endregion
    }
}
