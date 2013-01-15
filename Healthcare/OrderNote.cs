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
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;
using ClearCanvas.Common.Utilities;
using System;


namespace ClearCanvas.Healthcare
{
	[ExtensionPoint]
	public class VirtualOrderNoteProviderExtensionPoint : ExtensionPoint<IOrderNoteProvider>
	{
	}



	/// <summary>
	/// OrderNote component
	/// </summary>
	public partial class OrderNote
	{
		public static class Categories
		{
			public const string General = "General";
		}


		/// <summary>
		/// Gets all categories of notes associated with the specified order, excluding virtual notes.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public static IList<OrderNote> GetNotesForOrder(Order order)
		{
			return GetNotesForOrder(order, new string[0], false);
		}

		public static IList<OrderNote> GetNotesForOrder(Order order, string category, bool includeVirtual)
		{
			return GetNotesForOrder(order, new[] {category}, includeVirtual);
		}

		/// <summary>
		/// Gets all order notes of the specified categories associated with the specified order,
		/// optionally including virtual notes.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="categories"></param>
		/// <param name="includeVirtual"></param>
		/// <returns></returns>
		public static IList<OrderNote> GetNotesForOrder(Order order, IList<string> categories, bool includeVirtual)
		{
			var virtualNotes = new List<OrderNote>();
			if(includeVirtual)
			{
				foreach (IOrderNoteProvider virtualNoteProvider in new VirtualOrderNoteProviderExtensionPoint().CreateExtensions())
				{
					virtualNotes.AddRange(virtualNoteProvider.GetNotes(order, categories, PersistenceScope.CurrentContext));
				}
			}
			
			var where = new OrderNoteSearchCriteria();
			where.Order.EqualTo(order);
			where.PostTime.IsNotNull(); // only posted notes
			if (categories != null && categories.Count > 0)
			{
				where.Category.In(categories);
			}

			//run a query to find order notes
			//TODO: using PersistenceScope is maybe not ideal but no other option right now (fix #3472)
			var persistentNotes = PersistenceScope.CurrentContext.GetBroker<IOrderNoteBroker>().Find(where);

			return CollectionUtils.Concat(persistentNotes, virtualNotes);
		}

		/// <summary>
		/// Constructs a new General order note with the specified properties.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="author"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		public static OrderNote CreateGeneralNote(Order order, Staff author, string body)
		{
			return new OrderNote(
				Categories.General,
				body,
				false,
				Platform.Time,
				author,
				null,
				Platform.Time,
				false,
				false,
				new HashedSet<NotePosting>(),
				null,
				order);
		}

		/// <summary>
		/// Creates a new virtual note with the specified properties.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="category"></param>
		/// <param name="author"></param>
		/// <param name="body"></param>
		/// <param name="postTime"></param>
		/// <returns></returns>
		public static OrderNote CreateVirtualNote(Order order, string category, Staff author, string body, DateTime postTime)
		{
			return new OrderNote(
				category,
				body,
				false,
				postTime,
				author,
				null,
				postTime,
				false,
				false,
				new HashedSet<NotePosting>(),
				null,
				order);
		}

		/// <summary>
		/// Constructor for creating a new unposted note.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="category"></param>
		/// <param name="author"></param>
		/// <param name="onBehalfOf"></param>
		/// <param name="body"></param>
		/// <param name="urgent"></param>
		public OrderNote(Order order, string category, Staff author, StaffGroup onBehalfOf, string body, bool urgent)
			: base(category, author, onBehalfOf, body, urgent)
		{
			_order = order;
		}

		/// <summary>
		/// Creates a ghost copy of this order note.
		/// </summary>
		/// <returns></returns>
		public virtual OrderNote CreateGhostCopy()
		{
			return new OrderNote(
				this.Category,
				this.Body,
				this.Urgent,
				this.CreationTime,
				this.Author,
				this.OnBehalfOfGroup,
				this.PostTime,
				this.IsFullyAcknowledged,
				false,							// ghost copies do not have postings
				new HashedSet<NotePosting>(),	// ghost copies do not have postings
				this,
				_order);
		}


		/* Commented out for ticket #3709, where it is explicitly requested that user can post new notes without acknowledging previous notes. */
		///// <summary>
		///// Overridden to validate that the order does not have any notes that are pending acknowledgement
		///// that could be acknowledged by the author of this note.
		///// </summary>
		//protected override void BeforePost()
		//{
		//    // does the order have any notes, in the same category as this note,
		//    // that could have been acknowledged by the author of this note but haven't been?
		//    IList<OrderNote> allNotes = GetNotesForOrder(_order, this.Category);
		//    bool unAckedNotes = CollectionUtils.Contains(allNotes,
		//        delegate(OrderNote note)
		//        {
		//            // ignore this note
		//            return !Equals(this, note) && note.CanAcknowledge(this.Author);
		//        });

		//    if (unAckedNotes)
		//        throw new NoteAcknowledgementException("Order has associated notes that must be acknowledged by this author prior to posting a new note.");

		//    base.BeforePost();
		//}


		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}