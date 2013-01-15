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
using System.Drawing;
using System.Text;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class PointsMemento : List<PointF>, IEquatable<PointsMemento>
	{
		public PointsMemento() {}
		public PointsMemento(int capacity) : base(capacity) {}

		public override int GetHashCode()
		{
			int hashcode = -0x573C799C;
			foreach (PointF point in this)
			{
				hashcode ^= point.GetHashCode();
			}
			return hashcode;
		}

		public override bool Equals(object obj)
		{
			if (obj is PointsMemento)
				return this.Equals((PointsMemento)obj);
			return false;
		}

		public bool Equals(PointsMemento other)
		{
			if (this == other)
				return true;
			if (other == null || this.Count != other.Count)
				return false;

			for(int i = 0; i < this.Count; i++)
			{
				if (this[i] != other[i])
					return false;
			}
			return true;
		}

		public override string ToString()
		{
			const string separator = ", ";
			StringBuilder sb = new StringBuilder();
			sb.Append('{');
			foreach (PointF f in this)
			{
				sb.Append(f.ToString());
				sb.Append(separator);
			}
			if (this.Count > 0)
				sb.Remove(sb.Length - separator.Length, separator.Length);
			sb.Append('}');
			return sb.ToString();
		}
	}
}
