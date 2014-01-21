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
#include "MaximumIntensityProjection.h"
#include "NativeImplementation/IntensityProjection.h"

using namespace ClearCanvas::ImageViewer::Core::Functions;

void MaximumIntensityProjection::ProjectOrthogonal(unsigned int* slabData, unsigned int* pixelData, int subsamples, int pixelsPerSubsample)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, 0, pixelsPerSubsample);
};

void MaximumIntensityProjection::ProjectOrthogonal(int* slabData, int* pixelData, int subsamples, int pixelsPerSubsample)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, 0, pixelsPerSubsample);
};

void MaximumIntensityProjection::ProjectOrthogonal(unsigned short* slabData, unsigned short* pixelData, int subsamples, int pixelsPerSubsample)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, 0, pixelsPerSubsample);
};

void MaximumIntensityProjection::ProjectOrthogonal(short* slabData, short* pixelData, int subsamples, int pixelsPerSubsample)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, 0, pixelsPerSubsample);
};

void MaximumIntensityProjection::ProjectOrthogonal(unsigned char* slabData, unsigned char* pixelData, int subsamples, int pixelsPerSubsample)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, 0, pixelsPerSubsample);
};

void MaximumIntensityProjection::ProjectOrthogonal(signed char* slabData, signed char* pixelData, int subsamples, int pixelsPerSubsample)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, 0, pixelsPerSubsample);
};

void MaximumIntensityProjection::ProjectOrthogonal(unsigned int* slabData, unsigned int* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, blockOffset, blockCount);
};

void MaximumIntensityProjection::ProjectOrthogonal(int* slabData, int* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, blockOffset, blockCount);
};

void MaximumIntensityProjection::ProjectOrthogonal(unsigned short* slabData, unsigned short* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, blockOffset, blockCount);
};

void MaximumIntensityProjection::ProjectOrthogonal(short* slabData, short* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, blockOffset, blockCount);
};

void MaximumIntensityProjection::ProjectOrthogonal(unsigned char* slabData, unsigned char* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, blockOffset, blockCount);
};

void MaximumIntensityProjection::ProjectOrthogonal(signed char* slabData, signed char* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount)
{
	return IntensityProjection::ProjectMaximumOrthogonal(slabData, pixelData, subsamples, pixelsPerSubsample, blockOffset, blockCount);
};
