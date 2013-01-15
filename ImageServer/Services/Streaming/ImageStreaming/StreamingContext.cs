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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    public class StreamingContext
    {
        private HttpListenerRequest _request;
        private HttpListenerResponse _response;
        private string _studyInstanceUid;
        private string _seriesInstanceUid;
        private string _objectUid;
        private string _contentType;
        private StudyStorageLocation _studylocation;

        private string[] _acceptTypes;

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string ObjectUid
        {
            get { return _objectUid; }
            set { _objectUid = value; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _studylocation; }
            set { _studylocation = value; }
        }

        public HttpListenerRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set { _contentType = value; }
        }

        public string[] AcceptTypes
        {
            get { return _acceptTypes; }
            set { _acceptTypes = value; }
        }

        public HttpListenerResponse Response
        {
            get { return _response; }
            set { _response = value; }
        }
    }
}