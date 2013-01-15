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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralStudyAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public GeneralStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralStudy", new AnnotationResourceResolver(typeof(GeneralStudyAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralStudy.AccessionNumber",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.AccessionNumber; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName>
					(
						"Dicom.GeneralStudy.ReferringPhysiciansName",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.ReferringPhysiciansName; },
						DicomDataFormatHelper.PersonNameFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralStudy.StudyDate",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.StudyDate; },
						DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralStudy.StudyTime",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.StudyTime; },
						DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralStudy.StudyDescription",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.StudyDescription; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralStudy.StudyId",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.StudyId),
						DicomDataFormatHelper.RawStringFormat
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
