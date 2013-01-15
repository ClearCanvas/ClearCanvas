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

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicateContactPointRequest : DataContractBase
	{
		public MergeDuplicateContactPointRequest(
			EntityRef retained,
			EntityRef replaced)
		{
			this.RetainedContactPointRef = retained;
			this.ReplacedContactPointRef = replaced;
		}

		[DataMember]
		public EntityRef RetainedContactPointRef;

		[DataMember]
		public EntityRef ReplacedContactPointRef;

		/// <summary>
		/// If true, no merge will actually be performed.  Instead, the server will return some estimated
		/// measures of the cost of the merge operation if it were to be performed.
		/// </summary>
		[DataMember]
		public bool EstimateCostOnly;
	}
}
