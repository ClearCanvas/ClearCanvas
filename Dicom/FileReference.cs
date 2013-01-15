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

namespace ClearCanvas.Dicom
{
	internal class FileReference
	{
		#region Private Members

		private readonly string _filename;
		private readonly long _offset;
		private readonly long _length;
		private readonly Endian _endian;
		private readonly DicomVr _vr;

		#endregion

		#region Public Properties

		internal string Filename
		{
			get { return _filename; }
		}

		internal long Offset
		{
			get { return _offset; }
		}

		internal Endian Endian
		{
			get { return _endian; }
		}

		internal DicomVr Vr
		{
			get { return _vr; }
		}

		public uint Length
		{
			get { return (uint) _length; }
		}

		#endregion

		#region Constructors

		internal FileReference(string file, long offset, long length, Endian endian, DicomVr vr)
		{
			_filename = file;
			_offset = offset;
			_length = length;
			_endian = endian;
			_vr = vr;
		}

		#endregion
	}
}