using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using itk.simple;
using ClearCanvas.ImageViewer.Graphics;
namespace ClearCanvas.ImageViewer.ImageProcessing
{
    public static class SimpleItkHelper
    {
        public static Image CreateItkImage(GrayscaleImageGraphic imageGraphic)
        {
            PixelIDValueEnum pixelType;
            if(imageGraphic.BitsPerPixel == 16)
            {
                if (imageGraphic.IsSigned)
                {
                    pixelType = PixelIDValueEnum.sitkInt16;
                }
                else
                {
                    pixelType = PixelIDValueEnum.sitkUInt16;
                }
            }
            else
            {
                if (imageGraphic.IsSigned)
                {
                    pixelType = PixelIDValueEnum.sitkInt8;
                }
                else
                {
                    pixelType = PixelIDValueEnum.sitkUInt8;
                }
            }
            VectorUInt32 imageSize = new VectorUInt32(new uint[] { (uint)imageGraphic.Columns, (uint)imageGraphic.Rows });
            Image image = new Image(imageSize, pixelType);
            //SimpleITK.Show(image, "Test");
           
            return image;
        } 

        public static void CopyToItkImage(GrayscaleImageGraphic image, Image itkImage)
        {

        }

        private unsafe static void CopyToSigned16(ImageGraphic image, Image itkImage)
        {
            //fixed (byte* pSrcByte = image.PixelData.Raw)
            //{
            //    int height = image.Rows;
            //    int width = image.Columns;
            //    IntPtr buffer = itkImage.GetBufferAsInt16();
            //    itkImage.
            //    itkImageRegionIterator_ISS2 inputIt = new itkImageRegionIterator_ISS2(itkImage, itkImage.LargestPossibleRegion);
            //    short* pSrc = (short*)pSrcByte;
                
            //    short pixelValue;
            //    for (int y = 0; y < height; y++)
            //    {
            //        for (int x = 0; x < width; x++)
            //        {
            //            pixelValue = pSrc[0];
            //            inputIt.Set(pixelValue);
            //            pSrc++;
            //            inputIt++;
            //        }
            //    }
            //}
        }
    }
}
