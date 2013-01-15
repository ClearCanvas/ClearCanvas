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
    /// Statistics to store the number of bytes
    /// </summary>
    /// <remarks>
    /// <see cref="IStatistics.FormattedValue"/> of the <see cref="ByteCountStatistics"/> has unit of "GB", 'MB" or "KB"
    /// depending on the number of bytes being set.
    /// </remarks>
    public class ByteCountStatistics : Statistics<ulong>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ByteCountStatistics"/>
        /// </summary>
        /// <param name="name"></param>
        public ByteCountStatistics(string name)
            : this(name, 0)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="ByteCountStatistics"/> with specified name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ByteCountStatistics(string name, ulong value)
            : base(name, value)
        {
            ValueFormatter = ByteCountFormatter.Format;
        }

        /// <summary>
        /// Creates a copy of the original <see cref="ByteCountStatistics"/> object.
        /// </summary>
        /// <param name="source">The original <see cref="ByteCountStatistics"/> to copy</param>
        public ByteCountStatistics(ByteCountStatistics source)
            : base(source)
        {
        }

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Creates a copy of the current statistics
        /// </summary>
        /// <returns>A copy of the current <see cref="RateStatistics"/> object</returns>
        public override object Clone()
        {
            return new ByteCountStatistics(this);
        }

        /// <summary>
        /// Returns a new average statistics object corresponding to the current statistics
        /// </summary>
        /// <returns>A <see cref="AverageRateStatistics"/> object</returns>
        public override IAverageStatistics NewAverageStatistics()
        {
            return new AverageByteCountStatistics(this);
        }

        #endregion
    }
}