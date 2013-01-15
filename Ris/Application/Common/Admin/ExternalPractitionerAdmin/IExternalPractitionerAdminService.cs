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

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	/// <summary>
	/// Provides operations to administer staffs
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	public interface IExternalPractitionerAdminService
	{
		/// <summary>
		/// Returns a list of practitioners based on a textual query.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		TextQueryResponse<ExternalPractitionerSummary> TextQuery(TextQueryRequest request);

		/// <summary>
		/// Summary list of all practitioners
		/// </summary>
		[OperationContract]
		ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request);

		/// <summary>
		/// Add a new practitioner.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request);

		/// <summary>
		/// Update a new practitioner.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request);

		/// <summary>
		/// Delete an existing external practitioners.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteExternalPractitionerResponse DeleteExternalPractitioner(DeleteExternalPractitionerRequest request);

		/// <summary>
		/// Load details for a specified practitioner for editing.
		/// </summary>
		[OperationContract]
		LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request);

		/// <summary>
		/// Loads all form data needed to edit a practitioner.
		/// </summary>
		[OperationContract]
		LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request);

		/// <summary>
		/// Merge duplicate external practitioners contact points.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		MergeDuplicateContactPointResponse MergeDuplicateContactPoint(MergeDuplicateContactPointRequest request);

		/// <summary>
		/// Loads all form data needed to merge two contact points.
		/// </summary>
		[OperationContract]
		LoadMergeDuplicateContactPointFormDataResponse LoadMergeDuplicateContactPointFormData(LoadMergeDuplicateContactPointFormDataRequest request);

		/// <summary>
		/// Merge two external practitioners.
		/// </summary>
		[OperationContract]
		MergeExternalPractitionerResponse MergeExternalPractitioner(MergeExternalPractitionerRequest request);

		/// <summary>
		/// Load all form data needed to merge two external practitioners.
		/// </summary>
		[OperationContract]
		LoadMergeExternalPractitionerFormDataResponse LoadMergeExternalPractitionerFormData(LoadMergeExternalPractitionerFormDataRequest request);
	}
}
