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

using System;
using System.Globalization;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
    /// <summary>
    /// Generates pixel data for an image streaming response.
    /// </summary>
    [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    class PixelDataMimeTypeProcessor : IImageMimeTypeProcessor
    {
        #region IImageMimeTypeProcessor Members
        public string OutputMimeType
        {
            get { return "application/clearcanvas"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            Platform.CheckForNullReference(context, "context");

            DicomPixelData pd = context.PixelData;
            MimeTypeProcessorOutput output = new MimeTypeProcessorOutput();
            int frame = context.FrameNumber;

            if (context.FrameNumber < 0)
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("Requested FrameNumber {0} cannot be negative.", frame));
            }
            else if (frame >= pd.NumberOfFrames)
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("Requested FrameNumber {0} exceeds the number of frames in the image.", frame));
            }

            output.ContentType = OutputMimeType;

            PixelDataLoader loader = new PixelDataLoader(context);
            output.Output = loader.ReadFrame(frame);
            output.IsLast = (pd.NumberOfFrames == frame + 1);

            // note: the transfer syntax of the returned pixel data may not be the same as that in the original image.
            // In the future, the clients may specify different transfer syntaxes which may mean the compressed image must be decompressed or vice versa. 
            TransferSyntax transferSyntax = pd.TransferSyntax;
            output.IsCompressed = transferSyntax.LosslessCompressed || transferSyntax.LossyCompressed;
            
            #region Special Code

            // Note: this block of code inject special header fields to assist the clients handling the images
            // For eg, the

            if (output.IsLast)
                context.Response.Headers.Add("IsLast", "true");

            if (output.IsCompressed)
            {
                // Fields that can be used by the web clients to decompress the compressed images streamed by the server.

                context.Response.Headers.Add("Compressed", "true");
                context.Response.Headers.Add("TransferSyntaxUid", pd.TransferSyntax.UidString);

                context.Response.Headers.Add("BitsAllocated", pd.BitsAllocated.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("BitsStored", pd.BitsStored.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("DerivationDescription", pd.DerivationDescription);

                context.Response.Headers.Add("HighBit", pd.HighBit.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("ImageHeight", pd.ImageHeight.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("ImageWidth", pd.ImageWidth.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("LossyImageCompression", pd.LossyImageCompression);
                context.Response.Headers.Add("LossyImageCompressionMethod", pd.LossyImageCompressionMethod);
                context.Response.Headers.Add("LossyImageCompressionRatio", pd.LossyImageCompressionRatio.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("NumberOfFrames", pd.NumberOfFrames.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("PhotometricInterpretation", pd.PhotometricInterpretation);
                context.Response.Headers.Add("PixelRepresentation", pd.PixelRepresentation.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("PlanarConfiguration", pd.PlanarConfiguration.ToString(CultureInfo.InvariantCulture));
                context.Response.Headers.Add("SamplesPerPixel", pd.SamplesPerPixel.ToString(CultureInfo.InvariantCulture));

            }

            #endregion

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
            {
                Platform.Log(LogLevel.Debug, "Streaming {0} pixel data: {1} x {2} x {3} , {4} bits  [{5} KB] ({6})",
                         output.IsCompressed ? "compressed" : "uncompressed",
                         pd.ImageHeight,
                         pd.ImageWidth,
                         pd.SamplesPerPixel,
                         pd.BitsStored,
                         output.Output.Length/1024,
                         pd.TransferSyntax.Name);
            }


            return output;
        }
        

        #endregion
    }
}