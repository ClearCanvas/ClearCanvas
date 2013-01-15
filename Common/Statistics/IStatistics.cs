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

#pragma warning disable 1591

using System;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Defines the interface of a statistics object.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IStatistics : ICloneable
    {
        /// <summary>
        /// Sets or gets the context of the statistics
        /// </summary>
        IStatisticsContext Context { set; get; }

        /// <summary>
        /// Gets the name of the statistics
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the unit of the statistics value.
        /// </summary>
        string Unit { get; set; }


        /// <summary>
        /// Gets the formatted string representation of the value.
        /// </summary>
        string FormattedValue { get; }

        /// <summary>
        /// Gets the XML attribute representation of the statistics.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        XmlAttribute[] GetXmlAttributes(XmlDocument doc);

        /// <summary>
        /// Gets a new statistics instance that can be used to generate and store the average of current statistics 
        /// </summary>
        /// <returns></returns>
        IAverageStatistics NewAverageStatistics();
    }
}