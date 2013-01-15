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

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Client service for <see cref="IUsageTracking"/>.
    /// </summary>
    internal class UsageTrackingServiceClient : ClientBase<IUsageTracking>
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public UsageTrackingServiceClient()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpointConfigurationName">Endpoint configuration name.</param>
        public UsageTrackingServiceClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binding">Binding for the service.</param>
        /// <param name="remoteAddress">Remote address.</param>
        public UsageTrackingServiceClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpointConfigurationName">Binding configuration name.</param>
        /// <param name="remoteAddress">Remote address.</param>
        public UsageTrackingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        #endregion Constructor

        #region IUsageTracking Members

        /// <summary>
        /// Register the application.
        /// </summary>
        /// <param name="request">The register request.</param>
        public RegisterResponse Register(RegisterRequest request)
        {
            return Channel.Register(request);
        }

        #endregion
    }
}
