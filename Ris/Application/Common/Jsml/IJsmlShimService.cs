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
using System.ServiceModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    /// <summary>
    /// This is a pass-through service that provides a mechanism for invoking an operation
    /// on another service using JSML-encoded request/response objects rather than native
    /// .NET request/response objects.
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IJsmlShimService
    {
        /// <summary>
        /// Returns the names of the service operations provided by the specified service.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetOperationNamesResponse GetOperationNames(GetOperationNamesRequest request);

        /// <summary>
        /// Invokes the specified operation on the specified service.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		InvokeOperationResponse InvokeOperation(InvokeOperationRequest request); 
    }
}
