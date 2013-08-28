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
using MemoryInfo = ClearCanvas.ImageViewer.Common.DefaultMemoryManagementStrategy.MemoryInfo;

namespace ClearCanvas.ImageViewer.Common.Tests
{
    //These aren't so much "unit tests" as they are "scenario tests" and sanity checks, since it can be hard to get your system into these states.

    [TestFixture]
    public class MemoryManagementTests
    {
        private const long OneKilobyte = 1024;
        private const long OneMegabyte = 1024 * OneKilobyte;
        private const long OneGigabyte = 1024 * OneMegabyte;
        
        public void Initialize()
        {
            MemoryManagementSettings.Default.Reset();
            //MemoryManagementSettings.Default.Reload();
        }

        [Test]
        public void Testx86_ExplicitWatermarks()
        {
            var strategy = new DefaultMemoryManagementStrategy {_is32BitProcess = true};

            var memoryInfo = new MemoryInfo();
            memoryInfo.Refresh();
            strategy._memoryInfo = memoryInfo;

            memoryInfo.SystemFreeMemoryBytes = 2 * OneGigabyte;
            memoryInfo.ProcessVirtualMemoryBytes = 600 * OneMegabyte;
            memoryInfo.ProcessPrivateBytes = 500*OneMegabyte;
            memoryInfo.ProcessWorkingSetBytes = 200 * OneMegabyte;

            memoryInfo.HighWaterMarkBytes = 600 * OneMegabyte;
            memoryInfo.LowWaterMarkBytes = 500 * OneMegabyte;

            var high = strategy.GetMemoryHighWatermarkBytes();
            var low = strategy.GetMemoryLowWatermarkBytes(high);

            Assert.AreEqual(600 * OneMegabyte, high);
            Assert.AreEqual(500 * OneMegabyte, low);
        }

        [Test]
        public void Testx86_AutoWatermark()
        {
            var strategy = new DefaultMemoryManagementStrategy { _is32BitProcess = true };

            var memoryInfo = new MemoryInfo();
            memoryInfo.Refresh();
            strategy._memoryInfo = memoryInfo;

            memoryInfo.SystemFreeMemoryBytes = OneGigabyte;
            memoryInfo.ProcessVirtualMemoryBytes = 1400 * OneMegabyte;
            memoryInfo.ProcessPrivateBytes = 1100 * OneMegabyte;
            memoryInfo.ProcessWorkingSetBytes = 500 * OneMegabyte;
            memoryInfo.MemoryManagerLargeObjectBytesCount = OneGigabyte;

            memoryInfo.HighWaterMarkBytes = -1;
            memoryInfo.LowWaterMarkBytes = -1;

            var high = strategy.GetMemoryHighWatermarkBytes();
            var low = strategy.GetMemoryLowWatermarkBytes(high);

            //Forty percent of max possible virtual memory was carefully calculated through experimentation.
            Assert.AreEqual((long)(2*OneGigabyte*0.4), high);
            Assert.AreEqual(high - 0.25 * memoryInfo.MemoryManagerLargeObjectBytesCount, low);
        }

        [Test]
        public void Testx86_AutoWatermark_LowSystemMemory()
        {
            var strategy = new DefaultMemoryManagementStrategy { _is32BitProcess = true };

            var memoryInfo = new MemoryInfo();
            memoryInfo.Refresh();
            strategy._memoryInfo = memoryInfo;

            memoryInfo.SystemFreeMemoryBytes = 200*OneMegabyte;
            memoryInfo.ProcessVirtualMemoryBytes = 1400 * OneMegabyte;
            memoryInfo.ProcessPrivateBytes = 1100 * OneMegabyte;
            memoryInfo.ProcessWorkingSetBytes = 500 * OneMegabyte;
            memoryInfo.MemoryManagerLargeObjectBytesCount = OneGigabyte;

            memoryInfo.HighWaterMarkBytes = -1;
            memoryInfo.LowWaterMarkBytes = -1;

            var high = strategy.GetMemoryHighWatermarkBytes();
            var low = strategy.GetMemoryLowWatermarkBytes(high);

            //Forty percent of max possible virtual memory was carefully calculated through experimentation.
            Assert.AreEqual((long)((memoryInfo.SystemFreeMemoryBytes + memoryInfo.ProcessVirtualMemoryBytes) * 0.4), high);
            Assert.AreEqual(high - 0.25 * memoryInfo.MemoryManagerLargeObjectBytesCount, low);
        }

