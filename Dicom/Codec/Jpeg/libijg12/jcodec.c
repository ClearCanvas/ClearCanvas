#pragma region License (non-CC)

// This source code contains the work of the Independent JPEG Group.
// Please see accompanying notice in code comments and/or readme file
// for the terms of distribution and use regarding this code.

#pragma endregion

/*
 * jcodec.c
 *
 * Copyright (C) 1998, Thomas G. Lane.
 * This file is part of the Independent JPEG Group's software.
 * For conditions of distribution and use, see the accompanying README file.
 *
 * This file contains utility functions for the JPEG codec(s).
 */

#define JPEG_INTERNALS
#include "jinclude12.h"
#include "jpeglib12.h"
#include "jlossy12.h"
#include "jlossls12.h"


/*
 * Initialize the compression codec.
 * This is called only once, during master selection.
 */

GLOBAL(void)
jinit_c_codec (j_compress_ptr cinfo)
{
  if (cinfo->process == JPROC_LOSSLESS) {
#ifdef C_LOSSLESS_SUPPORTED
    jinit_lossless_c_codec(cinfo);
#else
    ERREXIT(cinfo, JERR_NOT_COMPILED);
#endif
  } else
    jinit_lossy_c_codec(cinfo);
}


/*
 * Initialize the decompression codec.
 * This is called only once, during master selection.
 */

GLOBAL(void)
jinit_d_codec (j_decompress_ptr cinfo)
{
  if (cinfo->process == JPROC_LOSSLESS) {
#ifdef D_LOSSLESS_SUPPORTED
    jinit_lossless_d_codec(cinfo);
#else
    ERREXIT(cinfo, JERR_NOT_COMPILED);
#endif
  } else
    jinit_lossy_d_codec(cinfo);
}
