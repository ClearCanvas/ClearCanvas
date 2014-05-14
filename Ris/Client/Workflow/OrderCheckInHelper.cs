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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public static class OrderCheckInHelper
	{
		public static bool CheckIn(EntityRef orderRef, string title, IDesktopWindow desktopWindow)
		{
			List<ProcedureSummary> procedures = null;
			Platform.GetService((IRegistrationWorkflowService service) =>
				procedures = service.ListProceduresForCheckIn(new ListProceduresForCheckInRequest(orderRef)).Procedures);

			if(procedures.Count == 0)
			{
				desktopWindow.ShowMessageBox(SR.MessageNoProceduresCanBeCheckedIn, MessageBoxActions.Ok);
				return false;
			}

			var checkInComponent = new CheckInOrderComponent(procedures);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				desktopWindow,
				checkInComponent,
				title);

			return (exitCode == ApplicationComponentExitCode.Accepted);
		}
	}
}
