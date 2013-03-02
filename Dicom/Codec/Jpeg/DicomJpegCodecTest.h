#pragma region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU Lesser Public
// License as published by the Free Software Foundation, either version 3 of
// the License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser Public License for more details.
//
// You should have received a copy of the GNU Lesser Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#pragma endregion

#pragma region Inline Attributions
// The source code contained in this file is based on an original work
// from
//
// mDCM: A C# DICOM library
//
// Copyright (c) 2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)
#pragma endregion

#pragma once

#if _DEBUG
using namespace System;
using namespace NUnit::Framework;
using namespace ClearCanvas::Common;
using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom::Codec::Tests;

#include "DicomJpegCodecFactory.h"

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {

public ref class StubExtensionFactory : IExtensionFactory
{
public:

virtual array<Object^>^ CreateExtensions(ExtensionPoint^ extensionPoint, ExtensionFilter^ filter, bool justOne);

virtual array<ExtensionInfo^>^ ListExtensions(ExtensionPoint^ extensionPoint, ExtensionFilter^ filter);

};

[NUnit::Framework::TestFixture]
public ref class DicomJpegCodecTest : AbstractCodecTest
{
public:

	[NUnit::Framework::TestFixtureSetUp]
	void DicomJpegCodecTest::Init();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegProcess1CodecTest();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegProcess24CodecTest();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegLossless14CodecTest();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegLossless14CodecTest_8BitsStored16BitsAllocated();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegLossless14SV1CodecTest();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegLossless14SV1CodecTest_8BitsStored16BitsAllocated();
};

}
}
}
}

#endif
