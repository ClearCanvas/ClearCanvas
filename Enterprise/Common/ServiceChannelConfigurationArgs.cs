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
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Arguments for configuration of a service channel.
	/// </summary>
    public struct ServiceChannelConfigurationArgs
    {
        public ServiceChannelConfigurationArgs(
            Type channelFactoryClass,
            Uri serviceUri,
            bool authenticationRequired,
            long maxReceivedMessageSize,
            X509CertificateValidationMode certificateValidationMode,
            X509RevocationMode revocationMode)
        {
            this.ChannelFactoryClass = channelFactoryClass;
            this.ServiceUri = serviceUri;
            this.AuthenticationRequired = authenticationRequired;
            this.MaxReceivedMessageSize = maxReceivedMessageSize;
            this.CertificateValidationMode = certificateValidationMode;
            this.RevocationMode = revocationMode;
        	this.SendTimeoutSeconds = 0;
			this.TransferMode = TransferMode.Buffered;
        }

		/// <summary>
		/// The class of the channel factory to create.
		/// </summary>
        public Type ChannelFactoryClass;

		/// <summary>
		/// The URI on which the service is hosted.
		/// </summary>
        public Uri ServiceUri;

		/// <summary>
		/// A value indicating whether the service requires authentication.
		/// </summary>
        public bool AuthenticationRequired;

		/// <summary>
		/// The maximum size of received messages to allow, in bytes.
		/// </summary>
        public long MaxReceivedMessageSize;

		/// <summary>
		/// The time, in seconds, in which a send operation must complete.
		/// </summary>
		/// <remarks>Value less than or equal to zero should be ignored.</remarks>
		public int SendTimeoutSeconds;

        /// <summary>
        /// Specifies the mode used for X509 certificate validation.
        /// </summary>
        public X509CertificateValidationMode CertificateValidationMode;

        /// <summary>
        /// Specifies the mode used to check for X509 certificate revocation.
        /// </summary>
        public X509RevocationMode RevocationMode;

		/// <summary>
		/// The TransferMode used for the service.
		/// </summary>
		public TransferMode TransferMode;
    }
}
