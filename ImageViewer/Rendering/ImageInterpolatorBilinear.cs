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

using System.Drawing;
using System.Runtime.InteropServices;

namespace ClearCanvas.ImageViewer.Rendering
{
    internal unsafe class ImageInterpolatorBilinear
    {
		[StructLayout(LayoutKind.Sequential)]
		public struct LutData
		{
			public int* Data;
			public int FirstMappedPixelData;
			public int Length;
		}

        public static unsafe void Interpolate(
            RectangleF srcRegionRectangle,
            byte* pSrcPixelData,
            int srcWidth,
            int srcHeight,
            int srcBytesPerPixel,
			int srcBitsStored,
            Rectangle dstRegionRectangle,
            byte* pDstPixelData,
            int dstWidth,
            int dstBytesPerPixel,
            bool swapXY,
            LutData* lutData,
            bool isRGB,
            bool isPlanar,
            bool isSigned)
        {
			InterpolateBilinear(
				pSrcPixelData, 
				srcWidth, 
				srcHeight, 
				srcBytesPerPixel,
				srcBitsStored,
				isSigned, 
				isRGB, 
				isPlanar,
				srcRegionRectangle.Left, 
				srcRegionRectangle.Top, 
				srcRegionRectangle.Right, 
				srcRegionRectangle.Bottom,
				pDstPixelData, 
				dstWidth, 
				dstBytesPerPixel,
				dstRegionRectangle.Left, 
				dstRegionRectangle.Top, 
				dstRegionRectangle.Right, 
				dstRegionRectangle.Bottom,
				swapXY, 
				lutData);
		}

		/// <summary>
		/// Import the C++ DLL that implements the fixed point bilinear interpolation method.
		/// </summary>
		[DllImport("BilinearInterpolation.dll", EntryPoint = "InterpolateBilinear", CallingConvention = CallingConvention.Cdecl)]
		private static extern int InterpolateBilinear
		(
			byte* pSrcPixelData,

			int srcWidth,
			int srcHeight,
			int srcBytesPerPixel,
			int srcBitsStored,

			bool isSigned,
			bool isRGB,
			bool isPlanar,

			float srcRegionRectLeft,
			float srcRegionRectTop,
			float srcRegionRectRight,
			float srcRegionRectBottom,

			byte* pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,

			int dstRegionRectLeft,
			int dstRegionRectTop,
			int dstRegionRectRight,
			int dstRegionRectBottom,

			bool swapXY,
			LutData* lutData
		);
    }
}

