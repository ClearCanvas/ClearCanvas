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
using System.Net;
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

    public abstract partial class PngEncoder : IPngEncoder
    {
        internal class ChunkInfo
        {
            public int DataLength;
            public string Type;

            public int TotalLength
            {
                get
                {
                    //Length,Type,CRC are all 4 bytes, plus the length of the actual data.
                    return ChunkInfoLength*3 + DataLength;
                }
            }

            public bool IsColorManagement
            {
                get { return IsColorManagementType(Type); }
            }
        }

        internal class DefaultEncoder : IPngEncoder
        {
            #region IPngEncoder Members

            public MemoryStream Encode(Bitmap bitmap)
            {
                using (var pngStream = new MemoryStream())
                {
                    bitmap.Save(pngStream, ImageFormat.Png);
                    var strippedStream = new MemoryStream();
                    StripColorManagement(pngStream, strippedStream);
                    return strippedStream;
                }
            }

            public void Encode(Bitmap bitmap, MemoryStream memoryStream)
            {
                using (var tempStream = new MemoryStream())
                {
                    bitmap.Save(tempStream, ImageFormat.Png);
                    StripColorManagement(tempStream, memoryStream);
                }
            }

            #endregion
        }

        public abstract MemoryStream Encode(Bitmap bitmap);
        public abstract void Encode(Bitmap bitmap, MemoryStream memoryStream);

        public static IPngEncoder Create()
        {
            try
            {
                return (IPngEncoder) new PngEncoderExtensionPoint().CreateExtension();
            }
            catch (NotSupportedException)
            {
            }

            return new DefaultEncoder();
        }

    }

    #region Parser

    public abstract partial class PngEncoder
    {
        //Length of chunk length, type and CRC sections.
        internal const int ChunkInfoLength = 4;

        private static readonly string[] _colorManagementTypes = new string[] {"gAMA", "iCCP", "sRGB", "cHRM"};

        //All PNGs have this exact 8 byte header.
        private static readonly byte[] _pngHeader = new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A};

        internal static void StripColorManagement(MemoryStream pngStream, MemoryStream outStream)
        {
            pngStream.Position = outStream.Position = 0;
            var header = ReadHeader(pngStream);

            //write the header
            outStream.Write(header, 0, header.Length);

            const int chunkInfoLength = 4;
            const int dataBufferLength = 16384;
            var lengthBuffer = new byte[chunkInfoLength];
            var typeBuffer = new byte[chunkInfoLength];
            var crcBuffer = new byte[chunkInfoLength];
            var dataBuffer = new byte[dataBufferLength];

            while (pngStream.Position < pngStream.Length)
            {
                //Read chunk length
                int chunkDataLength = ReadChunkDataLength(pngStream, lengthBuffer);
                string chunkType = ReadChunkType(pngStream, typeBuffer);

                if (IsColorManagementType(chunkType))
                {
                    //Skip this whole chunk
                    SkipChunkDataAndCRC(pngStream, chunkDataLength);
                    continue;
                }

                //Write out the chunk length and type
                outStream.Write(lengthBuffer, 0, lengthBuffer.Length);
                outStream.Write(typeBuffer, 0, typeBuffer.Length);

                //Read the chunk data in and then write it directly out.
                var bytesLeftToRead = chunkDataLength;
                while (bytesLeftToRead > 0)
                {
                    //Read the smaller of the bytes left to be read, or the buffer's actual length
                    var bytesToRead = Math.Min(dataBuffer.Length, bytesLeftToRead);
                    int bytesRead = pngStream.Read(dataBuffer, 0, bytesToRead);
                    if (bytesToRead != bytesRead)
                        throw new System.IO.InvalidDataException("PNG appears to be truncated.");

                    outStream.Write(dataBuffer, 0, bytesRead);
                    bytesLeftToRead -= bytesRead;
                }

                //Read the CRC and write it out.
                if (chunkInfoLength != pngStream.Read(crcBuffer, 0, chunkInfoLength))
                    throw new System.IO.InvalidDataException("PNG must have a 4 byte CRC field after data.");

                outStream.Write(crcBuffer, 0, chunkInfoLength);
            }

            //Nothing got written out but the header.
            if (outStream.Length <= _pngHeader.Length)
                throw new System.IO.InvalidDataException("PNG stream is invalid.");

            outStream.Position = 0;
        }

        internal static ChunkInfo[] GetChunkInfo(MemoryStream pngStream)
        {
            var chunkInfo = new List<ChunkInfo>();

            pngStream.Position = 0;
            ReadHeader(pngStream);

            const int chunkInfoLength = 4;
            var lengthBuffer = new byte[chunkInfoLength];
            var typeBuffer = new byte[chunkInfoLength];
            
            while (pngStream.Position < pngStream.Length)
            {
                int dataLength = ReadChunkDataLength(pngStream, lengthBuffer);
                string type = ReadChunkType(pngStream, typeBuffer);
                chunkInfo.Add(new ChunkInfo{Type = type, DataLength = dataLength});
                SkipChunkDataAndCRC(pngStream, dataLength);
            }

            return chunkInfo.ToArray();
        }

        internal static byte[] ReadHeader(MemoryStream pngStream)
        {
            //Since we're reading directly from a PNG we just saved, all these checks are kinda overkill, but it doesn't hurt in case we use it elsewhere later.
            var header = new byte[_pngHeader.Length];
            //read the 8-byte PNG header
            var bytesRead = pngStream.Read(header, 0, header.Length);
            if (bytesRead != header.Length)
                throw new System.IO.InvalidDataException("PNG header must be 8 bytes long.");

            if (!header.SequenceEqual(_pngHeader))
                throw new System.IO.InvalidDataException("PNG has an invalid header.");
            return header;
        }

        internal static int ReadChunkDataLength(MemoryStream pngStream, byte[] lengthBuffer)
        {
            if (lengthBuffer.Length != ChunkInfoLength)
                throw new ArgumentException("Input buffer must be 4 bytes long.");

            var bytesRead = pngStream.Read(lengthBuffer, 0, ChunkInfoLength);
            if (bytesRead != ChunkInfoLength)
                throw new System.IO.InvalidDataException("PNG chunk must have a 4 byte length field at the beginning.");

            unsafe
            {
                fixed (byte* length = lengthBuffer)
                {
                    //Network order is big endian, which is how PNGs are encoded, so convert to machine format.
                    return IPAddress.NetworkToHostOrder(*(Int32*) length);
                }
            }
        }

        internal static string ReadChunkType(MemoryStream pngStream, byte[] typeBuffer)
        {
            if (typeBuffer.Length != ChunkInfoLength)
                throw new ArgumentException("Input buffer must be 4 bytes long.");

            //Read chunk type
            if (ChunkInfoLength != pngStream.Read(typeBuffer, 0, ChunkInfoLength))
                throw new System.IO.InvalidDataException("PNG chunk must have a 4 byte type following the length.");

            return Encoding.ASCII.GetString(typeBuffer);
        }

        internal static void SkipChunkDataAndCRC(MemoryStream pngStream, int dataLength)
        {
            pngStream.Position += dataLength; //skip value
            pngStream.Position += ChunkInfoLength; //skip CRC
        }

        internal static bool IsColorManagementType(string type)
        {
            return _colorManagementTypes.Contains(type);
        }
    }

    #endregion
}
