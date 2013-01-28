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
    /// The type of information used in <see cref="RateStatistics"/>.
    /// </summary>
    public enum RateType
    {
        ///<summary>
        /// Rate statistics in number of bytes within a period
        ///</summary>
        BYTES, 
        /// <summary>
        /// Rate statistics in number of message within a period
        /// </summary>
        MESSAGES,

		/// <summary>
		/// Custom rate statistics.
		/// </summary>
        CUSTOM
    } ;

    /// <summary>
    /// Statistics class to store the rate of changes of the underlying information.
    /// </summary>
    /// <remarks>
    /// The information being supported include: Byte rates or Message rate. The number of bytes or messages is set by calling <see cref="SetData"/>.
    /// The rate will be calculated based on the value set by calling <see cref="SetData"/> in between <see cref="Start"/> and <see cref="End"/> calls.
    /// 
    /// <see cref="IStatistics.FormattedValue"/> of the <see cref="RateStatistics"/> has unit of "GB/s", "MB/s", "KB/s" or "msg/s"
    /// depending on type of value specified in the constructor.
    /// 
    /// 
    /// <example>
    /// <code>
    ///     RateStatistics transferSpeed = new RateStatistics("Speed", RateType.BYTES);
    ///     transferSpeed.Begin();
    ///     transferSpeed.SetData(2408);
    ///     transferSpeed.End();    
    /// </code>
    /// 
    /// <para>If the time elapsed between Begin() and End() is one second, then transferSpeed.FormattedValue = "2 KB/s"</para>
    /// <para>If the time elapsed is 5 seconds, then transferSpeed.FormattedValue = "0.4 KB/s"</para>
    /// 
    /// </example>
    /// 
    /// 
    /// 
    /// </remarks>
    public class RateStatistics : Statistics<double>
    {

        #region private members

        private double _data = 0;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private RateType _type;

        #endregion private members

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="RateStatistics"/> for the specified <see cref="RateType"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rateType"></param>
        public RateStatistics(string name, RateType rateType)
            : base(name)
        {
            Type = rateType;

            switch (Type)
            {
                case RateType.BYTES:
                    ValueFormatter = TransmissionRateFormatter.Format;
                    break;

                case RateType.MESSAGES:
                    ValueFormatter = MessageRateFormatter.Format;
                    break;
            }
        }

		/// <summary>
		///Creates an instance of <see cref="RateStatistics"/> for type <see cref="RateType.CUSTOM"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="unit"></param>
        public RateStatistics(string name, string unit)
            : base(name)
        {
            Type = RateType.CUSTOM;

            ValueFormatter = delegate(double rate)
                                 {
                                     return String.Format("{0:0.0} {1}/s", rate, unit);
                                 };
        }

        /// <summary>
        /// Creates a copy of the original <see cref="RateStatistics"/> object.
        /// </summary>
        /// <param name="source">The original <see cref="RateStatistics"/> to copy </param>
        public RateStatistics(RateStatistics source)
            : base(source)
        {
            Type = source.Type;
        }

        /// <summary>
        /// Gets or sets the type of the rate being measured
        /// </summary>
        public RateType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        #endregion Constructors
		/// <summary>
		/// The rate value.
		/// </summary>
        public override double Value
        {
            get
            {
                if (_stopWatch.ElapsedTicks > 0)
                    return _data / ((double)_stopWatch.ElapsedTicks / Stopwatch.Frequency);
                else
                    return 0;
            }
            set
            {
                base.Value = value;
            }
        }

        #region Public methods

        /// <summary>
        /// Gets the elapsed time being measured, in ticks.
        /// </summary>
        public long ElapsedTicks
        {
            get { return _stopWatch.ElapsedTicks; }
        }

        /// <summary>
        /// Gets the elapsed time being measured, in ticks.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return _stopWatch.Elapsed; }
        }


        /// <summary>
        /// Sets the value of the underlying data.
        /// </summary>
        /// <param name="value"></param>
        public void SetData(double value)
        {
            _data = value;
        }

        /// <summary>
        /// Signals the begining of the measurement.
        /// </summary>
        public void Start()
        {
            _stopWatch.Start();
        }

        /// <summary>
        /// Signals the end of the measurement.
        /// </summary>
        public void End()
        {
            Debug.Assert(_stopWatch.IsRunning);

            _stopWatch.Stop();
            //Debug.Assert(_stopWatch.ElapsedTicks > 0);
        }

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Creates a copy of the current statistics
        /// </summary>
        /// <returns>A copy of the current <see cref="RateStatistics"/> object</returns>
        public override object Clone()
        {
            return new RateStatistics(this);
        }

        /// <summary>
        /// Returns a new average statistics object corresponding to the current statistics
        /// </summary>
        /// <returns>A <see cref="AverageRateStatistics"/> object</returns>
        public override IAverageStatistics NewAverageStatistics()
        {
            return new AverageRateStatistics(this);
        }

        #endregion
    }
}