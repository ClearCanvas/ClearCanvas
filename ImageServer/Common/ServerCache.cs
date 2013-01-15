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
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// A generic Cache class with expiration times
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <remarks>
	/// <para>
	/// This class is similar to a Dictionary where the items in the
	/// dictionary expire and are automatically removed after a specified
	/// elapsed time.
	/// </para>
	/// </remarks>
	public class ServerCache<TKey, TValue> : IDisposable
		where TValue : new()
	{
		#region Private Classes
		/// <summary>
		/// Internal class used to store the cached value with an expiration time
		/// </summary>
		private class CachedValue
		{
			public CachedValue(TKey key, TValue val, TimeSpan retentionTime)
			{
				CachedKey = key;
				Value = val;
				Expiration = Platform.Time.Add(retentionTime);
			}
			public readonly TKey CachedKey;
			public readonly TValue Value;
			public DateTime Expiration;
		}
		#endregion

		#region Private Members
		private readonly TimeSpan _retentionTime;
		private readonly Dictionary<TKey, CachedValue> _locations = new Dictionary<TKey, CachedValue>();
		private readonly object _lock = new object();
		private Timer _timer;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="retention">The retention time of objects in the cache.</param>
		/// <param name="checkInterval">The interval at which the cache will be checked to purge expired values.</param>
		public ServerCache(TimeSpan retention, TimeSpan checkInterval)
		{
			_retentionTime = retention;
			_timer = new Timer(CheckCache, this, checkInterval, checkInterval);
		}
		#endregion

		#region Properties
		/// <summary>
		/// A count of values in the cache.
		/// </summary>
		public int Count
		{
			get { lock (_lock) return _locations.Count; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Internal routine for Checking for expired cache object
		/// </summary>
		/// <param name="theObject"></param>
		private void CheckCache(object theObject)
		{
			DateTime now = Platform.Time;
			List<CachedValue> removalList = new List<CachedValue>();

			lock (_lock)
			{
				foreach (CachedValue cache in _locations.Values)
				{
					if (cache.Expiration < now)
						removalList.Add(cache);
				}

				foreach (CachedValue cache in removalList)
					_locations.Remove(cache.CachedKey);
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the value associated with the specified key. 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TValue GetValue(TKey key)
		{
			CachedValue theValue;
			lock (_lock)
			{
				if (!_locations.TryGetValue(key, out theValue))
					return default(TValue);
			}

			return theValue.Value;
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue val)
		{
			val = default(TValue);
			CachedValue theValue;
			lock (_lock)
			{
				if (!_locations.TryGetValue(key, out theValue))
					return false;
			}

			val = theValue.Value;
			return true;
		}

		/// <summary>
		/// Determines if the cache contains the specified key.
		/// </summary>
		/// <param name="key">The key to check for.</param>
		/// <returns></returns>
		public bool ContainsKey(TKey key)
		{
			lock (_lock)
				return _locations.ContainsKey(key);
		}

		/// <summary>
		/// Adds the specified key and value to the cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public void Add(TKey key, TValue val)
		{
			lock (_lock)
			{
				if (!_locations.ContainsKey(key))
					_locations.Add(key, new CachedValue(key, val, _retentionTime));
			}
		}

		/// <summary>
		/// Removes the value with the specified key from the cache.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Remove(TKey key)
		{
			lock (_lock)
				return _locations.Remove(key);
		}
		#endregion

		#region IDisposable Implementation
		/// <summary>
		/// Releases all resources with the current instance of ServerCache.
		/// </summary>
		public void Dispose()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}
		#endregion

	}
}
