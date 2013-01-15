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
using System.Collections.Generic;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Generic base statistics class that implements <see cref="IStatistics"/>
    /// </summary>
    /// <typeparam name="T">The underlying data type of the statistics</typeparam>
    /// 
    /// 
    public class Statistics<T> : IStatistics
    {
        #region Delegates

        /// <summary>
        /// Defines the delegate used for formatting the value of the statistics.
        /// <seealso cref="FormattedValue"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate string ValueFormatterDelegate(T value);

        #endregion

        #region Private members

        private IStatisticsContext _context;
        private ValueFormatterDelegate _formatter;
        private string _name;
        private string _unit;
        private T _value;

        #endregion Private members

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Statistics"/> with specified name.
        /// </summary>
        /// <param name="name">Name of the <see cref="Statistics"/> object to be created</param>
        public Statistics(string name)
        {
            _name = name;
            _formatter = null;
        }

        /// <summary>
        /// Creates an instance of <see cref="Statistics"/> with specified name and value.
        /// </summary>
        /// <param name="name">Name of the <see cref="Statistics"/> object to be created</param>
        /// <param name="value">value to be assigned to the newly crated <see cref="Statistics"/> object</param>
        public Statistics(string name, T value)
            : this(name)
        {
            _value = value;
            _formatter = null;
        }


        /// <summary>
        /// Creates an separate copy instance of <see cref="Statistics"/> based on a specified <see cref="Statistics"/> object.
        /// </summary>
        /// <param name="source">The original <see cref="Statistics"/></param>
        /// <remarks>
        /// </remarks>
        public Statistics(Statistics<T> source)
            : this(source.Name)
        {
            _value = source.Value;
            ValueFormatter = source.ValueFormatter;
            Context = source.Context;
        }

        #endregion Constructors

        #region Protected methods

        protected string FormatValue(object obj)
        {
            if (ValueFormatter != null && obj is T)
                return ValueFormatter((T) obj);

            if (_unit != null)
                return String.Format("{0} {1}", obj, _unit);
            else
                return obj.ToString();
        }

        #endregion Protected methods

        #region Casting operators

        /// <summary>
        /// Casts the statistics to the underlying type.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static explicit operator T(Statistics<T> stat)
        {
            return stat.Value;
        }

        #endregion Casting operators

        #region Public properties

        /// <summary>
        /// Gets or sets the value associated with the statistics
        /// </summary>
        public virtual T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ValueFormatterDelegate ValueFormatter
        {
            get { return _formatter; }
            set { _formatter = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public virtual string FormattedValue
        {
            get { return FormatValue(Value); }
        }

        public IStatisticsContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        #endregion Public properties

        #region Public Methods

        public XmlAttribute[] GetXmlAttributes(XmlDocument doc)
        {
            List<XmlAttribute> list = new List<XmlAttribute>();

            XmlAttribute attr = doc.CreateAttribute(Name);
            if (_value != null)
                attr.Value = FormattedValue;

            list.Add(attr);
            return list.ToArray();
        }

        #endregion

        #region IStatistics Members

        public virtual object Clone()
        {
            Statistics<T> copy = new Statistics<T>(this);
            return copy;
        }


        public virtual IAverageStatistics NewAverageStatistics()
        {
            return new AverageStatistics<T>(this);
        }

        #endregion
    }
}