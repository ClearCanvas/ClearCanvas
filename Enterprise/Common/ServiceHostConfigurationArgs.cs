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
using ClearCanvas.Enterprise.Common.ServiceConfiguration.Server;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Arguments for configuration of a service host.
	/// </summary>
	public struct ServiceHostConfigurationArgs
	{
		public ServiceHostConfigurationArgs(Type serviceContract, Uri hostUri, bool authenticated,
			long maxReceivedMessageSize, CertificateSearchDirective certificateSearchParams)
		{
			ServiceContract = serviceContract;
			HostUri = hostUri;
			Authenticated = authenticated;
			MaxReceivedMessageSize = maxReceivedMessageSize;
			CertificateSearchDirective = certificateSearchParams;
			SendTimeoutSeconds = 0; //Treated as default (e.g. don't change)
			TransferMode = TransferMode.Buffered;
		}

		/// <summary>
		/// The parameters used for finding the certificate
		/// </summary>
		public CertificateSearchDirective CertificateSearchDirective;

		/// <summary>
		/// The service contract for which the host is created.
		/// </summary>
		public Type ServiceContract;

		/// <summary>
		/// The URI on which the service is being exposed.
		/// </summary>
		public Uri HostUri;

		/// <summary>
		/// A value indicating whether the service is authenticated, or allows anonymous access.
		/// </summary>
		public bool Authenticated;

		/// <summary>
		/// The maximum allowable size of received messages, in bytes.
		/// </summary>
		public long MaxReceivedMessageSize;

		/// <summary>
		/// The time, in seconds, in which a send operation must complete.
		/// </summary>
		/// <remarks>Value less than or equal to zero should be ignored.</remarks>
		public int SendTimeoutSeconds;

		/// <summary>
		/// The TransferMode used for the service.
		/// </summary>
		public TransferMode TransferMode;
	}
}
