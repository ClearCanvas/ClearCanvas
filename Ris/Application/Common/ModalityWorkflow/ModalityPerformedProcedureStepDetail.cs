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

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
	[DataContract]
	public class ModalityPerformedProcedureStepDetail : DataContractBase
	{
		public ModalityPerformedProcedureStepDetail(EntityRef modalityPerformendProcedureStepRef, EnumValueInfo state, DateTime startTime, DateTime? endTime, StaffSummary performer, List<ModalityProcedureStepSummary> modalityProcedureSteps, List<DicomSeriesDetail> dicomSeries, Dictionary<string, string> extendedProperties)
		{
			this.ModalityPerformendProcedureStepRef = modalityPerformendProcedureStepRef;
			this.State = state;
			this.StartTime = startTime;
			this.EndTime = endTime;
			this.Performer = performer;
			this.ModalityProcedureSteps = modalityProcedureSteps;
			this.DicomSeries = dicomSeries;
			this.ExtendedProperties = extendedProperties;
		}

		[DataMember]
		public EntityRef ModalityPerformendProcedureStepRef;

		[DataMember]
		public EnumValueInfo State;

		[DataMember]
		public DateTime StartTime;

		[DataMember]
		public DateTime? EndTime;

		[DataMember]
		public StaffSummary Performer;

		/// <summary>
		/// Modality procedure steps that were performed with this performed procedure step.
		/// </summary>
		[DataMember]
		public List<ModalityProcedureStepSummary> ModalityProcedureSteps;

		[DataMember]
		public List<DicomSeriesDetail> DicomSeries;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;
	}
}