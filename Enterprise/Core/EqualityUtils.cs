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

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Utility class that attempts to provide efficient equality checking for any type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class EqualityUtils<T>
	{
		#region EntityEqualityComparer class

		/// <summary>
		/// Implementation of EqualityComparer that ensures efficient comparison of entities.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		class EntityEqualityComparer<TEntity> : EqualityComparer<TEntity>
		{
			public override bool Equals(TEntity x, TEntity y)
			{
				// if these are the same instance, or both null, they are obviously equal
				// note: this check is critical! by checking for reference equality first,
				// we ensure that no proxy is unnecessarily initialized, which can significantly
				// affect performance
				if (ReferenceEquals(x, y))
					return true;

				// since they are not both null, they cannot be equal if either is null
				if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
					return false;

				// we already know they are not the same instance,
				// but they could still be equal if one is a proxy to the other 
				// we can rely on the implementation of Entity.Equals to handle this.
				// note that if an uninitialized proxy is involved, this call will initialize it
				return x.Equals(y);
			}

			public override int GetHashCode(TEntity obj)
			{
				// use Entity implementation
				return obj.GetHashCode();
			}
		}

		#endregion


		private static readonly EqualityComparer<T> _comparer;

		/// <summary>
		/// Static initializer.
		/// </summary>
		static EqualityUtils()
		{
			// if T is Entity or a subclass of Entity/EnumValue, then we want to use the special EntityEqualityComparer
			if (typeof(Entity).IsAssignableFrom(typeof(T))
				|| typeof(EnumValue).IsAssignableFrom(typeof(T)))
			{
				var type = typeof (EntityEqualityComparer<>).MakeGenericType(typeof(T), typeof(T));
				_comparer = (EqualityComparer<T>)Activator.CreateInstance(type);
			}
			else
			{
				// otherwise use the .Net frameworks default comparer for the type
				// this comparer seems to be fairly efficient in that it avoids boxing/unboxing where possible
				_comparer = EqualityComparer<T>.Default;
			}
		}


		/// <summary>
		/// Tests if the two specified instances are equal, according to the semantics of the type.
		/// </summary>
		/// <remarks>
		/// This method guarantees efficient handling of uninitialized entity proxies.  If either x or y is a proxy,
		/// the comparison will be performed without causing initialization.
		/// </remarks>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool AreEqual(T x, T y)
		{
			return _comparer.Equals(x, y);
		}
	}
}
