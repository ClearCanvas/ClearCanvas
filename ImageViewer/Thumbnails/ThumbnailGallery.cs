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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    internal delegate IThumbnailLoader GetThumbnailLoader();

    public class ThumbnailGallery : Gallery<IDisplaySet>, IDisposable
    {
        private ThumbnailLoader _loader;
        private IThumbnailFactory<IPresentationImage> _thumbnailFactory;
        private Size _thumbnailSize;
        private bool _isVisible;


        public ThumbnailGallery()
            : this(new BindingList<IGalleryItem>())
        {
        }

        public ThumbnailGallery(bool suppressLoadingThumbnails)
            : this(new BindingList<IGalleryItem>(), suppressLoadingThumbnails)
        {
        }

        public ThumbnailGallery(IEnumerable<IGalleryItem> galleryItems)
            : this(galleryItems, false)
        {
        }

        public ThumbnailGallery(IEnumerable<IGalleryItem> galleryItems, bool suppressLoadingThumbnails)
            : base(galleryItems)
        {
            base.GalleryItemFactory = new ThumbnailGalleryItemFactory(() => ThumbnailLoader, suppressLoadingThumbnails);
            _thumbnailSize = ThumbnailSizes.Medium;
        }

        private IThumbnailLoader ThumbnailLoader
        {
            get
            {
                if (_loader == null)
                    _loader = new ThumbnailLoader(new NullThumbnailRepository(ThumbnailFactory));
                return _loader;
            }
        }

        private IThumbnailFactory<IPresentationImage> ThumbnailFactory
        {
            get
            {
                if (_thumbnailFactory == null)
                    _thumbnailFactory = new ThumbnailFactory();
                return _thumbnailFactory;
            }
        }

        public Size ThumbnailSize
        {
            get { return _thumbnailSize; }
            set
            {
                if (value == _thumbnailSize)
                    return;

                _thumbnailSize = value;
                foreach (IThumbnailGalleryItem galleryItem in GalleryItems)
                    galleryItem.ThumbnailSize = _thumbnailSize;
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible)
                    return;

                _isVisible = value;
                foreach (IThumbnailGalleryItem galleryItem in GalleryItems)
                    galleryItem.IsVisible = _isVisible;
            }
        }

        public void SetThumbnailFactory(IThumbnailFactory<IPresentationImage> thumbnailFactory)
        {
            Platform.CheckForNullReference(thumbnailFactory, "thumbnailFactory");
            _thumbnailFactory = thumbnailFactory;
        }

        public void SetBitmapConverter(BitmapConverter bitmapConverter)
        {
            SetThumbnailFactory(new ThumbnailFactory(bitmapConverter));
        }

        protected override IGalleryItem CreateNew(IDisplaySet item)
        {
            var newItem = (IThumbnailGalleryItem)base.CreateNew(item);
            newItem.ThumbnailSize = _thumbnailSize;
            newItem.IsVisible = IsVisible;
            return newItem;
        }
    }
}