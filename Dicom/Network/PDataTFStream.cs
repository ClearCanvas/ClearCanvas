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
		private PDataTFWrite _pdu;
		private readonly NetworkBase _networkBase;
		private readonly bool _combineCommandData;
		#endregion

		#region Public Constructors
		public PDataTFStream(NetworkBase networkBase, byte pcid, uint max, bool combineCommandData)
		{
			_command = true;
			_pcid = pcid;
			_max = max;
			_pdu = new PDataTFWrite(max);
			_pdu.CreatePDV(pcid);

			_networkBase = networkBase;
			_combineCommandData = combineCommandData;
			BytesWritten = 0;
		}
		#endregion

		#region Public Properties
		public TickDelegate OnTick;

		public long BytesWritten { get; set; }

		public bool IsCommand
		{
			get { return _command; }
			set
			{
				if (!_combineCommandData)
				{
					WritePDU(true);
					_command = value;
					_pdu.CreatePDV(_pcid);
				}
				else
				{
					_pdu.CompletePDV(true, _command);
					_command = value;
					_pdu.CreatePDV(_pcid);
				}
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

		private void WritePDU(bool last)
		{
			_pdu.CompletePDV(last, _command);

			RawPDU raw = _pdu.Write();

			_networkBase.EnqueuePdu(raw);
			if (OnTick != null)
				OnTick();

			_pdu = new PDataTFWrite(_max);

		}

		private void AppendBuffer(byte[] buffer, int index, int count)
		{
			_pdu.AppendPdv(buffer, index, count);
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
			BytesWritten += count;

			int off = offset;
			int c = count;

			while (c > 0)
			{
				int bytesToWrite = Math.Min(c, (int)_pdu.GetRemaining());
				AppendBuffer(buffer, off, bytesToWrite);
				c -= bytesToWrite;
				off += bytesToWrite;

				if (_pdu.GetRemaining() == 0)
				{
					WritePDU(false);
					_pdu.CreatePDV(_pcid);
				}
			}
		}

		public void Write(Stream readStream)
		{
			var buffer = new byte[64 * 1024];
			int read;
			while ((read = readStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				Write(buffer, 0, read);
			}
		}

		#endregion
	}
}
