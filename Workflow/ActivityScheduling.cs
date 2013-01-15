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

namespace ClearCanvas.Workflow
{
    public class ActivityScheduling
    {
        private ActivityPerformer _performer;
        private DateTime? _startTime;
        private DateTime? _endTime;

        /// <summary>
        /// No-args constructor required by NHibernate.
        /// </summary>
        public ActivityScheduling()
        {

        }

        public ActivityPerformer Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        public DateTime? StartTime
        {
            get { return _startTime; }
            internal set { _startTime = value; }
        }

        public DateTime? EndTime
        {
            get { return _endTime; }
            internal set { _endTime = value; }
        }

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		public void TimeShift(int minutes)
		{
			_startTime = _startTime.HasValue ? _startTime.Value.AddMinutes(minutes) : _startTime;
			_endTime = _endTime.HasValue ? _endTime.Value.AddMinutes(minutes) : _endTime;
		}
	}
}
