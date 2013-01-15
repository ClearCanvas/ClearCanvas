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

using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Base statistics class that automatically calculates averages 
    /// of the underlying <see cref="StatisticsSetCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the statistics in the collection</typeparam>
    /// <remarks>
    /// The generated statistics contains fields with the average values of the corresponding fields in the collection.
    /// </remarks>
    public class CollectionAverageStatistics<T> : StatisticsSet
        where T : StatisticsSet
    {
        #region Private members

        #endregion Private members

        #region Public properties

        #endregion Public properties

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CollectionAverageStatistics{T}"/> with a specified name.
        /// </summary>
        /// <param name="name"></param>
        public CollectionAverageStatistics(string name)
            : base(name, name)
        {
        }

        #endregion Constructors

        #region Private Methods

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Returns the XML element which contains the average attributes for the child collection.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public override XmlElement GetXmlElement(XmlDocument doc, bool recursive)
        {
            CalculateAverage();
            return base.GetXmlElement(doc, recursive);
        }

        #endregion
    }
}