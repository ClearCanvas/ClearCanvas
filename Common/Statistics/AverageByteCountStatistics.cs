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
    /// Average statistics class based on samples of <see cref="ByteCountStatistics"/>.
    /// </summary>
    public class AverageByteCountStatistics : AverageStatistics<ulong>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageByteCountStatistics"/>
        /// </summary>
        public AverageByteCountStatistics()
            : this("AverageByteCountStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageByteCountStatistics"/> with a specific name.
        /// </summary>
        /// <param name="name">Name of the <see cref="AverageByteCountStatistics"/> instance object to be created</param>
        public AverageByteCountStatistics(string name)
            : base(name)
        {
            ValueFormatter = ByteCountFormatter.Format;
        }


        /// <summary>
        /// Creates an copy instance of <see cref="AverageByteCountStatistics"/> based on another <see cref="AverageByteCountStatistics"/> instance.
        /// </summary>
        /// <param name="source">The original <see cref="AverageByteCountStatistics"/> object</param>
        public AverageByteCountStatistics(ByteCountStatistics source)
            : base(source)
        {
            Debug.Assert(Value == source.Value);
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
            if (sample is ulong)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is long)
            {
                Samples.Add( (ulong) (long) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is int)
            {
                Samples.Add((ulong) (int) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is uint)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is ByteCountStatistics)
            {
                Samples.Add(((ByteCountStatistics) (object) sample).Value);
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
        /// Computes the average for the samples in <see cref="AverageStatistics{T}"/> list.
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