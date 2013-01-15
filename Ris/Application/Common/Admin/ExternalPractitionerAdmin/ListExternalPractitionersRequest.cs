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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[Serializable]
	public enum VerifiedState
	{
		All,
		Verified,
		NotVerified
	}

	[DataContract]
	public class ListExternalPractitionersRequest : ListRequestBase
	{
		public ListExternalPractitionersRequest()
			: this(null, null, new SearchResultPage())
		{
		}

		public ListExternalPractitionersRequest(string surname, string givenname)
			: this(surname, givenname, new SearchResultPage())
		{
		}

		public ListExternalPractitionersRequest(string surname, string givenname, SearchResultPage page)
			: this(surname, givenname, VerifiedState.All, null, null, true, false, false, false, false, false, page)
		{
			this.LastName = surname;
			this.FirstName = givenname;
			this.Page = page;
		}

		public ListExternalPractitionersRequest(
			string surname,
			string givenname,
			VerifiedState verifiedState,
			DateTime? lastVerifiedRangeFrom,
			DateTime? lastVerifiedRangeUntil,
			bool queryItems,
			bool queryCount,
			bool sortByLastVerifiedTime,
			bool sortByLastEditedTime,
			bool sortAscending,
			bool includeMerged,
			SearchResultPage page)
		{
			this.LastName = surname;
			this.FirstName = givenname;
			this.VerifiedState = verifiedState;
			this.LastVerifiedRangeFrom = lastVerifiedRangeFrom;
			this.LastVerifiedRangeUntil = lastVerifiedRangeUntil;
			this.QueryItems = queryItems;
			this.QueryCount = queryCount;
			this.SortByLastVerifiedTime = sortByLastVerifiedTime;
			this.SortByLastEditedTime = sortByLastEditedTime;
			this.SortAscending = sortAscending;
			this.IncludeMerged = includeMerged;
			this.Page = page;
		}

		[DataMember]
		public string FirstName;

		[DataMember]
		public string LastName;

		[DataMember]
		public VerifiedState VerifiedState;

		[DataMember]
		public bool QueryCount;

		[DataMember]
		public bool QueryItems;

		[DataMember]
		public bool SortByLastVerifiedTime;

		[DataMember]
		public bool SortByLastEditedTime;

		[DataMember]
		public bool SortAscending;

		[DataMember]
		public bool IncludeMerged;

		[DataMember]
		public DateTime? LastVerifiedRangeFrom;

		[DataMember]
		public DateTime? LastVerifiedRangeUntil;
	}
}