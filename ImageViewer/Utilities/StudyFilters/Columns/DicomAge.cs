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
using System.Text.RegularExpressions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public struct DicomAge : IComparable<DicomAge>, IComparable, IEquatable<DicomAge>
	{
		private static readonly Regex _parserPattern = new Regex("^(\\d{3})([DWMY])$", RegexOptions.Compiled);
		public static readonly DicomAge MaxValue = new DicomAge(999, DicomAgeUnits.Years);
		public static readonly DicomAge MinValue = new DicomAge(0, DicomAgeUnits.Days);
		public static readonly DicomAge Zero = new DicomAge(0, DicomAgeUnits.Days);
		public const double DaysPerYear = 365 + 97/400d; // average days per year (factoring in leap days over a 400 year period)
		public const double DaysPerMonth = DaysPerYear/12; // average days per month (factoring in leap days over a 400 year period)
		public const double DaysPerWeek = 7.0; // exactly 7 days per week

		private readonly int _count;
		private readonly DicomAgeUnits _units;

		public DicomAge(int count, DicomAgeUnits units)
		{
			Platform.CheckArgumentRange(count, 0, 999, "count");

			_count = count;
			_units = units;
		}

		public int Count
		{
			get { return _count; }
		}

		public DicomAgeUnits Units
		{
			get { return _units; }
		}

		public double TotalDays
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count;
					case DicomAgeUnits.Weeks:
						return _count/DaysPerWeek;
					case DicomAgeUnits.Months:
						return _count/DaysPerMonth;
					case DicomAgeUnits.Years:
					default:
						return _count/DaysPerYear;
				}
			}
		}

		public double TotalWeeks
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count*DaysPerWeek;
					case DicomAgeUnits.Weeks:
						return _count;
					case DicomAgeUnits.Months:
						return _count*DaysPerWeek/DaysPerMonth;
					case DicomAgeUnits.Years:
					default:
						return _count*DaysPerWeek/DaysPerYear;
				}
			}
		}

		public double TotalMonths
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count*DaysPerMonth;
					case DicomAgeUnits.Weeks:
						return _count*DaysPerMonth/DaysPerWeek;
					case DicomAgeUnits.Months:
						return _count;
					case DicomAgeUnits.Years:
					default:
						return _count*DaysPerMonth/DaysPerYear;
				}
			}
		}

		public double TotalYears
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count*DaysPerYear;
					case DicomAgeUnits.Weeks:
						return _count*DaysPerYear/DaysPerWeek;
					case DicomAgeUnits.Months:
						return _count*DaysPerYear/DaysPerMonth;
					case DicomAgeUnits.Years:
					default:
						return _count;
				}
			}
		}

		#region Conversion Members

		public static implicit operator TimeSpan (DicomAge x)
		{
			return new TimeSpan((int) x.TotalDays, 0, 0, 0);
		}

		public static implicit operator DicomAge (TimeSpan x)
		{
			return new DicomAge(x.Days, DicomAgeUnits.Days);
		}

		#endregion

		#region Hashing Members

		public override int GetHashCode()
		{
			return -0x15A760B5 ^ _count.GetHashCode() ^ _units.GetHashCode();
		}

		#endregion

		#region Comparison Members

		public int CompareTo(DicomAge other)
		{
			return this.TotalDays.CompareTo(other.TotalDays);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (obj is DicomAge)
				return this.CompareTo((DicomAge) obj);
			throw new ArgumentException("Parameter must be a DicomAge.", "obj");
		}

		public bool Equals(DicomAge other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is DicomAge)
				return this.Equals((DicomAge) obj);
			return false;
		}

		#endregion

		#region Formatting Members

		public string ToString(string format)
		{
			switch(format.ToLowerInvariant())
			{
				case "d":
					return this.ToString(DicomAgeUnits.Days);
				case "w":
					return this.ToString(DicomAgeUnits.Weeks);
				case "m":
					return this.ToString(DicomAgeUnits.Months);
				case "y":
					return this.ToString(DicomAgeUnits.Years);
				default:
					throw new FormatException("Invalid format specifier.");
			}
		}

		public string ToString(DicomAgeUnits units)
		{
			switch (units)
			{
				case DicomAgeUnits.Days:
					return string.Format("{0:000}D", (int) this.TotalDays);
				case DicomAgeUnits.Weeks:
					return string.Format("{0:000}W", (int) this.TotalWeeks);
				case DicomAgeUnits.Months:
					return string.Format("{0:000}M", (int) this.TotalMonths);
				default:
				case DicomAgeUnits.Years:
					return string.Format("{0:000}Y", (int) this.TotalYears);
			}
		}

		public override string ToString()
		{
			return this.ToString(_units);
		}

		#endregion

		#region Parsing Members

		public static DicomAge Parse(string s)
		{
			DicomAge age;
			if (TryParse(s, out age))
				return age;
			throw new FormatException("Parameter was not in the expected format.");
		}

		public static bool TryParse(string s, out DicomAge dicomAge)
		{
			if (s != null)
				s = s.Trim();

			dicomAge = Zero;

			Match m = _parserPattern.Match(s);
			if (!m.Success)
				return false;

			int value;
			int.TryParse(m.Groups[1].Value, out value);
			switch (m.Groups[2].Value)
			{
				case "D":
					dicomAge = new DicomAge(value, DicomAgeUnits.Days);
					break;
				case "W":
					dicomAge = new DicomAge(value, DicomAgeUnits.Weeks);
					break;
				case "M":
					dicomAge = new DicomAge(value, DicomAgeUnits.Months);
					break;
				case "Y":
				default:
					dicomAge = new DicomAge(value, DicomAgeUnits.Years);
					break;
			}
			return true;
		}

		#endregion

		#region Comparison Operators

		public static bool operator ==(DicomAge x, DicomAge y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(DicomAge x, DicomAge y)
		{
			return !x.Equals(y);
		}

		public static bool operator >(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) > 0;
		}

		public static bool operator <(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) < 0;
		}

		public static bool operator >=(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) >= 0;
		}

		public static bool operator <=(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) <= 0;
		}

		#endregion
	}

	public enum DicomAgeUnits
	{
		Days,
		Weeks,
		Months,
		Years
	}
}