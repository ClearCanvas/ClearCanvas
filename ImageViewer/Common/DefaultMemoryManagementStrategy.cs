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
	internal class DefaultMemoryManagementStrategy : MemoryManagementStrategy
	{
		private const string x86Architecture = "x86";

		private const long OneKilobyte = 1024;
		private const long OneMegabyte = 1024 * OneKilobyte;
		private const long OneGigabyte = 1024 * OneMegabyte;
		private const long TwoGigabytes = 2 * OneGigabyte;
		private const long ThreeGigabytes = 3 * OneGigabyte;
		private const long EightGigabytes = 8 * OneGigabyte;

		private const float TwentyFivePercent = 0.25F;
		private const float FortyPercent = 0.40F;
		private const float NinetyPercent = 0.90F;

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

		private long _processVirtualMemoryBytes;
		private long _processPrivateBytes;
		private long _processWorkingSetBytes;
		private long _gcTotalBytesAllocated;

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
				Platform.Log(LogLevel.Debug, "Time since last collection is less than 30 seconds; adjusting to 30 seconds.");
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

				MemoryCollectedEventArgs finalArgs = new MemoryCollectedEventArgs(
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

		private bool Is3GigEnabled()
		{
			//TODO: can't actually figure this out ...
			return false;
		}

		private void UpdateProcessMemoryData()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			Process currentProcess = Process.GetCurrentProcess();
			currentProcess.Refresh();

			_processVirtualMemoryBytes = currentProcess.VirtualMemorySize64;
			_processPrivateBytes = currentProcess.PrivateMemorySize64;
			_processWorkingSetBytes = currentProcess.WorkingSet64;
			_gcTotalBytesAllocated = GC.GetTotalMemory(false);

			clock.Stop();

			PerformanceReportBroker.PublishReport("Memory", "UpdateProcessMemoryData", clock.Seconds);
		}

		private long GetMaxVirtualMemorySizeBytes()
		{
			long currentVirtualMemorySize = _processVirtualMemoryBytes;
			long maxTheoreticalVirtualMemorySize = currentVirtualMemorySize + SystemResources.GetAvailableMemory(SizeUnits.Bytes);
			long maxVirtualMemorySizeBytes;

			if (Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") == x86Architecture)
			{
				long maxSystemVirtualMemorySize = Is3GigEnabled() ? ThreeGigabytes : TwoGigabytes;
				maxVirtualMemorySizeBytes = Math.Min(maxTheoreticalVirtualMemorySize, maxSystemVirtualMemorySize);
			}
			else
			{
				//let's not get greedy :)
				maxVirtualMemorySizeBytes = Math.Min(EightGigabytes, maxTheoreticalVirtualMemorySize);
			}

			return maxVirtualMemorySizeBytes;
		}

		private long GetMemoryHighWatermarkBytes()
		{
			if (MemoryManagementSettings.Default.HighWatermarkMegaBytes < 0)
				return (long)(GetMaxVirtualMemorySizeBytes() * FortyPercent);
			else
				return MemoryManagementSettings.Default.HighWatermarkMegaBytes * OneMegabyte;
		}

		private long GetMemoryLowWatermarkBytes(long highWatermark)
		{
			if (MemoryManagementSettings.Default.LowWatermarkMegaBytes < 0)
				return (long)(highWatermark - MemoryManager.LargeObjectBytesCount * TwentyFivePercent);
			else
				return Math.Min(MemoryManagementSettings.Default.LowWatermarkMegaBytes * OneMegabyte, (long)(highWatermark * NinetyPercent));
		}

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
				UpdateProcessMemoryData();

				long highWatermark = GetMemoryHighWatermarkBytes();
				long lowWatermark = GetMemoryLowWatermarkBytes(highWatermark);

				long bytesAboveHighWatermark = _processPrivateBytes - highWatermark;
				long bytesToCollect = _processPrivateBytes - lowWatermark;

				if (Platform.IsLogLevelEnabled(LogLevel.Debug))
				{
					Platform.Log(LogLevel.Debug,
						"Virtual Memory (MB): {0}, Private Bytes (MB): {1}, Working Set (MB): {2}, GC Total Bytes Allocated (MB): {3}",
						_processVirtualMemoryBytes / (float)OneMegabyte,
						_processPrivateBytes / (float)OneMegabyte,
						_processWorkingSetBytes / (float)OneMegabyte,
						_gcTotalBytesAllocated / (float)OneMegabyte);

					Platform.Log(LogLevel.Debug,
						"Large Object Containers: {0}, Large Objects Held: {1}, Total Large Object Bytes (MB): {2}",
						MemoryManager.LargeObjectContainerCount,
						MemoryManager.LargeObjectCount,
						MemoryManager.LargeObjectBytesCount / (float)OneMegabyte);

					Platform.Log(LogLevel.Debug,
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
				Platform.Log(LogLevel.Debug, e, "Failure when trying to determine number of bytes to collect; collecting 25% of held memory ...");
				return (long)(MemoryManager.LargeObjectBytesCount * TwentyFivePercent);
			}
		}

		private void Collect(long bytesToCollect)
		{
			bool continueCollecting = false;
			bool needMoreMemorySignalled = false;

			if (bytesToCollect <= 0)
			{
				Platform.Log(LogLevel.Debug,
					"Memory is not above high watermark; firing collected event to check if more memory is required.");

				MemoryCollectedEventArgs args = new MemoryCollectedEventArgs(0, 0, 0, TimeSpan.Zero, false);
				OnMemoryCollected(args);
				continueCollecting = needMoreMemorySignalled = args.NeedMoreMemory;
			}
			else
			{
				continueCollecting = true;
				Platform.Log(LogLevel.Debug, "Memory *is* above high watermark; collecting ...");
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

				MemoryCollectedEventArgs args = new MemoryCollectedEventArgs(
					containersUnloaded, largeObjectsCollected, bytesCollected, TimeSpan.FromSeconds(clock.Seconds), false);

				OnMemoryCollected(args);

				needMoreMemorySignalled = args.NeedMoreMemory;
				continueCollecting = needMoreMemorySignalled || _totalBytesCollected < bytesToCollect;

				if (Platform.IsLogLevelEnabled(LogLevel.Debug))
				{
					Platform.Log(LogLevel.Debug, 
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

			Platform.Log(LogLevel.Debug, "Performing garbage collection.");
			GC.Collect();

			clock.Stop();
			PerformanceReportBroker.PublishReport("Memory", "GarbageCollection", clock.Seconds);
		}
	}
}
