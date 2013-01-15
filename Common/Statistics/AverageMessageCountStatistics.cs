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

using System.Diagnostics;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Average message count statistics.
    /// </summary>
    public class AverageMessageCountStatistics : AverageStatistics<ulong>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageMessageCountStatistics"/>
        /// </summary>
        public AverageMessageCountStatistics()
            : this("AverageMessageCountStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageMessageCountStatistics"/> with a specified name.
        /// </summary>
        /// <param name="name">Name of the <see cref="AverageMessageCountStatistics"/> to be created</param>
        public AverageMessageCountStatistics(string name)
            : base(name)
        {
            Unit = "msg";
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageMessageCountStatistics"/> for a specified <see cref="MessageCountStatistics"/> object
        /// </summary>
        /// <param name="source">The <see cref="MessageCountStatistics"/> for which the <see cref="AverageMessageCountStatistics"/> to be created is based on</param>
        public AverageMessageCountStatistics(MessageCountStatistics source)
            : base(source)
        {
        }

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Adds a sample to the <see cref="AverageStatistics{T}.Samples"/> list.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <param name="sample"></param>
        public override void AddSample<TSample>(TSample sample)
        {
            if (sample is ulong)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is long)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is int)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is uint)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is MessageCountStatistics)
            {
                Samples.Add(((MessageCountStatistics) (object) sample).Value);
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
                foreach (ulong sample in Samples)
                {
                    sum += sample;
                }
                Value = (ulong) (sum/Samples.Count);
                NewSamepleAdded = false;
            }
        }

        #endregion
    }
}