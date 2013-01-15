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
	public interface IPresentationImageComparer : IComparer<IPresentationImage>, IEquatable<IPresentationImageComparer>
	{}

	/// <summary>
	/// Base class for comparing <see cref="IPresentationImage"/>s.
	/// </summary>
	public abstract class PresentationImageComparer : ComparerBase, IPresentationImageComparer, IEquatable<PresentationImageComparer>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageComparer"/>.
		/// </summary>
		protected PresentationImageComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageComparer"/>.
		/// </summary>
		protected PresentationImageComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		public abstract int Compare(IPresentationImage x, IPresentationImage y);

		#endregion

		public bool Equals(IPresentationImageComparer other)
		{
			return other is PresentationImageComparer && Equals((PresentationImageComparer) other);
		}
		
		public virtual bool Equals(PresentationImageComparer other)
		{
			return base.Equals(other);
		}
	}
}
