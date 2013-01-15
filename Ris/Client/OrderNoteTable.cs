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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteTable : Table<OrderNoteDetail>
	{
		private readonly TableColumn<OrderNoteDetail, string> _linkColumn;

		public OrderNoteTable()
			: base(2)
		{
			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnAuthor, n => n.Author == null ? SR.LabelMe : StaffNameAndRoleFormat.Format(n.Author), 0.25f));
			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnPostTime, n => n.PostTime == null ? SR.LabelNew : Format.DateTime(n.PostTime), 0.25f));
			this.Columns.Add(_linkColumn = new TableColumn<OrderNoteDetail, string>(" ", n => SR.ColumnMore, 0.05f));
			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnComments, n => RemoveLineBreak(n.NoteBody), 0.5f, 1));
		}

		public Action<OrderNoteDetail> UpdateNoteClickLinkDelegate
		{
			get { return _linkColumn.ClickLinkDelegate; }
			set { _linkColumn.ClickLinkDelegate = value; }
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
