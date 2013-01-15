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
using ClearCanvas.Common;

namespace ClearCanvas.Workflow {


    /// <summary>
    /// WorkQueueItem entity
    /// </summary>
	public partial class WorkQueueItem
	{
		public WorkQueueItem(string type) : this (
			Platform.Time,
			Platform.Time,
			null, 
			null,
			type,
			WorkQueueStatus.PN,
			null,
			0,
			null,
			new Dictionary<string, string>())
		{
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Marks the item as failed with the specified description, incrementing the failure count.
		/// </summary>
		/// <param name="description"></param>
		public virtual void Fail(string description)
		{
			_failureDescription = description.Length >= 1024 ? description.Substring(0, 1024) : description;
			_failureCount++;
			_status = WorkQueueStatus.F;
			_processedTime = Platform.Time;
		}

		/// <summary>
		/// Marks the item as complete.
		/// </summary>
		public virtual void Complete()
		{
			_status = WorkQueueStatus.CM;
			_processedTime = Platform.Time;
		}

		/// <summary>
		/// Reschedules the item for the specified time.
		/// </summary>
		/// <param name="time"></param>
		public virtual void Reschedule(DateTime time)
		{
			_status = WorkQueueStatus.PN;
			_scheduledTime = time;
		}

		/// <summary>
		/// Reschedules the item for the current time.
		/// </summary>
		public virtual void Reschedule()
		{
			Reschedule(Platform.Time);
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_creationTime = _creationTime.AddMinutes(minutes);
			_scheduledTime = _scheduledTime.AddMinutes(minutes);

			_expirationTime = _expirationTime.HasValue ? _expirationTime.Value.AddMinutes(minutes) : _expirationTime;
			_processedTime = _processedTime.HasValue ? _processedTime.Value.AddMinutes(minutes) : _processedTime;
		}
	}
}