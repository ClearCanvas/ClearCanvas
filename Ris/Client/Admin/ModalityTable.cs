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

namespace ClearCanvas.Ris.Client.Admin
{
	public class ModalityTable : Table<ModalitySummary>
	{
		public ModalityTable()
		{
			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnID,
				modality => modality.Id, 0.2f));

			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnName,
				modality => modality.Name, 1.0f));

			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnAETitle,
				modality => modality.AETitle, 1.0f));

			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnDicomModality,
				FormatDicomModality, 1.0f));

			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnFacility,
				FormatFacility, 1.0f));
		}

		private string FormatFacility(ModalitySummary modality)
		{
			return modality.Facility == null ? null : modality.Facility.Name;
		}

		private string FormatDicomModality(ModalitySummary modality)
		{
			return modality.DicomModality == null ? null 
				: string.Format("{0} - {1}", modality.DicomModality.Value, modality.DicomModality.Description);
		}
	}
}
