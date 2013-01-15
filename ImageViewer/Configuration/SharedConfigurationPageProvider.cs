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
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof(SharedConfigurationPageProviderExtensionPoint))]
	[ExtensionOf(typeof(ActivityMonitorQuickLinkHandlerExtensionPoint))]
	public class SharedConfigurationPageProvider : IConfigurationPageProvider, IActivityMonitorQuickLinkHandler
	{
        public const string LocalConfigurationPath = "LocalConfiguration";
        public const string ServerConfigurationPath = LocalConfigurationPath + @"/" + "ServerConfiguration";
		public const string DicomSendConfigurationPath = LocalConfigurationPath + @"/" + "DicomSendConfiguration";
		public const string StorageConfigurationPath = LocalConfigurationPath + @"/" + "StorageConfiguration";
        public const string PriorsServerConfigurationPath = LocalConfigurationPath + @"/" + "PriorsServersConfiguration";
        public const string PublishingConfigurationPath = "PublishingConfiguration";

		#region IConfigurationPageProvider Members

		IEnumerable<IConfigurationPage> IConfigurationPageProvider.GetPages()
		{
			var listPages = new List<IConfigurationPage>();

            if (PermissionsHelper.IsInRole(AuthorityTokens.Configuration.DicomServer) && Common.DicomServer.DicomServer.IsSupported)
				listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>(ServerConfigurationPath));

            if (PermissionsHelper.IsInRole(AuthorityTokens.Configuration.DicomServer) && Common.DicomServer.DicomServer.IsSupported)
				listPages.Add(new ConfigurationPage<DicomSendConfigurationComponent>(DicomSendConfigurationPath));

            if (PermissionsHelper.IsInRole(AuthorityTokens.Configuration.Storage) && Common.StudyManagement.StudyStore.IsSupported)
                listPages.Add(new ConfigurationPage<StorageConfigurationComponent>(StorageConfigurationPath));

            if (PermissionsHelper.IsInRole(AuthorityTokens.Configuration.Publishing))
                listPages.Add(new ConfigurationPage(PublishingConfigurationPath, new PublishingConfigurationComponent()));

            return listPages.AsReadOnly();
		}

		#endregion

		bool IActivityMonitorQuickLinkHandler.CanHandle(ActivityMonitorQuickLink link)
		{
            //Don't check SharedConfigurationDialog.CanShow because we want to show a permission message.
		    return (link == ActivityMonitorQuickLink.SystemConfiguration) ||
                   (link == ActivityMonitorQuickLink.LocalStorageConfiguration && PermissionsHelper.IsInRole(AuthorityTokens.Configuration.Storage));
		}

		void IActivityMonitorQuickLinkHandler.Handle(ActivityMonitorQuickLink link, IDesktopWindow window)
		{
		    try
		    {
                if (link == ActivityMonitorQuickLink.SystemConfiguration)
                {
                    SharedConfigurationDialog.Show(window);
                }
                if (link == ActivityMonitorQuickLink.LocalStorageConfiguration)
                {
                    SharedConfigurationDialog.Show(window, StorageConfigurationPath);
                }
		    }
		    catch (Exception e)
		    {
		        ExceptionHandler.Report(e, window);
		    }
		}
	}
}
