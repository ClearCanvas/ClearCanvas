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

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines an interface to a cache used for offline storage of enterprise data.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public interface IOfflineCache<TKey, TValue>
	{
		/// <summary>
		/// Creates a client for accessing this offline cache, for use by a single thread.
		/// </summary>
		/// <remarks>
		/// Implementations of this method must be safe for concurrent use by multiple threads.
		/// However, the returned object will only ever be used by a single thread, and therefore
		/// needn't be thread-safe.
		/// </remarks>
		/// <returns></returns>
		IOfflineCacheClient<TKey, TValue> CreateClient();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Safe for use by a single thread only.
	/// </remarks>
	public interface IOfflineCacheClient<TKey, TValue> : IDisposable
	{
		/// <summary>
		/// Gets the value at the specified key from the cache.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		TValue Get(TKey key);

		/// <summary>
		/// Puts the specified value into the cache at the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void Put(TKey key, TValue value);

		/// <summary>
		/// Removes the specified key and corresponding value from the cache.
		/// </summary>
		/// <param name="key"></param>
		void Remove(TKey key);
	}
}
