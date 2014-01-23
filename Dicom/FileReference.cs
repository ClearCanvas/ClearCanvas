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

namespace ClearCanvas.Dicom
{
	internal class FileReference
	{
		#region Private Members

		private readonly long _length;

		#endregion

		#region Public Properties

		internal Func<Stream> StreamOpener { get; private set; }

		internal long Offset { get; private set; }
		internal Endian Endian { get; private set; }
		internal DicomVr Vr { get; private set; }

		public uint Length
		{
			get { return (uint) _length; }
		}

		#endregion

		#region Constructors

		internal FileReference(Func<Stream> streamOpener, long offset, long length, Endian endian, DicomVr vr)
		{
			StreamOpener = streamOpener;
			Offset = offset;
			_length = length;
			Endian = endian;
			Vr = vr;
		}

		#endregion
	}
}