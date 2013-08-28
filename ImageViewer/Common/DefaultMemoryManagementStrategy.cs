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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
    internal interface IMemoryInfo
    {
        long ProcessVirtualMemoryBytes { get; }
        long ProcessPrivateBytes { get; }
        long ProcessWorkingSetBytes { get; }
        long GCTotalBytesAllocated { get; }
        long SystemFreeMemoryBytes { get; }

        long MemoryManagerLargeObjectBytesCount { get; }

        long HighWaterMarkBytes { get; }
        long LowWaterMarkBytes { get; }

        double HeldMemoryToCollectPercent { get; }
        
        long x64MinimumFreeSystemMemoryBytes { get; }
        double x64MaxMemoryUsagePercent { get; }
        long x64MaxMemoryToCollectBytes { get; }

        void Refresh();
    }

    internal partial class DefaultMemoryManagementStrategy
    {
        /// Exists separately with public settable properties for unit testing purposes.
        internal class MemoryInfo : IMemoryInfo
        {
            #region IMemoryInfo Members

            public long ProcessVirtualMemoryBytes { get; set; }
            public long ProcessPrivateBytes { get; set; }
            public long ProcessWorkingSetBytes { get; set; }
            public long GCTotalBytesAllocated { get; set; }
            public long SystemFreeMemoryBytes { get; set; }

            public long MemoryManagerLargeObjectBytesCount { get; set; }

            public long HighWaterMarkBytes { get; set; }
            public long LowWaterMarkBytes { get; set; }

            public double HeldMemoryToCollectPercent { get; set; }

            public long x64MinimumFreeSystemMemoryBytes { get; set; }
            public double x64MaxMemoryUsagePercent { get; set; }
            public long x64MaxMemoryToCollectBytes { get; set; }


            public void Refresh()
            {
                var clock = new CodeClock();
                clock.Start();

                var currentProcess = Process.GetCurrentProcess();
                currentProcess.Refresh();

                ProcessVirtualMemoryBytes = currentProcess.VirtualMemorySize64;
                ProcessPrivateBytes = currentProcess.PrivateMemorySize64;
                ProcessWorkingSetBytes = currentProcess.WorkingSet64;
                GCTotalBytesAllocated = GC.GetTotalMemory(false);
                SystemFreeMemoryBytes = SystemResources.GetAvailableMemory(SizeUnits.Bytes);

                HighWaterMarkBytes = MemoryManagementSettings.Default.HighWatermarkMegaBytes*OneMegabyte;
                LowWaterMarkBytes = MemoryManagementSettings.Default.LowWatermarkMegaBytes*OneMegabyte;
                HeldMemoryToCollectPercent = MemoryManagementSettings.Default.HeldMemoryToCollectPercent/100.0;

                x64MinimumFreeSystemMemoryBytes = MemoryManagementSettings.Default.x64MinimumFreeSystemMemoryMegabytes*OneMegabyte;
                x64MaxMemoryUsagePercent = MemoryManagementSettings.Default.x64MaxMemoryUsagePercent/100.0;
                x64MaxMemoryToCollectBytes = MemoryManagementSettings.Default.x64MaxMemoryToCollectMegabytes*OneMegabyte;

                MemoryManagerLargeObjectBytesCount = MemoryManager.LargeObjectBytesCount;

                clock.Stop();

                PerformanceReportBroker.PublishReport("Memory", "UpdateMemoryInfo", clock.Seconds);
            }

            #endregion
        }
    }


    internal partial class DefaultMemoryManagementStrategy : MemoryManagementStrategy
	{
        private const LogLevel _debugLogLevel = LogLevel.Debug;

		private const long OneKilobyte = 1024;
		private const long OneMegabyte = 1024 * OneKilobyte;
		private const long OneGigabyte = 1024 * OneMegabyte;
		private const long TwoGigabytes = 2 * OneGigabyte;

		private const double TwentyFivePercent = 0.25;
        private const double FortyPercent = 0.40;

		private IEnumerator<ILargeObjectContainer> _largeObjectEnumerator;
		
		private RegenerationCost _regenerationCost;
		private DateTime _collectionStartTime;
		private DateTime _lastCollectionTime;
		private TimeSpan _timeSinceLastCollection;
		private TimeSpan _maxTimeSinceLastAccess;
		private TimeSpan _maxTimeSinceLastAccessDecrement;

		private int _totalNumberOfCollections;
		private long _totalBytesCollected;
		private int _totalLargeObjectsCollected;
		private int _totalContainersUnloaded;

        internal IMemoryInfo _memoryInfo = new MemoryInfo();
        internal bool _is32BitProcess = IntPtr.Size == 4;
        
	    public DefaultMemoryManagementStrategy()
		{
			_lastCollectionTime = DateTime.Now;
		}

		#region IMemoryManagementStrategy Members

		public override void Collect(MemoryCollectionArgs collectionArgs)
		{
			_largeObjectEnumerator = collectionArgs.LargeObjectContainers.GetEnumerator();

			_regenerationCost = RegenerationCost.Low;

			//TODO (Time Review): Use Environment.TickCount?
			_collectionStartTime = DateTime.Now;
			_timeSinceLastCollection = _collectionStartTime - _lastCollectionTime;
			TimeSpan thirtySeconds = TimeSpan.FromSeconds(30);
			if (_timeSinceLastCollection < thirtySeconds)
			{
                Platform.Log(_debugLogLevel, "Time since last collection is less than 30 seconds; adjusting to 30 seconds.");
				_timeSinceLastCollection = thirtySeconds;
			}

			_maxTimeSinceLastAccess = _timeSinceLastCollection;
			_maxTimeSinceLastAccessDecrement = TimeSpan.FromSeconds(_timeSinceLastCollection.TotalSeconds / 3);

			_totalNumberOfCollections = 0;
			_totalBytesCollected = 0;
			_totalLargeObjectsCollected = 0;
			_totalContainersUnloaded = 0;

			try
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				Collect();

				clock.Stop();
				PerformanceReportBroker.PublishReport("Memory", "Collect", clock.Seconds);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Default memory management strategy failed to collect.");
			}
			finally
			{
				DateTime collectionEndTime = DateTime.Now;
				if (_totalContainersUnloaded > 0)
					_lastCollectionTime = collectionEndTime;

				_largeObjectEnumerator = null;

				TimeSpan totalElapsed = collectionEndTime - _collectionStartTime;

				var finalArgs = new MemoryCollectedEventArgs(
					_totalContainersUnloaded, _totalLargeObjectsCollected, _totalBytesCollected, totalElapsed, true);

                if ( _totalNumberOfCollections != 0
                  || _totalBytesCollected != 0
                  || _totalLargeObjectsCollected != 0
			      ||_totalContainersUnloaded != 0)
				    Platform.Log(LogLevel.Info, 
					    "Large object collection summary: freed {0} MB in {1} seconds and {2} iterations, Total Containers: {3}, Total Large Objects: {4}",
					    _totalBytesCollected/(float)OneMegabyte,
					    totalElapsed.TotalSeconds,
					    _totalNumberOfCollections,
					    _totalContainersUnloaded,
					    _totalLargeObjectsCollected);

				OnMemoryCollected(finalArgs);
			}
		}

		#endregion

        internal long Get32BitMemoryHighWatermark()
        {
            long currentVirtualMemorySize = _memoryInfo.ProcessVirtualMemoryBytes;
            long maxTheoreticalVirtualMemorySize = currentVirtualMemorySize + _memoryInfo.SystemFreeMemoryBytes;

            //On 32-bit systems, a process cannot have more than 2GB of virtual address space,
            //unless the /3GB switch is set and the process is "large address aware".
            //On 64-bit systems, a 32-bit process *can* actually have 4GB of virtual address space, but there are all sorts
            //of exceptions to that, depending on whether the process is "large address aware", etc.
            //So, because a lot of these things are hard or impossible to figure out, we just assume 32-bit processes
            //max out at 2GB of virtual memory, and if you need more than that, buy an x64 machine :)
            const long maxSystemVirtualMemorySize = TwoGigabytes;
            long maxVirtualMemorySizeBytes = Math.Min(maxTheoreticalVirtualMemorySize, maxSystemVirtualMemorySize);
            
            //Forty percent was carefully selected based on experiment. Basically, if we don't keep the memory being used
            //fairly low, the process heap gets fragmented and things start crashing when you can't get a large enough
            //contiguous block of memory for an image.
            return (long)(maxVirtualMemorySizeBytes * FortyPercent);
        }

        internal long Get64BitMemoryHighWatermark()
        {
            //We don't want to exhaust system memory so the machine is strapped.
            long maxAvailableSystemMemory = _memoryInfo.SystemFreeMemoryBytes;
            maxAvailableSystemMemory = Math.Max(0, maxAvailableSystemMemory - _memoryInfo.x64MinimumFreeSystemMemoryBytes);

            var theoreticalMax = _memoryInfo.ProcessPrivateBytes + maxAvailableSystemMemory;
            var highWatermark = (long)(_memoryInfo.x64MaxMemoryUsagePercent * theoreticalMax);
            return highWatermark;
        }

        internal long GetMemoryHighWatermarkBytes()
        {
            if (_memoryInfo.HighWaterMarkBytes < 0)
            {
                return _is32BitProcess ? Get32BitMemoryHighWatermark() : Get64BitMemoryHighWatermark();
            }
            else
            {
                return _memoryInfo.HighWaterMarkBytes;
            }
        }

        internal long GetMemoryLowWatermarkBytes(long highWatermark)
        {
            if (_memoryInfo.LowWaterMarkBytes < 0)
            {
                var amountToCollect = (long)(_memoryInfo.MemoryManagerLargeObjectBytesCount * _memoryInfo.HeldMemoryToCollectPercent);
                if (!_is32BitProcess)
                {
                    //Don't allow more than 2GB of memory to be collected, so that the CLR doesn't go haywire and pause for a really long time.
                    var maxAmountToCollect = _memoryInfo.x64MaxMemoryToCollectBytes;
                    if (amountToCollect > maxAmountToCollect)
                        amountToCollect = maxAmountToCollect;
                }

                return highWatermark - amountToCollect;
            }
            else
            {
                return _memoryInfo.LowWaterMarkBytes;
            }
        }

        // CR (JY 2013-APR-15): There is an interesting (read: obscure) unaccounted for scenario here that perhaps makes memory collection not as efficient as it could be
        // It is possible that the regeneration cost of a container is tied to the data held by another container
        // - e.g. MPR cached volumes - if a volume is unloaded, its derived frames elevate their regeneration cost
        // - Hypothetically, you could also have the reverse - a container has data that *must* be reloaded if its parent container is unloaded
        //   (and the actual unload is expensive, so rather than doing it immediately, it lowers the cost so that the memmgr can unload it at leisure)
        // - It is also possible that the regeneration cost of a container just changes due to some other unrelated event, after it has already been evaluated on the pass for that cost value
        //   e.g. on the 'low' cost pass, the objet is medium, but before the 'medium' cost pass executes, it changes to low.
        // - to account for these dynamically assessed costs, the condition should be 'container.RegenerationCost <= _regenerationCost'
        // - further, because of these dynamically assessed costs, it is possible that the same container could be yielded multiple times throughout the process of one collect
        //   which would actually be desirable if the container still held on to objects (perhaps because it refused to unload it the first time around)
        // - a check for container.BytesHeld > 0 would alleviate the number of 'already empty' containers yielded in each batch, making each pass able to collect more real objects and not ghosts
        private IEnumerable<ILargeObjectContainer> GetNextBatchOfContainersToCollect(int batchSize)
		{
			int i = 0;
			while (_regenerationCost <= RegenerationCost.High)
			{
				bool timeGreaterThanOrEqualToZero = true;
				while (timeGreaterThanOrEqualToZero)
				{
					while (_largeObjectEnumerator.MoveNext())
					{
						ILargeObjectContainer container = _largeObjectEnumerator.Current;
						if (!container.IsLocked && container.RegenerationCost == _regenerationCost)
						{
							TimeSpan timeSinceLastAccess = _collectionStartTime - container.LastAccessTime;
							if (timeSinceLastAccess >= _maxTimeSinceLastAccess)
							{
								yield return container;

								if (++i == batchSize)
									yield break;
							}
						}
					}

					_largeObjectEnumerator.Reset();

					// check this before doing the decrement for the odd case where the decrement takes 
					// the time slightly past zero when it should be exactly zero.
					timeGreaterThanOrEqualToZero = _maxTimeSinceLastAccess >= TimeSpan.Zero;
					_maxTimeSinceLastAccess -= _maxTimeSinceLastAccessDecrement;
				}

				_maxTimeSinceLastAccess = _timeSinceLastCollection;
				++_regenerationCost;
			}
		}

		private void Collect()
		{
			Collect(GetBytesToCollect());
		}

		private long GetBytesToCollect()
		{
			try
			{
				_memoryInfo.Refresh();

				long highWatermark = GetMemoryHighWatermarkBytes();
				long lowWatermark = GetMemoryLowWatermarkBytes(highWatermark);

				long bytesAboveHighWatermark = _memoryInfo.ProcessPrivateBytes - highWatermark;
                long bytesToCollect = _memoryInfo.ProcessPrivateBytes - lowWatermark;

                if (Platform.IsLogLevelEnabled(_debugLogLevel))
				{
                    Platform.Log(_debugLogLevel,
						"Virtual Memory (MB): {0}, Private Bytes (MB): {1}, Working Set (MB): {2}, GC Total Bytes Allocated (MB): {3}, System Free (MB): {4}",
                        _memoryInfo.ProcessVirtualMemoryBytes / (float)OneMegabyte,
                        _memoryInfo.ProcessPrivateBytes / (float)OneMegabyte,
                        _memoryInfo.ProcessWorkingSetBytes / (float)OneMegabyte,
                        _memoryInfo.GCTotalBytesAllocated / (float)OneMegabyte,
                        _memoryInfo.SystemFreeMemoryBytes / (float)OneMegabyte);

                    Platform.Log(_debugLogLevel,
						"Large Object Containers: {0}, Large Objects Held: {1}, Total Large Object Bytes (MB): {2}",
						MemoryManager.LargeObjectContainerCount,
						MemoryManager.LargeObjectCount,
						MemoryManager.LargeObjectBytesCount / (float)OneMegabyte);

                    Platform.Log(_debugLogLevel,
						"High Watermark (MB): {0}, Low Watermark (MB): {1}, MB Above: {2}, MB To Collect: {3}",
						highWatermark/(float)OneMegabyte,
						lowWatermark/(float)OneMegabyte,
						bytesAboveHighWatermark/(float)OneMegabyte,
						bytesToCollect/(float)OneMegabyte);
				}

				return (bytesAboveHighWatermark > 0) ? bytesToCollect : 0;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Failure when trying to determine number of bytes to collect; collecting 25% of held memory (up to 1GB max) ...");
				var twentyFivePercentOfHeld = (long)(MemoryManager.LargeObjectBytesCount * TwentyFivePercent);
			    return Math.Min(twentyFivePercentOfHeld, OneGigabyte);
			}
		}

		private void Collect(long bytesToCollect)
		{
			bool continueCollecting = false;
			bool needMoreMemorySignalled = false;

			if (bytesToCollect <= 0)
			{
				Platform.Log(_debugLogLevel,
					"Memory is not above high watermark; firing collected event to check if more memory is required.");

				var args = new MemoryCollectedEventArgs(0, 0, 0, TimeSpan.Zero, false);
				OnMemoryCollected(args);
				continueCollecting = needMoreMemorySignalled = args.NeedMoreMemory;
			}
			else
			{
				continueCollecting = true;
				Platform.Log(_debugLogLevel, "Memory *is* above high watermark; collecting ...");
			}

			if (!continueCollecting)
				return;

			int batchSize = 10;
			int collectionNumber = 0;

			while (continueCollecting)
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				long bytesCollected = 0;
				int largeObjectsCollected = 0;
				int containersUnloaded = 0;
				int i = 0;

				foreach (ILargeObjectContainer container in GetNextBatchOfContainersToCollect(batchSize))
				{
					++i;

					try
					{
						long bytesHeldBefore = container.BytesHeldCount;
						int largeObjectsHeldBefore = container.LargeObjectCount;

						container.Unload();

						long bytesHeldAfter = container.BytesHeldCount;
						int largeObjectsHeldAfter = container.LargeObjectCount;

						int largeObjectsHeldDifference = largeObjectsHeldBefore - largeObjectsHeldAfter;
						largeObjectsCollected += largeObjectsHeldDifference;
						if (largeObjectsHeldDifference > 0)
							++containersUnloaded;

						long bytesDifference = (bytesHeldBefore - bytesHeldAfter);
						bytesCollected += bytesDifference;
						_totalBytesCollected += bytesDifference;
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Warn, e, "An unexpected error occurred while attempting to collect large object memory.");
					}

					//when needMoreMemorySignalled is true, we need to be more aggressive and keep collecting.
					if (!needMoreMemorySignalled && _totalBytesCollected >= bytesToCollect)
						break;
				}

				batchSize *= 2;

				_totalContainersUnloaded += containersUnloaded;
				_totalLargeObjectsCollected += largeObjectsCollected;
				++_totalNumberOfCollections;

				clock.Stop();

				continueCollecting = i > 0;
				if (!continueCollecting)
					continue;

				PerformanceReportBroker.PublishReport("Memory", "CollectionIteration", clock.Seconds);

				var args = new MemoryCollectedEventArgs(containersUnloaded, largeObjectsCollected, bytesCollected, TimeSpan.FromSeconds(clock.Seconds), false);
				OnMemoryCollected(args);

				needMoreMemorySignalled = args.NeedMoreMemory;
				continueCollecting = needMoreMemorySignalled || _totalBytesCollected < bytesToCollect;

                if (Platform.IsLogLevelEnabled(_debugLogLevel))
				{
                    Platform.Log(_debugLogLevel, 
					             "Large object collection #{0}: freed {1} MB in {2}, Containers Unloaded: {3}, Large Objects Collected: {4}, Need More Memory: {5}, Last Batch: {6}",
					             ++collectionNumber, args.BytesCollectedCount / (float)OneMegabyte, clock,
					             containersUnloaded, largeObjectsCollected, needMoreMemorySignalled, i);
				}
			}

			CollectGarbage();
		}

		private void CollectGarbage()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			Platform.Log(_debugLogLevel, "Performing garbage collection.");
			GC.Collect();

			clock.Stop();
			PerformanceReportBroker.PublishReport("Memory", "GarbageCollection", clock.Seconds);
		}
	}
}
