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

using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class FileSizeColumn : SpecialColumn<FileSize>, INumericSortableColumn
	{
		public const string KEY = "FileSize";

		public FileSizeColumn() : base(SR.FileSize, KEY) { }

		public override FileSize GetTypedValue(IStudyItem item)
		{
			if (item == null || !File.Exists(item.Filename))
				return new FileSize(-1);
			return new FileSize(new FileInfo(item.Filename).Length);
		}

		public override bool Parse(string input, out FileSize output)
		{
			return FileSize.TryParse(input, out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}