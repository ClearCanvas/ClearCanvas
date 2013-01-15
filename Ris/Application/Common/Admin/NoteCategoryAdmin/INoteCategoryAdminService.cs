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

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    /// <summary>
    /// Provides operations to administer note categories
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface INoteCategoryAdminService
    {
        /// <summary>
        /// Summary list of all note categories
        /// </summary>
        /// <param name="request"><see cref="ListAllNoteCategoriesRequest"/></param>
        /// <returns><see cref="ListAllNoteCategoriesResponse"/></returns>
        [OperationContract]
        ListAllNoteCategoriesResponse ListAllNoteCategories(ListAllNoteCategoriesRequest request);

        /// <summary>
        /// Add a new note category.  A note category with the same name as an existing note category cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddNoteCategoryRequest"/></param>
        /// <returns><see cref="AddNoteCategoryResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddNoteCategoryResponse AddNoteCategory(AddNoteCategoryRequest request);

        /// <summary>
        /// Update a new note category.  A note category with the same name as an existing note category cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateNoteCategoryRequest"/></param>
        /// <returns><see cref="UpdateNoteCategoryResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateNoteCategoryResponse UpdateNoteCategory(UpdateNoteCategoryRequest request);

		/// <summary>
		/// Delete a note category.
		/// </summary>
		/// <param name="request"><see cref="DeleteNoteCategoryRequest "/></param>
		/// <returns><see cref="DeleteNoteCategoryResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteNoteCategoryResponse DeleteNoteCategory(DeleteNoteCategoryRequest request);
		
		/// <summary>
        /// Loads all form data needed to edit a note category
        /// </summary>
        /// <param name="request"><see cref="GetNoteCategoryEditFormDataRequest"/></param>
        /// <returns><see cref="GetNoteCategoryEditFormDataResponse"/></returns>
        [OperationContract]
        GetNoteCategoryEditFormDataResponse GetNoteCategoryEditFormData(GetNoteCategoryEditFormDataRequest request);

        /// <summary>
        /// Load details for a note category
        /// </summary>
        /// <param name="request"><see cref="LoadNoteCategoryForEditRequest"/></param>
        /// <returns><see cref="LoadNoteCategoryForEditResponse"/></returns>
        [OperationContract]
        LoadNoteCategoryForEditResponse LoadNoteCategoryForEdit(LoadNoteCategoryForEditRequest request);
    }
}
