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
    public class AverageCountStatistics : AverageStatistics<uint>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageCountStatistics"/>
        /// </summary>
        public AverageCountStatistics()
            : this("AverageCountStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageCountStatistics"/> with a specified name.
        /// </summary>
        /// <param name="name">Name of the <see cref="AverageCountStatistics"/> to be created</param>
        public AverageCountStatistics(string name)
            : base(name)
        {
            Unit = "";
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageCountStatistics"/> for a specified <see cref="CountStatistics"/> object
        /// </summary>
        /// <param name="source">The <see cref="CountStatistics"/> for which the <see cref="AverageCountStatistics"/> to be created is based on</param>
        public AverageCountStatistics(CountStatistics source)
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
            if (sample is uint)
            {
                Samples.Add((uint)(object)sample);
                NewSamepleAdded = true;
            }
            if (sample is int)
            {
                Samples.Add((uint)(object)sample);
                NewSamepleAdded = true;
            }
            else if (sample is CountStatistics)
            {
                Samples.Add(((CountStatistics)(object)sample).Value);
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
                foreach (int sample in Samples)
                {
                    sum += sample;
                }
                Value = (uint)(sum / Samples.Count);
                NewSamepleAdded = false;
            }
        }

        #endregion
    }
}