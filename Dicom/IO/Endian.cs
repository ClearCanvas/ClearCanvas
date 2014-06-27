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
        private bool _swapBytes;
        private byte[] _internalBuffer = new byte[8];
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
	            return new EndianBinaryReader(s, e);
            }
	        if (Endian.Big == e)
	        {
		        return new BinaryReader(s);
	        }
	        return new EndianBinaryReader(s, e);
        }
        #endregion

        #region Public Properties
        public Endian Endian
        {
            get
            {
	            if (BitConverter.IsLittleEndian)
                {
                    return _swapBytes ? Endian.Big : Endian.Little;
                }
	            return _swapBytes ? Endian.Little : Endian.Big;
            }
			set
			{
				_swapBytes = BitConverter.IsLittleEndian
					            ? Endian.Big == value
					            : Endian.Little == value;
			}
        }

        public bool UseInternalBuffer
        {
            get
            {
                return (_internalBuffer != null);
            }
            set
            {
                if (value && (_internalBuffer == null))
                {
                    _internalBuffer = new byte[8];
                }
                else
                {
                    _internalBuffer = null;
                }
            }
        }
        #endregion

        #region Private Methods
        private byte[] ReadBytesInternal(int count)
        {
            byte[] buffer;
            if (_internalBuffer != null)
            {
                base.Read(_internalBuffer, 0, count);
                buffer = _internalBuffer;
            }
            else
            {
                buffer = base.ReadBytes(count);
            }
            if (_swapBytes)
            {
                Array.Reverse(buffer, 0, count);
            }
            return buffer;
        }
        #endregion

        #region BinaryReader Overrides
        public override short ReadInt16()
        {
            if (_swapBytes)
            {
                return IPAddress.NetworkToHostOrder(base.ReadInt16());
            }
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            if (_swapBytes)
            {
                return IPAddress.NetworkToHostOrder(base.ReadInt32());
            }
            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            if (_swapBytes)
            {
                return IPAddress.NetworkToHostOrder(base.ReadInt64());
            }
            return base.ReadInt64();
        }

        public override float ReadSingle()
        {
            if (_swapBytes)
            {
                byte[] b = ReadBytesInternal(4);
                return BitConverter.ToSingle(b, 0);
            }
            return base.ReadSingle();
        }

        public override double ReadDouble()
        {
            if (_swapBytes)
            {
                byte[] b = ReadBytesInternal(8);
                return BitConverter.ToDouble(b, 0);
            }
            return base.ReadDouble();
        }

        public override ushort ReadUInt16()
        {
            if (_swapBytes)
            {
                ushort u = base.ReadUInt16();
                return unchecked((ushort)((u >> 8) | (u << 8)));
            }
            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            if (_swapBytes)
            {
                uint u = base.ReadUInt32();
                return unchecked((u >> 24) | ((u >> 8) & 0xFF00) | ((u << 8) & 0xFF0000) | (u << 24));
            }
            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            if (_swapBytes)
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
        private bool _swapBytes;
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
	            return new EndianBinaryWriter(s, e);
            }
	        if (Endian.Big == e)
	        {
		        return new BinaryWriter(s);
	        }
	        return new EndianBinaryWriter(s, e);
        }

	    #endregion

        #region Public Properties
        public Endian Endian
        {
            get
            {
	            if (BitConverter.IsLittleEndian)
                {
                    return _swapBytes ? Endian.Big : Endian.Little;
                }
	            return _swapBytes ? Endian.Little : Endian.Big;
            }
	        set
            {
                if (BitConverter.IsLittleEndian)
                {
                    _swapBytes = (Endian.Big == value);
                }
                else
                {
                    _swapBytes = (Endian.Little == value);
                }
            }
        }
        #endregion

        #region Private Methods
        private void WriteInternal(byte[] buffer)
        {
            if (_swapBytes)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }
        #endregion

        #region BinaryWriter Overrides
        public override void Write(double value)
        {
            if (_swapBytes)
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
            if (_swapBytes)
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
            if (_swapBytes)
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
            if (_swapBytes)
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
            if (_swapBytes)
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
            if (_swapBytes)
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
            if (_swapBytes)
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
            if (_swapBytes)
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

