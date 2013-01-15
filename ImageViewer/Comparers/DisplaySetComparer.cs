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

namespace ClearCanvas.ImageViewer.Comparers
{
	public interface IDisplaySetComparer : IComparer<IDisplaySet>, IEquatable<IDisplaySetComparer>
	{}

	/// <summary>
	/// Base class for comparers that compare some aspect of <see cref="IDisplaySet"/>s.
	/// </summary>
	public abstract class DisplaySetComparer : ComparerBase, IDisplaySetComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetComparer"/>.
		/// </summary>
		protected DisplaySetComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetComparer"/>.
		/// </summary>
		protected DisplaySetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IDisplaySet x, IDisplaySet y);

		#endregion

		public bool Equals(IDisplaySetComparer other)
		{
			return other is DisplaySetComparer && Equals((DisplaySetComparer)other);
		}

		public virtual bool Equals(DisplaySetComparer other)
		{
			return base.Equals(other);
		}
	}
}
