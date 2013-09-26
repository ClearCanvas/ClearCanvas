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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
	[DataContract]
	public class ListUsersRequest : DataContractBase
	{
		public ListUsersRequest()
		{
			this.Page = new SearchResultPage();
		}

		/// <summary>
		/// Filter by user name.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Filter by display name.
		/// </summary>
		[DataMember]
		public string DisplayName;

		/// <summary>
		/// Specify true to apply filters as exact match only.
		/// </summary>
		[DataMember]
		public bool ExactMatchOnly;

		/// <summary>
		/// Specify true to include Group accounts in the results.
		/// </summary>
		[DataMember]
		public bool IncludeGroupAccounts;

		/// <summary>
		/// Specify true to include System accounts in the results.
		/// </summary>
		[DataMember]
		public bool IncludeSystemAccounts;

		[DataMember]
		public SearchResultPage Page;
	}
}
