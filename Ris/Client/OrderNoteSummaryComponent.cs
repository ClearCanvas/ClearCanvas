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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteSummaryComponent : SummaryComponentBase<OrderNoteDetail, OrderNoteTable>
	{
		private readonly OrderNoteCategory _category;
		private IList<OrderNoteDetail> _notes;
		private readonly bool _canEdit;

		/// <summary>
		/// Constructor allowing edits.
		/// </summary>
		/// <param name="category"></param>
		public OrderNoteSummaryComponent(OrderNoteCategory category)
			: this(category, true)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="category">Specifies the category of order notes to display, and the category under which new notes are placed.</param>
		/// <param name="canEdit">Specifies if the component is editable.  If not, all action buttons are disabled.</param>
		public OrderNoteSummaryComponent(OrderNoteCategory category, bool canEdit)
			: base(false)
		{
			Platform.CheckForNullReference(category, "category");

			_canEdit = canEdit;
			_category = category;
			_notes = new List<OrderNoteDetail>();

			this.Table.UpdateNoteClickLinkDelegate = UpdateOrderNoteDetail;
		}

		public IList<OrderNoteDetail> Notes
		{
			get { return _notes; }
			set
			{
				_notes = value;
				this.Table.Items.Clear();
				this.Table.Items.AddRange(CollectionUtils.Select(_notes, d => d.Category == _category.Key));
			}
		}

		#region Overrides

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		protected override bool SupportsPaging
		{
			get { return false; }
		}

		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.Enabled = _canEdit;
		}

		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			var selectedNote = CollectionUtils.FirstElement(this.SelectedItems);
			if (selectedNote == null)
				return;

			// only un-posted notes can be edited or deleted
			this.ActionModel.Edit.Enabled = selectedNote.PostTime == null;
			this.ActionModel.Delete.Enabled = selectedNote.PostTime == null;
		}

		protected override IList<OrderNoteDetail> ListItems(int firstRow, int maxRows)
		{
			// Only list the notes of the specified category.
			return CollectionUtils.Select(_notes, d => d.Category == _category.Key);
		}

		protected override bool AddItems(out IList<OrderNoteDetail> addedItems)
		{
			addedItems = new List<OrderNoteDetail>();

			var newNote = new OrderNoteDetail(_category.Key, "", null, false, null, null);
			var editor = new OrderNoteEditorComponent(newNote);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText))
			{
				addedItems.Add(newNote);
				_notes.Add(newNote);
				return true;
			}

			return false;
		}

		protected override bool EditItems(IList<OrderNoteDetail> items, out IList<OrderNoteDetail> editedItems)
		{
			editedItems = new List<OrderNoteDetail>();

			var note = CollectionUtils.FirstElement(items);
			var editor = new OrderNoteEditorComponent(note);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText))
			{
				editedItems.Add(note);
				return true;
			}

			return false;
		}

		protected override bool DeleteItems(IList<OrderNoteDetail> items, out IList<OrderNoteDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<OrderNoteDetail>();

			foreach (var item in items)
			{
				deletedItems.Add(item);
				_notes.Remove(item);
			}

			return deletedItems.Count > 0;
		}

		protected override bool IsSameItem(OrderNoteDetail x, OrderNoteDetail y)
		{
			if (ReferenceEquals(x, y))
				return true;

			// if either one is null, we cannot assume they are the same item
			if (x.OrderNoteRef == null || y.OrderNoteRef == null)
				return false;

			return x.OrderNoteRef.Equals(y.OrderNoteRef, true);
		}

		#endregion

		private void UpdateOrderNoteDetail(OrderNoteDetail note)
		{
			this.SummarySelection = new Selection(note);
			this.EditSelectedItems();
		}
	}
}
