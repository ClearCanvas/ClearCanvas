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
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
	[ServiceImplementsContract(typeof(IWorklistAdminService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class WorklistAdminService : ApplicationServiceBase, IWorklistAdminService
	{
		private static readonly string[] _interpretationReviewClasses = new []
				                                  	{
				                                  		WorklistClassNames.ReportingToBeReviewedReportWorklist,
				                                  		WorklistClassNames.ReportingAwaitingReviewWorklist,
				                                  		WorklistClassNames.ReportingAssignedReviewWorklist
				                                  	};
		private static readonly string[] _transcriptionReviewClasses = new []
				                                  	{
				                                  		WorklistClassNames.TranscriptionToBeReviewedWorklist,
				                                  		WorklistClassNames.TranscriptionAwaitingReviewWorklist
				                                  	};


		#region IWorklistAdminService Members

		[ReadOperation]
		public ListWorklistCategoriesResponse ListWorklistCategories(ListWorklistCategoriesRequest request)
		{
			var categories = CollectionUtils.Map<Type, string>(
				ListClassesHelper(null, null, true),
				Worklist.GetCategory);

			// in case some worklist classes did not have assigned categories
			CollectionUtils.Remove(categories, (string s) => s == null);

			return new ListWorklistCategoriesResponse(CollectionUtils.Unique(categories));
		}

		[ReadOperation]
		public ListWorklistClassesResponse ListWorklistClasses(ListWorklistClassesRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			var worklistClasses =
				ListClassesHelper(request.ClassNames, request.Categories, request.IncludeStatic);

			var assembler = new WorklistAdminAssembler();
			return new ListWorklistClassesResponse(
				CollectionUtils.Map<Type, WorklistClassSummary>(worklistClasses,
					assembler.CreateClassSummary));
		}

		[ReadOperation]
		public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			var worklistClasses = ListClassesHelper(request.ClassNames, request.Categories, request.IncludeStatic);

			// grab the persistent worklists
			var broker = PersistenceContext.GetBroker<IWorklistBroker>();
			var persistentClassNames = CollectionUtils.Select(worklistClasses, t => !Worklist.GetIsStatic(t))
				.ConvertAll(Worklist.GetClassName);

			var worklists = broker.Find(request.WorklistName, request.IncludeUserDefinedWorklists, persistentClassNames, request.Page);

			// optionally include the static ones
			if (request.IncludeStatic)
			{
				foreach (var worklistClass in worklistClasses)
				{
					if (Worklist.GetIsStatic(worklistClass))
						worklists.Add(WorklistFactory.Instance.CreateWorklist(worklistClass));
				}
			}

			var adminAssembler = new WorklistAdminAssembler();
			return new ListWorklistsResponse(
				CollectionUtils.Map<Worklist, WorklistAdminSummary, List<WorklistAdminSummary>>(
				worklists,
				worklist => adminAssembler.CreateWorklistSummary(worklist, this.PersistenceContext)));
		}

		[ReadOperation]
		public ListProcedureTypeGroupsResponse ListProcedureTypeGroups(ListProcedureTypeGroupsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureTypeGroupClass, "request.ProcedureTypeGroupClass");

			var procedureTypeGroupClass = ProcedureTypeGroup.GetSubClass(request.ProcedureTypeGroupClass, PersistenceContext);
			if (procedureTypeGroupClass == null)
				throw new ArgumentException("Invalid ProcedureTypeGroupClass name");

			var response = new ListProcedureTypeGroupsResponse();

			var assembler = new ProcedureTypeGroupAssembler();
			response.ProcedureTypeGroups =
				CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
					PersistenceContext.GetBroker<IProcedureTypeGroupBroker>().Find(new ProcedureTypeGroupSearchCriteria(), procedureTypeGroupClass),
					group => assembler.GetProcedureTypeGroupSummary(group, PersistenceContext));

			return response;
		}

		[ReadOperation]
		public GetWorklistEditFormDataResponse GetWorklistEditFormData(GetWorklistEditFormDataRequest request)
		{
			var response = new GetWorklistEditFormDataResponse();

			if (request.GetWorklistEditFormChoicesRequest != null)
				response.GetWorklistEditFormChoicesResponse = GetWorklistEditFormChoices(request.GetWorklistEditFormChoicesRequest);

			if (request.GetWorklistEditValidationRequest != null)
				response.GetWorklistEditValidationResponse = GetWorklistEditValidation(request.GetWorklistEditValidationRequest);

			return response;
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public LoadWorklistForEditResponse LoadWorklistForEdit(LoadWorklistForEditRequest request)
		{
			var worklist = PersistenceContext.Load<Worklist>(request.EntityRef);

			var adminAssembler = new WorklistAdminAssembler();
			var adminDetail = adminAssembler.CreateWorklistDetail(worklist, this.PersistenceContext);
			return new LoadWorklistForEditResponse(worklist.GetRef(), adminDetail);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public AddWorklistResponse AddWorklist(AddWorklistRequest request)
		{
			if (string.IsNullOrEmpty(request.Detail.Name))
			{
				throw new RequestValidationException(SR.ExceptionWorklistNameRequired);
			}

			// create instance of worklist owner
			var owner = CreateOwner(request.Detail, request.IsUserWorklist);

			// ensure user has access to create this worklist
			CheckAccess(owner);

			CheckWorklistCountRestriction(owner);

			// create instance of appropriate class
			var worklist = WorklistFactory.Instance.CreateWorklist(request.Detail.WorklistClass.ClassName);

			// set owner
			worklist.Owner = owner;

			// update properties
			UpdateWorklistHelper(request.Detail, worklist);

			PersistenceContext.Lock(worklist, DirtyState.New);
			PersistenceContext.SynchState();

			var adminAssembler = new WorklistAdminAssembler();
			return new AddWorklistResponse(adminAssembler.CreateWorklistSummary(worklist, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public UpdateWorklistResponse UpdateWorklist(UpdateWorklistRequest request)
		{
			var worklist = this.PersistenceContext.Load<Worklist>(request.EntityRef);

			// check if user can update
			CheckAccess(worklist.Owner);

			// update
			UpdateWorklistHelper(request.Detail, worklist);

			var adminAssembler = new WorklistAdminAssembler();
			return new UpdateWorklistResponse(adminAssembler.CreateWorklistSummary(worklist, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public DeleteWorklistResponse DeleteWorklist(DeleteWorklistRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IWorklistBroker>();
				var item = broker.Load(request.WorklistRef, EntityLoadFlags.Proxy);

				// check if user can delete
				CheckAccess(item.Owner);

				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteWorklistResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Worklist))));
			}
		}

		#endregion
		
		private GetWorklistEditFormChoicesResponse GetWorklistEditFormChoices(GetWorklistEditFormChoicesRequest request)
		{
			var response = new GetWorklistEditFormChoicesResponse();

			var assembler = new WorklistAdminAssembler();
			response.WorklistClasses = CollectionUtils.Map<Type, WorklistClassSummary>(
				ListClassesHelper(null, null, false),
				assembler.CreateClassSummary);

			var staffAssembler = new StaffAssembler();
			response.StaffChoices = CollectionUtils.Map<Staff, StaffSummary>(
				this.PersistenceContext.GetBroker<IStaffBroker>().FindAll(false),
				item => staffAssembler.CreateStaffSummary(item, PersistenceContext));

			var staffGroupAssembler = new StaffGroupAssembler();
			response.GroupSubscriberChoices = CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
				this.PersistenceContext.GetBroker<IStaffGroupBroker>().FindAll(false),
				staffGroupAssembler.CreateSummary);

			var facilityAssembler = new FacilityAssembler();
			response.FacilityChoices = CollectionUtils.Map<Facility, FacilitySummary>(
				this.PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
				facilityAssembler.CreateFacilitySummary);

			var departmentAssembler = new DepartmentAssembler();
			response.DepartmentChoices = CollectionUtils.Map(
				this.PersistenceContext.GetBroker<IDepartmentBroker>().FindAll(false),
				(Department item) => departmentAssembler.CreateSummary(item, PersistenceContext));

			var locationAssembler = new LocationAssembler();
			response.PatientLocationChoices = CollectionUtils.Map<Location, LocationSummary>(
				this.PersistenceContext.GetBroker<ILocationBroker>().FindAll(false),
				locationAssembler.CreateLocationSummary);

			response.OrderPriorityChoices = EnumUtils.GetEnumValueList<OrderPriorityEnum>(PersistenceContext);
			response.PatientClassChoices = EnumUtils.GetEnumValueList<PatientClassEnum>(PersistenceContext);

			response.CurrentServerConfigurationRequiresTimeFilter = Worklist.CurrentServerConfigurationRequiresTimeFilter();
			response.CurrentServerConfigurationMaxSpanDays = Worklist.CurrentServerConfigurationMaxSpanDays();

			// add extra data iff editing a user-defined worklist (bug #4871)
			if (request.UserDefinedWorklist)
			{
				response.OwnerGroupChoices = CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
					this.CurrentUserStaff.ActiveGroups, // only current user's active staff groups should be choosable
					staffGroupAssembler.CreateSummary);
			}

			var proceduerTypesAssembler = new ProcedureTypeAssembler();
			response.ProcedureTypeChoices = CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(
				this.PersistenceContext.GetBroker<IProcedureTypeBroker>().FindAll(false),
				proceduerTypesAssembler.CreateSummary);

			return response;
		}

		private GetWorklistEditValidationResponse GetWorklistEditValidation(GetWorklistEditValidationRequest request)
		{
			WorklistOwner owner;

			if (!request.IsUserWorklist)
			{
				// if not creating a user worklist, the owner is Admin
				owner = WorklistOwner.Admin;
			}
			else if (request.OwnerGroup != null)
			{
				// if an owner group is specified, assign ownership to the group
				var group = PersistenceContext.Load<StaffGroup>(request.OwnerGroup.StaffGroupRef, EntityLoadFlags.Proxy);
				owner = new WorklistOwner(group);
			}
			else
			{
				// otherwise assign ownership to current user, regardless of whether a different owner staff specified
				owner = new WorklistOwner(CurrentUserStaff);
			}

			try
			{
				CheckWorklistCountRestriction(owner);
			}
			catch (RequestValidationException e)
			{
				return new GetWorklistEditValidationResponse(e.Message);
			}

			return new GetWorklistEditValidationResponse();
		}

		private WorklistOwner CreateOwner(WorklistAdminDetail detail, bool userWorklist)
		{
			// if not creating a user worklist, the owner is Admin
			if (!userWorklist)
				return WorklistOwner.Admin;

			// if an owner group is specified, assign ownership to the group
			if (detail.IsGroupOwned)
			{
				var group = PersistenceContext.Load<StaffGroup>(detail.OwnerGroup.StaffGroupRef, EntityLoadFlags.Proxy);
				return new WorklistOwner(group);
			}

			// otherwise assign ownership to current user, regardless of whether a different owner staff specified
			return new WorklistOwner(CurrentUserStaff);
		}

		/// <summary>
		/// Checks whether the current user has access to worklists owned by the specified worklist owner.
		/// </summary>
		/// <param name="owner"></param>
		private void CheckAccess(WorklistOwner owner)
		{
			// admin can access any worklist
			if (UserHasToken(AuthorityTokens.Admin.Data.Worklist))
				return;

			// if worklist is staff-owned, and user has personal token, they have access
			if (owner.IsStaffOwner && owner.Staff.Equals(this.CurrentUserStaff)
				 && UserHasToken(AuthorityTokens.Workflow.Worklist.Personal))
				return;

			// if worklist is group-owned, user must have group token and be a member of the group
			if (owner.IsGroupOwner && owner.Group.Members.Contains(this.CurrentUserStaff)
				&& UserHasToken(AuthorityTokens.Workflow.Worklist.Group))
				return;

			throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}

		/// <summary>
		/// Checks whether the current user has access to worklists owned by the specified worklist owner.
		/// </summary>
		/// <param name="owner"></param>
		/// <remarks>Throws a RequestValidationException if the worklist count exceed the configured maximum.</remarks>
		private void CheckWorklistCountRestriction(WorklistOwner owner)
		{
			// admin can have unlimited worklists
			if (owner.IsAdminOwner)
				return;

			var worklistCount = PersistenceContext.GetBroker<IWorklistBroker>().Count(owner);

			var settings = new WorklistSettings();
			if (owner.IsStaffOwner)
			{
				if (worklistCount >= settings.MaxPersonalOwnedWorklists)
					throw new RequestValidationException(SR.ExceptionMaximumWorklistsReachedForStaff);
			}
			else if (owner.IsGroupOwner)
			{
				if (worklistCount >= settings.MaxGroupOwnedWorklists)
					throw new RequestValidationException(SR.ExceptionMaximumWorklistsReachedForStaffGroup);
			}
		}

		private void UpdateWorklistHelper(WorklistAdminDetail detail, Worklist worklist)
		{
			var adminAssembler = new WorklistAdminAssembler();
			adminAssembler.UpdateWorklist(
				worklist,
				detail,
				worklist.Owner.IsAdminOwner,	// only update subscribers iff the worklist is admin owned
				this.PersistenceContext);
		}

		public static List<Type> ListClassesHelper(List<string> classNames, List<string> categories, bool includeStatic)
		{
			var worklistClasses = (IEnumerable<Type>)WorklistFactory.Instance.ListWorklistClasses(true);

			// optionally filter classes by class name
			if (classNames != null && classNames.Count > 0)
			{
				worklistClasses = worklistClasses.Where(t => classNames.Contains(Worklist.GetClassName(t)));
			}

			// optionally filter classes by category
			if (categories != null && categories.Count > 0)
			{
				worklistClasses = worklistClasses.Where(t => categories.Contains(Worklist.GetCategory(t)));
			}

			// optionally exclude static
			if (!includeStatic)
			{
				worklistClasses = worklistClasses.Where(t => !Worklist.GetIsStatic(t));
			}

			// manually exclude some classes based on workflow settings
			var workflowConfig = new WorkflowConfigurationReader();
			if (!workflowConfig.EnableInterpretationReviewWorkflow)
			{
				worklistClasses = worklistClasses.Where(t => !_interpretationReviewClasses.Contains(Worklist.GetClassName(t)));
			}
			if (!workflowConfig.EnableTranscriptionReviewWorkflow)
			{
				worklistClasses = worklistClasses.Where(t => !_transcriptionReviewClasses.Contains(Worklist.GetClassName(t)));
			}

			return worklistClasses.ToList();
		}
	}
}
