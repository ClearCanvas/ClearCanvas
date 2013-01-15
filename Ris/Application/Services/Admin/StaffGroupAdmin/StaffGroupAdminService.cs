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
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;
using System.Threading;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffGroupAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IStaffGroupAdminService))]
    public class StaffGroupAdminService : ApplicationServiceBase, IStaffGroupAdminService
    {
        #region IStaffGroupAdminService Members

        [ReadOperation]
        public TextQueryResponse<StaffGroupSummary> TextQuery(StaffGroupTextQueryRequest request)
        {
            var broker = PersistenceContext.GetBroker<IStaffGroupBroker>();
            var assembler = new StaffGroupAssembler();

            var helper = new TextQueryHelper<StaffGroup, StaffGroupSearchCriteria, StaffGroupSummary>(
                    delegate
                    {
                        var rawQuery = request.TextQuery;

                        // allow matching on name (assume entire query is a name which may contain spaces)
                        var nameCriteria = new StaffGroupSearchCriteria();
                        nameCriteria.Name.StartsWith(rawQuery);
						if(request.ElectiveGroupsOnly)
							nameCriteria.Elective.EqualTo(true);

                        return new []{ nameCriteria };
                    },
                    assembler.CreateSummary,
                    (criteria, threshold) => broker.Count(criteria) <= threshold,
                    broker.Find);

            return helper.Query(request);
        }

        [ReadOperation]
        public ListStaffGroupsResponse ListStaffGroups(ListStaffGroupsRequest request)
        {
            Platform.CheckForNullReference(request, "request");

        	var where = new StaffGroupSearchCriteria();
			where.Name.SortAsc(0);
			if (request.ElectiveGroupsOnly)
				where.Elective.EqualTo(true);
			if (!request.IncludeDeactivated)
				where.Deactivated.EqualTo(false);

            var broker = PersistenceContext.GetBroker<IStaffGroupBroker>();
			var items = broker.Find(where, request.Page);

            var assembler = new StaffGroupAssembler();
            return new ListStaffGroupsResponse(
                CollectionUtils.Map(items, (StaffGroup item) => assembler.CreateSummary(item))
                );
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public LoadStaffGroupForEditResponse LoadStaffGroupForEdit(LoadStaffGroupForEditRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.StaffGroupRef, "request.StaffGroupRef");

            var item = PersistenceContext.Load<StaffGroup>(request.StaffGroupRef);

            var assembler = new StaffGroupAssembler();
            return new LoadStaffGroupForEditResponse(assembler.CreateDetail(item, PersistenceContext));
        }

        [ReadOperation]
        public LoadStaffGroupEditorFormDataResponse LoadStaffGroupEditorFormData(LoadStaffGroupEditorFormDataRequest request)
        {
			var allStaff = PersistenceContext.GetBroker<IStaffBroker>().FindAll(false);

			var worklistClasses = WorklistAdminService.ListClassesHelper(null, null, false);

			// grab the persistent worklists
			var broker = PersistenceContext.GetBroker<IWorklistBroker>();
			var persistentClassNames = 
				CollectionUtils.Select(worklistClasses, t => !Worklist.GetIsStatic(t))
				.ConvertAll(t => Worklist.GetClassName(t));

			var adminWorklists = broker.Find(null, false, persistentClassNames, null);
			
			var staffAssembler = new StaffAssembler();
			var worklistAssembler = new WorklistAssembler();
            return new LoadStaffGroupEditorFormDataResponse(
                CollectionUtils.Map(allStaff, (Staff staff) => staffAssembler.CreateStaffSummary(staff, PersistenceContext)),
				CollectionUtils.Map(adminWorklists, (Worklist worklist) => worklistAssembler.GetWorklistSummary(worklist, PersistenceContext))
				);
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public AddStaffGroupResponse AddStaffGroup(AddStaffGroupRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.StaffGroup, "request.StaffGroup");

            var item = new StaffGroup();

            var assembler = new StaffGroupAssembler();
			var worklistEditable = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.Worklist);
			assembler.UpdateStaffGroup(item, request.StaffGroup, worklistEditable, true, PersistenceContext);

            PersistenceContext.Lock(item, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddStaffGroupResponse(assembler.CreateSummary(item));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public UpdateStaffGroupResponse UpdateStaffGroup(UpdateStaffGroupRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.StaffGroup, "request.StaffGroup");
            Platform.CheckMemberIsSet(request.StaffGroup.StaffGroupRef, "request.StaffGroup.StaffGroupRef");

            var item = PersistenceContext.Load<StaffGroup>(request.StaffGroup.StaffGroupRef);

            var assembler = new StaffGroupAssembler();
			var worklistEditable = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.Worklist);
			assembler.UpdateStaffGroup(item, request.StaffGroup, worklistEditable, false, PersistenceContext);

            PersistenceContext.SynchState();

            return new UpdateStaffGroupResponse(assembler.CreateSummary(item));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public DeleteStaffGroupResponse DeleteStaffGroup(DeleteStaffGroupRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IStaffGroupBroker>();
				var item = broker.Load(request.StaffGroupRef, EntityLoadFlags.Proxy);

				// Remove worklist association before deleting a staff group
				var worklists = PersistenceContext.GetBroker<IWorklistBroker>().Find(item);
				CollectionUtils.ForEach(worklists, worklist => worklist.GroupSubscribers.Remove(item));
				
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteStaffGroupResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(StaffGroup))));
			}
		}

    	#endregion
    }
}
