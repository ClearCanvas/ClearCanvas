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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides utilities for staff name resolution.
	/// </summary>
	public class StaffLookupHandler : LookupHandler<StaffTextQueryRequest, StaffSummary>
	{
		private readonly DesktopWindow _desktopWindow;
		private readonly string[] _staffTypesFilter;

		public StaffLookupHandler(DesktopWindow desktopWindow)
			: this(desktopWindow, new string[] { })
		{
		}

		public StaffLookupHandler(DesktopWindow desktopWindow, string[] staffTypesFilter)
		{
			_desktopWindow = desktopWindow;
			_staffTypesFilter = staffTypesFilter;
		}

		protected override TextQueryResponse<StaffSummary> DoQuery(StaffTextQueryRequest request)
		{
			if (_staffTypesFilter != null && _staffTypesFilter.Length > 0)
			{
				request.StaffTypesFilter = _staffTypesFilter;
			}

			TextQueryResponse<StaffSummary> response = null;
			Platform.GetService<IStaffAdminService>(
				service => response = service.TextQuery(request));
			return response;
		}

		/// <summary>
		/// Shows a dialog to allow user to resolve the specified query to a single staff.
		/// The query may consist of part of the surname,
		/// optionally followed by a comma and then part of the given name (e.g. "sm, b" for smith, bill).
		/// The method returns true if the name is successfully resolved, or false otherwise.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool ResolveNameInteractive(string query, out StaffSummary result)
		{
			result = null;

			var staffComponent = new StaffSummaryComponent(true, _staffTypesFilter);
			staffComponent.IncludeDeactivatedItems = this.IncludeDeactivatedItems;
			if (!string.IsNullOrEmpty(query))
			{
				var names = query.Split(',');
				if (names.Length > 0)
					staffComponent.LastName = names[0].Trim();
				if (names.Length > 1)
					staffComponent.FirstName = names[1].Trim();
			}

			var exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, staffComponent, SR.TitleStaff);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = (StaffSummary)staffComponent.SummarySelection.Item;
			}

			return (result != null);
		}


		public override string FormatItem(StaffSummary item)
		{
			return StaffNameAndRoleFormat.Format(item);
		}
	}
}
