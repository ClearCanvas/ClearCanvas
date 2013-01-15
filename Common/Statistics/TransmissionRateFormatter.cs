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

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Transmission rate formatter class.
    /// </summary>
    public static class TransmissionRateFormatter
    {
        #region Constants

        private const double GIGABYTES = 1024*MEGABYTES;
        private const double KILOBYTES = 1024;
        private const double MEGABYTES = 1024*KILOBYTES;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Formats a transmission rate in appropriate units
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static string Format(double rate)
        {
            if (rate > GIGABYTES)
                return String.Format("{0:0.00} GB/s", rate/GIGABYTES);
            if (rate > MEGABYTES)
                return String.Format("{0:0.00} MB/s", rate/MEGABYTES);
            if (rate > KILOBYTES)
                return String.Format("{0:0.00} KB/s", rate/KILOBYTES);

            return String.Format("{0:0} bytes/s", rate);
        }

        #endregion
    }
}