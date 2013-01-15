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

namespace ClearCanvas.ImageViewer.Layout
{
    public class RectangularGrid
    {
        public int Rows;
        public int Columns;

        public class Location
        {
            public int Row;
            public int Column;

            public Location ParentGridLocation;

            public override bool Equals(object obj)
            {
                var other = obj as Location;
                if (other != null)
                    return other.Row == Row && other.Column == Column && Equals(other.ParentGridLocation, ParentGridLocation);
                return false;
            }

            public override int GetHashCode()
            {
                var hash = 0x7f4c2145 ^ Row.GetHashCode() ^ Column.GetHashCode();
                if (ParentGridLocation != null)
                    hash ^= ParentGridLocation.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as RectangularGrid;
            if (other != null)
                return other.Rows == Rows && other.Columns == Columns;
            return false;
        }

        public override int GetHashCode()
        {
            return 0x41c59841 ^ Rows.GetHashCode() ^ Columns.GetHashCode();
        }
    }
}