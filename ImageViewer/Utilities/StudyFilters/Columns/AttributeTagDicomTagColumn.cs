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
using System.Text.RegularExpressions;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class AttributeTagDicomTagColumn : UnsignedDicomTagColumn
	{
		private static readonly Regex _pattern = new Regex(@"^(?<Open>[(])?\s*([0-9a-fA-F]{4})\s*[,]?\s*([0-9a-fA-F]{4})\s*(?(Open)[)]|)$", RegexOptions.Compiled);

		public AttributeTagDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<uint> GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<uint>();

			uint?[] result;
			try
			{
				result = new uint?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					uint value;
					if (attribute.TryGetUInt32(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<uint>(result, FormatArray(result));
		}

		public override bool Parse(string input, out DicomArray<uint> output)
		{
			return DicomArray<uint>.TryParse(input, TagTryParse, out output);
		}

		private static bool TagTryParse(string s, out uint result)
		{
			result = 0;

			Match m = _pattern.Match(s);
			if (m.Success)
				result = uint.Parse(m.Groups[1].Value + ushort.Parse(m.Groups[2].Value));
			return m.Success;
		}

		private static string FormatArray(IEnumerable<uint?> input)
		{
			StringBuilder sb = new StringBuilder();
			foreach (uint? element in input)
			{
				if (element.HasValue)
					sb.AppendFormat("({0:x4},{1:x4})", (element.Value >> 16) & 0x0000ffff, element.Value & 0x0000ffff);
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}
	}
}