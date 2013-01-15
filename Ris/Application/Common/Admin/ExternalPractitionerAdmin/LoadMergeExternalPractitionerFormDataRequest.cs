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
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadMergeExternalPractitionerFormDataRequest : DataContractBase
	{
		public LoadMergeExternalPractitionerFormDataRequest()
		{
			this.DeactivatedContactPointRefs = new List<EntityRef>();
		}

		/// <summary>
		/// Specifies the reference of an external practitioner.
		/// Request to return a list of duplicate external practitioners.
		/// </summary>
		[DataMember]
		public EntityRef PractitionerRef;

		/// <summary>
		/// A list of contact point references that will become deactivated.
		/// Request to return a list of orders affected by these contact points.
		/// </summary>
		[DataMember]
		public List<EntityRef> DeactivatedContactPointRefs;
	}
}
