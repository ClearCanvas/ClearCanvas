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
using System.IO.Compression;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    public class HeaderStreamingServiceClient : ClientBase<IHeaderStreamingService>, IHeaderStreamingService
    {
        /// <summary>
        /// Constructor - uses the default endpoint and binding configuration.
        /// </summary>
        public HeaderStreamingServiceClient()
		{
		}

        /// <summary>
        /// Constructor - uses the default binding configuration and the given endpoint address.
        /// </summary>
        public HeaderStreamingServiceClient(EndpointAddress remoteAddress)
        {
            Endpoint.Address = remoteAddress;
        }

        /// <summary>
        /// Constructor - uses the default binding configuration and the given endpoint uri.
        /// </summary>
        public HeaderStreamingServiceClient(Uri uri)
        {
            Endpoint.Address = new EndpointAddress(uri);
        }

		/// <summary>
		/// Constructor - uses input configuration name to configure endpoint and bindings.
		/// </summary>
		public HeaderStreamingServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint and binding.
		/// </summary>
		public HeaderStreamingServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint, loads bindings from given configuration name.
		/// </summary>
        public HeaderStreamingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
		}
        
        #region IHeaderStreamingService Members

        /// <summary>
        /// Gets the study header (xml) as a gzipped stream.
        /// </summary>
        /// <param name="callingAETitle">AE title of the local application.</param>
        /// <param name="parameters">Input parameters.</param>
        /// <returns></returns>
        public System.IO.Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters)
        {
            return base.Channel.GetStudyHeader(callingAETitle, parameters);
        }

        #endregion

        /// <summary>
        /// Gets the study header (via <see cref="GetStudyHeader"/>), unzips the gzipped stream, and returns it as <see cref="StudyXml"/>.
        /// </summary>
        /// <param name="callingAETitle">AE title of the local application.</param>
        /// <param name="parameters">Input parameters.</param>
        /// <returns></returns>
        public StudyXml GetStudyXml(string callingAETitle, HeaderStreamingParameters parameters)
        {
            StudyXmlMemento theMemento;
            using (var stream = GetStudyHeader(callingAETitle, parameters))
            {
                using (var gzStream = new GZipStream(stream, CompressionMode.Decompress))
                {
					theMemento = new StudyXmlMemento();
					StudyXmlIo.Read(theMemento, gzStream);
                    gzStream.Close();
                }
            }

            var studyXml = new StudyXml();
            studyXml.SetMemento(theMemento);
            return studyXml;
        }
    }
}
