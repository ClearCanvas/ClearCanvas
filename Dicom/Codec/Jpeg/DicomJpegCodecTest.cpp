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

#ifdef _DEBUG

#include "DicomJpegCodecTest.h"

using namespace System;
using namespace NUnit::Framework;
using namespace ClearCanvas::Common;
using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom::Codec::Tests;

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {


array<Object^>^ StubExtensionFactory::CreateExtensions(ExtensionPoint^ extensionPoint, ExtensionFilter^ filter, bool justOne)
{
	if (extensionPoint->GetType() != DicomCodecFactoryExtensionPoint::typeid)
		return gcnew array<Object^>(0);

	array<Object^>^ theArray = gcnew array<Object^>(4);
	theArray[0] = gcnew DicomJpegProcess1CodecFactory;
	theArray[1] = gcnew DicomJpegProcess24CodecFactory;
	theArray[2] = gcnew DicomJpegLossless14CodecFactory;
	theArray[3] = gcnew DicomJpegLossless14SV1CodecFactory;
	return theArray;
}

array<ExtensionInfo^>^ StubExtensionFactory::ListExtensions(ExtensionPoint^ extensionPoint, ExtensionFilter^ filter)
{
	return gcnew array<ExtensionInfo^>(0);
}

void DicomJpegCodecTest::Init()
{
	Platform::SetExtensionFactory(gcnew StubExtensionFactory());

	//HACK: for now, call the static constructor again, so it will repopulate the dictionary
	System::Reflection::ConstructorInfo^ staticConstructor = DicomCodecRegistry::typeid->TypeInitializer;
	staticConstructor->Invoke(nullptr, nullptr);
}

void DicomJpegCodecTest::DicomJpegProcess1CodecTest()
{
	TransferSyntax^ syntax = TransferSyntax::JpegBaselineProcess1;

	DicomFile^ file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 8, 8, true, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 8, 8, false, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "YBR_FULL", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "YBR_FULL", 8, 8, false, 2);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 12, 16, false, 1);
	ExpectedFailureTest(syntax, file);

	file = CreateUNFile(255, 255, "RGB", 8, 8, false, 1);
	file->Save();
    DicomFile^ newFile = gcnew DicomFile(file->Filename);
    newFile->Load();

	LossyImageTest(syntax, newFile);
}

void DicomJpegCodecTest::DicomJpegProcess24CodecTest()
{
	TransferSyntax^ syntax = TransferSyntax::JpegExtendedProcess24;
	DicomFile^ file = CreateFile(512, 512, "MONOCHROME1", 12, 16, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "MONOCHROME1", 12, 16, true, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 12, 16, true, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 12, 16, true, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 12, 16, true, 3);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "YBR_FULL", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "YBR_FULL", 8, 8, false, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 16, 16, false, 1);
	ExpectedFailureTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 15, 16, false, 1);
	ExpectedFailureTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 14, 16, false, 1);
	ExpectedFailureTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 13, 16, false, 1);
	ExpectedFailureTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 11, 16, false, 1);
	ExpectedFailureTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 10, 16, false, 1);
	ExpectedFailureTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME2", 9, 16, false, 1);
	ExpectedFailureTest(syntax, file);

}

void DicomJpegCodecTest::DicomJpegLossless14CodecTest()
{
	TransferSyntax^ syntax = TransferSyntax::JpegLosslessNonHierarchicalProcess14;
	DicomFile^ file = CreateFile(512, 512, "MONOCHROME1", 12, 16, false, 1);
	LosslessImageTest(syntax, file);

	// Lossy test for signed, becaue it converts
	file = CreateFile(512, 512, "MONOCHROME1", 12, 16, true, 1);
	LosslessImageTestWithConversion(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 1);
	LosslessImageTestWithConversion(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, true, 1);
	LosslessImageTestWithConversion(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 15, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 14, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 13, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 12, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 11, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 10, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 9, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "YBR_FULL", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "YBR_FULL", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

}

void DicomJpegCodecTest::DicomJpegLossless14CodecTest_8BitsStored16BitsAllocated()
{
	TransferSyntax^ syntax = TransferSyntax::JpegLosslessNonHierarchicalProcess14;
	DicomFile^ file = CreateFile(256, 256, "MONOCHROME1", 8, 16, false, 1);
	LosslessImageTestWithBitsAllocatedConversion(syntax, file);
}

void DicomJpegCodecTest::DicomJpegLossless14SV1CodecTest()
{
	TransferSyntax^ syntax = TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1;
	DicomFile^ file = CreateFile(512, 512, "MONOCHROME1", 12, 16, false, 1);
	LosslessImageTest(syntax, file);

	// Lossy test for signed, becaue it converts
	file = CreateFile(512, 512, "MONOCHROME1", 12, 16, true, 1);
	LosslessImageTestWithConversion(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 1);
	LosslessImageTestWithConversion(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, true, 1);
	LosslessImageTestWithConversion(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 15, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 14, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 13, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 12, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 11, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 10, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 9, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "YBR_FULL", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "YBR_FULL", 8, 8, false, 5);
	LosslessImageTest(syntax, file);
}

void DicomJpegCodecTest::DicomJpegLossless14SV1CodecTest_8BitsStored16BitsAllocated()
{
	TransferSyntax^ syntax = TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1;
	DicomFile^ file = CreateFile(256, 256, "MONOCHROME1", 8, 16, false, 1);
	LosslessImageTestWithBitsAllocatedConversion(syntax, file);
}

}
}
}
}

#endif
