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
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Common
{
    internal class FastDateTime
    {
        private bool _initialized;
        private DateTime _now;
        private int _lastUpdateTicks;
        private readonly int _updateMilliseconds;

        internal static Func<int> GetTickCount = DefaultGetTickCount;

        public FastDateTime(int updateMilliseconds = 2000)
        {
            //Has to be worth doing.
            _updateMilliseconds = Math.Max(10, updateMilliseconds);
        }

        private static int TickCount
        {
            get { return GetTickCount(); }
        }

        internal int LastUpdateMilliseconds
        {
            get { return TickCount - _lastUpdateTicks; }
        }

        public DateTime Now
        {
            get
            {
                if (!_initialized)
                {
                    _now = DateTime.Now;
                    _lastUpdateTicks = TickCount;
                    _initialized = true;
                    //Trace.WriteLine("Initialized FastDateTime");
                    return _now;
                }

                if (LastUpdateMilliseconds >= _updateMilliseconds)
                {
                    _now = DateTime.Now;
                    _lastUpdateTicks = TickCount;
                    //Trace.WriteLine(String.Format("FastDateTime - [{0:HH:mm:ss.ffff}]", _now));
                    return _now;
                }

                var now = _now.AddMilliseconds(LastUpdateMilliseconds);
                //Trace.WriteLine(String.Format("FastDateTime - [{0:HH:mm:ss.ffff}]+{1}=[{2:HH:mm:ss.ffff}]", _now, LastUpdateMilliseconds/1000F, now));
                return now;
            }
        }

        private static int DefaultGetTickCount()
        {
            return Environment.TickCount;
        }
    }
}