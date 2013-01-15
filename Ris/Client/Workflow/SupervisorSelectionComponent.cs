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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class SupervisorSelectionComponent : StaffSelectionComponent
	{
		protected override string[] StaffTypes
		{
			get
			{
				// create supervisor lookup handler, using filters supplied in application settings
				string filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
				string[] staffTypes = string.IsNullOrEmpty(filters)
										? new string[] { }
										: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); }).ToArray();
				return staffTypes;
			}
		}

		public override string LabelText
		{
			get { return "Supervisor"; }
		}
	}

	public class ReportingSupervisorSelectionComponent : SupervisorSelectionComponent
	{
		protected override string DefaultSupervisorID
		{
			get { return ReportingSettings.Default.SupervisorID; }
		}

		protected override void SetStaff(StaffSummary staff)
		{
			base.SetStaff(staff);
			ReportingSettings.Default.SupervisorID = staff == null ? "" : staff.StaffId;
			ReportingSettings.Default.Save();
		}
	}

}