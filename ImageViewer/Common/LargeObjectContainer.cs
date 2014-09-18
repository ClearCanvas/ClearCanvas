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

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// Indicator of the relative cost to regenerate an object were the <see cref="MemoryManager"/> to unload it.
	/// </summary>
	public enum RegenerationCost
	{
		/// <summary>
		/// Fairly low cost to regenerate.
		/// </summary>
		Low = 0,
		/// <summary>
		/// Average cost to regenerate.
		/// </summary>
		Medium = 1,
		/// <summary>
		/// Very high cost to regenerate.
		/// </summary>
		High = 2
	}

    /// <summary>
	/// Simple data class describing the contents of an <see cref="ILargeObjectContainer"/>.
	/// </summary>
	public class LargeObjectContainerData : ILargeObjectContainer
	{
		private readonly Guid _identifier;
		private int _lockCount;
		private volatile int _largeObjectCount;
		private long _totalBytesHeld;
		private volatile RegenerationCost _regenerationCost;
		private DateTime _lastAccessTime;
        [ThreadStatic]
        private static FastDateTime _fastDateTime;

		/// <summary>
		/// Constructor.
		/// </summary>
		public LargeObjectContainerData(Guid identifier)
		{
			_identifier = identifier;
		}

        private static FastDateTime FastDateTime
        {
            get { return _fastDateTime ?? (_fastDateTime = new FastDateTime()); }
        }

        #region ILargeObjectContainer Members

		/// <summary>
		/// Gets the unique identifier for the container.
		/// </summary>
		public Guid Identifier
		{
			get { return _identifier; }
		}

		/// <summary>
		/// Gets or sets the total number of large objects held by the container.
		/// </summary>
		/// <remarks>A large object is typically a large array, like a byte array.</remarks>
		public int LargeObjectCount
		{
			get { return _largeObjectCount; }
			set { _largeObjectCount = value; }
		}

		/// <summary>
		/// Gets or sets the total number of bytes held by the container.
		/// </summary>
		public long BytesHeldCount
		{
			get { return _totalBytesHeld; }
			set { _totalBytesHeld = value; }
		}

        /// <summary>
        /// No longer used.
        /// </summary>
        [Obsolete("No longer used by the framework. This property will be removed.")]
		public int LastAccessTimeAccuracyMilliseconds { get; set; }

		/// <summary>
		/// Gets the last time the container was accessed.
		/// </summary>
		/// <remarks>
		/// The default <see cref="IMemoryManagementStrategy"/> uses a "least recently used" approach
		/// when deciding which <see cref="ILargeObjectContainer"/>s to unload.
		/// </remarks>
		public DateTime LastAccessTime
		{
			get { return _lastAccessTime; }
		}

		/// <summary>
		/// Gets or sets the <see cref="RegenerationCost"/>, which is a relative value intended to 
		/// give the <see cref="IMemoryManagementStrategy">memory management strategy</see> a hint
		/// when deciding which containers to unload.
		/// </summary>
		public RegenerationCost RegenerationCost
		{
			get { return _regenerationCost; }
			set { _regenerationCost = value; }
		}

		/// <summary>
		/// Gets whether or not the container is locked.  See <see cref="Lock"/> for details.
		/// </summary>
		public bool IsLocked
		{
			get { return Thread.VolatileRead(ref _lockCount) > 0; }
		}

		/// <summary>
		/// Updates the <see cref="LastAccessTime"/>.
		/// </summary>
		public void UpdateLastAccessTime()
		{
            _lastAccessTime = FastDateTime.Now;
		}

		/// <summary>
		/// Locks the container.
		/// </summary>
		/// <remarks>
		/// <para>
		///	The <see cref="IMemoryManagementStrategy">memory management strategy</see> will look at the <see cref="IsLocked"/> property
		/// when deciding whether or not to unload the container.  The <see cref="IMemoryManagementStrategy">memory management strategy</see>
		/// could still call <see cref="ILargeObjectContainer.Unload"/> if more memory is needed.
		/// </para>
		/// <para>
		/// Make sure to call <see cref="Unlock"/> once for every call to <see cref="Lock"/>.
		/// </para>
		/// </remarks>
		public void Lock()
		{
			Interlocked.Increment(ref _lockCount);
		}

		/// <summary>
		/// Unlocks the container.  See <see cref="Lock"/> for details.
		/// </summary>
		public void Unlock()
		{
			Interlocked.Decrement(ref _lockCount);
		}

		/// <summary>
		/// Unloads the container.
		/// </summary>
		/// <remarks>
		/// Although the <see cref="IMemoryManagementStrategy">memory management strategy</see> can try to unload
		/// the container, it is still ultimately up to the container itself whether or not it actually responds to
		/// the request.  Ideally, the container should always try to unload when the request is made.
		/// </remarks>
		public void Unload()
		{
			throw new NotSupportedException("The method or operation is not implemented.");
		}

		#endregion

		#region Regeneration Cost Presets

		/// <summary>
		/// Preset cost for data that is generated and not computed from other data sources.
		/// </summary>
		public static readonly RegenerationCost PresetGeneratedData = RegenerationCost.Low;

		/// <summary>
		/// Preset cost for data that is loaded from disk (or other low latency sources) with little or no additional processing.
		/// </summary>
		public static readonly RegenerationCost PresetDiskLoadedData = RegenerationCost.Medium;

		/// <summary>
		/// Preset cost for data that is loaded over network (or other high latency sources) with little or no additional processing.
		/// </summary>
		public static readonly RegenerationCost PresetNetworkLoadedData = RegenerationCost.Medium;

		/// <summary>
		/// Preset cost for data that is computed from other data sources, or loaded directly from a source but requires additional processing.
		/// </summary>
		public static readonly RegenerationCost PresetComputedData = RegenerationCost.High;

		#endregion
	}

	/// <summary>
	/// Defines an object that is a container for one or more "large objects".  See <see cref="MemoryManager"/> for more details.
	/// </summary>
	public interface ILargeObjectContainer
	{
		/// <summary>
		/// Gets the unique identifier for the container.
		/// </summary>
		Guid Identifier { get; }

		/// <summary>
		/// Gets the total number of large objects held by the container.
		/// </summary>
		/// <remarks>A large object is typically a large array, like a byte array.</remarks>
		int LargeObjectCount { get; }
		
		/// <summary>
		/// Gets the total number of bytes held by the container.
		/// </summary>
		long BytesHeldCount { get; }

		/// <summary>
		/// Gets the last time the container was accessed.
		/// </summary>
		/// <remarks>
		/// The <see cref="DefaultMemoryManagementStrategy"/> uses a "least recently used" approach
		/// when deciding which <see cref="ILargeObjectContainer"/>s to unload.
		/// </remarks>
		DateTime LastAccessTime { get; }

		/// <summary>
		/// Gets or sets the <see cref="RegenerationCost"/>, which is a relative value intended to 
		/// give the <see cref="IMemoryManagementStrategy"/> a hint when deciding which containers to unload.
		/// </summary>
		RegenerationCost RegenerationCost { get; }

		/// <summary>
		/// Gets whether or not the container is locked.  See <see cref="Lock"/> for details.
		/// </summary>
		bool IsLocked { get; }

		/// <summary>
		/// Locks the container.
		/// </summary>
		/// <remarks>
		/// <para>
		///	The <see cref="IMemoryManagementStrategy">memory management strategy</see> will look at the <see cref="IsLocked"/> property
		/// when deciding whether or not to unload the container.  The <see cref="IMemoryManagementStrategy">memory management strategy</see>
		/// could still call <see cref="ILargeObjectContainer.Unload"/> if more memory is needed.
		/// </para>
		/// <para>
		/// Make sure to call <see cref="Unlock"/> once for every call to <see cref="Lock"/>.
		/// </para>
		/// </remarks>
		void Lock();

		/// <summary>
		/// Unlocks the container.  See <see cref="Lock"/> for details.
		/// </summary>
		void Unlock();

		/// <summary>
		/// Unloads the container.
		/// </summary>
		/// <remarks>
		/// Although the <see cref="IMemoryManagementStrategy">memory management strategy</see> can try to unload
		/// the container, it is still ultimately up to the container itself whether or not it actually responds to
		/// the request.  Ideally, the container should always try to unload when the request is made.
		/// </remarks>
		void Unload();
	}
}
