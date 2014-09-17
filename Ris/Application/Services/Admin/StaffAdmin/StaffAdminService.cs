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

using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IStaffAdminService))]
	public class StaffAdminService : ApplicationServiceBase, IStaffAdminService
	{
		#region IStaffAdminService Members

		[ReadOperation]
		// note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin
		// because it is used in non-admin situations - perhaps we need to create a separate operation???
		public ListStaffResponse ListStaff(ListStaffRequest request)
		{

			var assembler = new StaffAssembler();

			var criteria = new StaffSearchCriteria();
			criteria.Name.FamilyName.SortAsc(0);

            if (!string.IsNullOrEmpty(request.StaffID))
				ApplyCondition(criteria.Id, request.StaffID, request.ExactMatch);

            if (!string.IsNullOrEmpty(request.GivenName))
				ApplyCondition(criteria.Name.GivenName, request.GivenName, request.ExactMatch); 

			if (!string.IsNullOrEmpty(request.FamilyName))
				ApplyCondition(criteria.Name.FamilyName, request.FamilyName, request.ExactMatch);

			if (!string.IsNullOrEmpty(request.UserName))
				criteria.UserName.EqualTo(request.UserName);

            if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

			ApplyStaffTypesFilter(request.StaffTypesFilter, new [] { criteria });

			return new ListStaffResponse(
				CollectionUtils.Map<Staff, StaffSummary, List<StaffSummary>>(
					PersistenceContext.GetBroker<IStaffBroker>().Find(criteria, request.Page),
					s => assembler.CreateStaffSummary(s, PersistenceContext)));
		}

		private static void ApplyCondition(ISearchCondition<string> condition, string value, bool exactMatch)
		{
			if(exactMatch)
				condition.EqualTo(value);
			else
				condition.StartsWith(value);
		}

		[ReadOperation]
		public LoadStaffForEditResponse LoadStaffForEdit(LoadStaffForEditRequest request)
		{
			var s = PersistenceContext.Load<Staff>(request.StaffRef);

			// ensure user has access to edit this staff
			CheckReadAccess(s);

			var assembler = new StaffAssembler();
			return new LoadStaffForEditResponse(assembler.CreateStaffDetail(s, this.PersistenceContext));
		}

		[ReadOperation]
		public LoadStaffEditorFormDataResponse LoadStaffEditorFormData(LoadStaffEditorFormDataRequest request)
		{
			var groupAssember = new StaffGroupAssembler();

			return new LoadStaffEditorFormDataResponse(
				EnumUtils.GetEnumValueList<StaffTypeEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<SexEnum>(this.PersistenceContext),
				(new SimplifiedPhoneTypeAssembler()).GetPatientPhoneTypeChoices(),
				EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext),
				CollectionUtils.Map(PersistenceContext.GetBroker<IStaffGroupBroker>().FindAll(false),
				                    (StaffGroup group) => groupAssember.CreateSummary(group))
				);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Staff)]
		public AddStaffResponse AddStaff(AddStaffRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.StaffDetail, "StaffDetail");

			// if trying to associate with a user account, check the account is free
			if(!string.IsNullOrEmpty(request.StaffDetail.UserName))
			{
				ValidateUserNameFree(request.StaffDetail.UserName);
			}

			// create new staff
			var staff = new Staff();

			// set properties from request
			var assembler = new StaffAssembler();

			var groupsEditable = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.StaffGroup);
			assembler.UpdateStaff(request.StaffDetail,
				staff,
				groupsEditable,
				groupsEditable,
				PersistenceContext);

			PersistenceContext.Lock(staff, DirtyState.New);

			// ensure the new staff is assigned an OID before using it in the return value
			PersistenceContext.SynchState();

			return new AddStaffResponse(assembler.CreateStaffSummary(staff, PersistenceContext));
		}

		[UpdateOperation]
		public UpdateStaffResponse UpdateStaff(UpdateStaffRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.StaffDetail, "StaffDetail");

			var staff = PersistenceContext.Load<Staff>(request.StaffDetail.StaffRef);

			// ensure user has access to edit this staff
			CheckWriteAccess(staff);

			// if trying to associate with a new user account, check the account is free
			if (!string.IsNullOrEmpty(request.StaffDetail.UserName) && request.StaffDetail.UserName != staff.UserName)
			{
				ValidateUserNameFree(request.StaffDetail.UserName);
			}

			var assembler = new StaffAssembler();
			assembler.UpdateStaff(request.StaffDetail,
				staff,
				request.UpdateElectiveGroups && (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.StaffGroup) || staff.UserName == this.CurrentUser),
				request.UpdateNonElectiveGroups && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.StaffGroup),
				PersistenceContext);

			return new UpdateStaffResponse(assembler.CreateStaffSummary(staff, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Staff)]
		public DeleteStaffResponse DeleteStaff(DeleteStaffRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IStaffBroker>();
				var item = broker.Load(request.StaffRef, EntityLoadFlags.Proxy);

				//bug #3324: because StaffGroup owns the collection, need to iterate over each group
				//and manually remove this staff
				var groups = new List<StaffGroup>(item.Groups);
				foreach (var group in groups)
				{
					group.RemoveMember(item);
				}

				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteStaffResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Staff))));
			}
		}

		[ReadOperation]
		public TextQueryResponse<StaffSummary> TextQuery(StaffTextQueryRequest request)
		{
			var broker = PersistenceContext.GetBroker<IStaffBroker>();
			var assembler = new StaffAssembler();

			var helper = new TextQueryHelper<Staff, StaffSearchCriteria, StaffSummary>(
                    delegate
					{
                        var rawQuery = request.TextQuery;

						// this will hold all criteria
						var criteria = new List<StaffSearchCriteria>();

						// build criteria against names
						var names = TextQueryHelper.ParsePersonNames(rawQuery);
						criteria.AddRange(CollectionUtils.Map(names,
							(PersonName n) =>
							{
								var sc = new StaffSearchCriteria();
								sc.Name.FamilyName.StartsWith(n.FamilyName);
								if (n.GivenName != null)
									sc.Name.GivenName.StartsWith(n.GivenName);
								return sc;
							}));

						// build criteria against identifiers
						var ids = TextQueryHelper.ParseIdentifiers(rawQuery);
						criteria.AddRange(CollectionUtils.Map(ids,
									 (string word) =>
									 {
										 var c = new StaffSearchCriteria();
										 c.Id.StartsWith(word);
										 return c;
									 }));


						ApplyStaffTypesFilter(request.StaffTypesFilter, criteria);

						return criteria.ToArray();
					},
                    staff => assembler.CreateStaffSummary(staff, PersistenceContext),
                    (criteria, threshold) => broker.Count(criteria) <= threshold,
					broker.Find);

			return helper.Query(request);
		}

		#endregion

		/// <summary>
		/// Throws an exception if the current user does not have access to edit specified staff.
		/// </summary>
		/// <param name="staff"></param>
		private void CheckReadAccess(Staff staff)
		{
			// users with Admin.Data.Staff token can access any staff
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.Staff))
				return;

			// users can access their own staff profile
			if (staff.UserName == this.CurrentUser)
				return;

			throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}

		/// <summary>
		/// Throws an exception if the current user does not have access to edit specified staff.
		/// </summary>
		/// <param name="staff"></param>
		private void CheckWriteAccess(Staff staff)
		{
			// users with Admin.Data.Staff token can access any staff
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.Staff))
				return;

			// users can update their own staff profile with the Update token
			if (staff.UserName == this.CurrentUser && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.StaffProfile.Update))
				return;

			throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}

		/// <summary>
		/// Applies the specified staff types filter to the specified set of criteria objects.
		/// </summary>
		/// <param name="staffTypesFilter"></param>
		/// <param name="criteria"></param>
		private void ApplyStaffTypesFilter(IEnumerable<string> staffTypesFilter, IEnumerable<StaffSearchCriteria> criteria)
		{
			var broker = PersistenceContext.GetBroker<IEnumBroker>();
			if (staffTypesFilter != null)
			{
				// parse strings into StaffType 
				var typeFilters = CollectionUtils.Map(staffTypesFilter, (string t) => broker.Find<StaffTypeEnum>(t));

				if (typeFilters.Count > 0)
				{
					// apply type filter to each criteria object
					foreach (var criterion in criteria)
					{
						criterion.Type.In(typeFilters);
					}
				}
			}
		}

		private void ValidateUserNameFree(string userName)
		{
			var staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
			try
			{
				var where = new StaffSearchCriteria();
				where.UserName.EqualTo(userName);
				var existing = staffBroker.FindOne(where);
				if (existing != null)
					throw new RequestValidationException(
						string.Format(SR.InvalidRequest_UserAlreadyAssociatedWithStaff,
							userName, existing.Name));
			}
			catch (EntityNotFoundException)
			{
				// no previously associated staff
			}
		}
	}
}
