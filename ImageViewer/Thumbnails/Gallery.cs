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

using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IGallery
    {
        IObservableList<IGalleryItem> GalleryItems { get; }
    }

    public interface IGallery<TSourceItem> : IGallery
    {
        IGalleryItemFactory<TSourceItem> GalleryItemFactory { get; }
        IObservableList<TSourceItem> SourceItems { get; set; }
    }

    public enum NameAndDescriptionFormat
    {
        NameAndDescription,
        NoDescription,
        VerboseNameNoDescription,
        VerboseNameAndDescription
    }

    public struct GalleryItemCreationArgs<TSourceItem>
    {
        public TSourceItem SourceItem;
        public NameAndDescriptionFormat NameAndDescriptionFormat;
    }

    public interface IGalleryItemFactory<TSourceItem>
    {
        IGalleryItem Create(GalleryItemCreationArgs<TSourceItem> args);
    }

    public class Gallery<TSourceItem> : IGallery<TSourceItem> where TSourceItem : class, IDisposable
    {
        private IObservableList<TSourceItem> _sourceItems;
        private int _lastChangedIndex = -1;

        public Gallery()
            : this(new BindingList<IGalleryItem>())
        {
        }

        public Gallery(IEnumerable<IGalleryItem> galleryItems)
        {
            GalleryItems = new ObservableList<IGalleryItem>(galleryItems);
        }

        public Gallery(IEnumerable<IGalleryItem> galleryItems, IGalleryItemFactory<TSourceItem> galleryItemFactory)
            : this(galleryItems)
        {
            GalleryItemFactory = galleryItemFactory;
        }

        public NameAndDescriptionFormat NameAndDescriptionFormat { get; set; }

        #region IGallery<TSourceItem> Members

        public IObservableList<TSourceItem> SourceItems
        {
            get { return _sourceItems; }
            set
            {
                if (Equals(_sourceItems, value))
                    return;

                if (_sourceItems != null)
                {
                    _sourceItems.ItemAdded -= OnSourceItemAdded;
                    _sourceItems.ItemChanging -= OnSourceItemChanging;
                    _sourceItems.ItemChanged -= OnSourceItemChanged;
                    _sourceItems.ItemRemoved -= OnSourceItemRemoved;
                }

                _sourceItems = value;

                Clear();
                if (_sourceItems == null)
                    return;

                _sourceItems.ItemAdded += OnSourceItemAdded;
                _sourceItems.ItemChanging += OnSourceItemChanging;
                _sourceItems.ItemChanged += OnSourceItemChanged;
                _sourceItems.ItemRemoved += OnSourceItemRemoved;

                foreach (var sourceItem in _sourceItems)
                    GalleryItems.Add(CreateNew(sourceItem));
            }
        }

        protected IGalleryItemFactory<TSourceItem> GalleryItemFactory { get; set; }

        IGalleryItemFactory<TSourceItem> IGallery<TSourceItem>.GalleryItemFactory { get { return GalleryItemFactory; } }

        #endregion

        #region IGallery Members

        public IObservableList<IGalleryItem> GalleryItems { get; private set; }

        #endregion

        private void OnSourceItemAdded(object sender, ListEventArgs<TSourceItem> e)
        {
            GalleryItems.Add(CreateNew(e.Item));
        }

        private void OnSourceItemChanging(object sender, ListEventArgs<TSourceItem> e)
        {
            _lastChangedIndex = IndexOf(e.Item);
        }

        private void OnSourceItemChanged(object sender, ListEventArgs<TSourceItem> e)
        {
            if (_lastChangedIndex >= 0)
            {
                var oldItem = GalleryItems[_lastChangedIndex];
                var newItem = CreateNew(e.Item);
                GalleryItems[_lastChangedIndex] = newItem;
                OnItemRemoved(oldItem);
                OnItemChanged(newItem);
            }
            else
            {
                //This is really an error condition, but it'll never happen anyway.
                GalleryItems.Add(CreateNew(e.Item));
            }
        }

        private void OnSourceItemRemoved(object sender, ListEventArgs<TSourceItem> e)
        {
            var index = IndexOf(e.Item);
            if (index < 0)
                return;

            var item = GalleryItems[index];
            GalleryItems.RemoveAt(index);
            OnItemRemoved(item);
        }

        private int IndexOf(TSourceItem item)
        {
            int i = 0;
            foreach(var galleryItem in GalleryItems)
            {
                if (galleryItem.Item == item)
                    return i;
                ++i;
            }

            return -1;
        }

        protected virtual IGalleryItem CreateNew(TSourceItem item)
        {
            return GalleryItemFactory.Create(new GalleryItemCreationArgs<TSourceItem> { SourceItem = item, NameAndDescriptionFormat = NameAndDescriptionFormat });
        }

        protected virtual void OnItemChanged(IGalleryItem item)
        {
        }

        protected virtual void OnItemRemoved(IGalleryItem item)
        {
            item.Dispose();
        }

        private void Clear()
        {
            foreach (var item in GalleryItems)
                OnItemRemoved(item);

            GalleryItems.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Clear();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
            }
        }

        #endregion
    }
}
