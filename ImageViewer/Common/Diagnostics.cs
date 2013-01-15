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
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	//TODO (cr Oct 2009): Get rid of this class?

	/// <summary>
	/// Static helper class for use when debugging.
	/// </summary>
	public static class Diagnostics
	{
		#region Memory

		private static long _totalLargeObjectMemoryBytes = 0;

		private static readonly object _syncLock = new object(); 
		private static event EventHandler _totalLargeObjectBytesChanged;

		/// <summary>
		/// Gets the running total byte count of large objects held in memory.
		/// </summary>
		public static long TotalLargeObjectBytes
		{
			get
			{
				//synchronize changes, but not reads
				return Thread.VolatileRead(ref _totalLargeObjectMemoryBytes);
			}	
		}

		/// <summary>
		/// Occurs when <see cref="TotalLargeObjectBytes"/> has changed.
		/// </summary>
		public static event EventHandler TotalLargeObjectBytesChanged
		{
			add
			{
				lock(_syncLock)
				{
					_totalLargeObjectBytesChanged += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_totalLargeObjectBytesChanged -= value;
				}
			}
		}

		/// <summary>
		/// Called when a large object is allocated.
		/// </summary>
		/// <remarks>
		/// Although it is not necessary to call this method when you allocate a large object,
		/// such as a byte array for pixel data, it is recommended that you do so in order
		/// for this class to provide accurate data.
		/// </remarks>
		public static void OnLargeObjectAllocated(long bytes)
		{
			lock(_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes += bytes;
				EventsHelper.Fire(_totalLargeObjectBytesChanged, null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Called when a large object is released.
		/// </summary>
		/// <remarks>
		/// You should call this method exactly once when you are certain the large object
		/// in question is no longer in use.
		/// </remarks>
		public static void OnLargeObjectReleased(long bytes)
		{
			lock (_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes -= bytes;
				EventsHelper.Fire(_totalLargeObjectBytesChanged, null, EventArgs.Empty);
			}
		}

		#endregion
	}
}
