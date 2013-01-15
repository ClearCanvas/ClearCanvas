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

namespace HeaderStressTest
{
    class InstanceInfo
    {
        private string _SopUid;

        public string SopUid
        {
            get { return _SopUid; }
            set { _SopUid = value; }
        }
    }

    class SeriesInfo
    {
        private string _SeriesUid;
        private List<InstanceInfo> _instances = new List<InstanceInfo>();
        public string SeriesUid
        {
            get { return _SeriesUid; }
            set { _SeriesUid = value; }
        }

        public List<InstanceInfo> Instances
        {
            get { return _instances; }
            set { _instances = value; }
        }
    }

    class StudyInfo
    {
        private string _StudyUid;
        private List<SeriesInfo> _series = new List<SeriesInfo>();
        public string StudyUid
        {
            get { return _StudyUid; }
            set { _StudyUid = value; }
        }

        public List<SeriesInfo> Series
        {
            get { return _series; }
            set { _series = value; }
        }
    }
}
