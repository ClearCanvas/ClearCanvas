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
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Implementation of <see cref="IUserCredentialsProvider"/> that obtains credentials from
    /// the <see cref="Thread.CurrentPrincipal"/>, assuming that the current principal
    /// implements the <see cref="IUserCredentialsProvider"/> interface.
    /// </summary>
	public class DefaultUserCredentialsProvider : IUserCredentialsProvider
	{
		#region IUserCredentialsProvider Members

		public string UserName
		{
			get { return GetThreadCredentials().UserName; }
		}

		public string SessionTokenId
		{
			get { return GetThreadCredentials().SessionTokenId; }
		}

		#endregion

		private static IUserCredentialsProvider GetThreadCredentials()
		{
			var provider = Thread.CurrentPrincipal as IUserCredentialsProvider;
			if(provider == null)
                throw new InvalidOperationException("Thread.CurrentPrincipal value does not implement IUserCredentialsProvider.");

			return provider;
		}
	}
}
