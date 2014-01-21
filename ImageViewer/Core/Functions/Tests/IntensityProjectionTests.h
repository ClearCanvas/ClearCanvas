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

using namespace System;
using namespace NUnit::Framework;

namespace ClearCanvas {
namespace ImageViewer {
namespace Core {
namespace Functions {
namespace Tests {

	template <typename PixelType> public ref class IntensityProjectionTestBase abstract
	{
	public:
		[TestAttribute]
		virtual void TestMaximumIntensityProjection1();

		[TestAttribute]
		virtual void TestMaximumIntensityProjection2();

		[TestAttribute]
		virtual void TestMinimumIntensityProjection1();

		[TestAttribute]
		virtual void TestMinimumIntensityProjection2();

		[TestAttribute]
		virtual void TestAverageIntensityProjection1();

		[TestAttribute]
		virtual void TestAverageIntensityProjection2();

	protected:
		IntensityProjectionTestBase();

	private:
		void TestMaximumIntensityProjection(int pixels, int subsamples);
		void TestMaximumIntensityProjection(int pixels, int subsamples, int blockOffset, int blockSize);
		void TestMinimumIntensityProjection(int pixels, int subsamples);
		void TestMinimumIntensityProjection(int pixels, int subsamples, int blockOffset, int blockSize);
		void TestAverageIntensityProjection(int pixels, int subsamples);
		void TestAverageIntensityProjection(int pixels, int subsamples, int blockOffset, int blockSize);
		void FillRandomValues(int seed, array<PixelType> ^data);
	};

	[TestFixtureAttribute]
	public ref class IntensityProjectionTestUInt8 : public IntensityProjectionTestBase<Byte>
	{
	public:
		IntensityProjectionTestUInt8();
	};

	[TestFixtureAttribute]
	public ref class IntensityProjectionTestInt8 : public IntensityProjectionTestBase<SByte>
	{
	public:
		IntensityProjectionTestInt8();
	};

	[TestFixtureAttribute]
	public ref class IntensityProjectionTestUInt16 : public IntensityProjectionTestBase<UInt16>
	{
	public:
		IntensityProjectionTestUInt16();
	};

	[TestFixtureAttribute]
	public ref class IntensityProjectionTestInt16 : public IntensityProjectionTestBase<Int16>
	{
	public:
		IntensityProjectionTestInt16();
	};

	[TestFixtureAttribute]
	public ref class IntensityProjectionTestUInt32 : public IntensityProjectionTestBase<UInt32>
	{
	public:
		IntensityProjectionTestUInt32();
	};

	[TestFixtureAttribute]
	public ref class IntensityProjectionTestInt32 : public IntensityProjectionTestBase<Int32>
	{
	public:
		IntensityProjectionTestInt32();
	};

}
}
}
}
}
