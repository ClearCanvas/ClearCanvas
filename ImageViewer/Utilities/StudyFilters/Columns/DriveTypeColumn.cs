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
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class DriveTypeColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "DriveType";

		public DriveTypeColumn() : base(SR.DriveType, KEY) { }

		public override string GetTypedValue(IStudyItem item)
		{
			DriveInfo drive = DriveColumn.GetDriveInfo(item);
			if (drive != null)
			{
				switch (drive.DriveType)
				{
					case DriveType.Removable:
						return SR.RemovableMedia;
					case DriveType.Fixed:
						return SR.FixedMedia;
					case DriveType.Network:
						return SR.NetworkMedia;
					case DriveType.CDRom:
						return SR.CDRomMedia;
					case DriveType.Ram:
						return SR.RAM;
				}
			}
			return SR.Unknown;
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