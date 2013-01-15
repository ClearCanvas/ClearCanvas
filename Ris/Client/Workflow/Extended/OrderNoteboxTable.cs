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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public class OrderNoteboxTable : Table<OrderNoteboxItemSummary>
	{
		private const int NumRows = 4;
		private readonly DateTimeTableColumn<OrderNoteboxItemSummary> _postTimeColumn;

		public OrderNoteboxTable()
			: this(NumRows)
		{
		}

		private OrderNoteboxTable(int cellRowCount)
			: base(cellRowCount)
		{
			var resolver = new ResourceResolver(this.GetType().Assembly);

			var urgentColumn = new TableColumn<OrderNoteboxItemSummary, IconSet>(SR.ColumnUrgent,
				item => item.Urgent ? new IconSet("SingleExclamation.png") : null, 0.3f)
				{
					Comparison = (item1, item2) => item1.Urgent.CompareTo(item2.Urgent),
					ResourceResolver = resolver
				};
			this.Columns.Add(urgentColumn);

			/* JR: this isn't needed right now, because acknowledged notes are never shown.
			TableColumn<OrderNoteboxItemSummary, IconSet> acknowledgedColumn =
				new TableColumn<OrderNoteboxItemSummary, IconSet>(SR.ColumnStatus,
					delegate(OrderNoteboxItemSummary item) { return GetIsAcknowledgedIcon(item.IsAcknowledged); },
					0.3f);
			acknowledgedColumn.Comparison = delegate(OrderNoteboxItemSummary item1, OrderNoteboxItemSummary item2)
				{ return item1.IsAcknowledged.CompareTo(item2.IsAcknowledged); };
			acknowledgedColumn.ResourceResolver = resolver;
			this.Columns.Add(acknowledgedColumn);
			 */

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnMRN, item => MrnFormat.Format(item.Mrn), 1.0f));
			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnPatientName, item => PersonNameFormat.Format(item.PatientName), 1.0f));
			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnDescription, 
				item => string.Format("{0} {1}", AccessionFormat.Format(item.AccessionNumber), item.DiagnosticServiceName),
				1.0f, 1));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnFrom,
				item => item.OnBehalfOfGroup != null
					? String.Format(SR.FormatFromOnBehalf, StaffNameAndRoleFormat.Format(item.Author), item.OnBehalfOfGroup.Name, item.PostTime)
					: String.Format(SR.FormatFrom, StaffNameAndRoleFormat.Format(item.Author), item.PostTime),
				1.0f, 2));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnTo,
				item => String.Format(SR.FormatTo, RecipientsList(item.StaffRecipients, item.GroupRecipients)),
				1.0f, 3));

			this.Columns.Add(_postTimeColumn = new DateTimeTableColumn<OrderNoteboxItemSummary>(SR.ColumnPostTime,
				item => item.PostTime));
			_postTimeColumn.Visible = false;

			this.Sort(new TableSortParams(_postTimeColumn, false));
		}

		/* this isn't needed right now, because acknowledged notes are never shown.
		private static IconSet GetIsAcknowledgedIcon(bool isAcknowledged)
		{
			return isAcknowledged ? new IconSet("NoteRead.png") : new IconSet("NoteUnread.png");
		}
		*/

		// Creates a semi-colon delimited list of the recipients
		private static string RecipientsList(IEnumerable<StaffSummary> staffRecipients, IEnumerable<StaffGroupSummary> groupRecipients)
		{
			var sb = new StringBuilder();
			const string itemSeparator = ";  ";

			foreach (var staffSummary in staffRecipients)
			{
				if (String.Equals(PersonNameFormat.Format(staffSummary.Name), PersonNameFormat.Format(LoginSession.Current.FullName)))
				{
					sb.Insert(0, "me; ");
				}
				else
				{
					sb.Append(StaffNameAndRoleFormat.Format(staffSummary));
					sb.Append(itemSeparator);
				}
			}

			foreach (var groupSummary in groupRecipients)
			{
				sb.Append(groupSummary.Name);
				sb.Append(itemSeparator);
			}

			return sb.ToString().TrimEnd(itemSeparator.ToCharArray());
		}
	}
}