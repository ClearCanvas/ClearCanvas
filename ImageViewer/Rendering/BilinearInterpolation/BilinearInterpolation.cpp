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

// ClearCanvas.ImageViewer.Renderer.BilinearInterpolation.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "BilinearInterpolation.h"
#include "math.h"
#include <memory>

#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

using std::auto_ptr;

#define SLIGHTLYGREATERTHANONE 1.001F
#define FIXEDPRECISION 7
#define FIXEDSCALE 128.0F

///////////////////////////////////////////////////////////////////////
///
/// Fixed Point (fast) bilinear interpolation algorithm.  Except
/// for the inner (x) loop, the function is the same as the RGB
/// function, but they have been implemented separately for the sake
/// of speed.  Basically, we don't want to have to check isRGB 
/// within the x-loop for every single pixel since it is unnecessary -
/// we only want to perform the operations that are absolutely
/// necessary within the inner (x) loop.
///
/// Some notes about fixed point math:
///   - Fixed point math is a way of performing floating point
///     arithmetic with integers.
///   - Take, for example, a 16 bit image where the bits stored is 16.
///     We need to be sure that we won't overflow the 32 bit integer
///     in which we are calculating the fixed point results. 
///   - Basically, a fixed point# is represented as an integer in the 
///     format I.F, where I is the number of bits representing the 
///     integer portion of the number, and F represents the fractional
///     part.
///   - When multiplying or dividing, the decimal place gets shifted to
///     the left depending on the # of fractional bits in the numbers
///     being multiplied together.  For example, if we multiply a 16.8
///     (I.F) number by another 16.8 number, we get a 32.16 (48 bits!!)
///     number as a result.
///   - In the function below, you will notice that 7 bits are used 
///     for the fractional portion of the #.  This is because we can 
///     guarantee that the dx and dy values are between 0 and 1, so
///     they are 0.7 #s.  And the pixel values (depending on the #bits
///     stored) CAN be as large as 16 bits.  Therefore, if we multiply
///     #s like this together, we get:
///            16.7 * 0.7 = 16.14 (30 bits).
///     Leaving 1 bit for the (potential) sign of the pixel value makes
///     the result a 31 bit number.  Therefore, 7 is the maximum value
///     we can use for this type of calculation without using a 64 bit
///     integer (this slows things down a lot!).
///   - Basically, what this means if we use 7 bits for the fractional
///     part of the #, is that we can only calculate #s to the resolution
///     of 1/128 ~ 0.007.  So, you can think of this interpolation
///     algorithm as one that subdivides each pixel into a 128x128 grid
///     and interpolates using a finite fraction for dx and dy 
///     (e.g. 1/128, 2/128, 3/128).
///   - What this means is that the algorithm is not a 'true' bilinear
///     interpolation algorithm, but for the most part it is very close.
///     The maximum absolute error can be calculated as follows:
///
///
///     Consider the following neighbourhood of 4 pixels:
///                  0      0
///              65535  65535
///
///     where dx = dy = 0.992187 (just less than 127/128)
///     
///     Using floating point arithmetic, 
///
///     yInterpolated1 = 65535 + (0-65535)*(0.992187) = 512.025
///     yInterpolated1 = 65535 + (0-65535)*(0.992187) = 512.025
///     yInterpolated  = 512.025
///
///     Using fixed point,
///
///     yInterpolated1 = 65535 + (0-65535)*(126/128) = 1023.984
///     yInterpolated1 = 65535 + (0-65535)*(126/128) = 1023.984
///     yInterpolated  = 1023.984
///
///     For this extreme case, we are out by ~512, which is less than
///     1% of the total range of pixel values.  This is a very extreme
///     case - in most situations, neighbouring pixel values are much
///     closer together than this and result in a much smaller error.
///
///     Consider also that the purpose of doing interpolation is simply
///     to smooth out the image.  Whether or not the algorithm 
///     does the calculation *exactly* is pretty much irrelevant.  The
///     interpolated data is only an approximation, and in most
///     situations (except perhaps at a sharp edge boundary) this 
///     approximation is reasonably accurate.
///
///     The error also decreases as the bits stored of the image
///     decreases.  For example, for a 14 bit image, the error would
///     be 512/4 = 128 for the extreme case shown above.
///
/// Notes about signed representation in DICOM:
///
/// In DICOM, negative numbers are represented as two's complement
/// where the sign bit is the high bit, *not* the most significant bit.
/// For example, in a 9-bit signed image, -1 would be represented as 
/// 0000 0001 1111 1111, *not* 1111 1111 1111 1111. For the interpolation
/// arithmetic, this representation is problematic, so we
/// convert the DICOM representation to standard representation by
/// padding bits above the high bit with 1's.  In the example above,
/// this means that -1 would be changed from 0000 0001 1111 1111 to
/// 1111 1111 1111 1111.
///
///////////////////////////////////////////////////////////////////////

