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
using System.Threading;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    internal partial class ThumbnailGalleryItemFactory : IGalleryItemFactory<IDisplaySet>
    {
        private readonly GetThumbnailLoader _getLoader;
        private readonly bool _suppressLoadingThumbnails;
        private readonly SynchronizationContext _synchronizationContext;

        public ThumbnailGalleryItemFactory(GetThumbnailLoader getLoader)
            : this(getLoader, false)
        {
        }

        public ThumbnailGalleryItemFactory(GetThumbnailLoader getLoader, bool suppressLoadingThumbnails)
        {
            _synchronizationContext = SynchronizationContext.Current;
            if (_synchronizationContext == null)
                throw new InvalidOperationException("It is expected that the factory will be instantiated on and accessed from a UI thread.");

            _getLoader = getLoader;
            _suppressLoadingThumbnails = suppressLoadingThumbnails;
        }

        public IThumbnailLoader Loader { get { return _getLoader(); } }

        private string LoadingMessage { get { return SR.MessageLoading; } }
        private string NoImagesMessage { get { return SR.MessageNoImages; } }

        #region IGalleryItemFactory<IDisplaySet> Members

        public IGalleryItem Create(GalleryItemCreationArgs<IDisplaySet> args)
        {
            return new ThumbnailGalleryItem(this, args.SourceItem, args.NameAndDescriptionFormat, _suppressLoadingThumbnails);
        }

        #endregion

        private static string GetDisplaySetName(IDisplaySet displaySet)
        {
            string name = displaySet.Name;
            name = name.Replace("\r\n", " ");
            name = name.Replace("\r", " ");
            name = name.Replace("\n", " ");
            return name;
        }

        private static string GetDisplaySetDescription(IDisplaySet displaySet)
        {
            var name = GetDisplaySetName(displaySet);
            int number = displaySet.PresentationImages.Count;
            if (number <= 1)
                return String.Format(SR.FormatThumbnailName1Image, name);

            return String.Format(SR.FormatThumbnailName, number, name);
        }
    }
}