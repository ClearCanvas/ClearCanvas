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
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.Dicom;
using System.IO;
using System.Globalization;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
	[ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
	public class ImageHeaderMimeType : IImageMimeTypeProcessor
	{
		#region IImageMimeTypeProcessor Members

		public string OutputMimeType
		{
			get { return "application/clearcanvas-header"; }
		}

		public MimeTypeProcessorOutput Process(ImageStreamingContext context)
		{
			uint stopTag;
			if (!uint.TryParse(context.Request.QueryString["stopTag"] ?? "", NumberStyles.HexNumber, null, out stopTag))
				stopTag = DicomTags.PixelData;

			if (stopTag > DicomTags.PixelData)
				throw new WADOException(HttpStatusCode.BadRequest,
										"Stop tag must be less than PixelData tag.");

			MimeTypeProcessorOutput output = new MimeTypeProcessorOutput();
			output.ContentType = OutputMimeType;
			DicomFile file = new DicomFile(context.ImagePath);
			file.Load(stopTag, DicomReadOptions.Default);

			output.ContentType = OutputMimeType;

			MemoryStream memStream = new MemoryStream();
			file.Save(memStream, DicomWriteOptions.Default);
			output.Output = memStream.ToArray();

			return output;
		}

		#endregion
	}
}
