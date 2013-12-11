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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	/// <summary>
	/// A factory for SC images for the purpose of creating key image presentation states on dynamically generated images.
	/// </summary>
	internal class SecondaryCaptureImageFactory : SopInstanceFactory
	{
		private readonly Dictionary<string, SeriesInfo> _seriesInfo = new Dictionary<string, SeriesInfo>();
		private readonly List<DicomFile> _files = new List<DicomFile>();
		private readonly NextSeriesNumberDelegate _nextSeriesNumberDelegate;

		public SecondaryCaptureImageFactory(NextSeriesNumberDelegate nextSeriesNumberDelegate)
		{
			_nextSeriesNumberDelegate = nextSeriesNumberDelegate;
		}

		public IEnumerable<DicomFile> Files
		{
			get { return _files; }
		}

		public IPresentationImage CreateSecondaryCapture(IPresentationImage image)
		{
			var imageSopProvider = image as IImageSopProvider;
			if (imageSopProvider == null)
			{
				const string msg = "image must implement IImageSopProvider";
				throw new ArgumentException(msg, "image");
			}

			SeriesInfo seriesInfo;
			var seriesKey = MakeSeriesKey(imageSopProvider.Frame.StudyInstanceUid, imageSopProvider.Sop.Modality);
			if (!_seriesInfo.TryGetValue(seriesKey, out seriesInfo))
				_seriesInfo[seriesKey] = seriesInfo = new SeriesInfo(_nextSeriesNumberDelegate.Invoke(imageSopProvider.Frame.StudyInstanceUid));

			var dcf = CreatePrototypeFile(imageSopProvider.Sop.DataSource);
			FillGeneralSeriesModule(dcf.DataSet, imageSopProvider.Frame, seriesInfo);
			FillScEquipmentModule(dcf.DataSet, Manufacturer, ManufacturersModelName, SoftwareVersions);
			FillFrameOfReferenceModule(dcf.DataSet, imageSopProvider.Frame);
			FillGeneralImageModule(dcf.DataSet, imageSopProvider.Frame, seriesInfo);
			FillScImageModule(dcf.DataSet, imageSopProvider.Frame);
			FillImagePlaneModule(dcf.DataSet, imageSopProvider.Frame);
			FillSopCommonModule(dcf.DataSet, SopClass.SecondaryCaptureImageStorageUid);
			FillAuxiliaryImageData(dcf.DataSet, imageSopProvider.Frame);

			if (image is GrayscalePresentationImage)
			{
				FillModalityLutModule(dcf.DataSet, imageSopProvider.Frame);
				FillVoiLutModule(dcf.DataSet, imageSopProvider.Frame);

				// create image pixel last - this method may need to override some attributes set previously
				CreateImagePixelModuleGrayscale(dcf.DataSet, imageSopProvider.Frame);
			}
			else if (image is ColorPresentationImage)
			{
				// create image pixel last - this method may need to override some attributes set previously
				CreateImagePixelModuleColor(dcf.DataSet, imageSopProvider.Frame);
			}
			else
			{
				// create image pixel last - this method may need to override some attributes set previously
				CreateImagePixelModuleRasterRgb(dcf.DataSet, image);
			}

			dcf.MediaStorageSopClassUid = dcf.DataSet[DicomTags.SopClassUid].ToString();
			dcf.MediaStorageSopInstanceUid = dcf.DataSet[DicomTags.SopInstanceUid].ToString();
			_files.Add(dcf);

			using (var sop = new ImageSop(new LocalSopDataSource(dcf)))
			{
				var secondaryCapture = PresentationImageFactory.Create(sop).Single();
				try
				{
					var presentationState = DicomSoftcopyPresentationState.IsSupported(image) ? DicomSoftcopyPresentationState.Create(image) : null;
					if (presentationState != null)
					{
						presentationState.DeserializeOptions |= DicomSoftcopyPresentationStateDeserializeOptions.IgnoreImageRelationship;
						presentationState.Deserialize(secondaryCapture);

						// override the spatial transform of the secondary capture because the presentation state doesn't save exact parameters
						var sourceTransform = image as ISpatialTransformProvider;
						var targetTransform = secondaryCapture as ISpatialTransformProvider;
						if (sourceTransform != null && targetTransform != null)
						{
							targetTransform.SpatialTransform.CenterOfRotationXY = sourceTransform.SpatialTransform.CenterOfRotationXY;
							targetTransform.SpatialTransform.FlipX = sourceTransform.SpatialTransform.FlipX;
							targetTransform.SpatialTransform.FlipY = sourceTransform.SpatialTransform.FlipY;
							targetTransform.SpatialTransform.RotationXY = sourceTransform.SpatialTransform.RotationXY;
							targetTransform.SpatialTransform.Scale = sourceTransform.SpatialTransform.Scale;
							targetTransform.SpatialTransform.TranslationX = sourceTransform.SpatialTransform.TranslationX;
							targetTransform.SpatialTransform.TranslationY = sourceTransform.SpatialTransform.TranslationY;

							var sourceImageTransform = sourceTransform as IImageSpatialTransform;
							var targetImageTransform = targetTransform as IImageSpatialTransform;
							if (sourceImageTransform != null && targetImageTransform != null)
								targetImageTransform.ScaleToFit = sourceImageTransform.ScaleToFit;
						}
					}

					// force a render to update the client rectangle and scaling of the image
					secondaryCapture.RenderImage(image.ClientRectangle).Dispose();
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "An error has occurred while deserializing the image presentation state.");
				}
				return secondaryCapture;
			}
		}

		#region Basic Modules

		private static void FillGeneralSeriesModule(IDicomAttributeProvider target, IDicomAttributeProvider source, SeriesInfo seriesInfo)
		{
			var sourceModule = new GeneralSeriesModuleIod(source);
			var targetModule = new GeneralSeriesModuleIod(target);

			targetModule.Modality = sourceModule.Modality;
			targetModule.PatientPosition = string.Empty;
			targetModule.SeriesDateTime = seriesInfo.SeriesDateTime;
			targetModule.SeriesDescription = seriesInfo.SeriesDescription;
			targetModule.SeriesInstanceUid = seriesInfo.SeriesInstanceUid;
			targetModule.SeriesNumber = seriesInfo.SeriesNumber;
			targetModule.AnatomicalOrientationType = sourceModule.AnatomicalOrientationType;
			// body part examined and (series) laterality are not filled in because they may vary for multiple SC images
			// we'll use Image Laterality instead, and BPE is optional anyway
		}

		private static void FillScEquipmentModule(IDicomAttributeProvider target, string manufacturer, string modelName, string softwareVersions)
		{
			var targetModule = new ScEquipmentModuleIod(target);
			targetModule.ConversionType = @"WSD";
			// modality is already filled in by General Series module
			targetModule.SecondaryCaptureDeviceManufacturer = manufacturer;
			targetModule.SecondaryCaptureDeviceManufacturersModelName = modelName;
			targetModule.SecondaryCaptureDeviceSoftwareVersions = new[] {softwareVersions};
		}

		private static void FillFrameOfReferenceModule(IDicomAttributeProvider target, IDicomAttributeProvider source)
		{
			var sourceModule = new FrameOfReferenceModuleIod(source);
			var targetModule = new FrameOfReferenceModuleIod(target);

			var frameOfReferenceUid = sourceModule.FrameOfReferenceUid;
			if (!string.IsNullOrEmpty(frameOfReferenceUid))
			{
				targetModule.FrameOfReferenceUid = frameOfReferenceUid;
				targetModule.PositionReferenceIndicator = sourceModule.PositionReferenceIndicator;
			}
		}

		private static void FillGeneralImageModule(IDicomAttributeProvider target, IDicomAttributeProvider source, SeriesInfo seriesInfo)
		{
			var sourceModule = new GeneralImageModuleIod(source);
			var targetModule = new GeneralImageModuleIod(target);

			targetModule.InstanceNumber = seriesInfo.GetNextInstanceNumber();
			targetModule.PatientOrientation = string.Empty;
			targetModule.ContentDateTime = Platform.Time;
			targetModule.ImageType = sourceModule.ImageType;
			targetModule.DerivationDescription = sourceModule.DerivationDescription;
			targetModule.DerivationCodeSequence = sourceModule.DerivationCodeSequence;
			targetModule.SourceImageSequence = sourceModule.SourceImageSequence;
			targetModule.ImageComments = sourceModule.ImageComments;
			targetModule.QualityControlImage = sourceModule.QualityControlImage;
			targetModule.BurnedInAnnotation = sourceModule.BurnedInAnnotation;
			targetModule.RecognizableVisualFeatures = sourceModule.RecognizableVisualFeatures;
			targetModule.LossyImageCompression = sourceModule.LossyImageCompression;
			targetModule.LossyImageCompressionRatio = sourceModule.LossyImageCompressionRatio;
			targetModule.LossyImageCompressionMethod = sourceModule.LossyImageCompressionMethod;
		}

		private static void FillScImageModule(IDicomAttributeProvider target, Frame sourceFrame)
		{
			var targetModule = new ScImageModuleIod(target);
			targetModule.DateTimeOfSecondaryCapture = Platform.Time;

			if (!sourceFrame.NormalizedPixelSpacing.IsNull)
			{
				targetModule.PixelSpacing = new[] {sourceFrame.NormalizedPixelSpacing.Row, sourceFrame.NormalizedPixelSpacing.Column};
				targetModule.PixelSpacingCalibrationType = sourceFrame.NormalizedPixelSpacing.CalibrationType.ToPixelSpacingCalibrationType();
				targetModule.PixelSpacingCalibrationDescription = !string.IsNullOrEmpty(sourceFrame.NormalizedPixelSpacing.CalibrationDetails)
				                                                  	? sourceFrame.NormalizedPixelSpacing.CalibrationDetails
				                                                  	: sourceFrame.NormalizedPixelSpacing.CalibrationType.GetDescription();
			}
		}

		private static void FillImagePlaneModule(IDicomAttributeProvider target, Frame sourceFrame)
		{
			var sourceModule = new ImagePlaneModuleIod(sourceFrame);
			var targetModule = new ImagePlaneModuleIod(target);

			// pixel spacing is already filled in by SC Image module

			var imageOrientationPatient = sourceFrame.ImageOrientationPatient;
			if (!imageOrientationPatient.IsNull)
				target[DicomTags.ImageOrientationPatient].SetStringValue(imageOrientationPatient.ToString());

			var imagePositionPatient = sourceFrame.ImagePositionPatient;
			if (!imagePositionPatient.IsNull)
				target[DicomTags.ImagePositionPatient].SetStringValue(imagePositionPatient.ToString());

			var sliceThickness = sourceModule.SliceThickness;
			if (sliceThickness.HasValue)
				targetModule.SliceThickness = sliceThickness;

			// slice location is not filled in, since it's only useful for relative position within a series, and the series will contain images from various unrelated positions
		}

		private static void FillModalityLutModule(IDicomAttributeProvider target, Frame sourceFrame)
		{
			var targetModule = new ModalityLutModuleIod(target);
			targetModule.RescaleIntercept = sourceFrame.RescaleIntercept;
			targetModule.RescaleSlope = sourceFrame.RescaleSlope;
			targetModule.RescaleType = (string) sourceFrame.RescaleUnits;
		}

		private static void FillVoiLutModule(IDicomAttributeProvider target, Frame sourceFrame)
		{
			var sourceModule = new VoiLutModuleIod(sourceFrame);
			var targetModule = new VoiLutModuleIod(target);

			targetModule.VoiLutSequence = sourceModule.VoiLutSequence;
			targetModule.WindowCenter = sourceModule.WindowCenter;
			targetModule.WindowWidth = sourceModule.WindowWidth;
			targetModule.WindowCenterWidthExplanation = sourceModule.WindowCenterWidthExplanation;
			targetModule.VoiLutFunction = sourceModule.VoiLutFunction;
		}

		private static void FillSopCommonModule(IDicomAttributeProvider target, string sopClassUid)
		{
			var targetModule = new SopCommonModuleIod(target);
			targetModule.SopClassUid = sopClassUid;
			targetModule.SopInstanceUid = DicomUid.GenerateUid().UID;
		}

		private static void FillAuxiliaryImageData(IDicomAttributeProvider target, Frame sourceFrame)
		{
			var laterality = sourceFrame.Laterality;
			if (!string.IsNullOrEmpty(laterality))
				target[DicomTags.ImageLaterality].SetStringValue(laterality);

			if (sourceFrame.ParentImageSop.Modality == "PT")
			{
				try
				{
					target[DicomTags.Units].SetStringValue(sourceFrame.RescaleUnits.ToPetSeriesUnits());
				}
				catch (InvalidOperationException) {}
			}
		}

		#endregion

		#region Image Pixel Module Generation

		private static void CreateImagePixelModuleGrayscale(IDicomAttributeProvider target, Frame sourceFrame)
		{
			target[DicomTags.SamplesPerPixel].SetInt32(0, 1);
			target[DicomTags.PhotometricInterpretation].SetStringValue(sourceFrame.PhotometricInterpretation.Code);
			target[DicomTags.Rows].SetInt32(0, sourceFrame.Rows);
			target[DicomTags.Columns].SetInt32(0, sourceFrame.Columns);
			target[DicomTags.BitsAllocated].SetInt32(0, sourceFrame.BitsAllocated);
			target[DicomTags.BitsStored].SetInt32(0, sourceFrame.BitsStored);
			target[DicomTags.HighBit].SetInt32(0, sourceFrame.BitsStored - 1);
			target[DicomTags.PixelRepresentation].SetInt32(0, sourceFrame.PixelRepresentation);
			target[DicomTags.PlanarConfiguration].SetInt32(0, 0);
			target[DicomTags.PixelAspectRatio].SetStringValue(GetPixelAspectRatioString(sourceFrame));

			var pixelDataAttribute = (DicomAttributeBinary) target[DicomTags.PixelData];
			using (var stream = pixelDataAttribute.AsStream())
			{
				stream.Seek(0, SeekOrigin.Begin);

				var pixelData = sourceFrame.GetNormalizedPixelData();
				var length = pixelData.Length;

				stream.Write(pixelData, 0, length);

				if (length%2 != 0)
				{
					++length;
					stream.WriteByte(0);
				}
				stream.SetLength(length);
			}
		}

		private static unsafe void CreateImagePixelModuleColor(IDicomAttributeProvider target, Frame sourceFrame)
		{
			target[DicomTags.SamplesPerPixel].SetInt32(0, 3);
			target[DicomTags.PhotometricInterpretation].SetStringValue(PhotometricInterpretation.Rgb.Code);
			target[DicomTags.Rows].SetInt32(0, sourceFrame.Rows);
			target[DicomTags.Columns].SetInt32(0, sourceFrame.Columns);
			target[DicomTags.BitsAllocated].SetInt32(0, 8);
			target[DicomTags.BitsStored].SetInt32(0, 8);
			target[DicomTags.HighBit].SetInt32(0, 7);
			target[DicomTags.PixelRepresentation].SetInt32(0, 0);
			target[DicomTags.PlanarConfiguration].SetInt32(0, 0);
			target[DicomTags.PixelAspectRatio].SetStringValue(GetPixelAspectRatioString(sourceFrame));

			var pixelDataAttribute = (DicomAttributeBinary) target[DicomTags.PixelData];
			using (var stream = pixelDataAttribute.AsStream())
			{
				stream.Seek(0, SeekOrigin.Begin);

				var pixelData = sourceFrame.GetNormalizedPixelData();
				var pixelCount = pixelData.Length/4;
				var rgbLength = pixelCount*3;

				fixed (byte* pPixelData = pixelData)
				{
					var pValue = (int*) pPixelData;
					for (var n = 0; n < pixelCount; ++n)
					{
						var value = *pValue++; // endianess concerns (i.e. ARGB vs BGRA) eliminated by reading as int
						stream.WriteByte((byte) (value >> 16)); // R
						stream.WriteByte((byte) (value >> 8)); // G
						stream.WriteByte((byte) (value)); // B
					}
				}

				if (rgbLength%2 != 0)
				{
					++rgbLength;
					stream.WriteByte(0);
				}
				stream.SetLength(rgbLength);
			}
		}

		private static unsafe void CreateImagePixelModuleRasterRgb(IDicomAttributeProvider target, IPresentationImage sourceImage)
		{
			target[DicomTags.SamplesPerPixel].SetInt32(0, 3);
			target[DicomTags.PhotometricInterpretation].SetStringValue(PhotometricInterpretation.Rgb.Code);
			target[DicomTags.Rows].SetInt32(0, sourceImage.ClientRectangle.Height);
			target[DicomTags.Columns].SetInt32(0, sourceImage.ClientRectangle.Width);
			target[DicomTags.BitsAllocated].SetInt32(0, 8);
			target[DicomTags.BitsStored].SetInt32(0, 8);
			target[DicomTags.HighBit].SetInt32(0, 7);
			target[DicomTags.PixelRepresentation].SetInt32(0, 0);
			target[DicomTags.PlanarConfiguration].SetInt32(0, 0);
			target[DicomTags.PixelAspectRatio].SetStringValue(@"1\1");

			using (var helper = new RenderImageHelper(sourceImage))
			{
				// because we effectively "flattened" the image here, any header information regarding orientation, position and pixel spacing is now invalid
				// (cause they just got baked into the pixel data)
				target[DicomTags.ImageOrientationPatient].SetStringValue2(helper.GetImageOrientationPatient());
				target[DicomTags.ImagePositionPatient].SetStringValue2(helper.GetImagePositionPatient());
				target[DicomTags.PixelSpacing].SetStringValue2(helper.GetPixelSpacing());

				var pixelDataAttribute = (DicomAttributeBinary) target[DicomTags.PixelData];
				using (var stream = pixelDataAttribute.AsStream())
				using (var bitmap = helper.Image.RenderImage(sourceImage.ClientRectangle))
				{
					stream.Seek(0, SeekOrigin.Begin);

					var pixelCount = bitmap.Width*bitmap.Height;
					var rgbLength = pixelCount*3;

					var pPixelData = Marshal.AllocHGlobal(pixelCount*4);
					try
					{
						bitmap.CopyArgbDataTo(pPixelData);

						var pValue = (int*) pPixelData;
						for (var n = 0; n < pixelCount; ++n)
						{
							var value = *pValue++; // endianess concerns (i.e. ARGB vs BGRA) eliminated by reading as int
							stream.WriteByte((byte) (value >> 16)); // R
							stream.WriteByte((byte) (value >> 8)); // G
							stream.WriteByte((byte) (value)); // B
						}
					}
					finally
					{
						Marshal.FreeHGlobal(pPixelData);
					}

					if (rgbLength%2 != 0)
					{
						++rgbLength;
						stream.WriteByte(0);
					}
					stream.SetLength(rgbLength);
				}
			}
		}

		#endregion

		#region Utility Methods

		private static string GetPixelAspectRatioString(Frame sourceFrame)
		{
			if (!sourceFrame.NormalizedPixelSpacing.IsNull)
				return sourceFrame.NormalizedPixelSpacing.GetPixelAspectRatioString();
			return !sourceFrame.PixelAspectRatio.IsNull ? sourceFrame.PixelAspectRatio.ToString() : string.Empty;
		}

		private static string MakeSeriesKey(string studyInstanceUid, string modality)
		{
			return string.Concat(studyInstanceUid, ":::", modality);
		}

		#endregion

		#region RenderImageHelper Class

		private class RenderImageHelper : IDisposable
		{
			private readonly bool _annotationLayoutVisible;
			private readonly bool _overlayGraphicsVisible;
			private readonly bool _applicationGraphicsVisible;
			private readonly bool _dicomGraphicsVisible;
			private IPresentationImage _image;

			public RenderImageHelper(IPresentationImage image)
			{
				_image = image;

				var annotationLayoutProvider = image as IAnnotationLayoutProvider;
				if (annotationLayoutProvider != null)
				{
					_annotationLayoutVisible = annotationLayoutProvider.AnnotationLayout.Visible;
					annotationLayoutProvider.AnnotationLayout.Visible = false;
				}

				var overlayGraphicsProvider = image as IOverlayGraphicsProvider;
				if (overlayGraphicsProvider != null)
				{
					_overlayGraphicsVisible = overlayGraphicsProvider.OverlayGraphics.Visible;
					overlayGraphicsProvider.OverlayGraphics.Visible = true;
				}

				var applicationGraphicsProvider = image as IApplicationGraphicsProvider;
				if (applicationGraphicsProvider != null)
				{
					_applicationGraphicsVisible = applicationGraphicsProvider.ApplicationGraphics.Visible;
					applicationGraphicsProvider.ApplicationGraphics.Visible = false;
				}

				var dicomPresentationImage = image as IDicomPresentationImage;
				if (dicomPresentationImage != null)
				{
					_dicomGraphicsVisible = dicomPresentationImage.DicomGraphics.Visible;
					dicomPresentationImage.DicomGraphics.Visible = false;
				}
			}

			public IPresentationImage Image
			{
				get { return _image; }
			}

			public string GetImageOrientationPatient()
			{
				Vector3D row, column;

				var patientPresentationProvider = _image as IPatientPresentationProvider;
				if (patientPresentationProvider != null && patientPresentationProvider.PatientPresentation.IsValid)
				{
					patientPresentationProvider.PatientPresentation.GetOrientation(out row, out column);
					return string.Concat(row.ToDicomAttributeString(), @"\", column.ToDicomAttributeString());
				}

				return BasicPatientPresentation.GetOrientation(_image, out row, out column)
				       	? string.Concat(row.ToDicomAttributeString(), @"\", column.ToDicomAttributeString())
				       	: string.Empty;
			}

			public string GetImagePositionPatient()
			{
				var spatialTransformProvider = _image as ISpatialTransformProvider;
				var imageSopProvider = _image as IImageSopProvider;
				if (spatialTransformProvider != null && imageSopProvider != null && imageSopProvider.Frame.ImagePlaneHelper.IsValid)
				{
					var positionSource = spatialTransformProvider.SpatialTransform.ConvertToSource(new PointF(0, 0));
					var positionPatient = imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(positionSource);
					return positionPatient != null ? positionPatient.ToDicomAttributeString() : string.Empty;
				}
				return string.Empty;
			}

			public string GetPixelSpacing()
			{
				var spatialTransformProvider = _image as ISpatialTransformProvider;
				var imageSopProvider = _image as IImageSopProvider;
				if (spatialTransformProvider != null && imageSopProvider != null && imageSopProvider.Frame.ImagePlaneHelper.IsValid)
				{
					var imageSize = _image.ClientRectangle.Size;
					var positionSource = spatialTransformProvider.SpatialTransform.ConvertToSource(new PointF(0, 0));
					var bottomLeftSource = spatialTransformProvider.SpatialTransform.ConvertToSource(new PointF(0, imageSize.Height));
					var topRightSource = spatialTransformProvider.SpatialTransform.ConvertToSource(new PointF(imageSize.Width, 0));

					var positionPatient = imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(positionSource);
					var imageHeightPatient = (imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(bottomLeftSource) - positionPatient).Magnitude;
					var imageWidthPatient = (imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(topRightSource) - positionPatient).Magnitude;

					var columnSpacing = imageWidthPatient/imageSize.Width;
					var rowSpacing = imageHeightPatient/imageSize.Height;

					// if the aspect ratio is supposed to be 1, ensure the pixel spacing actually says so, regardless of floating point calculation differences
					if (FloatComparer.AreEqual(1, rowSpacing/columnSpacing))
						rowSpacing = columnSpacing = (rowSpacing + columnSpacing)/2;

					return new SizeF(columnSpacing, rowSpacing).ToDicomAttributeString();
				}
				return string.Empty;
			}

			public void Dispose()
			{
				var annotationLayoutProvider = _image as IAnnotationLayoutProvider;
				if (annotationLayoutProvider != null)
					annotationLayoutProvider.AnnotationLayout.Visible = _annotationLayoutVisible;

				var overlayGraphicsProvider = _image as IOverlayGraphicsProvider;
				if (overlayGraphicsProvider != null)
					overlayGraphicsProvider.OverlayGraphics.Visible = _overlayGraphicsVisible;

				var applicationGraphicsProvider = _image as IApplicationGraphicsProvider;
				if (applicationGraphicsProvider != null)
					applicationGraphicsProvider.ApplicationGraphics.Visible = _applicationGraphicsVisible;

				var dicomPresentationImage = _image as IDicomPresentationImage;
				if (dicomPresentationImage != null)
					dicomPresentationImage.DicomGraphics.Visible = _dicomGraphicsVisible;

				_image = null;
			}
		}

		#endregion

		#region SeriesInfo Class

		private class SeriesInfo
		{
			private int _instanceNumber = 1;

			public SeriesInfo(int seriesNumber)
			{
				SeriesNumber = seriesNumber;
				SeriesDateTime = Platform.Time;
				SeriesInstanceUid = DicomUid.GenerateUid().UID;
				SeriesDescription = "FOR KEY IMAGES";
			}

			public int SeriesNumber { get; private set; }
			public DateTime SeriesDateTime { get; private set; }
			public string SeriesInstanceUid { get; private set; }
			public string SeriesDescription { get; private set; }

			public int GetNextInstanceNumber()
			{
				return _instanceNumber++;
			}
		}

		#endregion
	}
}