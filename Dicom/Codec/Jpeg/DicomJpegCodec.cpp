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

#pragma region GetJpegBitDepth() Copyright

/*
 *  The GetJpegBitDepth() method has this copyright:  
 *
 *  Copyright (C) 1997-2008, OFFIS
 *
 *  This software and supporting documentation were developed by
 *
 *    Kuratorium OFFIS e.V.
 *    Healthcare Information and Communication Systems
 *    Escherweg 2
 *    D-26121 Oldenburg, Germany
 *
 *  THIS SOFTWARE IS MADE AVAILABLE,  AS IS,  AND OFFIS MAKES NO  WARRANTY
 *  REGARDING  THE  SOFTWARE,  ITS  PERFORMANCE,  ITS  MERCHANTABILITY  OR
 *  FITNESS FOR ANY PARTICULAR USE, FREEDOM FROM ANY COMPUTER DISEASES  OR
 *  ITS CONFORMITY TO ANY SPECIFICATION. THE ENTIRE RISK AS TO QUALITY AND
 *  PERFORMANCE OF THE SOFTWARE IS WITH THE USER.
 */

#pragma endregion

#include "DicomJpegCodec.h"

using namespace System;
using namespace System::IO;

using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Common;

#include "JpegCodec.h"
#include "DicomJpegParameters.h"

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {

	String^ DicomJpegCodec::Name::get()
	{ 
		return nullptr;
	}
	ClearCanvas::Dicom::TransferSyntax^ DicomJpegCodec::CodecTransferSyntax::get()
	{ 
		return nullptr;
	}
	
	ClearCanvas::Dicom::TransferSyntax^ DicomJpegProcess1Codec::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegBaselineProcess1;
	}
	String^ DicomJpegProcess1Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegBaselineProcess1->Name;	
	}

    TransferSyntax^ DicomJpegProcess24Codec::CodecTransferSyntax::get()  {
		return TransferSyntax::JpegExtendedProcess24;
	}
	String^ DicomJpegProcess24Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegExtendedProcess24->Name;	
	}

	TransferSyntax^ DicomJpegLossless14Codec::CodecTransferSyntax::get()  {
		return TransferSyntax::JpegLosslessNonHierarchicalProcess14;
	}
	String^ DicomJpegLossless14Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalProcess14->Name;	
	}

    ClearCanvas::Dicom::TransferSyntax^ DicomJpegLossless14SV1Codec::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1;
	}
	String^ DicomJpegLossless14SV1Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1->Name;	
	}
	
	
void DicomJpegCodec::Encode(DicomUncompressedPixelData^ oldPixelData, DicomCompressedPixelData^ newPixelData, DicomCodecParameters^ parameters)
{
	if (parameters == nullptr) parameters = gcnew DicomJpegParameters();

	if (parameters->GetType() != DicomJpegParameters::typeid)
        throw gcnew DicomCodecException("Invalid codec parameters");

	DicomJpegParameters^ jparams = (DicomJpegParameters^)parameters;

	// Convert to RGB
	if (oldPixelData->HasPaletteColorLut && jparams->ConvertPaletteToRGB)
	{
		oldPixelData->ConvertPaletteColorToRgb();
		newPixelData->HasPaletteColorLut = false;
		newPixelData->SamplesPerPixel = oldPixelData->SamplesPerPixel;
		newPixelData->PlanarConfiguration = oldPixelData->PlanarConfiguration;
		newPixelData->PhotometricInterpretation = oldPixelData->PhotometricInterpretation;
	}

	IJpegCodec^ codec = GetCodec(oldPixelData->BitsStored, jparams);

	for (int frame = 0; frame < oldPixelData->NumberOfFrames; frame++) {
		codec->Encode(oldPixelData, newPixelData, jparams, frame);
	}

	if (codec->Mode != JpegMode::Lossless) {
		newPixelData->LossyImageCompressionMethod = "ISO_10918_1";
		
		float oldSize = (float)oldPixelData->BitsStoredFrameSize;
		float newSize = (float)newPixelData->GetCompressedFrameSize(0);
		String^ ratio = String::Format("{0:0.000}", oldSize / newSize);
		newPixelData->LossyImageCompressionRatio = (float) oldSize / newSize;
		newPixelData->LossyImageCompression = "01";
		newPixelData->DerivationDescription = String::Format("IJG Compressed: {0:0.000}:1", oldSize / newSize);
	}

	if (oldPixelData->PhotometricInterpretation == "RGB") {
		if (codec->Mode != JpegMode::Lossless) {
			if (jparams->SampleFactor == JpegSampleFactor::SF422)
				newPixelData->PhotometricInterpretation = "YBR_FULL_422";
			else if (jparams->SampleFactor == JpegSampleFactor::SF444)
				newPixelData->PhotometricInterpretation = "YBR_FULL";
		}
	}

	if (oldPixelData->BitsAllocated == 16 && oldPixelData->BitsStored == 8)
	{
		// #8940, forcing the Bits allocated to 8, so it can be more easily be decompressed.
		newPixelData->BitsAllocated = 8;
	}
}

