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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Iod
{
	public struct CIELabColor
	{
		private ushort _l;
		private ushort _a;
		private ushort _b;

		public CIELabColor(ushort l, ushort a, ushort b)
		{
			_l = l;
			_a = a;
			_b = b;
		}

		public ushort L
		{
			get { return _l; }
			set { _l = value; }
		}

		public ushort A
		{
			get { return _a; }
			set { _a = value; }
		}

		public ushort B
		{
			get { return _b; }
			set { _b = value; }
		}

		public ushort[] ToArray()
		{
			return new ushort[] {_l, _a, _b};
		}
	}
}
