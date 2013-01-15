#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU Lesser Public
// License as published by the Free Software Foundation, either version 3 of
// the License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser Public License for more details.
//
// You should have received a copy of the GNU Lesser Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.Utilities.Imaging
{
    public interface IPngEncoder
    {
        MemoryStream Encode(Bitmap bitmap);
        void Encode(Bitmap bitmap, MemoryStream memoryStream);
    }

    [ExtensionPoint]
    public class PngEncoderExtensionPoint : ExtensionPoint<IPngEncoder>
    {
    }

    public abstract class PngEncoder : IPngEncoder
    {
        private class DefaultEncoder : IPngEncoder
        {
            #region IPngEncoder Members

            public MemoryStream Encode(Bitmap bitmap)
            {
                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                return stream;
            }

            public void Encode(Bitmap bitmap, MemoryStream memoryStream)
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
            }

            #endregion
        }

        public abstract MemoryStream Encode(Bitmap bitmap);
        public abstract void Encode(Bitmap bitmap, MemoryStream memoryStream);

        public static IPngEncoder Create()
        {
            try
            {
                return (IPngEncoder)new PngEncoderExtensionPoint().CreateExtension();
            }
            catch (NotSupportedException)
            {
            }

            return new DefaultEncoder();
        }
    }
}
