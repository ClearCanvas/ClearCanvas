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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class ChangePasswordRequest : DataContractBase
	{
		public ChangePasswordRequest(string user, string password, string newPassword)
		{
			this.UserName = user;
			this.CurrentPassword = password;
			this.NewPassword = newPassword;
		}

		/// <summary>
		/// User account.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Current password.
		/// </summary>
		[DataMember]
		public string CurrentPassword;

		/// <summary>
		/// New password.
		/// </summary>
		[DataMember]
		public string NewPassword;

	}
}
