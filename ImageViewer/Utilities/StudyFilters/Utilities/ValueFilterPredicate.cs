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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public class ValueFilterPredicate : FilterPredicate
	{
		public readonly StudyFilterColumn Column;
		public readonly object Value;

		public ValueFilterPredicate(StudyFilterColumn column, object value)
		{
			this.Column = column;
			this.Value = value;
		}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetValue(item).Equals(this.Value);
		}
	}

	public class ValueFilterPredicate<T> : ValueFilterPredicate where T : IEquatable<T>
	{
		public ValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public new StudyFilterColumnBase<T> Column
		{
			get { return (StudyFilterColumnBase<T>) base.Column; }
		}

		public new T Value
		{
			get { return (T) base.Value; }
		}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).Equals(this.Value);
		}
	}

	public class GreaterValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public GreaterValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) > 0;
		}
	}

	public class LesserValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public LesserValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) < 0;
		}
	}

	public class GreaterOrEqualValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public GreaterOrEqualValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) >= 0;
		}
	}

	public class LesserOrEqualValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public LesserOrEqualValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) <= 0;
		}
	}
}