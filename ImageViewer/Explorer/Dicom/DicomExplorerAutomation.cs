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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.Automation;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.DesktopServices;
using System.Threading;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	#region Hosting

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class DicomExplorerAutomationServiceHostTool : DesktopServiceHostTool
	{
		public DicomExplorerAutomationServiceHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			ServiceHost host = new ServiceHost(typeof(DicomExplorerAutomation));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = AutomationNamespace.Value;

			return host;
		}
	}

	#endregion

	//Note: should the need arise, we could later allow the different explorers to be enumerated, but right now it's not necessary.

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = true, ConfigurationName = "DicomExplorerAutomation", Namespace = AutomationNamespace.Value)]
	public class DicomExplorerAutomation : IDicomExplorerAutomation
	{
		#region IDicomExplorerAutomation Members

		public SearchLocalStudiesResult SearchLocalStudies(SearchLocalStudiesRequest request)
		{
			if (request == null)
				throw new FaultException("The request cannot be null.");
			
			if (!StudyStore.IsSupported)
				throw new FaultException<NoLocalStoreFault>(new NoLocalStoreFault(), "No local store was found.");

			DicomExplorerComponent explorerComponent = GetDicomExplorer();

			if (request.SearchCriteria == null)
				request.SearchCriteria = new DicomExplorerSearchCriteria();

			//Select the local server node.
			explorerComponent.ServerTreeComponent.SetSelection(explorerComponent.ServerTreeComponent.ServerTree.LocalServer);

			SynchronizationContext.Current.Post(
			    ignore => explorerComponent.StudyBrowserComponent.Search(request.SearchCriteria.ToIdentifier(true)), null); 

			return new SearchLocalStudiesResult();
		}

		public SearchRemoteStudiesResult SearchRemoteStudies(SearchRemoteStudiesRequest request)
		{
			if (request == null)
				throw new FaultException("The request cannot be null.");

			DicomExplorerComponent explorerComponent = GetDicomExplorer();

			if (request.SearchCriteria == null)
				request.SearchCriteria = new DicomExplorerSearchCriteria();

			string aeTitle = (request.AETitle ?? "").Trim();
			if (String.IsNullOrEmpty(aeTitle))
			{
				explorerComponent.SelectPriorsServers();
			}
			else
			{
			    var server = explorerComponent.ServerTreeComponent.ServerTree.RootServerGroup
                    .GetAllServers().OfType<IServerTreeDicomServer>().FirstOrDefault(s => s.AETitle == aeTitle);
				if (server == null)
					throw new FaultException<ServerNotFoundFault>(new ServerNotFoundFault(), String.Format("Server '{0}' not found.", aeTitle));

				explorerComponent.ServerTreeComponent.SetSelection(server);
			}

			SynchronizationContext.Current.Post(
			    ignore => explorerComponent.StudyBrowserComponent.Search(request.SearchCriteria.ToIdentifier(true)), null); 
			
			return new SearchRemoteStudiesResult();
		}

		#endregion

		private static DicomExplorerComponent GetDicomExplorer()
		{
			List<DicomExplorerComponent> explorerComponents = DicomExplorerComponent.GetActiveComponents();
			if (explorerComponents.Count == 0)
				throw new FaultException<DicomExplorerNotFoundFault>(new DicomExplorerNotFoundFault(), "No dicom explorers were found.");

			IDesktopWindow parentDesktopWindow;
			IDesktopObject parentShelfOrWorkspace;
			GetOwnerWindows(explorerComponents[0], out parentDesktopWindow, out parentShelfOrWorkspace);
			if (parentDesktopWindow != null) //activate the owner, if it was found.
				parentDesktopWindow.Activate();
			if (parentShelfOrWorkspace != null)
				parentShelfOrWorkspace.Activate();

			//there's only ever one of these right now anyway.
			return explorerComponents[0];
		}

		private static void GetOwnerWindows(DicomExplorerComponent explorerComponent, 
			out IDesktopWindow parentDesktopWindow, out IDesktopObject parentShelfOrWorkspace)
		{
			parentDesktopWindow = null;
			parentShelfOrWorkspace = null;

			foreach (IDesktopWindow desktopWindow in Application.DesktopWindows)
			{
				foreach (IWorkspace workspace in desktopWindow.Workspaces)
				{
					if (workspace.Component == explorerComponent)
					{
						parentDesktopWindow = desktopWindow;
						parentShelfOrWorkspace = workspace;
						return;
					}
				}

				foreach (IShelf shelf in desktopWindow.Shelves)
				{
					if (shelf.Component == explorerComponent)
					{
						parentDesktopWindow = desktopWindow;
						parentShelfOrWorkspace = shelf;
						return;
					}
				}
			}
		}
	}
}
