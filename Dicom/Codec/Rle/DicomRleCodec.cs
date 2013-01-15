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

#region Inline Attributions
// The source code contained in this file is based on an original work
// from
//
// mDCM: A C# DICOM library
//
// Copyright (c) 2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Codec.Rle
{
    /// <summary>
    /// Parameters for RLE compression.
    /// </summary>
    public class DicomRleCodecParameters : DicomCodecParameters
    {
        #region Private Members
        bool _reverseByteOrder;
        #endregion

        #region Public Members
        public DicomRleCodecParameters()
        {
            if (ByteBuffer.LocalMachineEndian == Endian.Little)
                _reverseByteOrder = false;
            else
                _reverseByteOrder = true;
        }

        public DicomRleCodecParameters(bool reverseByteOrder)
        {
            _reverseByteOrder = reverseByteOrder;
        }
        #endregion

        #region Public Properties
        public bool ReverseByteOrder
        {
            get { return _reverseByteOrder; }
            set { _reverseByteOrder = value; }
        }
        #endregion
    }

    /// <summary>
    /// The DICOM RLE Transfer Syntax codec.
    /// </summary>
    public class DicomRleCodec : IDicomCodec
    {
        public string Name
        {
            get { return "RLE Lossless"; }
        }

        public TransferSyntax CodecTransferSyntax
        {
            get { return TransferSyntax.RleLossless; }
        }


        #region Encode
        private class RLEEncoder
        {
            #region Private Members
            private int _count;
            private readonly uint[] _offsets;
            private readonly MemoryStream _stream;
            private readonly BinaryWriter _writer;
            private readonly byte[] _buffer;

            private int _prevByte;
            private int _repeatCount;
            private int _bufferPos;
            #endregion

            #region Public Constructors
            public RLEEncoder()
            {
                _count = 0;
                _offsets = new uint[15];
                _stream = new MemoryStream();
                _writer = EndianBinaryWriter.Create(_stream, Endian.Little);
                _buffer = new byte[132];
                WriteHeader();

                _prevByte = -1;
                _repeatCount = 0;
                _bufferPos = 0;
            }
            #endregion

            #region Public Members
            public int NumberOfSegments
            {
                get { return _count; }
            }

        	private long Length
            {
                get { return _stream.Length; }
            }

            public byte[] GetBuffer()
            {
                Flush();
                WriteHeader();
                return _stream.ToArray();
            }

            public void NextSegment()
            {
                Flush();
                if ((Length & 1) == 1)
                    _stream.WriteByte(0x00);
                _offsets[_count++] = (uint)_stream.Length;
            }

            public void Encode(byte b)
            {
                if (b == _prevByte)
                {
                    _repeatCount++;

                    if (_repeatCount > 2 && _bufferPos > 0)
                    {
                        // We're starting a run, flush out the buffer
                        while (_bufferPos > 0)
                        {
                            int count = Math.Min(128, _bufferPos);
                            _stream.WriteByte((byte)(count - 1));
                            MoveBuffer(count);
                        }
                    }
                    else if (_repeatCount > 128)
                    {
                        int count = Math.Min(_repeatCount, 128);
                        _stream.WriteByte((byte)(257 - count));
                        _stream.WriteByte((byte)_prevByte);
                        _repeatCount -= count;
                    }
                }
                else
                {
                    switch (_repeatCount)
                    {
                        case 0:
                            break;
                        case 1:
                            {
                                _buffer[_bufferPos++] = (byte)_prevByte;
                                break;
                            }
                        case 2:
                            {
                                _buffer[_bufferPos++] = (byte)_prevByte;
                                _buffer[_bufferPos++] = (byte)_prevByte;
                                break;
                            }
                        default:
                            {
                                while (_repeatCount > 0)
                                {
                                    int count = Math.Min(_repeatCount, 128);
                                    _stream.WriteByte((byte)(257 - count));
                                    _stream.WriteByte((byte)_prevByte);
                                    _repeatCount -= count;
                                }

                                break;
                            }
                    }

                    while (_bufferPos > 128)
                    {
                        int count = Math.Min(128, _bufferPos);
                        _stream.WriteByte((byte)(count - 1));
                        MoveBuffer(count);
                    }

                    _prevByte = b;
                    _repeatCount = 1;
                }
            }

            public void MakeEvenLength()
            {
                // Make even length
                if (_stream.Length % 2 == 1)
                    _stream.WriteByte(0);
            }

            public void Flush()
            {
                if (_repeatCount < 2)
                {
                    while (_repeatCount > 0)
                    {
                        _buffer[_bufferPos++] = (byte)_prevByte;
                        _repeatCount--;
                    }
                }

                while (_bufferPos > 0)
                {
                    int count = Math.Min(128, _bufferPos);
                    _stream.WriteByte((byte)(count - 1));
                    MoveBuffer(count);
                }

                if (_repeatCount >= 2)
                {
                    while (_repeatCount > 0)
                    {
                        int count = Math.Min(_repeatCount, 128);
                        _stream.WriteByte((byte)(257 - count));
                        _stream.WriteByte((byte)_prevByte);
                        _repeatCount -= count;
                    }
                }

                _prevByte = -1;
                _repeatCount = 0;
                _bufferPos = 0;
            }
            #endregion

            #region Private Members
            private void MoveBuffer(int count)
            {
                _stream.Write(_buffer, 0, count);
                for (int i = count, n = 0; i < _bufferPos; i++, n++)
                {
                    _buffer[n] = _buffer[i];
                }
                _bufferPos = _bufferPos - count;
            }

            private void WriteHeader()
            {
                _stream.Seek(0, SeekOrigin.Begin);
                _writer.Write((uint)_count);
                for (int i = 0; i < 15; i++)
                {
                    _writer.Write(_offsets[i]);
                }
            }
            #endregion
        }

        public void Encode(DicomUncompressedPixelData oldPixelData, DicomCompressedPixelData newPixelData, DicomCodecParameters parameters)
        {
            DicomRleCodecParameters rleParams = parameters as DicomRleCodecParameters ?? new DicomRleCodecParameters();

            // Convert to RGB
			if (oldPixelData.HasPaletteColorLut && parameters.ConvertPaletteToRGB)
			{
				oldPixelData.ConvertPaletteColorToRgb();
				newPixelData.HasPaletteColorLut = false;
				newPixelData.SamplesPerPixel = oldPixelData.SamplesPerPixel;
				newPixelData.PlanarConfiguration = oldPixelData.PlanarConfiguration;
				newPixelData.PhotometricInterpretation = oldPixelData.PhotometricInterpretation;
			}

        	int pixelCount = oldPixelData.ImageWidth * oldPixelData.ImageHeight;
            int numberOfSegments = oldPixelData.BytesAllocated * oldPixelData.SamplesPerPixel;

            for (int i = 0; i < oldPixelData.NumberOfFrames; i++)
            {
                RLEEncoder encoder = new RLEEncoder();
                byte[] frameData = oldPixelData.GetFrame(i);

                for (int s = 0; s < numberOfSegments; s++)
                {
                    encoder.NextSegment();

                    int sample = s / oldPixelData.BytesAllocated;
                    int sabyte = s % oldPixelData.BytesAllocated;

                    int pos;
                    int offset;

                    if (newPixelData.PlanarConfiguration == 0)
                    {
                        pos = sample * oldPixelData.BytesAllocated;
                        offset = numberOfSegments;
                    }
                    else
                    {
                        pos = sample * oldPixelData.BytesAllocated * pixelCount;
                        offset = oldPixelData.BytesAllocated;
                    }

                    if (rleParams.ReverseByteOrder)
                        pos += sabyte;
                    else
                        pos += oldPixelData.BytesAllocated - sabyte - 1;

                    for (int p = 0; p < pixelCount; p++)
                    {
                        if (pos >= frameData.Length)
                            throw new DicomCodecException("");
                        encoder.Encode(frameData[pos]);
                        pos += offset;
                    }
                    encoder.Flush();
                }

                encoder.MakeEvenLength();

                newPixelData.AddFrameFragment(encoder.GetBuffer());
            }
        }
        #endregion

        #region Decode
        private class RLEDecoder
        {
            #region Private Members
            private readonly int _count;
            private readonly int[] _offsets;
            private readonly byte[] _data;
            #endregion

            #region Public Constructors
            public RLEDecoder(IList<DicomFragment> data)
            {
            	uint size = 0;
            	foreach (DicomFragment frag in data)
                    size += frag.Length;
                MemoryStream stream = new MemoryStream(data[0].GetByteArray());
                for (int i = 1; i < data.Count; i++)
                {
                    stream.Seek(0, SeekOrigin.End);
                    byte[] ba = data[i].GetByteArray();
                    stream.Write(ba, 0, ba.Length);
                }
                BinaryReader reader = EndianBinaryReader.Create(stream, Endian.Little);
                _count = (int)reader.ReadUInt32();
                _offsets = new int[15];
                for (int i = 0; i < 15; i++)
                {
                    _offsets[i] = reader.ReadInt32();
                }
                _data = new byte[stream.Length - 64]; // take off 64 bytes for the offsets
                stream.Read(_data, 0, _data.Length);
            }
            #endregion

            #region Public Members
            public int NumberOfSegments
            {
                get { return _count; }
            }

            public void DecodeSegment(int segment, byte[] buffer)
            {
                if (segment < 0 || segment >= _count)
                    throw new IndexOutOfRangeException("Segment number out of range");

                int offset = GetSegmentOffset(segment);
                int length = GetSegmentLength(segment);

                Decode(buffer, _data, offset, length);
            }

            private static void Decode(byte[] buffer, byte[] rleData, int offset, int count)
            {
				// Note: SB - this is a literal translation of the decoder as described in
				// the Dicom standard.  It works exactly the same way as the existing code
				// but would be easier to make unsafe if we wanted boost performance.
				// Rewrote it while fixing #2349 to make sure the existing code was correct (and it is).

				//int bufferPos = 0;
				//for (int rlePos = offset; rlePos < offset + count; )
				//{
				//    if (rlePos > rleData.Length - 1)
				//        return; //shouldn't happen, write to the log
				//    if (bufferPos >= buffer.Length)
				//        return; //shouldn't happen, write to the log
				
				//    sbyte n = (sbyte)rleData[rlePos++];
				//    if (n >= 0 && n <= 127)
				//    {
				//        int numberBytesToOutput = n + 1;
				//        int bufferBytesRemaining = buffer.Length - bufferPos - 1;
				//        int rleBytesRemaining = rleData.Length - rlePos - 1;
				//        if (numberBytesToOutput > bufferBytesRemaining || numberBytesToOutput > rleBytesRemaining)
				//        {
				//				//shouldn't happen, write to the log
				//            numberBytesToOutput = Math.Min(bufferBytesRemaining, rleBytesRemaining);
				//        }

				//        Array.Copy(rleData, rlePos, buffer, bufferPos, numberBytesToOutput);
				//        bufferPos += numberBytesToOutput;
				//        rlePos += numberBytesToOutput;
				//    }
				//    else if (n <= -1 && n >= -127)
				//    {
				//        byte value = rleData[rlePos++];
				//        int repeatCount = -n + 1;
				//        for (int j = 0; j < repeatCount; ++j)
				//        {
				//            if (bufferPos >= buffer.Length)
				//                break;//shouldn't happen, write to the log

				//            buffer[bufferPos++] = value;
				//        }
				//    }
				//}

                int pos = 0;
                int end = offset + count;
                for (int i = offset; i < end; )
                {
                    int n = rleData[i++];
                    if ((n & 0x80) != 0)
                    {
                        int c = 257 - n;
                        if (i >= end)
                        {
                            Platform.Log(LogLevel.Error, "RLE Segement unexpectedly wrong.");
                            return;
                        }
                        byte b = rleData[i++];
                        while (c-- > 0)
                        {
                            if (pos >= buffer.Length)
                            {
								Platform.Log(LogLevel.Error, "RLE segment unexpectedly too long.  Ignoring data.");
                                return;
                            }
                            buffer[pos++] = b;
                        }
                    }
                    else
                    {
                        if (n == 0 && i == end) // Single padding char
                            return;
                        int c = (n & 0x7F) + 1;
                        if ((i + c) >= end)
                        {
                            c = offset + count - i;
                        }
                        if (i > rleData.Length || pos + c > buffer.Length)
                        {
							Platform.Log(LogLevel.Error, "Invalid formatted RLE data.  RLE segment unexpectedly too long.");
                            return;
                        }

                        Array.Copy(rleData, i, buffer, pos, c);
                        pos += c;
                        i += c;
                    }
                }
            }
            #endregion

            #region Private Members
            private int GetSegmentOffset(int segment)
            {
                return _offsets[segment] - 64;
            }

            private int GetSegmentLength(int segment)
            {
                int offset = GetSegmentOffset(segment);
                if (segment < (_count - 1))
                {
                    int next = GetSegmentOffset(segment + 1);
                    return next - offset;
                }
            	return _data.Length - offset;
            }
            #endregion
        }

        public void Decode(DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters)
        {
            DicomRleCodecParameters rleParams = parameters as DicomRleCodecParameters ?? new DicomRleCodecParameters();

            int pixelCount = oldPixelData.ImageWidth * oldPixelData.ImageHeight;
            int numberOfSegments = oldPixelData.BytesAllocated * oldPixelData.SamplesPerPixel;
            int segmentLength = (pixelCount & 1) == 1 ? pixelCount + 1 : pixelCount;

            byte[] segment = new byte[segmentLength];
            byte[] frameData = new byte[oldPixelData.UncompressedFrameSize];

            for (int i = 0; i < oldPixelData.NumberOfFrames; i++)
            {
                IList<DicomFragment> rleData = oldPixelData.GetFrameFragments(i);
                RLEDecoder decoder = new RLEDecoder(rleData);

                if (decoder.NumberOfSegments != numberOfSegments)
                    throw new DicomCodecException("Unexpected number of RLE segments!");

                for (int s = 0; s < numberOfSegments; s++)
                {
                    decoder.DecodeSegment(s, segment);

                    int sample = s / oldPixelData.BytesAllocated;
                    int sabyte = s % oldPixelData.BytesAllocated;

                    int pos;
                    int offset;

                    if (newPixelData.PlanarConfiguration == 0)
                    {
                        pos = sample * oldPixelData.BytesAllocated;
                        offset = oldPixelData.SamplesPerPixel * oldPixelData.BytesAllocated;
                    }
                    else
                    {
                        pos = sample * oldPixelData.BytesAllocated * pixelCount;
                        offset = oldPixelData.BytesAllocated;
                    }

                    if (rleParams.ReverseByteOrder)
                        pos += sabyte;
                    else
                        pos += oldPixelData.BytesAllocated - sabyte - 1;

                    for (int p = 0; p < pixelCount; p++)
                    {
                        frameData[pos] = segment[p];
                        pos += offset;
                    }
                }

                newPixelData.AppendFrame(frameData);
            }
        }

        public void DecodeFrame(int frame, DicomCompressedPixelData oldPixelData,
                                DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters)
        {
            DicomRleCodecParameters rleParams = parameters as DicomRleCodecParameters ?? new DicomRleCodecParameters();

            int pixelCount = oldPixelData.ImageWidth * oldPixelData.ImageHeight;
            int numberOfSegments = oldPixelData.BytesAllocated * oldPixelData.SamplesPerPixel;
            int segmentLength = (pixelCount & 1) == 1 ? pixelCount + 1 : pixelCount;

            byte[] segment = new byte[segmentLength];
            byte[] frameData = new byte[oldPixelData.UncompressedFrameSize];

            IList<DicomFragment> rleData = oldPixelData.GetFrameFragments(frame);
            RLEDecoder decoder = new RLEDecoder(rleData);

            if (decoder.NumberOfSegments != numberOfSegments)
                throw new DicomCodecException("Unexpected number of RLE segments!");

            for (int s = 0; s < numberOfSegments; s++)
            {
                decoder.DecodeSegment(s, segment);

                int sample = s / oldPixelData.BytesAllocated;
                int sabyte = s % oldPixelData.BytesAllocated;

                int pos;
                int offset;

                if (newPixelData.PlanarConfiguration == 0)
                {
                    pos = sample * oldPixelData.BytesAllocated;
                    offset = oldPixelData.SamplesPerPixel * oldPixelData.BytesAllocated;
                }
                else
                {
                    pos = sample * oldPixelData.BytesAllocated * pixelCount;
                    offset = oldPixelData.BytesAllocated;
                }

                if (rleParams.ReverseByteOrder)
                    pos += sabyte;
                else
                    pos += oldPixelData.BytesAllocated - sabyte - 1;

                for (int p = 0; p < pixelCount; p++)
                {
                    frameData[pos] = segment[p];
                    pos += offset;
                }
            }

            newPixelData.AppendFrame(frameData);
        }

        #endregion
    }
}