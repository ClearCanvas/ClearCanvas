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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
	internal static class Utilities
	{
		public static DicomAttribute GetAttribute(this DicomTagPath path, IDicomAttributeProvider attributes)
		{
			return GetAttribute(path, attributes, false);
		}

		public static DicomAttribute GetAttribute(this DicomTagPath path, IDicomAttributeProvider attributes, bool create)
		{
			DicomAttribute attribute;
			var tags = new Queue<DicomTag>(path.TagsInPath);

			do
			{
				var tag = tags.Dequeue();
				attribute = attributes[tag];
				if (tags.Count == 0)
					break;

				var sequenceItems = attribute.Values as DicomSequenceItem[];
				if (sequenceItems == null || sequenceItems.Length == 0)
				{
					if (!create)
						return null;

					attribute.AddSequenceItem(new DicomSequenceItem());
					sequenceItems = (DicomSequenceItem[]) attribute.Values;
				}

				attributes = sequenceItems[0];
			} while (tags.Count > 0);

			if (attribute.IsEmpty && create)
				attribute.SetNullValue();

			return attribute.IsEmpty ? null : attribute;
		}
	}
}