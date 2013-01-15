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

using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.DepartmentAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IDepartmentAdminService))]
	public class DepartmentAdminService : ApplicationServiceBase, IDepartmentAdminService
	{
		#region IDepartmentAdminService Members

		[ReadOperation]
		public ListDepartmentsResponse ListDepartments(ListDepartmentsRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			var where = new DepartmentSearchCriteria();
			where.Id.SortAsc(0);

			var broker = PersistenceContext.GetBroker<IDepartmentBroker>();
			var items = broker.Find(where, request.Page);

			var assembler = new DepartmentAssembler();
			return new ListDepartmentsResponse(
				CollectionUtils.Map(items,(Department item) => assembler.CreateSummary(item, PersistenceContext))
				);
		}

		[ReadOperation]
		public LoadDepartmentForEditResponse LoadDepartmentForEdit(LoadDepartmentForEditRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DepartmentRef, "request.DepartmentRef");

			var item = PersistenceContext.Load<Department>(request.DepartmentRef);

			var assembler = new DepartmentAssembler();
			return new LoadDepartmentForEditResponse(assembler.CreateDetail(item, PersistenceContext));
		}

		[ReadOperation]
		public LoadDepartmentEditorFormDataResponse LoadDepartmentEditorFormData(LoadDepartmentEditorFormDataRequest request)
		{
			var facilityAssembler = new FacilityAssembler();
			var facilities = PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false);
			return new LoadDepartmentEditorFormDataResponse(
				CollectionUtils.Map(facilities, (Facility f) => facilityAssembler.CreateFacilitySummary(f)));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Department)]
		public AddDepartmentResponse AddDepartment(AddDepartmentRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Department, "request.Department");

			var item = new Department();

			var assembler = new DepartmentAssembler();
			assembler.UpdateDepartment(item, request.Department, PersistenceContext);

			PersistenceContext.Lock(item, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddDepartmentResponse(assembler.CreateSummary(item, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Department)]
		public UpdateDepartmentResponse UpdateDepartment(UpdateDepartmentRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Department, "request.Department");
			Platform.CheckMemberIsSet(request.Department.DepartmentRef, "request.Department.DepartmentRef");

			var item = PersistenceContext.Load<Department>(request.Department.DepartmentRef);

			var assembler = new DepartmentAssembler();
			assembler.UpdateDepartment(item, request.Department, PersistenceContext);

			PersistenceContext.SynchState();

			return new UpdateDepartmentResponse(assembler.CreateSummary(item, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Department)]
		public DeleteDepartmentResponse DeleteDepartment(DeleteDepartmentRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IDepartmentBroker>();
				var item = broker.Load(request.DepartmentRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteDepartmentResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Department))));
			}
		}

		#endregion
	}
}
