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
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IImageData<T> : IImageData
    {
        new T Image { get; }
        new IImageData<T> Clone();
    }

    public interface IImageData : IDisposable
    {
        object Image { get; }
        IImageData Clone();
    }

    public interface IImageDataFactory
    {
        IImageData CreateImage(object sourceItem);
    }

    public interface IImageDataFactory<TSourceItem>
    {
        IImageData CreateImage(TSourceItem sourceItem);
    }
    
    public class MemoryStreamImageData : ImageDataBase<MemoryStream>
    {
        public MemoryStreamImageData(MemoryStream image)
            : base(image)
        {
        }

        public override IImageData<MemoryStream> Clone()
        {
            return new MemoryStreamImageData(new MemoryStream(Image.GetBuffer()));
        }
    }

    public class CloneableImageData<T> : ImageDataBase<T> where T : class, IDisposable, ICloneable
    {
        public CloneableImageData(T image)
            : base(image)
        {
        }

        public override IImageData<T> Clone()
        {
            return new CloneableImageData<T>((T)Image.Clone());
        }
    }
    
    public abstract class ImageDataBase<T> : IImageData<T> where T : class, IDisposable
    {
        protected ImageDataBase(T image)
        {
            Image = image;
        }

        #region IImageData<T> Members

        public T Image { get; private set; }
        public abstract IImageData<T> Clone();

        #endregion

        #region IImageData Members

        object IImageData.Image { get { return Image; } }

        IImageData IImageData.Clone()
        {
            return Clone();
        }
        
        #endregion

        private void Dispose(bool disposing)
        {
            if (!disposing || Image == null)
                return;

            Image.Dispose();
            Image = null;
        }

        #region IDisposable Members

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