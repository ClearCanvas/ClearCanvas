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

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
	/// <summary>
	/// Provides operations to administer modaltiies
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	public interface IModalityAdminService
	{
		/// <summary>
		/// Summary list all modalities
		/// </summary>
		/// <param name="request"><see cref="ListAllModalitiesRequest"/></param>
		/// <returns><see cref="ListAllModalitiesResponse"/></returns>
		[OperationContract]
		ListAllModalitiesResponse ListAllModalities(ListAllModalitiesRequest request);

		/// <summary>
		/// Add a new modality.  A modality with the same ID as an existing modality cannnot be added.
		/// </summary>
		/// <param name="request"><see cref="AddModalityRequest"/></param>
		/// <returns><see cref="AddModalityResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddModalityResponse AddModality(AddModalityRequest request);

		/// <summary>
		/// Update a new modality.  A modality with the same ID as an existing modality cannnot be updated.
		/// </summary>
		/// <param name="request"><see cref="UpdateModalityRequest"/></param>
		/// <returns><see cref="UpdateModalityResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateModalityResponse UpdateModality(UpdateModalityRequest request);

		/// <summary>
		/// Delete a modality.
		/// </summary>
		/// <param name="request"><see cref="DeleteModalityRequest"/></param>
		/// <returns><see cref="DeleteModalityResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteModalityResponse DeleteModality(DeleteModalityRequest request);
		
		/// <summary>
		/// Load details for a specified modality
		/// </summary>
		/// <param name="request"><see cref="LoadModalityForEditRequest"/></param>
		/// <returns><see cref="LoadModalityForEditResponse"/></returns>
		[OperationContract]
		LoadModalityForEditResponse LoadModalityForEdit(LoadModalityForEditRequest request);

		/// <summary>
		/// Loads all form data needed to edit an item.
		/// </summary>
		[OperationContract]
		LoadModalityEditorFormDataResponse LoadModalityEditorFormData(LoadModalityEditorFormDataRequest request);
	}
}
