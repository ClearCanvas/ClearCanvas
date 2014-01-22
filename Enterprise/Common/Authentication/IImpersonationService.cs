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

namespace ClearCanvas.Enterprise.Common.Authentication
{
	/// <summary>
	/// Provides a means of initiating a session on behalf of another user.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(true)] //todo: this needs to be true! but first we need a way for shredhosts to authenticate
	public interface IImpersonationService
	{
		/// <summary>
		/// Initiates a new impersontated session for the specified user.
		/// </summary>
		/// <exception cref="UserAccessDeniedException">Invalid username or password.</exception>
		/// <remarks>
		/// Once an impersonated session has been obtained, the <see cref="IAuthenticationService"/>
		/// should be used to perform further operations on it, such as validation and termination.
		/// </remarks>
		[OperationContract]
		[FaultContract(typeof(UserAccessDeniedException))]
		InitiateSessionResponse InitiateSession(InitiateImpersonatedSessionRequest request);
	}
}