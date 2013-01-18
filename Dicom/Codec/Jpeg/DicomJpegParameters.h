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


#ifndef __DCMJPEGPARAMETERS_H__
#define __DCMJPEGPARAMETERS_H__

#pragma once

using namespace System;
using namespace System::IO;

using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom;

#include "JpegCodec.h"

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {

public enum class JpegSampleFactor {
	SF444,
	SF422,
	Unknown
};

public ref class DicomJpegParameters : public DicomCodecParameters {
private:
	int _quality;
	int _smoothing;
	bool _convertColorspace;
	JpegSampleFactor _sample;
	int _predictor;
	int _pointTransform;

public:
	DicomJpegParameters() {
		_quality = 90;
		_smoothing = 0;
		_convertColorspace = true;
		_sample = JpegSampleFactor::SF444;
		_predictor = 1;
		_pointTransform = 0;
	}

	///<summary>
	///The quality factor (0-100) for compression.  Default is 90.
	///</summary>
	property int Quality {
		int get() { return _quality; }
		void set(int value) { _quality = value; }
	}

	property int SmoothingFactor {
		int get() { return _smoothing; }
		void set(int value) { _smoothing = value; }
	}

	///<summary>
	///Convert YBR encoded compressed pixel to RGB on decompress
	///</summary>
	property bool ConvertYBRtoRGB {
		bool get() { return _convertColorspace; }
		void set(bool value) { _convertColorspace = value; }
	}

	///<summary>
	/// The YBR Sampling factor for compression of color images.
	///</summary>
	property JpegSampleFactor SampleFactor {
		JpegSampleFactor get() { return _sample; }
		void set(JpegSampleFactor value) { _sample = value; }
	}

	property int Predictor {
		int get() { return _predictor; }
		void set(int value) { _predictor = value; }
	}

	property int PointTransform {
		int get() { return _pointTransform; }
		void set(int value) { _pointTransform = value; }
	}
};

} // Jpeg
} // Codec
} // Dicom
} // ClearCanvas

#endif