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
using System.Diagnostics;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Statistics to hold one of more <see cref="IStatistics"/>.
    /// </summary>
    public class StatisticsSet : ICloneable
    {
        private IStatisticsContext _context;
        private string _description;

        // list of sub-statistics
        protected Dictionary<object, IStatistics> _fields = new Dictionary<object, IStatistics>();
        protected String _name;

        private List<StatisticsSet> _subStatistics = new List<StatisticsSet>();

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the statistics set.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the statistics fields in the set.
        /// </summary>
        public ICollection<IStatistics> Fields
        {
            get { return _fields.Values; }
        }

        public List<StatisticsSet> SubStatistics
        {
            get { return _subStatistics; }
            set { _subStatistics = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public IStatisticsContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        /// <summary>
        /// Gets or sets the statistics field in the set based on a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IStatistics this[object key]
        {
            get
            {
                if (!_fields.ContainsKey(key))
                {
                    return null;
                }
                return _fields[key];
            }
            set
            {
                _fields[key] = value;
                value.Context = Context;
            }
        }

        #endregion

        #region Constructors

        public StatisticsSet(string name, string description)
        {
            Name = name;
            Description = description;
            Context = new StatisticsContext(name);
        }

        public StatisticsSet(string name)
            : this(name, name)
        {
        }

        public StatisticsSet() : this(String.Empty, String.Empty)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a specified statistics into the set using its name as the key.
        /// </summary>
        /// <param name="stat"></param>
        public void AddField(IStatistics stat)
        {
            object key = StatisticsHelper.ResolveID(stat);
            _fields[key] = stat;

            stat.Context = Context;
        }


        /// <summary>
        /// Adds a specified statistics into the set using its name as the key.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddField(string name, string value)
        {
            Statistics<string> newField = new Statistics<string>(name);
            newField.Value = value;
            AddField(newField);
        }


        /// <summary>
        /// Adds a sub-statistics.
        /// </summary>
        /// <param name="stat"></param>
        public void AddSubStats(StatisticsSet stat)
        {
            Debug.Assert(stat.Context != null);
            Platform.CheckForNullReference(stat, "stat");
            _subStatistics.Add(stat);
        }

        /// <summary>
        /// Creats and calculates the averages for applicable fields in the sub-statistics
        /// </summary>
        public void CalculateAverage()
        {
            foreach (StatisticsSet substat in SubStatistics)
            {
                ComputeAverage(substat);
            }
        }


        /// <summary>
        /// Gets the XML representation of the statistics set.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual XmlElement GetXmlElement(XmlDocument doc, bool recursive)
        {
            XmlElement el = doc.CreateElement("Statistics");

            if (GetType() != typeof (StatisticsSet))
            {
                XmlAttribute attrType = doc.CreateAttribute("Type");
                attrType.Value = GetType().Name;
                el.Attributes.Append(attrType);
            }

            if (Context != null)
            {
                XmlAttribute attrContext = doc.CreateAttribute("Context");
                attrContext.Value = Context.ID;
                el.Attributes.Append(attrContext);
            }
            else
            {
                XmlAttribute attrName = doc.CreateAttribute("Name");
                attrName.Value = Name;
                el.Attributes.Append(attrName);
            }


            XmlAttribute attrDescription = doc.CreateAttribute("Description");
            attrDescription.Value = Description;
            el.Attributes.Append(attrDescription);

            foreach (IStatistics field in Fields)
            {
                XmlAttribute[] attrValues = field.GetXmlAttributes(doc);
                foreach (XmlAttribute a in attrValues)
                {
                    el.Attributes.Append(a);
                }
            }

            if (recursive)
            {
                foreach (StatisticsSet substat in SubStatistics)
                {
                    el.AppendChild(substat.GetXmlElement(doc, recursive));
                }
            }


            return el;
        }

        #endregion

        #region Private Methods

        protected virtual void ComputeAverage(StatisticsSet statistics)
        {
            foreach (IStatistics field in statistics.Fields)
            {
                IAverageStatistics average = field.NewAverageStatistics();

                if (average == null)
                    continue;

                object key = StatisticsHelper.ResolveID(average);
                if (this[key] != null)
                {
                    average = this[key] as IAverageStatistics;
                    Debug.Assert(average != null);
                }
                else
                {
                    AddField(average);
                }

                if (field is Statistics<int>)
                {
                    average.AddSample(((Statistics<int>) field).Value);
                }
                else if (field is Statistics<uint>)
                {
                    average.AddSample(((Statistics<uint>) field).Value);
                }
                else if (field is Statistics<long>)
                {
                    //sum += ((Statistics<long>)field).Value;
                    average.AddSample(((Statistics<long>) field).Value);
                }
                else if (field is Statistics<ulong>)
                {
                    average.AddSample(((Statistics<ulong>) field).Value);
                }
                else if (field is Statistics<double>)
                {
                    average.AddSample(((Statistics<double>) field).Value);
                }
                else if (field is TimeSpanStatistics)
                {
                    TimeSpanStatistics stat = field as TimeSpanStatistics;
                    if (stat.IsSet)
                        average.AddSample(((TimeSpanStatistics) field).Value);
                }
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            // create an instance of this specific type instead of StatisticsSet
            object newObject = Activator.CreateInstance(GetType());

            // copy the basic stuff
            StatisticsSet copy = newObject as StatisticsSet;
            copy.Name = Name;
            copy.Description = Description;
            copy.Context = Context;

            // perform deep copy
            foreach (IStatistics field in Fields)
            {
                copy.AddField(field.Clone() as IStatistics);
            }

            foreach (StatisticsSet substat in SubStatistics)
            {
                copy.AddSubStats(substat.Clone() as StatisticsSet);
            }

            return copy;
        }

        #endregion
    }
}