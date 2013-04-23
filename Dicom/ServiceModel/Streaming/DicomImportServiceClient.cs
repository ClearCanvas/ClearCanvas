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
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    public class DicomImportServiceClient : ClientBase<IDicomImportService>, IDicomImportService
    {
        /// <summary>
        /// Constructor - uses the default endpoint and binding configuration.
        /// </summary>
        public DicomImportServiceClient()
        {
        }

        /// <summary>
        /// Constructor - uses the default binding configuration and the given endpoint address.
        /// </summary>
        public DicomImportServiceClient(EndpointAddress remoteAddress)
        {
            Endpoint.Address = remoteAddress;
        }

        /// <summary>
        /// Constructor - uses the default binding configuration and the given endpoint uri.
        /// </summary>
        public DicomImportServiceClient(Uri uri)
        {
            Endpoint.Address = new EndpointAddress(uri);
        }

        /// <summary>
        /// Constructor - uses input configuration name to configure endpoint and bindings.
        /// </summary>
        public DicomImportServiceClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Constructor - uses input endpoint and binding.
        /// </summary>
        public DicomImportServiceClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Constructor - uses input endpoint, loads bindings from given configuration name.
        /// </summary>
        public DicomImportServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        #region IDicomImportService Members

        /// <summary>
        /// Gets the study header (xml) as a gzipped stream.
        /// </summary>
        /// <param name="request">AE title of the local application.</param>
        /// <returns></returns>
        public ImportSopResponse ImportSop(ImportSopRequest request)
        {
            return base.Channel.ImportSop(request);
        }

        #endregion

        /// <summary>
        /// Imports in memory <see cref="DicomFile"/> to a remote server.
        /// </summary>
        /// <param name="callingAETitle">AE title of the local application.</param>
        /// <param name="calledAeTitle">AE title of the remote server</param>
        /// <param name="file">The DICOM file to import</param>
        /// <returns></returns>
        public ImportSopResponse ImportFile(string callingAETitle, string calledAeTitle, DicomFile file)
        {
            var request = new ImportSopRequest
                {
                    CallingAETitle = callingAETitle,
                    CalledAETitle = calledAeTitle,
                    SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].ToString(),
                };

            using (var stream = new LargeMemoryStream())
            {
                file.Save(stream, DicomWriteOptions.Default);
                request.SopInstance = stream.ToArray();
            }

            var response = ImportSop(request);

            return response;
        }
    }
}
