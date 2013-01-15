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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.VtkItkAdapters;
using itk;
using FilterType = itk.itkSmoothingRecursiveGaussianImageFilter;
using intensityFilterType = itk.itkRescaleIntensityImageFilter;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuSmoothingRecursiveGaussian", "Apply")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuSmoothingRecursiveGaussian", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class SmoothingRecursiveGaussianFilterTool : ImageViewerTool
	{
        public SmoothingRecursiveGaussianFilterTool()
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

            itkImageBase input = ItkHelper.CreateItkImage(image as GrayscaleImageGraphic);
            itkImageBase output = itkImage.New(input);
            ItkHelper.CopyToItkImage(image as GrayscaleImageGraphic, input);
            
            FilterType filter = FilterType.New(input, output);
            bool abc = false;
            if (abc)
            {
                byte[] pixels = image.PixelData.Raw;
                unsafe
                {
                    byte[] dummy = new byte[512 * 512];
                    IntPtr bptr = input.Buffer;
                    void* pbptr = bptr.ToPointer();
                    {
                        filter.SetInput(bptr);
                    }
                    fixed (byte* dummyAddr = &dummy[0])
                    {
                        *dummyAddr = 1;
                        *(dummyAddr + 1) = 2;
                        IntPtr ptr = new IntPtr(dummyAddr);
                        filter.SetInput(ptr);
                    }
                    fixed (byte* pByte = image.PixelData.Raw)
                    {
                        IntPtr x = new IntPtr((void*)pByte);
                        void* p = x.ToPointer();
                        filter.SetInput(x);//runtime memory protected exception because it expects x.ToPointer() is ITK::Image_XX* (see implementation of MITK)
                    }
                }
            }
            else
            {
                filter.SetInput(input);
            }
            filter.NormalizeAcrossScale = false;
            filter.Sigma = 3;
            filter.Update();
            filter.GetOutput(output);

            filter.GetOutput(output);
            ItkHelper.CopyFromItkImage(image as GrayscaleImageGraphic, output);
            image.Draw();

            filter.Dispose();
            input.Dispose();
            output.Dispose();
		}
	}
}
