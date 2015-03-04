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
using ClearCanvas.Common;
using System.Threading;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// Extension point for custom implementations of <see cref="IMemoryManagementStrategy"/>.
	/// </summary>
	public sealed class MemoryManagementStrategyExtensionPoint : ExtensionPoint<IMemoryManagementStrategy>
	{ }

	/// <summary>
	/// The memory manager.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The memory manager is responsible for managing <see cref="ILargeObjectContainer"/>s that have
	/// registered themselves with the memory manager as candidates for unloading when memory usage
	/// starts to get high.  The memory manager defers the actual collection mechanism to the
	/// <see cref="IMemoryManagementStrategy"/>, of which there is a default implementation, but it can
	/// be replaced via the <see cref="MemoryManagementStrategyExtensionPoint"/>.
	/// </para>
	/// <para>
	/// When there are <see cref="ILargeObjectContainer"/>s to be managed, the <see cref="MemoryManager"/>
	/// starts a collection thread that will call <see cref="IMemoryManagementStrategy.Collect"/> periodically.
	/// The <see cref="IMemoryManagementStrategy"/> is then responsible for analyzing the current memory
	/// usage of the process and determining which, if any, <see cref="ILargeObjectContainer"/>s should be unloaded.
	/// </para>
	/// <para>
	/// Once unloaded, it is good practice for the <see cref="ILargeObjectContainer"/> to remove itself
	/// from the <see cref="MemoryManager"/> by calling <see cref="Remove"/> since it is no longer necessary to keep
	/// track of it until it has been loaded again, at which time it should re-add itself by calling <see cref="Add"/>.
	/// </para>
	/// </remarks>
	public static partial class MemoryManager
	{
		private const int _defaultWaitTimeMilliseconds = 1000;

		/// <summary>
		/// A command that can be executed repeatedly until it succeeds.
		/// </summary>
		/// <exception cref="OutOfMemoryException">When thrown by the command, the <see cref="MemoryManager"/>
		/// will continue to retry by executing the command repeatedly.
		/// See <see cref="MemoryManager.Execute(ClearCanvas.ImageViewer.Common.MemoryManager.RetryableCommand,System.TimeSpan)"/> for more details.</exception>
		public delegate void RetryableCommand();

		#region Private Fields

		private static readonly IMemoryManagementStrategy _strategy;
		
		private static readonly object _syncLock = new object();
		private static readonly List<ILargeObjectContainer> _containersToAdd = new List<ILargeObjectContainer>();
		private static readonly List<ILargeObjectContainer> _containersToRemove = new List<ILargeObjectContainer>();

		private static readonly LargeObjectContainerCache _containerCache = new LargeObjectContainerCache();
		private static volatile Thread _collectionThread;
		private static volatile bool _collecting = false;
		private static int _waitingClients = 0;
		private static event EventHandler<MemoryCollectedEventArgs> _memoryCollected;

#if UNIT_TESTS
#pragma warning disable 1591
		public static bool Enabled = true;
#pragma warning restore 1591
#endif

		#endregion

		static MemoryManager()
		{
			//_strategy = MemoryManagementStrategy.Null;
			//return;
			
			IMemoryManagementStrategy strategy = null;

			try
			{
				strategy = (IMemoryManagementStrategy)new MemoryManagementStrategyExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No memory management strategy extension found.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Creation of memory management strategy failed; using default.");
			}

			_strategy = strategy ?? new DefaultMemoryManagementStrategy();
			_strategy.MemoryCollected += OnMemoryCollected;
		}

		#region Properties

		/// <summary>
		/// Gets the total number of bytes held by all the <see cref="ILargeObjectContainer"/>s currently being managed by the memory manager.
		/// </summary>
		public static long LargeObjectBytesCount
		{
			get { return _containerCache.LastLargeObjectBytesCount; }	
		}

		/// <summary>
		/// Gets the total number of <see cref="ILargeObjectContainer"/>s held by all the <see cref="ILargeObjectContainer"/>s currently being managed by the memory manager.
		/// </summary>
		public static long LargeObjectContainerCount
		{
			get { return _containerCache.LastLargeObjectContainerCount; }
		}

		/// <summary>
		/// Gets the total number of "large objects" held by all the <see cref="ILargeObjectContainer"/>s currently being managed by the memory manager.
		/// </summary>

		public static long LargeObjectCount
		{
			get { return _containerCache.LastLargeObjectCount; }
		}

		#endregion

		/// <summary>
		/// Forwards the <see cref="IMemoryManagementStrategy.MemoryCollected"/> event.
		/// </summary>
		public static event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add
			{
				lock (_syncLock)
				{
					_memoryCollected += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_memoryCollected -= value;
				}
			}
		}

		#region Methods

		private static void OnMemoryCollected(object sender, MemoryCollectedEventArgs args)
		{
			Delegate[] delegates;

			lock (_syncLock)
			{
				if (args.IsLast)
					_collecting = false;

				if (_memoryCollected != null)
					delegates = _memoryCollected.GetInvocationList();
				else
					delegates = new Delegate[0];
			}

			foreach (Delegate @delegate in delegates)
			{
				try
				{
					@delegate.DynamicInvoke(_strategy, args);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Unexpected error encountered during memory collection notification.");
				}
			}
		}

		private static void StartCollectionThread()
		{
			lock (_syncLock)
			{
				if (_collectionThread == null)
				{
					Platform.Log(LogLevel.Debug, "Starting memory collection thread.");

					_collectionThread = new Thread(RunCollectionThread)
					                    	{
					                    		Priority = ThreadPriority.Highest, 
												IsBackground = true,
                                                Name = "Memory Manager"
					                    	};

					_collectionThread.Start();
				}
			}
		}

		private static void RunCollectionThread()
		{
			const int waitTimeMilliseconds = 10000;

			while (true)
			{
				try
				{
					lock (_syncLock)
					{
						if (_waitingClients == 0)
							Monitor.Wait(_syncLock, waitTimeMilliseconds);

						if (Platform.IsLogLevelEnabled(LogLevel.Debug))
						{
							Platform.Log(LogLevel.Debug, "Adding {0} containers and removing {1} from large object container cache.", 
							_containersToAdd.Count, _containersToRemove.Count);
						}

						foreach (ILargeObjectContainer container in _containersToRemove)
							_containerCache.Remove(container);
						foreach (ILargeObjectContainer container in _containersToAdd)
							_containerCache.Add(container);

						_containersToRemove.Clear();
						_containersToAdd.Clear();

						if (_waitingClients == 0 && _containerCache.IsEmpty)
						{
							Platform.Log(LogLevel.Debug, "Exiting collection thread, container cache is empty.");
							_containerCache.CleanupDeadItems(true); //updates the estimates
							_collectionThread = null;
							break;
						}
					}

					CodeClock clock = new CodeClock();
					clock.Start();

					_containerCache.CleanupDeadItems(true);

					clock.Stop();
					PerformanceReportBroker.PublishReport("Memory", "CleanupDeadItems", clock.Seconds);

					_strategy.Collect(new MemoryCollectionArgs(_containerCache));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Unexpected error occurred while collecting large objects.");
				}
				finally
				{
					if (_collecting)
					{
						Platform.Log(LogLevel.Debug, "Memory management strategy failed to fire 'complete' event; firing to avoid deadlocks.");
						OnMemoryCollected(null, new MemoryCollectedEventArgs(0, 0, 0, TimeSpan.Zero, true));
					}
				}
			}
		}

		/// <summary>
		/// Adds an <see cref="ILargeObjectContainer"/> to the memory manager's list.
		/// </summary>
		public static void Add(ILargeObjectContainer container)
		{
#if UNIT_TESTS
			if (!Enabled)
				return;
#endif
			// CR (JY 2013-APR-15): Assert container != null
			lock(_syncLock)
			{
				_containersToRemove.Remove(container);
				_containersToAdd.Add(container);

				StartCollectionThread();
			}
		}

		/// <summary>
		/// Removes an <see cref="ILargeObjectContainer"/> from the memory manager's list.
		/// </summary>
		public static void Remove(ILargeObjectContainer container)
		{
			lock (_syncLock)
			{
				_containersToAdd.Remove(container);
				_containersToRemove.Add(container);
			}
		}

		#region Collect

		/// <summary>
		/// Triggers an immediate call to <see cref="IMemoryManagementStrategy.Collect"/>
		/// and does not wait for it to complete.
		/// </summary>
		/// <remarks>Note that this call does not guarantee that any memory will be
		/// collected by the <see cref="IMemoryManagementStrategy"/>.</remarks>
		public static void Collect()
		{
			Collect(false);
		}

		/// <summary>
		/// Triggers an immediate call to <see cref="IMemoryManagementStrategy.Collect"/>
		/// and will wait for it to complete, if <paramref name="wait"/> is true.
		/// </summary>
		/// <remarks>Note that this call does not guarantee that any memory will be
		/// collected by the <see cref="IMemoryManagementStrategy"/>.</remarks>
		public static void Collect(bool wait)
		{
			if (wait)
				Collect(Timeout.Infinite);
			else
				Collect(0);
		}

		/// <summary>
		/// Triggers an immediate call to <see cref="IMemoryManagementStrategy.Collect"/>
		/// and will wait up to <paramref name="waitTimeoutMilliseconds"/> for it to complete.
		/// </summary>
		/// <remarks>Note that this call does not guarantee that any memory will be
		/// collected by the <see cref="IMemoryManagementStrategy"/>.</remarks>
		public static void Collect(int waitTimeoutMilliseconds)
		{
			Collect(TimeSpan.FromMilliseconds(waitTimeoutMilliseconds));
		}

		/// <summary>
		/// Triggers an immediate call to <see cref="IMemoryManagementStrategy.Collect"/>
		/// and will wait up to <paramref name="waitTimeout"/> for it to complete.
		/// </summary>
		/// <remarks>Note that this call does not guarantee that any memory will be
		/// collected by the <see cref="IMemoryManagementStrategy"/>.</remarks>
		public static void Collect(TimeSpan waitTimeout)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			new MemoryCollector(waitTimeout).Collect();

			clock.Stop();
			if (Platform.IsLogLevelEnabled(LogLevel.Debug)) 
			{
				Platform.Log(LogLevel.Debug, 
					"Waited a total of {0} for memory to be collected; max wait time was {1} seconds.", clock, waitTimeout.TotalSeconds);
			}
		}

		#endregion

		#region Execute

		/// <summary>
		/// Executes the given <see cref="RetryableCommand"/> inside the memory manager.
		/// </summary>
		/// <remarks>
		/// The <see cref="RetryableCommand"/> will be executed at least once, but may be called
		/// repeatedly in the case where it throws an <see cref="OutOfMemoryException"/>.  When the
		/// <see cref="RetryableCommand"/> does throw an <see cref="OutOfMemoryException"/>, a memory
		/// collection will be triggered and the memory manager will continue to call the <see cref="RetryableCommand"/>
		/// until the call succeeds or the default timeout of 1 second has elapsed.  If the timeout
		/// has elapsed and the call fails, the <see cref="OutOfMemoryException"/> is rethrown.
		/// </remarks>
		public static void Execute(RetryableCommand retryableCommand)
		{
			Execute(retryableCommand, _defaultWaitTimeMilliseconds);
		}

		/// <summary>
		/// Executes the given <see cref="RetryableCommand"/> inside the memory manager.
		/// </summary>
		/// <remarks>
		/// The <see cref="RetryableCommand"/> will be executed at least once, but may be called
		/// repeatedly in the case where it throws an <see cref="OutOfMemoryException"/>.  When the
		/// <see cref="RetryableCommand"/> does throw an <see cref="OutOfMemoryException"/>, a memory
		/// collection will be triggered and the memory manager will continue to call the <see cref="RetryableCommand"/>
		/// until the call succeeds or <paramref name="maxWaitTimeMilliseconds"/> has elapsed.  If the timeout
		/// has elapsed and the call fails, the <see cref="OutOfMemoryException"/> is rethrown.
		/// </remarks>
		public static void Execute(RetryableCommand retryableCommand, int maxWaitTimeMilliseconds)
		{
			Execute(retryableCommand, TimeSpan.FromMilliseconds(maxWaitTimeMilliseconds));
		}

		/// <summary>
		/// Executes the given <see cref="RetryableCommand"/> inside the memory manager.
		/// </summary>
		/// <remarks>
		/// The <see cref="RetryableCommand"/> will be executed at least once, but may be called
		/// repeatedly in the case where it throws an <see cref="OutOfMemoryException"/>.  When the
		/// <see cref="RetryableCommand"/> does throw an <see cref="OutOfMemoryException"/>, a memory
		/// collection will be triggered and the memory manager will continue to call the <see cref="RetryableCommand"/>
		/// until the call succeeds or <paramref name="maxWaitTime"/> has elapsed.  If the timeout
		/// has elapsed and the call fails, the <see cref="OutOfMemoryException"/> is rethrown.
		/// </remarks>
		public static void Execute(RetryableCommand retryableCommand, TimeSpan maxWaitTime)
		{
			new RetryableCommandExecutor(retryableCommand, maxWaitTime).Execute();
		}


		#endregion

		/// <summary>
		/// Attempts to allocate an array of the specified type.
		/// </summary>
		/// <remarks>The memory manager will try to allocate a buffer of the requested size.  If an
		/// <see cref="OutOfMemoryException"/> is detected, a memory
		/// collection will be triggered and the memory manager will continue to try and allocate the buffer
		/// until it is successful, or the default timeout of 1 second has elapsed.  If the timeout
		/// has elapsed and the allocation fails, the <see cref="OutOfMemoryException"/> is rethrown.
		/// </remarks>
		public static T[] Allocate<T>(int count)
		{
			return Allocate<T>(count, _defaultWaitTimeMilliseconds);
		}

		/// <summary>
		/// Attempts to allocate an array of the specified type.
		/// </summary>
		/// <remarks>The memory manager will try to allocate a buffer of the requested size.  If an
		/// <see cref="OutOfMemoryException"/> is detected, a memory
		/// collection will be triggered and the memory manager will continue to try and allocate the buffer
		/// until it is successful, or <paramref name="maxWaitTimeMilliseconds"/> has elapsed.  If the timeout
		/// has elapsed and the allocation fails, the <see cref="OutOfMemoryException"/> is rethrown.
		/// </remarks>
		public static T[] Allocate<T>(int count, int maxWaitTimeMilliseconds)
		{
			return Allocate<T>(count, TimeSpan.FromMilliseconds(maxWaitTimeMilliseconds));
		}

		/// <summary>
		/// Attempts to allocate an array of the specified type.
		/// </summary>
		/// <remarks>The memory manager will try to allocate a buffer of the requested size.  If an
		/// <see cref="OutOfMemoryException"/> is detected, a memory
		/// collection will be triggered and the memory manager will continue to try and allocate the buffer
		/// until it is successful, or or <paramref name="maxWaitTime"/> has elapsed.  If the timeout
		/// has elapsed and the allocation fails, the <see cref="OutOfMemoryException"/> is rethrown.
		/// </remarks>
		public static T[] Allocate<T>(int count, TimeSpan maxWaitTime)
		{
			T[] returnValue = null;
			Execute(delegate { returnValue = new T[count]; }, maxWaitTime);

			if (returnValue == null)
				returnValue = new T[count];

			return returnValue;
		}

		#endregion
	}
}
