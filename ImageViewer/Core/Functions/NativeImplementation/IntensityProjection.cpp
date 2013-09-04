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

#include "stdafx.h"
#include "IntensityProjection.h"

template <typename pixel> void IntensityProjection::ProjectMaximumOrthogonal(pixel* slabData, pixel* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	// copy the first subsample directly to the output
	memcpy(pixelData + blockOffset, slabData + blockOffset, blockCount*sizeof(pixel));

	// iterate over the remaining subsamples
	pixel* pOutput;
	pixel* pInput = slabData + pixelsPerSubsample + blockOffset;
	for(int f = 1; f < subsamples; ++f)
	{
		pOutput = pixelData + blockOffset;

		// check if each pixel in this subsample is greater than the current output value, and update if it is
		for(int n = 0; n < blockCount; ++n)
		{
			pixel value = *pInput++;
			if (value > *pOutput) *pOutput = value;
			++pOutput;
		}

		pInput = pInput + pixelsPerSubsample - blockCount;
	}
};

template <typename pixel> void IntensityProjection::ProjectMinimumOrthogonal(pixel* slabData, pixel* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	// copy the first subsample directly to the output
	memcpy(pixelData + blockOffset, slabData + blockOffset, blockCount*sizeof(pixel));
	
	// iterate over the remaining subsamples
	pixel* pOutput;
	pixel* pInput = slabData + pixelsPerSubsample + blockOffset;
	for(int f = 1; f < subsamples; ++f)
	{
		pOutput = pixelData + blockOffset;

		// check if each pixel in this subsample is less than the current output value, and update if it is
		for(int n = 0; n < blockCount; ++n)
		{
			pixel value = *pInput++;
			if (value < *pOutput) *pOutput = value;
			++pOutput;
		}

		pInput = pInput + pixelsPerSubsample - blockCount;
	}
};

template <typename pixel, typename sumtype> void IntensityProjection::ProjectAverageOrthogonal(pixel* slabData, pixel* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount, sumtype nil)
{
	// create an array suitable for storing the sums at each pixel location
	sumtype* pSums0 = new sumtype[blockCount];
	sumtype* pSums = pSums0;

	// initialize sums array with values of first subsample
	pixel* pInput = slabData + blockOffset;
	for(int n = 0; n < blockCount; ++n)
	{
		*pSums++ = *pInput++;
	}
	
	// iterate over the remaining subsamples
	for(int f = 1; f < subsamples; ++f)
	{
		pInput = pInput + pixelsPerSubsample - blockCount;
		pSums = pSums0;

		// add each pixel in this subsample to the current sum value
		for(int n = 0; n < blockCount; ++n)
		{
			pixel value = *pInput++;
			*pSums += value;
			++pSums;
		}
	}

	// iterate over the sums array
	pSums = pSums0;
	pixel* pOutput = pixelData + blockOffset;
	for(int n = 0; n < blockCount; ++n)
	{
		// calculate the average by dividing by number of subsamples and doing proper rounding, then assign to output
		sumtype sum = *pSums++;
		*pOutput = pixel(1.0*sum/subsamples + (sum > 0 ? 0.5 : -0.5));
		++pOutput;
	}

	delete [] pSums0;
};
