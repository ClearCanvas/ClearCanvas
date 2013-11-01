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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public interface IDicomTagColumn
	{
		uint Tag { get; }
		string VR { get; }

		string Name { get; }
		string Key { get; }
		IStudyFilter Owner { get; }
		string GetText(IStudyItem item);
		object GetValue(IStudyItem item);
		Type GetValueType();
	}

	public abstract class DicomTagColumn<T> : StudyFilterColumnBase<T>, IDicomTagColumn
	{
		private readonly string _tagName;
		private readonly uint _tag;
		private readonly string _vr;

		protected DicomTagColumn(DicomTag dicomTag)
		{
			_tag = dicomTag.TagValue;
			_vr = dicomTag.VR.Name;

			uint tagGroup = (_tag >> 16) & 0x0000FFFF;
			uint tagElement = _tag & 0x0000FFFF;
			_tagName = string.Format(SR.FormatDicomTag, tagGroup, tagElement, dicomTag.Name);
		}

		public override string Name
		{
			get { return _tagName; }
		}

		public override string Key
		{
			get { return _tag.ToString("x8"); }
		}

		public uint Tag
		{
			get { return _tag; }
		}

		public string VR
		{
			get { return _vr; }
		}

		protected static int CountValues(DicomAttribute attribute)
		{
			return (int) Math.Min(50, attribute.Count);
		}
	}
}