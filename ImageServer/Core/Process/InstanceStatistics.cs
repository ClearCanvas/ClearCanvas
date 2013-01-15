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

namespace ClearCanvas.ImageServer.Core.Process
{
	/// <summary>
	/// Stores statistics of a WorkQueue instance processing.
	/// </summary>
	public class InstanceStatistics : StatisticsSet
	{
		#region Constructors

		public InstanceStatistics()
			: this("Instance")
		{
		}

		public InstanceStatistics(string name)
			: base(name)
		{

		}

		#endregion Constructors

		#region Public Properties

		public TimeSpanStatistics ProcessTime
		{
			get
			{
				if (this["ProcessTime"]==null)
					this["ProcessTime"] = new TimeSpanStatistics("ProcessTime");

				return (this["ProcessTime"] as TimeSpanStatistics);
			}
			set { this["ProcessTime"] = value; }
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

		public ulong FileSize
		{
			set
			{
				this["FileSize"] = new ByteCountStatistics("FileSize", value);
			}
			get
			{
				if (this["FileSize"] == null)
					this["FileSize"] = new ByteCountStatistics("FileSize");

				return (this["FileSize"] as ByteCountStatistics).Value;
			}
		}

		public TimeSpanStatistics SopRulesLoadTime
		{
			get
			{
				if (this["SopRulesLoadTime"] == null)
					this["SopRulesLoadTime"] = new TimeSpanStatistics("SopRulesLoadTime");

				return (this["SopRulesLoadTime"] as TimeSpanStatistics);
			}
			set { this["SopRulesLoadTime"] = value; }
		}

		public TimeSpanStatistics SeriesRulesLoadTime
		{
			get
			{
				if (this["SeriesRulesLoadTime"] == null)
					this["SeriesRulesLoadTime"] = new TimeSpanStatistics("SeriesRulesLoadTime");

				return (this["SeriesRulesLoadTime"] as TimeSpanStatistics);
			}
			set { this["SeriesRulesLoadTime"] = value; }
		}


		public TimeSpanStatistics StudyRulesLoadTime
		{
			get
			{
				if (this["StudyRulesLoadTime"] == null)
					this["StudyRulesLoadTime"] = new TimeSpanStatistics("StudyRulesLoadTime");

				return (this["StudyRulesLoadTime"] as TimeSpanStatistics);
			}
			set { this["StudyRulesLoadTime"] = value; }
		}


		public TimeSpanStatistics SopEngineExecutionTime
		{
			get
			{
				if (this["SopEngineExecutionTime"] == null)
					this["SopEngineExecutionTime"] = new TimeSpanStatistics("SopEngineExecutionTime");

				return (this["SopEngineExecutionTime"] as TimeSpanStatistics);
			}
			set { this["SopEngineExecutionTime"] = value; }
		}

		public TimeSpanStatistics SeriesEngineExecutionTime
		{
			get
			{
				if (this["SeriesEngineExecutionTime"] == null)
					this["SeriesEngineExecutionTime"] = new TimeSpanStatistics("SeriesEngineExecutionTime");

				return (this["SeriesEngineExecutionTime"] as TimeSpanStatistics);
			}
			set { this["SeriesEngineExecutionTime"] = value; }
		}

		public TimeSpanStatistics StudyEngineExecutionTime
		{
			get
			{
				if (this["StudyEngineExecutionTime"] == null)
					this["StudyEngineExecutionTime"] = new TimeSpanStatistics("StudyEngineExecutionTime");

				return (this["StudyEngineExecutionTime"] as TimeSpanStatistics);
			}
			set { this["StudyEngineExecutionTime"] = value; }
		}

		public TimeSpanStatistics InsertStreamTime
		{
			get
			{
				if (this["InsertStreamTime"] == null)
					this["InsertStreamTime"] = new TimeSpanStatistics("InsertStreamTime");

				return (this["InsertStreamTime"] as TimeSpanStatistics);
			}
			set { this["InsertStreamTime"] = value; }
		}

		public TimeSpanStatistics InsertDBTime
		{
			get
			{
				if (this["InsertDBTime"] == null)
					this["InsertDBTime"] = new TimeSpanStatistics("InsertDBTime");

				return (this["InsertDBTime"] as TimeSpanStatistics);
			}
			set { this["InsertDBTime"] = value; }
		}

		public TimeSpanStatistics FileLoadTime
		{
			get
			{
				if (this["FileLoadTime"] == null)
					this["FileLoadTime"] = new TimeSpanStatistics("FileLoadTime");

				return (this["FileLoadTime"] as TimeSpanStatistics);
			}
			set { this["FileLoadTime"] = value; }
		}

		#endregion Public Properties
	}
}