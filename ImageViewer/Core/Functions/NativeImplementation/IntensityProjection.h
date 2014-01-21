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

#pragma once

class IntensityProjection abstract sealed
{
public:
	template <typename pixel> static void ProjectMaximumOrthogonal(pixel* slabData, pixel* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);
	template <typename pixel> static void ProjectMinimumOrthogonal(pixel* slabData, pixel* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);
	template <typename pixel, typename sumtype> static void ProjectAverageOrthogonal(pixel* slabData, pixel* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount, sumtype nil);
};

template void IntensityProjection::ProjectMaximumOrthogonal(signed char*, signed char*, int, int, int, int);
template void IntensityProjection::ProjectMaximumOrthogonal(unsigned char*, unsigned char*, int, int, int, int);
template void IntensityProjection::ProjectMaximumOrthogonal(short*, short*, int, int, int, int);
template void IntensityProjection::ProjectMaximumOrthogonal(unsigned short*, unsigned short*, int, int, int, int);
template void IntensityProjection::ProjectMaximumOrthogonal(int*, int*, int, int, int, int);
template void IntensityProjection::ProjectMaximumOrthogonal(unsigned int*, unsigned int*, int, int, int, int);

template void IntensityProjection::ProjectMinimumOrthogonal(signed char*, signed char*, int, int, int, int);
template void IntensityProjection::ProjectMinimumOrthogonal(unsigned char*, unsigned char*, int, int, int, int);
template void IntensityProjection::ProjectMinimumOrthogonal(short*, short*, int, int, int, int);
template void IntensityProjection::ProjectMinimumOrthogonal(unsigned short*, unsigned short*, int, int, int, int);
template void IntensityProjection::ProjectMinimumOrthogonal(int*, int*, int, int, int, int);
template void IntensityProjection::ProjectMinimumOrthogonal(unsigned int*, unsigned int*, int, int, int, int);

template void IntensityProjection::ProjectAverageOrthogonal(signed char*, signed char*, int, int, int, int, int);
template void IntensityProjection::ProjectAverageOrthogonal(unsigned char*, unsigned char*, int, int, int, int, int);
template void IntensityProjection::ProjectAverageOrthogonal(short*, short*, int, int, int, int, int);
template void IntensityProjection::ProjectAverageOrthogonal(unsigned short*, unsigned short*, int, int, int, int, int);
template void IntensityProjection::ProjectAverageOrthogonal(int*, int*, int, int, int, int, long long);
template void IntensityProjection::ProjectAverageOrthogonal(unsigned int*, unsigned int*, int, int, int, int, long long);
