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

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class TimeSpanFormat
	{
		/// <summary>
		/// This is a specialized method to Format the timeSpan in to a descriptive text.
		/// The formatted text may be "HH:mm hours", "HH hours" or "mm minutes".   All units below minutes are ignored.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		public static string FormatDescriptive(TimeSpan timeSpan)
		{
			if (timeSpan.Days == 0 && timeSpan.Hours == 0)
				return string.Format("{0} minutes", timeSpan.Minutes);

			var totalHours = Math.Floor(timeSpan.TotalHours);
			return timeSpan.Minutes == 0 
				? string.Format("{0} hours", totalHours) 
				: string.Format("{0}:{1} hours", totalHours, timeSpan.Minutes);
		}
	}
}
