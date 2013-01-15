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

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
    [RisApplicationService]
    [ServiceContract]
    public interface ICannedTextService
    {
		/// <summary>
		/// List all the canned text subscribe by the current user.
		/// </summary>
		/// <param name="request"><see cref="ListCannedTextForUserRequest"/></param>
		/// <returns><see cref="ListCannedTextForUserResponse"/></returns>
		[OperationContract]
		ListCannedTextForUserResponse ListCannedTextForUser(ListCannedTextForUserRequest request);

		/// <summary>
		/// Loads all form data needed to edit a canned text.
		/// </summary>
		/// <param name="request"><see cref="GetCannedTextEditFormDataRequest"/></param>
		/// <returns><see cref="GetCannedTextEditFormDataResponse"/></returns>
		[OperationContract]
		GetCannedTextEditFormDataResponse GetCannedTextEditFormData(GetCannedTextEditFormDataRequest request);

		/// <summary>
		/// Load details for a specified canned text.
		/// </summary>
		/// <param name="request"><see cref="LoadCannedTextForEditRequest"/></param>
		/// <returns><see cref="LoadCannedTextForEditResponse"/></returns>
		[OperationContract]
		LoadCannedTextForEditResponse LoadCannedTextForEdit(LoadCannedTextForEditRequest request);

		/// <summary>
		/// Adds a new canned text.
		/// </summary>
		/// <param name="request"><see cref="AddCannedTextRequest"/></param>
		/// <returns><see cref="AddCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddCannedTextResponse AddCannedText(AddCannedTextRequest request);

		/// <summary>
		/// Updates an existing canned text.
		/// </summary>
		/// <param name="request"><see cref="UpdateCannedTextRequest"/></param>
		/// <returns><see cref="UpdateCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request);

		/// <summary>
		/// Deletes an existing canned text.
		/// </summary>
		/// <param name="request"><see cref="DeleteCannedTextRequest"/></param>
		/// <returns><see cref="DeleteCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request);

		/// <summary>
		/// Modifies the category of a set of existing canned texts.
		/// </summary>
		/// <param name="request"><see cref="EditCannedTextCategoriesRequest"/></param>
		/// <returns><see cref="EditCannedTextCategoriesResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		EditCannedTextCategoriesResponse EditCannedTextCategories(EditCannedTextCategoriesRequest request);
	}
}
