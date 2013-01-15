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
	public class TextDicomTagColumn : DicomTagColumn<string>, ILexicalSortableColumn
	{
		public TextDicomTagColumn(DicomTag dicomTag) : base(dicomTag) { }

		public override string GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return string.Empty;

			return attribute.ToString();
		}

		public override bool Parse(string input, out string output)
		{
			output = input;
			return true;
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}