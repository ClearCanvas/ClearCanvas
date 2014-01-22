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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof (AnnotationItemProviderExtensionPoint))]
	public class MRImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public MRImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.MRImage", new AnnotationResourceResolver(typeof (MRImageAnnotationItemProvider).Assembly))
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
								bool tagExists = frame[DicomTags.EffectiveEchoTime].TryGetFloat64(0, out value) || frame[DicomTags.EchoTime].TryGetFloat64(0, out value);
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
								bool tagExists = frame[DicomTags.MagneticFieldStrength].TryGetFloat64(0, out value);
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
								// the acquisition matrix of an MR image refers to the dimensions of the raw data as acquired by the modality
								// which is encoded in frequency domain, so the two axes are phase and frequency
								// it seems that we simply want the width x height of the raw data here
								// which is basically figuring out whether phase is rows or cols, and formatting the numbers appropriately
								string phaseDirection = frame[DicomTags.InPlanePhaseEncodingDirection].ToString().ToUpperInvariant();

								var acqAttrib = frame[DicomTags.AcquisitionMatrix];
								if (!acqAttrib.IsEmpty && acqAttrib.Count >= 4)
								{
									// the order of the values in this attribute is: freq-rows \ freq-cols \ phase-rows \ phase-cols
									// the acquisition matrix tag is used by MR Image module, which uses the code string COL and ROW
									switch (phaseDirection)
									{
										case "COL":
											const int phaseColumns = 3;
											const int frequencyRows = 0;
											return String.Format(SR.Format2Dimensions, acqAttrib.GetUInt16(phaseColumns, 0), acqAttrib.GetUInt16(frequencyRows, 0));
										case "ROW":
										default:
											const int frequencyColumns = 1;
											const int phaseRows = 2;
											return String.Format(SR.Format2Dimensions, acqAttrib.GetUInt16(frequencyColumns, 0), acqAttrib.GetUInt16(phaseRows, 0));
									}
								}
								else
								{
									int phaseSteps, frequencySteps;
									if (frame[DicomTags.MrAcquisitionFrequencyEncodingSteps].TryGetInt32(0, out frequencySteps)
									    && frame[DicomTags.MrAcquisitionPhaseEncodingStepsInPlane].TryGetInt32(0, out phaseSteps))
									{
										// in the MR FOV/Geometry functional group, the code strings are COLUMN, ROW and OTHER
										switch (phaseDirection)
										{
											case "COLUMN":
												return string.Format(SR.Format2Dimensions, phaseSteps, frequencySteps);
											case "ROW":
												return string.Format(SR.Format2Dimensions, frequencySteps, phaseSteps);
										}
									}
								}

								return string.Empty;
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
								value = frame[DicomTags.ReceiveCoilName].GetString(0, null);
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
								bool tagExists = frame[DicomTags.RepetitionTime].TryGetFloat64(0, out value);
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
								bool tagExists = frame[DicomTags.EchoTrainLength].TryGetInt32(0, out value);
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
								var tagExists = frame[DicomTags.InversionTime].TryGetFloat64(0, out value);
								if (tagExists) return string.Format(SR.FormatMilliseconds, value.ToString("F2"));

								DicomAttribute dicomAttribute;
								if (((IDicomAttributeProvider) frame).TryGetAttribute(DicomTags.InversionTimes, out dicomAttribute) && !dicomAttribute.IsEmpty && !dicomAttribute.IsNull)
								{
									var values = dicomAttribute.Values as double[];
									if (values != null) return string.Format(SR.FormatMilliseconds, DicomStringHelper.GetDicomStringArray(values, "F2"));
								}
								return string.Empty;
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
								// TODO CR (30 Sep 2013): Update to support enhanced MR - this tag is actually for cardiac MR, and now appears in the cardiac synchronization functional group
								double value;
								var tagExists = frame[DicomTags.TriggerTime].TryGetFloat64(0, out value);
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
								var tagExists = frame[DicomTags.NumberOfAverages].TryGetFloat64(0, out value);
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
								var tagExists = frame[DicomTags.PixelBandwidth].TryGetFloat64(0, out value);
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
								var tagExists = frame[DicomTags.FlipAngle].TryGetFloat64(0, out value);
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