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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	[ExtensionPoint()]
	public sealed class ServerTreeToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint()]
	public sealed class ServerTreeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public interface IServerTreeToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }

		ServerTree ServerTree { get; }

		DicomServiceNodeList SelectedServers { get; }
		event EventHandler SelectedServerChanged;

		bool IsReadOnly { get; }
		int UpdateType { get; set; }
	}

	[AssociateView(typeof(ServerTreeComponentViewExtensionPoint))]
	public class ServerTreeComponent : ApplicationComponent
	{
		public class ServerTreeToolContext : ToolContext, IServerTreeToolContext
		{
			ServerTreeComponent _component;

			public ServerTreeToolContext(ServerTreeComponent component)
			{
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			#region IServerTreeToolContext Members

			public ServerTree ServerTree
			{
				get { return _component._serverTree; }
			}

			public DicomServiceNodeList SelectedServers
			{
				get { return _component.SelectedServers; }
			}

			public event EventHandler SelectedServerChanged
			{
				add { _component.SelectedServerChanged += value; }
				remove { _component.SelectedServerChanged -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public bool IsReadOnly
			{
				get { return _component.IsReadOnly; }
			}

			public int UpdateType
			{
				get { return _component.UpdateType; }
				set { _component.UpdateType = value; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			#endregion
		}

		#region Fields

		private ServerTree _serverTree;
		private event EventHandler _selectedServerChanged;
		private DicomServiceNodeList _selectedServers;
		private int _updateType;
		private ToolSet _toolSet;
		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;
		private ClickHandlerDelegate _defaultActionHandler;
		private bool _showTools = true;
		private bool _showTitlebar = true;
		private bool _showLocalServerNode = true;
		private bool _isReadOnly = false;
		private bool _showCheckBoxes = false;
		private bool _isEnabled = true;

		#endregion


		public ServerTreeComponent()
		{
			_serverTree = new ServerTree();

			if (_serverTree.CurrentNode != null)
			{
                _selectedServers = new DicomServiceNodeList(_serverTree.CurrentNode.ToDicomServiceNodes())
			                           {Name = _serverTree.CurrentNode.DisplayName, Id = _serverTree.CurrentNode.Path};
			}
			else
			{
                _selectedServers = new DicomServiceNodeList();
			}
		}

		#region Presentation Model

		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				if(value != _isEnabled)
				{
					_isEnabled = value;
					NotifyPropertyChanged("IsEnabled");
				}
			}
		}

		public ServerTree ServerTree
		{
			get { return _serverTree; }
		}

		public DicomServiceNodeList SelectedServers
		{
			get { return _selectedServers; }
		}

		public int UpdateType
		{
			get { return _updateType; }
			set { _updateType = value; }
		}

		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelNode ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public bool ShowTools
		{
			get { return _showTools; }
			set { _showTools = value; }
		}

		public bool ShowTitlebar
		{
			get { return _showTitlebar; }
			set { _showTitlebar = value; }
		}

		public bool ShowCheckBoxes
		{
			get { return _showCheckBoxes; }
			set { _showCheckBoxes = value; }
		}

		public bool ShowLocalServerNode
		{
			get { return _showLocalServerNode; }
			set { _showLocalServerNode = value; }
		}

		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set { _isReadOnly = value; }
		}

		#endregion

		public void SetSelection(IServerTreeNode serverTreeNode)
		{
		    _selectedServers = new DicomServiceNodeList(serverTreeNode.ToDicomServiceNodes())
		                           {
		                               Name = serverTreeNode.DisplayName,
		                               Id = serverTreeNode.Path
		                           };

		    _serverTree.CurrentNode = serverTreeNode;
            FireSelectedServerChangedEvent();
		}

		public bool NodeMoved(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
		{
			if (!CanMove(destinationNode, movingDataNode))
				return false;

			if (movingDataNode.IsServer)
			{
				_serverTree.CurrentNode = movingDataNode;
				_serverTree.DeleteServer();

				((IServerTreeGroup)destinationNode).AddChild(movingDataNode);
				SetSelection(movingDataNode);
			}
			else if (movingDataNode.IsServerGroup)
			{
                var movingGroup = (IServerTreeGroup)movingDataNode;
				_serverTree.CurrentNode = movingGroup;
				_serverTree.DeleteGroup();
                _serverTree.Save();

                ((IServerTreeGroup)destinationNode).AddChild(movingGroup);
				SetSelection(movingGroup);
			}
			_serverTree.Save();
			return true;
		}

		public bool CanMove(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
		{
			if (IsReadOnly)
				return false;

			return _serverTree.CanMove(destinationNode, movingDataNode);
		}
		
		public event EventHandler SelectedServerChanged
		{
			add { _selectedServerChanged += value; }
			remove { _selectedServerChanged -= value; }
		}

		private void FireSelectedServerChangedEvent()
		{
			EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
		}

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_toolSet = new ToolSet(new ServerTreeToolExtensionPoint(), new ServerTreeToolContext(this));
			_toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "servertree-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "servertree-contextmenu", _toolSet.Actions);
		}

		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		public void NodeDoubleClick()
		{
			//Could be something other than edit that does no harm, but we might as well just disallow unconditionally.
			if (IsReadOnly)
				return;
        	
			// according to the framework architecture, the default action handler
			// for this component is set up by the ServerEditTool
			// however, since the tool is used for both Server and ServerGroup
			// and we want to retain the behaviour of expanding the tree node when
			// a ServerGroup is d-clicked, we only want the edit tool invoked if
			// the node is a Server
			if (!_serverTree.CurrentNode.IsLocalServer && 
			    !_serverTree.CurrentNode.IsServerGroup && 
			    null != _defaultActionHandler &&
			    _showTools)
				_defaultActionHandler();
		}
	}

	public enum ServerUpdateType
	{
		None = 0,
		Add,
		Edit,
		Delete
	}
}