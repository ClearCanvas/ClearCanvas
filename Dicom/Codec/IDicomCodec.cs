#region License

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

#endregion

using System.Xml;

namespace ClearCanvas.Dicom.Codec
{
	//TODO: empty class - just return an object?

	/// <summary>
    /// Abstract base class for parameters to codecs.
    /// </summary>
    public abstract class DicomCodecParameters
    {
        #region Private Members

		#endregion

        #region Public Members
        #endregion

        #region Public Properties

		/// <summary>
		/// Specifies if Palette Color images should be converted to RGB for compression.
		/// </summary>
		public bool ConvertPaletteToRGB { get; set; }

		#endregion
    }

    /// <summary>
    /// Interface for Dicom Compressor/Decompressors.
    /// </summary>
    public interface IDicomCodec
    {
        /// <summary>
        /// The name of the Codec
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The <see cref="CodecTransferSyntax"/> the codec supports.
        /// </summary>
        TransferSyntax CodecTransferSyntax { get; }
        /// <summary>
        /// Encode (compress) the entire pixel data.
        /// </summary>
        /// <param name="oldPixelData">The uncompressed pixel data</param>
        /// <param name="newPixelData">The output compressed pixel data</param>
        /// <param name="parameters">The codec parameters</param>
        void Encode(DicomUncompressedPixelData oldPixelData, DicomCompressedPixelData newPixelData, DicomCodecParameters parameters);
        /// <summary>
        /// Decode (decompress) the entire pixel data.
        /// </summary>
        /// <param name="oldPixelData">The source compressed pixel data.</param>
        /// <param name="newPixelData">The output pixel data.</param>
        /// <param name="parameters">The codec parameters.</param>
        void Decode(DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters);

        /// <summary>
        /// Decode a single frame of pixel data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note this method is strictly used with the <see cref="DicomCompressedPixelData"/> class's
        /// GetFrame() method to compress data frame by frame.  It is expected that the frame data will be output into 
        /// <paramref name="newPixelData"/> as a single frame of data.
        /// </para>
        /// <para>
        /// If a DICOM file is loaded with the <see cref="DicomReadOptions.StorePixelDataReferences"/>
        /// option set, this method in conjunction with the <see cref="DicomCompressedPixelData"/> 
        /// class can allow the library to only load a frame of data at a time.
        /// </para>
        /// </remarks>
        /// <param name="frame">A zero offset frame number</param>
        /// <param name="oldPixelData">The input pixel data (including all frames)</param>
        /// <param name="newPixelData">The output pixel data is stored here</param>
        /// <param name="parameters">The codec parameters</param>
        void DecodeFrame(int frame, DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters);	

    }

    /// <summary>
    /// Interface for factory for creating DICOM Compressors/Decompressors.
    /// </summary>
    public interface IDicomCodecFactory
    {
        /// <summary>
        /// The name of the factory.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets a value indicating whether or not the codec factory is enabled.
        /// </summary>
        bool Enabled { get; }
        /// <summary>
        /// The transfer syntax associated with the factory.
        /// </summary>
        TransferSyntax CodecTransferSyntax { get; }
        /// <summary>
        /// Get the codec parameters.
        /// </summary>
        /// <param name="dataSet">The data set to get codec parameters for.  Note that this value may be null.</param>
        /// <returns>The codec parameters.</returns>
        DicomCodecParameters GetCodecParameters(DicomAttributeCollection dataSet);
		/// <summary>
		/// Get the codec parameters.
		/// </summary>
		/// <param name="parms">XML based codec parameters.</param>
		/// <returns>The codec parameters.</returns>
		DicomCodecParameters GetCodecParameters(XmlDocument parms);
        /// <summary>
        /// Get an <see cref="IDicomCodec"/> codec.
        /// </summary>
        /// <returns></returns>
        IDicomCodec GetDicomCodec();
    }
}
