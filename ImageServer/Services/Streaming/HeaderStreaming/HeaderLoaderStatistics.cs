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

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    internal class HeaderLoaderStatistics : StatisticsSet
    {
        #region Constructors

        public HeaderLoaderStatistics()
            : base("HeaderLoading")
        {
            AddField(new ByteCountStatistics("HeaderSize"));
            AddField(new TimeSpanStatistics("FindStudyFolder"));
            AddField(new TimeSpanStatistics("CompressHeader"));
            AddField(new TimeSpanStatistics("LoadHeaderStream"));
        }

        #endregion Constructors

        #region Public Properties

        public ulong Size
        {
            get { return (this["HeaderSize"] as ByteCountStatistics).Value; }
            set { (this["HeaderSize"] as ByteCountStatistics).Value = value; }
        }

        public TimeSpanStatistics FindStudyFolder
        {
            get { return this["FindStudyFolder"] as TimeSpanStatistics; }
            set { this["FindStudyFolder"] = value; }
        }

        public TimeSpanStatistics LoadHeaderStream
        {
            get { return this["LoadHeaderStream"] as TimeSpanStatistics; }
            set { this["LoadHeaderStream"] = value; }
        }

        public TimeSpanStatistics CompressHeader
        {
            get { return this["CompressHeader"] as TimeSpanStatistics; }
            set { this["CompressHeader"] = value; }
        }

        #endregion Public Properties
    }
}