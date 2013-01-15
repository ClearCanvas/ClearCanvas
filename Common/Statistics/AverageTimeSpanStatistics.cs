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
using System.Diagnostics;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Average time span statistics.
    /// </summary>
    public class AverageTimeSpanStatistics : AverageStatistics<TimeSpan>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageTimeSpanStatistics"/>
        /// </summary>
        public AverageTimeSpanStatistics()
            : this("AverageTimeSpanStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageTimeSpanStatistics"/> with specified name.
        /// </summary>
        /// <param name="name"></param>
        public AverageTimeSpanStatistics(string name)
            : base(name)
        {
            Value = new TimeSpan();
            ValueFormatter = TimeSpanFormatter.Format;
        }

        /// <summary>
        /// Creates a copy of the original <see cref="AverageTimeSpanStatistics"/> object.
        /// </summary>
        /// <param name="source"></param>
        public AverageTimeSpanStatistics(TimeSpanStatistics source)
            : base(source)
        {
        }

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Adds a sample to the <see cref="AverageStatistics{T}.Samples"/> list.
        /// </summary>
        /// <typeparam name="TSample">Type of the sample value to be inserted</typeparam>
        /// <param name="sample"></param>
        public override void AddSample<TSample>(TSample sample)
        {
            if (sample is TimeSpan)
            {
                TimeSpan ts = (TimeSpan) (object) sample;
                Samples.Add(new TimeSpan(ts.Ticks));
                NewSamepleAdded = true;
            }
            else if (sample is TimeSpanStatistics)
            {
                TimeSpanStatistics stat = (TimeSpanStatistics) (object) sample;
                Samples.Add(stat.Value);
                NewSamepleAdded = true;
            }
            else
            {
                base.AddSample(sample);
            }
        }

        #endregion

        #region Overridden Protected Methods

        /// <summary>
        /// Computes the average for the samples in <see cref="AverageStatistics{T}.Samples"/> list.
        /// </summary>
        protected override void ComputeAverage()
        {
            if (NewSamepleAdded)
            {
                Debug.Assert(Samples.Count > 0);

                double sum = 0;
                foreach (TimeSpan sample in Samples)
                {
                    sum += sample.Ticks;
                }
                Value = new TimeSpan((long) sum/Samples.Count);
                NewSamepleAdded = false;
            }
        }

        #endregion
    }
}