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
	internal class RectangleMemento : IEquatable<RectangleMemento>
	{
		PointF _topLeft;
		PointF _bottomRight;

		public RectangleMemento()
		{
		}

		public PointF TopLeft
		{
			get { return _topLeft; }
			set { _topLeft = value; }
		}
		
		public PointF BottomRight
		{
			get { return _bottomRight; }
			set { _bottomRight = value; }
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as RectangleMemento);
		}

		#region IEquatable<RectangleMemento> Members

		public bool Equals(RectangleMemento other)
		{
			if (other == null)
				return false;

			return TopLeft == other.TopLeft && BottomRight == other.BottomRight;
		}

		#endregion
	}
}
