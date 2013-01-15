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

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public abstract class SortPredicate : IComparer<IStudyItem>
	{
		public readonly StudyFilterColumn Column;

		public SortPredicate(StudyFilterColumn column)
		{
			this.Column = column;
		}

		public override sealed bool Equals(object obj)
		{
			SortPredicate other = obj as SortPredicate;
			if (other != null)
				return this.Column == other.Column && this.GetType() == other.GetType();
			return false;
		}

		public override sealed int GetHashCode()
		{
			return 0x00DBFF0B ^ this.GetType().GetHashCode() ^ this.Column.GetHashCode();
		}

		public virtual int Compare(IStudyItem x, IStudyItem y)
		{
			return this.Column.Compare(x, y);
		}
	}

	public sealed class AscendingSortPredicate : SortPredicate
	{
		public AscendingSortPredicate(StudyFilterColumn column) : base(column) {}
	}

	public sealed class DescendingSortPredicate : SortPredicate
	{
		public DescendingSortPredicate(StudyFilterColumn column) : base(column) {}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return base.Compare(y, x);
		}
	}
}