using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using itk.simple;

namespace ClearCanvas.ImageViewer.ImageProcessing
{
    //[MenuAction("apply", "global-menus/MenuTools/Image Processing/Test", "Apply")]
    //[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class FilterTestTool : ImageViewerTool
    {
        public FilterTestTool()
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

            SmoothingRecursiveGaussianImageFilter guassian = new SmoothingRecursiveGaussianImageFilter();
            //input = ItkHelper.CreateItkImage(image as GrayscaleImageGraphic);
            DesktopWindow.ShowMessageBox("FilterTestTool", MessageBoxActions.Ok);
            if (this.SelectedImageGraphicProvider == null)
                return;
           

            if (image == null)
                return;

            if (!(image is GrayscaleImageGraphic))
                return;

            var x = SimpleItkHelper.CreateItkImage(image as GrayscaleImageGraphic);
        }
    }
}
