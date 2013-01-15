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
using System.Collections;

namespace ClearCanvas.ImageViewer.Comparers
{
    public sealed class ReverseComparer<T> : IComparer<T>, IEquatable<ReverseComparer<T>>
    {
        private readonly Comparison<T> _realComparer;

        public ReverseComparer(IComparer<T> realComparer)
            : this(realComparer.Compare)
        {
        }

        public ReverseComparer(Comparison<T> realComparer)
        {
            _realComparer = realComparer;
        }

        public int Compare(T x, T y)
        {
            return -_realComparer(x, y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ReverseComparer<T>)
                return Equals((ReverseComparer<T>) obj);
            return false;
        }

        #region IEquatable<ReverseComparer<T>> Members

        public bool Equals(ReverseComparer<T> other)
        {
            return other != null &&  _realComparer.Target.Equals(other._realComparer.Target);
        }

        #endregion
    }

    public sealed class ReverseComparer : IComparer, IEquatable<ReverseComparer>
    {
        private readonly Comparison<object> _realComparer;

        public ReverseComparer(IComparer realComparer)
            : this(realComparer.Compare)
        {
        }

        public ReverseComparer(Comparison<object> realComparer)
        {
            _realComparer = realComparer;
        }

        public int Compare(object x, object y)
        {
            return -_realComparer(x, y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ReverseComparer)
                return Equals((ReverseComparer) obj);
            return false;
        }

        #region IEquatable<ReverseComparer Members

        public bool Equals(ReverseComparer other)
        {
            return other != null &&  _realComparer.Target.Equals(other._realComparer.Target);
        }

        #endregion
    }
}
