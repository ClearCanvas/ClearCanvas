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
using System.Reflection;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// External Practitioner table used by the Admin summary page.
	/// </summary>
	public class ExternalPractitionerAdminTable : Table<ExternalPractitionerSummary>
	{
		public ExternalPractitionerAdminTable()
		{
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnFamilyName
				, item => item.Name.FamilyName, 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnGivenName,
				item => item.Name.GivenName, 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
				item => item.LicenseNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
				item => item.BillingNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, bool>(SR.ColumnVerified,
				item => item.IsVerified, 0.25f));
		}
	}

	/// <summary>
	/// External Practitioner table used by the folder system.
	/// </summary>
	public class ExternalPractitionerWorkflowTable : Table<ExternalPractitionerSummary>
	{
		private readonly ITableColumn _timeColumn;
		private PropertyInfo _timeInfo;

		public ExternalPractitionerWorkflowTable()
		{

			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColulmnPractitionerName
				, item => PersonNameFormat.Format(item.Name), 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
				item => item.LicenseNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
				item => item.BillingNumber, 0.5f));
			this.Columns.Add(_timeColumn = new DateTimeTableColumn<ExternalPractitionerSummary>(SR.ColumnTime,
				item => _timeInfo == null ? item.LastVerifiedTime : (DateTime?)_timeInfo.GetValue(item, null), 0.75f));

			// Perform initial sort
			this.Sort(new TableSortParams(_timeColumn, true));
		}

		public string PropertyNameForTimeColumn
		{
			get { return _timeInfo == null ? null : _timeInfo.Name; }
			set
			{
				// Get the field info that matches the sortByTimeFieldName.  Make sure the field name is actually a date time type
				var info = string.IsNullOrEmpty(value) ? null : typeof(ExternalPractitionerSummary).GetProperty(value);
				if (info != null && info.PropertyType.Equals(typeof (DateTime?)))
					_timeInfo = info;
			}
		}

		public bool SortAscending
		{
			get { return this.SortParams == null ? true : this.SortParams.Ascending; }
			set
			{
				if (this.SortParams == null)
				{
					this.Sort(new TableSortParams(_timeColumn, value));
				}
				else
				{
					var newSortParams = new TableSortParams(this.SortParams.Column, value);
					this.Sort(newSortParams);
				}
			}
		}

	}
}
