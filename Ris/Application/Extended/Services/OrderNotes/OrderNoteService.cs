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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;
using ClearCanvas.Ris.Application.Services;

namespace ClearCanvas.Ris.Application.Extended.Services.OrderNotes
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IOrderNoteService))]
	public class OrderNoteService : ApplicationServiceBase, IOrderNoteService
	{
		/// <summary>
		/// Implementation of <see cref="IWorklistQueryContext"/>.
		/// </summary>
		class NoteboxQueryContext : INoteboxQueryContext
		{
			private readonly ApplicationServiceBase _applicationService;
			private readonly SearchResultPage _page;
			private readonly StaffGroup _staffGroup;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="service"></param>
			/// <param name="page"></param>
			/// <param name="staffGroup"></param>
			public NoteboxQueryContext(ApplicationServiceBase service, SearchResultPage page, StaffGroup staffGroup)
			{
				_applicationService = service;
				_page = page;
				_staffGroup = staffGroup;
			}

			#region INoteboxQueryContext Members

			/// <summary>
			/// Gets the current user <see cref="Healthcare.Staff"/> object.
			/// </summary>
			public Staff Staff
			{
				get { return _applicationService.CurrentUserStaff; }
			}

			/// <summary>
			/// Gets the staff group (relevant only for group folders).
			/// </summary>
			public StaffGroup StaffGroup
			{
				get { return _staffGroup; }
			}

			/// <summary>
			/// Gets the <see cref="SearchResultPage"/> that specifies which page of the worklist is requested.
			/// </summary>
			public SearchResultPage Page
			{
				get { return _page; }
			}

			/// <summary>
			/// Obtains an instance of the specified broker.
			/// </summary>
			/// <typeparam name="TBrokerInterface"></typeparam>
			/// <returns></returns>
			public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
			{
				return _applicationService.PersistenceContext.GetBroker<TBrokerInterface>();
			}

			#endregion
		}

		#region IOrderNoteService Members

		[ReadOperation]
		public ListStaffGroupsResponse ListStaffGroups(ListStaffGroupsRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			var assembler = new StaffGroupAssembler();
			return new ListStaffGroupsResponse(
				CollectionUtils.Map(this.CurrentUserStaff.Groups, (StaffGroup g) => assembler.CreateSummary(g)));
		}

		[UpdateOperation]
		public AddStaffGroupsResponse AddStaffGroups(AddStaffGroupsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.StaffGroups, "StaffGroups");

			CollectionUtils.ForEach(request.StaffGroups,
				delegate(StaffGroupSummary s)
				{
					var group = PersistenceContext.Load<StaffGroup>(s.StaffGroupRef, EntityLoadFlags.Proxy);
					if (!group.Elective)
						throw new RequestValidationException(string.Format("You cannot be added to non-elective staff group '{0}'.", group.Name));
					group.AddMember(this.CurrentUserStaff);
				});

			return new AddStaffGroupsResponse();
		}

		[ReadOperation]
		public QueryNoteboxResponse QueryNotebox(QueryNoteboxRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.NoteboxClass, "request.NoteboxClass");

			var notebox = NoteboxFactory.Instance.CreateNotebox(request.NoteboxClass);
			var page = new SearchResultPage(request.Page.FirstRow, request.Page.MaxRows);
			var nqc = new NoteboxQueryContext(this, page, request.StaffGroupRef == null ?
				null : PersistenceContext.Load<StaffGroup>(request.StaffGroupRef));

			IList results = null;
			if (request.QueryItems)
			{
				// get the first page, up to the default max number of items per page
				results = notebox.GetItems(nqc);
			}

			var count = -1;
			if (request.QueryCount)
			{
				// if the items were already queried, and the number returned is less than the max per page, and this is the first page
				// then there is no need to do a separate count query
				if (results != null && results.Count < page.MaxRows && page.FirstRow == 0)
					count = results.Count;
				else
					count = notebox.GetItemCount(nqc);
			}

			if (request.QueryItems)
			{
				var assembler = new OrderNoteboxItemAssembler();
				var items = CollectionUtils.Map(
					results,
					(OrderNoteboxItem item) => assembler.CreateSummary(item, PersistenceContext));

				return new QueryNoteboxResponse(items, count);
			}

			return new QueryNoteboxResponse(new List<OrderNoteboxItemSummary>(), count);
		}

		[ReadOperation]
		public GetConversationResponse GetConversation(GetConversationRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "request.OrderRef");

			var order = PersistenceContext.Load<Order>(request.OrderRef);

			// select only notes that are actually posted, and meet category filter
			var notes = CollectionUtils.Select(OrderNote.GetNotesForOrder(order),
											   n => n.IsPosted && (request.CategoryFilters == null || request.CategoryFilters.Count == 0
												|| request.CategoryFilters.Contains(n.Category)));

			// put most recent notes first
			notes.Reverse();

			var noteAssembler = new OrderNoteAssembler();
			if (request.CountOnly)
				return new GetConversationResponse(order.GetRef(), null, notes.Count);

			return new GetConversationResponse(
				order.GetRef(),
				CollectionUtils.Map(notes, (OrderNote n) => noteAssembler.CreateOrderNoteDetail(n, CurrentUserStaff, PersistenceContext)),
				notes.Count);
		}

		[ReadOperation]
		public GetConversationEditorFormDataResponse GetConversationEditorFormData(
			GetConversationEditorFormDataRequest request)
		{
			var staffAssembler = new StaffAssembler();
			var groupAssembler = new StaffGroupAssembler();
			var response = new GetConversationEditorFormDataResponse(
				CollectionUtils.Map(
					this.CurrentUserStaff.ActiveGroups,	// only active staff groups should be choices
					(StaffGroup sg) => groupAssembler.CreateSummary(sg)));

			if (request.RecipientStaffIDs != null && request.RecipientStaffIDs.Count > 0)
			{
				var criteria = new StaffSearchCriteria();
				criteria.Id.In(request.RecipientStaffIDs);
				response.RecipientStaffs = CollectionUtils.Map(
					PersistenceContext.GetBroker<IStaffBroker>().Find(criteria),
					(Staff s) => staffAssembler.CreateStaffSummary(s, PersistenceContext));
			}

			if (request.RecipientStaffGroupNames != null && request.RecipientStaffGroupNames.Count > 0)
			{
				var criteria = new StaffGroupSearchCriteria();
				criteria.Name.In(request.RecipientStaffGroupNames);
				response.RecipientStaffGroups = CollectionUtils.Map(
					PersistenceContext.GetBroker<IStaffGroupBroker>().Find(criteria),
					(StaffGroup sg) => groupAssembler.CreateSummary(sg));
			}

			return response;
		}

		[UpdateOperation]
		public AcknowledgeAndPostResponse AcknowledgeAndPost(AcknowledgeAndPostRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "request.OrderRef");

			var order = PersistenceContext.Load<Order>(request.OrderRef);

			// process acknowledgements first
			if (request.OrderNotesToAcknowledge != null)
			{
				var notes = CollectionUtils.Map(request.OrderNotesToAcknowledge,
					(EntityRef noteRef) => PersistenceContext.Load<OrderNote>(noteRef));

				foreach (var note in notes)
				{
					//validate that the note is actually associated with the correct order
					if (!Equals(note.Order, order))
						throw new ArgumentException("Attempt to acknowledge a note that is not associated with this order.");

					note.Acknowledge(CurrentUserStaff);
				}
			}

			try
			{
				// process reply note
				OrderNote replyNote = null;
				var noteAssembler = new OrderNoteAssembler();
				if (request.OrderNote != null)
				{
					replyNote = noteAssembler.CreateOrderNote(
						request.OrderNote, order, CurrentUserStaff, true, PersistenceContext);
				}
				PersistenceContext.SynchState();

				return replyNote != null ?
					new AcknowledgeAndPostResponse(noteAssembler.CreateOrderNoteDetail(replyNote, CurrentUserStaff, PersistenceContext))
					: new AcknowledgeAndPostResponse(null);
			}
			catch (NoteAcknowledgementException e)
			{
				// occurs when there are notes that this author must ack prior to posting a new note
				throw new RequestValidationException(e.Message);
			}
		}

		#endregion
	}
}
