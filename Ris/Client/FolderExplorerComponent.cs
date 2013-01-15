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
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="FolderExplorerComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FolderExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistExplorerComponent class
    /// </summary>
    [AssociateView(typeof(FolderExplorerComponentViewExtensionPoint))]
    public class FolderExplorerComponent : ApplicationComponent, IFolderExplorerComponent
    {
		enum InitializationState
		{
			NotInitialized,
			Initializing,
			Initialized
		}

		private readonly FolderTreeRoot _folderTreeRoot;
		private FolderTreeNode _selectedTreeNode;
        private event EventHandler _selectedFolderChanged;
    	private event EventHandler _intialized;
		private InitializationState _initializationState;

        private readonly IFolderSystem _folderSystem;
    	private Timer _folderInvalidateTimer;

    	private readonly FolderExplorerGroupComponent _owner;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponent(IFolderSystem folderSystem, FolderExplorerGroupComponent owner)
        {
			_folderTreeRoot = new FolderTreeRoot(this);
            _folderSystem = folderSystem;
        	_owner = owner;
        }

		#region IFolderExplorerComponent implementation

    	/// <summary>
    	/// Gets a value indicating whether this folder explorer has already been initialized.
    	/// </summary>
    	bool IFolderExplorerComponent.IsInitialized
    	{
			get { return IsInitialized; }
    	}

    	/// <summary>
    	/// Instructs the folder explorer to initialize (build the folder system).
    	/// </summary>
    	void IFolderExplorerComponent.Initialize()
		{
			Initialize();
		}

		/// <summary>
		/// Occurs when asynchronous initialization of this folder system has completed.
		/// </summary>
		event EventHandler IFolderExplorerComponent.Initialized
		{
			add { _intialized += value; }
			remove { _intialized -= value; }
		}


		/// <summary>
		/// Gets or sets the currently selected folder.
		/// </summary>
		IFolder IFolderExplorerComponent.SelectedFolder
		{
			get { return this.SelectedFolder; }
			set
			{
				this.SelectedFolder = value;
			}
		}

		/// <summary>
		/// Invalidates all folders.
		/// </summary>
    	void IFolderExplorerComponent.InvalidateFolders()
		{
			// check initialized
			if (!IsInitialized)
				return;

			// invalidate all folders, and update starting at the root
			_folderSystem.InvalidateFolders();
		}

    	/// <summary>
    	/// Gets the underlying folder system associated with this folder explorer.
    	/// </summary>
    	IFolderSystem IFolderExplorerComponent.FolderSystem
		{
			get { return _folderSystem; }
		}

    	/// <summary>
    	/// Occurs when the selected folder changes.
    	/// </summary>
    	event EventHandler IFolderExplorerComponent.SelectedFolderChanged
		{
			add { _selectedFolderChanged += value; }
			remove { _selectedFolderChanged -= value; }
		}

		/// <summary>
		/// Executes a search on this folder system.
		/// </summary>
		/// <param name="searchParams"></param>
		void IFolderExplorerComponent.ExecuteSearch(SearchParams searchParams)
		{
			// check initialized
			if (!IsInitialized)
				return;

			if (_folderSystem.SearchEnabled)
				_folderSystem.ExecuteSearch(searchParams);
		}

		void IFolderExplorerComponent.LaunchAdvancedSearchComponent()
		{
			_folderSystem.LaunchSearchComponent();
		}

    	/// <summary>
    	/// Gets the application component that displays the content of a folder for this folder system.
    	/// </summary>
    	/// <returns></returns>
    	IApplicationComponent IFolderExplorerComponent.GetContentComponent()
    	{
    		return _folderSystem.GetContentComponent();
    	}

    	#endregion

		#region Application Component overrides

		public override void Start()
        {
			// if the folder system needs immediate initialization, do that now
			if(!_folderSystem.LazyInitialize)
			{
				Initialize();
			}

        	base.Start();
        }

    	public override void Stop()
		{
			if (_folderInvalidateTimer != null)
			{
				_folderInvalidateTimer.Stop();
				_folderInvalidateTimer.Dispose();
			}

			// un-subscribe to events (important because the folderSystem object may be re-used by another explorer)
			_folderSystem.Folders.ItemAdded -= FolderAddedEventHandler;
			_folderSystem.Folders.ItemRemoved -= FolderRemovedEventHandler;
			_folderSystem.FoldersChanged -= FoldersChangedEventHandler;
			_folderSystem.FoldersInvalidated -= FoldersInvalidatedEventHandler;
			_folderSystem.FolderPropertiesChanged -= FolderPropertiesChangedEventHandler;
			_folderSystem.Dispose();

			base.Stop();
		}

        public override IActionSet ExportedActions
        {
            get 
            { 
                return _folderSystem.FolderTools == null
                    ? new ActionSet()
                    : _folderSystem.FolderTools.Actions; 
            }
        }

        #endregion

        #region Presentation Model

    	public ITree FolderTree
        {
			get { return _folderTreeRoot.GetSubTree(); }
        }

        public ISelection SelectedFolderTreeNode
        {
            get { return new Selection(_selectedTreeNode); }
            set
            {
				var nodeToSelect = (FolderTreeNode)value.Item;
                SelectFolder(nodeToSelect);
            }
        }

        public ITable FolderContentsTable
        {
            get { return _selectedTreeNode == null ? null : _selectedTreeNode.Folder.ItemsTable; }
        }

        public event EventHandler SelectedFolderChanged
        {
            add { _selectedFolderChanged += value; }
            remove { _selectedFolderChanged -= value; }
        }

        public ActionModelNode FoldersContextMenuModel
        {
            get
            {
				// need to return the menu model for the entire Group component, rather than just our own
            	return _owner.ContextMenuModel;
            }
        }

        #endregion

		#region Private methods

		private bool IsInitialized
		{
			get { return _initializationState == InitializationState.Initialized; }
		}

		private void Initialize()
		{
			// check already initialized, or initialization in progress
			if (_initializationState != InitializationState.NotInitialized)
				return;

			_initializationState = InitializationState.Initializing;

			Async.Invoke(this,
				() => _folderSystem.Initialize(),
				delegate
				{
					// subscribe to events
					_folderSystem.Folders.ItemAdded += FolderAddedEventHandler;
					_folderSystem.Folders.ItemRemoved += FolderRemovedEventHandler;
					_folderSystem.FoldersChanged += FoldersChangedEventHandler;
					_folderSystem.FoldersInvalidated += FoldersInvalidatedEventHandler;
					_folderSystem.FolderPropertiesChanged += FolderPropertiesChangedEventHandler;

					// build the initial folder tree, but do not udpate it, as this will be done on demand
					// when this folder system is selected
					BuildFolderTree();

					// this timer is responsible for monitoring the auto-invalidation of all folders
					// in the folder system, and performing the appropriate invalidations
					// bug #6909: increase timer interval from 1 sec to 10 seconds, to reduce lockup issues when time provider can't access network
					_folderInvalidateTimer = new Timer(delegate { AutoInvalidateFolders(); }) { IntervalMilliseconds = 10000 };
					_folderInvalidateTimer.Start();

					// notify that this folder system is now initialized
					_initializationState = InitializationState.Initialized;
					EventsHelper.Fire(_intialized, this, EventArgs.Empty);
				});
		}

		private void AutoInvalidateFolders()
		{
			try
			{
				var count = 0;
				foreach (var folder in _folderSystem.Folders)
				{
					if (folder.AutoInvalidateInterval > TimeSpan.Zero
						&& (Platform.Time - folder.LastUpdateTime) > folder.AutoInvalidateInterval)
					{
						_folderSystem.InvalidateFolder(folder);
						count++;
					}
				}

				if (count > 0)
				{
					// update folder tree in case any folders were invalidated
					// this is done regardless of whether this folder explorer is currently visible, because
					// we need to keep the title bars of the folder explorers updated
					_folderTreeRoot.Update();
				}

			}
			catch (Exception e)
			{
				// Bug #2445 : given that this occurs inside a Timer callback, we might as well swallow
				// and log any exceptions that might occur, to prevent client from crashing
				Platform.Log(LogLevel.Error, e);
			}
		}

		/// <summary>
		/// Gets or sets the currently selected folder.
		/// </summary>
		private IFolder SelectedFolder
		{
			get { return _selectedTreeNode == null ? null : _selectedTreeNode.Folder; }
			set
			{
				this.SelectedFolderTreeNode = new Selection(_folderTreeRoot.FindNode(value));
			}
		}

		private void SelectFolder(FolderTreeNode node)
        {
            if (_selectedTreeNode != node)
            {
                if (_selectedTreeNode != null)
                {
					_selectedTreeNode.Folder.CloseFolder();
                }

				if (node != null)
                {
					node.Folder.OpenFolder();
					
					// ensure the content of this nodes folder is up to date
					node.Update();
                }

				_selectedTreeNode = node;
				EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
			}
		}

        internal DragDropKind CanFolderAcceptDrop(FolderTreeNode treeNode, object dropData, DragDropKind kind)
        {
            if (treeNode.Folder != _selectedTreeNode && dropData is ISelection)
            {
                return treeNode.Folder.CanAcceptDrop((dropData as ISelection).Items, kind);
            }
            return DragDropKind.None;
        }

		internal DragDropKind FolderAcceptDrop(FolderTreeNode treeNode, object dropData, DragDropKind kind)
        {
			if (treeNode.Folder != _selectedTreeNode && dropData is ISelection)
            {
                // inform the target folder to accept the drop
				var result = treeNode.Folder.AcceptDrop((dropData as ISelection).Items, kind);

                // inform the source folder that a drag was completed
                _selectedTreeNode.Folder.DragComplete((dropData as ISelection).Items, result);
            }
            return DragDropKind.None;
        }

		private void FolderAddedEventHandler(object sender, ListEventArgs<IFolder> e)
		{
			// folder was added to the folder system, so add it to the tree
			_folderTreeRoot.InsertFolder(e.Item, false);
		}

		private void FolderRemovedEventHandler(object sender, ListEventArgs<IFolder> e)
		{
			// bug: noticed that if the folder being removed or one of its parents is currently selected,
			// the UI may exhibit strange behaviour
			// to be safe, just remove the current selection
			this.SelectedFolder = null;

			// folder was removed from the folder system, so remove it from the tree
			_folderTreeRoot.RemoveFolder(e.Item);
		}

		private void FoldersChangedEventHandler(object sender, EventArgs e)
		{
			BuildFolderTree();
		}

		private void FoldersInvalidatedEventHandler(object sender, EventArgs e)
		{
			//TODO: only do update if this explorer is active
			_folderTreeRoot.Update();
		}

		private void FolderPropertiesChangedEventHandler(object sender, ItemEventArgs<IFolder> e)
		{
			var folder = e.Item;

			// apply customizations to folder
			FolderExplorerComponentSettings.Default.ApplyFolderCustomizations(_folderSystem, folder);

			// notify UI to update folder properties
			_folderTreeRoot.NotifyFolderPropertiesUpdated(folder);
		}

		private void BuildFolderTree()
		{
			// clear existing
			_folderTreeRoot.GetSubTree().Items.Clear();

			var orderedFolders = FolderExplorerComponentSettings.Default.ApplyFolderCustomizations(_folderSystem);
			orderedFolders = CollectionUtils.Select(orderedFolders, f => f.Visible);

			_folderTreeRoot.InsertFolders(orderedFolders, false);
		}

		#endregion
    }
}
