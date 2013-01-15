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


	/// <summary>
	/// Stores statistics of a WorkQueue instance processing.
	/// </summary>
	internal class CompressInstanceStatistics : StatisticsSet
	{
		#region Constructors

		public CompressInstanceStatistics()
			: this("Instance")
		{ }

		public CompressInstanceStatistics(string name)
			: base(name)
		{ }

		#endregion Constructors

		#region Public Properties

		public TimeSpanStatistics ProcessTime
		{
			get
			{
				if (this["ProcessTime"] == null)
					this["ProcessTime"] = new TimeSpanStatistics("ProcessTime");

				return (this["ProcessTime"] as TimeSpanStatistics);
			}
			set { this["ProcessTime"] = value; }
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

				return ((ByteCountStatistics) this["FileSize"]).Value;
			}
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

		public TimeSpanStatistics CompressTime
		{
			get
			{
				if (this["CompressTime"] == null)
					this["CompressTime"] = new TimeSpanStatistics("CompressTime");

				return (this["CompressTime"] as TimeSpanStatistics);
			}
			set { this["CompressTime"] = value; }
		}
		#endregion Public Properties
	}


}
