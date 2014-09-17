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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client
{
	public static class OrderCancelHelper
	{
		public static bool CancelOrder(EntityRef orderRef, string description, IDesktopWindow desktopWindow)
		{
			// first check for warnings
			QueryCancelOrderWarningsResponse response = null;
			Platform.GetService<IOrderEntryService>(
				service => response = service.QueryCancelOrderWarnings(new QueryCancelOrderWarningsRequest(orderRef)));

			if (response.Errors != null && response.Errors.Count > 0)
			{
				var error = CollectionUtils.FirstElement(response.Errors);
				desktopWindow.ShowMessageBox(error, MessageBoxActions.Ok);
				return false;
			}

			if (response.Warnings != null && response.Warnings.Count > 0)
			{
				var warn = CollectionUtils.FirstElement(response.Warnings);
				var action = desktopWindow.ShowMessageBox(
					warn + "\n\n" + SR.MessageConfirmCancelOrder,
					MessageBoxActions.YesNo);
				if (action == DialogBoxAction.No)
					return false;
			}

			var cancelOrderComponent = new CancelOrderComponent(orderRef);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				desktopWindow,
				cancelOrderComponent,
				String.Format(SR.TitleCancelOrder, description));

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				Platform.GetService<IOrderEntryService>(
					service => service.CancelOrder(new CancelOrderRequest(orderRef, cancelOrderComponent.SelectedCancelReason)));

				return true;
			}
			return false;
		}
	}
}
