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
using System.Text;
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    internal class FilesystemReprocessStatistics : StatisticsSet
    {
        #region Public Properties

        public string Filesystem
        {
            set { this["Filesystem"] = new Statistics<string>("Filesystem", value); }
            get { return (this["Filesystem"] as Statistics<string>).Value; }
        }

        public RateStatistics StudyRate
        {
            get { return this["StudyRate"] as RateStatistics; }
        }


        public int NumStudies
        {
            set { (this["NumStudies"] as Statistics<int>).Value = value; }
            get { return (this["NumStudies"] as Statistics<int>).Value; }
        }

        #endregion Public Properties

        #region Constructors

        public FilesystemReprocessStatistics()
            : base("FilesystemReprocess")
        {
            AddField(new Statistics<string>("Filesystem"));
            AddField(new Statistics<int>("NumStudies"));
            AddField(new RateStatistics("StudyRate", "studies"));
        }

        #endregion Constructors
    }
}