void DicomJpegCodec::Decode(DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters)
{
	if (parameters == nullptr) parameters = gcnew DicomJpegParameters();

	if (parameters->GetType() != DicomJpegParameters::typeid)
		throw gcnew DicomCodecException("Invalid codec parameters");

	DicomJpegParameters^ jparams = (DicomJpegParameters^)parameters;


	for (int frame = 0; frame < oldPixelData->NumberOfFrames; frame++) {
		array<unsigned char>^ jpegData = oldPixelData->GetFrameFragmentData(frame);
		pin_ptr<unsigned char> jpegPin = &jpegData[0];
		unsigned char* jpegPtr = jpegPin;
		size_t jpegSize = jpegData->Length;
	
		unsigned char bitsStored = GetJpegBitDepth(jpegPtr,jpegData->Length);
		if (bitsStored != oldPixelData->BitsStored)
			Platform::Log(LogLevel::Warn,"Bit depth in jpeg data ({0}) doesn't match DICOM header bit depth ({1}).",
							bitsStored, oldPixelData->BitsStored);

		IJpegCodec^ codec = GetCodec(bitsStored, jparams);
		codec->Decode(oldPixelData, newPixelData, jparams, frame);	

		if (oldPixelData->PhotometricInterpretation->StartsWith("YBR_")) {
			if (jparams->ConvertYBRtoRGB) {
				newPixelData->PhotometricInterpretation = "RGB";
			}
		}
	}
}

void DicomJpegCodec::DecodeFrame(int frame, DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters)
{
	if (parameters == nullptr) parameters = gcnew DicomJpegParameters();

	if (parameters->GetType() != DicomJpegParameters::typeid)
		throw gcnew DicomCodecException("Invalid codec parameters");

	DicomJpegParameters^ jparams = (DicomJpegParameters^)parameters;

	array<unsigned char>^ jpegData = oldPixelData->GetFrameFragmentData(frame);
	pin_ptr<unsigned char> jpegPin = &jpegData[0];
	unsigned char* jpegPtr = jpegPin;
	size_t jpegSize = jpegData->Length;

	unsigned char bitsStored = GetJpegBitDepth(jpegPtr,jpegData->Length);
	if (bitsStored != oldPixelData->BitsStored)
		Platform::Log(LogLevel::Warn,"Bit depth in jpeg data ({0}) doesn't match DICOM header bit depth ({1}).",
						bitsStored, oldPixelData->BitsStored);

	IJpegCodec^ codec = GetCodec(bitsStored, jparams);
	codec->Decode(oldPixelData, newPixelData, jparams, frame);	

	if (oldPixelData->PhotometricInterpretation->StartsWith("YBR_")) {
		if (jparams->ConvertYBRtoRGB) {
			newPixelData->PhotometricInterpretation = "RGB";
		}
	}
}

