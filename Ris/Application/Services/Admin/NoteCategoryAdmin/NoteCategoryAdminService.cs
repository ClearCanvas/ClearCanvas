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
using System.Text;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;
using System.Security.Permissions;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.NoteCategoryAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(INoteCategoryAdminService))]
    public class NoteCategoryAdminService : ApplicationServiceBase, INoteCategoryAdminService
    {
        #region INoteCategoryAdminService Members

        /// <summary>
        /// Return all NoteCategory options
        /// </summary>
        /// <returns></returns>
        [ReadOperation]
        public ListAllNoteCategoriesResponse ListAllNoteCategories(ListAllNoteCategoriesRequest request)
        {
            PatientNoteCategorySearchCriteria criteria = new PatientNoteCategorySearchCriteria();
			criteria.Name.SortAsc(0);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();
            return new ListAllNoteCategoriesResponse(
                CollectionUtils.Map<PatientNoteCategory, PatientNoteCategorySummary, List<PatientNoteCategorySummary>>(
                    PersistenceContext.GetBroker<IPatientNoteCategoryBroker>().Find(criteria, request.Page),
                    delegate(PatientNoteCategory category)
                    {
                        return assembler.CreateNoteCategorySummary(category, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetNoteCategoryEditFormDataResponse GetNoteCategoryEditFormData(GetNoteCategoryEditFormDataRequest request)
        {
            List<EnumValueInfo> severityChoices = EnumUtils.GetEnumValueList<NoteSeverityEnum>(PersistenceContext);
            return new GetNoteCategoryEditFormDataResponse(severityChoices);

        }

        [ReadOperation]
        public LoadNoteCategoryForEditResponse LoadNoteCategoryForEdit(LoadNoteCategoryForEditRequest request)
        {
            // note that the version of the NoteCategoryRef is intentionally ignored here (default behaviour of ReadOperation)
            PatientNoteCategory category = PersistenceContext.Load<PatientNoteCategory>(request.NoteCategoryRef);
            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();

            return new LoadNoteCategoryForEditResponse(category.GetRef(), assembler.CreateNoteCategoryDetail(category, this.PersistenceContext));
        }

        /// <summary>
        /// Add the specified NoteCategory
        /// </summary>
        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.PatientNoteCategory)]
        public AddNoteCategoryResponse AddNoteCategory(AddNoteCategoryRequest request)
        {
            PatientNoteCategory noteCategory = new PatientNoteCategory();

            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();
            assembler.UpdateNoteCategory(request.NoteCategoryDetail, noteCategory);

            PersistenceContext.Lock(noteCategory, DirtyState.New);

            // ensure the new NoteCategory is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddNoteCategoryResponse(assembler.CreateNoteCategorySummary(noteCategory, this.PersistenceContext));
        }


        /// <summary>
        /// Update the specified NoteCategory
        /// </summary>
        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.PatientNoteCategory)]
		public UpdateNoteCategoryResponse UpdateNoteCategory(UpdateNoteCategoryRequest request)
        {
            PatientNoteCategory noteCategory = PersistenceContext.Load<PatientNoteCategory>(request.NoteCategoryDetail.NoteCategoryRef, EntityLoadFlags.CheckVersion);

            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();
            assembler.UpdateNoteCategory(request.NoteCategoryDetail, noteCategory);

            return new UpdateNoteCategoryResponse(assembler.CreateNoteCategorySummary(noteCategory, this.PersistenceContext));
        }

		/// <summary>
		/// Delete the specified NoteCategory
		/// </summary>
		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.PatientNoteCategory)]
		public DeleteNoteCategoryResponse DeleteNoteCategory(DeleteNoteCategoryRequest request)
		{
			try
			{
				IPatientNoteCategoryBroker broker = PersistenceContext.GetBroker<IPatientNoteCategoryBroker>();
				PatientNoteCategory item = broker.Load(request.NoteCategoryRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteNoteCategoryResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(PatientNoteCategory))));
			}
		}

		#endregion

    }
}
