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

using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
   [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    public class PdfMimeType : IImageMimeTypeProcessor
    {
        public string OutputMimeType
        {
            get { return "application/pdf"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            var output = new MimeTypeProcessorOutput
                             {
                                 ContentType = OutputMimeType
                             };

            var file = new DicomFile(context.ImagePath);
            file.Load(DicomReadOptions.StorePixelDataReferences);

            if (!file.SopClass.Equals(SopClass.EncapsulatedPdfStorage))
                throw new WADOException(HttpStatusCode.NotImplemented, "image/pdf is not supported for this type of object: " + file.SopClass.Name);

            var iod = new EncapsulatedDocumentModuleIod(file.DataSet);

            output.Output = iod.EncapsulatedDocument;
            return output;            
        }
    }
}
