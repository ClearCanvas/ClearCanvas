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

using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{

    [DataContract]
    public class ImportSopRequest
    {
        [DataMember(IsRequired = true)]
        public string CallingAETitle { get; set; }

        [DataMember(IsRequired = true)]
        public string CalledAETitle { get; set; }
        
        [DataMember(IsRequired = true)]
        public string SopInstanceUid { get; set; }

        [DataMember(IsRequired = true)]
        public byte[] SopInstance { get; set; }
    }

    [DataContract]
    public class ImportSopResponse
    {
        [DataMember(IsRequired = false)]
        public bool Successful;

        [DataMember(IsRequired = true)] 
        public ushort DicomStatusCode { get; set; }

        [DataMember(IsRequired = false)]
        public string FailureMessage { get; set; }
    }

    [ServiceContract]
    public interface IDicomImportService
    {
        /// <summary>
        /// ImportsRetrieves a stream containing the study header information.
        /// </summary>
        /// <param name="request">Query parameters</param>
        /// <returns>The stream containing the study header information in compressed XML format</returns>
        /// <seealso cref="HeaderStreamingParameters"></seealso>
        [OperationContract]
        [FaultContract(typeof(string))]
        ImportSopResponse ImportSop(ImportSopRequest request);
    }
}
