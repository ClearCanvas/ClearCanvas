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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface to a context for tools that operate on a folder explorer group.
	/// </summary>
	public interface IFolderExplorerGroupToolContext : IToolContext
	{
		/// <summary>
		/// Gets the desktop window.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the selected folder system (the active folder explorer).
		/// </summary>
		IFolderSystem SelectedFolderSystem { get; }

		/// <summary>
		/// Occurs after the <see cref="SelectedFolderSystem"/> property changes.
		/// </summary>
		event EventHandler SelectedFolderSystemChanged;

		/// <summary>
		/// Gets the selected folder, or null if no folder is selected.
		/// </summary>
		IFolder SelectedFolder { get; }

		/// <summary>
		/// Occurs after the <see cref="SelectedFolder"/> property changes.
		/// </summary>
		event EventHandler SelectedFolderChanged;
	}

	[ExtensionPoint]
	public class FolderExplorerGroupToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// Extension point for views onto <see cref="FolderExplorerGroupComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class FolderExplorerGroupComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// FolderExplorerGroupComponent class
	/// </summary>
	[AssociateView(typeof(FolderExplorerGroupComponentViewExtensionPoint))]
	public class FolderExplorerGroupComponent : ApplicationComponent, ISearchDataHandler
	{
		#region ISearchDataHandler implementation

		public SearchParams SearchParams
		{
			set { Search(value); }
		}

		bool ISearchDataHandler.SearchEnabled
		{
			get { return this.AdvancedSearchEnabled; }
		}

		public event EventHandler SearchEnabledChanged
		{
			add { _selectedFolderExplorerChanged += value; }
			remove { _selectedFolderExplorerChanged -= value; }
		}

		#endregion

		#region IFolderSystemContext implementation

		class FolderSystemContext : ToolContext, IFolderSystemContext
		{
			private readonly FolderExplorerGroupComponent _owner;
			private readonly IFolderExplorerComponent _explorerComponent;
			private readonly FolderContentsComponent _contentsComponent;

			private event EventHandler _selectedItemsChanged;
			private event EventHandler _selectedItemDoubleClicked;


			public FolderSystemContext(FolderExplorerGroupComponent owner, IFolderExplorerComponent explorerComponent, FolderContentsComponent contentsComponent)
			{
				_owner = owner;
				_explorerComponent = explorerComponent;
				_contentsComponent = contentsComponent;
				_contentsComponent.SelectedItemsChanged += SelectedItemsChangedEventHandler;
				_contentsComponent.SelectedItemDoubleClicked += SelectedItemDoubleClickeEventHandler;
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.Host.DesktopWindow; }
			}

			public IFolder SelectedFolder
			{
				get { return _explorerComponent.SelectedFolder; }
				set { _explorerComponent.SelectedFolder = value; }
			}

			public event EventHandler SelectedFolderChanged
			{
				add { _explorerComponent.SelectedFolderChanged += value; }
				remove { _explorerComponent.SelectedFolderChanged -= value; }
			}

			public ISelection SelectedItems
			{
				get
				{
					// the folder system should see the selection as empty if it is not the active folder system
					return (_explorerComponent == _owner._selectedFolderExplorer) ? _contentsComponent.SelectedItems : Selection.Empty;
				}
			}

			public event EventHandler SelectedItemsChanged
			{
				add { _selectedItemsChanged += value; }
				remove { _selectedItemsChanged -= value; }
			}

			public event EventHandler SelectedItemDoubleClicked
			{
				add { _selectedItemDoubleClicked += value; }
				remove { _selectedItemDoubleClicked -= value; }
			}

			private void SelectedItemDoubleClickeEventHandler(object sender, EventArgs e)
			{
				// it only makes sense to notify the folder system if it is the active folder system
				if (_explorerComponent == _owner._selectedFolderExplorer)
					EventsHelper.Fire(_selectedItemDoubleClicked, _contentsComponent, EventArgs.Empty);
			}

			private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
			{
				// it only makes sense to notify the folder system if it is the active folder system
				if (_explorerComponent == _owner._selectedFolderExplorer)
					EventsHelper.Fire(_selectedItemsChanged, _contentsComponent, EventArgs.Empty);
			}

		}

		#endregion

		#region FolderExplorerGroupToolContext

		class FolderExplorerGroupToolContext : IFolderExplorerGroupToolContext
		{
			private readonly FolderExplorerGroupComponent _owner;

			public FolderExplorerGroupToolContext(FolderExplorerGroupComponent owner)
			{
				_owner = owner;
			}

			#region IFolderExplorerGroupToolContext Members

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.Host.DesktopWindow; }
			}

			public IFolderSystem SelectedFolderSystem
			{
				get { return _owner.SelectedFolderExplorer == null ? null : _owner.SelectedFolderExplorer.FolderSystem; }
			}

			public event EventHandler SelectedFolderSystemChanged
			{
				add { _owner.SelectedFolderExplorerChanged += value; }
				remove { _owner.SelectedFolderExplorerChanged -= value; }
			}

			public IFolder SelectedFolder
			{
				get { return _owner.SelectedFolderExplorer == null ? null : _owner.SelectedFolderExplorer.SelectedFolder; }
			}

			public event EventHandler SelectedFolderChanged
			{
				add { _owner.SelectedFolderChanged += value; }
				remove { _owner.SelectedFolderChanged -= value; }
			}

			#endregion
		}

		#endregion

		private readonly List<IFolderSystem> _folderSystems;
		private readonly FolderContentsComponent _contentComponent;
		private readonly StackTabComponentContainer _stackTabComponent;
		private ChildComponentHost _stackTabComponentContainerHost;

		private readonly Dictionary<IFolderSystem, IFolderExplorerComponent> _folderExplorerComponents;
		private IFolderExplorerComponent _selectedFolderExplorer;

		private event EventHandler _selectedFolderExplorerChanged;
		private event EventHandler _selectedFolderChanged;

		private ToolSet _toolSet;

		/// <summary>
		/// Constructor
		/// </summary>
		public FolderExplorerGroupComponent(List<IFolderSystem> folderSystems, FolderContentsComponent contentComponent)
		{
			_folderSystems = folderSystems;
			_contentComponent = contentComponent;
			_stackTabComponent = new StackTabComponentContainer(StackStyle.ShowOneOnly, false);
			_folderExplorerComponents = new Dictionary<IFolderSystem, IFolderExplorerComponent>();

		}

		#region Application Overrides

		public override void Start()
		{
			BuildFolderExplorers();

			// Subscribe to page changed event before starting component, so that when component start the event will fire
			// for the initial page that open
			_stackTabComponent.CurrentPageChanged += OnCurrentPageChanged;
			_stackTabComponentContainerHost = new ChildComponentHost(this.Host, _stackTabComponent);
			_stackTabComponentContainerHost.StartComponent();

			// register folder system instances with document manager
			foreach (var folderSystem in _folderSystems)
			{
				DocumentManager.RegisterFolderSystem(folderSystem);
			}

			// create tools
			_toolSet = new ToolSet(new FolderExplorerGroupToolExtensionPoint(), new FolderExplorerGroupToolContext(this));

			base.Start();
		}

		public override void Stop()
		{
			if (_toolSet != null)
			{
				_toolSet.Dispose();
				_toolSet = null;
			}

			if (_stackTabComponentContainerHost != null)
			{
				_stackTabComponent.CurrentPageChanged -= OnCurrentPageChanged;
				_stackTabComponentContainerHost.StopComponent();
				_stackTabComponentContainerHost = null;
			}

			// un-register folder system instances with document manager
			foreach (var folderSystem in _folderSystems)
			{
				DocumentManager.UnregisterFolderSystem(folderSystem);
			}

			base.Stop();
		}

		#endregion

		#region Public API

		public event EventHandler SelectedFolderExplorerChanged
		{
			add { _selectedFolderExplorerChanged += value; }
			remove { _selectedFolderExplorerChanged -= value; }
		}

		public event EventHandler SelectedFolderChanged
		{
			add { _selectedFolderChanged += value; }
			remove { _selectedFolderChanged -= value; }
		}

		public IFolderExplorerComponent SelectedFolderExplorer
		{
			get { return _selectedFolderExplorer; }
		}

		#endregion

		#region Presentation Model

		public ApplicationComponentHost StackTabComponentContainerHost
		{
			get { return _stackTabComponentContainerHost; }
		}

		public ActionModelNode ToolbarModel
		{
			get
			{
				return CreateActionModel("folderexplorer-folders-toolbar");
			}
		}

		public ActionModelNode ContextMenuModel
		{
			get
			{
				return CreateActionModel("folderexplorer-folders-contextmenu");
			}
		}

		public bool SearchEnabled
		{
			get { return _selectedFolderExplorer == null ? false : this._selectedFolderExplorer.FolderSystem.SearchEnabled; }
		}

		public bool AdvancedSearchEnabled
		{
			get { return _selectedFolderExplorer == null ? false : this._selectedFolderExplorer.FolderSystem.AdvancedSearchEnabled; }
		}

		public string SearchMessage
		{
			get { return _selectedFolderExplorer == null ? null : this._selectedFolderExplorer.FolderSystem.SearchMessage; }
		}

		public void Search(string textSearch)
		{
			var searchParams = this.SelectedFolderExplorer.FolderSystem.CreateSearchParams(textSearch);
			Search(searchParams);
		}

		protected void Search(SearchParams searchParams)
		{
			if (!searchParams.UseAdvancedSearch && string.IsNullOrEmpty(searchParams.TextSearch))
			{
				this.Host.ShowMessageBox(this.SearchMessage, MessageBoxActions.Ok);
				return;
			}

			this.SelectedFolderExplorer.ExecuteSearch(searchParams);
		}

		public void AdvancedSearch()
		{
			this.SelectedFolderExplorer.LaunchAdvancedSearchComponent();
		}

		#endregion

		#region Event Handlers

		private void OnCurrentPageChanged(object sender, EventArgs e)
		{
			var explorer = (FolderExplorerComponent)_stackTabComponent.CurrentPage.Component;

			if (_selectedFolderExplorer == explorer)
				return;

			_selectedFolderExplorer = explorer;
			SearchComponentManager.EnsureProperSearchComponent(_selectedFolderExplorer.FolderSystem);
			NotifyPropertyChanged("SearchEnabled");
			NotifyPropertyChanged("SearchMessage");
			EventsHelper.Fire(_selectedFolderExplorerChanged, this, EventArgs.Empty);

			// refresh folders in newly selected folder explorer
			if (_selectedFolderExplorer.IsInitialized)
			{
				_selectedFolderExplorer.InvalidateFolders();
			}
			else
			{
				_selectedFolderExplorer.Initialize();
			}
		}

		private void OnSelectedFolderChanged(object sender, EventArgs e)
		{
			EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Handles the <see cref="FolderExplorerComponent.Initialized"/> event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FolderSystemInitializedEventHandler(object sender, EventArgs e)
		{
			// this event handler is only needed to force the initial invalidation of the
			// first selected folder explorer
			if (sender == _selectedFolderExplorer)
			{
				_selectedFolderExplorer.InvalidateFolders();
			}
		}

		private void FolderSystemTitleChangedEventHandler(object sender, EventArgs e)
		{
			var folderSystem = (IFolderSystem)sender;
			var page = FindPage(folderSystem);
			page.Title = folderSystem.Title;
		}

		private void FolderSystemIconChangedEventHandler(object sender, EventArgs e)
		{
			var folderSystem = (IFolderSystem)sender;
			var page = FindPage(folderSystem);
			page.IconSet = folderSystem.TitleIcon;
		}

		#endregion

		#region Helpers

		private void BuildFolderExplorers()
		{
			var folderSystems = FolderExplorerComponentSettings.Default.ApplyFolderSystemsOrder(_folderSystems);

			// create a folder explorer component and a tab page for each folder system
			foreach (var folderSystem in folderSystems)
			{
				IFolderExplorerComponent explorer = new FolderExplorerComponent(folderSystem, this);
				explorer.Initialized += FolderSystemInitializedEventHandler;
				explorer.SelectedFolderChanged += OnSelectedFolderChanged;

				folderSystem.SetContext(new FolderSystemContext(this, explorer, _contentComponent));
				folderSystem.TitleChanged += FolderSystemTitleChangedEventHandler;
				folderSystem.TitleIconChanged += FolderSystemIconChangedEventHandler;

				var page = CreatePageForFolderExplorer(explorer);
				_folderExplorerComponents.Add(folderSystem, (FolderExplorerComponent)page.Component);
				_stackTabComponent.Pages.Add(page);
			}
		}

		private static StackTabPage CreatePageForFolderExplorer(IFolderExplorerComponent explorer)
		{
			return new StackTabPage(
				explorer.FolderSystem.Title,
				(IApplicationComponent)explorer,
				explorer.FolderSystem.Title,
				explorer.FolderSystem.TitleIcon,
				explorer.FolderSystem.ResourceResolver)
					{
						// set folder explorers to start immediately, 
						// so that they can update the title bar if needed
						LazyStart = false
					};
		}

		private StackTabPage FindPage(IFolderSystem folderSystem)
		{
			var explorer = _folderExplorerComponents[folderSystem];
			return CollectionUtils.SelectFirst(_stackTabComponent.Pages, page => ReferenceEquals(page.Component, explorer));
		}

		/// <summary>
		/// Creates an action model that is the union of the tools for this component plus
		/// the tools for the currently selected folder explorer.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		private ActionModelNode CreateActionModel(string site)
		{
			var allActions = _toolSet.Actions;
			var explorerComponent = _selectedFolderExplorer as IApplicationComponent;
			if (explorerComponent != null)
			{
				allActions = allActions.Union(explorerComponent.ExportedActions);
			}
			return ActionModelRoot.CreateModel(this.GetType().FullName, site, allActions);
		}

		#endregion
	}
}
