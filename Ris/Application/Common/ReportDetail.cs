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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ReportDetail : DataContractBase
    {
        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public EnumValueInfo ReportStatus;

		/// <summary>
		/// This may not contains all the parts that are in a report.
		/// The cancelled reports may not be included.
		/// </summary>
        [DataMember]
        public List<ReportPartDetail> Parts;

		/// <summary>
		/// Gets the procedures associated with the report.
		/// </summary>
        [DataMember]
        public List<ProcedureDetail> Procedures;

		/// <summary>
		/// Return the report part correspond to the report part index
		/// </summary>
		/// <param name="reportPartIndex">The report part index, not the array index of the list of Parts</param>
		/// <returns></returns>
        public ReportPartDetail GetPart(int reportPartIndex)
        {
			if (this.Parts == null || reportPartIndex < 0)
                return null;

			return CollectionUtils.SelectFirst(this.Parts,
				delegate(ReportPartDetail detail) { return detail.Index.Equals(reportPartIndex); });
        }
    }
}