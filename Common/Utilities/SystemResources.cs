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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Memory and storage size units
	/// </summary>
	public enum SizeUnits
	{
		/// <summary>
		/// Bytes
		/// </summary>
		Bytes,
		/// <summary>
		/// Kilobytes
		/// </summary>
		Kilobytes,
		/// <summary>
		/// Megabytes
		/// </summary>
		Megabytes,
		/// <summary>
		/// Gigabytes
		/// </summary>
		Gigabytes
	}

	/// <summary>
	/// Provides convenience methods for querying system resources.
	/// </summary>
	public static class SystemResources
	{
		private static volatile PerformanceCounter _memoryPerformanceCounter;
		private static readonly object _syncRoot = new object();

		private static PerformanceCounter MemoryPerformanceCounter
		{
			get
			{
				if (_memoryPerformanceCounter == null)
				{
					lock (_syncRoot)
					{
						if (_memoryPerformanceCounter == null)
							_memoryPerformanceCounter = new PerformanceCounter("Memory", "Available Bytes");
					}
				}

				return _memoryPerformanceCounter;
			}
		}

		/// <summary>
		/// Gets the available physical memory.
		/// </summary>
		/// <param name="units"></param>
		/// <returns></returns>
		public static long GetAvailableMemory(SizeUnits units)
		{
			long availableBytes = Convert.ToInt64(MemoryPerformanceCounter.NextValue());

			if (units == SizeUnits.Bytes)
				return availableBytes;
			else if (units == SizeUnits.Kilobytes)
				return availableBytes / 1024;
			else if (units == SizeUnits.Megabytes)
				return availableBytes / 1048576;
			else
				return availableBytes / 1073741824;
		}


        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out long lpFreeBytesAvailableToCaller,
                                            out long lpTotalNumberOfBytes, out long lpTotalNumberOfFreeBytes);

        public static DriveInformation GetDriveInformation(string path)
	    {
            long available, total, free;
            bool result = GetDiskFreeSpaceEx(path, out available, out total, out free);

            if (result)
            {
                return new DriveInformation()
                           {
                               RootDirectory = System.IO.Path.GetPathRoot(path),
                               Total = total,
                               Free = free
                           };
            }

            throw new Win32Exception(string.Format("Unable to get drive information {0}. Error code: {1}", path, 0));
	    }
	}

    public class DriveInformation
    {
        public string RootDirectory { get; set; }

        public long Total { get; set; }

        public long Free { get; set; }
    }
}
