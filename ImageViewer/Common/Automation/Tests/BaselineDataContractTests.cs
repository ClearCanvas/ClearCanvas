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
using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Automation.Tests
{
	[TestFixture]
	internal sealed class BaselineDataContractTests : AbstractDataContractTest
	{
		[Test]
		[Obsolete("This tests an obsolete usage")]
		public void TestNoActiveViewersFault()
		{
			TestResponseContractCompatibility<BaselineDataContracts.NoActiveViewersFault>(new NoActiveViewersFault());
		}

		[Test]
		public void TestNoViewersFault()
		{
			TestResponseContractCompatibility<BaselineDataContracts.NoViewersFault>(new NoViewersFault());
		}

		[Test]
		public void TestViewerNotFoundFault()
		{
			var referenceObject = new ViewerNotFoundFault
			                      	{
			                      		FailureDescription = "hello"
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.ViewerNotFoundFault>(referenceObject);
			Assert.AreEqual(referenceObject.FailureDescription, currentObject.FailureDescription);
		}

		[Test]
		public void TestOpenStudiesFault()
		{
			var referenceObject = new OpenStudiesFault
			                      	{
			                      		FailureDescription = "hello"
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.OpenStudiesFault>(referenceObject);
			Assert.AreEqual(referenceObject.FailureDescription, currentObject.FailureDescription);
		}

		[Test]
		public void TestOpenFilesFault()
		{
			var referenceObject = new OpenFilesFault
			                      	{
			                      		FailureDescription = "hello"
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.OpenFilesFault>(referenceObject);
			Assert.AreEqual(referenceObject.FailureDescription, currentObject.FailureDescription);
		}

		[Test]
		public void TestViewer()
		{
			var referenceObject = new Viewer
			                      	{
			                      		Identifier = new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}"),
			                      		PrimaryStudyIdentifier = new StudyRootStudyIdentifier
			                      		                         	{
			                      		                         		AccessionNumber = "ACC",
			                      		                         		StudyInstanceUid = "123.456"
			                      		                         	}
			                      	};

			var legacyObject = TestResponseContractCompatibility<BaselineDataContracts.Viewer>(referenceObject);
			Assert.AreEqual(referenceObject.Identifier, legacyObject.Identifier);
			Assert.AreEqual(referenceObject.PrimaryStudyInstanceUid, legacyObject.PrimaryStudyInstanceUid);

			var currentObject = TestRequestContractCompatibility<Viewer>(legacyObject);
			Assert.AreEqual(referenceObject.Identifier, currentObject.Identifier);
			Assert.AreEqual(referenceObject.PrimaryStudyInstanceUid, currentObject.PrimaryStudyInstanceUid);
			Assert.IsNotNull(currentObject.PrimaryStudyIdentifier);
			Assert.AreEqual(referenceObject.PrimaryStudyIdentifier.StudyInstanceUid, currentObject.PrimaryStudyIdentifier.StudyInstanceUid);

			// test for how the legacy UID field interacts with the identifier field in code
			{
				const string accessionNumber = "A789";
				const string sid1 = "123.456.789";
				const string sid2 = "78.90";
				const string sid3 = "54.21.45";

				var viewer = new Viewer {PrimaryStudyIdentifier = new StudyRootStudyIdentifier {AccessionNumber = accessionNumber, StudyInstanceUid = sid1}};
				Assert.AreEqual(accessionNumber, viewer.PrimaryStudyIdentifier.AccessionNumber);
				Assert.AreEqual(sid1, viewer.PrimaryStudyIdentifier.StudyInstanceUid);
				Assert.AreEqual(sid1, viewer.PrimaryStudyInstanceUid);

				viewer.PrimaryStudyInstanceUid = sid1;
				Assert.AreEqual(accessionNumber, viewer.PrimaryStudyIdentifier.AccessionNumber);
				Assert.AreEqual(sid1, viewer.PrimaryStudyIdentifier.StudyInstanceUid);
				Assert.AreEqual(sid1, viewer.PrimaryStudyInstanceUid);

				viewer.PrimaryStudyInstanceUid = sid2;
				Assert.AreEqual(null, viewer.PrimaryStudyIdentifier.AccessionNumber);
				Assert.AreEqual(sid2, viewer.PrimaryStudyIdentifier.StudyInstanceUid);
				Assert.AreEqual(sid2, viewer.PrimaryStudyInstanceUid);

				viewer.PrimaryStudyIdentifier = null;
				Assert.IsNull(viewer.PrimaryStudyIdentifier);
				Assert.IsNull(viewer.PrimaryStudyInstanceUid);

				viewer.PrimaryStudyInstanceUid = sid3;
				Assert.IsNotNull(viewer.PrimaryStudyIdentifier);
				Assert.AreEqual(null, viewer.PrimaryStudyIdentifier.AccessionNumber);
				Assert.AreEqual(sid3, viewer.PrimaryStudyIdentifier.StudyInstanceUid);
				Assert.AreEqual(sid3, viewer.PrimaryStudyInstanceUid);
			}
		}

		[Test]
		[Obsolete("This tests an obsolete usage")]
		public void TestGetActiveViewersResult()
		{
			var referenceObject = new GetActiveViewersResult
			                      	{
			                      		ActiveViewers = new List<Viewer>
			                      		                	{
			                      		                		new Viewer(new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}"))
			                      		                	}
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.GetActiveViewersResult>(referenceObject);
			AssertAreSequenceEqual(referenceObject.ActiveViewers, currentObject.ActiveViewers, (a, b) => a.Identifier == b.Identifier);
		}

		[Test]
		public void TestGetViewersRequest()
		{
			TestRequestContractCompatibility<GetViewersRequest>(new BaselineDataContracts.GetViewersRequest());
		}

		[Test]
		public void TestGetViewersResult()
		{
			var referenceObject = new GetViewersResult
			                      	{
			                      		Viewers = new List<Viewer>
			                      		          	{
			                      		          		new Viewer(new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}"))
			                      		          	}
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.GetViewersResult>(referenceObject);
			AssertAreSequenceEqual(referenceObject.Viewers, currentObject.Viewers, (a, b) => a.Identifier == b.Identifier);
		}

		[Test]
		public void TestGetViewerInfoRequest()
		{
			var referenceObject = new BaselineDataContracts.GetViewerInfoRequest
			                      	{
			                      		Viewer = new BaselineDataContracts.Viewer {Identifier = new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}")}
			                      	};

			var currentObject = TestRequestContractCompatibility<GetViewerInfoRequest>(referenceObject);
			Assert.AreEqual(referenceObject.Viewer.Identifier, currentObject.Viewer.Identifier);
		}

		[Test]
		public void TestGetViewerInfoResult()
		{
			var referenceObject = new GetViewerInfoResult
			                      	{
			                      		AdditionalStudyInstanceUids = new List<string>
			                      		                              	{
			                      		                              		"123",
			                      		                              		"345"
			                      		                              	}
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.GetViewerInfoResult>(referenceObject);
			Assert.AreEqual(referenceObject.AdditionalStudyInstanceUids, currentObject.AdditionalStudyInstanceUids);
		}

		[Test]
		public void TestOpenStudiesResult()
		{
			var referenceObject = new OpenStudiesResult
			                      	{
			                      		Viewer = new Viewer(new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}"))
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.OpenStudiesResult>(referenceObject);
			Assert.AreEqual(referenceObject.Viewer.Identifier, currentObject.Viewer.Identifier);
		}

		[Test]
		public void TestOpenStudyInfo()
		{
			var referenceObject = new BaselineDataContracts.OpenStudyInfo
			                      	{
			                      		StudyInstanceUid = "13",
			                      		SourceAETitle = "sdfdfsdf"
			                      	};

			var currentObject = TestRequestContractCompatibility<OpenStudyInfo>(referenceObject);
			Assert.AreEqual(referenceObject.StudyInstanceUid, currentObject.StudyInstanceUid);
			Assert.AreEqual(referenceObject.SourceAETitle, currentObject.SourceAETitle);
		}

		[Test]
		public void TestOpenFilesRequest()
		{
			var referenceObject = new BaselineDataContracts.OpenFilesRequest
			                      	{
			                      		Files = new List<string>
			                      		        	{
			                      		        		"123",
			                      		        		"553"
			                      		        	},
			                      		WaitForFilesToOpen = true,
			                      		ReportFaultToUser = true
			                      	};

			var currentObject = TestRequestContractCompatibility<OpenFilesRequest>(referenceObject);
			Assert.AreEqual(referenceObject.Files, currentObject.Files);
			Assert.AreEqual(referenceObject.WaitForFilesToOpen, currentObject.WaitForFilesToOpen);
			Assert.AreEqual(referenceObject.ReportFaultToUser, currentObject.ReportFaultToUser);
		}

		[Test]
		public void TestOpenFilesResult()
		{
			var referenceObject = new OpenFilesResult
			                      	{
			                      		Viewer = new Viewer(new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}"))
			                      	};

			var currentObject = TestResponseContractCompatibility<BaselineDataContracts.OpenFilesResult>(referenceObject);
			Assert.AreEqual(referenceObject.Viewer.Identifier, currentObject.Viewer.Identifier);
		}

		[Test]
		public void TestOpenStudiesRequest()
		{
			var referenceObject = new BaselineDataContracts.OpenStudiesRequest
			                      	{
			                      		ActivateIfAlreadyOpen = false,
			                      		StudiesToOpen = new List<BaselineDataContracts.OpenStudyInfo>
			                      		                	{
			                      		                		new BaselineDataContracts.OpenStudyInfo {StudyInstanceUid = "1.234"},
			                      		                		new BaselineDataContracts.OpenStudyInfo {StudyInstanceUid = "56.78"},
			                      		                		new BaselineDataContracts.OpenStudyInfo {StudyInstanceUid = "9.0"}
			                      		                	}
			                      	};

			var currentObject = TestRequestContractCompatibility<OpenStudiesRequest>(referenceObject);
			Assert.AreEqual(referenceObject.ActivateIfAlreadyOpen, currentObject.ActivateIfAlreadyOpen);
			AssertAreSequenceEqual(referenceObject.StudiesToOpen, currentObject.StudiesToOpen, (a, b) => a.StudyInstanceUid == b.StudyInstanceUid);
		}

		[Test]
		public void TestCloseViewerRequest()
		{
			var referenceObject = new BaselineDataContracts.CloseViewerRequest
			                      	{
			                      		Viewer = new BaselineDataContracts.Viewer {Identifier = new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}")}
			                      	};

			var currentObject = TestRequestContractCompatibility<CloseViewerRequest>(referenceObject);
			Assert.AreEqual(referenceObject.Viewer.Identifier, currentObject.Viewer.Identifier);
		}

		[Test]
		public void TestActivateViewerRequest()
		{
			var referenceObject = new BaselineDataContracts.ActivateViewerRequest
			                      	{
			                      		Viewer = new BaselineDataContracts.Viewer {Identifier = new Guid("{72D6F6AB-A7B8-4A52-85BB-537F7FC7BAE5}")}
			                      	};

			var currentObject = TestRequestContractCompatibility<ActivateViewerRequest>(referenceObject);
			Assert.AreEqual(referenceObject.Viewer.Identifier, currentObject.Viewer.Identifier);
		}

		[Test]
		public void TestDicomExplorerNotFoundFault()
		{
			TestResponseContractCompatibility<BaselineDataContracts.DicomExplorerNotFoundFault>(new DicomExplorerNotFoundFault());
		}

		[Test]
		public void TestServerNotFoundFault()
		{
			TestResponseContractCompatibility<BaselineDataContracts.ServerNotFoundFault>(new ServerNotFoundFault());
		}

		[Test]
		public void TestNoLocalStoreFault()
		{
			TestResponseContractCompatibility<BaselineDataContracts.NoLocalStoreFault>(new NoLocalStoreFault());
		}

		[Test]
		public void TestDicomExplorerSearchCriteria()
		{
			var referenceObject = new BaselineDataContracts.DicomExplorerSearchCriteria
			                      	{
			                      		StudyDateFrom = DateTime.Today.AddYears(-1),
			                      		StudyDateTo = DateTime.Now,
			                      		PatientId = "asdf",
			                      		PatientsName = "fdsa",
			                      		AccessionNumber = "anumber",
			                      		StudyDescription = "desu",
			                      		Modalities = new List<string> {"MR", "CT"}
			                      	};

			var currentObject = TestRequestContractCompatibility<DicomExplorerSearchCriteria>(referenceObject);
			Assert.AreEqual(referenceObject.StudyDateFrom, currentObject.StudyDateFrom);
			Assert.AreEqual(referenceObject.StudyDateTo, currentObject.StudyDateTo);
			Assert.AreEqual(referenceObject.PatientId, currentObject.PatientId);
			Assert.AreEqual(referenceObject.PatientsName, currentObject.PatientsName);
			Assert.AreEqual(referenceObject.AccessionNumber, currentObject.AccessionNumber);
			Assert.AreEqual(referenceObject.StudyDescription, currentObject.StudyDescription);
			Assert.AreEqual(referenceObject.Modalities, currentObject.Modalities);
		}

		[Test]
		public void TestSearchLocalStudiesRequest()
		{
			var referenceObject = new BaselineDataContracts.SearchLocalStudiesRequest
			                      	{
			                      		SearchCriteria = new BaselineDataContracts.DicomExplorerSearchCriteria
			                      		                 	{
			                      		                 		PatientId = "asdf"
			                      		                 	}
			                      	};

			var currentObject = TestRequestContractCompatibility<SearchLocalStudiesRequest>(referenceObject);
			Assert.AreEqual(referenceObject.SearchCriteria.PatientId, currentObject.SearchCriteria.PatientId);
		}

		[Test]
		public void TestSearchLocalStudiesResult()
		{
			TestResponseContractCompatibility<BaselineDataContracts.SearchLocalStudiesResult>(new SearchLocalStudiesResult());
		}

		[Test]
		public void TestSearchRemoteStudiesRequest()
		{
			var referenceObject = new BaselineDataContracts.SearchRemoteStudiesRequest
			                      	{
			                      		SearchCriteria = new BaselineDataContracts.DicomExplorerSearchCriteria
			                      		                 	{
			                      		                 		PatientId = "asdf"
			                      		                 	}
			                      	};

			var currentObject = TestRequestContractCompatibility<SearchRemoteStudiesRequest>(referenceObject);
			Assert.AreEqual(referenceObject.SearchCriteria.PatientId, currentObject.SearchCriteria.PatientId);
		}

		[Test]
		public void TestSearchRemoteStudiesResult()
		{
			TestResponseContractCompatibility<BaselineDataContracts.SearchRemoteStudiesResult>(new SearchRemoteStudiesResult());
		}
	}
}

#endif