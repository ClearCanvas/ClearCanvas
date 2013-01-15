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

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Provides helper method to generate <see cref="TimeSpanStatistics"/>
    /// </summary>
    public class TimeSpanStatisticsHelper
    {
        /// <summary>
        /// Defines the delegate to a code block whose execution time will be measured using <see cref="Measure"/>.
        /// </summary>
        public delegate void ExecutationBlock();

        /// <summary>
        /// Measures the elapsed time taken by an code block.
        /// </summary>
        /// <param name="executionBlock">Delegate to the code block which will be executed by this method and its elapsed will be measured</param>
        /// <returns>The <see cref="TimeSpanStatistics"/> object contains the elapsed time of the execution</returns>
        static public TimeSpanStatistics Measure(ExecutationBlock executionBlock)
        {
            TimeSpanStatistics stat = new TimeSpanStatistics();

            stat.Start();
            try
            {
                executionBlock();
                return stat;
            }
            finally
            {
                stat.End();
            }
        }
    }
}
