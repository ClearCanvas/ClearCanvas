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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.Automation.Tests
{
	/// <summary>
	/// Legacy <see cref="Viewer"/> data contract as it was defined prior to version 9.0
	/// </summary>
	[DataContract(Name = "Viewer", Namespace = "http://www.clearcanvas.ca/imageViewer/automation")]
	public class LegacyViewerPre90
	{
		[DataMember(IsRequired = true)]
		public Guid Identifier { get; set; }

		[DataMember(IsRequired = true)]
		public string PrimaryStudyInstanceUid { get; set; }
	}

	/// <summary>
	/// Legacy <see cref="OpenStudiesRequest"/> data contract as it was defined around version 3.0
	/// </summary>
	[DataContract(Name = "OpenStudiesRequest", Namespace = "http://www.clearcanvas.ca/imageViewer/automation")]
	public class LegacyOpenStudiesRequestCirca30
	{
		[DataMember(IsRequired = true)]
		public List<OpenStudyInfo> StudiesToOpen { get; set; }

		[DataMember(IsRequired = false)]
		public bool? ActivateIfAlreadyOpen { get; set; }

		[DataMember(IsRequired = false)]
		public bool ReportFaultToUser { get; set; }
	}

	/// <summary>
	/// Legacy <see cref="OpenStudiesRequest"/> data contract as it was defined around version 2.0
	/// </summary>
	[DataContract(Name = "OpenStudiesRequest", Namespace = "http://www.clearcanvas.ca/imageViewer/automation")]
	public class LegacyOpenStudiesRequestCirca20
	{
		[DataMember(IsRequired = true)]
		public List<OpenStudyInfo> StudiesToOpen { get; set; }

		[DataMember(IsRequired = false)]
		public bool? ActivateIfAlreadyOpen { get; set; }
	}
}

#endif