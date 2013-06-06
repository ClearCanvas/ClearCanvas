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
using System.Net;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class MimeTypeProcessorError:Exception
    {
        public HttpStatusCode HttpError { get; private set; }

        public MimeTypeProcessorError(HttpStatusCode errorCode, string message):base(message)
        {
            HttpError = errorCode;
        }
    }

    public class MimeTypeProcessorOutput
    {
        private string _contentType;
        private byte[] _output;
        private bool _isCompressed;
        private bool _isLast;

        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public byte[] Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public bool IsLast
        {
            get { return _isLast; }
            set { _isLast = value; }
        }

        public bool IsCompressed
        {
            get { return _isCompressed; }
            set { _isCompressed = value; }
        }
    }
}