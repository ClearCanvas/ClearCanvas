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


#ifndef __DCMJPEG2000CODEC_H__
#define __DCMJPEG2000CODEC_H__

#pragma once

using namespace System;

using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg2000 {

	public ref class DicomJpeg2000Parameters : public DicomCodecParameters {
	private:
		bool _irreversible;
		float _rate;
		bool _isVerbose;
		bool _enableMct;
		bool _updatePmi;
		bool _enablebypass;
		bool _enablereset;
		bool _enablerestart;
		bool _enablevsc;
		bool _enableerterm;
		bool _enablesegmark;

	public:
		DicomJpeg2000Parameters() {
			_irreversible = false;
			_rate = 8;
			_isVerbose = false;
			_enableMct = true;
			_updatePmi = true;
			_enablebypass = false;
			_enablereset = false;
			_enablerestart = false;
			_enablevsc = false;
			_enableerterm = false;
			_enablesegmark = false;
		}

		property bool Irreversible {
			bool get() { return _irreversible; }
			void set(bool value) { _irreversible = value; }
		}

		///<summary>
		///The compression rate.  Default value: 8
		///</summary>
		property float Rate {
			float get() { return _rate; }
			void set(float value) { _rate = value; }
		}

		property bool IsVerbose {
			bool get() { return _isVerbose; }
			void set(bool value) { _isVerbose = value; }
		}

		property bool EnableBypass {
			bool get() { return _enablebypass; }
			void set(bool value) { _enablebypass = value; }
		}

		property bool EnableReset {
			bool get() { return _enablereset; }
			void set(bool value) { _enablereset = value; }
		}

		property bool EnableRestart {
			bool get() { return _enablerestart; }
			void set(bool value) { _enablerestart = value; }
		}

		property bool EnableVsc {
			bool get() { return _enablevsc; }
			void set(bool value) { _enablevsc = value; }
		}

		///<summary>
		/// Enable predictable termination of code blocks
		///</summary>
		property bool EnableErterm {
			bool get() { return _enableerterm; }
			void set(bool value) { _enableerterm = value; }
		}

		///<summary>
		/// Enable segment markers in images.
		///</summary>
		property bool EnableSegmark {
			bool get() { return _enablesegmark; }
			void set(bool value) { _enablesegmark = value; }
		}

		/// <summary>
		/// Multi-component transorm enabled, ie, transform to YBR
		/// </summary>
		property bool AllowMCT {
			bool get() { return _enableMct; }
			void set(bool value) { _enableMct = value; }
		}

		property bool UpdatePhotometricInterpretation {
			bool get() { return _updatePmi; }
			void set(bool value) { _updatePmi = value; }
		}
	};


	public ref class DicomJpeg2000Codec abstract : public IDicomCodec
	{
	public:
		virtual property String^ Name { String^ get(); };
		virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { ClearCanvas::Dicom::TransferSyntax^ get(); };

		virtual DicomCodecParameters^ GetDefaultParameters() {
			return gcnew DicomJpeg2000Parameters();
		}

		virtual void Encode(DicomUncompressedPixelData^ oldPixelData, DicomCompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);
		virtual void Decode(DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);
		virtual void DecodeFrame(int frame, DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);
	};

	public ref class DicomJpeg2000LossyCodec : public DicomJpeg2000Codec
	{
	public:
	    property String^ Name { virtual String^ get() override;}
	    property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { virtual ClearCanvas::Dicom::TransferSyntax^ get() override; };
	};

	public ref class DicomJpeg2000LosslessCodec : public DicomJpeg2000Codec
	{
	public:
	    property String^ Name { virtual String^ get() override;}
	    property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { virtual ClearCanvas::Dicom::TransferSyntax^ get() override; };
	};

} // Jpeg2000
} // Codec
} // Dicom
} // ClearCanvas

#endif