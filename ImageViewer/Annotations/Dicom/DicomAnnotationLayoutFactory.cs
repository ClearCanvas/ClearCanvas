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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	internal static class DicomAnnotationLayoutFactory
	{
		public static IAnnotationLayout CreateLayout(IImageSopProvider dicomImage)
		{
			string layoutId = DicomFilteredAnnotationLayoutStore.Instance.GetMatchingStoredLayoutId(dicomImage);
			return AnnotationLayoutFactory.CreateLayout(layoutId);
		}

		public static IAnnotationLayout CreateLayout(IDicomAttributeProvider dicomAttributeProvider)
		{
			string layoutId = DicomFilteredAnnotationLayoutStore.Instance.GetMatchingStoredLayoutId(dicomAttributeProvider);
			return AnnotationLayoutFactory.CreateLayout(layoutId);
		}

		public static IAnnotationLayout CreateLayout(List<KeyValuePair<string, string>> filterCandidates)
		{
			string layoutId = DicomFilteredAnnotationLayoutStore.Instance.GetMatchingStoredLayoutId(filterCandidates);
			return AnnotationLayoutFactory.CreateLayout(layoutId);
		}
	}
}