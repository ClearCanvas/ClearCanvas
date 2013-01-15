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
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class StaffGroupLookupHandler : LookupHandler<StaffGroupTextQueryRequest, StaffGroupSummary>
	{
		private readonly DesktopWindow _desktopWindow;
		private readonly bool _electiveGroupsOnly;

		public StaffGroupLookupHandler(DesktopWindow desktopWindow, bool electiveGroupsOnly)
		{
			_desktopWindow = desktopWindow;
			_electiveGroupsOnly = electiveGroupsOnly;
		}

		public override bool ResolveNameInteractive(string query, out StaffGroupSummary result)
		{
			result = null;

			var staffComponent = new StaffGroupSummaryComponent(true, query, _electiveGroupsOnly);
			staffComponent.IncludeDeactivatedItems = this.IncludeDeactivatedItems;
			var exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, staffComponent, SR.TitleStaffGroups);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = (StaffGroupSummary)CollectionUtils.FirstElement(staffComponent.SummarySelection.Items);
			}

			return (result != null);
		}

		public override string FormatItem(StaffGroupSummary item)
		{
			return item.Name;
		}

		protected override TextQueryResponse<StaffGroupSummary> DoQuery(StaffGroupTextQueryRequest request)
		{
			request.ElectiveGroupsOnly = _electiveGroupsOnly;

			TextQueryResponse<StaffGroupSummary> response = null;
			Platform.GetService<IStaffGroupAdminService>(
				service => response = service.TextQuery(request));
			return response;
		}
	}
}