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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class DiagnosticServiceSummary : DataContractBase, IEquatable<DiagnosticServiceSummary>
    {
        public DiagnosticServiceSummary(EntityRef diagnosticServiceRef, string id, string name, bool deactivated)
        {
            this.DiagnosticServiceRef = diagnosticServiceRef;
            this.Id = id;
            this.Name = name;
        	this.Deactivated = deactivated;
        }

		public DiagnosticServiceSummary()
		{
		}

        [DataMember]
        public EntityRef DiagnosticServiceRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

		[DataMember]
		public bool Deactivated;

        public bool Equals(DiagnosticServiceSummary diagnosticServiceSummary)
        {
            if (diagnosticServiceSummary == null) return false;
            return Equals(DiagnosticServiceRef, diagnosticServiceSummary.DiagnosticServiceRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as DiagnosticServiceSummary);
        }

        public override int GetHashCode()
        {
            return DiagnosticServiceRef.GetHashCode();
        }
    }
}
