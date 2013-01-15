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

using ClearCanvas.Common.Statistics;

#pragma warning disable 1591

namespace ClearCanvas.Dicom.Utilities.Statistics
{
    /// <summary>
    /// Transmission statistics class.
    /// </summary>
    public class TransmissionStatistics : StatisticsSet
    {
        #region Private members

        private TimeSpanStatistics _timeSpan = new TimeSpanStatistics("TimeSpan");

        #endregion Private members

        #region Constructors

        public TransmissionStatistics(string name)
            : base("Transmission", name)
        {
        }

        #endregion Constructors

        #region Public Properties

        public RateStatistics Speed
        {
            get
            {
                if (this["Speed"] == null)
                {
                    this["Speed"] = new RateStatistics("Speed", RateType.BYTES);
                }
                return this["Speed"] as RateStatistics;
            }
            set
            {
                this["Speed"] = value;
            }
        }

        public RateStatistics MessageRate
        {
            get
            {
                if (this["MessageRate"] == null)
                {
                    this["MessageRate"] = new RateStatistics("MessageRate", RateType.MESSAGES);
                }
                return this["MessageRate"] as RateStatistics;
            }
            set
            {
                this["MessageRate"] = value;
            }
        }

        public ulong IncomingBytes
        {
            get
            {
                if (this["IncomingBytes"] == null)
                {
                    this["IncomingBytes"] = new ByteCountStatistics("IncomingBytes");
                }
                return (this["IncomingBytes"] as ByteCountStatistics).Value;
            }
            set
            {
                this["IncomingBytes"] = new ByteCountStatistics("IncomingBytes", value); 
            }
        }

        public ulong IncomingMessages
        {
            get
            {
                if (this["IncomingMessages"] == null)
                {
                    this["IncomingMessages"] = new MessageCountStatistics("IncomingMessages");
                }
                return (this["IncomingMessages"] as MessageCountStatistics).Value;
            }
            set { this["IncomingMessages"] = new MessageCountStatistics("IncomingMessages", value); }
        }

        public ulong OutgoingBytes
        {
            get
            {
                if (this["OutgoingBytes"] == null)
                {
                    this["OutgoingBytes"] = new ByteCountStatistics("OutgoingBytes");
                }
                return (this["OutgoingBytes"] as ByteCountStatistics).Value;
            }
            set { this["OutgoingBytes"] = new ByteCountStatistics("OutgoingBytes", value); }
        }

        public ulong OutgoingMessages
        {
            get
            {
                if (this["OutgoingMessages"] == null)
                {
                    this["OutgoingMessages"] = new MessageCountStatistics("OutgoingMessages");
                }
                return (this["OutgoingMessages"] as MessageCountStatistics).Value;
            }
            set { this["OutgoingMessages"] = new MessageCountStatistics("OutgoingMessages", value); }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Signals the start of the transmission.
        /// </summary>
        public void Begin()
        {
            MessageRate.Start();
            Speed.Start();
        }

        /// <summary>
        /// Signals the end of the transmission.
        /// </summary>
        public void End()
        {
            if (OutgoingBytes > IncomingBytes)
            {
                Speed.SetData(OutgoingBytes);
                Speed.End();

                MessageRate.SetData(OutgoingMessages);
                MessageRate.End();
            }
            else
            {
                Speed.SetData(IncomingBytes);
                Speed.End();

                MessageRate.SetData(IncomingMessages);
                MessageRate.End();
            }
        }

        #endregion Public Methods
    }
}
