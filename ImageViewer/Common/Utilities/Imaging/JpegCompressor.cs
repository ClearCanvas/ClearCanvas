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
using System.Drawing.Imaging;
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.Utilities.Imaging
{
    [ExtensionPoint]
    public class JpegCompressorExtensionPoint : ExtensionPoint<IJpegCompressor>
    {
    }

    public interface IJpegCompressor
    {
        void Compress(Bitmap image, int quality, MemoryStream memoryStream);
        MemoryStream Compress(Bitmap image, int quality);
    }

    public abstract class JpegCompressor : IJpegCompressor
    {
        private class DefaultCompressor : IJpegCompressor
        {
            private const string _mimeType = "image/jpeg";
            private readonly ImageCodecInfo _codec;

            public DefaultCompressor()
            {
                _codec = GetEncoderInfo();
            }

            private static ImageCodecInfo GetEncoderInfo()
            {
                int j;
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
                for (j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType.Equals(_mimeType))
                        return encoders[j];
                }
                return null;
            }

            #region IJpegCompressor Members

            public MemoryStream Compress(Bitmap image, int quality)
            {
                var memoryStream = new MemoryStream();
                Compress(image, quality, memoryStream);
                return memoryStream;
            }

            public void Compress(Bitmap image, int quality, MemoryStream memoryStream)
            {
                var eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                ImageCodecInfo ici = _codec;

                image.Save(memoryStream, ici, eps);
            }

            #endregion
        }

        public static IJpegCompressor Create()
        {
            try
            {
                return (IJpegCompressor)new JpegCompressorExtensionPoint().CreateExtension();

            }
            catch (NotSupportedException)
            {
            }
            
            return new DefaultCompressor();
        }

        #region IJpegCompressor Members

        public abstract void Compress(Bitmap image, int quality, MemoryStream memoryStream);
        public abstract MemoryStream Compress(Bitmap image, int quality);

        #endregion
    }
}
