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

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = false)]
	public partial class RemoteCoreServiceProvider : RemoteServiceProviderBase<EnterpriseCoreServiceAttribute>
	{
		public RemoteCoreServiceProvider() 
			: base(GetSettings())
		{
		}

		private static RemoteServiceProviderArgs GetSettings()
		{
			return new RemoteServiceProviderArgs(
				RemoteCoreServiceSettings.Default.BaseUrl,
				RemoteCoreServiceSettings.Default.FailoverBaseUrl,
				RemoteCoreServiceSettings.Default.ConfigurationClass,
				RemoteCoreServiceSettings.Default.MaxReceivedMessageSize,
				RemoteCoreServiceSettings.Default.CertificateValidationMode,
				RemoteCoreServiceSettings.Default.RevocationMode,
				RemoteCoreServiceSettings.Default.UserCredentialsProviderClass
				)
					{
						FailedEndpointBlackoutTime = RemoteCoreServiceSettings.Default.FailedEndpointBlackoutTime
					};
		}
	}
}
