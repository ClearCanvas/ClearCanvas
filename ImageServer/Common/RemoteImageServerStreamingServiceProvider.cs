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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    ///  Marks an interface as Image Server service interface (as opposed to an Enterpise service).
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ImageServerStreamingServiceAttribute : Attribute { }

    /// <summary>
    /// Provide access to the Image Server services hosted on another machine.
    /// </summary>
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
	public class RemoteImageServerStreamingServiceProvider : RemoteServiceProviderBase<ImageServerStreamingServiceAttribute>
    {
		public RemoteImageServerStreamingServiceProvider()
			: base(GetSettings())
		{
		}

		private static RemoteServiceProviderArgs GetSettings()
		{
			return new RemoteServiceProviderArgs(
                RemoteImageServerServiceSettings.Default.BaseUrl,
                RemoteImageServerServiceSettings.Default.FailoverBaseUrl,
                RemoteImageServerServiceSettings.Default.StreamingConfigurationClass,
                RemoteImageServerServiceSettings.Default.StreamingMaxReceivedMessageSize,
                RemoteImageServerServiceSettings.Default.CertificateValidationMode,
                RemoteImageServerServiceSettings.Default.RevocationMode,
                RemoteImageServerServiceSettings.Default.UserCredentialsProviderClass
				)
			       	{
			       		SendTimeoutSeconds = RemoteImageServerServiceSettings.Default.SendTimeoutSeconds
			       	};
		}
    }
}
