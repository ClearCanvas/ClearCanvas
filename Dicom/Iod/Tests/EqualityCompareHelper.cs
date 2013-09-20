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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.Dicom.Iod.Tests
{
	internal static class EqualityCompareHelper
	{
		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable, Func<T, T, bool> comparison)
		{
			return enumerable.Distinct(new EqualityComparer<T>(comparison));
		}

		private class EqualityComparer<T> : IEqualityComparer<T>
		{
			private readonly Func<T, T, bool> _equality;

			public EqualityComparer(Func<T, T, bool> equality)
			{
				_equality = equality;
			}

			public bool Equals(T x, T y)
			{
				return _equality.Invoke(x, y);
			}

			public int GetHashCode(T obj)
			{
				return 0;
			}
		}
	}
}

#endif