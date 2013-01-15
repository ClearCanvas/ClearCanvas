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

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Represents the statistics generates by <see cref="WADORequestProcessor"/>
    /// </summary>
    public class WADORequestProcessorStatistics : StatisticsSet
    {
        #region Constructors
        public WADORequestProcessorStatistics(string name)
            : base(name)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Total process time.
        /// </summary>
        public TimeSpanStatistics TotalProcessTime
        {
            get
            {
                if (this["TotalProcessTime"] == null)
                    this["TotalProcessTime"] = new TimeSpanStatistics("TotalProcessTime");

                return (this["TotalProcessTime"] as TimeSpanStatistics);
            }
            set { this["TotalProcessTime"] = value; }
        }

        /// <summary>
        /// Transmission speed.
        /// </summary>
        public RateStatistics TransmissionSpeed
        {
            get
            {
                if (this["TransmissionSpeed"] == null)
                    this["TransmissionSpeed"] = new RateStatistics("TransmissionSpeed", RateType.BYTES);

                return (this["TransmissionSpeed"] as RateStatistics);
            }
            set { this["TransmissionSpeed"] = value; }
        }
        #endregion
    }
    
}
