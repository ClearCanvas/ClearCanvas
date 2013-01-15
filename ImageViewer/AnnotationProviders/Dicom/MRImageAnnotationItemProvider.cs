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
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class MRImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public MRImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.MRImage", new AnnotationResourceResolver(typeof(MRImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.EchoTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.EchoTime].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatMilliseconds, value.ToString("F2"));

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.MagneticFieldStrength",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.MagneticFieldStrength].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatTeslas, value.ToString("F1"));

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.AcquisitionMatrix",
						resolver,
						delegate(Frame frame)
						{
							//TODO (CR Mar 2010): check for the 2 that aren't zero.
							string phaseDirection = frame.ParentImageSop[DicomTags.InPlanePhaseEncodingDirection].ToString().ToUpperInvariant();

							DicomAttribute acqAttrib = frame.ParentImageSop[DicomTags.AcquisitionMatrix];
							if (!acqAttrib.IsEmpty && acqAttrib.Count > 3)
							{
								ushort frequencyRows = acqAttrib.GetUInt16(0, 0);
								ushort frequencyColumns = acqAttrib.GetUInt16(1, 0);
								ushort phaseRows = acqAttrib.GetUInt16(2, 0);
								ushort phaseColumns = acqAttrib.GetUInt16(3, 0);

								switch (phaseDirection)
								{
									case "COL":
										return String.Format(SR.Format2Dimensions, phaseColumns, frequencyRows);
									case "ROW":
									default:
										return String.Format(SR.Format2Dimensions, frequencyColumns, phaseRows);
								}
							}

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.ReceiveCoilName",
						resolver,
						delegate(Frame frame)
						{
							string value;
							value = frame.ParentImageSop[DicomTags.ReceiveCoilName].GetString(0, null);
							return value;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.RepetitionTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.RepetitionTime].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatMilliseconds, value.ToString("F2"));

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.EchoTrainLength",
						resolver,
						delegate(Frame frame)
						{
							int value;
							bool tagExists = frame.ParentImageSop[DicomTags.EchoTrainLength].TryGetInt32(0, out value);
							if (tagExists)
								return String.Format("{0}", value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.InversionTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							var tagExists = frame.ParentImageSop[DicomTags.InversionTime].TryGetFloat64(0, out value);
							return tagExists ? string.Format(SR.FormatMilliseconds, value.ToString("F2")) : string.Empty;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.TriggerTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							var tagExists = frame.ParentImageSop[DicomTags.TriggerTime].TryGetFloat64(0, out value);
							return tagExists ? string.Format(SR.FormatMilliseconds, value.ToString("F2")) : string.Empty;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.NumberOfAverages",
						resolver,
						delegate(Frame frame)
						{
							double value;
							var tagExists = frame.ParentImageSop[DicomTags.NumberOfAverages].TryGetFloat64(0, out value);
							return tagExists ? string.Format("{0}", value) : string.Empty;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.PixelBandwidth",
						resolver,
						delegate(Frame frame)
						{
							double value;
							var tagExists = frame.ParentImageSop[DicomTags.PixelBandwidth].TryGetFloat64(0, out value);
							return tagExists ? string.Format(SR.FormatHertzPerPixel, value.ToString("F2")) : string.Empty;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.FlipAngle",
						resolver,
						delegate(Frame frame)
						{
							double value;
							var tagExists = frame.ParentImageSop[DicomTags.FlipAngle].TryGetFloat64(0, out value);
							return tagExists ? string.Format(SR.FormatDegrees, value.ToString("F2")) : string.Empty;
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