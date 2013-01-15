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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public abstract class OrderNoteboxFolder : WorkflowFolder<OrderNoteboxItemSummary>
	{
		private readonly string _noteboxClassName;

		public OrderNoteboxFolder(OrderNoteboxFolderSystem folderSystem, string noteboxClassName)
			: base(new OrderNoteboxTable())
		{
			_noteboxClassName = noteboxClassName;

			this.AutoInvalidateInterval = new TimeSpan(0, 0, 0, 0, OrderNoteboxFolderSystemSettings.Default.RefreshTime);
		}

		public override int PageSize
		{
			get { return OrderNoteboxFolderSystemSettings.Default.ItemsPerPage; }
		}

		protected override QueryItemsResult QueryItems(int firstRow, int maxRows)
		{
			QueryItemsResult result = null;
			Platform.GetService(
				delegate(IOrderNoteService service)
				{
					var request = new QueryNoteboxRequest(_noteboxClassName, true, true) {Page = new SearchResultPage(firstRow, maxRows)};
					PrepareQueryRequest(request);
					var response = service.QueryNotebox(request);
					result = new QueryItemsResult(response.NoteboxItems, response.ItemCount);
				});

			return result;
		}

		protected override int QueryCount()
		{
			int count = -1;
			Platform.GetService(
				delegate(IOrderNoteService service)
				{
					var request = new QueryNoteboxRequest(_noteboxClassName, true, false);
					PrepareQueryRequest(request);
					var response = service.QueryNotebox(request);
					count = response.ItemCount;
				});

			return count;
		}

		protected virtual void PrepareQueryRequest(QueryNoteboxRequest request)
		{
			// nothing to do
		}
	}
}