void InterpolateBilinearUnsigned8(

		BYTE* pDstPixelData,

		float dstRegionWidth,
		float dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		BYTE* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		float srcRegionOriginY,

		float xRatio,
		float yRatio,
		LUTDATA* pLutData,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{
	// NY: Bug #295: When I originally changed this method so that
	// int pointers are used instead of byte pointers, I simply
	// incremented the pointer by 1 to go to the next pixel.  That obviously
	// doesn't work when the image has been rotated 90 deg.  And so we need
	// to use xDstIncrement when incrementing the pointer.  Problem is though,
	// xDstIncrement is in bytes.  So to compensate, we divide xDstIncrement
	// by 4 (i.e. right shift 2 bits) to get the increment in ints.
	xDstIncrement = xDstIncrement >> 2;

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;
	int ySrcPixel, dyFixed;
	int* pRowDstPixelData;
	BYTE* pRowSrcPixelData;
	int *pxPixel, *pdxFixed;
	BYTE *pSrcPixel00, *pSrcPixel01, *pSrcPixel10, *pSrcPixel11;
	int yInterpolated1, yInterpolated2, finalInterpolated;

	for (float y = 0; y < dstRegionHeight; ++y)  //so we're not constantly converting ints to floats.
	{
		float ySrcCoordinate = srcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		ySrcPixel = (int)ySrcCoordinate;
		dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		pRowDstPixelData = (int*)pDstPixelData;
		pRowSrcPixelData = pSrcPixelData + ySrcPixel * srcWidth;
	    
		pxPixel = spxSrcPixels.get();
		pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
			pSrcPixel00 = pRowSrcPixelData + (*pxPixel);
			pSrcPixel01 = pSrcPixel00 + 1;
			pSrcPixel10 = pSrcPixel00 + srcWidth;
			pSrcPixel11 = pSrcPixel10 + 1;
			
			//wherever you multiply, you have to downshift again to keep the decimal precision of the #s the same.
			yInterpolated1 = ((*pSrcPixel00) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
			yInterpolated2 = ((*pSrcPixel01) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);

			finalInterpolated = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;
			*pRowDstPixelData = *(pLutData->LutData + finalInterpolated - pLutData->FirstMappedPixelValue);

			pRowDstPixelData += xDstIncrement;

			++pxPixel;
			++pdxFixed;
		}

		pDstPixelData += yDstIncrement;
	}
}

void InterpolateBilinearSigned8(

		BYTE* pDstPixelData,

		float dstRegionWidth,
		float dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		char* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		unsigned int srcBitsStored,
		float srcRegionOriginY,

		float xRatio,
		float yRatio,
		LUTDATA* pLutData,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{
	// NY: Bug #295: When I originally changed this method so that
	// int pointers are used instead of byte pointers, I simply
	// incremented the pointer by 1 to go to the next pixel.  That obviously
	// doesn't work when the image has been rotated 90 deg.  And so we need
	// to use xDstIncrement when incrementing the pointer.  Problem is though,
	// xDstIncrement is in bytes.  So to compensate, we divide xDstIncrement
	// by 4 (i.e. right shift 2 bits) to get the increment in ints.
	xDstIncrement = xDstIncrement >> 2;

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;
	int ySrcPixel, dyFixed;
	int* pRowDstPixelData;
	char* pRowSrcPixelData;
	int *pxPixel, *pdxFixed;
	char *pSrcPixel00, *pSrcPixel01, *pSrcPixel10, *pSrcPixel11;
	char srcPixel00, srcPixel01, srcPixel10, srcPixel11;
	int yInterpolated1, yInterpolated2, finalInterpolated;

	// Mask used to determine if a pixel value is signed or not.  Note that the
	// sign bit is the high bit.  Thus, if the bits stored = 9, the sign bit is 8
	char signMask = (char)(1 << (srcBitsStored - 1));

	// Used to turn a signed pixel value of arbitrary bit depth into a 8 bit signed equivalent
	char signPadding = (char)(0xff << (srcBitsStored - 1));

	for (float y = 0; y < dstRegionHeight; ++y)  //so we're not constantly converting ints to floats.
	{
		float ySrcCoordinate = srcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		ySrcPixel = (int)ySrcCoordinate;
		dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		pRowDstPixelData = (int*)pDstPixelData;
		pRowSrcPixelData = pSrcPixelData + ySrcPixel * srcWidth;
	    
		pxPixel = spxSrcPixels.get();
		pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
			pSrcPixel00 = pRowSrcPixelData + (*pxPixel);
			pSrcPixel01 = pSrcPixel00 + 1;
			pSrcPixel10 = pSrcPixel00 + srcWidth;
			pSrcPixel11 = pSrcPixel10 + 1;
			
			if ((*pSrcPixel00 & signMask) != 0)
				srcPixel00 = *pSrcPixel00 | signPadding;
			else
				srcPixel00 = *pSrcPixel00;

			if ((*pSrcPixel01 & signMask) != 0)
				srcPixel01 = *pSrcPixel01 | signPadding;
			else
				srcPixel01 = *pSrcPixel01;

			if ((*pSrcPixel10 & signMask) != 0)
				srcPixel10 = *pSrcPixel10 | signPadding;
			else
				srcPixel10 = *pSrcPixel10;

			if ((*pSrcPixel11 & signMask) != 0)
				srcPixel11 = *pSrcPixel11 | signPadding;
			else
				srcPixel11 = *pSrcPixel11;

			//wherever you multiply, you have to downshift again to keep the decimal precision of the #s the same.
			yInterpolated1 = ((srcPixel00) << FIXEDPRECISION) + ((dyFixed * ((srcPixel10 - srcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
			yInterpolated2 = ((srcPixel01) << FIXEDPRECISION) + ((dyFixed * ((srcPixel11 - srcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);

			finalInterpolated = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;
			*pRowDstPixelData = *(pLutData->LutData + finalInterpolated - pLutData->FirstMappedPixelValue);

			pRowDstPixelData += xDstIncrement;

			++pxPixel;
			++pdxFixed;
		}

		pDstPixelData += yDstIncrement;
	}
}

void InterpolateBilinearUnsigned16(

		BYTE* pDstPixelData,

		float dstRegionWidth,
		float dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		unsigned short* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		float srcRegionOriginY,

		float xRatio,
		float yRatio,
		LUTDATA* pLutData,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{
	// NY: Bug #295: When I originally changed this method so that
	// int pointers are used instead of byte pointers, I simply
	// incremented the pointer by 1 to go to the next pixel.  That obviously
	// doesn't work when the image has been rotated 90 deg.  And so we need
	// to use xDstIncrement when incrementing the pointer.  Problem is though,
	// xDstIncrement is in bytes.  So to compensate, we divide xDstIncrement
	// by 4 (i.e. right shift 2 bits) to get the increment in ints.
	xDstIncrement = xDstIncrement >> 2;

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;

	int ySrcPixel, dyFixed;

	int* pRowDstPixelData;
	unsigned short* pRowSrcPixelData;
	int *pxPixel, *pdxFixed;
	unsigned short *pSrcPixel00, *pSrcPixel01, *pSrcPixel10, *pSrcPixel11;
	int yInterpolated1, yInterpolated2, finalInterpolated;

	for (float y = 0; y < dstRegionHeight; ++y)  //so we're not constantly converting ints to floats.
	{
		float ySrcCoordinate = srcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		ySrcPixel = (int)ySrcCoordinate;
		dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		pRowDstPixelData = (int*)pDstPixelData;
		pRowSrcPixelData = pSrcPixelData + ySrcPixel * srcWidth;
	    
		pxPixel = spxSrcPixels.get();
		pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
			pSrcPixel00 = pRowSrcPixelData + (*pxPixel);
			pSrcPixel01 = pSrcPixel00 + 1;
			pSrcPixel10 = pSrcPixel00 + srcWidth;
			pSrcPixel11 = pSrcPixel10 + 1;
			
			//wherever you multiply, you have to downshift again to keep the decimal precision of the #s the same.
			yInterpolated1 = ((*pSrcPixel00) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
			yInterpolated2 = ((*pSrcPixel01) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);

			finalInterpolated = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;
			*pRowDstPixelData = *(pLutData->LutData + finalInterpolated - pLutData->FirstMappedPixelValue);

			pRowDstPixelData += xDstIncrement;

			++pxPixel;
			++pdxFixed;
		}

		pDstPixelData += yDstIncrement;
	}
}

// In the case where bits stored = 16, the DICOM representation of negative numbers
// is the same as the standard representation, so we don't have to do anything special
// to handle them.
void InterpolateBilinearSigned16(

		BYTE* pDstPixelData,

		float dstRegionWidth,
		float dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		short* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		float srcRegionOriginY,

		float xRatio,
		float yRatio,
		
		LUTDATA* pLutData,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{
	// NY: Bug #295: When I originally changed this method so that
	// int pointers are used instead of byte pointers, I simply
	// incremented the pointer by 1 to go to the next pixel.  That obviously
	// doesn't work when the image has been rotated 90 deg.  And so we need
	// to use xDstIncrement when incrementing the pointer.  Problem is though,
	// xDstIncrement is in bytes.  So to compensate, we divide xDstIncrement
	// by 4 (i.e. right shift 2 bits) to get the increment in ints.
	xDstIncrement = xDstIncrement >> 2;

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;
	int ySrcPixel, dyFixed;
	int* pRowDstPixelData;
	short* pRowSrcPixelData;
	int *pxPixel, *pdxFixed;
	short *pSrcPixel00, *pSrcPixel01, *pSrcPixel10, *pSrcPixel11;
	int yInterpolated1, yInterpolated2, finalInterpolated;

	for (float y = 0; y < dstRegionHeight; ++y)  //so we're not constantly converting ints to floats.
	{
		float ySrcCoordinate = srcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		ySrcPixel = (int)ySrcCoordinate;
		dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		pRowDstPixelData = (int*)pDstPixelData;
		pRowSrcPixelData = pSrcPixelData + ySrcPixel * srcWidth;
	    
		pxPixel = spxSrcPixels.get();
		pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
			pSrcPixel00 = pRowSrcPixelData + (*pxPixel);
			pSrcPixel01 = pSrcPixel00 + 1;
			pSrcPixel10 = pSrcPixel00 + srcWidth;
			pSrcPixel11 = pSrcPixel10 + 1;
			
			//wherever you multiply, you have to downshift again to keep the decimal precision of the #s the same.
			yInterpolated1 = ((*pSrcPixel00) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
			yInterpolated2 = ((*pSrcPixel01) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);

			finalInterpolated = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;
			*pRowDstPixelData = *(pLutData->LutData + finalInterpolated - pLutData->FirstMappedPixelValue);
			
			pRowDstPixelData += xDstIncrement;

			++pxPixel;
			++pdxFixed;
		}

		pDstPixelData += yDstIncrement;
	}
}

// In the case where bits stored < 16, the DICOM representation of negative numbers
// is NOT the same as the standard representation, so we need some special case code to
// convert the DICOM representation to the standard representation.
void InterpolateBilinearSignedSub16(

		BYTE* pDstPixelData,

		float dstRegionWidth,
		float dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		short* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		unsigned int srcBitsStored,
		float srcRegionOriginY,

		float xRatio,
		float yRatio,
		LUTDATA* pLutData,
		
		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{
	// NY: Bug #295: When I originally changed this method so that
	// int pointers are used instead of byte pointers, I simply
	// incremented the pointer by 1 to go to the next pixel.  That obviously
	// doesn't work when the image has been rotated 90 deg.  And so we need
	// to use xDstIncrement when incrementing the pointer.  Problem is though,
	// xDstIncrement is in bytes.  So to compensate, we divide xDstIncrement
	// by 4 (i.e. right shift 2 bits) to get the increment in ints.
	xDstIncrement = xDstIncrement >> 2;

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;
	int ySrcPixel, dyFixed;
	int* pRowDstPixelData;
	short* pRowSrcPixelData;
	int *pxPixel, *pdxFixed;
	short *pSrcPixel00, *pSrcPixel01, *pSrcPixel10, *pSrcPixel11;
	short srcPixel00, srcPixel01, srcPixel10, srcPixel11;
	int yInterpolated1, yInterpolated2, finalInterpolated;

	// Mask used to determine if a pixel value is signed or not.  Note that the
	// sign bit is the high bit.  Thus, if the bits stored = 9, the sign bit is 8
	short signMask = (short)(1 << (srcBitsStored - 1));

	// Used to turn a signed pixel value of arbitrary bit depth into a 16 bit signed equivalent
	short signPadding = (short)(0xffff << (srcBitsStored - 1));
	int offset = 1 << srcBitsStored;

	for (float y = 0; y < dstRegionHeight; ++y)  //so we're not constantly converting ints to floats.
	{
		float ySrcCoordinate = srcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		ySrcPixel = (int)ySrcCoordinate;
		dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		pRowDstPixelData = (int*)pDstPixelData;
		pRowSrcPixelData = pSrcPixelData + ySrcPixel * srcWidth;
	    
		pxPixel = spxSrcPixels.get();
		pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
			pSrcPixel00 = pRowSrcPixelData + (*pxPixel);
			pSrcPixel01 = pSrcPixel00 + 1;
			pSrcPixel10 = pSrcPixel00 + srcWidth;
			pSrcPixel11 = pSrcPixel10 + 1;

			// Convert DICOM representation of negative numbers to 
			// standard 16-bit representation
			if ((*pSrcPixel00 & signMask) != 0 )
				srcPixel00 = *pSrcPixel00 | signPadding;
			else
				srcPixel00 = *pSrcPixel00;

			if ((*pSrcPixel01 & signMask) != 0 )
				srcPixel01 = *pSrcPixel01 | signPadding;
			else
				srcPixel01 = *pSrcPixel01;

			if ((*pSrcPixel10 & signMask) != 0 )
				srcPixel10 = *pSrcPixel10 | signPadding;
			else
				srcPixel10 = *pSrcPixel10;

			if ((*pSrcPixel11 & signMask) != 0 )
				srcPixel11 = *pSrcPixel11 | signPadding;
			else
				srcPixel11 = *pSrcPixel11;

			//wherever you multiply, you have to downshift again to keep the decimal precision of the #s the same.
			yInterpolated1 = ((srcPixel00) << FIXEDPRECISION) + ((dyFixed * ((srcPixel10 - srcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
			yInterpolated2 = ((srcPixel01) << FIXEDPRECISION) + ((dyFixed * ((srcPixel11 - srcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);

			finalInterpolated = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;
			*pRowDstPixelData = *(pLutData->LutData + finalInterpolated - pLutData->FirstMappedPixelValue);

			pRowDstPixelData += xDstIncrement;

			++pxPixel;
			++pdxFixed;
		}

		pDstPixelData += yDstIncrement;
	}
}

void InterpolateBilinearRGB1ChanLut(

		BYTE* pDstPixelData,

		float dstRegionWidth,
		float dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		BYTE* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		float srcRegionOriginY,

		int xSrcStride,
		int ySrcStride,
		unsigned int srcNextChannelOffset,

		float xRatio,
		float yRatio,

		LUTDATA* pLutData,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;

	for (float y = 0; y < dstRegionHeight; ++y)
	{
		float ySrcCoordinate = srcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		int ySrcPixel = (int)ySrcCoordinate;
		int dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		BYTE* pRowDstPixelData = pDstPixelData;
		BYTE* pRowSrcPixelData = pSrcPixelData + ySrcPixel * ySrcStride;
	    
		int* pxPixel = spxSrcPixels.get();
		int* pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
            BYTE* pSrcPixel00 = pRowSrcPixelData + (*pxPixel) * xSrcStride; 
    
            for (int i = 0; i < 3; ++i)
            {
                BYTE* pSrcPixel01 = pSrcPixel00 + xSrcStride;
                BYTE* pSrcPixel10 = pSrcPixel00 + ySrcStride;
                BYTE* pSrcPixel11 = pSrcPixel10 + xSrcStride;

				int yInterpolated1 = (*pSrcPixel00 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int yInterpolated2 = (*pSrcPixel01 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int IFinal = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;

                pRowDstPixelData[i] = (BYTE) (*(pLutData->LutData + (BYTE)(IFinal) - pLutData->FirstMappedPixelValue)); //R(i=0), G(1), B(2), A(3)

                pSrcPixel00 += srcNextChannelOffset;
            }

			{
                BYTE* pSrcPixel01 = pSrcPixel00 + xSrcStride;
                BYTE* pSrcPixel10 = pSrcPixel00 + ySrcStride;
                BYTE* pSrcPixel11 = pSrcPixel10 + xSrcStride;

				int yInterpolated1 = (*pSrcPixel00 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int yInterpolated2 = (*pSrcPixel01 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int IFinal = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;

                pRowDstPixelData[3] = (BYTE)(IFinal); //R(i=0), G(1), B(2), A(3)

                pSrcPixel00 += srcNextChannelOffset;
			}

			pRowDstPixelData += xDstIncrement;
			++pxPixel;
			++pdxFixed;
		}


		pDstPixelData += yDstIncrement;
	}
}

void InterpolateBilinearRGB(

		BYTE* pDstPixelData,

		float dstRegionWidth,
		float dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		BYTE* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		float srcRegionOriginY,

		int xSrcStride,
		int ySrcStride,
		unsigned int srcNextChannelOffset,

		float xRatio,
		float yRatio,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;

	for (float y = 0; y < dstRegionHeight; ++y)
	{
		float ySrcCoordinate = srcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		int ySrcPixel = (int)ySrcCoordinate;
		int dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		BYTE* pRowDstPixelData = pDstPixelData;
		BYTE* pRowSrcPixelData = pSrcPixelData + ySrcPixel * ySrcStride;
	    
		int* pxPixel = spxSrcPixels.get();
		int* pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
            BYTE* pSrcPixel00 = pRowSrcPixelData + (*pxPixel) * xSrcStride; 
    
            for (int i = 0; i < 4; ++i)
            {
                BYTE* pSrcPixel01 = pSrcPixel00 + xSrcStride;
                BYTE* pSrcPixel10 = pSrcPixel00 + ySrcStride;
                BYTE* pSrcPixel11 = pSrcPixel10 + xSrcStride;

				int yInterpolated1 = (*pSrcPixel00 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int yInterpolated2 = (*pSrcPixel01 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int IFinal = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;

                pRowDstPixelData[i] = (BYTE)(IFinal); //R(i=0), G(1), B(2), A(3)

                pSrcPixel00 += srcNextChannelOffset;
            }

			pRowDstPixelData += xDstIncrement;
			++pxPixel;
			++pdxFixed;
		}


		pDstPixelData += yDstIncrement;
	}
}


BOOL InterpolateBilinear
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
)
{
	int dstRegionHeight, dstRegionWidth;
	unsigned int xDstStride, yDstStride, xDstIncrement, yDstIncrement;

    if (swapXY)
    {
        dstRegionHeight = abs(dstRegionRectRight - dstRegionRectLeft);
        dstRegionWidth = abs(dstRegionRectBottom - dstRegionRectTop);
        xDstStride = dstWidth * dstBytesPerPixel;
        yDstStride = dstBytesPerPixel;
		xDstIncrement = ((dstRegionRectBottom - dstRegionRectTop) < 0 ? -1: 1) * xDstStride;
		yDstIncrement = ((dstRegionRectRight - dstRegionRectLeft) < 0 ? -1: 1) * yDstStride;

		//Offset Top/Left by 1 if the height/width is negative.
		int zeroBasedTop = dstRegionRectTop;
		if (xDstIncrement < 0) //use the width because it's actually the height.
			--zeroBasedTop;

		int zeroBasedLeft = dstRegionRectLeft;
		if (yDstIncrement < 0) //use the height because it's actually the width.
			--zeroBasedLeft;

		pDstPixelData += (zeroBasedTop * xDstStride) + (zeroBasedLeft * yDstStride);
    }
    else
    {
        dstRegionHeight = abs(dstRegionRectBottom - dstRegionRectTop);
        dstRegionWidth = abs(dstRegionRectRight - dstRegionRectLeft);
        xDstStride = dstBytesPerPixel;
        yDstStride = dstWidth * dstBytesPerPixel;
        xDstIncrement = ((dstRegionRectRight - dstRegionRectLeft) < 0 ? -1: 1) * xDstStride;
        yDstIncrement = ((dstRegionRectBottom - dstRegionRectTop) < 0 ? -1: 1) * yDstStride;

		//Offset Top/Left by 1 if the height/width is negative.
		int zeroBasedTop = dstRegionRectTop;
		if (yDstIncrement < 0)
			--zeroBasedTop;

		int zeroBasedLeft = dstRegionRectLeft;
		if (xDstIncrement < 0)
			--zeroBasedLeft;

		pDstPixelData += (zeroBasedTop * yDstStride) + (zeroBasedLeft * xDstStride);
    }

    float srcRegionWidth = srcRegionRectRight - srcRegionRectLeft;
    float srcRegionHeight = srcRegionRectBottom - srcRegionRectTop;

    float srcSlightlyLessThanWidthMinusOne = (float)srcWidth - SLIGHTLYGREATERTHANONE;

    float xRatio = srcRegionWidth / (float)dstRegionWidth;
    float yRatio = srcRegionHeight / (float)dstRegionHeight;

	std::auto_ptr<int> spxSrcPixels(new int[dstRegionWidth]);
	int* pxPixel = spxSrcPixels.get();

	std::auto_ptr<int> spdxFixedAtSrcPixelCoordinates(new int[dstRegionWidth]);
	int * pdxFixed = spdxFixedAtSrcPixelCoordinates.get();

	float floatDstRegionWidth = (float)dstRegionWidth;

	for (float x = 0; x < floatDstRegionWidth; ++x)
	{
		float xCoord = srcRegionRectLeft + (x + 0.5F) * xRatio;

		//a necessary evil, I'm afraid.
		if (xCoord < 0)
			xCoord = 0;
		if (xCoord > srcSlightlyLessThanWidthMinusOne)
			xCoord = srcSlightlyLessThanWidthMinusOne; //force it to be just barely before the last pixel.

		*pxPixel = (int)xCoord;
		*pdxFixed = (int)((xCoord - (float)(*pxPixel)) * FIXEDSCALE);

		++pxPixel;
		++pdxFixed;
	}

	if (isRGB != FALSE)
	{
		int xSrcStride, ySrcStride, srcNextChannelOffset;

		if (!isPlanar)
		{
			xSrcStride = 4;
			ySrcStride = srcWidth * 4;
		}
		else
		{
			xSrcStride = 1;
			ySrcStride = srcWidth;
		}

		if (!isPlanar)
			srcNextChannelOffset = 1;
		else
			srcNextChannelOffset = srcWidth * srcHeight;

		if (pLutData == NULL)
		{

		InterpolateBilinearRGB(
				pDstPixelData,
				floatDstRegionWidth,
				(float)dstRegionHeight,
				xDstIncrement,
				yDstIncrement,
				pSrcPixelData,
				srcWidth,
				srcHeight,
				srcRegionRectTop,
				xSrcStride,
				ySrcStride,
				srcNextChannelOffset,
				xRatio,
				yRatio,
				spxSrcPixels,
				spdxFixedAtSrcPixelCoordinates);

		}
		else
		{

		InterpolateBilinearRGB1ChanLut(
				pDstPixelData,
				floatDstRegionWidth,
				(float)dstRegionHeight,
				xDstIncrement,
				yDstIncrement,
				pSrcPixelData,
				srcWidth,
				srcHeight,
				srcRegionRectTop,
				xSrcStride,
				ySrcStride,
				srcNextChannelOffset,
				xRatio,
				yRatio,
				pLutData,
				spxSrcPixels,
				spdxFixedAtSrcPixelCoordinates);

		}
	}
	else
	{
		if (srcBytesPerPixel == 2)
		{
			if (isSigned == FALSE)
			{
				InterpolateBilinearUnsigned16
				(
					pDstPixelData,
					floatDstRegionWidth,
					(float)dstRegionHeight,
					xDstIncrement,
					yDstIncrement,
					(unsigned short*)pSrcPixelData,
					srcWidth,
					srcHeight,
					srcRegionRectTop,
					xRatio,
					yRatio,
					pLutData,
					spxSrcPixels,
					spdxFixedAtSrcPixelCoordinates);
			}
			else
			{
				if (srcBitsStored == 16)
				{
					InterpolateBilinearSigned16
					(
						pDstPixelData,
						floatDstRegionWidth,
						(float)dstRegionHeight,
						xDstIncrement,
						yDstIncrement,
						(short*)pSrcPixelData,
						srcWidth,
						srcHeight,
						srcRegionRectTop,
						xRatio,
						yRatio,
						pLutData,
						spxSrcPixels,
						spdxFixedAtSrcPixelCoordinates);
				}
				else
				{
					InterpolateBilinearSignedSub16
					(
						pDstPixelData,
						floatDstRegionWidth,
						(float)dstRegionHeight,
						xDstIncrement,
						yDstIncrement,
						(short*)pSrcPixelData,
						srcWidth,
						srcHeight,
						srcBitsStored,
						srcRegionRectTop,
						xRatio,
						yRatio,
						pLutData,
						spxSrcPixels,
						spdxFixedAtSrcPixelCoordinates);
				}
			}
		}
		else
		{
			if (isSigned == FALSE)
			{
				InterpolateBilinearUnsigned8
				(
					pDstPixelData,
					floatDstRegionWidth,
					(float)dstRegionHeight,
					xDstIncrement,
					yDstIncrement,
					pSrcPixelData,
					srcWidth,
					srcHeight,
					srcRegionRectTop,
					xRatio,
					yRatio,
					pLutData,
					spxSrcPixels,
					spdxFixedAtSrcPixelCoordinates);
			}
			else
			{
				InterpolateBilinearSigned8
				(
					pDstPixelData,
					floatDstRegionWidth,
					(float)dstRegionHeight,
					xDstIncrement,
					yDstIncrement,
					(char*)pSrcPixelData,
					srcWidth,
					srcHeight,
					srcBitsStored,
					srcRegionRectTop,
					xRatio,
					yRatio,
					pLutData,
					spxSrcPixels,
					spdxFixedAtSrcPixelCoordinates);
			}
		}
	}

	return TRUE;
}
