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
	public class DXImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public DXImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.DXImage", new AnnotationResourceResolver(typeof (DXImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Positioning module
					"Dicom.DXImage.DetectorPrimaryAngle",
					resolver,
					f => FormatFloat64(f, DicomTags.DetectorPrimaryAngle, SR.FormatDegrees),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Positioning module
					"Dicom.DXImage.BodyPartThickness",
					resolver,
					f => string.Format(SR.FormatMillimeters, FormatFloat64(f, DicomTags.BodyPartThickness, "{0:F1}")),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Positioning module
					"Dicom.DXImage.CompressionForce",
					resolver,
					f => FormatInt32(f, DicomTags.CompressionForce, SR.FormatNewtons),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Series module
					"Dicom.DXImage.PresentationIntentType",
					resolver,
					f => f.ParentImageSop[DicomTags.PresentationIntentType].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// whoa, this is actually DX Image module
					"Dicom.DXImage.AcquisitionDeviceProcessingDescription",
					resolver,
					f => f.ParentImageSop[DicomTags.AcquisitionDeviceProcessingDescription].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Detector module
					"Dicom.DXImage.CassetteId",
					resolver,
					f => f.ParentImageSop[DicomTags.CassetteId].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Detector module
					"Dicom.DXImage.PlateId",
					resolver,
					f => f.ParentImageSop[DicomTags.PlateId].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't even a DX module, but other X-Ray related modules
					"Dicom.DXImage.KVP",
					resolver,
					f => FormatFloat64(f, DicomTags.Kvp, SR.FormatKilovolts),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// legacy item (new layouts should use Exposure) so that current annotation layouts don't break - can be removed in a future release
					"Dicom.DXImage.ExposureInMas",
					resolver,
					f => GetExposureInMas(f, SR.FormatMilliampSeconds),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't defined in a DX module, but rather in other X-Ray related modules
					"Dicom.DXImage.Exposure",
					resolver,
					f => GetExposureInMas(f, SR.FormatMilliampSeconds),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't defined in a DX module, but rather in other X-Ray related modules
					"Dicom.DXImage.ExposureTime",
					resolver,
					f => GetExposureTimeInMs(f, SR.FormatMilliseconds),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't defined in a DX module, but rather in other X-Ray related modules
					"Dicom.DXImage.XRayTubeCurrent",
					resolver,
					f => GetXRayTubeCurrentInMa(f, SR.FormatMilliamps),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't even a DX module, but other X-Ray related modules
					"Dicom.DXImage.FilterMaterial",
					resolver,
					f => f.ParentImageSop[DicomTags.FilterMaterial].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new CodeSequenceAnnotationItem
					(
					// this isn't even a DX module, but other X-Ray related modules
					"Dicom.DXImage.ContrastBolusAgent",
					resolver,
					DicomTags.ContrastBolusAgentSequence,
					DicomTags.ContrastBolusAgent
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		internal static string GetExposureInMas(Frame frame, string formatString)
		{
			double dValue;
			if (frame.ParentImageSop[DicomTags.ExposureInMas].TryGetFloat64(0, out dValue))
				return string.Format(formatString, dValue.ToString(@"F2"));

			int iValue;
			if (frame.ParentImageSop[DicomTags.ExposureInUas].TryGetInt32(0, out iValue))
				return string.Format(formatString, (iValue/1000f).ToString(@"F2"));

			return frame.ParentImageSop[DicomTags.Exposure].TryGetInt32(0, out iValue) ? string.Format(formatString, iValue) : string.Empty;
		}

		internal static string GetExposureTimeInMs(Frame frame, string formatString)
		{
			double dValue;
			if (frame.ParentImageSop[DicomTags.ExposureTimeInMs].TryGetFloat64(0, out dValue))
				return string.Format(formatString, dValue.ToString(@"F2"));

			if (frame.ParentImageSop[DicomTags.ExposureTimeInUs].TryGetFloat64(0, out dValue))
				return string.Format(formatString, (dValue/1000.0).ToString(@"F2"));

			int iValue;
			return frame.ParentImageSop[DicomTags.ExposureTime].TryGetInt32(0, out iValue) ? string.Format(formatString, iValue) : string.Empty;
		}

		internal static string GetXRayTubeCurrentInMa(Frame frame, string formatString)
		{
			double dValue;
			if (frame.ParentImageSop[DicomTags.XRayTubeCurrentInMa].TryGetFloat64(0, out dValue))
				return string.Format(formatString, dValue.ToString(@"F2"));

			if (frame.ParentImageSop[DicomTags.XRayTubeCurrentInUa].TryGetFloat64(0, out dValue))
				return string.Format(formatString, (dValue/1000.0).ToString(@"F2"));

			int iValue;
			return frame.ParentImageSop[DicomTags.XRayTubeCurrent].TryGetInt32(0, out iValue) ? string.Format(formatString, iValue) : string.Empty;
		}

		private static string FormatInt32(Frame frame, uint dicomTag, string formatString)
		{
			int value;
			bool tagExists = frame.ParentImageSop[dicomTag].TryGetInt32(0, out value);
			if (tagExists)
				return String.Format(formatString, value);

			return "";
		}

		private static string FormatFloat64(Frame frame, uint dicomTag, string formatString)
		{
			double value;
			bool tagExists = frame.ParentImageSop[dicomTag].TryGetFloat64(0, out value);
			if (tagExists)
				return String.Format(formatString, value);

			return "";
		}
	}
}