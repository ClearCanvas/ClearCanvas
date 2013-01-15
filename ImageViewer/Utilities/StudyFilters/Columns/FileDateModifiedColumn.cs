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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof (SpecialColumnExtensionPoint))]
	public class FileDateModifiedColumn : SpecialColumn<FileDateTime>, ITemporalSortableColumn
	{
		public const string KEY = "FileDateModified";

		public FileDateModifiedColumn() : base(SR.DateModified, KEY) {}

		public override FileDateTime GetTypedValue(IStudyItem item)
		{
			if (item == null || !File.Exists(item.Filename))
				return new FileDateTime(null);
			return new FileDateTime(File.GetLastWriteTime(item.Filename));
		}

		public override bool Parse(string input, out FileDateTime output)
		{
			return FileDateTime.TryParse(input, out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareTemporally(x, y);
		}

		public int CompareTemporally(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}