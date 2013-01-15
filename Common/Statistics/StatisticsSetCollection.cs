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

using System.Collections.Generic;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Base collection of <see cref="StatisticsSet"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StatisticsSetCollection<T>
        where T : StatisticsSet, new()
    {
        #region Private members

        private List<T> _list = new List<T>();

        #endregion Private members

        #region public properties

        public List<T> Items
        {
            get { return _list; }
            set { _list = value; }
        }

        public int Count
        {
            get { return Items.Count; }
        }

        #endregion public properties

        #region public methods

        ///// <summary>
        ///// Returns a new instance of the underlying statistics set.
        ///// </summary>
        ///// <param name="name">Name to be assigned to the statistics set.</param>
        ///// <returns></returns>
        //public T NewStatistics(string name)
        //{
        //    T newStat = new T();
        //    newStat.Name = name;
        //    _list.Add(newStat);
        //    return newStat;
        //}

        /// <summary>
        /// Returns the statistics collection as a list of XML elements.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual List<XmlElement> ToXmlElements(XmlDocument doc, bool recursive)
        {
            List<XmlElement> list = new List<XmlElement>();

            foreach (StatisticsSet item in Items)
            {
                XmlElement xml = item.GetXmlElement(doc, recursive);
                list.Add(xml);
            }

            return list;
        }

        #endregion public methods
    }
}