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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public struct DicomObject : IComparable<DicomObject>, IComparable, IEquatable<DicomObject>
	{
		public static readonly DicomObject Empty = new DicomObject(-1, string.Empty);

		public readonly long Length;
		public readonly string VR;

		public DicomObject(long size, string vr)
		{
			this.Length = size;
			this.VR = vr;
		}

		public int CompareTo(DicomObject other)
		{
			return this.Length.CompareTo(other.Length);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (obj is DicomObject)
				return this.CompareTo((DicomObject) obj);
			throw new ArgumentException("Parameter must be a DicomObject.", "obj");
		}

		public bool Equals(DicomObject other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is DicomObject)
				return this.Equals((DicomObject) obj);
			return false;
		}

		public override int GetHashCode()
		{
			return -0x6D22B552 ^ this.Length.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(SR.LabelBinaryTagValue, this.VR, this.Length);
		}
	}
}