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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	[ExtensionPoint]
	public sealed class ThumbnailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ThumbnailComponentViewExtensionPoint))]
	public class ThumbnailComponent : ApplicationComponent
    {
        #region For Item Binding
        
        private class ThumbnailTreeItemBinding : ImageSetTreeItemBinding
        {
            private readonly string _primaryStudyInstanceUid;
            private readonly ThumbnailComponent _owner;

            public ThumbnailTreeItemBinding(ThumbnailComponent owner, string primaryStudyInstanceUid)
            {
                _owner = owner;
                _primaryStudyInstanceUid = primaryStudyInstanceUid;
            }

            public override IconSet GetIconSet(object item)
            {
                var imageSetItem = item as ImageSetTreeItem;
                if (imageSetItem!=null)
                {
                    if (imageSetItem.ImageSet.Uid == _primaryStudyInstanceUid)
                    {
                        return new IconSet("PrimaryImageSet.png");
                    }
                }


                return base.GetIconSet(item);
            }


            public override IResourceResolver GetResourceResolver(object item)
            {
                return _owner._resourceResolver;
            }
        }
        
        #endregion

        private static readonly Dictionary<IImageViewer, ImageSetTree> 
			_viewerTrees = new Dictionary<IImageViewer, ImageSetTree>();

		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _activeViewer;

		private readonly ImageSetTree _dummyTree;
		private ImageSetTree _currentTree;
        private ThumbnailGallery _thumbnailGallery;
        private readonly IResourceResolver _resourceResolver;
	    private BindingList<IGalleryItem> _galleryItems; 

		public ThumbnailComponent(IDesktopWindow desktopWindow)
		{
            _resourceResolver = new ApplicationThemeResourceResolver(this.GetType().Assembly);
			
			_desktopWindow = desktopWindow;
			_dummyTree = new ImageSetTree(new ObservableList<IImageSet>(), null);
			_currentTree = _dummyTree;
		    _thumbnailGallery = new ThumbnailGallery {NameAndDescriptionFormat = NameAndDescriptionFormat.VerboseNameNoDescription};
            _galleryItems = new BindingList<IGalleryItem>();

		    foreach (var galleryItem in _thumbnailGallery.GalleryItems)
		        _galleryItems.Add(galleryItem);

            _thumbnailGallery.GalleryItems.ItemAdded += GalleryItemAdded;
            _thumbnailGallery.GalleryItems.ItemRemoved += GallerItemRemoved;
            _thumbnailGallery.GalleryItems.ItemChanging += GalleryItemChanging;
            _thumbnailGallery.GalleryItems.ItemChanged += GalleryItemChanged;
        }

        private void GalleryItemAdded(object sender, ListEventArgs<IGalleryItem> e)
        {
            _galleryItems.Add(e.Item);
        }

        private void GalleryItemChanging(object sender, ListEventArgs<IGalleryItem> e)
        {
            _galleryItems.RemoveAt(e.Index);
        }

        private void GalleryItemChanged(object sender, ListEventArgs<IGalleryItem> e)
        {
            _galleryItems.Insert(e.Index, e.Item);
        }

        private void GallerItemRemoved(object sender, ListEventArgs<IGalleryItem> e)
        {
            _galleryItems.RemoveAt(e.Index);
        }

		#region Presentation Model

		public ITree Tree
		{
			get { return _currentTree.TreeRoot; }
		}

		public ISelection TreeSelection
		{
			get { return _currentTree.Selection; }
			set { _currentTree.Selection = value; }
		}

		public BindingList<IGalleryItem> Thumbnails
		{
            get
            {
                return _galleryItems; }
		}

		#endregion

		private void UpdateTree()
		{
			if (_activeViewer == null)
			{
				SetCurrentTree(_dummyTree);
			}
			else
			{
				if (!_viewerTrees.ContainsKey(_activeViewer))
				{
					var imageSets = _activeViewer.LogicalWorkspace.ImageSets;
					string primaryStudyInstanceUid = GetPrimaryStudyInstanceUid(_activeViewer.StudyTree);
                    var tree = new ImageSetTree(imageSets, primaryStudyInstanceUid, new ThumbnailTreeItemBinding(this, primaryStudyInstanceUid));
					_viewerTrees.Add(_activeViewer, tree);
				}

				SetCurrentTree(_viewerTrees[_activeViewer]);
			}
		}

		private void SetCurrentTree(ImageSetTree currentTree)
		{
			if (_currentTree == currentTree)
				return;

			if (_currentTree != null)
			{
				_currentTree.TreeChanged -= OnTreeChangedInternal;
				_currentTree.TreeUpdated -= OnTreeUpdatedInternal;
				_currentTree.SelectionChanged -= OnTreeSelectionChangedInternal;
			}

			_currentTree = currentTree;

			if (_currentTree != null)
			{
				_currentTree.TreeChanged += OnTreeChangedInternal;
				_currentTree.TreeUpdated += OnTreeUpdatedInternal;
				_currentTree.SelectionChanged += OnTreeSelectionChangedInternal;

				OnTreeChangedInternal(null, null);
			}
		}

		private void OnTreeChangedInternal(object sender, EventArgs e)
		{
			NotifyPropertyChanged("Tree");
			OnTreeSelectionChangedInternal(null, null);
		}

		private void OnTreeUpdatedInternal(object sender, EventArgs e)
		{
			//crappy hack - when things are added to the tree dynamically, the view seems to lose the selection.
			NotifyPropertyChanged("TreeSelection");
		}

		private void OnTreeSelectionChangedInternal(object sender, EventArgs e)
		{
			NotifyPropertyChanged("TreeSelection");
			RefreshThumbnails();
		}

		private void OnLoadingPriorsChanged(object sender, EventArgs e)
		{
			UpdateTitle();
		}

		private void UpdateTitle()
		{
			if (_activeViewer != null && _activeViewer.PriorStudyLoader.IsActive)
				Host.Title = SR.TitleThumbnailsLoadingPriors;
			else
				Host.Title = SR.TitleThumbnails;
		}

		internal static IShelf Launch(IDesktopWindow desktopWindow)
		{
			var component = new ThumbnailComponent(desktopWindow);
			var shelf = LaunchAsShelf(
				desktopWindow,
				component,
				SR.TitleThumbnails,
				"Thumbnails",
				ShelfDisplayHint.DockTop | ShelfDisplayHint.DockAutoHide);

			return shelf;
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_desktopWindow.Workspaces.ItemActivationChanged += OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed += OnWorkspaceClosed;
			
			SetImageViewer(_desktopWindow.ActiveWorkspace);
			UpdateTree();
			
			base.Start();
		    _thumbnailGallery.IsVisible = true;
		}

		public override void Stop()
		{
			SetCurrentTree(null);
			_dummyTree.Dispose();

			foreach (ImageSetTree info in _viewerTrees.Values)
				info.Dispose();

			_viewerTrees.Clear();

			_desktopWindow.Workspaces.ItemActivationChanged -= OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed -= OnWorkspaceClosed;

            _thumbnailGallery.GalleryItems.ItemAdded -= GalleryItemAdded;
            _thumbnailGallery.GalleryItems.ItemRemoved -= GallerItemRemoved;
            _thumbnailGallery.GalleryItems.ItemChanging -= GalleryItemChanging;
            _thumbnailGallery.GalleryItems.ItemChanged -= GalleryItemChanged;

			SetImageViewer(null);
            _thumbnailGallery.Dispose();

            base.Stop();
        }

		private void OnActiveWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
		{
			SetImageViewer(e.Item);
			UpdateTree();
		}

		private void OnWorkspaceClosed(object sender, ClosedItemEventArgs<Workspace> e)
		{
			var viewer = CastToImageViewer(e.Item);
			if (viewer != null && _viewerTrees.ContainsKey(viewer))
			{
				var tree = _viewerTrees[viewer];
				_viewerTrees.Remove(viewer);
				tree.Dispose();
			}

		    if (_desktopWindow.Workspaces.Count != 0)
                return;

		    SetImageViewer(null);
		    UpdateTree();
		}

		private static IImageViewer CastToImageViewer(Workspace workspace)
		{
			IImageViewer viewer = null;
			if (workspace != null)
				viewer = ImageViewerComponent.GetAsImageViewer(workspace);

			return viewer;
		}

		private void SetImageViewer(Workspace workspace)
		{
			var viewer = CastToImageViewer(workspace);
			if (viewer == _activeViewer)
				return;

			if (_activeViewer != null)
				_activeViewer.PriorStudyLoader.IsActiveChanged -= OnLoadingPriorsChanged;

			_activeViewer = viewer;

			if (_activeViewer != null)
				_activeViewer.PriorStudyLoader.IsActiveChanged += OnLoadingPriorsChanged;

			UpdateTitle();
		}

		#region Thumbnail Methods

		private void RefreshThumbnails()
		{
			ClearThumbnails();

			if (_activeViewer == null)
				return;

			var imageSetItem = TreeSelection.Item as ImageSetTreeItem;
			if (imageSetItem == null)
				return;

		    _thumbnailGallery.SourceItems = imageSetItem.ImageSet.DisplaySets;
        }

        private void ClearThumbnails()
        {
            _thumbnailGallery.SourceItems = null;
        }

	    private static string GetPrimaryStudyInstanceUid(StudyTree studyTree)
		{
			foreach (Patient patient in studyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					return study.StudyInstanceUid;
				}
			}

			return null;
		}

		#endregion
	}
}