        [Test]
        public void Testx64_ExplicitWatermarks()
        {
            var strategy = new DefaultMemoryManagementStrategy { _is32BitProcess = false };

            var memoryInfo = new MemoryInfo();
            memoryInfo.Refresh();
            strategy._memoryInfo = memoryInfo;

            memoryInfo.SystemFreeMemoryBytes = 2 * OneGigabyte;
            memoryInfo.ProcessVirtualMemoryBytes = 4 * OneGigabyte;
            memoryInfo.ProcessPrivateBytes = 3 * OneGigabyte;
            memoryInfo.ProcessWorkingSetBytes = 1 * OneGigabyte;

            memoryInfo.HighWaterMarkBytes = 8 * OneGigabyte;
            memoryInfo.LowWaterMarkBytes = 7 * OneGigabyte;

            var high = strategy.GetMemoryHighWatermarkBytes();
            var low = strategy.GetMemoryLowWatermarkBytes(high);

            Assert.AreEqual(8 * OneGigabyte, high);
            Assert.AreEqual(7 * OneGigabyte, low);
        }

        [Test]
        public void Testx64_AutoWatermark_HighFreeMemory()
        {
            var strategy = new DefaultMemoryManagementStrategy { _is32BitProcess = false };

            var memoryInfo = new MemoryInfo();
            memoryInfo.Refresh();
            strategy._memoryInfo = memoryInfo;

            memoryInfo.SystemFreeMemoryBytes = 10*OneGigabyte;
            memoryInfo.ProcessVirtualMemoryBytes = 2 * OneGigabyte;
            memoryInfo.ProcessPrivateBytes = 1500*OneMegabyte; //1.5GB
            memoryInfo.ProcessWorkingSetBytes = 500 * OneMegabyte;
            memoryInfo.MemoryManagerLargeObjectBytesCount = 1200*OneMegabyte;//1.2GB

            memoryInfo.HeldMemoryToCollectPercent = 0.25;
            memoryInfo.x64MinimumFreeSystemMemoryBytes = OneGigabyte;
            memoryInfo.x64MaxMemoryUsagePercent = 0.85;
            memoryInfo.x64MaxMemoryToCollectBytes = OneGigabyte;

            memoryInfo.HighWaterMarkBytes = -1;
            memoryInfo.LowWaterMarkBytes = -1;

            var high = strategy.GetMemoryHighWatermarkBytes();
            var low = strategy.GetMemoryLowWatermarkBytes(high);

            var highcalc = (long)(memoryInfo.x64MaxMemoryUsagePercent * (memoryInfo.SystemFreeMemoryBytes - memoryInfo.x64MinimumFreeSystemMemoryBytes  + memoryInfo.ProcessPrivateBytes));
            var lowcalc =  highcalc - (long)(memoryInfo.HeldMemoryToCollectPercent * memoryInfo.MemoryManagerLargeObjectBytesCount);
            
            Assert.AreEqual(highcalc, high);
            Assert.AreEqual(lowcalc, low);

            //This is a perfectly valid scenario, as a lot of the memory could have been paged out.
            memoryInfo.SystemFreeMemoryBytes = 5 * OneGigabyte;
            memoryInfo.ProcessVirtualMemoryBytes = 10 * OneGigabyte;
            memoryInfo.ProcessPrivateBytes = 9 * OneGigabyte;
            memoryInfo.ProcessWorkingSetBytes = 3 * OneGigabyte;
            memoryInfo.MemoryManagerLargeObjectBytesCount = (long)(8.5*OneGigabyte);

            high = strategy.GetMemoryHighWatermarkBytes();
            low = strategy.GetMemoryLowWatermarkBytes(high);

            highcalc = (long)(memoryInfo.x64MaxMemoryUsagePercent * (memoryInfo.SystemFreeMemoryBytes - memoryInfo.x64MinimumFreeSystemMemoryBytes + memoryInfo.ProcessPrivateBytes));
           
            //Can't exceed "max memory" to collect; avoids long hangups on machines with lots of memory.
            lowcalc = highcalc - memoryInfo.x64MaxMemoryToCollectBytes;

            Assert.AreEqual(highcalc, high);
            Assert.AreEqual(lowcalc, low);
        }
    }
}

#endif