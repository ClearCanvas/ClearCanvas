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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralSeriesAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public GeneralSeriesAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralSeries", new AnnotationResourceResolver(typeof(GeneralSeriesAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.BodyPartExamined",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.BodyPartExamined; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.Laterality",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.Laterality; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.Modality",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.Modality; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName[]>
					(
						"Dicom.GeneralSeries.OperatorsName",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.OperatorsName; },
						DicomDataFormatHelper.PersonNameListFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.PerformedProcedureStepDescription",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.PerformedProcedureStepDescription),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName[]>
					(
						"Dicom.GeneralSeries.PerformingPhysiciansName",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PerformingPhysiciansName; },
						DicomDataFormatHelper.PersonNameListFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.ProtocolName",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.ProtocolName),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesDate",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.SeriesDate; },
						DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesTime",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.SeriesTime; },
						DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesDescription",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.SeriesDescription; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesNumber",
						resolver,
						delegate(Frame frame)
						{
							if (frame.ParentImageSop.ParentSeries != null)
							{
								return String.Format(SR.FormatSeriesNumberAndCount, frame.ParentImageSop.SeriesNumber, 
									frame.ParentImageSop.ParentSeries.ParentStudy.Series.Count);
							}
							else
							{
								return frame.ParentImageSop.SeriesNumber.ToString();
							}
						},
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
