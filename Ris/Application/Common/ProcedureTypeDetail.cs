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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProcedureTypeDetail : DataContractBase
    {
		public ProcedureTypeDetail()
		{
		}

		public ProcedureTypeDetail(
			EntityRef entityRef,
			string id,
			string name,
			ModalitySummary defaultModality,
			int defaultDuration,
			bool deactivated)
        {
            this.ProcedureTypeRef = entityRef;
            this.Id = id;
            this.Name = name;
			this.CustomProcedurePlan = false;
			this.DefaultModality = defaultModality;
			this.DefaultDuration = defaultDuration;
			this.Deactivated = deactivated;
        }

		public ProcedureTypeDetail(
			EntityRef entityRef,
			string id,
			string name,
			ProcedureTypeSummary baseType,
			string planXml,
			int defaultDuration,
			bool deactivated)
		{
			this.ProcedureTypeRef = entityRef;
			this.Id = id;
			this.Name = name;
			this.BaseType = baseType;
			this.CustomProcedurePlan = true;
			this.PlanXml = planXml;
			this.DefaultDuration = defaultDuration;
			this.Deactivated = deactivated;
		}

        [DataMember]
        public EntityRef ProcedureTypeRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

		[DataMember]
		public int DefaultDuration;

		[DataMember]
		public bool Deactivated;

		/// <summary>
		/// Specifies the default modality used by the default procedure plan (assuming <see cref="CustomProcedurePlan"/> is false).
		/// </summary>
		[DataMember]
    	public ModalitySummary DefaultModality;

		/// <summary>
		/// Specifies whether a custom procedure plan is used.
		/// </summary>
		[DataMember]
    	public bool CustomProcedurePlan;

		/// <summary>
		/// Specifies the base type, or null if <see cref="CustomProcedurePlan"/> is false.
		/// </summary>
		[DataMember]
		public ProcedureTypeSummary BaseType;

		/// <summary>
		/// Specifies the custom plan XML, or null if <see cref="CustomProcedurePlan"/> is false.
		/// </summary>
		[DataMember]
		public string PlanXml;


		public ProcedureTypeSummary GetSummary()
        {
            return new ProcedureTypeSummary(this.ProcedureTypeRef, this.Name, this.Id, this.DefaultDuration, this.Deactivated);
        }
    }
}
