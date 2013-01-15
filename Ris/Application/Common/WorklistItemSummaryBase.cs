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
	public class WorklistItemSummaryBase : DataContractBase, IVersionedEquatable<WorklistItemSummaryBase>
	{
		public WorklistItemSummaryBase(
			EntityRef procedureStepRef,
			EntityRef procedureRef,
			EntityRef orderRef,
			EntityRef patientRef,
			EntityRef profileRef,
			CompositeIdentifierDetail mrn,
			PersonNameDetail name,
			string accessionNumber,
			EnumValueInfo orderPriority,
			EnumValueInfo patientClass,
			string diagnosticServiceName,
			string procedureName,
			bool procedurePortable,
			EnumValueInfo procedureLaterality,
			string procedureStepName,
			DateTime? time)
		{
			this.ProcedureStepRef = procedureStepRef;
			this.ProcedureRef = procedureRef;
			this.OrderRef = orderRef;
			this.PatientRef = patientRef;
			this.PatientProfileRef = profileRef;
			this.Mrn = mrn;
			this.PatientName = name;
			this.AccessionNumber = accessionNumber;
			this.OrderPriority = orderPriority;
			this.PatientClass = patientClass;
			this.DiagnosticServiceName = diagnosticServiceName;
			this.ProcedureName = procedureName;
			this.ProcedurePortable = procedurePortable;
			this.ProcedureLaterality = procedureLaterality;
			this.ProcedureStepName = procedureStepName;
			this.Time = time;
		}

		public WorklistItemSummaryBase()
		{
		}

		[DataMember]
		public EntityRef ProcedureStepRef;

		[DataMember]
		public EntityRef ProcedureRef;

		[DataMember]
		public EntityRef OrderRef;

		[DataMember]
		public EntityRef PatientRef;

		[DataMember]
		public EntityRef PatientProfileRef;

		[DataMember]
		public CompositeIdentifierDetail Mrn;

		[DataMember]
		public PersonNameDetail PatientName;

		[DataMember]
		public string AccessionNumber;

		[DataMember]
		public EnumValueInfo OrderPriority;

		[DataMember]
		public EnumValueInfo PatientClass;

		[DataMember]
		public string DiagnosticServiceName;

		[DataMember]
		public string ProcedureName;

		[DataMember]
		public bool ProcedurePortable;

		[DataMember]
		public EnumValueInfo ProcedureLaterality;

		[DataMember]
		public string ProcedureStepName;

		[DataMember]
		public DateTime? Time;

		/// <summary>
		/// Implements equality based on all entity-refs, and is version-sensitive.
		/// </summary>
		/// <param name="worklistItemSummaryBase"></param>
		/// <returns></returns>
		public bool Equals(WorklistItemSummaryBase worklistItemSummaryBase)
		{
			return Equals(worklistItemSummaryBase, false);
		}

		/// <summary>
		/// Overridden to provide equality based on all entity-refs.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as WorklistItemSummaryBase);
		}

		/// <summary>
		/// Overridden to provide hash-code based on all entity refs.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			var result = ProcedureStepRef != null ? ProcedureStepRef.GetHashCode() : 0;
			result = 29 * result + (ProcedureRef != null ? ProcedureRef.GetHashCode() : 0);
			result = 29 * result + (OrderRef != null ? OrderRef.GetHashCode() : 0);
			result = 29 * result + PatientRef.GetHashCode();
			result = 29 * result + PatientProfileRef.GetHashCode();
			return result;
		}

		#region IVersionedEquatable<WorklistItemSummaryBase> Members

		public bool Equals(WorklistItemSummaryBase other, bool ignoreVersion)
		{
			if (other == null) return false;
			if (!EntityRef.Equals(ProcedureStepRef, other.ProcedureStepRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(ProcedureRef, other.ProcedureRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(OrderRef, other.OrderRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(PatientRef, other.PatientRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(PatientProfileRef, other.PatientProfileRef, ignoreVersion)) return false;
			return true;
		}

		#endregion

		#region IVersionedEquatable Members

		public bool Equals(object other, bool ignoreVersion)
		{
			return Equals(other as WorklistItemSummaryBase, ignoreVersion);
		}

		#endregion
	}
}
