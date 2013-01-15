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
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class MergeOrderRequest : DataContractBase
	{
		public MergeOrderRequest(List<EntityRef> sourceOrderRefs, EntityRef destinationOrderRef)
		{
			this.SourceOrderRefs = sourceOrderRefs;
			this.DestinationOrderRef = destinationOrderRef;
		}

		[DataMember]
		public List<EntityRef> SourceOrderRefs;

		[DataMember]
		public EntityRef DestinationOrderRef;

		[DataMember]
		public bool DryRun;

		/// <summary>
		/// Validation will always be performed for dry-run.  But if only validation is needed, set this flag to true.  The DryRun flag will then be ignored.
		/// </summary>
		[DataMember]
		public bool ValidationOnly;
	}
}
