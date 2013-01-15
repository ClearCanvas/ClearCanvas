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
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	/// <summary>
	/// Provides operations to administer DiagnosticService entities.
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	public interface IDiagnosticServiceAdminService
	{
		/// <summary>
		/// Summary list of all items.
		/// </summary>
		[OperationContract]
		ListDiagnosticServicesResponse ListDiagnosticServices(ListDiagnosticServicesRequest request);

		/// <summary>
		/// Performs text-based query for diagnostic services.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		TextQueryResponse<DiagnosticServiceSummary> TextQuery(TextQueryRequest request);

		/// <summary>
		/// Loads details of specified itemfor editing.
		/// </summary>
		[OperationContract]
		LoadDiagnosticServiceForEditResponse LoadDiagnosticServiceForEdit(LoadDiagnosticServiceForEditRequest request);

		/// <summary>
		/// Loads all form data needed to edit an item.
		/// </summary>
		[OperationContract]
		LoadDiagnosticServiceEditorFormDataResponse LoadDiagnosticServiceEditorFormData(LoadDiagnosticServiceEditorFormDataRequest request);

		/// <summary>
		/// Adds a new item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddDiagnosticServiceResponse AddDiagnosticService(AddDiagnosticServiceRequest request);

		/// <summary>
		/// Updates an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateDiagnosticServiceResponse UpdateDiagnosticService(UpdateDiagnosticServiceRequest request);

		/// <summary>
		/// Deletes an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteDiagnosticServiceResponse DeleteDiagnosticService(DeleteDiagnosticServiceRequest request);
	}
}
