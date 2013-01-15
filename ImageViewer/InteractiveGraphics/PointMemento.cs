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
using System.Drawing;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class PointMemento : IEquatable<PointMemento>
	{
		public readonly PointF Point;

		public PointMemento(PointF point)
		{
			this.Point = point;
		}

		public override int GetHashCode()
		{
			return this.Point.GetHashCode() ^ 0x0BE4AD82;
		}

		public override bool Equals(object obj)
		{
			if (obj is PointMemento)
				return this.Equals((PointMemento)obj);
			return false;
		}

		public bool Equals(PointMemento other)
		{
			if (other == null)
				return false;
			return this.Point == other.Point;
		}

		public override string ToString()
		{
			return this.Point.ToString();
		}
	}
}
