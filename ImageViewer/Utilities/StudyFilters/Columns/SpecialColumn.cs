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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionPoint]
	public sealed class SpecialColumnExtensionPoint : ExtensionPoint<ISpecialColumn> {}

	public interface ISpecialColumn
	{
		string Name { get; }
		string Key { get; }
		IStudyFilter Owner { get; }
		string GetText(IStudyItem item);
		object GetValue(IStudyItem item);
		Type GetValueType();
	}

	public abstract class SpecialColumn<T> : StudyFilterColumnBase<T>, ISpecialColumn
	{
		private readonly string _key;
		private readonly string _name;

		protected SpecialColumn(string name, string key)
		{
			_name = name;
			_key = key;
		}

		public override sealed string Name
		{
			get { return _name; }
		}

		public override sealed string Key
		{
			get { return _key; }
		}
	}
}