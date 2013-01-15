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
using System.Drawing;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    internal interface IThumbnailGalleryItem : IGalleryItem
    {
        Size ThumbnailSize { get; set; }
        bool IsVisible { get; set; }
    }

    internal partial class ThumbnailGalleryItemFactory
    {
        private class ThumbnailGalleryItem : GalleryItem, IThumbnailGalleryItem
        {
            private readonly ThumbnailGalleryItemFactory _factory;
            private bool _suppressLoadingImage;
            private LoadThumbnailRequest _pendingRequest;
            private Size _thumbnailSize;
            private bool _isImageLoaded;
            private bool _isImageValid;
            private bool _isVisible;
            private bool _isDisposed;

            internal ThumbnailGalleryItem(ThumbnailGalleryItemFactory factory, IDisplaySet displaySet, NameAndDescriptionFormat nameAndDescriptionFormat, bool suppressLoadingThumbnails)
                : base(displaySet)
            {
                _factory = factory;
                _suppressLoadingImage = suppressLoadingThumbnails;

                switch (nameAndDescriptionFormat)
                {
                    case NameAndDescriptionFormat.NameAndDescription:
                        Name = GetDisplaySetName(displaySet);
                        Description = GetDisplaySetDescription(displaySet);
                        break;
                    case NameAndDescriptionFormat.NoDescription:
                        Name = GetDisplaySetName(displaySet);
                        break;
                    case NameAndDescriptionFormat.VerboseNameNoDescription:
                        Name = GetDisplaySetDescription(displaySet);
                        break;
                    case NameAndDescriptionFormat.VerboseNameAndDescription:
                        Name = Description = GetDisplaySetDescription(displaySet);
                        break;
                }
            }

            private SynchronizationContext SynchronizationContext { get { return _factory._synchronizationContext; } }
            private IThumbnailLoader Loader { get { return _factory.Loader; } }

            public Size ThumbnailSize
            {
                get { return _thumbnailSize; }
                set
                {
                   if (_thumbnailSize == value) 
                       return;

                    if (value.IsEmpty)
                        throw new ArgumentException("Cannot make the thumbnail zero empty.");

                    _thumbnailSize = value;

                    CancelPendingRequest();
                    if (ImageData != null)
                        ImageData.Dispose();

                    _isImageLoaded = false;
                    _isImageValid = false;

                    if (!_isDisposed && _isVisible)
                       UpdateImage();
                }
            }

            public bool IsVisible
            {
                get { return _isVisible; }
                set
                {
                    if (_isVisible == value)
                        return;

                    _isVisible = value;
                    if(!_isVisible)
                        CancelPendingRequest();
                    else
                        UpdateImage();
                }
            }

            private void CancelPendingRequest()
            {
                if (_pendingRequest == null)
                    return;

                Loader.Cancel(_pendingRequest);
                _pendingRequest = null;
            }

            private void UpdateImage()
            {
                if (_isImageLoaded || ThumbnailSize.IsEmpty)
                    return;

                var descriptor = ThumbnailDescriptor.Create((IDisplaySet)Item, true);
                if (descriptor == null)
                {
                    if (!_isImageValid)
                    {
                        ImageData = Loader.GetDummyThumbnail(_factory.NoImagesMessage, _thumbnailSize);
                        _isImageValid = true;
                    }
                }
                else
                {
                    IImageData imageData;
                    if (!_isImageValid && Loader.TryGetThumbnail(descriptor, _thumbnailSize, out imageData))
                    {
                        ImageData = imageData;
                        _isImageValid = _isImageLoaded = true;
                    }
                    else
                    {
                        if (!_isImageValid && !_suppressLoadingImage)
                        {
                            ImageData = Loader.GetDummyThumbnail(_factory.LoadingMessage, _thumbnailSize);
                            _isImageValid = true;
                        }

                        Loader.LoadThumbnailAsync(new LoadThumbnailRequest(descriptor, _thumbnailSize, OnThumbnailLoadedAsync));
                    }
                }
            }

            private void OnThumbnailLoadedAsync(LoadThumbnailResult result)
            {
                try
                {
                    //Dispose the image on the thread on which the image was rendered because of some weirdities with current rendering implementation.
                    result.Descriptor.ReferenceImage.Dispose();
                    SynchronizationContext.Post(ignore => OnThumbnailLoaded(result), null);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }

            private void OnThumbnailLoaded(LoadThumbnailResult result)
            {
                //just toss it if it's not the size we are currently looking for, or we're disposed.
                if (result.Error == null && (result.Size != _thumbnailSize || _isDisposed))
                {
                    if (result.ThumbnailData != null)
                        result.ThumbnailData.Dispose();
                }
                else
                {
                    ImageData = result.ThumbnailData ?? Loader.GetErrorThumbnail(_thumbnailSize);
                    _isImageValid = _isImageLoaded = true;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _isDisposed = true;

                base.Dispose(disposing);
            }
        }
    }
}
