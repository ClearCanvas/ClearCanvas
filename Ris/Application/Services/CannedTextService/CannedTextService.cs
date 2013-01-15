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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	[ServiceImplementsContract(typeof(ICannedTextService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class CannedTextService : ApplicationServiceBase, ICannedTextService
	{
		#region ICannedTextService Members

		[ReadOperation]
		public ListCannedTextForUserResponse ListCannedTextForUser(ListCannedTextForUserRequest request)
		{
			var assembler = new CannedTextAssembler();
			var criterias = new List<CannedTextSearchCriteria>();

			var personalCannedTextCriteria = new CannedTextSearchCriteria();
			personalCannedTextCriteria.Staff.EqualTo(this.CurrentUserStaff);
			if (!string.IsNullOrEmpty(request.Name))
				personalCannedTextCriteria.Name.EqualTo(request.Name);
			criterias.Add(personalCannedTextCriteria);

			if (this.CurrentUserStaff.Groups != null && this.CurrentUserStaff.Groups.Count > 0)
			{
				var groupCannedTextCriteria = new CannedTextSearchCriteria();
				groupCannedTextCriteria.StaffGroup.In(this.CurrentUserStaff.Groups);
				if (!string.IsNullOrEmpty(request.Name))
					groupCannedTextCriteria.Name.EqualTo(request.Name);
				criterias.Add(groupCannedTextCriteria);
			}

			var results = PersistenceContext.GetBroker<ICannedTextBroker>().Find(criterias.ToArray(), request.Page);

			var staffCannedText = CollectionUtils.Map<CannedText, CannedTextSummary>(results,
				cannedText => assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));

			return new ListCannedTextForUserResponse(staffCannedText);
		}

		[ReadOperation]
		public GetCannedTextEditFormDataResponse GetCannedTextEditFormData(GetCannedTextEditFormDataRequest request)
		{
			var groupAssembler = new StaffGroupAssembler();

			return new GetCannedTextEditFormDataResponse(
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
					this.CurrentUserStaff.ActiveGroups,	// only active staff groups should be choosable
					groupAssembler.CreateSummary));
		}

		[ReadOperation]
		public LoadCannedTextForEditResponse LoadCannedTextForEdit(LoadCannedTextForEditRequest request)
		{
			var broker = PersistenceContext.GetBroker<ICannedTextBroker>();
			CannedText cannedText;

			if (request.CannedTextRef != null)
			{
				cannedText = broker.Load(request.CannedTextRef);
			}
			else
			{
				var criteria = new CannedTextSearchCriteria();

				if (!string.IsNullOrEmpty(request.Name))
					criteria.Name.EqualTo(request.Name);

				if (!string.IsNullOrEmpty(request.Category))
					criteria.Category.EqualTo(request.Category);

				if (!string.IsNullOrEmpty(request.StaffId))
					criteria.Staff.Id.EqualTo(request.StaffId);

				if (!string.IsNullOrEmpty(request.StaffGroupName))
					criteria.StaffGroup.Name.EqualTo(request.StaffGroupName);

				cannedText = broker.FindOne(criteria);
			}

			var assembler = new CannedTextAssembler();
			return new LoadCannedTextForEditResponse(assembler.GetCannedTextDetail(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public AddCannedTextResponse AddCannedText(AddCannedTextRequest request)
		{
			CheckCannedTextWriteAccess(request.Detail);

			if (string.IsNullOrEmpty(request.Detail.Name))
				throw new RequestValidationException(SR.ExceptionCannedTextNameRequired);

			if (string.IsNullOrEmpty(request.Detail.Category))
				throw new RequestValidationException(SR.ExceptionCannedTextCategoryRequired);

			var assembler = new CannedTextAssembler();
			var cannedText = assembler.CreateCannedText(request.Detail, this.CurrentUserStaff, this.PersistenceContext);

			PersistenceContext.Lock(cannedText, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request)
		{
			CheckCannedTextWriteAccess(request.Detail);

			var cannedText = this.PersistenceContext.Load<CannedText>(request.CannedTextRef);

			var assembler = new CannedTextAssembler();
			assembler.UpdateCannedText(cannedText, request.Detail, this.CurrentUserStaff, this.PersistenceContext);

			PersistenceContext.SynchState();
			return new UpdateCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request)
		{
			var cannedText = this.PersistenceContext.Load<CannedText>(request.CannedTextRef, EntityLoadFlags.Proxy);
			CheckCannedTextWriteAccess(cannedText);

			PersistenceContext.GetBroker<ICannedTextBroker>().Delete(cannedText);

			return new DeleteCannedTextResponse();
		}

		[UpdateOperation]
		public EditCannedTextCategoriesResponse EditCannedTextCategories(EditCannedTextCategoriesRequest request)
		{
			var cannedTexts = CollectionUtils.Map<EntityRef, CannedText>(
				request.CannedTextRefs,
				cannedTextRef => this.PersistenceContext.Load<CannedText>(cannedTextRef));

			foreach (var cannedText in cannedTexts)
			{
				CheckCannedTextWriteAccess(cannedText);
				cannedText.Category = request.Category;
			}

			this.PersistenceContext.SynchState();

			var assembler = new CannedTextAssembler();
			return new EditCannedTextCategoriesResponse(CollectionUtils.Map<CannedText, CannedTextSummary>(
				cannedTexts,
				cannedText => assembler.GetCannedTextSummary(cannedText, this.PersistenceContext)));
		}

		#endregion

		private static void CheckCannedTextWriteAccess(CannedText cannedText)
		{
			CheckCannedTextWriteAccess(cannedText.StaffGroup == null);
		}

		private static void CheckCannedTextWriteAccess(CannedTextDetail cannedText)
		{
			CheckCannedTextWriteAccess(cannedText.IsPersonal);
		}

		private static void CheckCannedTextWriteAccess(bool isPersonal)
		{
			if (isPersonal && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.CannedText.Personal) == false ||
				!isPersonal && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.CannedText.Group) == false)
				throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}

	}
}
