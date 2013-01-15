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
using System.Drawing;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class LoadThumbnailRequest : IEquatable<LoadThumbnailRequest>
    {
        public LoadThumbnailRequest(ThumbnailDescriptor descriptor, Size size, Action<LoadThumbnailResult> resultCallback)
        {
            Platform.CheckForNullReference(descriptor, "descriptor");
            Platform.CheckForNullReference(resultCallback, "resultCallback");

            Descriptor = descriptor;
            Size = size;
            ResultCallback = resultCallback;
        }

        public readonly ThumbnailDescriptor Descriptor;
        public readonly Size Size;
        public readonly Action<LoadThumbnailResult> ResultCallback;

        public override string ToString()
        {
            return String.Format("{0}/{1}x{2}", Descriptor, Size.Width, Size.Height);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is LoadThumbnailRequest)
                return Equals((LoadThumbnailRequest) obj);

            return false;
        }

        #region IEquatable<LoadThumbnailRequest> Members

        public bool Equals(LoadThumbnailRequest other)
        {
            return other != null && Equals(other.Descriptor, Descriptor) && other.Size.Equals(Size);
        }

        #endregion
    }

    public class LoadThumbnailResult
    {
        public LoadThumbnailResult(ThumbnailDescriptor descriptor, IImageData thumbnailData, Size size)
        {
            Platform.CheckForNullReference(descriptor, "descriptor");
            Platform.CheckForNullReference(thumbnailData, "thumbnailData");

            Size = size;
            Descriptor = descriptor;
            ThumbnailData = thumbnailData;
        }

        public LoadThumbnailResult(ThumbnailDescriptor descriptor, Exception error)
        {
            Platform.CheckForNullReference(descriptor, "descriptor");
            Platform.CheckForNullReference(error, "error");

            Descriptor = descriptor;
            Error = error;
        }

        public readonly ThumbnailDescriptor Descriptor;
        public readonly Size Size;
        public readonly IImageData ThumbnailData;
        public readonly Exception Error;
    }

    public interface IThumbnailLoader
    {
        IImageData GetDummyThumbnail(string message, Size size);
        IImageData GetErrorThumbnail(Size size);
        bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IImageData thumbnail);

        void LoadThumbnailAsync(LoadThumbnailRequest request);
        void Cancel(LoadThumbnailRequest request);
    }

    public class ThumbnailLoader : IThumbnailLoader
    {
        private readonly IThumbnailRepository _repository;
        private readonly object _syncLock = new object();
        private readonly List<LoadThumbnailRequest> _pendingRequests = new List<LoadThumbnailRequest>();
        private bool _isLoading;

        public ThumbnailLoader(IThumbnailRepository repository)
        {
            _repository = repository;
        }

        #region IThumbnailLoader Members

        public IImageData GetDummyThumbnail(string message, Size size)
        {
            return _repository.GetDummyThumbnail(message, size);
        }

        public IImageData GetErrorThumbnail(Size size)
        {
            return _repository.GetErrorThumbnail(size);
        }

        public bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IImageData bitmap)
        {
            return _repository.TryGetThumbnail(descriptor, size, out bitmap);
        }

        public void LoadThumbnailAsync(LoadThumbnailRequest request)
        {
            lock (_syncLock)
            {
                _pendingRequests.Add(request);
                if (_isLoading)
                    return;

                _isLoading = true;
                ThreadPool.QueueUserWorkItem(Load, null);
            }
        }

        public void Cancel(LoadThumbnailRequest request)
        {
            lock(_syncLock)
            {
                while (true)
                {
                    if (!_pendingRequests.Remove(request))
                        break;
                }
            }
        }

        #endregion

        public void Load(object state)
        {
            while (true)
            {
                LoadThumbnailRequest request;
                lock (_syncLock)
                {
                    if (_pendingRequests.Count == 0)
                    {
                        _isLoading = false;
                        break;
                    }

                    request = _pendingRequests[0];
                    _pendingRequests.RemoveAt(0);
                }

                LoadThumbnailResult result;

                try
                {
                    var image = _repository.GetThumbnail(request.Descriptor, request.Size);
                    result = new LoadThumbnailResult(request.Descriptor, image, request.Size);
                }
                catch (Exception e)
                {
                    result = new LoadThumbnailResult(request.Descriptor, e);
                }

                request.ResultCallback(result);
            }
        }
    }
}