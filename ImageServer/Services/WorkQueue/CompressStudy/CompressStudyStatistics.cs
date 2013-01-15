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

namespace ClearCanvas.ImageServer.Services.WorkQueue.CompressStudy
{
	class CompressStudyStatistics : StatisticsSet
	{
		#region Public Properties

        public string StudyInstanceUid
        {
            set
            {
                this["StudyInstanceUid"] = new Statistics<string>("StudyInstanceUid", value);
            }
            get
            {
                if (this["StudyInstanceUid"] == null)
                    this["StudyInstanceUid"] = new Statistics<string>("StudyInstanceUid");

                return ((Statistics<string>) this["StudyInstanceUid"]).Value;
            }
        }

        public string Modality
        {
            set { this["Modality"] = new Statistics<string>("Modality", value); }
            get
            {
                if (this["Modality"] == null)
                    this["Modality"] = new Statistics<string>("Modality");

                return ((Statistics<string>) this["Modality"]).Value;
            }
        }


        public int NumInstances
        {
            set { this["NumInstances"] = new Statistics<int>("NumInstances", value); }
            get
            {
                if (this["NumInstances"] == null)
                    this["NumInstances"] = new Statistics<int>("Modality");

                return ((Statistics<int>) this["NumInstances"]).Value;
            }
        }

        public TimeSpanStatistics TotalProcessTime
        {
            get
            {
                if (this["TotalProcessTime"] == null)
                    this["TotalProcessTime"] = new TimeSpanStatistics("TotalProcessTime");

                return (this["TotalProcessTime"] as TimeSpanStatistics);
            }
            set { this["TotalProcessTime"] = value; }
        }

        public TimeSpanStatistics DBUpdateTime
        {
            get
            {
                if (this["DBUpdateTime"] == null)
                    this["DBUpdateTime"] = new TimeSpanStatistics("DBUpdateTime");

                return (this["DBUpdateTime"] as TimeSpanStatistics);
            }
            set { this["DBUpdateTime"] = value; }
        }

        public TimeSpanStatistics StudyXmlLoadTime
        {
            get
            {
                if (this["StudyXmlLoadTime"] == null)
                    this["StudyXmlLoadTime"] = new TimeSpanStatistics("StudyXmlLoadTime");

                return (this["StudyXmlLoadTime"] as TimeSpanStatistics);
            }
            set { this["StudyXmlLoadTime"] = value; }
        }

		public TimeSpanStatistics StorageLocationLoadTime
		{
			get
			{
				if (this["StorageLocationLoadTime"] == null)
					this["StorageLocationLoadTime"] = new TimeSpanStatistics("StorageLocationLoadTime");

				return (this["StorageLocationLoadTime"] as TimeSpanStatistics);
			}
			set { this["StorageLocationLoadTime"] = value; }
		}

		public TimeSpanStatistics UidsLoadTime
		{
			get
			{
				if (this["UidsLoadTime"] == null)
					this["UidsLoadTime"] = new TimeSpanStatistics("UidsLoadTime");

				return (this["UidsLoadTime"] as TimeSpanStatistics);
			}
			set { this["UidsLoadTime"] = value; }
		}

        #endregion Public Properties

        #region Constructors

        public CompressStudyStatistics() : this("CompressStudy")
        {
        }


		public CompressStudyStatistics(string name)
            : base(name)
        {
        }

        #endregion Constructors
	}
}
