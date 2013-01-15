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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[RisApplicationService]
	[ServiceContract]
	[ServiceKnownType(typeof(ReportingWorklistItemSummary))]
	public interface ITranscriptionWorkflowService : IWorklistService<ReportingWorklistItemSummary>, IWorkflowService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		GetRejectReasonChoicesResponse GetRejectReasonChoices(GetRejectReasonChoicesRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		StartTranscriptionResponse StartTranscription(StartTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DiscardTranscriptionResponse DiscardTranscription(DiscardTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		SaveTranscriptionResponse SaveTranscription(SaveTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		SubmitTranscriptionForReviewResponse SubmitTranscriptionForReview(SubmitTranscriptionForReviewRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		CompleteTranscriptionResponse CompleteTranscription(CompleteTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		RejectTranscriptionResponse RejectTranscription(RejectTranscriptionRequest request);

		/// <summary>
		/// Load the report of a given reporting step
		/// </summary>
		/// <param name="request"><see cref="LoadTranscriptionForEditRequest"/></param>
		/// <returns><see cref="LoadTranscriptionForEditResponse"/></returns>
		[OperationContract]
		LoadTranscriptionForEditResponse LoadTranscriptionForEdit(LoadTranscriptionForEditRequest request);
	}
}
