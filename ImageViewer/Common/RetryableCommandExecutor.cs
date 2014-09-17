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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
	public static partial class MemoryManager
	{
		private class RetryableCommandExecutor
		{
			private readonly object _waitObject = new object();
			private bool _needMoreMemory;

			private readonly RetryableCommand _retryableCommand;
			private readonly TimeSpan _maxWaitTime;
			private int _startTicks;
			private TimeSpan _waitTimeRemaining;

			public RetryableCommandExecutor(RetryableCommand retryableCommand, TimeSpan maxWaitTime)
			{
				Platform.CheckForNullReference(retryableCommand, "retryableCommand");
				Platform.CheckNonNegative((int)maxWaitTime.TotalMilliseconds, "maxWaitTime");

				_maxWaitTime = maxWaitTime;
				_retryableCommand = retryableCommand;
				_needMoreMemory = true;
			}
			
			public void Execute()
			{
                _startTicks = Environment.TickCount;

				try
				{
					_retryableCommand();
				}
				catch (OutOfMemoryException)
				{
					UpdateWaitTimeRemaining();
					if (_waitTimeRemaining <= TimeSpan.Zero)
						throw;

					if (Platform.IsLogLevelEnabled(LogLevel.Debug))
					{
						Platform.Log(LogLevel.Debug, "Detected out of memory condition; retrying for up to {0} seconds.",
								 _waitTimeRemaining.TotalSeconds);
					}

					RetryExecute();
				}
			}


			private void RetryExecute()
			{
				lock (_waitObject)
				{
					lock (_syncLock)
					{
						if (_collectionThread == null)
						{
							Platform.Log(LogLevel.Debug, "RetryExecute called with no objects in the cache; returning immediately.");
							return;
						}

						++_waitingClients;
						MemoryCollected += OnMemoryCollected;
						Monitor.Pulse(_syncLock);
					}
					
					try
					{
						DoRetryExecute();
					}
					finally
					{
						//do this no matter what the reason.
						_needMoreMemory = false;
						Monitor.Pulse(_waitObject);
					}
				}
			}

			private void DoRetryExecute()
			{
				int retryNumber = 0;
				while (true)
				{
                    var begin = Environment.TickCount;
					++retryNumber;

					Platform.Log(LogLevel.Debug, "Waiting for memory collected signal before retrying command.");
					Monitor.Wait(_waitObject, _waitTimeRemaining);

					try
					{
						_retryableCommand();
						if (Platform.IsLogLevelEnabled(LogLevel.Debug))
						{
							var elapsed = TimeSpan.FromMilliseconds(Environment.TickCount - begin);
							Platform.Log(LogLevel.Debug, "Retry #{0} succeeded and took {0} seconds.", retryNumber, elapsed.TotalSeconds);
						}

						return;
					}
					catch (OutOfMemoryException)
					{
						this.UpdateWaitTimeRemaining();
						if (_waitTimeRemaining <= TimeSpan.Zero)
						{
							Platform.Log(LogLevel.Debug,
								"Retry #{0} failed and timeout has expired; throwing OutOfMemoryException.", retryNumber);

							throw;
						}

						if (Platform.IsLogLevelEnabled(LogLevel.Debug))
						{
							var elapsed = Environment.TickCount - begin;
							Platform.Log(LogLevel.Debug, "Retry #{0} failed and took {1} seconds; wait time remaining is {2} seconds.",
							retryNumber, elapsed, _waitTimeRemaining.TotalSeconds);
						}
					}
					finally
					{
						Monitor.Pulse(_waitObject);
					}
				}
			}

			private void UpdateWaitTimeRemaining()
			{
				var elapsed = TimeSpan.FromMilliseconds(Environment.TickCount - _startTicks);
				_waitTimeRemaining = _maxWaitTime - elapsed;
				if (_waitTimeRemaining < TimeSpan.Zero)
					_waitTimeRemaining = TimeSpan.Zero;
			}

			private void OnMemoryCollected(object sender, MemoryCollectedEventArgs args)
			{
				lock (_waitObject)
				{
					Platform.Log(LogLevel.Debug, "Received memory collected signal; waiting to see if more memory is required.");
					Monitor.Pulse(_waitObject);
					if (_needMoreMemory)
					{
						Monitor.Wait(_waitObject);
						args.NeedMoreMemory = _needMoreMemory;
					}

					if (!_needMoreMemory)
					{
						lock (_syncLock)
						{
							--_waitingClients;
							MemoryCollected -= OnMemoryCollected;
						}
					}
				}
			}
		}
	}
}
