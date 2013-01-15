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
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	//[ButtonAction("test", "global-menus/Test/Test Study Locator Client", "TestClient")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class StudyLocatorServiceHostTool : DesktopServiceHostTool
	{
		public StudyLocatorServiceHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			ServiceHost host = new ServiceHost(typeof(Configuration.StudyLocator));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = QueryNamespace.Value;

			return host;
		}

		private void TestClient()
		{
			try
			{
				using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
				{
					bridge.QueryByAccessionNumber("test");
				}

				base.Context.DesktopWindow.ShowMessageBox("Success!", MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				base.Context.DesktopWindow.ShowMessageBox(e.Message, MessageBoxActions.Ok);
			}
		}
	}
}
