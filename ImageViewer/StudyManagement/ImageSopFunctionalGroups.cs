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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	partial class ImageSop
	{
		private readonly IDictionary<uint, FunctionalGroupDescriptor> _functionalGroups;

		public override sealed DicomAttribute this[DicomTag tag]
		{
			get { return GetDicomAttribute(-1, tag); }
		}

		public override sealed DicomAttribute this[uint tag]
		{
			get { return GetDicomAttribute(-1, tag); }
		}

		public virtual DicomAttribute GetDicomAttribute(int frameNumber, DicomTag tag)
		{
			return GetMultiFrameDicomAttribute(frameNumber, tag) ?? base[tag];
		}

		public virtual DicomAttribute GetDicomAttribute(int frameNumber, uint tag)
		{
			return GetMultiFrameDicomAttribute(frameNumber, tag) ?? base[tag];
		}

		public virtual bool TryGetDicomAttribute(int frameNumber, DicomTag tag, out DicomAttribute dicomAttribute)
		{
			dicomAttribute = GetMultiFrameDicomAttribute(frameNumber, tag);
			return dicomAttribute != null || DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		public virtual bool TryGetDicomAttribute(int frameNumber, uint tag, out DicomAttribute dicomAttribute)
		{
			dicomAttribute = GetMultiFrameDicomAttribute(frameNumber, tag);
			return dicomAttribute != null || DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		private DicomAttribute GetMultiFrameDicomAttribute(int frameNumber, DicomTag tag)
		{
			FunctionalGroupDescriptor functionalGroupDescriptor;
			if (frameNumber > 0 && _functionalGroups != null && _functionalGroups.TryGetValue(tag.TagValue, out functionalGroupDescriptor))
			{
				DicomAttribute dicomAttribute;
				var functionalGroup = MultiFrameFunctionalGroupsModuleIod.GetFunctionalGroup(functionalGroupDescriptor, DataSource, frameNumber);
				var item = functionalGroup != null ? functionalGroup.SingleItem : null;
				if (item != null && item.TryGetAttribute(tag, out dicomAttribute)) return dicomAttribute;
			}
			return null;
		}

		private DicomAttribute GetMultiFrameDicomAttribute(int frameNumber, uint tag)
		{
			FunctionalGroupDescriptor functionalGroupDescriptor;
			if (frameNumber > 0 && _functionalGroups != null && _functionalGroups.TryGetValue(tag, out functionalGroupDescriptor))
			{
				DicomAttribute dicomAttribute;
				var functionalGroup = MultiFrameFunctionalGroupsModuleIod.GetFunctionalGroup(functionalGroupDescriptor, DataSource, frameNumber);
				var item = functionalGroup != null ? functionalGroup.SingleItem : null;
				if (item != null && item.TryGetAttribute(tag, out dicomAttribute)) return dicomAttribute;
			}
			return null;
		}

		private static IDictionary<uint, FunctionalGroupDescriptor> GetFunctionalGroupMap(ISopDataSource sopDataSource)
		{
			return new MultiFrameFunctionalGroupsModuleIod(sopDataSource).HasValues() ? FunctionalGroupDescriptor.GetFunctionalGroupMap(sopDataSource.SopClassUid) : null;
		}
	}
}