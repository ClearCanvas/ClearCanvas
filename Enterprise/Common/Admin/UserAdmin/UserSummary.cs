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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
	[DataContract]
	public class UserSummary : DataContractBase
	{
		/// <summary>
		/// Constructor for returning only the most basic information about a user.
		/// </summary>
		public UserSummary(EnumValueInfo accountType, string userId, string displayName, string emailAddress)
		{
			AccountType = accountType;
			UserName = userId;
			DisplayName = displayName;
			EmailAddress = emailAddress;
		}

		/// <summary>
		/// Constructor for returning full user summary.
		/// </summary>
		public UserSummary(
			EnumValueInfo accountType,
			string userId,
			string displayName,
			string emailAddress,
			DateTime creationTime,
			DateTime? validFrom,
			DateTime? validUntil,
			DateTime? lastLoginTime,
			DateTime? passwordExpiry,
			bool enabled,
			int sessionCount)
		{
			AccountType = accountType;
			UserName = userId;
			DisplayName = displayName;
			EmailAddress = emailAddress;
			CreationTime = creationTime;
			ValidFrom = validFrom;
			ValidUntil = validUntil;
			LastLoginTime = lastLoginTime;
			Enabled = enabled;
			PasswordExpiry = passwordExpiry;
			SessionCount = sessionCount;
		}

		[DataMember]
		public EnumValueInfo AccountType;

		[DataMember]
		public string UserName;

		[DataMember]
		public string DisplayName;

		[DataMember]
		public string EmailAddress;

		[DataMember]
		public DateTime CreationTime;

		[DataMember]
		public DateTime? ValidFrom;

		[DataMember]
		public DateTime? ValidUntil;

		[DataMember]
		public DateTime? LastLoginTime;

		[DataMember]
		public bool Enabled;

		[DataMember]
		public DateTime? PasswordExpiry;

		[DataMember]
		public int SessionCount;

		protected bool Equals(UserSummary userSummary)
		{
			if (userSummary == null) return false;
			return Equals(UserName, userSummary.UserName);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as UserSummary);
		}

		public override int GetHashCode()
		{
			return UserName.GetHashCode();
		}
	}
}
