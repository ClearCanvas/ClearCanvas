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

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin
{
	/// <summary>
	/// Provides operations to administer ProcedureType entities.
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	public interface IProcedureTypeAdminService
	{
		/// <summary>
		/// Returns a list of procedure type based on a textual query.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		TextQueryResponse<ProcedureTypeSummary> TextQuery(TextQueryRequest request);

		/// <summary>
		/// Summary list of all items.
		/// </summary>
		[OperationContract]
		ListProcedureTypesResponse ListProcedureTypes(ListProcedureTypesRequest request);

		/// <summary>
		/// Loads details of specified item for editing.
		/// </summary>
		[OperationContract]
		LoadProcedureTypeForEditResponse LoadProcedureTypeForEdit(LoadProcedureTypeForEditRequest request);

		/// <summary>
		/// Loads all form data needed to edit an item.
		/// </summary>
		[OperationContract]
		LoadProcedureTypeEditorFormDataResponse LoadProcedureTypeEditorFormData(LoadProcedureTypeEditorFormDataRequest request);

		/// <summary>
		/// Adds a new item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddProcedureTypeResponse AddProcedureType(AddProcedureTypeRequest request);

		/// <summary>
		/// Updates an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateProcedureTypeResponse UpdateProcedureType(UpdateProcedureTypeRequest request);

		/// <summary>
		/// Deletes an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteProcedureTypeResponse DeleteProcedureType(DeleteProcedureTypeRequest request);
	}
}
