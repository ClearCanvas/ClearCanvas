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
using System.Security.Policy;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	internal class DicomExplorerComponent : SplitComponentContainer
	{
		private static readonly object _syncLock = new object();
		private static readonly List<DicomExplorerComponent> _activeComponents = new List<DicomExplorerComponent>();

		private ServerTreeComponent _serverTreeComponent;
		private IStudyBrowserComponent _studyBrowserComponent;
		private ISearchPanelComponent _searchPanelComponent;

		private DicomExplorerComponent(SplitPane pane1, SplitPane pane2)
			: base(pane1, pane2, Desktop.SplitOrientation.Horizontal)
		{
		}

		public ServerTreeComponent ServerTreeComponent
		{
			get { return _serverTreeComponent; }
		}

		public IStudyBrowserComponent StudyBrowserComponent
		{
			get { return _studyBrowserComponent; }
		}

		public ISearchPanelComponent SearchPanelComponent
		{
			get { return _searchPanelComponent; }
		}

		public override void Start()
		{
			base.Start();

			lock (_syncLock)
			{
				_activeComponents.Add(this);
			}

			_searchPanelComponent.SearchRequested += OnSearchPanelComponentSearchRequested;
			_searchPanelComponent.SearchCancelled += OnSearchPanelComponentSearchCancelled;
			_studyBrowserComponent.SearchStarted += OnStudyBrowserComponentSearchStarted;
			_studyBrowserComponent.SearchEnded += OnStudyBrowserComponentSearchCompleted;

			// if initially selected server is local, begin an initial dicom query
			if (_serverTreeComponent.ShowLocalServerNode && _serverTreeComponent.SelectedServers.IsLocalServer)
			{
				try
				{
					_studyBrowserComponent.Search(new StudyRootStudyIdentifier());
				}
				catch (PolicyException)
				{
					//TODO: ignore this on startup or show message?
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Host.DesktopWindow);
				}
			}
		}

		public override void Stop()
		{
			_searchPanelComponent.SearchRequested -= OnSearchPanelComponentSearchRequested;
			_searchPanelComponent.SearchCancelled -= OnSearchPanelComponentSearchCancelled;
			_studyBrowserComponent.SearchStarted -= OnStudyBrowserComponentSearchStarted;
			_studyBrowserComponent.SearchEnded -= OnStudyBrowserComponentSearchCompleted;

			lock (_syncLock)
			{
				_activeComponents.Remove(this);
			}

			base.Stop();
		}

		public static List<DicomExplorerComponent> GetActiveComponents()
		{
			lock (_syncLock)
			{
				return new List<DicomExplorerComponent>(_activeComponents);
			}
		}

		public static DicomExplorerComponent Create()
		{
		    var serverTreeComponent = new ServerTreeComponent {ShowLocalServerNode = StudyStore.IsSupported};

		    bool hasEditPermission = PermissionsHelper.IsInRole(AuthorityTokens.Configuration.MyServers);
			serverTreeComponent.IsReadOnly = !hasEditPermission;

			var studyBrowserComponent 
                = CreateComponentFromExtensionPoint<StudyBrowserComponentExtensionPoint, IStudyBrowserComponent>() 
                ?? new StudyBrowserComponent();

			serverTreeComponent.SelectedServerChanged +=
				delegate { studyBrowserComponent.SelectedServers = serverTreeComponent.SelectedServers; };

			var searchPanelComponent 
                = CreateComponentFromExtensionPoint<SearchPanelComponentExtensionPoint, ISearchPanelComponent>()
				?? new SearchPanelComponent();
			SelectPriorsServerNode(serverTreeComponent);

			var leftPane = new SplitPane(SR.TitleServerTreePane, serverTreeComponent, 0.25f);
			var rightPane = new SplitPane(SR.TitleStudyBrowserPane, studyBrowserComponent, 0.75f);

			var bottomContainer =
				new SplitComponentContainer(
				leftPane,
				rightPane,
				SplitOrientation.Vertical);

			var topPane = new SplitPane(SR.TitleSearchPanelPane, searchPanelComponent, true);
			var bottomPane = new SplitPane(SR.TitleStudyNavigatorPane, bottomContainer, false);

		    var component = new DicomExplorerComponent(topPane, bottomPane)
		                        {
		                            _studyBrowserComponent = studyBrowserComponent,
		                            _searchPanelComponent = searchPanelComponent,
		                            _serverTreeComponent = serverTreeComponent
		                        };
		    return component;
		}

		private void OnStudyBrowserComponentSearchStarted(object sender, EventArgs e)
		{
			_searchPanelComponent.SearchInProgress = true;
			_serverTreeComponent.IsEnabled = false;
		}

		private void OnStudyBrowserComponentSearchCompleted(object sender, EventArgs e)
		{
			_searchPanelComponent.SearchInProgress = false;
			_serverTreeComponent.IsEnabled = true;
		}

		private void OnSearchPanelComponentSearchRequested(object sender, SearchRequestedEventArgs e)
		{
			try
			{
				_studyBrowserComponent.Search(e.QueryCriteria);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Host.DesktopWindow);
			}
		}

		private void OnSearchPanelComponentSearchCancelled(object sender, EventArgs e)
		{
			try
			{
				_studyBrowserComponent.CancelSearch();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Host.DesktopWindow);
			}
		}

		internal void SelectPriorsServers()
		{
			SelectPriorsServers(_serverTreeComponent);
		}

		private static void SelectPriorsServerNode(ServerTreeComponent serverTreeComponent)
		{
			if (serverTreeComponent.ShowLocalServerNode && !DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup)
			{
				serverTreeComponent.SetSelection(serverTreeComponent.ServerTree.LocalServer);
			}
			else
			{
				SelectPriorsServers(serverTreeComponent);
			}
		}

        private static void SelectPriorsServers(ServerTreeComponent serverTreeComponent)
		{
            ServerTree serverTree = serverTreeComponent.ServerTree;

            var priorsServers = ServerDirectory.GetPriorsServers(false);
            CheckPriorsServers(serverTree, priorsServers);
            IServerTreeNode initialSelection = GetFirstPriorsServerOrGroup(serverTree.RootServerGroup);
            UncheckAllServers(serverTree);

            if (initialSelection == null)
            {
                if (serverTreeComponent.ShowLocalServerNode)
                    initialSelection = serverTreeComponent.ServerTree.LocalServer;
                else
                    initialSelection = serverTreeComponent.ServerTree.RootServerGroup;
            }

            serverTreeComponent.SetSelection(initialSelection);
		}

        private static IServerTreeNode GetFirstPriorsServerOrGroup(IServerTreeGroup serverGroup)
        {
            if (serverGroup.IsEntireGroupChecked())
                return serverGroup;

            //consider groups and servers at this level
            foreach (IServerTreeGroup group in serverGroup.ChildGroups)
            {
                if (group.IsEntireGroupChecked())
                    return group;
            }

            foreach (IServerTreeDicomServer server in serverGroup.Servers)
            {
                if (server.IsChecked)
                    return server;
            }

            //repeat for children of the groups at this level
            foreach (IServerTreeGroup group in serverGroup.ChildGroups)
            {
                IServerTreeNode priorsServerOrGroup = GetFirstPriorsServerOrGroup(group);
                if (priorsServerOrGroup != null)
                    return priorsServerOrGroup;
            }

            return null;
        }

        private static void CheckPriorsServers(ServerTree serverTree, IList<IDicomServiceNode> priorsServers)
        {
            foreach (var server in serverTree.RootServerGroup.GetAllServers())
            {
                var treeServer = server;
                if (priorsServers.Any(s => s.Name == treeServer.Name))
                    server.IsChecked = true;
            }
        }

        private static void UncheckAllServers(ServerTree serverTree)
        {
            foreach (IServerTreeDicomServer server in serverTree.RootServerGroup.GetAllServers())
                server.IsChecked = false;
        }

		private static TComponent CreateComponentFromExtensionPoint<TExtensionPoint, TComponent>()
			where TExtensionPoint : IExtensionPoint, new()
			where TComponent : class, IApplicationComponent
		{
			try
			{
				var xp = new TExtensionPoint();
				return (TComponent)xp.CreateExtension();
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
