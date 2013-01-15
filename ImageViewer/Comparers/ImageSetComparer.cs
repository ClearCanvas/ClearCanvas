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
	public interface IImageSetComparer : IComparer<IImageSet>, IEquatable<IImageSetComparer>
	{ }

	/// <summary>
	/// Base class for comparers that compare some aspect of <see cref="IImageSet"/>s.
	/// </summary>
	public abstract class ImageSetComparer : ComparerBase, IImageSetComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		/// <summary>
		/// Compares two <see cref="IImageSet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IImageSet x, IImageSet y);

		#endregion

		public bool Equals(IImageSetComparer other)
		{
			return other is ImageSetComparer && Equals((ImageSetComparer)other);
		}

		public virtual bool Equals(ImageSetComparer other)
		{
			return base.Equals(other);
		}
	}
}
