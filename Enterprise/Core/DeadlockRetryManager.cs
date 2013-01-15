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

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Encapsulates logic for re-trying a given block of code in the event of a <see cref="DeadlockException"/>.
	/// </summary>
	class DeadlockRetryManager : IDisposable
	{
		public delegate void Action();

		/// <summary>
		/// Gets or sets the maximum number of attempts that will be made before giving up.
		/// </summary>
		public int MaxAttempts { get; set; }

		/// <summary>
		/// Gets or sets the minimum wait time in milliseconds between retries.
		/// </summary>
		public int MinWaitTime { get; set; }

		/// <summary>
		/// Gets or sets the maximum wait time in milliseconds between retries.
		/// </summary>
		public int MaxWaitTime { get; set; }

		/// <summary>
		/// Gets the number of attempts already made.
		/// </summary>
		public int Attempts { get; private set; }

		private readonly Random _random;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DeadlockRetryManager()
		{
			_random = new Random();

			// set defaults
			MaxAttempts = 3;
			MinWaitTime = 100;
			MaxWaitTime = 500;
		}

		/// <summary>
		/// Executes the specified action, re-trying in the event of a <see cref="DeadlockException"/>.
		/// </summary>
		/// <remarks>
		/// The specified action must retryable - that is, it must be formulated such that it can be executed
		/// repeatedly in the event that it encounters a deadlock situation.  If a deadlock situation is
		/// encountered, the action must throw a <see cref="DeadlockException"/>.
		/// </remarks>
		/// <param name="action"></param>
		public void Execute(Action action)
		{
			while (true)
			{
				try
				{
					action();

					// success, break out of retry loop
					break;
				}
				catch (DeadlockException)
				{
					Attempts++;

					if (ShouldRetry)
					{
						Platform.Log(LogLevel.Warn, "Deadlock detected - will retry operation.");
						WaitRandom();
					}
					else
					{
						// rethrow
						throw;
					}
				}
			}
		}

		public void Dispose()
		{
		}


		private bool ShouldRetry
		{
			get { return Attempts < MaxAttempts; }
		}

		private void WaitRandom()
		{
			// wait a random amount of time, so that other deadlock counterparts
			// don't retry at exactly the same instant
			Thread.Sleep(_random.Next(MinWaitTime, MaxWaitTime));
		}
	}
}
