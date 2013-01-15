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

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class AttachmentSummaryTable : Table<AttachmentSummary>
	{
		public AttachmentSummaryTable()
		{
			var receivedDateColumn = new DateTableColumn<AttachmentSummary>(
				SR.ColumnReceivedDate,
				summary => summary.Document.ReceivedTime,
				0.2f);
			this.Columns.Add(receivedDateColumn);
			this.Columns.Add(new TableColumn<AttachmentSummary, string>(
								SR.ColumnCategory,
								summary => summary.Category.Value,
								0.2f));
			this.Columns.Add(new TableColumn<AttachmentSummary, string>(
								SR.ColumnAttachedBy,
								summary => summary.AttachedBy == null ? "me" : PersonNameFormat.Format(summary.AttachedBy.Name),
								0.2f));
			this.Columns.Add(new TableColumn<AttachmentSummary, string>(
								SR.ColumnAttachmentType,
								summary => summary.Document.DocumentTypeName,
								0.2f));

			this.Sort(new TableSortParams(receivedDateColumn, false));
		}
	}
}