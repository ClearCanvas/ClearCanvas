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

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Null implementation of <see cref="IOfflineCache{TKey,TValue}"/>.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	class NullOfflineCache<TKey, TValue> : IOfflineCache<TKey, TValue>, IOfflineCacheClient<TKey, TValue>
	{
		#region Implementation of IOfflineCache

		public IOfflineCacheClient<TKey, TValue> CreateClient()
		{
			return this;
		}

		#endregion

		#region Implementation of IOfflineCacheClient

		public TValue Get(TKey key)
		{
			return default(TValue);
		}

		public void Put(TKey key, TValue value)
		{
		}

		public void Remove(TKey key)
		{
		}

		#endregion

		#region Implementation of IDisposable

		public void Dispose()
		{
		}

		#endregion
	}
}