unsigned short DicomJpegCodec::readUint16(const unsigned char *data)
{
  return (((unsigned short)(*data) << 8) | ((unsigned short)(*(data+1))));
}

unsigned char DicomJpegCodec::GetJpegBitDepth(
  const unsigned char *data,
  const unsigned int fragmentLength)
{
  unsigned int offset = 0;
  while(offset+4 < fragmentLength)
  {
    switch(readUint16(data+offset))
    {
      case 0xffc0: // SOF_0: JPEG baseline
        return data[offset+4];
        /* break; */
      case 0xffc1: // SOF_1: JPEG extended sequential DCT
        return data[offset+4];
        /* break; */
      case 0xffc2: // SOF_2: JPEG progressive DCT
        return data[offset+4];
        /* break; */
      case 0xffc3 : // SOF_3: JPEG lossless sequential
        return data[offset+4];
        /* break; */
      case 0xffc5: // SOF_5: differential (hierarchical) extended sequential, Huffman
        return data[offset+4];
        /* break; */
      case 0xffc6: // SOF_6: differential (hierarchical) progressive, Huffman
        return data[offset+4];
        /* break; */
      case 0xffc7: // SOF_7: differential (hierarchical) lossless, Huffman
        return data[offset+4];
        /* break; */
      case 0xffc8: // Reserved for JPEG extentions
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffc9: // SOF_9: extended sequential, arithmetic
        return data[offset+4];
        /* break; */
      case 0xffca: // SOF_10: progressive, arithmetic
        return data[offset+4];
        /* break; */
      case 0xffcb: // SOF_11: lossless, arithmetic
        return data[offset+4];
        /* break; */
      case 0xffcd: // SOF_13: differential (hierarchical) extended sequential, arithmetic
        return data[offset+4];
        /* break; */
      case 0xffce: // SOF_14: differential (hierarchical) progressive, arithmetic
        return data[offset+4];
        /* break; */
      case 0xffcf: // SOF_15: differential (hierarchical) lossless, arithmetic
        return data[offset+4];
        /* break; */
      case 0xffc4: // DHT
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffcc: // DAC
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffd0: // RST m
      case 0xffd1:
      case 0xffd2:
      case 0xffd3:
      case 0xffd4:
      case 0xffd5:
      case 0xffd6:
      case 0xffd7:
        offset +=2;
        break;
      case 0xffd8: // SOI
        offset +=2;
        break;
      case 0xffd9: // EOI
        offset +=2;
        break;
      case 0xffda: // SOS
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffdb: // DQT
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffdc: // DNL
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffdd: // DRI
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffde: // DHP
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffdf: // EXP
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xffe0: // APPn
      case 0xffe1:
      case 0xffe2:
      case 0xffe3:
      case 0xffe4:
      case 0xffe5:
      case 0xffe6:
      case 0xffe7:
      case 0xffe8:
      case 0xffe9:
      case 0xffea:
      case 0xffeb:
      case 0xffec:
      case 0xffed:
      case 0xffee:
      case 0xffef:
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xfff0: // JPGn
      case 0xfff1:
      case 0xfff2:
      case 0xfff3:
      case 0xfff4:
      case 0xfff5:
      case 0xfff6:
      case 0xfff7:
      case 0xfff8:
      case 0xfff9:
      case 0xfffa:
      case 0xfffb:
      case 0xfffc:
      case 0xfffd:
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xfffe: // COM
        offset += readUint16(data+offset+2)+2;
        break;
      case 0xff01: // TEM
        break;
      default:
        if ((data[offset]==0xff) && (data[offset+1]>2) && (data[offset+1] <= 0xbf)) // RES reserved markers
        {
          offset += 2;
        }
        else return 0; // syntax error, stop parsing
        break;
    }
  } // while
  return 0; // no SOF marker found
}

} // Jpeg
} // Codec
} // Dicom
} // ClearCanvas