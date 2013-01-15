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

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class PatientNoteTable : Table<PatientNoteDetail>
	{
		private const int NumRows = 2;
		private const int NoteCommentRow = 1;

		public PatientNoteTable()
			: base(NumRows)
		{
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnSeverity,
				n => (n.Category == null ? "" : n.Category.Severity.Value), 0.1f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnCategory,
				n => (n.Category == null ? "" : n.Category.Name), 0.2f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnAuthor,
				n => n.Author == null ? SR.LabelMe : PersonNameFormat.Format(n.Author.Name), 0.2f));

			ITableColumn _createdOnColumn;
			this.Columns.Add(_createdOnColumn = new TableColumn<PatientNoteDetail, string>(SR.ColumnCreatedOn,
				n => n.CreationTime == null ? SR.LabelNew : Format.DateTime(n.CreationTime), 0.2f));
			this.Columns.Add(new DateTableColumn<PatientNoteDetail>(SR.ColumnExpiryDate,
				n => n.ValidRangeUntil, 0.2f));

			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnComments,
				n => RemoveLineBreak(n.Comment), 1.0f, NoteCommentRow));

			// there aren't any items to sort right now, but calling this sets the default sort parameters to "Created" column desc
			this.Sort(new TableSortParams(_createdOnColumn, false));
		}

		private static string RemoveLineBreak(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			var newString = input.Replace("\r\n", " ");
			newString = newString.Replace("\r", " ");
			newString = newString.Replace("\n", " ");
			return newString;
		}
	}
}
