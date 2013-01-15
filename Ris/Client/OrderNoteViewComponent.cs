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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class OrderNoteContext : DataContractBase
		{
			[DataContract]
			public class Foo
			{
				[DataMember]
				public OrderNoteDetail NoteDetail;

				[DataMember]
				public bool WillAcknowledge;
			}

			public OrderNoteContext(List<OrderNoteDetail> orderNotes)
			{
				this.OrderNotes = CollectionUtils.Map(orderNotes,
					(OrderNoteDetail note) => new Foo { NoteDetail = note });
			}

			[DataMember]
			public List<Foo> OrderNotes;
		}

		private event EventHandler _checkedItemsChanged;

		private readonly OrderNoteContext _context;

		public OrderNoteViewComponent()
			: this(null)
		{
		}

		public OrderNoteViewComponent(List<OrderNoteDetail> orderNotes)
		{
			_context = orderNotes == null ? null : new OrderNoteContext(orderNotes);
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.OrderNotePreviewPageUrl);
			base.Start();
		}

		public bool HasExistingNotes
		{
			get { return _context.OrderNotes.Count > 0; }
		}

		/// <summary>
		/// Gets a value indicating whether there are any notes that can be ack'd by the current user.
		/// </summary>
		public bool HasAcknowledgeableNotes
		{
			get
			{
				return CollectionUtils.Contains(_context.OrderNotes, note => note.NoteDetail.CanAcknowledge);
			}
		}

		/// <summary>
		/// Gets the set of notes that can be ack'ed by the current user.
		/// </summary>
		public List<OrderNoteDetail> AcknowledgeableNotes
		{
			get
			{
				return CollectionUtils.Map(
					CollectionUtils.Select(_context.OrderNotes, note => note.NoteDetail.CanAcknowledge),
					(OrderNoteContext.Foo f) => f.NoteDetail);
			}
		}

		/// <summary>
		/// Gets the set of notes that are currently checked off to be acknowledged.
		/// </summary>
		public List<OrderNoteDetail> CheckedNotes
		{
			get
			{
				return CollectionUtils.Map(
					CollectionUtils.Select(_context.OrderNotes, f => f.NoteDetail.CanAcknowledge && f.WillAcknowledge),
					(OrderNoteContext.Foo f) => f.NoteDetail);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the user has checked all acknowledgeable notes.
		/// </summary>
		public bool AllAcknowledgeableNotesAreChecked
		{
			get
			{
				// all acknowledgeable notes will be acknowledged
				return CollectionUtils.TrueForAll(_context.OrderNotes,
					f => !f.NoteDetail.CanAcknowledge || f.WillAcknowledge);
			}
		}

		public event EventHandler CheckedItemsChanged
		{
			add { _checkedItemsChanged += value; }
			remove { _checkedItemsChanged -= value; }
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected override void SetTag(string tag, string data)
		{
			// the page uses SetTag to indicate that the check state has changed
			var checkStates = JsmlSerializer.Deserialize<List<bool>>(data);
			for (var i = 0; i < _context.OrderNotes.Count; i++)
			{
				_context.OrderNotes[i].WillAcknowledge = checkStates[i];
			}
			EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
		}

	}
}
