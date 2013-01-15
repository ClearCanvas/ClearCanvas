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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class BiographyNoteTable : Table<PatientNoteDetail>
	{
		private const int NumRows = 2;
		private const int NoteCommentRow = 1;

		public BiographyNoteComponent _component;

		public BiographyNoteTable(BiographyNoteComponent component)
			: this(component, NumRows)
		{
		}

		private BiographyNoteTable(BiographyNoteComponent component, int cellRowCount)
			: base(cellRowCount)
		{
			_component = component;

			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnSeverity,
				n => (n.Category == null ? "" : n.Category.Severity.Value), 0.06f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnCategory,
				n => (n.Category == null ? "" : n.Category.Name), 0.2f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnDescription,
				n => (n.Category == null ? "" : n.Category.Description), 0.4f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnAuthor,
				n => PersonNameFormat.Format(n.Author.Name), 0.2f));

			ITableColumn _createdOnColumn;
			this.Columns.Add(_createdOnColumn = new DateTimeTableColumn<PatientNoteDetail>(SR.ColumnCreatedOn,
				n => n.CreationTime, 0.1f));
			this.Columns.Add(new DateTableColumn<PatientNoteDetail>(SR.ColumnValidFrom,
				n => n.ValidRangeFrom, 0.1f));
			this.Columns.Add(new DateTableColumn<PatientNoteDetail>(SR.ColumnValidUntil,
				n => n.ValidRangeUntil, 0.1f));

			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(" ",
				n => SR.ColumnMore, 0.05f)
					{
						ClickLinkDelegate = component.ShowNoteDetail
					});

			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnComments,
				n => (string.IsNullOrEmpty(n.Comment) ? "" : String.Format("Comment: {0}", RemoveLineBreak(n.Comment))), 0.1f, NoteCommentRow));

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
