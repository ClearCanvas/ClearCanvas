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


namespace ClearCanvas.Healthcare
{
	public enum DateTimePrecision
	{
		Day,
		Hour,
		Minute,
		Second
	}

	public static class DateTimeExtensions
	{
		/// <summary>
		/// Returns a DateTime truncated to the specified precision.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="precision"> </param>
		/// <returns></returns>
		public static DateTime Truncate(this DateTime t, DateTimePrecision precision)
		{
			switch (precision)
			{
				case DateTimePrecision.Day:
					return t.Date;
				case DateTimePrecision.Hour:
					return new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0);
				case DateTimePrecision.Minute:
					return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);
				case DateTimePrecision.Second:
					return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
			}
			throw new ArgumentOutOfRangeException("precision");
		}
	}
}
