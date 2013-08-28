#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Reflection;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Helper class for managing and raising events in a synchronized manner.
	/// </summary>
	/// <remarks>
	/// This class provides for synchronizing event subscriptions and raising events when an object is used in a
	/// multithreaded environment. A mutually exclusive lock is acquired while the event subscriptions list is
	/// being read or modified, but the actual invocation of the subscribed handlers occurs outside the lock,
	/// thereby improving the performance for events with frequent subscription changes and/or many subscribers.
	/// </remarks>
	public sealed class SynchronizedEventHelper : SynchronizedEventHelper<EventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SynchronizedEventHelper"/> with internal synchronization.
		/// </summary>
		public SynchronizedEventHelper()
			: base(null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="SynchronizedEventHelper"/> with external synchronization.
		/// </summary>
		/// <param name="syncRoot">The object to use for synchronization.</param>
		public SynchronizedEventHelper(object syncRoot)
			: base(syncRoot) {}
	}

	/// <summary>
	/// Helper class for managing and raising events in a synchronized manner.
	/// </summary>
	/// <remarks>
	/// This class provides for synchronizing event subscriptions and raising events when an object is used in a
	/// multithreaded environment. A mutually exclusive lock is acquired while the event subscriptions list is
	/// being read or modified, but the actual invocation of the subscribed handlers occurs outside the lock,
	/// thereby improving the performance for events with frequent subscription changes and/or many subscribers.
	/// </remarks>
	public class SynchronizedEventHelper<T>
		where T : EventArgs
	{
		private readonly object _syncRoot;
		private event EventHandler<T> _eventHandler;

		/// <summary>
		/// Initializes a new instance of <see cref="SynchronizedEventHelper{T}"/> with internal synchronization.
		/// </summary>
		public SynchronizedEventHelper()
			: this(null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="SynchronizedEventHelper{T}"/> with external synchronization.
		/// </summary>
		/// <param name="syncRoot">The object to use for synchronization.</param>
		public SynchronizedEventHelper(object syncRoot)
		{
			_syncRoot = syncRoot ?? new object();
		}

		/// <summary>
		/// Adds a subscription to this event.
		/// </summary>
		/// <param name="eventHandler">The <see cref="EventHandler{T}"/> to be added.</param>
		public void AddHandler(EventHandler<T> eventHandler)
		{
			lock (_syncRoot)
			{
				_eventHandler += eventHandler;
			}
		}

		/// <summary>
		/// Removes a subscription from this event.
		/// </summary>
		/// <param name="eventHandler">The <see cref="EventHandler{T}"/> to be removed.</param>
		public void RemoveHandler(EventHandler<T> eventHandler)
		{
			lock (_syncRoot)
			{
				_eventHandler -= eventHandler;
			}
		}

		/// <summary>
		/// Raises the event.
		/// </summary>
		/// <param name="sender">The instance from which the event originates.</param>
		/// <param name="eventArgs">The <see cref="EventArgs"/> containing details about the event.</param>
		/// <exception cref="MemberAccessException"></exception>
		/// <exception cref="TargetException"></exception>
		/// <exception cref="TargetInvocationException"></exception>
		public void Fire(object sender, T eventArgs)
		{
			Delegate[] delegates;
			lock (_syncRoot)
			{
				if (_eventHandler == null) return;
				delegates = _eventHandler.GetInvocationList();
			}

			foreach (var sink in delegates)
			{
				try
				{
					sink.DynamicInvoke(sender, eventArgs);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex);
					throw;
				}
			}
		}
	}
}