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

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class IntegerDicomTagColumn : DicomTagColumn<DicomArray<int>>, INumericSortableColumn
	{
		public IntegerDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<int> GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<int>();

			int?[] result;
			try
			{
				result = new int?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					int value;
					if (attribute.TryGetInt32(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<int>(result);
		}

		public override bool Parse(string input, out DicomArray<int> output)
		{
			return DicomArray<int>.TryParse(input, int.TryParse, out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(IStudyItem x, IStudyItem y)
		{
			return DicomArray<int>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}