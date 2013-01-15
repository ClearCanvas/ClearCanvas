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
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class StudyLocatorServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IStudyRootQuery))
			{
				if (AppDomain.CurrentDomain == DesktopServiceHostTool.HostAppDomain)
				{
					//just return an instance when in the same process/domain.
					return new Configuration.StudyLocator();
				}
				else
				{
					//return the true service client.
					return new StudyRootQueryServiceClient();
				}
			}
			else if (serviceType == typeof(IStudyLocator))
			{
				if (AppDomain.CurrentDomain == DesktopServiceHostTool.HostAppDomain)
				{
					//just return an instance when in the same process/domain.
					return new Configuration.StudyLocator();
				}
				else
				{
					//return the true service client.
					return new StudyLocatorServiceClient();
				}
			}

			return null;
		}

		#endregion
	}
}