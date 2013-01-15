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

using NUnit.Framework;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace ClearCanvas.Enterprise.Common.Tests
{
	[TestFixture]
	public class AopInterceptorChainTests
	{
		internal class Counter
		{
			private int _count;

			public int NextValue { get { return _count++; } }
		}

		internal class TestInvocation : AbstractInvocation, IInvocation
		{
			public TestInvocation()
				: base(null, null, null, null, null, new object[] { })
			{
			}

			public int ProceedCount { get; private set; }

			public int InvokeCount { get; private set; }

			public bool Proceeded
			{
				get { return ProceedCount > 0; }
			}

			public bool Invoked
			{
				get { return InvokeCount > 0; }
			}

			protected override void InvokeMethodOnTarget()
			{
				InvokeCount++;
			}

			void IInvocation.Proceed()
			{
				ProceedCount++;
				base.Proceed();
			}
		}

		internal class SimpleInterceptor : IInterceptor
		{
			private readonly Counter _counter;
			private readonly bool _proceed;

			internal SimpleInterceptor(Counter counter)
				:this(counter, true)
			{
			}

			internal SimpleInterceptor(Counter counter, bool proceed)
			{
				_counter = counter;
				_proceed = proceed;
				this.InterceptIndex = -1;
			}

			public int InterceptIndex { get; private set; }
			public int InterceptCount { get; private set; }

			public void Intercept(IInvocation invocation)
			{
				InterceptIndex = _counter.NextValue;
				InterceptCount++;

				if(_proceed)
				{
					invocation.Proceed();
				}
			}
		}

		internal class RetryInterceptor : IInterceptor
		{
			private readonly int _retries;

			internal RetryInterceptor(int retries)
			{
				_retries = retries;
			}

			public int InterceptCount { get; private set; }

			public void Intercept(IInvocation invocation)
			{
				InterceptCount++;
				// of course in reality we would only retry if the call failed,
				// but that isn't relevant here
				for(var i = 0; i < _retries; i++)
				{
					invocation.Proceed();
				}
			}
		}


		[Test]
		public void Test_basic_interception_chain()
		{
			var counter = new Counter();
			var a = new SimpleInterceptor(counter);
			var b = new SimpleInterceptor(counter);
			var c = new SimpleInterceptor(counter);

			var inv = new TestInvocation();

			Assert.IsFalse(inv.Proceeded);
			Assert.IsFalse(inv.Invoked);

			var chain = new AopInterceptorChain(new[] {a, b, c});
			chain.Intercept(inv);

			// check invocation ultimately invoked, but NOT via the Proceed() method
			Assert.IsFalse(inv.Proceeded);
			Assert.IsTrue(inv.Invoked);

			// check each interceptor called in correct order
			Assert.AreEqual(0, a.InterceptIndex);
			Assert.AreEqual(1, b.InterceptIndex);
			Assert.AreEqual(2, c.InterceptIndex);
		}

		[Test]
		public void Test_null_interception_chain()
		{
			var inv = new TestInvocation();

			Assert.IsFalse(inv.Proceeded);
			Assert.IsFalse(inv.Invoked);

			var chain = new AopInterceptorChain(null);
			chain.Intercept(inv);

			// check invocation ultimately invoked, but NOT via the Proceed() method
			Assert.IsFalse(inv.Proceeded);
			Assert.IsTrue(inv.Invoked);
		}

		[Test]
		public void Test_empty_interception_chain()
		{
			var inv = new TestInvocation();

			Assert.IsFalse(inv.Proceeded);
			Assert.IsFalse(inv.Invoked);

			var chain = new AopInterceptorChain(new IInterceptor[0]);
			chain.Intercept(inv);

			// check invocation ultimately invoked, but NOT via the Proceed() method
			Assert.IsFalse(inv.Proceeded);
			Assert.IsTrue(inv.Invoked);
		}

		[Test]
		public void Test_interceptor_does_not_proceed()
		{
			var counter = new Counter();
			var a = new SimpleInterceptor(counter, false);

			var inv = new TestInvocation();

			Assert.IsFalse(inv.Proceeded);
			Assert.IsFalse(inv.Invoked);

			var chain = new AopInterceptorChain(new[] { a });
			chain.Intercept(inv);

			// check invocation not invoked
			Assert.IsFalse(inv.Proceeded);
			Assert.IsFalse(inv.Invoked);

			// check each interceptor called in correct order
			Assert.AreEqual(0, a.InterceptIndex);
		}

		[Test]
		public void Test_retry_interceptor()
		{
			var counter = new Counter();
			var a = new SimpleInterceptor(counter);
			var b = new RetryInterceptor(10);
			var c = new SimpleInterceptor(counter);

			var inv = new TestInvocation();

			Assert.AreEqual(0, inv.ProceedCount);
			Assert.AreEqual(0, inv.InvokeCount);

			var chain = new AopInterceptorChain(new IInterceptor[] { a, b, c });
			chain.Intercept(inv);

			// check invocation ultimately invoked 10 times, but NOT via the Proceed() method
			Assert.AreEqual(0, inv.ProceedCount);
			Assert.AreEqual(10, inv.InvokeCount);

			// check each interceptor called correct number of times
			Assert.AreEqual(1, a.InterceptCount);
			Assert.AreEqual(1, b.InterceptCount);
			Assert.AreEqual(10, c.InterceptCount);
		}
	}
}

#endif
