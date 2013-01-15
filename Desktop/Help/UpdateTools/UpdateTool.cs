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
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Help;
using ClearCanvas.Desktop.Help.UpdateTools.UpdateInformationService;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help.UpdateTools
{
	[MenuAction("checkForUpdates", "global-menus/MenuHelp/MenuCheckForUpdates", "CheckForUpdates")]
	[GroupHint("checkForUpdates", "Application.Help.Updates")]

	[Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class UpdateTool : Tool<IDesktopToolContext>
	{
		public UpdateTool()
		{
		}

		public void CheckForUpdates()
		{
			using (var service = new UpdateInformationService.UpdateInformationService())
			{
				service.Url = Settings.Default.UpdateInformationServiceUrl;

				var installedProduct = new Product
				                               	{
				                               		Name = ProductInformation.Component,
				                               		Version = ProductInformation.Version.ToString(),
				                               		VersionSuffix = ProductInformation.VersionSuffix,
				                               		Edition = ProductInformation.Edition,
				                               		Release = ProductInformation.Release
				                               	};

				try
				{
					var request = new UpdateInformationRequest {InstalledProduct = installedProduct};
					UpdateInformationResult result = service.GetUpdateInformation(request);
					if (result == null)
						throw new Exception("Bad data received from service.");

					if (!IsValidComponent(result.InstalledProduct) || IsSameComponent(result.InstalledProduct, installedProduct))
					{
						base.Context.DesktopWindow.ShowMessageBox(SR.MessageNoUpdate, MessageBoxActions.Ok);
					}
					else
					{
						var upgrade = result.InstalledProduct;
						string message = String.Format(SR.MessageUpdateAvailable, ToString(upgrade));
						UpdateAvailableForm.Show(message, result.DownloadUrl);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "The request for update information failed.");
					Context.DesktopWindow.ShowMessageBox(SR.MessageUpdateRequestFailed, MessageBoxActions.Ok);
				}
			}
		}

		private string ToString(Component component)
		{
			var builder = new StringBuilder();

			builder.Append(String.IsNullOrEmpty(component.Name) ? "Unknown" : component.Name);
			builder.AppendFormat(" {0}", String.IsNullOrEmpty(component.Version) ? "?" : component.Version);
			if (!String.IsNullOrEmpty(component.Edition))
				builder.AppendFormat(" {0}", component.Edition);

			if (!String.IsNullOrEmpty(component.VersionSuffix))
				builder.AppendFormat(" {0}", component.VersionSuffix);

			if (!String.IsNullOrEmpty(component.Release))
				builder.AppendFormat(" {0}", component.Release);

			return builder.ToString();

		}

		private bool IsSameComponent(Component component1, Component component2)
		{
			return component1.Name == component2.Name && component1.Version == component2.Version;
		}

		private static bool IsValidComponent(Component component)
		{
			return component != null && !String.IsNullOrEmpty(component.Version);
		}
	}
}
