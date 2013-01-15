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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	internal class GenerateOverlayTestImagesApplication : IApplicationRoot
	{
		private class CommandLine : ClearCanvas.Common.Utilities.CommandLine
		{
			[CommandLineParameter(0, "Output Directory")]
			public string OutputDirectory { get; set; }
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new CommandLine();
			cmdLine.Parse(args);

			string path = !String.IsNullOrEmpty(cmdLine.OutputDirectory)
							? cmdLine.OutputDirectory
			              	: Path.Combine(Environment.CurrentDirectory, "OverlayTestImages");

			var testImages = new GeneratedOverlayTestImages(path);
			foreach (var file in testImages.GetAll())
				Console.WriteLine(file.Filename);
		}

		#endregion
	}

	internal class GeneratedOverlayTestImages
	{
		private readonly string _path;
		private readonly DicomAttributeCollection _baseDataset;
		private DicomFile _imageEmbeddedOverlay;
		private DicomFile _imageDataOverlay;
		private DicomFile _imageDataOverlayDifferentSize;
		private DicomFile _imageDataOverlayMultiframe;
		private DicomFile _multiframeImageEmbeddedOverlay;
		private DicomFile _multiframeImageDataOverlay;
		private DicomFile _multiframeImageDataOverlayDifferentSize;
		private DicomFile _multiframeImageDataOverlayNotMultiframe;
		private DicomFile _multiframeImageDataOverlayLowSubrangeImplicitOrigin;
		private DicomFile _multiframeImageDataOverlayLowSubrange;
		private DicomFile _multiframeImageDataOverlayMidSubrange;
		private DicomFile _multiframeImageDataOverlayHighSubrange;

		private DicomFile _imageEmbeddedOverlaySigned;
		private DicomFile _imageDataOverlaySigned;
		private DicomFile _imageDataOverlayDifferentSizeSigned;
		private DicomFile _imageDataOverlayMultiframeSigned;
		private DicomFile _multiframeImageEmbeddedOverlaySigned;
		private DicomFile _multiframeImageDataOverlaySigned;
		private DicomFile _multiframeImageDataOverlayDifferentSizeSigned;
		private DicomFile _multiframeImageDataOverlayNotMultiframeSigned;
		private DicomFile _multiframeImageDataOverlayLowSubrangeImplicitOriginSigned;
		private DicomFile _multiframeImageDataOverlayLowSubrangeSigned;
		private DicomFile _multiframeImageDataOverlayMidSubrangeSigned;
		private DicomFile _multiframeImageDataOverlayHighSubrangeSigned;

		private DicomFile _imageEmbeddedOverlay8Bit;
		private DicomFile _imageDataOverlayOWAttribute;
		private DicomFile _multiframeImageEmbeddedOverlay8Bit;
		private DicomFile _multiframeImageDataOverlayOWAttribute;

		private DicomFile _imageEmbeddedOverlay8BitSigned;
		private DicomFile _imageDataOverlayOWAttributeSigned;
		private DicomFile _multiframeImageEmbeddedOverlay8BitSigned;
		private DicomFile _multiframeImageDataOverlayOWAttributeSigned;

		public DicomFile ImageEmbeddedOverlay
		{
			get
			{
				if (_imageEmbeddedOverlay == null)
				{
					const int number = 1;
					const string description = "(unsigned) image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 1, 16, 12, 11, false),
					                                      257, 263, 1, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
					                                       OverlayType.G, new Point(1, 1), 13, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageEmbeddedOverlay = dcf;
				}
				return _imageEmbeddedOverlay;
			}
		}

		public DicomFile ImageEmbeddedOverlaySigned
		{
			get
			{
				if (_imageEmbeddedOverlaySigned == null)
				{
					const int number = 1;
					const string description = "(signed) image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 1, 16, 12, 11, true),
														  257, 263, 1, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
														   OverlayType.G, new Point(1, 1), 13, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageEmbeddedOverlaySigned = dcf;
				}
				return _imageEmbeddedOverlaySigned;
			}
		}

		public DicomFile ImageDataOverlay
		{
			get
			{
				if (_imageDataOverlay == null)
				{
					const int number = 2;
					const string description = "(unsigned) image with data overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 1, 16, 12, 11, false),
					                                      257, 263, 1, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
					                                       OverlayType.G, new Point(1, 1), 257, 263, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlay = dcf;
				}
				return _imageDataOverlay;
			}
		}

		public DicomFile ImageDataOverlaySigned
		{
			get
			{
				if (_imageDataOverlaySigned == null)
				{
					const int number = 2;
					const string description = "(signed) image with data overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 1, 16, 12, 11, true),
														  257, 263, 1, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
														   OverlayType.G, new Point(1, 1), 257, 263, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlaySigned = dcf;
				}
				return _imageDataOverlaySigned;
			}
		}

		public DicomFile ImageDataOverlayDifferentSize
		{
			get
			{
				if (_imageDataOverlayDifferentSize == null)
				{
					const int number = 3;
					const string description = "(unsigned) image with data overlay (different size)";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 1, 16, 12, 11, false),
					                                      257, 263, 1, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 1),
					                                       OverlayType.G, new Point(64, 64), 131, 137, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlayDifferentSize = dcf;
				}
				return _imageDataOverlayDifferentSize;
			}
		}

		public DicomFile ImageDataOverlayDifferentSizeSigned
		{
			get
			{
				if (_imageDataOverlayDifferentSizeSigned == null)
				{
					const int number = 3;
					const string description = "(signed) image with data overlay (different size)";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 1, 16, 12, 11, true),
														  257, 263, 1, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 1),
														   OverlayType.G, new Point(64, 64), 131, 137, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlayDifferentSizeSigned = dcf;
				}
				return _imageDataOverlayDifferentSizeSigned;
			}
		}

		public DicomFile ImageDataOverlayMultiframe
		{
			get
			{
				if (_imageDataOverlayMultiframe == null)
				{
					const int number = 4;
					const string description = "(unsigned) image with data overlay (multiframe)";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 1, 16, 12, 11, false),
					                                      257, 263, 1, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 17),
					                                       OverlayType.G, new Point(64, 64), 131, 137, 17, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlayMultiframe = dcf;
				}
				return _imageDataOverlayMultiframe;
			}
		}

		public DicomFile ImageDataOverlayMultiframeSigned
		{
			get
			{
				if (_imageDataOverlayMultiframeSigned == null)
				{
					const int number = 4;
					const string description = "(signed) image with data overlay (multiframe)";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 1, 16, 12, 11, true),
														  257, 263, 1, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 17),
														   OverlayType.G, new Point(64, 64), 131, 137, 17, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlayMultiframeSigned = dcf;
				}
				return _imageDataOverlayMultiframeSigned;
			}
		}

		public DicomFile MultiframeImageEmbeddedOverlay
		{
			get
			{
				if (_multiframeImageEmbeddedOverlay == null)
				{
					const int number = 5;
					const string description = "(unsigned) multiframe image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
					                                       OverlayType.G, new Point(1, 1), 13, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageEmbeddedOverlay = dcf;
				}
				return _multiframeImageEmbeddedOverlay;
			}
		}

		public DicomFile MultiframeImageEmbeddedOverlaySigned
		{
			get
			{
				if (_multiframeImageEmbeddedOverlaySigned == null)
				{
					const int number = 5;
					const string description = "(signed) multiframe image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
														   OverlayType.G, new Point(1, 1), 13, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageEmbeddedOverlaySigned = dcf;
				}
				return _multiframeImageEmbeddedOverlaySigned;
			}
		}

		public DicomFile MultiframeImageDataOverlay
		{
			get
			{
				if (_multiframeImageDataOverlay == null)
				{
					const int number = 6;
					const string description = "(unsigned) multiframe image with data overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
					                                       OverlayType.G, new Point(1, 1), 257, 263, 17, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlay = dcf;
				}
				return _multiframeImageDataOverlay;
			}
		}

		public DicomFile MultiframeImageDataOverlaySigned
		{
			get
			{
				if (_multiframeImageDataOverlaySigned == null)
				{
					const int number = 6;
					const string description = "(signed) multiframe image with data overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
														   OverlayType.G, new Point(1, 1), 257, 263, 17, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlaySigned = dcf;
				}
				return _multiframeImageDataOverlaySigned;
			}
		}

		public DicomFile MultiframeImageDataOverlayDifferentSize
		{
			get
			{
				if (_multiframeImageDataOverlayDifferentSize == null)
				{
					const int number = 7;
					const string description = "(unsigned) multiframe image with data overlay (different size)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 17),
					                                       OverlayType.G, new Point(64, 64), 131, 137, 17, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayDifferentSize = dcf;
				}
				return _multiframeImageDataOverlayDifferentSize;
			}
		}

		public DicomFile MultiframeImageDataOverlayDifferentSizeSigned
		{
			get
			{
				if (_multiframeImageDataOverlayDifferentSizeSigned == null)
				{
					const int number = 7;
					const string description = "(signed) multiframe image with data overlay (different size)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 17),
														   OverlayType.G, new Point(64, 64), 131, 137, 17, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayDifferentSizeSigned = dcf;
				}
				return _multiframeImageDataOverlayDifferentSizeSigned;
			}
		}

		public DicomFile MultiframeImageDataOverlayNotMultiframe
		{
			get
			{
				if (_multiframeImageDataOverlayNotMultiframe == null)
				{
					const int number = 8;
					const string description = "(unsigned) multiframe image with data overlay (non-multiframe)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 1),
					                                       OverlayType.G, new Point(64, 64), 131, 137, null, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayNotMultiframe = dcf;
				}
				return _multiframeImageDataOverlayNotMultiframe;
			}
		}

		public DicomFile MultiframeImageDataOverlayNotMultiframeSigned
		{
			get
			{
				if (_multiframeImageDataOverlayNotMultiframeSigned == null)
				{
					const int number = 8;
					const string description = "(signed) multiframe image with data overlay (non-multiframe)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 1),
														   OverlayType.G, new Point(64, 64), 131, 137, null, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayNotMultiframeSigned = dcf;
				}
				return _multiframeImageDataOverlayNotMultiframeSigned;
			}
		}

		public DicomFile MultiframeImageDataOverlayLowSubrangeImplicitOrigin
		{
			get
			{
				if (_multiframeImageDataOverlayLowSubrangeImplicitOrigin == null)
				{
					const int number = 9;
					const string description = "(unsigned) multiframe image with data overlay (start-7)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
					                                       OverlayType.G, new Point(64, 64), 131, 137, 7, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayLowSubrangeImplicitOrigin = dcf;
				}
				return _multiframeImageDataOverlayLowSubrangeImplicitOrigin;
			}
		}

		public DicomFile MultiframeImageDataOverlayLowSubrangeImplicitOriginSigned
		{
			get
			{
				if (_multiframeImageDataOverlayLowSubrangeImplicitOriginSigned == null)
				{
					const int number = 9;
					const string description = "(signed) multiframe image with data overlay (start-7)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
														   OverlayType.G, new Point(64, 64), 131, 137, 7, null, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayLowSubrangeImplicitOriginSigned = dcf;
				}
				return _multiframeImageDataOverlayLowSubrangeImplicitOriginSigned;
			}
		}

		public DicomFile MultiframeImageDataOverlayLowSubrange
		{
			get
			{
				if (_multiframeImageDataOverlayLowSubrange == null)
				{
					const int number = 10;
					const string description = "(unsigned) multiframe image with data overlay (1-7)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
					                                       OverlayType.G, new Point(64, 64), 131, 137, 7, 1, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayLowSubrange = dcf;
				}
				return _multiframeImageDataOverlayLowSubrange;
			}
		}

		public DicomFile MultiframeImageDataOverlayLowSubrangeSigned
		{
			get
			{
				if (_multiframeImageDataOverlayLowSubrangeSigned == null)
				{
					const int number = 10;
					const string description = "(signed) multiframe image with data overlay (1-7)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
														   OverlayType.G, new Point(64, 64), 131, 137, 7, 1, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayLowSubrangeSigned = dcf;
				}
				return _multiframeImageDataOverlayLowSubrangeSigned;
			}
		}

		public DicomFile MultiframeImageDataOverlayMidSubrange
		{
			get
			{
				if (_multiframeImageDataOverlayMidSubrange == null)
				{
					const int number = 11;
					const string description = "(unsigned) multiframe image with data overlay (6-12)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
					                                       OverlayType.G, new Point(64, 64), 131, 137, 7, 6, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayMidSubrange = dcf;
				}
				return _multiframeImageDataOverlayMidSubrange;
			}
		}

		public DicomFile MultiframeImageDataOverlayMidSubrangeSigned
		{
			get
			{
				if (_multiframeImageDataOverlayMidSubrangeSigned == null)
				{
					const int number = 11;
					const string description = "(signed) multiframe image with data overlay (6-12)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
														   OverlayType.G, new Point(64, 64), 131, 137, 7, 6, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayMidSubrangeSigned = dcf;
				}
				return _multiframeImageDataOverlayMidSubrangeSigned;
			}
		}

		public DicomFile MultiframeImageDataOverlayHighSubrange
		{
			get
			{
				if (_multiframeImageDataOverlayHighSubrange == null)
				{
					const int number = 12;
					const string description = "(unsigned) multiframe image with data overlay (11-17)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
					                                       OverlayType.G, new Point(64, 64), 131, 137, 7, 11, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayHighSubrange = dcf;
				}
				return _multiframeImageDataOverlayHighSubrange;
			}
		}

		public DicomFile MultiframeImageDataOverlayHighSubrangeSigned
		{
			get
			{
				if (_multiframeImageDataOverlayHighSubrangeSigned == null)
				{
					const int number = 12;
					const string description = "(signed) multiframe image with data overlay (11-17)";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateSmallOverlayFrame, 7),
														   OverlayType.G, new Point(64, 64), 131, 137, 7, 11, false, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayHighSubrangeSigned = dcf;
				}
				return _multiframeImageDataOverlayHighSubrangeSigned;
			}
		}

		public DicomFile ImageEmbeddedOverlay8Bit
		{
			get
			{
				if (_imageEmbeddedOverlay8Bit == null)
				{
					const int number = 13;
					const string description = "(unsigned) 8-bit image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 1, 8, 7, 7, false),
					                                      257, 263, 1, 8, 7, 7, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
					                                       OverlayType.G, new Point(1, 1), 0, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageEmbeddedOverlay8Bit = dcf;
				}
				return _imageEmbeddedOverlay8Bit;
			}
		}

		public DicomFile ImageEmbeddedOverlay8BitSigned
		{
			get
			{
				if (_imageEmbeddedOverlay8BitSigned == null)
				{
					const int number = 13;
					const string description = "(signed) 8-bit image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 1, 8, 7, 7, true),
														  257, 263, 1, 8, 7, 7, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
														   OverlayType.G, new Point(1, 1), 0, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageEmbeddedOverlay8BitSigned = dcf;
				}
				return _imageEmbeddedOverlay8BitSigned;
			}
		}

		public DicomFile ImageDataOverlayOWAttribute
		{
			get
			{
				if (_imageDataOverlayOWAttribute == null)
				{
					const int number = 14;
					const string description = "(unsigned) image with OW data overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 1, 16, 12, 11, false),
					                                      257, 263, 1, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
					                                       OverlayType.G, new Point(1, 1), 257, 263, false, true);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlayOWAttribute = dcf;
				}
				return _imageDataOverlayOWAttribute;
			}
		}

		public DicomFile ImageDataOverlayOWAttributeSigned
		{
			get
			{
				if (_imageDataOverlayOWAttributeSigned == null)
				{
					const int number = 14;
					const string description = "(signed) image with OW data overlay";

					var dcf = CreateInstance(number, description, SopClass.SecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 1, 16, 12, 11, true),
														  257, 263, 1, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 1),
														   OverlayType.G, new Point(1, 1), 257, 263, false, true);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_imageDataOverlayOWAttributeSigned = dcf;
				}
				return _imageDataOverlayOWAttributeSigned;
			}
		}

		public DicomFile MultiframeImageEmbeddedOverlay8Bit
		{
			get
			{
				if (_multiframeImageEmbeddedOverlay8Bit == null)
				{
					const int number = 15;
					const string description = "(unsigned) 8-bit multiframe image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 8, 7, 7, false),
					                                      257, 263, 17, 8, 7, 7, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
					                                       OverlayType.G, new Point(1, 1), 0, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageEmbeddedOverlay8Bit = dcf;
				}
				return _multiframeImageEmbeddedOverlay8Bit;
			}
		}

		public DicomFile MultiframeImageEmbeddedOverlay8BitSigned
		{
			get
			{
				if (_multiframeImageEmbeddedOverlay8BitSigned == null)
				{
					const int number = 15;
					const string description = "(signed) 8-bit multiframe image with embedded overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 8, 7, 7, true),
														  257, 263, 17, 8, 7, 7, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
														   OverlayType.G, new Point(1, 1), 0, false);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageEmbeddedOverlay8BitSigned = dcf;
				}
				return _multiframeImageEmbeddedOverlay8BitSigned;
			}
		}

		public DicomFile MultiframeImageDataOverlayOWAttribute
		{
			get
			{
				if (_multiframeImageDataOverlayOWAttribute == null)
				{
					const int number = 16;
					const string description = "(unsigned) multiframe image with OW data overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
					                                      CreatePixelData(CreateImageFrame, 17, 16, 12, 11, false),
					                                      257, 263, 17, 16, 12, 11, false);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
					                                       OverlayType.G, new Point(1, 1), 257, 263, 17, null, false, true);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayOWAttribute = dcf;
				}
				return _multiframeImageDataOverlayOWAttribute;
			}
		}

		public DicomFile MultiframeImageDataOverlayOWAttributeSigned
		{
			get
			{
				if (_multiframeImageDataOverlayOWAttributeSigned == null)
				{
					const int number = 16;
					const string description = "(signed) multiframe image with OW data overlay";

					var dcf = CreateInstance(number, description, SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage, _baseDataset);
					DicomOverlayTestHelper.SetImagePixels(dcf.DataSet,
														  CreatePixelData(CreateImageFrame, 17, 16, 12, 11, true),
														  257, 263, 17, 16, 12, 11, true);
					DicomOverlayTestHelper.AddOverlayPlane(dcf.DataSet, 0, CreateOverlayData(CreateLargeOverlayFrame, 17),
														   OverlayType.G, new Point(1, 1), 257, 263, 17, null, false, true);
					dcf.Save(Path.Combine(_path, string.Format("{0}.dcm", description.Replace(' ', '_'))));
					_multiframeImageDataOverlayOWAttributeSigned = dcf;
				}
				return _multiframeImageDataOverlayOWAttributeSigned;
			}
		}

		public GeneratedOverlayTestImages()
			: this(Path.Combine(Environment.CurrentDirectory, typeof(GeneratedOverlayTestImages).FullName))
		{
		}

		public GeneratedOverlayTestImages(string path)
		{
			_path = path;
			if (Directory.Exists(_path))
				Directory.Delete(_path, true);
			Directory.CreateDirectory(_path);

			_baseDataset = CreateBaseDataSet();
		}

		public IEnumerable<DicomFile> GetAll()
		{
			yield return ImageDataOverlay;
			yield return ImageDataOverlayDifferentSize;
			yield return ImageDataOverlayMultiframe;
			yield return ImageDataOverlayOWAttribute;
			yield return ImageEmbeddedOverlay;
			yield return ImageEmbeddedOverlay8Bit;
			yield return MultiframeImageDataOverlay;
			yield return MultiframeImageDataOverlayDifferentSize;
			yield return MultiframeImageDataOverlayHighSubrange;
			yield return MultiframeImageDataOverlayLowSubrange;
			yield return MultiframeImageDataOverlayLowSubrangeImplicitOrigin;
			yield return MultiframeImageDataOverlayMidSubrange;
			yield return MultiframeImageDataOverlayNotMultiframe;
			yield return MultiframeImageDataOverlayOWAttribute;
			yield return MultiframeImageEmbeddedOverlay;
			yield return MultiframeImageEmbeddedOverlay8Bit;

			yield return ImageDataOverlaySigned;
			yield return ImageDataOverlayDifferentSizeSigned;
			yield return ImageDataOverlayMultiframeSigned;
			yield return ImageDataOverlayOWAttributeSigned;
			yield return ImageEmbeddedOverlaySigned;
			yield return ImageEmbeddedOverlay8BitSigned;
			yield return MultiframeImageDataOverlaySigned;
			yield return MultiframeImageDataOverlayDifferentSizeSigned;
			yield return MultiframeImageDataOverlayHighSubrangeSigned;
			yield return MultiframeImageDataOverlayLowSubrangeSigned;
			yield return MultiframeImageDataOverlayLowSubrangeImplicitOriginSigned;
			yield return MultiframeImageDataOverlayMidSubrangeSigned;
			yield return MultiframeImageDataOverlayNotMultiframeSigned;
			yield return MultiframeImageDataOverlayOWAttributeSigned;
			yield return MultiframeImageEmbeddedOverlaySigned;
			yield return MultiframeImageEmbeddedOverlay8BitSigned;
		}

		private delegate float[] FrameDataGetter(int frameNumber);

		private static byte[] CreatePixelData(FrameDataGetter frameDataGetter, int frameCount, int bitsAllocated, int bitsStored, int highBit, bool isSigned)
		{
			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("bitsAllocated must be either 8 or 16.", "bitsAllocated");

			byte[] pixelData = null;
			for (int n = 0; n < frameCount; n++)
			{
				var frameData = frameDataGetter.Invoke(n + 1);
				if (pixelData == null)
				{
					pixelData = new byte[frameData.Length*frameCount*bitsAllocated/8];
				}

				int min = DicomPixelData.GetMinPixelValue(bitsStored, isSigned);
				int max = DicomPixelData.GetMaxPixelValue(bitsStored, isSigned);

				if (bitsAllocated == 16)
				{
					var cursor = frameData.Length*n*2;
					for (int i = 0; i < frameData.Length; i++)
					{
						var value = Math.Max(min, Math.Min(max, (int)(min + (max - min) * frameData[i])));
						value = value << (highBit - bitsStored + 1);

						if (ByteBuffer.LocalMachineEndian == Endian.Little)
						{
							pixelData[cursor++] = (byte) (value & 0x00FF);
							pixelData[cursor++] = (byte) ((value >> 8) & 0x00FF);
						}
						else
						{
							pixelData[cursor++] = (byte) ((value >> 8) & 0x00FF);
							pixelData[cursor++] = (byte) (value & 0x00FF);
						}
					}
				}
				else if (bitsAllocated == 8)
				{
					var cursor = frameData.Length*n;
					for (int i = 0; i < frameData.Length; i++)
					{
						var value = Math.Max(min, Math.Min(max, (int)(min + (max - min) * frameData[i])));
						value = value << (highBit - bitsStored + 1);
						pixelData[cursor++] = (byte) (value & 0x00FF);
					}
				}
			}

			//just in case, we'll clean the pixel data up.  This also (cheaply) removes the 
			//higher order 1s that shouldn't be there for the signed case.
			DicomUncompressedPixelData.ZeroUnusedBits(pixelData, bitsAllocated, bitsStored, highBit);
			return pixelData;
		}

		private static bool[] CreateOverlayData(FrameDataGetter frameDataGetter, int frameCount)
		{
			bool[] overlayData = null;
			for (int n = 0; n < frameCount; n++)
			{
				var frameData = frameDataGetter.Invoke(n + 1);
				if (overlayData == null)
				{
					overlayData = new bool[frameData.Length*frameCount];
				}

				var cursor = frameData.Length*n;
				for (int i = 0; i < frameData.Length; i++)
				{
					overlayData[cursor++] = frameData[i] > 0.5f;
				}
			}
			return overlayData;
		}

		/// <summary>
		/// Creates a 263x257 image frame.
		/// </summary>
		private static float[] CreateImageFrame(int frameNumber)
		{
			const int rows = 257;
			const int cols = 263;

			var data = new float[rows*cols];
			using (var bmp = new Bitmap(cols, rows, PixelFormat.Format32bppArgb))
			{
				using (var g = System.Drawing.Graphics.FromImage(bmp))
				{
					g.DrawLine(Pens.DarkGray, 87, 0, 87, rows);
					g.DrawLine(Pens.DarkGray, 175, 0, 175, rows);
					g.DrawLine(Pens.DarkGray, 0, 85, cols, 85);
					g.DrawLine(Pens.DarkGray, 0, 171, cols, 171);

					g.DrawLine(Pens.DarkGray, 105, 171, 105, 217);
					g.DrawLine(Pens.DarkGray, 122, 171, 122, 217);
					g.DrawLine(Pens.DarkGray, 140, 171, 140, 217);
					g.DrawLine(Pens.DarkGray, 157, 171, 157, 217);
					g.DrawLine(Pens.DarkGray, 87, 217, 175, 217);

					// print frame number barcode
					if ((frameNumber >> 0)%2 == 1)
						g.FillRectangle(Brushes.White, 157, 194, 18, 23);
					if ((frameNumber >> 1)%2 == 1)
						g.FillRectangle(Brushes.White, 140, 194, 17, 23);
					if ((frameNumber >> 2)%2 == 1)
						g.FillRectangle(Brushes.White, 122, 194, 18, 23);
					if ((frameNumber >> 3)%2 == 1)
						g.FillRectangle(Brushes.White, 105, 194, 17, 23);
					if ((frameNumber >> 4)%2 == 1)
						g.FillRectangle(Brushes.White, 88, 194, 17, 23);

					using (var f = new Font(FontFamily.GenericSansSerif, 14, GraphicsUnit.Point))
					{
						using (var sf = new StringFormat())
						{
							sf.Alignment = StringAlignment.Near;
							sf.LineAlignment = StringAlignment.Center;
							g.DrawString(string.Format("Image Fr#{0}", frameNumber), f, Brushes.White, new RectangleF(63, 0, 137, 85), sf);
						}
					}
				}

				for (int n = 0; n < data.Length; n++)
				{
					data[n] = bmp.GetPixel(n%cols, n/cols).GetBrightness();
				}
			}
			return data;
		}

		/// <summary>
		/// Creates a 263x257 overlay frame.
		/// </summary>
		private static float[] CreateLargeOverlayFrame(int frameNumber)
		{
			const int rows = 257;
			const int cols = 263;

			var data = new float[rows*cols];
			using (var bmp = new Bitmap(cols, rows, PixelFormat.Format32bppArgb))
			{
				using (var g = System.Drawing.Graphics.FromImage(bmp))
				{
					g.FillRectangle(Brushes.White, 176, 63, 24, 22);
					g.FillRectangle(Brushes.White, 63, 172, 24, 22);

					// print frame number barcode
					if ((frameNumber >> 0)%2 == 1)
						g.FillRectangle(Brushes.White, 157, 172, 18, 23);
					if ((frameNumber >> 1)%2 == 1)
						g.FillRectangle(Brushes.White, 140, 172, 17, 23);
					if ((frameNumber >> 2)%2 == 1)
						g.FillRectangle(Brushes.White, 122, 172, 18, 23);
					if ((frameNumber >> 3)%2 == 1)
						g.FillRectangle(Brushes.White, 105, 172, 17, 23);
					if ((frameNumber >> 4)%2 == 1)
						g.FillRectangle(Brushes.White, 88, 172, 17, 23);

					using (var f = new Font(FontFamily.GenericSansSerif, 14, GraphicsUnit.Point))
					{
						using (var sf = new StringFormat())
						{
							sf.Alignment = StringAlignment.Near;
							sf.LineAlignment = StringAlignment.Center;
							g.DrawString(string.Format("Overlay Fr#{0}", frameNumber), f, Brushes.White, new RectangleF(63, 85, 137, 86), sf);
						}
					}
				}

				for (int n = 0; n < data.Length; n++)
				{
					data[n] = bmp.GetPixel(n%cols, n/cols).GetBrightness();
				}
			}
			return data;
		}

		/// <summary>
		/// Creates a 137x131 (image offset of 64,64) image frame.
		/// </summary>
		private static float[] CreateSmallOverlayFrame(int frameNumber)
		{
			const int rows = 131;
			const int cols = 137;

			var data = new float[rows*cols];
			using (var bmp = new Bitmap(cols, rows, PixelFormat.Format32bppArgb))
			{
				using (var g = System.Drawing.Graphics.FromImage(bmp))
				{
					g.FillRectangle(Brushes.White, 113, 0, 24, 22);
					g.FillRectangle(Brushes.White, 0, 109, 24, 22);

					// print frame index barcode
					if ((frameNumber >> 0)%2 == 1)
						g.FillRectangle(Brushes.White, 94, 109, 18, 23);
					if ((frameNumber >> 1)%2 == 1)
						g.FillRectangle(Brushes.White, 77, 109, 17, 23);
					if ((frameNumber >> 2)%2 == 1)
						g.FillRectangle(Brushes.White, 59, 109, 18, 23);
					if ((frameNumber >> 3)%2 == 1)
						g.FillRectangle(Brushes.White, 42, 109, 17, 23);
					if ((frameNumber >> 4)%2 == 1)
						g.FillRectangle(Brushes.White, 25, 109, 17, 23);

					using (var f = new Font(FontFamily.GenericSansSerif, 14, GraphicsUnit.Point))
					{
						using (var sf = new StringFormat())
						{
							sf.Alignment = StringAlignment.Near;
							sf.LineAlignment = StringAlignment.Center;
							g.DrawString(string.Format("Overlay Fr#{0}", frameNumber), f, Brushes.White, new RectangleF(0, 22, cols, 86), sf);
						}
					}
				}

				for (int n = 0; n < data.Length; n++)
				{
					data[n] = bmp.GetPixel(n%cols, n/cols).GetBrightness();
				}
			}
			return data;
		}

		private static DicomFile CreateInstance(int number, string description, SopClass sopClass, DicomAttributeCollection baseDataSet)
		{
			DateTime now = DateTime.Now;
			DicomUid sopInstanceUid = DicomUid.GenerateUid();
			DicomFile dicomFile = new DicomFile(string.Empty, new DicomAttributeCollection(), baseDataSet.Copy(true, true, true));

			//meta info
			dicomFile.MediaStorageSopInstanceUid = sopInstanceUid.UID;
			dicomFile.MediaStorageSopClassUid = sopClass.Uid;

			//Series
			dicomFile.DataSet[DicomTags.Modality].SetStringValue("OT");
			dicomFile.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dicomFile.DataSet[DicomTags.SeriesNumber].SetInt32(0, number);
			dicomFile.DataSet[DicomTags.SeriesDescription].SetStringValue(description);

			//General Image
			dicomFile.DataSet[DicomTags.InstanceNumber].SetInt32(0, 1);

			//SC Image
			dicomFile.DataSet[DicomTags.DateOfSecondaryCapture].SetDateTime(0, now);
			dicomFile.DataSet[DicomTags.TimeOfSecondaryCapture].SetDateTime(0, now);

			//Sop Common
			dicomFile.DataSet[DicomTags.InstanceCreationDate].SetDateTime(0, now);
			dicomFile.DataSet[DicomTags.InstanceCreationTime].SetDateTime(0, now);
			dicomFile.DataSet[DicomTags.SopClassUid].SetStringValue(sopClass.Uid);
			dicomFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid.UID);

			return dicomFile;
		}

		private static DicomAttributeCollection CreateBaseDataSet()
		{
			const string patientId = "8940353";
			const string patientName = "KIRK^JAMES^TIBERIUS";
			var now = DateTime.Now;

			var baseDataSet = new DicomAttributeCollection();

			//Patient
			baseDataSet[DicomTags.PatientId].SetStringValue(patientId);
			baseDataSet[DicomTags.PatientsName].SetStringValue(patientName);

			baseDataSet[DicomTags.PatientsBirthDate].SetDateTime(0, new DateTime(2233, 03, 22));
			baseDataSet[DicomTags.PatientsSex].SetStringValue("M");

			//Study
			baseDataSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			baseDataSet[DicomTags.StudyDate].SetDateTime(0, now);
			baseDataSet[DicomTags.StudyTime].SetDateTime(0, now);
			baseDataSet[DicomTags.AccessionNumber].SetStringValue("A8940353");
			baseDataSet[DicomTags.StudyDescription].SetStringValue("generated overlay test images");
			baseDataSet[DicomTags.ReferringPhysiciansName].SetNullValue();
			baseDataSet[DicomTags.StudyId].SetStringValue("S8940353");

			//SC Equipment
			baseDataSet[DicomTags.ConversionType].SetStringValue("WSD");

			//General Image
			baseDataSet[DicomTags.ImageType].SetStringValue(@"DERIVED\SECONDARY");
			baseDataSet[DicomTags.PatientOrientation].SetNullValue();

			return baseDataSet;
		}
	}
}

#endif