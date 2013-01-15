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
using System.Net;
using System.Text;

namespace ClearCanvas.Dicom.IO
{
    #region EndianBinaryReader
    public class EndianBinaryReader : BinaryReader
    {
        #region Private Members
        private bool SwapBytes = false;
        private byte[] InternalBuffer = new byte[8];
        #endregion

        #region Public Constructors
        public EndianBinaryReader(Stream s)
            : base(s)
        {
        }
        public EndianBinaryReader(Stream s, Encoding e)
            : base(s, e)
        {
        }
        public EndianBinaryReader(Stream s, Endian e)
            : base(s)
        {
            Endian = e;
        }
        public EndianBinaryReader(Stream s, Encoding enc, Endian end)
            : base(s, enc)
        {
            Endian = end;
        }

        public static BinaryReader Create(Stream s, Endian e)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (Endian.Little == e)
                {
                    return new BinaryReader(s);
                }
                else
                {
                    return new EndianBinaryReader(s, e);
                }
            }
            else
            {
                if (Endian.Big == e)
                {
                    return new BinaryReader(s);
                }
                else
                {
                    return new EndianBinaryReader(s, e);
                }
            }
        }
        #endregion

        #region Public Properties
        public Endian Endian
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                {
                    return SwapBytes ? Endian.Big : Endian.Little;
                }
                else
                {
                    return SwapBytes ? Endian.Little : Endian.Big;
                }
            }
            set
            {
                if (BitConverter.IsLittleEndian)
                {
                    SwapBytes = (Endian.Big == value);
                }
                else
                {
                    SwapBytes = (Endian.Little == value);
                }
            }
        }

        public bool UseInternalBuffer
        {
            get
            {
                return (InternalBuffer != null);
            }
            set
            {
                if (value && (InternalBuffer == null))
                {
                    InternalBuffer = new byte[8];
                }
                else
                {
                    InternalBuffer = null;
                }
            }
        }
        #endregion

        #region Private Methods
        private byte[] ReadBytesInternal(int count)
        {
            byte[] Buffer = null;
            if (InternalBuffer != null)
            {
                base.Read(InternalBuffer, 0, count);
                Buffer = InternalBuffer;
            }
            else
            {
                Buffer = base.ReadBytes(count);
            }
            if (SwapBytes)
            {
                Array.Reverse(Buffer, 0, count);
            }
            return Buffer;
        }
        #endregion

        #region BinaryReader Overrides
        public override short ReadInt16()
        {
            if (SwapBytes)
            {
                return IPAddress.NetworkToHostOrder(base.ReadInt16());
            }
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            if (SwapBytes)
            {
                return IPAddress.NetworkToHostOrder(base.ReadInt32());
            }
            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            if (SwapBytes)
            {
                return IPAddress.NetworkToHostOrder(base.ReadInt64());
            }
            return base.ReadInt64();
        }

        public override float ReadSingle()
        {
            if (SwapBytes)
            {
                byte[] b = ReadBytesInternal(4);
                return BitConverter.ToSingle(b, 0);
            }
            return base.ReadSingle();
        }

        public override double ReadDouble()
        {
            if (SwapBytes)
            {
                byte[] b = ReadBytesInternal(8);
                return BitConverter.ToDouble(b, 0);
            }
            return base.ReadDouble();
        }

        public override ushort ReadUInt16()
        {
            if (SwapBytes)
            {
                ushort u = base.ReadUInt16();
                return unchecked((ushort)((u >> 8) | (u << 8)));
            }
            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            if (SwapBytes)
            {
                uint u = base.ReadUInt32();
                return unchecked((u >> 24) | ((u >> 8) & 0xFF00) | ((u << 8) & 0xFF0000) | (u << 24));
            }
            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            if (SwapBytes)
            {
                byte[] b = ReadBytesInternal(8);
                return BitConverter.ToUInt64(b, 0);
            }
            return base.ReadUInt64();
        }
        #endregion
    }
    #endregion

    #region EndianBinaryWriter
    public class EndianBinaryWriter : BinaryWriter
    {
        #region Private Members
        private bool SwapBytes = false;
        #endregion

        #region Public Constructors
        public EndianBinaryWriter(Stream s)
            : base(s)
        {
        }
        public EndianBinaryWriter(Stream s, Encoding e)
            : base(s, e)
        {
        }
        public EndianBinaryWriter(Stream s, Endian e)
            : base(s)
        {
            Endian = e;
        }
        public EndianBinaryWriter(Stream s, Encoding enc, Endian end)
            : base(s, enc)
        {
            Endian = end;
        }

        public static BinaryWriter Create(Stream s, Endian e)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (Endian.Little == e)
                {
                    return new BinaryWriter(s);
                }
                else
                {
                    return new EndianBinaryWriter(s, e);
                }
            }
            else
            {
                if (Endian.Big == e)
                {
                    return new BinaryWriter(s);
                }
                else
                {
                    return new EndianBinaryWriter(s, e);
                }
            }
        }
        #endregion

        #region Public Properties
        public Endian Endian
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                {
                    return SwapBytes ? Endian.Big : Endian.Little;
                }
                else
                {
                    return SwapBytes ? Endian.Little : Endian.Big;
                }
            }
            set
            {
                if (BitConverter.IsLittleEndian)
                {
                    SwapBytes = (Endian.Big == value);
                }
                else
                {
                    SwapBytes = (Endian.Little == value);
                }
            }
        }
        #endregion

        #region Private Methods
        private void WriteInternal(byte[] Buffer)
        {
            if (SwapBytes)
            {
                Array.Reverse(Buffer);
            }
            base.Write(Buffer);
        }
        #endregion

        #region BinaryWriter Overrides
        public override void Write(double value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }

        public override void Write(float value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }

        public override void Write(int value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }

        public override void Write(long value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }

        public override void Write(short value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }

        public override void Write(uint value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }

        public override void Write(ulong value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }

        public override void Write(ushort value)
        {
            if (SwapBytes)
            {
                byte[] b = BitConverter.GetBytes(value);
                WriteInternal(b);
            }
            else
            {
                base.Write(value);
            }
        }
        #endregion
    }
    #endregion

}

