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

using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Automation.Tests
{
	[TestFixture]
	internal sealed class Version30DataContractTests : AbstractDataContractTest
	{
		[Test]
		public void TestOpenStudiesRequestContract()
		{
			var referenceObject = new LegacyOpenStudiesRequest
			                      	{
			                      		ActivateIfAlreadyOpen = false,
			                      		ReportFaultToUser = true,
			                      		StudiesToOpen = new List<BaselineDataContracts.OpenStudyInfo>
			                      		                	{
			                      		                		new BaselineDataContracts.OpenStudyInfo {StudyInstanceUid = "1.234"},
			                      		                		new BaselineDataContracts.OpenStudyInfo {StudyInstanceUid = "56.78"},
			                      		                		new BaselineDataContracts.OpenStudyInfo {StudyInstanceUid = "9.0"}
			                      		                	}
			                      	};

			var currentObject = TestRequestContractCompatibility<OpenStudiesRequest>(referenceObject);
			Assert.AreEqual(referenceObject.ActivateIfAlreadyOpen, currentObject.ActivateIfAlreadyOpen);
			Assert.AreEqual(referenceObject.ReportFaultToUser, currentObject.ReportFaultToUser);
			AssertAreSequenceEqual(referenceObject.StudiesToOpen, currentObject.StudiesToOpen, (a, b) => a.StudyInstanceUid == b.StudyInstanceUid);
		}

		/// <summary>
		/// The OpenStudiesRequest contract as it existed around version 3.0
		/// </summary>
		[DataContract(Name = "OpenStudiesRequest", Namespace = "http://www.clearcanvas.ca/imageViewer/automation")]
		public class LegacyOpenStudiesRequest
		{
			[DataMember(IsRequired = true)]
			public List<BaselineDataContracts.OpenStudyInfo> StudiesToOpen { get; set; }

			[DataMember(IsRequired = false)]
			public bool? ActivateIfAlreadyOpen { get; set; }

			[DataMember(IsRequired = false)]
			public bool ReportFaultToUser { get; set; }
		}
	}
}

#endif