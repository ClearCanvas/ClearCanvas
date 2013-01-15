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

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Supplies credentials to an <see cref="IChannelProvider"/>.
	/// </summary>
	public class ChannelCredentials
	{
		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		public string Password { get; set; }
	}


    /// <summary>
    /// Defines an interface to an object that provides channel factories for
    /// remote services.
    /// </summary>
    /// <remarks>
    /// This interface is consumed by <see cref="RemoteServiceProviderBase"/>, 
    /// which must obtain channel factories for the services it provides.
    /// Implementations must be thread-safe.
    /// </remarks>
    public interface IChannelProvider
    {
        /// <summary>
        /// Gets a primary channel instance for the specified service contract.
        /// </summary>
        /// <remarks>
        /// The service provider will call <see cref="GetPrimary"/> every time an instance
        /// of a given service is requested.  The implementation is free to return a 
        /// different channel as the "primary" channel for any given call, in order to 
        /// achieve a load-balancing scheme if desired.  Implementations must be thread-safe,
        /// and should never return the same channel for use by more than one thread.
        /// </remarks>
        /// <param name="serviceContract"></param>
        /// <param name="credentials">May be null if authentication is not required.</param>
        /// <returns>A channel instance.</returns>
        IClientChannel GetPrimary(Type serviceContract, ChannelCredentials credentials);

        /// <summary>
        /// Attempts to obtain an alternate channel instance for the specified failed channel,
        /// in the event that its endpoint is unreachable.
        /// </summary>
        /// <remarks>
        /// This method should only be called if the primary channel is unreachable.
        /// It may be called multiple times, in the event that a returned failover
        /// channel is also unreachable.  With each successive call, the caller must
        /// provide the failed channel, so that the implementation
        /// can track which channels have failed and avoid returning the same channel
		/// repeatedly.  Implementations must be thread-safe,
		/// and should never return the same channel for use by more than one thread.
        /// </remarks>
		/// <param name="failedChannel"></param>
        /// <returns>An alternate channel, or null if no alternate is available.</returns>
		IClientChannel GetFailover(IClientChannel failedChannel);
    }
}
