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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof (AnnotationItemProviderExtensionPoint))]
	public class GeneralEquipmentAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public GeneralEquipmentAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralEquipment", new AnnotationResourceResolver(typeof (GeneralEquipmentAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.DateOfLastCalibration",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.DateOfLastCalibration),
					DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.TimeOfLastCalibration",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.TimeOfLastCalibration),
					DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.DeviceSerialNumber",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.DeviceSerialNumber),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.InstitutionAddress",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.InstitutionAddress),
					SingleLineStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.InstitutionalDepartmentName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.InstitutionalDepartmentName; },
					SingleLineStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.InstitutionName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.InstitutionName; },
					SingleLineStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.Manufacturer",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.Manufacturer; },
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.ManufacturersModelName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.ManufacturersModelName; },
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.StationName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.StationName; },
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string[]>
					(
					"Dicom.GeneralEquipment.SoftwareVersions",
					resolver,
					FrameDataRetrieverFactory.GetStringArrayRetriever(DicomTags.SoftwareVersions),
					DicomDataFormatHelper.StringListFormat
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		private static string SingleLineStringFormat(string input)
		{
			return string.Join(SR.SeparatorSingleLine, input.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries));
		}
	}
}