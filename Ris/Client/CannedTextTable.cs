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
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Client
{
	public class CannedTextTable : Table<CannedTextSummary>
	{
		public CannedTextTable()
		{
			this.Columns.Add(new TableColumn<CannedTextSummary, string>("Name", SR.ColumnCannedTextName, c => c.Name, 1.0f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>("Category", SR.ColumnCannedTextCategory, c => c.Category, 1.0f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>("Text", SR.ColumnCannedTextText, c => FormatCannedTextSnippet(c.TextSnippet), 3.0f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>("Owner", SR.ColumnCannedTextOwner,
			                                                            item => item.IsPersonal ? SR.ColumnPersonal : item.StaffGroup.Name, 1.0f));

			// Apply sort from settings
			var sortColumnIndex = this.Columns.FindIndex(column => column.Name.Equals(CannedTextSettings.Default.SummarySortColumnName));
			if (sortColumnIndex < 0) sortColumnIndex = 0;
			this.Sort(new TableSortParams(this.Columns[sortColumnIndex], CannedTextSettings.Default.SummarySortAscending));

			this.Sorted += OnCannedTextTableSorted;
		}

		private void OnCannedTextTableSorted(object sender, EventArgs e)
		{
			if (this.SortParams == null)
				return;

			// Save last sort
			CannedTextSettings.Default.SummarySortColumnName = this.SortParams.Column.Name;
			CannedTextSettings.Default.SummarySortAscending = this.SortParams.Ascending;
			CannedTextSettings.Default.Save();
		}

		private static string FormatCannedTextSnippet(string text)
		{
			return text.Length < CannedTextSummary.MaxTextLength ? text : string.Concat(text, "...");
		}
	}
}