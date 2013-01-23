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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionPoint]
	public sealed class ActionModelConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// Interface for the <see cref="ActionModelConfigurationComponent"/>.
	/// </summary>
	public interface IActionModelConfigurationComponent
	{
		/// <summary>
		/// Gets the current action model configuration <see cref="ITree"/>.
		/// </summary>
		ITree ActionModelTreeRoot { get; }

		/// <summary>
		/// Gets a value indicating whether or not the action model configuration tree must be flat.
		/// </summary>
		bool EnforceFlatActionModel { get; }

		/// <summary>
		/// Gets the toolbar action model for the component's view.
		/// </summary>
		ActionModelRoot ToolbarActionModel { get; }

		/// <summary>
		/// Gets the context menu action model for the component's view.
		/// </summary>
		ActionModelRoot ContextMenuActionModel { get; }

		/// <summary>
		/// Gets or sets the currently selected node in the <see cref="ActionModelTreeRoot"/>.
		/// </summary>
		AbstractActionModelTreeNode SelectedNode { get; set; }

		/// <summary>
		/// Fired when the value of <see cref="SelectedNode"/> changes.
		/// </summary>
		event EventHandler SelectedNodeChanged;

		/// <summary>
		/// Gets the extended properties components of the currently selected node.
		/// </summary>
		ActionModelConfigurationComponent.INodeProperties SelectedNodeProperties { get; }

		/// <summary>
		/// Gets or sets the current validation policy in effect.
		/// </summary>
		NodePropertiesValidationPolicy ValidationPolicy { get; set; }
	}

	[AssociateView(typeof (ActionModelConfigurationComponentViewExtensionPoint))]
	public partial class ActionModelConfigurationComponent : ApplicationComponent, IConfigurationApplicationComponent, IActionModelConfigurationComponent
	{
		private const string _tolbarActionSite = "actionmodelconfig-toolbar";
		private const string _contextMenuActionSite = "actionmodelconfig-contextmenu";

		private event EventHandler _selectedNodeChanged;

		private readonly IDesktopWindow _desktopWindow;
		private readonly string _namespace;
		private readonly string _site;

		private readonly bool _enforceFlatActionModel;

		private readonly ActionModelRoot _actionModel;
		private readonly AbstractActionModelTreeRoot _actionModelTreeRoot;
		private readonly ActionNodeMapDictionary _actionNodeMapDictionary;

		private ToolSet _toolSet;
		private ActionModelRoot _toolbarActionModel;
		private ActionModelRoot _contextMenuActionModel;

		private AbstractActionModelTreeNode _selectedNode;
		private NodePropertiesComponentContainerHost _propertiesContainerHost;

		private NodePropertiesValidationPolicy _validationPolicy;

		public ActionModelConfigurationComponent(string @namespace, string site, IActionSet actionSet, IDesktopWindow desktopWindow)
			: this(@namespace, site, actionSet, desktopWindow, false) { }

		public ActionModelConfigurationComponent(string @namespace, string site, IActionSet actionSet, IDesktopWindow desktopWindow, bool flatActionModel)
		{
			_namespace = @namespace;
			_site = site;
			_desktopWindow = desktopWindow;

			if (_desktopWindow is DesktopWindow)
			{
				DesktopWindow concreteDesktopWindow = (DesktopWindow) _desktopWindow;
				if (_site == DesktopWindow.GlobalMenus || _site == DesktopWindow.GlobalToolbars)
					actionSet = actionSet.Union(concreteDesktopWindow.DesktopTools.Actions);
			}

			_actionModel = ActionModelSettings.DefaultInstance.BuildAbstractActionModel(_namespace, _site, actionSet.Select(a => a.Path.Site == site));
			_actionModelTreeRoot = new AbstractActionModelTreeRoot(_site);

			_enforceFlatActionModel = flatActionModel;
			if (flatActionModel)
				BuildFlatActionModelTree(_actionModel, _actionModelTreeRoot);
			else
				BuildActionModelTree(_actionModel, _actionModelTreeRoot);

			_actionNodeMapDictionary = new ActionNodeMapDictionary();
			foreach (AbstractActionModelTreeNode node in _actionModelTreeRoot.EnumerateDescendants())
			{
				if (node is AbstractActionModelTreeLeafAction)
				{
					_actionNodeMapDictionary.AddToMap((AbstractActionModelTreeLeafAction)node);
				}
			}
		}

		public bool EnforceFlatActionModel
		{
			get { return _enforceFlatActionModel; }
		}

		public string ActionModelId
		{
			get { return string.Format("{0}:{1}", _namespace, _site); }
		}

		public string ActionModelNamespace
		{
			get { return _namespace; }
		}

		public string ActionModelSite
		{
			get { return _site; }
		}

		ITree IActionModelConfigurationComponent.ActionModelTreeRoot
		{
			get { return _actionModelTreeRoot.Tree; }
		}

		public AbstractActionModelTreeRoot AbstractActionModelTreeRoot
		{
			get { return _actionModelTreeRoot; }
		}

		public IActionNodeMap ActionNodeMap
		{
			get { return _actionNodeMapDictionary; }
		}

		public AbstractActionModelTreeNode SelectedNode
		{
			get { return _selectedNode; }
			set
			{
				if (_selectedNode != value)
				{
					_selectedNode = value;
					this.OnSelectedNodeChanged();
				}
			}
		}

		public NodePropertiesValidationPolicy ValidationPolicy
		{
			get { return _validationPolicy; }
			set { _validationPolicy = value; }
		}

		ActionModelRoot IActionModelConfigurationComponent.ToolbarActionModel
		{
			get { return _toolbarActionModel; }
		}

		ActionModelRoot IActionModelConfigurationComponent.ContextMenuActionModel
		{
			get { return _contextMenuActionModel; }
		}

		INodeProperties IActionModelConfigurationComponent.SelectedNodeProperties
		{
			get { return _propertiesContainerHost.Component; }
		}

		protected virtual void OnSelectedNodeChanged()
		{
			this.DisposeNodePropertiesComponent();
			this.InitializeNodePropertiesComponent();

			EventsHelper.Fire(_selectedNodeChanged, this, EventArgs.Empty);
		}

		public event EventHandler SelectedNodeChanged
		{
			add { _selectedNodeChanged += value; }
			remove { _selectedNodeChanged -= value; }
		}

		public override void Start()
		{
			base.Start();
			this.InitializeNodePropertiesComponent();

			_toolSet = new ToolSet(new ActionModelConfigurationComponentToolExtensionPoint(), new ActionModelConfigurationComponentToolContext(this));
			_toolbarActionModel = ActionModelRoot.CreateModel(this.GetType().FullName, _tolbarActionSite, _toolSet.Actions);
			_contextMenuActionModel = ActionModelRoot.CreateModel(this.GetType().FullName, _contextMenuActionSite, _toolSet.Actions);

			_actionModelTreeRoot.NodeValidationRequested += OnActionModelTreeRootNodeValidationRequested;
			_actionModelTreeRoot.NodeValidated += OnActionModelTreeRootNodeValidated;
		}

		public override void Stop()
		{
			_actionModelTreeRoot.NodeValidationRequested -= OnActionModelTreeRootNodeValidationRequested;
			_actionModelTreeRoot.NodeValidated -= OnActionModelTreeRootNodeValidated;

			_contextMenuActionModel = null;
			_toolbarActionModel = null;
			_toolSet.Dispose();

			this.DisposeNodePropertiesComponent();
			base.Stop();
		}

		public virtual void Save()
		{
			ActionModelRoot actionModelRoot = _actionModelTreeRoot.GetAbstractActionModel();
            ActionModelSettings.DefaultInstance.PersistAbstractActionModel(_namespace, _site, actionModelRoot);

			if (_desktopWindow is DesktopWindow)
			{
				DesktopWindow concreteDesktopWindow = (DesktopWindow) _desktopWindow;
				if (_site == DesktopWindow.GlobalMenus || _site == DesktopWindow.GlobalToolbars)
					concreteDesktopWindow.UpdateView();
			}
		}

		protected virtual IEnumerable<NodePropertiesComponent> CreateNodePropertiesComponents(AbstractActionModelTreeNode node)
		{
			if (node != null)
			{
				foreach (object extension in new NodePropertiesComponentProviderExtensionPoint().CreateExtensions())
				{
					INodePropertiesComponentProvider provider = extension as INodePropertiesComponentProvider;
					if (provider != null)
					{
						foreach (NodePropertiesComponent component in provider.CreateComponents(node))
							yield return component;
					}
				}
			}
		}

		protected virtual bool OnRequestNodePropertiesValidation(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			if (_validationPolicy == null)
				return true;
			return _validationPolicy.Validate(node, propertyName, value);
		}

		protected virtual void OnNodePropertiesValidated(AbstractActionModelTreeNode node, string propertyName, object value) {}

		private void OnActionModelTreeRootNodeValidationRequested(object sender, NodeValidationRequestedEventArgs e)
		{
			e.IsValid = OnRequestNodePropertiesValidation(e.Node, e.PropertyName, e.Value);
		}

		private void OnActionModelTreeRootNodeValidated(object sender, NodeValidatedEventArgs e)
		{
			OnNodePropertiesValidated(e.Node, e.PropertyName, e.Value);
		}

		private void InitializeNodePropertiesComponent()
		{
			_propertiesContainerHost = new NodePropertiesComponentContainerHost(this);
			_propertiesContainerHost.StartComponent();
		}

		private void DisposeNodePropertiesComponent()
		{
			if (_propertiesContainerHost != null)
			{
				_propertiesContainerHost.StopComponent();
				_propertiesContainerHost = null;
			}
		}

		private static void BuildFlatActionModelTree(ActionModelNode actionModel, AbstractActionModelTreeBranch abstractActionModelTreeBranch)
		{
			foreach (ActionModelNode childNode in actionModel.GetLeafNodesInOrder())
			{
				if (childNode is ActionNode)
				{
					ActionNode actionNode = (ActionNode) childNode;
					if (actionNode.Action.Persistent)
					{
						if (actionNode.Action is IClickAction)
							abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafClickAction((IClickAction) actionNode.Action));
						else
							abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafAction(actionNode.Action));
					}
				}
				else if (childNode is SeparatorNode)
				{
					abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafSeparator());
				}
			}
		}

		private static void BuildActionModelTree(ActionModelNode actionModel, AbstractActionModelTreeBranch abstractActionModelTreeBranch)
		{
			foreach (ActionModelNode childNode in actionModel.ChildNodes)
			{
				if (childNode is ActionNode)
				{
					ActionNode actionNode = (ActionNode) childNode;
					if (actionNode.Action.Persistent)
					{
						if (actionNode.Action is IClickAction)
							abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafClickAction((IClickAction) actionNode.Action));
						else
							abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafAction(actionNode.Action));
					}
				}
				else if (childNode is SeparatorNode)
				{
					abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafSeparator());
				}
				else if (childNode is BranchNode)
				{
					AbstractActionModelTreeBranch treeBranch = new AbstractActionModelTreeBranch(childNode.PathSegment);
					BuildActionModelTree(childNode, treeBranch);
					abstractActionModelTreeBranch.AppendChild(treeBranch);
				}
			}
		}

		#region IActionNodeMap Interface

		public interface IActionNodeMap
		{
			IEnumerable<AbstractActionModelTreeLeafAction> this[string actionId] { get; }
			IEnumerable<AbstractActionModelTreeLeafAction> ActionNodes { get; }
			IEnumerable<string> ActionIds { get; }
		}

		#endregion

		#region ActionNodeMapDictionary Class

		//TODO (CR Sept 2010): MapDictionary?
		private class ActionNodeMapDictionary : IActionNodeMap
		{
			private readonly Dictionary<string, IList<AbstractActionModelTreeLeafAction>> _actionMap = new Dictionary<string, IList<AbstractActionModelTreeLeafAction>>();

			public IEnumerable<AbstractActionModelTreeLeafAction> this[string actionId]
			{
				get
				{
					if (!_actionMap.ContainsKey(actionId))
						throw new KeyNotFoundException();
					return _actionMap[actionId];
				}
			}

			public IEnumerable<AbstractActionModelTreeLeafAction> ActionNodes
			{
				get
				{
					foreach (IList<AbstractActionModelTreeLeafAction> list in _actionMap.Values)
					{
						foreach (AbstractActionModelTreeLeafAction action in list)
							yield return action;
					}
				}
			}

			public IEnumerable<string> ActionIds
			{
				get { return _actionMap.Keys; }
			}

			public void AddToMap(AbstractActionModelTreeLeafAction actionNode)
			{
				if (!_actionMap.ContainsKey(actionNode.ActionId))
					_actionMap.Add(actionNode.ActionId, new List<AbstractActionModelTreeLeafAction>());
				_actionMap[actionNode.ActionId].Add(actionNode);
			}
		}

		#endregion

		#region INodeProperties Interface

		public interface INodeProperties
		{
			IEnumerable<IApplicationComponent> Components { get; }
			IEnumerable<IApplicationComponentView> ComponentViews { get; }
		}

		#endregion

		#region NodePropertiesComponentContainerHost Class

		private class NodePropertiesComponentContainerHost : ApplicationComponentHost
		{
			private readonly ActionModelConfigurationComponent _owner;

			public NodePropertiesComponentContainerHost(ActionModelConfigurationComponent owner)
				: base(new NodePropertiesComponentContainer(owner.CreateNodePropertiesComponents(owner.SelectedNode)))
			{
				_owner = owner;
			}

			public new NodePropertiesComponentContainer Component
			{
				get { return (NodePropertiesComponentContainer) base.Component; }
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _owner.Host.DesktopWindow; }
			}
		}

		#endregion

		#region NodePropertiesComponentContainer Class

		private class NodePropertiesComponentContainer : ApplicationComponentContainer, INodeProperties
		{
			private readonly List<ContainedComponentHost> _componentHosts = new List<ContainedComponentHost>();

			internal NodePropertiesComponentContainer(IEnumerable<NodePropertiesComponent> propertiesComponents)
			{
				Platform.CheckForNullReference(propertiesComponents, "propertiesComponents");
				foreach (NodePropertiesComponent component in propertiesComponents)
					_componentHosts.Add(new ContainedComponentHost(this, component));
			}

			public IEnumerable<IApplicationComponent> Components
			{
				get
				{
					foreach (ContainedComponentHost host in _componentHosts)
						yield return host.Component;
				}
			}

			public IEnumerable<IApplicationComponentView> ComponentViews
			{
				get
				{
					foreach (ContainedComponentHost host in _componentHosts)
						yield return host.ComponentView;
				}
			}

			public override IEnumerable<IApplicationComponent> ContainedComponents
			{
				get { return this.Components; }
			}

			public override IEnumerable<IApplicationComponent> VisibleComponents
			{
				get { return this.Components; }
			}

			public override void Start()
			{
				base.Start();
				foreach (ContainedComponentHost host in _componentHosts)
					host.StartComponent();
			}

			public override void Stop()
			{
				foreach (ContainedComponentHost host in _componentHosts)
					host.StopComponent();
				base.Stop();
			}

			public override void EnsureVisible(IApplicationComponent component) {}

			public override void EnsureStarted(IApplicationComponent component) {}
		}

		#endregion
	}
}