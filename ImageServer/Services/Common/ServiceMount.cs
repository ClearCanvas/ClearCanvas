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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Services.Common
{
    /// <summary>
    /// Creates or stop web services.
    /// </summary>
    public class ServiceMount : ClearCanvas.Enterprise.Core.ServiceModel.ServiceMount
    {
        public ServiceMount(Uri baseAddress, IServiceHostConfiguration configuration) 
            : base(baseAddress, configuration)
        {
        }

        public ServiceMount(Uri baseAddress, string serviceHostConfigurationClass) 
            : base(baseAddress, serviceHostConfigurationClass)
        {
        }

        protected override void ApplyInterceptors(IList<Castle.Core.Interceptor.IInterceptor> interceptors)
        {
            // NO-OP
        }
    }
}