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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Jsml;

namespace ClearCanvas.Ris.Application.Services.Jsml
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IJsmlShimService))]
    public class JsmlShimService : ApplicationServiceBase, IJsmlShimService
    {
        #region IJsmlShimService Members

        public GetOperationNamesResponse GetOperationNames(GetOperationNamesRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ServiceContractName, "ServiceContractName");

            GetOperationNamesResponse response = new GetOperationNamesResponse();
        	response.OperationNames = ShimUtil.GetOperationNames(request.ServiceContractName);
            return response;
        }

        public InvokeOperationResponse InvokeOperation(InvokeOperationRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ServiceContractName, "ServiceContractName");
			Platform.CheckMemberIsSet(request.OperationName, "OperationName");
			Platform.CheckMemberIsSet(request.RequestJsml, "RequestJsml");

        	string responseJsml = ShimUtil.InvokeOperation(
				request.ServiceContractName, request.OperationName, request.RequestJsml.Value);

			InvokeOperationResponse response = new InvokeOperationResponse();
			response.ResponseJsml = new JsmlBlob(responseJsml);
			return response;
        }

        #endregion
    }
}
