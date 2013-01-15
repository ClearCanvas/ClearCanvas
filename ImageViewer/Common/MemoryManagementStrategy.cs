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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IMemoryManagementStrategy.MemoryCollected"/> events.
	/// </summary>
	public class MemoryCollectedEventArgs : EventArgs
	{
		private bool _needMoreMemory = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MemoryCollectedEventArgs(int largeObjectContainersUnloadedCount,
			int largeObjectsCollectedCount, long bytesCollectedCount, TimeSpan elapsedTime, bool isLast)
		{
			ElapsedTime = elapsedTime;
			LargeObjectContainersUnloadedCount = largeObjectContainersUnloadedCount;
			LargeObjectsCollectedCount = largeObjectsCollectedCount;
			BytesCollectedCount = bytesCollectedCount;
			IsLast = isLast;
		}

		/// <summary>
		/// The total time taken to collect memory.
		/// </summary>
		public readonly TimeSpan ElapsedTime;

		/// <summary>
		/// The total number of <see cref="ILargeObjectContainer"/>s that were unloaded.
		/// </summary>
		public readonly int LargeObjectContainersUnloadedCount;
		/// <summary>
		/// The total number of large objects collected.
		/// </summary>
		public readonly int LargeObjectsCollectedCount;
		/// <summary>
		/// The total number of bytes collected.
		/// </summary>
		public readonly long BytesCollectedCount;
		/// <summary>
		/// Indicates whether or not this is the last <see cref="IMemoryManagementStrategy.MemoryCollected"/> event
		/// for the current collection.
		/// </summary>
		public readonly bool IsLast;

		/// <summary>
		/// Gets or sets whether or not more memory is needed by any entity currently observing the
		/// <see cref="IMemoryManagementStrategy.MemoryCollected"/> event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="IMemoryManagementStrategy">memory management strategy</see> will look at this value
		/// when determining whether or not to continue collecting memory.
		/// </para>
		/// <para>
		/// Once set to true, the value cannot be set back to false.
		/// </para>
		/// </remarks>
		public bool NeedMoreMemory
		{
			get { return _needMoreMemory; }
			set
			{
				if (value)
					_needMoreMemory = true;
			}
		}
	}

	/// <summary>
	/// Argument class passed to the <see cref="IMemoryManagementStrategy"/>.
	/// </summary>
	public class MemoryCollectionArgs
	{
		internal MemoryCollectionArgs(IEnumerable<ILargeObjectContainer> largeObjectContainers)
		{
			LargeObjectContainers = largeObjectContainers;
		}

		/// <summary>
		/// Gets all the <see cref="ILargeObjectContainer"/>s currently being managed by the <see cref="MemoryManager"/>.
		/// </summary>
		public readonly IEnumerable<ILargeObjectContainer> LargeObjectContainers;
	}

	/// <summary>
	/// Defines the interface to a memory management strategy.
	/// </summary>
	/// <remarks>
	/// Implementers must mark their class as an extension of <see cref="MemoryManagementStrategyExtensionPoint"/> in order
	/// to override the default strategy.
	/// </remarks>
	public interface IMemoryManagementStrategy
	{
		/// <summary>
		/// Called by the <see cref="MemoryManager"/> to collect memory from
		/// <see cref="ILargeObjectContainer"/>s, if necessary.  See <see cref="MemoryManager"/> for more details.
		/// </summary>
		void Collect(MemoryCollectionArgs collectionArgs);
		/// <summary>
		/// Fired when memory has been collected.
		/// </summary>
		/// <remarks>The event must be fired at least once, but may be fired repeatedly in order to try
		/// and return control to waiting threads as quickly as possible.
		/// </remarks>
		event EventHandler<MemoryCollectedEventArgs> MemoryCollected;
	}

	/// <summary>
	/// Abstract base implementation <see cref="IMemoryManagementStrategy"/>.
	/// </summary>
	public abstract class MemoryManagementStrategy : IMemoryManagementStrategy
	{
		private class NullMemoryManagementStrategy : IMemoryManagementStrategy
		{
			#region IMemoryManagementStrategy Members

			public void Collect(MemoryCollectionArgs collectionArgs)
			{
			}

			public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
			{
				add { }
				remove { }
			}

			#endregion
		}

		internal static readonly IMemoryManagementStrategy Null = new NullMemoryManagementStrategy();

		private event EventHandler<MemoryCollectedEventArgs> _memoryCollected;
		
		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected MemoryManagementStrategy()
		{
		}

		#region IMemoryManagementStrategy Members

		/// <summary>
		/// Called by the <see cref="MemoryManager"/> to collect memory from
		/// <see cref="ILargeObjectContainer"/>s, if necessary.  See <see cref="MemoryManager"/> for more details.
		/// </summary>
		public abstract void Collect(MemoryCollectionArgs collectionArgs);

		/// <summary>
		/// Fired when memory has been collected.
		/// </summary>
		/// <remarks>The event must be fired at least once, but may be fired repeatedly in order to try
		/// and return control to waiting threads as quickly as possible.
		/// </remarks>
		public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add { _memoryCollected += value; }
			remove { _memoryCollected -= value; }
		}

		#endregion

		/// <summary>
		/// Fires the <see cref="MemoryCollected"/> event.
		/// </summary>
		protected void OnMemoryCollected(MemoryCollectedEventArgs args)
		{
			try
			{
				EventsHelper.Fire(_memoryCollected, this, args);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected failure while firing memory collected event.");
			}
		}
	}
}
