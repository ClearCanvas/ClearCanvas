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


#include "DicomJpeg2000CodecFactory.h"
#include "DicomJpeg2000Codec.h"

using namespace System;
using namespace System::IO;

using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom;

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg2000 {
	
	//DicomJpeg2000LosslessCodecFactory
	ClearCanvas::Dicom::TransferSyntax^ DicomJpeg2000LosslessCodecFactory::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::Jpeg2000ImageCompressionLosslessOnly;
	}
	String^ DicomJpeg2000LosslessCodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::Jpeg2000ImageCompressionLosslessOnly->Name;	
	}
	bool DicomJpeg2000LosslessCodecFactory::Enabled::get()  {
		return true;
	}
	DicomCodecParameters^ DicomJpeg2000LosslessCodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		return gcnew DicomJpeg2000Parameters();
	}
	DicomCodecParameters^ DicomJpeg2000LosslessCodecFactory::GetCodecParameters(XmlDocument^ parms)
    {
		DicomJpeg2000Parameters^ codecParms = gcnew DicomJpeg2000Parameters();
		codecParms->Irreversible = false;
		codecParms->UpdatePhotometricInterpretation = true;
		codecParms->Rate = 1; //1 == Lossless

		XmlElement^ element = parms->DocumentElement;
		if (element->Attributes["convertFromPalette"])
		{
			String^ boolString = element->Attributes["convertFromPalette"]->Value;
			bool convert;
			if (false == bool::TryParse(boolString, convert))
				throw gcnew ApplicationException("Invalid convertFromPalette specified for JPEG 2000 Lossless: " + boolString);
			codecParms->ConvertPaletteToRGB = convert;
		}
		else
			codecParms->ConvertPaletteToRGB = true;

		return codecParms;
	}
	IDicomCodec^ DicomJpeg2000LosslessCodecFactory::GetDicomCodec() {
		return gcnew DicomJpeg2000LosslessCodec();
	}

	//DicomJpeg2000LossyCodecFactory
	TransferSyntax^ DicomJpeg2000LossyCodecFactory::CodecTransferSyntax::get()  {
		return TransferSyntax::Jpeg2000ImageCompression;
	}

	String^ DicomJpeg2000LossyCodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::Jpeg2000ImageCompression->Name;	
	}

	bool DicomJpeg2000LossyCodecFactory::Enabled::get()  {
		return true;
	}

	DicomCodecParameters^ DicomJpeg2000LossyCodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		DicomJpeg2000Parameters^ codecParms = gcnew DicomJpeg2000Parameters();

		codecParms->Irreversible = true;
		codecParms->UpdatePhotometricInterpretation = true;
		codecParms->Rate = 5.0;
		return codecParms;
	}

	DicomCodecParameters^ DicomJpeg2000LossyCodecFactory::GetCodecParameters(XmlDocument^ parms)
    {
		DicomJpeg2000Parameters^ codecParms = gcnew DicomJpeg2000Parameters();

		codecParms->Irreversible = true;
		codecParms->UpdatePhotometricInterpretation = true;

		XmlElement^ element = parms->DocumentElement;

		String^ ratioString = element->Attributes["ratio"]->Value;
		float ratio;
		if (false == float::TryParse(ratioString, ratio))
			throw gcnew ApplicationException("Invalid compression ratio specified for JPEG 2000 Lossy: " + ratioString);

		codecParms->Rate = ratio;

		if (element->Attributes["convertFromPalette"])
		{
			String^ boolString = element->Attributes["convertFromPalette"]->Value;
			bool convert;
			if (false == bool::TryParse(boolString, convert))
				throw gcnew ApplicationException("Invalid convertFromPalette specified for JPEG 2000 Lossy: " + boolString);
			codecParms->ConvertPaletteToRGB = convert;
		}
		else
			codecParms->ConvertPaletteToRGB = true;

		return codecParms;
	}
	IDicomCodec^ DicomJpeg2000LossyCodecFactory::GetDicomCodec() {
		return gcnew DicomJpeg2000LossyCodec();
	}

} // Jpeg2000
} // Codec
} // Dicom
} // ClearCanvas
