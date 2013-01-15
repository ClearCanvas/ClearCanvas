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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailRepository
    {
        IImageData GetDummyThumbnail(string message, Size size);
        IImageData GetErrorThumbnail(Size size);

        bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IImageData thumbnail);
        IImageData GetThumbnail(ThumbnailDescriptor descriptor, Size size);
    }

    public abstract class ThumbnailRepository : IThumbnailRepository
    {
        #region IThumbnailRepository Members

        public abstract IImageData GetDummyThumbnail(string message, Size size);
        public abstract IImageData GetErrorThumbnail(Size size);

        public abstract bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IImageData thumbnail);
        public abstract IImageData GetThumbnail(ThumbnailDescriptor descriptor, Size size);

        #endregion

        public static IThumbnailRepository Create()
        {
            //TODO:!change back to use caching
            //if (ThumbnailCache.IsSupported)
            //    return new CachingThumbnailRepository(ThumbnailCache.Create("viewer-display-sets"));

            return new NullThumbnailRepository();
        }
    }

    internal class NullThumbnailRepository : ThumbnailRepository
    {
        private readonly IThumbnailFactory<IPresentationImage> _factory;

        public NullThumbnailRepository()
            : this(new ThumbnailFactory())
        {
        }

        public NullThumbnailRepository(BitmapConverter bitmapConverter)
            : this(new ThumbnailFactory(bitmapConverter))
        {
        }

        public NullThumbnailRepository(IThumbnailFactory<IPresentationImage> factory)
        {
            Platform.CheckForNullReference(factory, "factory");
            _factory = factory;
        }

        public override bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IImageData thumbnail)
        {
            thumbnail = null;
            return false;
        }

        public override IImageData GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            return _factory.CreateImage(descriptor.ReferenceImage, size);
        }

        public override IImageData GetDummyThumbnail(string message, Size size)
        {
            return _factory.CreateDummy(message, size);
        }

        public override IImageData GetErrorThumbnail(Size size)
        {
            return _factory.CreateError(size);
        }
    }

/*
    internal class CachingThumbnailRepository : ThumbnailRepository
    {
        private readonly ICache<IImageData> _cache;
        private readonly IThumbnailFactory<IPresentationImage> _factory;

        public CachingThumbnailRepository()
            : this(new ThumbnailFactory())
        {
        }

        public CachingThumbnailRepository(BitmapConverter bitmapConverter)
            : this(new ThumbnailFactory(bitmapConverter))
        {
        }

        public CachingThumbnailRepository(BitmapConverter bitmapConverter, ICache<IImageData> cache)
            : this(new ThumbnailFactory(bitmapConverter), cache)
        {
        }

        public CachingThumbnailRepository(IThumbnailFactory<IPresentationImage> factory)
        {
            Platform.CheckForNullReference(factory, "factory");
            _factory = factory;
        }

        public CachingThumbnailRepository(IThumbnailFactory<IPresentationImage> factory, ICache<IImageData> cache)
        {
            Platform.CheckForNullReference(factory, "factory");
            Platform.CheckForNullReference(cache, "cache");

            _factory = factory;
            _cache = cache;
        }

        public override bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IImageData thumbnail)
        {
            string key = descriptor.Identifier;
            if (!String.IsNullOrEmpty(key))
            {
                thumbnail = _cache.Get(key);
                if (thumbnail != null)
                    return true;
            }

            thumbnail = null;
            return false;
        }

        public override IImageData GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            string key = descriptor.Identifier;
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Thumbnail descriptor must have a unique identifier.");

            var thumbnail = _cache.Get(key);
            if (thumbnail != null)
                return thumbnail;

            thumbnail = _factory.CreateImage(descriptor.ReferenceImage, size);
            _cache.Put(key, thumbnail);
            return thumbnail;
        }

        public override IImageData GetDummyThumbnail(string message, Size size)
        {
            //TODO: bother caching these, seeing as we end up creating copies of them anyway?
            return _factory.CreateDummy(message, size);
        }

        public override IImageData GetErrorThumbnail(Size size)
        {
            return _factory.CreateError(size);
        }
    }
 */
}