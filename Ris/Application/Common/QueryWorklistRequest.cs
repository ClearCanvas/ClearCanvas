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
using ClearCanvas.Common.Caching;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class QueryWorklistRequest : PagedDataContractBase, IDefinesCacheKey
	{
		public QueryWorklistRequest(string worklistType, bool queryItems, bool queryCount, bool downtimeRecoveryMode, EntityRef workingFacilityRef)
		{
			this.WorklistClass = worklistType;
			this.QueryItems = queryItems;
			this.QueryCount = queryCount;
			this.DowntimeRecoveryMode = downtimeRecoveryMode;
			this.WorkingFacilityRef = workingFacilityRef;
		}

		public QueryWorklistRequest(EntityRef worklistRef, bool queryItems, bool queryCount, bool downtimeRecoveryMode, EntityRef workingFacilityRef)
		{
			this.WorklistRef = worklistRef;
			this.QueryItems = queryItems;
			this.QueryCount = queryCount;
			this.DowntimeRecoveryMode = downtimeRecoveryMode;
			this.WorkingFacilityRef = workingFacilityRef;
		}

		/// <summary>
		/// Specifies the worklist instance to query, for instanced worklists.
		/// </summary>
		[DataMember]
		public EntityRef WorklistRef;

		/// <summary>
		/// Specifies the class of worklist to query, for singleton worklists.
		/// </summary>
		[DataMember]
		public string WorklistClass;

		/// <summary>
		/// Specifies whether to return a count of the total number of items in the notebox.
		/// </summary>
		[DataMember]
		public bool QueryCount;

		/// <summary>
		/// Specifies whether to return the list of items in the notebox.
		/// </summary>
		[DataMember]
		public bool QueryItems;

		/// <summary>
		/// Specifies whether to return the list of items in the notebox.
		/// </summary>
		[DataMember]
		public bool DowntimeRecoveryMode;

		/// <summary>
		/// Specifies the working facility.
		/// </summary>
		[DataMember]
		public EntityRef WorkingFacilityRef;

		string IDefinesCacheKey.GetCacheKey()
		{
			// important to include working facility in the cache key, because this affects the response
			var r = this.WorklistRef == null ? this.WorklistClass : this.WorklistRef.ToString(true, false);
			var f = this.WorkingFacilityRef == null ? "" : this.WorkingFacilityRef.ToString(false, false);
			return string.Format("{0}:{1}:{2}:{3}:{4}", r, f, QueryCount, QueryItems, DowntimeRecoveryMode);
		}
	}
}
