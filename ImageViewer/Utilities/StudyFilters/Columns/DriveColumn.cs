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
using System.Text.RegularExpressions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class DriveColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "Drive";

		public DriveColumn() : base(SR.Drive, KEY) { }

		public override string GetTypedValue(IStudyItem item)
		{
			DriveInfo drive = GetDriveInfo(item);
			if (drive == null)
				return string.Empty;
			return drive.Name;
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

		private static readonly Regex _localDrive = new Regex("^[A-Za-z]:\\\\?$", RegexOptions.Compiled);

		internal static DriveInfo GetDriveInfo(IStudyItem item)
		{
			if (item == null)
				return null;
			string root = Path.GetPathRoot(item.Filename);
			if (_localDrive.IsMatch(root))
				return new DriveInfo(root[0].ToString());
			return null;
		}
	}
}