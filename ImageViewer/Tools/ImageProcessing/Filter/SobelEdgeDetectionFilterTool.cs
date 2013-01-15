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

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.VtkItkAdapters;
using itk;
using FilterType = itk.itkSobelEdgeDetectionImageFilter;
using intensityFilterType = itk.itkRescaleIntensityImageFilter;
using CastImageFilterType = itk.itkCastImageFilter;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuSobelEdgeDetection", "Apply")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuSobelEdgeDetection", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class SobelEdgeDetectionFilterTool : ImageViewerTool
	{
        public SobelEdgeDetectionFilterTool()
		{

		}

		public void Apply()
		{
			if (this.SelectedImageGraphicProvider == null)
				return;

			ImageGraphic image = this.SelectedImageGraphicProvider.ImageGraphic;

			if (image == null)
				return;

			if (!(image is GrayscaleImageGraphic))
				return;

            byte[] pixels = image.PixelData.Raw;

            itkImageBase input = ItkHelper.CreateItkImage(image as GrayscaleImageGraphic);
            itkImageRegion region = input.LargestPossibleRegion;

            itkImageBase output = itkImage.New(input);
            ItkHelper.CopyToItkImage(image as GrayscaleImageGraphic, input);

            string mangledType = input.MangledTypeString;
            string mangledType2 = input.PixelType.MangledTypeString;
            CastImageFilterType castToIF2 = CastImageFilterType.New(mangledType + "IF2");
            castToIF2.SetInput(input);

            FilterType filter = FilterType.New("IF2IF2");
            filter.SetInput(castToIF2.GetOutput());

            intensityFilterType intensityFilter = intensityFilterType.New("IF2" + mangledType);
            intensityFilter.SetInput(filter.GetOutput());
            intensityFilter.OutputMinimum = 0;
            if (image.BitsPerPixel == 16)
                intensityFilter.OutputMaximum = (image as GrayscaleImageGraphic).ModalityLut.MaxInputValue;
            else
                intensityFilter.OutputMaximum = 255;
            intensityFilter.Update();


            intensityFilter.GetOutput(output);
            ItkHelper.CopyFromItkImage(image as GrayscaleImageGraphic, output);
            image.Draw();

            filter.Dispose();
            intensityFilter.Dispose();
            input.Dispose();
            output.Dispose();
		}
	}
}
