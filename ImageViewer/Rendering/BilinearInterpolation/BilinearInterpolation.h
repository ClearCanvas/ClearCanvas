#pragma region License

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

#pragma endregion

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the BILINEARINTERPOLATION_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// BILINEARINTERPOLATION_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.

#ifdef BILINEARINTERPOLATION_EXPORTS
#define BILINEARINTERPOLATION_API __declspec(dllexport)
#else
#define BILINEARINTERPOLATION_API __declspec(dllimport)
#endif

struct LUTDATA
{
	int *LutData;
	int FirstMappedPixelValue;
	int Length;
};

extern "C"
{
	BILINEARINTERPOLATION_API BOOL InterpolateBilinear
	(
            BYTE* pSrcPixelData,

			unsigned int srcWidth,
            unsigned int srcHeight,
            unsigned int srcBytesPerPixel,
			unsigned int srcBitsStored,

			BOOL isSigned,
			BOOL isRGB,
			BOOL isPlanar,

			float srcRegionRectLeft,
            float srcRegionRectTop,
            float srcRegionRectRight,
            float srcRegionRectBottom,
			
            BYTE* pDstPixelData,
            unsigned int dstWidth,
            unsigned int dstBytesPerPixel,

			int dstRegionRectLeft,
            int dstRegionRectTop,
            int dstRegionRectRight,
            int dstRegionRectBottom,

			BOOL swapXY,
			LUTDATA* pLutData
	);
}
