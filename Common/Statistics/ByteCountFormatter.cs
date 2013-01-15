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
    /// Provide methods to format a number of bytes in different units
    /// </summary>
    public static class ByteCountFormatter
    {
        #region Constants

        private const double GIGABYTES = 1024*MEGABYTES;
        private const double KILOBYTES = 1024;
        private const double MEGABYTES = 1024*KILOBYTES;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Formats a byte number in different units
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Format(ulong bytes)
        {
            if (bytes > GIGABYTES)
                return String.Format("{0:0.00} GB", bytes/GIGABYTES);
            if (bytes > MEGABYTES)
                return String.Format("{0:0.00} MB", bytes/MEGABYTES);
            if (bytes > KILOBYTES)
                return String.Format("{0:0.00} KB", bytes/KILOBYTES);

            return String.Format("{0} bytes", bytes);
        }

        #endregion
    }
}