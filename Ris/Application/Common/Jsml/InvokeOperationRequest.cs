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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;
using System.IO;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    [DataContract]
    public class InvokeOperationRequest : DataContractBase
    {
        public InvokeOperationRequest(string serviceContractName, string operationName, JsmlBlob requestJsml)
        {
            this.ServiceContractName = serviceContractName;
            this.OperationName = operationName;
            this.RequestJsml = requestJsml;
        }

        /// <summary>
        /// The assembly-qualified name of the service contract.
        /// </summary>
        [DataMember]
        public string ServiceContractName;

        /// <summary>
        /// The service operation to invoke.
        /// </summary>
        [DataMember]
        public string OperationName;

        /// <summary>
        /// The request argument to be passed to the service operation.
        /// </summary>
        [DataMember]
        public JsmlBlob RequestJsml;
    }
}
