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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof (AnnotationItemProviderExtensionPoint))]
	public class DXImageAnnotationItemProvider : AnnotationItemProvider
	{
		private const string _keyReconstructionSequence = "RECONSTRUCTION";
		private const string _keyAcquisitionSequence = "ACQUISITION";

		private readonly List<IAnnotationItem> _annotationItems;
		private readonly AnnotationDataSourceContext<Frame> _frameContext;

		public DXImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.DXImage", new AnnotationResourceResolver(typeof (DXImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();
			_frameContext = new AnnotationDataSourceContext<Frame>();
			_frameContext.DataSourceChanged += OnFrameContextDataSourceChanged;

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
						"Dicom.DXImage.PositionerPrimaryAngle",
						resolver,
						f => FormatFloat64(f, DicomTags.PositionerPrimaryAngle, SR.FormatDegrees),
						DicomDataFormatHelper.RawStringFormat
						)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
						(
						// this is actually DX Positioning module
						"Dicom.DXImage.PositionerSecondaryAngle",
						resolver,
						f => FormatFloat64(f, DicomTags.PositionerSecondaryAngle, SR.FormatDegrees),
						DicomDataFormatHelper.RawStringFormat
						)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
						(
						// this is actually DX Positioning or X-Ray 3D Acquisition module
						"Dicom.DXImage.BodyPartThickness",
						resolver,
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return string.Format(SR.FormatMillimeters, FormatMultiValues(dataset, FormatFloat64, DicomTags.BodyPartThickness, "{0:F1}"));
							},
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
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return FormatMultiValues(dataset, FormatInt32, DicomTags.CompressionForce, SR.FormatNewtons);
							},
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
						f => f[DicomTags.PresentationIntentType].ToString(),
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
						f => f[DicomTags.AcquisitionDeviceProcessingDescription].ToString(),
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
						f => f[DicomTags.CassetteId].ToString(),
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
						f => f[DicomTags.PlateId].ToString(),
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
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return FormatMultiValues(dataset, FormatFloat64, DicomTags.Kvp, SR.FormatKilovolts);
							},
						DicomDataFormatHelper.RawStringFormat
						)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
						(
						// this isn't even a DX module, but other X-Ray related modules
						"Dicom.DXImage.AveragePulseWidth",
						resolver,
						f => FormatFloat64(f, DicomTags.AveragePulseWidth, SR.FormatMilliseconds),
						DicomDataFormatHelper.RawStringFormat
						)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
						(
						// this isn't even a DX module, but other X-Ray related modules
						"Dicom.DXImage.ImageAndFluoroscopyAreaDoseProduct",
						resolver,
						f => FormatFloat64(f, DicomTags.ImageAndFluoroscopyAreaDoseProduct, SR.FormatDecigraySquareCentimeters),
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
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return FormatMultiValues(dataset, GetExposureInMas, SR.FormatMilliampSeconds);
							},
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
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return FormatMultiValues(dataset, GetExposureInMas, SR.FormatMilliampSeconds);
							},
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
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return FormatMultiValues(dataset, GetExposureTimeInMs, SR.FormatMilliseconds);
							},
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
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return FormatMultiValues(dataset, GetXRayTubeCurrentInMa, SR.FormatMilliamps);
							},
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
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								var dataset = acquisitions != null ? (IEnumerable<IDicomAttributeProvider>) acquisitions.Select(a => a.DicomSequenceItem) : new IDicomAttributeProvider[] {f};
								return FormatMultiValues(dataset, FormatString, DicomTags.FilterMaterial, null);
							},
						DicomDataFormatHelper.RawStringFormat
						)
				);

			_annotationItems.Add
				(
					new CodeSequenceAnnotationItem
						(
						// definition of the contrast/bolus agent item takes into account the X-Ray 3D Acquisition sequence, and is thus separate from the normal contast/bolus modules
						"Dicom.DXImage.ContrastBolusAgent",
						resolver,
						DicomTags.ContrastBolusAgentSequence,
						DicomTags.ContrastBolusAgent,
						f =>
							{
								var acquisitions = _frameContext.GetData<IXRay3DAcquisitionSequenceItem[]>(f, _keyAcquisitionSequence);
								return acquisitions != null ? (IDicomAttributeProvider) acquisitions.Select(d => d.DicomSequenceItem).FirstOrDefault() : f;
							}
						)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		private static void OnFrameContextDataSourceChanged(object sender, AnnotationDataSourceContextEventArgs<Frame> e)
		{
			if (e.DataSource != null)
			{
				var reconstruction = GetReconstruction(e.DataSource);
				if (reconstruction == null) return;
				e[_keyReconstructionSequence] = reconstruction;

				var acquisitions = GetAcquisitions(e.DataSource, reconstruction.AcquisitionIndex);
				if (acquisitions == null) return;
				e[_keyAcquisitionSequence] = acquisitions;
			}
		}

		private static XRay3DReconstructionSequenceItem GetReconstruction(Frame frame)
		{
			var reconstructionModule = new XRay3DReconstructionModule(frame.ParentImageSop);
			if (!reconstructionModule.HasValues()) return null;

			var reconstructionItems = reconstructionModule.XRay3DReconstructionSequence;
			if (reconstructionItems == null) return null;

			var reconstructionIndex = frame[DicomTags.ReconstructionIndex].GetInt32(0, -1); // DICOM indexes are 1-based
			return reconstructionIndex > 0 && reconstructionItems.Length >= reconstructionIndex ? reconstructionItems[reconstructionIndex - 1] : null;
		}

		private static IXRay3DAcquisitionSequenceItem[] GetAcquisitions(Frame frame, int[] acquisitionIndices)
		{
			var acquisitionModule = new XRay3DAcquisitionModule(frame.ParentImageSop);
			if (!acquisitionModule.HasValues()) return null;

			var acquisitionItems = acquisitionModule.XRay3DAcquisitionSequence;
			if (acquisitionItems == null) return null;

			// DICOM indexes are 1-based
			return acquisitionIndices != null && acquisitionIndices.Min() > 0
			       && acquisitionItems.Length >= acquisitionIndices.Max() ? acquisitionIndices.Select(i => acquisitionItems[i - 1]).ToArray() : null;
		}

		private static string FormatMultiValues(IEnumerable<IDicomAttributeProvider> frames, Func<IDicomAttributeProvider, string, string> formatter, string formatString)
		{
			return DicomStringHelper.GetDicomStringArray(frames.Select(f => formatter(f, formatString)));
		}

		private static string FormatMultiValues(IEnumerable<IDicomAttributeProvider> frames, Func<IDicomAttributeProvider, uint, string, string> formatter, uint dicomTag, string formatString)
		{
			return DicomStringHelper.GetDicomStringArray(frames.Select(f => formatter(f, dicomTag, formatString)));
		}

		internal static string GetExposureInMas(IDicomAttributeProvider frame, string formatString)
		{
			double dValue;
			if (frame[DicomTags.ExposureInMas].TryGetFloat64(0, out dValue))
				return string.Format(formatString, dValue.ToString(@"F2"));

			int iValue;
			if (frame[DicomTags.ExposureInUas].TryGetInt32(0, out iValue))
				return string.Format(formatString, (iValue/1000f).ToString(@"F2"));

			return frame[DicomTags.Exposure].TryGetInt32(0, out iValue) ? string.Format(formatString, iValue) : string.Empty;
		}

		internal static string GetExposureTimeInMs(IDicomAttributeProvider frame, string formatString)
		{
			double dValue;
			if (frame[DicomTags.ExposureTimeInMs].TryGetFloat64(0, out dValue))
				return string.Format(formatString, dValue.ToString(@"F2"));

			if (frame[DicomTags.ExposureTimeInUs].TryGetFloat64(0, out dValue))
				return string.Format(formatString, (dValue/1000.0).ToString(@"F2"));

			int iValue;
			return frame[DicomTags.ExposureTime].TryGetInt32(0, out iValue) ? string.Format(formatString, iValue) : string.Empty;
		}

		internal static string GetXRayTubeCurrentInMa(IDicomAttributeProvider frame, string formatString)
		{
			double dValue;
			if (frame[DicomTags.XRayTubeCurrentInMa].TryGetFloat64(0, out dValue))
				return string.Format(formatString, dValue.ToString(@"F2"));

			if (frame[DicomTags.XRayTubeCurrentInUa].TryGetFloat64(0, out dValue))
				return string.Format(formatString, (dValue/1000.0).ToString(@"F2"));

			int iValue;
			return frame[DicomTags.XRayTubeCurrent].TryGetInt32(0, out iValue) ? string.Format(formatString, iValue) : string.Empty;
		}

		private static string FormatInt32(IDicomAttributeProvider frame, uint dicomTag, string formatString)
		{
			int value;
			bool tagExists = frame[dicomTag].TryGetInt32(0, out value);
			if (tagExists)
				return String.Format(formatString, value);

			return "";
		}

		private static string FormatFloat64(IDicomAttributeProvider frame, uint dicomTag, string formatString)
		{
			double value;
			bool tagExists = frame[dicomTag].TryGetFloat64(0, out value);
			if (tagExists)
				return String.Format(formatString, value);

			return "";
		}

		private static string FormatString(IDicomAttributeProvider frame, uint dicomTag, string unused)
		{
			DicomAttribute attribute;
			bool tagExists = frame.TryGetAttribute(dicomTag, out attribute) && !attribute.IsEmpty;
			if (tagExists)
				return attribute.ToString();

			return "";
		}
	}
}