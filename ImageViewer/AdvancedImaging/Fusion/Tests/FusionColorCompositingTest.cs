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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[TestFixture(Description = "Tests for validating the output colours of compositing the overlay data on top of an underlying image")]
	public class FusionColorCompositingTest
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test(Description = "Tests the fusion of two images at varying opacity levels using the HOT IRON colour map for the overlay.")]
		public void TestHotIronColorMap()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			var results = DiffFusionOperatorResults(new HotIronColorMapFactory().Create(), false, testName);

			for (int n = 0; n < results.Count; n++)
				Assert.Less(results[n], 0.01, "{0}:: opacity test frame #{1} exceeds difference limit.", testName, n);
		}

		private static IList<double> DiffFusionOperatorResults(IColorMap colorMap, bool thresholding, string testName)
		{
			var outputPath = new DirectoryInfo(Path.Combine(typeof (FusionColorCompositingTest).FullName, testName));
			if (outputPath.Exists)
				outputPath.Delete(true);
			outputPath.Create();

			// this kind of test requires that the base and overlay slices be unsigned and precisely coincident
			using (var data = new FusionTestDataContainer(
				() => TestDataFunction.GradientX.CreateSops(false, Modality.CT, new Vector3D(1.0f, 1.0f, 15.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit),
				() => TestDataFunction.GradientY.CreateSops(false, Modality.PT, new Vector3D(1.0f, 1.0f, 15.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit)))
			{
				using (var log = File.CreateText(Path.Combine(outputPath.FullName, "data.csv")))
				{
					log.WriteLine("{0}, {1}, {2}, {3}", "n", "opacity", "alpha", "diff");
					using (var baseDisplaySet = data.CreateBaseDisplaySet())
					{
						using (var overlayDisplaySet = data.CreateOverlayDisplaySet())
						{
							using (var fusionDisplaySet = data.CreateFusionDisplaySet())
							{
								var list = new List<double>();

								var imageIndex = fusionDisplaySet.PresentationImages.Count/2;
								var fusionImage = fusionDisplaySet.PresentationImages[imageIndex];

								for (int n = 0; n <= 10; n++)
								{
									var opacity = n/10f;

									SetFusionDisplayParameters(fusionImage, colorMap, opacity, thresholding);
									using (IPresentationImage referenceImage =
										Fuse(baseDisplaySet.PresentationImages[imageIndex], overlayDisplaySet.PresentationImages[imageIndex], colorMap, opacity, thresholding))
									{
										using (var referenceBmp = DrawToBitmap(referenceImage))
										{
											referenceBmp.Save(Path.Combine(outputPath.FullName, string.Format("reference{0}.png", n)));
										}

										using (var fusionBmp = DrawToBitmap(fusionImage))
										{
											fusionBmp.Save(Path.Combine(outputPath.FullName, string.Format("test{0}.png", n)));
										}

										// because the fusion display set is generated from real base images and *reformatted* overlay images,
										// there will necessarily be higher differences at the edges of the useful image area

										Bitmap diff;
										double result = ImageDiff.Compare(ImageDiffAlgorithm.Euclidian, referenceImage, fusionImage, new Rectangle(18, 18, 98, 98), out diff);
										diff.Save(Path.Combine(outputPath.FullName, string.Format("diff{0}.png", n)));
										diff.Dispose();
										log.WriteLine("{0}, {1:f2}, {2}, {3:f6}", n, opacity, (int) (255*opacity), result);
										list.Add(result);
									}
								}

								return list;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Reference implementation of the image fusion operator.
		/// </summary>
		private static IPresentationImage Fuse(IPresentationImage baseImage, IPresentationImage overlayImage, IColorMap colorMap, float opacity, bool thresholding)
		{
			Platform.CheckTrue(baseImage is IImageSopProvider, "baseImage must be a IImageSopProvider.");
			Platform.CheckTrue(overlayImage is IImageSopProvider, "overlayImage must be a IImageSopProvider.");
			Platform.CheckTrue(baseImage is IImageGraphicProvider, "baseImage must be a IImageGraphicProvider.");
			Platform.CheckTrue(overlayImage is IImageGraphicProvider, "overlayImage must be a IImageGraphicProvider.");

			var baseImageSopProvider = (IImageSopProvider) baseImage;
			var baseImageGraphicProvider = (IImageGraphicProvider) baseImage;
			var overlayImageSopProvider = (IImageSopProvider) overlayImage;
			var overlayImageGraphicProvider = (IImageGraphicProvider) overlayImage;
			var rows = baseImageSopProvider.Frame.Rows;
			var cols = baseImageSopProvider.Frame.Columns;
			var pixelData = new byte[3*rows*cols];

			colorMap.MinInputValue = ushort.MinValue;
			colorMap.MaxInputValue = ushort.MaxValue;

			// this here is the magic
			baseImageGraphicProvider.ImageGraphic.PixelData.ForEachPixel(
				(n, x, y, i) =>
					{
						// and this is why the base and overlay slices must be unsigned precisely coincident
						var patientLocation = baseImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(new PointF(x, y));
						var overlayCoordinate = overlayImageSopProvider.Frame.ImagePlaneHelper.ConvertToImagePlane(patientLocation);
						var baseValue = (ushort) baseImageGraphicProvider.ImageGraphic.PixelData.GetPixel(i);
						var overlayValue = overlayImageGraphicProvider.ImageGraphic.PixelData.GetPixel((int) overlayCoordinate.X, (int) overlayCoordinate.Y);

						// the fusion operator: output = underlyingGrey*(1-alpha) + overlayingColour*(alpha) (see DICOM 2009 PS 3.4 N.2.4.3)
						var compositeColor = ToRgbVectorFromGrey(baseValue)*(1 - opacity) + ToRgbVector(colorMap[overlayValue])*opacity;
						pixelData[3*n] = (byte) compositeColor.X;
						pixelData[3*n + 1] = (byte) compositeColor.Y;
						pixelData[3*n + 2] = (byte) compositeColor.Z;
					});

			var dicomFile = new DicomFile();
			var dataset = dicomFile.DataSet;
			dataset[DicomTags.PatientId].SetStringValue(baseImageSopProvider.ImageSop.PatientId);
			dataset[DicomTags.PatientsName].SetStringValue(baseImageSopProvider.ImageSop.PatientsName);
			dataset[DicomTags.StudyId].SetStringValue(baseImageSopProvider.ImageSop.StudyId);
			dataset[DicomTags.StudyInstanceUid].SetStringValue(baseImageSopProvider.ImageSop.StudyInstanceUid);
			dataset[DicomTags.SeriesDescription].SetStringValue(baseImageSopProvider.ImageSop.SeriesDescription);
			dataset[DicomTags.SeriesNumber].SetInt32(0, 9001);
			dataset[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopClassUid].SetStringValue(ModalityConverter.ToSopClassUid(Modality.SC));
			dataset[DicomTags.Modality].SetStringValue("SC");
			dataset[DicomTags.FrameOfReferenceUid].SetStringValue(baseImageSopProvider.Frame.FrameOfReferenceUid);
			dataset[DicomTags.ImageOrientationPatient].SetStringValue(baseImageSopProvider.Frame.ImageOrientationPatient.ToString());
			dataset[DicomTags.ImagePositionPatient].SetStringValue(baseImageSopProvider.Frame.ImagePositionPatient.ToString());
			dataset[DicomTags.PixelSpacing].SetStringValue(baseImageSopProvider.Frame.PixelSpacing.ToString());
			dataset[DicomTags.PhotometricInterpretation].SetStringValue("RGB");
			dataset[DicomTags.SamplesPerPixel].SetInt32(0, 3);
			dataset[DicomTags.BitsStored].SetInt32(0, 8);
			dataset[DicomTags.BitsAllocated].SetInt32(0, 8);
			dataset[DicomTags.HighBit].SetInt32(0, 7);
			dataset[DicomTags.PixelRepresentation].SetInt32(0, 0);
			dataset[DicomTags.Rows].SetInt32(0, rows);
			dataset[DicomTags.Columns].SetInt32(0, cols);
			dataset[DicomTags.PixelData].Values = pixelData;
			dicomFile.MediaStorageSopClassUid = dataset[DicomTags.SopClassUid];
			dicomFile.MediaStorageSopInstanceUid = dataset[DicomTags.SopInstanceUid];

			using (var sopDataSource = new XSopDataSource(dicomFile))
			{
				return PresentationImageFactory.Create(new ImageSop(sopDataSource))[0];
			}
		}

		private static void SetFusionDisplayParameters(IPresentationImage image, IColorMap colorMap, float opacity, bool thresholding)
		{
			if (image is IColorMapProvider)
			{
				var colorMapProvider = (IColorMapProvider) image;
				colorMapProvider.ColorMapManager.InstallColorMap(colorMap);
			}

			if (image is ILayerOpacityProvider)
			{
				var layerOpacityProvider = (ILayerOpacityProvider) image;
				layerOpacityProvider.LayerOpacityManager.Thresholding = thresholding;
				layerOpacityProvider.LayerOpacityManager.Opacity = opacity;
			}
		}

		protected static Vector3D ToRgbVector(Color color)
		{
			return new Vector3D(color.R, color.G, color.B);
		}

		protected static Vector3D ToRgbVector(int argb)
		{
			return ToRgbVector(Color.FromArgb(argb));
		}

		protected static Vector3D ToRgbVectorFromGrey(ushort grey)
		{
			float v = grey*255f/ushort.MaxValue;
			return new Vector3D(v, v, v);
		}

		protected static Bitmap DrawToBitmap(IPresentationImage presentationImage)
		{
			var imageGraphicProvider = (IImageGraphicProvider) presentationImage;
			var annotationLayoutProvider = presentationImage as IAnnotationLayoutProvider;
			var annotationLayoutVisible = true;

			if (annotationLayoutProvider != null)
			{
				annotationLayoutVisible = annotationLayoutProvider.AnnotationLayout.Visible;
				annotationLayoutProvider.AnnotationLayout.Visible = false;
			}

			try
			{
				return presentationImage.DrawToBitmap(imageGraphicProvider.ImageGraphic.Columns, imageGraphicProvider.ImageGraphic.Rows);
			}
			finally
			{
				if (annotationLayoutProvider != null)
				{
					annotationLayoutProvider.AnnotationLayout.Visible = annotationLayoutVisible;
				}
			}
		}

		private class XSopDataSource : DicomMessageSopDataSource
		{
			public XSopDataSource(DicomMessageBase sourceMessage) : base(sourceMessage) {}
		}
	}
}

#endif