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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	public class ProgressStreamTests
	{
		[Test]
		public void TestReadProgressStream()
		{
			const int streamSize = 26543;
			const int bufferSize = 4096;
			const int readSize = 3000;
			long bytesRead = 0;

			using (var memoryStream = new LargeMemoryStream(new byte[streamSize]))
			{
				using (var progressStream = new ReadProgressStream(memoryStream))
				{
					var eventFired = false;
					progressStream.ProgressChanged += (sender, args) => eventFired = true;

					var buffer = new byte[bufferSize];
					while (bytesRead < memoryStream.Length)
					{
						eventFired = false;
						bytesRead += progressStream.Read(buffer, 0, readSize);
						var progress = 100*bytesRead / (double)streamSize;
						Assert.AreEqual(progress, progressStream.ProgressPercent, 0.01);
						Assert.IsTrue(eventFired);
					}

					Assert.AreEqual(100, progressStream.ProgressPercent, 0.01);
				}
			}
		}

		[Test]
		public void TestWriteProgressStream()
		{
			const int writeSize = 3276;
			const int bytesToWrite = writeSize * 10;
			const int bufferSize = 4096;
			long bytesWritten = 0;

			using (var memoryStream = new LargeMemoryStream())
			{
				using (var progressStream = new WriteProgressStream(memoryStream, bytesToWrite))
				{
					var eventFired = false;
					progressStream.ProgressChanged += (sender, args) => eventFired = true;

					var buffer = new byte[bufferSize];
					while (bytesWritten < bytesToWrite)
					{
						eventFired = false;
						progressStream.Write(buffer, 0, writeSize);
						bytesWritten += writeSize;
						var progress = 100 * bytesWritten / (double)bytesToWrite;
						Assert.AreEqual(progress, progressStream.ProgressPercent, 0.01);
						Assert.IsTrue(eventFired);
					}

					Assert.AreEqual(100, progressStream.ProgressPercent, 0.01);
				}
			}
		}
	}
}

#endif