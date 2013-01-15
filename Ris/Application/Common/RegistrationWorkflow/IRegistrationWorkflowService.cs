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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    /// <summary>
    /// Provides registration workflow related operations, including retrieving registration worklist, worklist preview, 
    /// patient search, cancel orders and check-in patients
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
	[ServiceKnownType(typeof(RegistrationWorklistItemSummary))]
	public interface IRegistrationWorkflowService : IWorklistService<RegistrationWorklistItemSummary>, IWorkflowService
    {

        /// <summary>
        /// Returns a list of patient profiles based on a textual query.
        /// </summary>
        /// <param name="request"><see cref="TextQueryRequest"/></param>
        /// <returns></returns>
        [OperationContract]
        TextQueryResponse<PatientProfileSummary> PatientProfileTextQuery(TextQueryRequest request);

        /// <summary>
        /// Get procedures that can be checked-in for a patient
        /// </summary>
        /// <param name="request"><see cref="ListProceduresForCheckInRequest"/></param>
        /// <returns><see cref="ListProceduresForCheckInResponse"/></returns>
        [OperationContract]
        ListProceduresForCheckInResponse ListProceduresForCheckIn(ListProceduresForCheckInRequest request);

        /// <summary>
        /// Check in procedures for a patient
        /// </summary>
        /// <param name="request"><see cref="CheckInProcedureRequest"/></param>
        /// <returns><see cref="CheckInProcedureResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request);
    }
}
