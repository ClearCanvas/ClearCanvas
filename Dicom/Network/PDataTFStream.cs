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

namespace ClearCanvas.Dicom.Network
{
    internal class PDataTFStream : Stream
    {
        public delegate void TickDelegate();

        #region Private Members
        private bool _command;
        private readonly uint _max;
        private readonly byte _pcid;
        private PDataTF _pdu;
        private byte[] _bytes;
        private MemoryStream _buffer;
        private readonly NetworkBase _networkBase;
    	private readonly bool _combineCommandData;
        #endregion

        #region Public Constructors
        public PDataTFStream(NetworkBase networkBase, byte pcid, uint max, uint total, bool combineCommandData)
        {
            _command = true;
            _pcid = pcid;
            _max = max;
            _pdu = new PDataTF();
            _buffer = new MemoryStream((int)total + 1024);
            _networkBase = networkBase;
        	_combineCommandData = combineCommandData;
        }
        #endregion

        #region Public Properties
        public TickDelegate OnTick;

        public bool IsCommand
        {
            get { return _command; }
            set
            {
                CreatePDV(true);
                _command = value;
				if (!_combineCommandData)
					WritePDU(true);
            }
        }
        #endregion

        #region Public Members
        public void Flush(bool last)
        {
            WritePDU(last);
            //_network.Flush();
        }
        #endregion

        #region Private Members
        private uint CurrentPduSize()
        {
            return _pdu.GetLengthOfPDVs();
        }

        private bool CreatePDV(bool isLast)
        {
            uint len = Math.Min(GetBufferLength(), _max - (CurrentPduSize() + 6));

            //Platform.Log(LogLevel.Info, "Created PDV of length: {0}",len);
            if (_bytes == null || _bytes.Length != len || _pdu.PDVs.Count > 0)
            {
                _bytes = new byte[len];
            }
            _buffer.Read(_bytes, 0, (int)len);

            PDV pdv = new PDV(_pcid, _bytes, _command, isLast);
            _pdu.PDVs.Add(pdv);

            return pdv.IsLastFragment;
        }

        private void WritePDU(bool last)
        {
            if (_pdu.PDVs.Count == 0 || ((CurrentPduSize() + 6) < _max && GetBufferLength() > 0))
            {
                CreatePDV(last);
            }
            if (_pdu.PDVs.Count > 0)
            {
                if (last)
                {
                    _pdu.PDVs[_pdu.PDVs.Count - 1].IsLastFragment = true;
                }
                RawPDU raw = _pdu.Write();

                _networkBase.EnqueuePdu(raw);
                if (OnTick != null)
                    OnTick();
                _pdu = new PDataTF();
            }
        }

        private void AppendBuffer(byte[] buffer, int offset, int count)
        {
            long pos = _buffer.Position;
            _buffer.Seek(0, SeekOrigin.End);
            _buffer.Write(buffer, offset, count);
            _buffer.Position = pos;
        }

        private uint GetBufferLength()
        {
            return (uint)(_buffer.Length - _buffer.Position);
        }
        #endregion

        #region Stream Members
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            //_network.Flush();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // This is in need of a serious refactoring.  The Total length of the 
            // message shouldn't be needed for this object, first of all.  Secondly,
            // We should rewrite this mechanism so that we append directly to a RawPDU object
            // as we receive data so we don't have to make multiple copies of the data.  THis
            // should be accomplished by using a MemoryStream and seeking to rewrite PDV lengths
            // as data is appended. Thirdly, we should add a Write override that allows a 
            // stream to be passed to us or as we read from disk, we shoudl read the pixel data 
            // in smaller chunks when its cached on disk so it doesn't all have to be loaded 
            // into memory for a large image.
            AppendBuffer(buffer, offset, count);
            while ((CurrentPduSize() + 6 + GetBufferLength()) > _max)
            {
                WritePDU(false);
            }
        }
        #endregion
    }
}
