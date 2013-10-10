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

using System.IO;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
    [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    public class StandardMimeType : IImageMimeTypeProcessor
    {
        public string OutputMimeType
        {
            get { return "application/dicom"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            MimeTypeProcessorOutput output = new MimeTypeProcessorOutput();
            output.ContentType = OutputMimeType;
            using (FileStream stream = FileStreamOpener.OpenForRead(context.ImagePath, FileMode.Open))
            {
                output.ContentType = OutputMimeType;
                byte[] buffer = new byte[stream.Length];
                int offset = 0;
                int readBytes = 0;
                do
                {
                    readBytes = stream.Read(buffer, offset, buffer.Length - offset);
                    if (readBytes > 0)
                    {
                        offset += readBytes;
                    }
                } while (readBytes > 0);
                output.Output = buffer;
                stream.Close();
            }
            return output; 
            
        }
    }
}