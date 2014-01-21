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

#ifdef UNIT_TESTS

#include "IntensityProjectionTests.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Linq;
using namespace ClearCanvas::Common::Utilities::Tests;
using namespace ClearCanvas::ImageViewer::Core::Functions;
using namespace ClearCanvas::ImageViewer::Core::Functions::Tests;
using namespace NUnit::Framework;

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMaximumIntensityProjection1()
{
	const int pixels = 512*512;
	const int subsamples = 11;
	TestMaximumIntensityProjection(pixels, subsamples);
	TestMaximumIntensityProjection(pixels, subsamples, 0, pixels);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMaximumIntensityProjection2()
{
	const int pixels = 512*512;
	const int subsamples = 11;
	const int blockOffset = 517;
	const int blockSize = 40771;
	TestMaximumIntensityProjection(pixels, subsamples, blockOffset, blockSize);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMinimumIntensityProjection1()
{
	const int pixels = 512*512;
	const int subsamples = 11;
	TestMinimumIntensityProjection(pixels, subsamples);
	TestMinimumIntensityProjection(pixels, subsamples, 0, pixels);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMinimumIntensityProjection2()
{
	const int pixels = 512*512;
	const int subsamples = 11;
	const int blockOffset = 517;
	const int blockSize = 40771;
	TestMinimumIntensityProjection(pixels, subsamples, blockOffset, blockSize);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestAverageIntensityProjection1()
{
	const int pixels = 512*512;
	const int subsamples = 11;
	TestAverageIntensityProjection(pixels, subsamples);
	TestAverageIntensityProjection(pixels, subsamples, 0, pixels);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestAverageIntensityProjection2()
{
	const int pixels = 512*512;
	const int subsamples = 11;
	const int blockOffset = 517;
	const int blockSize = 40771;
	TestAverageIntensityProjection(pixels, subsamples, blockOffset, blockSize);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMaximumIntensityProjection(int pixels, int subsamples)
{
	array<PixelType> ^slabData = gcnew array<PixelType>(pixels*subsamples);
	FillRandomValues(0x2DB8498F, slabData);

	array<PixelType> ^expectedResults = gcnew array<PixelType>(pixels);
	for (int p = 0; p < pixels; ++p)
	{
		System::Collections::Generic::List<PixelType> ^r = gcnew System::Collections::Generic::List<PixelType>();
		for (int s = 0; s < subsamples; ++s) r->Add(slabData[s*pixels + p]);
		expectedResults[p] = Enumerable::Max(r);
	}

	array<PixelType> ^actualResults = gcnew array<PixelType>(pixels);

	pin_ptr<PixelType> pSlabData = &slabData[0];
	pin_ptr<PixelType> pOutput = &actualResults[0];
	try
	{
		MaximumIntensityProjection::ProjectOrthogonal(pSlabData, pOutput, subsamples, pixels);
	}
	finally
	{
		pSlabData = nullptr;
		pOutput = nullptr;
	}

	Assert::AreEqual(expectedResults, actualResults);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMaximumIntensityProjection(int pixels, int subsamples, int blockOffset, int blockSize)
{
	array<PixelType> ^slabData = gcnew array<PixelType>(pixels*subsamples);
	FillRandomValues(0x2DB8498F, slabData);

	array<PixelType> ^expectedResults = gcnew array<PixelType>(pixels);
	FillRandomValues(0x71A34991, expectedResults);
	for (int p = blockOffset; p < blockOffset + blockSize; ++p)
	{
		System::Collections::Generic::List<PixelType> ^r = gcnew System::Collections::Generic::List<PixelType>();
		for (int s = 0; s < subsamples; ++s) r->Add(slabData[s*pixels + p]);
		expectedResults[p] = Enumerable::Max(r);
	}

	array<PixelType> ^actualResults = gcnew array<PixelType>(pixels);
	for (int p = 0; p < blockOffset; ++p) actualResults[p] = expectedResults[p];
	for (int p = blockOffset + blockSize; p < pixels; ++p) actualResults[p] = expectedResults[p];

	pin_ptr<PixelType> pSlabData = &slabData[0];
	pin_ptr<PixelType> pOutput = &actualResults[0];
	try
	{
		MaximumIntensityProjection::ProjectOrthogonal(pSlabData, pOutput, subsamples, pixels, blockOffset, blockSize);
	}
	finally
	{
		pSlabData = nullptr;
		pOutput = nullptr;
	}

	Assert::AreEqual(expectedResults, actualResults);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMinimumIntensityProjection(int pixels, int subsamples)
{
	array<PixelType> ^slabData = gcnew array<PixelType>(pixels*subsamples);
	FillRandomValues(0x2DB8498F, slabData);

	array<PixelType> ^expectedResults = gcnew array<PixelType>(pixels);
	for (int p = 0; p < pixels; ++p)
	{
		System::Collections::Generic::List<PixelType> ^r = gcnew System::Collections::Generic::List<PixelType>();
		for (int s = 0; s < subsamples; ++s) r->Add(slabData[s*pixels + p]);
		expectedResults[p] = Enumerable::Min(r);
	}

	array<PixelType> ^actualResults = gcnew array<PixelType>(pixels);

	pin_ptr<PixelType> pSlabData = &slabData[0];
	pin_ptr<PixelType> pOutput = &actualResults[0];
	try
	{
		MinimumIntensityProjection::ProjectOrthogonal(pSlabData, pOutput, subsamples, pixels);
	}
	finally
	{
		pSlabData = nullptr;
		pOutput = nullptr;
	}

	Assert::AreEqual(expectedResults, actualResults);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestMinimumIntensityProjection(int pixels, int subsamples, int blockOffset, int blockSize)
{
	array<PixelType> ^slabData = gcnew array<PixelType>(pixels*subsamples);
	FillRandomValues(0x2DB8498F, slabData);

	array<PixelType> ^expectedResults = gcnew array<PixelType>(pixels);
	FillRandomValues(0x71A34991, expectedResults);
	for (int p = blockOffset; p < blockOffset + blockSize; ++p)
	{
		System::Collections::Generic::List<PixelType> ^r = gcnew System::Collections::Generic::List<PixelType>();
		for (int s = 0; s < subsamples; ++s) r->Add(slabData[s*pixels + p]);
		expectedResults[p] = Enumerable::Min(r);
	}

	array<PixelType> ^actualResults = gcnew array<PixelType>(pixels);
	for (int p = 0; p < blockOffset; ++p) actualResults[p] = expectedResults[p];
	for (int p = blockOffset + blockSize; p < pixels; ++p) actualResults[p] = expectedResults[p];

	pin_ptr<PixelType> pSlabData = &slabData[0];
	pin_ptr<PixelType> pOutput = &actualResults[0];
	try
	{
		MinimumIntensityProjection::ProjectOrthogonal(pSlabData, pOutput, subsamples, pixels, blockOffset, blockSize);
	}
	finally
	{
		pSlabData = nullptr;
		pOutput = nullptr;
	}

	Assert::AreEqual(expectedResults, actualResults);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestAverageIntensityProjection(int pixels, int subsamples)
{
	array<PixelType> ^slabData = gcnew array<PixelType>(pixels*subsamples);
	FillRandomValues(0x2DB8498F, slabData);

	array<PixelType> ^expectedResults = gcnew array<PixelType>(pixels);
	for (int p = 0; p < pixels; ++p)
	{
		System::Collections::Generic::List<Int64> ^r = gcnew System::Collections::Generic::List<Int64>();
		for (int s = 0; s < subsamples; ++s) r->Add(slabData[s*pixels + p]);
		expectedResults[p] = PixelType(Math::Round(Enumerable::Average(r)));
	}

	array<PixelType> ^actualResults = gcnew array<PixelType>(pixels);

	pin_ptr<PixelType> pSlabData = &slabData[0];
	pin_ptr<PixelType> pOutput = &actualResults[0];
	try
	{
		AverageIntensityProjection::ProjectOrthogonal(pSlabData, pOutput, subsamples, pixels);
	}
	finally
	{
		pSlabData = nullptr;
		pOutput = nullptr;
	}

	Assert::AreEqual(expectedResults, actualResults);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::TestAverageIntensityProjection(int pixels, int subsamples, int blockOffset, int blockSize)
{
	array<PixelType> ^slabData = gcnew array<PixelType>(pixels*subsamples);
	FillRandomValues(0x2DB8498F, slabData);

	array<PixelType> ^expectedResults = gcnew array<PixelType>(pixels);
	FillRandomValues(0x71A34991, expectedResults);
	for (int p = blockOffset; p < blockOffset + blockSize; ++p)
	{
		System::Collections::Generic::List<Int64> ^r = gcnew System::Collections::Generic::List<Int64>();
		for (int s = 0; s < subsamples; ++s) r->Add(slabData[s*pixels + p]);
		expectedResults[p] = PixelType(Math::Round(Enumerable::Average(r)));
	}

	array<PixelType> ^actualResults = gcnew array<PixelType>(pixels);
	for (int p = 0; p < blockOffset; ++p) actualResults[p] = expectedResults[p];
	for (int p = blockOffset + blockSize; p < pixels; ++p) actualResults[p] = expectedResults[p];

	pin_ptr<PixelType> pSlabData = &slabData[0];
	pin_ptr<PixelType> pOutput = &actualResults[0];
	try
	{
		AverageIntensityProjection::ProjectOrthogonal(pSlabData, pOutput, subsamples, pixels, blockOffset, blockSize);
	}
	finally
	{
		pSlabData = nullptr;
		pOutput = nullptr;
	}

	Assert::AreEqual(expectedResults, actualResults);
};

template <typename PixelType> void IntensityProjectionTestBase<PixelType>::FillRandomValues(int seed, array<PixelType> ^data)
{
	PseudoRandom ^rng = gcnew PseudoRandom(seed);
	for (int n = 0; n < data->Length; ++n)
		data[n] = (PixelType) rng->Next(PixelType::MinValue, PixelType::MaxValue);
};

template <> void IntensityProjectionTestBase<UInt32>::FillRandomValues(int seed, array<UInt32> ^data)
{
	PseudoRandom ^rng = gcnew PseudoRandom(seed);
	for (int n = 0; n < data->Length; ++n)
		data[n] = UInt32(rng->Next(Int32::MinValue, Int32::MaxValue));
};

template <typename PixelType> IntensityProjectionTestBase<PixelType>::IntensityProjectionTestBase()
{
};

IntensityProjectionTestUInt8::IntensityProjectionTestUInt8() : IntensityProjectionTestBase()
{
};

IntensityProjectionTestInt8::IntensityProjectionTestInt8() : IntensityProjectionTestBase()
{
};

IntensityProjectionTestUInt16::IntensityProjectionTestUInt16() : IntensityProjectionTestBase()
{
};

IntensityProjectionTestInt16::IntensityProjectionTestInt16() : IntensityProjectionTestBase()
{
};

IntensityProjectionTestUInt32::IntensityProjectionTestUInt32() : IntensityProjectionTestBase()
{
};

IntensityProjectionTestInt32::IntensityProjectionTestInt32() : IntensityProjectionTestBase()
{
};

#endif
