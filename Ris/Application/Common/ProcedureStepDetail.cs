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
    public class ProcedureStepDetail : DataContractBase
    {
        public ProcedureStepDetail(
            EntityRef procedureStepRef,
            string procedureStepName,
            string stepClassName,
			string description,
            EnumValueInfo state,
			DateTime? creationTime,
            DateTime? scheduledStartTime,
            DateTime? startTime,
            DateTime? endTime,
            StaffSummary scheduledPerformer,
            StaffSummary performer,
			ModalitySummary modality
            )
        {
            this.ProcedureStepRef = procedureStepRef;
            this.ProcedureStepName = procedureStepName;
            this.StepClassName = stepClassName;
        	this.Description = description;
            this.State = state;
        	this.CreationTime = creationTime;
            this.ScheduledStartTime = scheduledStartTime;
            this.StartTime = startTime;
            this.EndTime = endTime;
			this.Modality = modality;
			this.ScheduledPerformer = scheduledPerformer;
            this.Performer = performer;
        }

		public ProcedureStepDetail()
		{			
		}

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public string ProcedureStepName;

        [DataMember] 
        public string StepClassName;

		[DataMember]
		public string Description;

		[DataMember]
        public EnumValueInfo State;

		[DataMember]
		public DateTime? CreationTime;

        [DataMember]
        public DateTime? ScheduledStartTime;

        [DataMember]
        public DateTime? StartTime;

        [DataMember]
        public DateTime? EndTime;

        [DataMember]
        public StaffSummary ScheduledPerformer;

        [DataMember]
        public StaffSummary Performer;

		/// <summary>
		/// Specifies the modality of a MPS.  This field is null for other types of procedure step.
		/// </summary>
		[DataMember]
		public ModalitySummary Modality;

    }
}