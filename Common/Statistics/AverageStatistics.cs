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
using System.Collections.Generic;
using System.Diagnostics;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Base average statistics class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AverageStatistics<T> : Statistics<T>, IAverageStatistics
    {
        private readonly List<T> _samples = new List<T>();
        private bool _newSamepleAdded = false;

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageStatistics{T}"/>
        /// </summary>
        public AverageStatistics()
            : base("AverageStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageStatistics{T}"/> with a specified name.
        /// </summary>
        /// <param name="name">Name of the <see cref="AverageStatistics{T}"/> object to be created</param>
        public AverageStatistics(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageStatistics{T}"/> instance for a specified field.
        /// </summary>
        /// <param name="field">The statistics field for which the average statistics is to be created</param>
        public AverageStatistics(Statistics<T> field)
            : base(field)
        {
            string name = Context != null
                              ? String.Format("Average_{0}_{1}", Context.ID, Name)
                              : String.Format("Average_{0}", Name);
            Name = name;
        }

        #endregion

        #region Overridden Public Properties

        /// <summary>
        /// Gets or set the value associated with the average statistics
        /// </summary>
        public override T Value
        {
            get
            {
                ComputeAverage();
                return base.Value;
            }
            set { base.Value = value; }
        }

        /// <summary>
        /// Gets the formatted string representation for the value of the average satistics.
        /// </summary>
        public override string FormattedValue
        {
            get
            {
                ComputeAverage();
                return base.FormattedValue;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the list of samples whose average will be generated.
        /// </summary>
        protected List<T> Samples
        {
            get { return _samples; }
        }

        #endregion

        #region Public Properties

        public int SampleCount
        {
            get { return _samples.Count; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a new sample has been added.
        /// </summary>
        public bool NewSamepleAdded
        {
            get { return _newSamepleAdded; }
            set { _newSamepleAdded = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Computes the average for the samples in <see cref="Samples"/> list.
        /// </summary>
        protected virtual void ComputeAverage()
        {
            if (NewSamepleAdded)
            {
                Debug.Assert(Samples.Count > 0);

                double sum = 0;
                foreach (T sample in Samples)
                {
                    if (sample is byte)
                        sum += (byte) (object) sample;
                    if (sample is short)
                        sum += (short) (object) sample;
                    else if (sample is ushort)
                        sum += (ushort) (object) sample;
                    else if (sample is int)
                        sum += (int) (object) sample;
                    else if (sample is uint)
                        sum += (uint) (object) sample;
                    else if (sample is long)
                        sum += (long) (object) sample;
                    else if (sample is ulong)
                        sum += (ulong) (object) sample;
                    else if (sample is float)
                        sum += (float) (object) sample;
                    else if (sample is double)
                        sum += (double) (object) sample;
                    else
                        throw new Exception(
                            String.Format("ComputeAverage() failed: Unsupported type : {0} ", sample.GetType().Name));
                }


                if (typeof (T) == typeof (byte))
                    Value = (T) (object) (byte) (sum/Samples.Count);
                else if (typeof (T) == typeof (short))
                    Value = (T) (object) (short) (sum/Samples.Count);
                else if (typeof (T) == typeof (ushort))
                    Value = (T) (object) (ushort) (sum/Samples.Count);
                else if (typeof (T) == typeof (int))
                    Value = (T) (object) (int) (sum/Samples.Count);
                else if (typeof (T) == typeof (uint))
                    Value = (T) (object) (uint) (sum/Samples.Count);
                else if (typeof (T) == typeof (long))
                    Value = (T) (object) (long) (sum/Samples.Count);
                else if (typeof (T) == typeof (ulong))
                    Value = (T) (object) (ulong) (sum/Samples.Count);
                else if (typeof (T) == typeof (float))
                    Value = (T) (object) (float) (sum/Samples.Count);
                else if (typeof (T) == typeof (double))
                    Value = (T) (object) (sum/Samples.Count);


                NewSamepleAdded = false;
            }
        }

        #endregion

        #region IAverageStatistics Members

        /// <summary>
        /// Adds a sample to the <see cref="Samples"/> list.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <param name="sample"></param>
        public virtual void AddSample<TSample>(TSample sample)
        {
            if (sample is T)
            {
                Samples.Add((T) (object) sample);
                NewSamepleAdded = true;
            }
        }

        #endregion
    }
}