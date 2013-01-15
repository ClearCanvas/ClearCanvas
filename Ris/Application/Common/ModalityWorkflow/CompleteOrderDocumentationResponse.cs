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
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class CompleteOrderDocumentationResponse : DataContractBase
    {

		/// <summary>
		/// Returns the updated procedure plan.
		/// </summary>
        [DataMember]
        public ProcedurePlanDetail ProcedurePlan;

		/// <summary>
		/// Returns the set of interpretation steps that are now scheduled for the procedures in this order.
		/// </summary>
		//JR: this was added for the benefit of the Oto workflow service, that needs to know how to proceed in the workflow.
		[DataMember]
    	public List<EntityRef> InterpretationStepRefs;
    }
}