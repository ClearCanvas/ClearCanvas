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

#include <vector>
#include <vcclr.h>

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Iod;

#include "JpegCodec.h"

#define IJGVERS IJG16
#define JPEGCODEC Jpeg16Codec

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {

extern "C" {
#define boolean ijg_boolean
#include "stdio.h"
#include "string.h"
#include "setjmp.h"
#include "libijg16/jpeglib16.h"
#include "libijg16/jerror16.h"
#include "libijg16/jpegint16.h"
#undef boolean

// disable any preprocessor magic the IJG library might be doing with the "const" keyword
#ifdef const
#undef const
#endif
} // extern "C"

#include "JpegCodec.i"

} // Jpeg
} // Codec
} // Dicom
} // ClearCanvas