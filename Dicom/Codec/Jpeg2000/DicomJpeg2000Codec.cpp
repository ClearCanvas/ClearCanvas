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


#include "stdio.h"
#include "string.h"

#include "DicomJpeg2000Codec.h"

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

using namespace ClearCanvas::Common;
using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;

extern "C" {
#include "OpenJPEG/openjpeg.h"
#include "OpenJPEG/j2k.h"

	void opj_error_callback(const char *msg, void *usr) {
		Platform::Log(LogLevel::Error, "OpenJPEG: {0}", gcnew String(msg));
	}

	void opj_warning_callback(const char *msg, void *) {
		Platform::Log(LogLevel::Warn, "OpenJPEG Warning: {0}", gcnew String(msg));
	}

	void opj_info_callback(const char *msg, void *) {
		Platform::Log(LogLevel::Info, "OpenJPEG: {0}", gcnew String(msg));
	}
}
namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg2000 {

OPJ_COLOR_SPACE getOpenJpegColorSpace(String^ photometricInterpretation) {
	if (photometricInterpretation == "RGB")
		return CLRSPC_SRGB;
	else if (photometricInterpretation == "MONOCHROME1" || photometricInterpretation == "MONOCHROME2")
		return CLRSPC_GRAY;
	else if (photometricInterpretation == "PALETTE COLOR")
		return CLRSPC_GRAY;
	else if (photometricInterpretation == "YBR_FULL" || photometricInterpretation == "YBR_FULL_422" || photometricInterpretation == "YBR_PARTIAL_422")
		return CLRSPC_SYCC;
	else
		return CLRSPC_UNKNOWN;
}

	String^ DicomJpeg2000Codec::Name::get()
	{ 
		return nullptr;
	}
	ClearCanvas::Dicom::TransferSyntax^ DicomJpeg2000Codec::CodecTransferSyntax::get()
	{ 
		return nullptr;
	}
	
	ClearCanvas::Dicom::TransferSyntax^ DicomJpeg2000LossyCodec::CodecTransferSyntax::get()  {
		return TransferSyntax::Jpeg2000ImageCompression;
	}
	String^ DicomJpeg2000LossyCodec::Name::get()  {
		return TransferSyntax::Jpeg2000ImageCompression->Name;	
	}
	ClearCanvas::Dicom::TransferSyntax^ DicomJpeg2000LosslessCodec::CodecTransferSyntax::get()  {
		return TransferSyntax::Jpeg2000ImageCompressionLosslessOnly;
	}
	String^ DicomJpeg2000LosslessCodec::Name::get()  {
		return TransferSyntax::Jpeg2000ImageCompressionLosslessOnly->Name;	
	}


void DicomJpeg2000Codec::Encode(DicomUncompressedPixelData^ oldPixelData, DicomCompressedPixelData^ newPixelData, DicomCodecParameters^ parameters) {
	if ((oldPixelData->PhotometricInterpretation == "YBR_FULL_422")    ||
		(oldPixelData->PhotometricInterpretation == "YBR_PARTIAL_422") ||
		(oldPixelData->PhotometricInterpretation == "YBR_PARTIAL_420"))
		throw gcnew DicomCodecUnsupportedSopException(String::Format("Photometric Interpretation '{0}' not supported by JPEG 2000 encoder",
														oldPixelData->PhotometricInterpretation));

	DicomJpeg2000Parameters^ jparams = (DicomJpeg2000Parameters^)parameters;
	if (jparams == nullptr)
	{
		jparams = (DicomJpeg2000Parameters^)GetDefaultParameters();
		if (newPixelData->TransferSyntax->Equals(TransferSyntax::Jpeg2000ImageCompression))
			jparams->Irreversible = true;
	}

	// Convert to RGB
	if (oldPixelData->HasPaletteColorLut && jparams->ConvertPaletteToRGB)
	{
		oldPixelData->ConvertPaletteColorToRgb();
		newPixelData->HasPaletteColorLut = false;
		newPixelData->SamplesPerPixel = oldPixelData->SamplesPerPixel;
		newPixelData->PlanarConfiguration = oldPixelData->PlanarConfiguration;
		newPixelData->PhotometricInterpretation = oldPixelData->PhotometricInterpretation;
	}

	int pixelCount = oldPixelData->ImageHeight * oldPixelData->ImageWidth;

	for (int frame = 0; frame < oldPixelData->NumberOfFrames; frame++) {
		array<unsigned char>^ frameArray = oldPixelData->GetFrame(frame);
		pin_ptr<unsigned char> framePin = &frameArray[0];
		unsigned char* frameData = framePin;
		int frameDataSize = frameArray->Length;

		opj_image_cmptparm_t cmptparm[3];
		opj_cparameters_t eparams;  /* compression parameters */
		opj_event_mgr_t event_mgr;  /* event manager */
		opj_cinfo_t* cinfo = NULL;  /* handle to a compressor */
		opj_image_t *image = NULL;
		opj_cio_t *cio = NULL;

		memset(&event_mgr, 0, sizeof(opj_event_mgr_t));
		event_mgr.error_handler = opj_error_callback;
		if (jparams->IsVerbose) {
			event_mgr.warning_handler = opj_warning_callback;
			event_mgr.info_handler = opj_info_callback;
		}			

		cinfo = opj_create_compress(CODEC_J2K);

		opj_set_event_mgr((opj_common_ptr)cinfo, &event_mgr, NULL);

		opj_set_default_encoder_parameters(&eparams);

		// Progression order by quality
		eparams.prog_order = LRCP;
		// Use Tiles?
		eparams.tile_size_on=false;
		// Tile Size
		eparams.cp_tdx=1;
		eparams.cp_tdy=1;
		// Tile Origin
		eparams.cp_tx0=0;
		eparams.cp_ty0=0;

		eparams.numresolution = 6;

		// Image Offset
		eparams.image_offset_x0 = 0;
		eparams.image_offset_y0 = 0;

		// Mode
		eparams.mode = (jparams->EnableBypass ? 1 : 0) + (jparams->EnableReset ? 2 : 0)
			+ (jparams->EnableRestart ? 4 : 0) + (jparams->EnableVsc ? 8 : 0)
			+ (jparams->EnableErterm ? 16 : 0) + (jparams->EnableSegmark ? 32 : 0);

		// Comment
		eparams.cp_comment = "ClearCanvas DICOM OpenJPEG";

		if (newPixelData->TransferSyntax->Equals(TransferSyntax::Jpeg2000ImageCompression) && jparams->Irreversible) {
			eparams.irreversible = 1;
			eparams.tcp_rates[0] = jparams->Rate;
			eparams.tcp_numlayers = 1;
			eparams.cp_disto_alloc = 1;

		} else {
			eparams.irreversible = 0;
			eparams.tcp_rates[0] = 1; // Lossless = 1
			eparams.tcp_numlayers = 1;
			eparams.cp_disto_alloc = 1;
		}

		if (oldPixelData->PhotometricInterpretation == "RGB" && jparams->AllowMCT)
			eparams.tcp_mct = 1;

		memset(&cmptparm[0], 0, sizeof(opj_image_cmptparm_t) * 3);
		for (int i = 0; i < oldPixelData->SamplesPerPixel; i++) {
			cmptparm[i].bpp = oldPixelData->BitsAllocated;
			cmptparm[i].prec = oldPixelData->BitsStored;
			cmptparm[i].sgnd = oldPixelData->IsSigned ? 1 : 0;
			cmptparm[i].dx = eparams.subsampling_dx;
			cmptparm[i].dy = eparams.subsampling_dy;
			cmptparm[i].h = oldPixelData->ImageHeight;
			cmptparm[i].w = oldPixelData->ImageWidth;
		}

		try {
			OPJ_COLOR_SPACE color_space = getOpenJpegColorSpace(oldPixelData->PhotometricInterpretation);
			image = opj_image_create(oldPixelData->SamplesPerPixel, &cmptparm[0], color_space);

			image->x0 = 0;
			image->y0 = 0;
			image->x1 =	image->x0 + ((oldPixelData->ImageWidth - 1) * eparams.subsampling_dx) + 1;
			image->y1 =	image->y0 + ((oldPixelData->ImageHeight - 1) * eparams.subsampling_dy) + 1;

			for (int c = 0; c < image->numcomps; c++) {
				opj_image_comp_t* comp = &image->comps[c];

				int pos = 0;
				int offset = 0;

				if (oldPixelData->IsPlanar) {
					pos = c * pixelCount;
					offset = 1;
				}
				else {
					pos = c;
					offset = image->numcomps;
				}

				if (oldPixelData->BytesAllocated == 1) {
					if (oldPixelData->IsSigned) {
						if (oldPixelData->BitsStored < 8)
						{
							int shiftBits = 8 - oldPixelData->BitsStored;
							for (int p = 0; p < pixelCount; p++) {
								char pixel = ((char)(frameData[pos] << shiftBits)) >> shiftBits;
								comp->data[p] = (int)pixel;
								pos += offset;
							}
						}
						else
						{
							for (int p = 0; p < pixelCount; p++) {
								comp->data[p] = (int)((char)frameData[pos]);
								pos += offset;
							}
						}
					}
					else {
						for (int p = 0; p < pixelCount; p++) {
							comp->data[p] = frameData[pos];
							pos += offset;
						}
					}
				}
				else if (oldPixelData->BytesAllocated == 2) {
					if (oldPixelData->IsSigned) {
						if (oldPixelData->BitsStored < 16)
						{
							int shiftBits = 16 - oldPixelData->BitsStored;
							unsigned short* frameData16 = (unsigned short*)frameData;
							for (int p = 0; p < pixelCount; p++) {
								short pixel = ((short)(frameData16[pos] << shiftBits)) >> shiftBits;
								comp->data[p] = (int)pixel;
								pos += offset;
							}
						}
						else
						{
							short* frameData16 = (short*)frameData;
							for (int p = 0; p < pixelCount; p++) {
								comp->data[p] = (int)frameData16[pos];
								pos += offset;
							}
						}
					}
					else {
						unsigned short* frameData16 = (unsigned short*)frameData;
						for (int p = 0; p < pixelCount; p++) {
							comp->data[p] = frameData16[pos];
							pos += offset;
						}
					}
				}
				else
					throw gcnew DicomCodecUnsupportedSopException("JPEG 2000 codec only supports Bits Allocated == 8 or 16");
			}

			opj_setup_encoder(cinfo, &eparams, image);

			cio = opj_cio_open((opj_common_ptr)cinfo, NULL, 0);

			if (opj_encode(cinfo, cio, image, NULL)) {
				int clen = cio_tell(cio);
				// Force the output fragment/frame to be even length
				array<unsigned char>^ cbuf = gcnew array<unsigned char>(clen%2==1 ? clen+1 : clen);
				Marshal::Copy((IntPtr)cio->buffer, cbuf, 0, clen);
				newPixelData->AddFrameFragment(cbuf);
			} else
				throw gcnew DicomCodecException("Unable to JPEG 2000 encode image");
		}
		finally {
			if (cio != nullptr)
				opj_cio_close(cio);
			if (image != nullptr)
				opj_image_destroy(image);
			if (cinfo != nullptr)
				opj_destroy_compress(cinfo);
		}
	}

	if (oldPixelData->PhotometricInterpretation == "RGB" && jparams->AllowMCT) {
		if (jparams->UpdatePhotometricInterpretation) {
			if (newPixelData->TransferSyntax->Equals(TransferSyntax::Jpeg2000ImageCompressionLosslessOnly))
				newPixelData->PhotometricInterpretation = "YBR_RCT";
			else
				newPixelData->PhotometricInterpretation = "YBR_ICT";
		}
	}

	if (newPixelData->TransferSyntax->Equals(TransferSyntax::Jpeg2000ImageCompression) && jparams->Irreversible) {
		newPixelData->LossyImageCompressionMethod = "ISO_15444_1";
		
		double oldSize = oldPixelData->BitsStoredFrameSize;
		double newSize = newPixelData->GetCompressedFrameSize(0);
		newPixelData->LossyImageCompressionRatio = (float) (oldSize / newSize);
		newPixelData->LossyImageCompression = "01";
		newPixelData->DerivationDescription = String::Format("OpenJPEG Compressed: {0:0.000}:1", oldSize / newSize);
	}

}
void DicomJpeg2000Codec::DecodeFrame(int frame, DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters) {
	DicomJpeg2000Parameters^ jparams = (DicomJpeg2000Parameters^)parameters;
	if (jparams == nullptr)
		jparams = (DicomJpeg2000Parameters^)GetDefaultParameters();

	array<unsigned char>^ destArray = gcnew array<unsigned char>(oldPixelData->UncompressedFrameSize);
	pin_ptr<unsigned char> destPin = &destArray[0];
	unsigned char* destData = destPin;
	int destDataSize = destArray->Length;

	int pixelCount = oldPixelData->ImageHeight * oldPixelData->ImageWidth;

	if (newPixelData->PhotometricInterpretation == "YBR_RCT" || newPixelData->PhotometricInterpretation == "YBR_ICT")
		newPixelData->PhotometricInterpretation = "RGB";

	array<unsigned char>^ jpegArray = oldPixelData->GetFrameFragmentData(frame);
	pin_ptr<unsigned char> jpegPin = &jpegArray[0];
	unsigned char* jpegData = jpegPin;
	int jpegDataSize = jpegArray->Length;

	opj_dparameters_t dparams;
	opj_event_mgr_t event_mgr;
	opj_image_t *image = NULL;
	opj_dinfo_t* dinfo = NULL;
	opj_cio_t *cio = NULL;

	memset(&event_mgr, 0, sizeof(opj_event_mgr_t));
	event_mgr.error_handler = opj_error_callback;
	if (jparams->IsVerbose) {
		event_mgr.warning_handler = opj_warning_callback;
		event_mgr.info_handler = opj_info_callback;
	}

	opj_set_default_decoder_parameters(&dparams);
	dparams.cp_layer=0;
	dparams.cp_reduce=0;

	try {
		dinfo = opj_create_decompress(CODEC_J2K);

		opj_set_event_mgr((opj_common_ptr)dinfo, &event_mgr, NULL);

		opj_setup_decoder(dinfo, &dparams);

		bool opj_err = false;
		dinfo->client_data = (void*)&opj_err;

		cio = opj_cio_open((opj_common_ptr)dinfo, jpegData, (int)jpegDataSize);
		image = opj_decode(dinfo, cio);

		if (image == nullptr)
			throw gcnew DicomCodecException("Error in JPEG 2000 code stream!");

		for (int c = 0; c < image->numcomps; c++) {
			opj_image_comp_t* comp = &image->comps[c];

			int pos = 0;
			int offset = 0;

			if (oldPixelData->IsPlanar) {
				pos = c * pixelCount;
				offset = 1;
			}
			else {
				pos = c;
				offset = image->numcomps;
			}

			if (oldPixelData->BytesAllocated == 1) {
				if (oldPixelData->IsSigned) {
					unsigned char signBit = 1 << oldPixelData->HighBit;
					unsigned char maskBit = 0xFF ^ signBit;
					for (int p = 0; p < pixelCount; p++) {
						int i = comp->data[p];
						if (i < 0)
							destArray[pos] = (unsigned char)((i & maskBit) | signBit);
						else
							destArray[pos] = (unsigned char)(i & maskBit);
						pos += offset;
					}
				}
				else {
					for (int p = 0; p < pixelCount; p++) {
						destArray[pos] = (unsigned char)comp->data[p];
						pos += offset;
					}
				}
			}
			else if (oldPixelData->BytesAllocated == 2) {
				unsigned short signBit = 1 << oldPixelData->HighBit;
				unsigned short maskBit = 0xFFFF ^ signBit;
				unsigned short* destData16 = (unsigned short*)destData;
				if (oldPixelData->IsSigned) {
					for (int p = 0; p < pixelCount; p++) {
						int i = comp->data[p];
						if (i < 0)
							destData16[pos] = (unsigned short)((i & maskBit) | signBit);
						else
							destData16[pos] = (unsigned short)(i & maskBit);
						pos += offset;
					}
				}
				else {
					for (int p = 0; p < pixelCount; p++) {
						destData16[pos] = (unsigned short)comp->data[p];
						pos += offset;
					}
				}
			}
			else
				throw gcnew DicomCodecUnsupportedSopException("JPEG 2000 module only supports Bytes Allocated == 8 or 16!");
		}

		newPixelData->AppendFrame(destArray);
	}
	finally {
		if (cio != nullptr)
			opj_cio_close(cio);
		if (dinfo != nullptr)
			opj_destroy_decompress(dinfo);
		if (image != nullptr)
			opj_image_destroy(image);
	}
	
}

void DicomJpeg2000Codec::Decode(DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters) {
	DicomJpeg2000Parameters^ jparams = (DicomJpeg2000Parameters^)parameters;
	if (jparams == nullptr)
		jparams = (DicomJpeg2000Parameters^)GetDefaultParameters();

	for (int frame = 0; frame < oldPixelData->NumberOfFrames; frame++) {
		DecodeFrame(frame,oldPixelData,newPixelData,parameters);
	}
}

} // Jpeg2000
} // Codec
} // Dicom
} // ClearCanvas