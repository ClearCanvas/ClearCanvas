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
using System.ServiceModel.Security;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.Automation;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	/// <remarks>
	/// This class is implemented as a desktop tool rather than an application tool in order
	/// to take advantage of the 'UseSynchronizationContext' WCF service behaviour, which
	/// automatically marshals all service request over to the thread on which the service host was
	/// started.
	/// </remarks>

	//[ButtonAction("test", "global-menus/Test/Test Automation Client", "TestClient")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	[DesktopServiceHostPermission(new string[] { AuthorityTokens.Study.Open })]
	public class ViewerAutomationServiceHostTool : DesktopServiceHostTool
	{
		public ViewerAutomationServiceHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			ServiceHost host = new ServiceHost(typeof(ViewerAutomation));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = AutomationNamespace.Value;

			return host;
		}

		private void TestClient()
		{
			SynchronizationContext context = SynchronizationContext.Current;

			//Have to test client on another thread, otherwise there is a deadlock b/c the service is hosted on the main thread.
			ThreadPool.QueueUserWorkItem(delegate
											{
												try
												{
													using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
													{
														client.GetViewers(new GetViewersRequest());
													}

													context.Post(delegate
																	{
																		base.Context.DesktopWindow.ShowMessageBox("Success!", MessageBoxActions.Ok);
																	}, null);
												}
												catch (Exception e)
												{
													context.Post(delegate
													{
														base.Context.DesktopWindow.ShowMessageBox(e.Message, MessageBoxActions.Ok);
													}, null);

												}
											});

		}
	}
}
