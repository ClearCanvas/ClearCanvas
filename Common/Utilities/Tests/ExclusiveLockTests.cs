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

#if UNIT_TESTS

using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	internal class ExclusiveLockTests
	{
		[Test]
		public void TestInstanceFileSystemLock()
		{
			const int numTesters = 10;
			const int testLengthSeconds = 30;

			var testers = new InstanceFileSystemLockTester[numTesters];
			for (var n = 0; n < testers.Length; ++n)
				testers[n] = new InstanceFileSystemLockTester(n, testLengthSeconds);

			Trace.WriteLine(string.Format(@"Starting {0} tester threads to test for {1} seconds.", numTesters, testLengthSeconds));
			foreach (var tester in testers)
				tester.Start();

			Trace.WriteLine(string.Format(@"Test is running..."));
			foreach (var tester in testers)
				tester.Wait();

			Trace.WriteLine(string.Format(@"Test finished."));
			foreach (var tester in testers)
				Assert.IsFalse(tester.Failed, @"Tester {0} recorded a failure", tester.TesterId);

			Trace.WriteLine(string.Format(@"No synchronization errors found."));
		}

		[Test]
		public void TestStaticFileSystemLock()
		{
			const int numTesters = 10;
			const int testLengthSeconds = 30;

			var testers = new StaticFileSystemLockTester[numTesters];
			for (var n = 0; n < testers.Length; ++n)
				testers[n] = new StaticFileSystemLockTester(n, testLengthSeconds);

			Trace.WriteLine(string.Format(@"Starting {0} tester threads to test for {1} seconds.", numTesters, testLengthSeconds));
			foreach (var tester in testers)
				tester.Start();

			Trace.WriteLine(string.Format(@"Test is running..."));
			foreach (var tester in testers)
				tester.Wait();

			Trace.WriteLine(string.Format(@"Test finished."));
			foreach (var tester in testers)
				Assert.IsFalse(tester.Failed, @"Tester {0} recorded a failure", tester.TesterId);

			Trace.WriteLine(string.Format(@"No synchronization errors found."));
		}

		private class InstanceFileSystemLockTester : LockTesterBase
		{
			private static int _counter = 0;

			public InstanceFileSystemLockTester(int testerId, ushort testLengthSeconds)
				: base(testerId, testLengthSeconds) {}

			protected override void Execute()
			{
				var start = Environment.TickCount;
				var random = new Random(TesterId);
				do
				{
					// for simulating a broken lock to verify that this test does what is intended, use x as conditional on _lock calls
					// var x = _testerId != random.Next(0, 1000); 

					var @lock = ExclusiveLock.CreateFileSystemLock(typeof (InstanceFileSystemLockTester).FullName);
					@lock.Lock();
					Interlocked.Increment(ref _counter);
					try
					{
						Thread.Sleep(random.Next(1, 20)); // add some variability to iteration length so that threads aren't just running in lock step
						Assert.AreEqual(1, _counter, @"Tester {0} entered lock while another tester has lock", TesterId);
						FlagFailed(_counter != 1);
					}
					finally
					{
						Interlocked.Decrement(ref _counter);
						@lock.Unlock();
					}
				} while (Environment.TickCount - start < TestLengthMs);
			}
		}

		private class StaticFileSystemLockTester : LockTesterBase
		{
			private static readonly ExclusiveLock _lock = ExclusiveLock.CreateFileSystemLock(typeof (StaticFileSystemLockTester).FullName);
			private static int _counter = 0;

			public StaticFileSystemLockTester(int testerId, ushort testLengthSeconds)
				: base(testerId, testLengthSeconds) {}

			protected override void Execute()
			{
				var start = Environment.TickCount;
				var random = new Random(TesterId);
				do
				{
					// for simulating a broken lock to verify that this test does what is intended, use x as conditional on _lock calls
					// var x = _testerId != random.Next(0, 1000); 

					_lock.Lock();
					Interlocked.Increment(ref _counter);
					try
					{
						Thread.Sleep(random.Next(1, 20)); // add some variability to iteration length so that threads aren't just running in lock step
						Assert.AreEqual(1, _counter, @"Tester {0} entered lock while another tester has lock", TesterId);
						FlagFailed(_counter != 1);
					}
					finally
					{
						Interlocked.Decrement(ref _counter);
						_lock.Unlock();
					}
				} while (Environment.TickCount - start < TestLengthMs);
			}
		}

		private abstract class LockTesterBase
		{
			private TestMethod _testMethod;
			private IAsyncResult _asyncResult;

			private delegate void TestMethod();

			protected LockTesterBase(int testerId, ushort testLengthSeconds)
			{
				TesterId = testerId;
				TestLengthMs = 1000*testLengthSeconds;
				Failed = false;
			}

			public int TesterId { get; private set; }

			protected int TestLengthMs { get; private set; }

			public bool Failed { get; private set; }

			public void Start()
			{
				_testMethod = Execute;
				_asyncResult = _testMethod.BeginInvoke(null, null);
			}

			public void Wait()
			{
				_testMethod.EndInvoke(_asyncResult);
			}

			protected void FlagFailed(bool failureCondition)
			{
				if (failureCondition)
					Failed = true;
			}

			protected abstract void Execute();
		}
	}
}

#endif