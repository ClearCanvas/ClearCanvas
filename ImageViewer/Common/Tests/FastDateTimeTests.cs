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
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.Tests
{
    [TestFixture]
    public class FastDateTimeTests
    {
        private const int _toleranceMilliseconds = 15;

        private int _tickCount;

        [TestFixtureSetUp]
        public void Initialize()
        {
            FastDateTime.GetTickCount = GetTickCount;
        }

        private int GetTickCount()
        {
            return _tickCount;
        }

        [Test]
        public void TestSimple()
        {
            var realStart = Environment.TickCount;
            const int tickCount = 1000;

            var realNow = DateTime.Now;
            _tickCount = tickCount;
            var fast = new FastDateTime(100);
            var fastNow = fast.Now;
            
            Assert.AreEqual(0, (realNow - fastNow).TotalMilliseconds, _toleranceMilliseconds);
            Assert.LessOrEqual(fast.LastUpdateMilliseconds, 100);
            Assert.GreaterOrEqual(fast.LastUpdateMilliseconds, 0);
            
            Thread.Sleep(80);

            realNow = DateTime.Now;
            _tickCount = tickCount + Environment.TickCount - realStart;
            fastNow = fast.Now;

            Assert.AreEqual(0, (realNow - fastNow).TotalMilliseconds, _toleranceMilliseconds);
            Assert.LessOrEqual(fast.LastUpdateMilliseconds, 100);
            Assert.GreaterOrEqual(fast.LastUpdateMilliseconds, 0);

            Thread.Sleep(25);

            realNow = DateTime.Now;
            _tickCount = tickCount + Environment.TickCount - realStart;
            fastNow = fast.Now;

            Assert.AreEqual(0, (realNow - fastNow).TotalMilliseconds, _toleranceMilliseconds);
            Assert.LessOrEqual(fast.LastUpdateMilliseconds, 100);
            Assert.GreaterOrEqual(fast.LastUpdateMilliseconds, 0);
        }

        [Test]
        public void TestTicksOverflow()
        {
            var realStart = Environment.TickCount;
            const int tickCount = int.MaxValue - 5;

            var realNow = DateTime.Now;
            var fast = new FastDateTime(100);
            _tickCount = tickCount;
            var fastNow = fast.Now;

            Assert.AreEqual(0, (realNow - fastNow).TotalMilliseconds, _toleranceMilliseconds);
            Assert.LessOrEqual(fast.LastUpdateMilliseconds, 100);
            Assert.GreaterOrEqual(fast.LastUpdateMilliseconds, 0);

            Thread.Sleep(80);

            realNow = DateTime.Now;
            unchecked
            {
                _tickCount = tickCount + Environment.TickCount - realStart;
            }
            fastNow = fast.Now;

            Assert.Less(_tickCount, 0);
            Assert.AreEqual(0, (realNow - fastNow).TotalMilliseconds, _toleranceMilliseconds);
            Assert.LessOrEqual(fast.LastUpdateMilliseconds, 100);
            Assert.GreaterOrEqual(fast.LastUpdateMilliseconds, 0);

            Thread.Sleep(25);

            realNow = DateTime.Now;
            unchecked
            {
                _tickCount = tickCount + Environment.TickCount - realStart;
            }
            fastNow = fast.Now;

            Assert.Less(_tickCount, 0);
            Assert.AreEqual(0, (realNow - fastNow).TotalMilliseconds, _toleranceMilliseconds);
            Assert.LessOrEqual(fast.LastUpdateMilliseconds, 100);
            Assert.GreaterOrEqual(fast.LastUpdateMilliseconds, 0);

            Thread.Sleep(25);

            realNow = DateTime.Now;
            unchecked
            {
                _tickCount = tickCount + Environment.TickCount - realStart;
            }
            fastNow = fast.Now;

            Assert.Less(_tickCount, 0);
            Assert.AreEqual(0, (realNow - fastNow).TotalMilliseconds, _toleranceMilliseconds);
            Assert.LessOrEqual(fast.LastUpdateMilliseconds, 100);
            Assert.GreaterOrEqual(fast.LastUpdateMilliseconds, 0);
        }
    }
}

#endif