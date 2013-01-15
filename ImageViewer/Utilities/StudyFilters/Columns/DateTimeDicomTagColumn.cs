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
using System.Globalization;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class DateTimeDicomTagColumn : DicomTagColumn<DicomArray<DateTime>>, ITemporalSortableColumn
	{
		private readonly string _dateTimeFormat;

		public DateTimeDicomTagColumn(DicomTag dicomTag) : base(dicomTag)
		{
			_dateTimeFormat = "f";
			if (base.VR == "DA")
				_dateTimeFormat = "d";
			else if (base.VR == "TM")
				_dateTimeFormat = "t";
		}

		private bool DateTimeTryParseExact(string s, out DateTime result)
		{
			return DateTime.TryParseExact(s, _dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out result);
		}

		public override DicomArray<DateTime> GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<DateTime>();

			DateTime?[] result;
			try
			{
				result = new DateTime?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					DateTime value;
					if (attribute.TryGetDateTime(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}

			return new DicomArray<DateTime>(result, FormatArray(result, _dateTimeFormat));
		}

		public override bool Parse(string input, out DicomArray<DateTime> output)
		{
			if (DicomArray<DateTime>.TryParse(input, DateTimeTryParseExact, out output))
				return true;

			if (DicomArray<DateTime>.TryParse(input, DateTime.TryParse, out output))
				return true;

			switch (base.VR)
			{
				case "DA":
					if (DicomArray<DateTime>.TryParse(input, DateParser.Parse, out output))
						return true;
					break;
				case "TM":
					if (DicomArray<DateTime>.TryParse(input, TimeParser.Parse, out output))
						return true;
					break;
				case "DT":
				default:
					if (DicomArray<DateTime>.TryParse(input, DateTimeParser.Parse, out output))
						return true;
					break;
			}
			return false;
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareTemporally(x, y);
		}

		public int CompareTemporally(IStudyItem x, IStudyItem y)
		{
			return DicomArray<DateTime>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}

		private static string FormatArray(IEnumerable<DateTime?> input, string elementFormat)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DateTime? element in input)
			{
				if (element.HasValue)
					sb.Append(element.Value.ToString(elementFormat));
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}
	}
}