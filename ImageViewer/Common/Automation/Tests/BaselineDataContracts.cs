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
	/// Legacy data contract definitions as they existed at baseline (version 2.0)
	/// </summary>
	internal static class BaselineDataContracts
	{
		private const string _namespace = "http://www.clearcanvas.ca/imageViewer/automation";

		[DataContract(Name = "NoActiveViewersFault", Namespace = _namespace)]
		public class NoActiveViewersFault {}

		[DataContract(Name = "NoViewersFault", Namespace = _namespace)]
		public class NoViewersFault {}

		[DataContract(Name = "ViewerNotFoundFault", Namespace = _namespace)]
		public class ViewerNotFoundFault
		{
			[DataMember(IsRequired = false)]
			public string FailureDescription { get; set; }
		}

		[DataContract(Name = "OpenStudiesFault", Namespace = _namespace)]
		public class OpenStudiesFault
		{
			[DataMember(IsRequired = false)]
			public string FailureDescription { get; set; }
		}

		[DataContract(Name = "OpenFilesFault", Namespace = _namespace)]
		public class OpenFilesFault
		{
			[DataMember(IsRequired = false)]
			public string FailureDescription { get; set; }
		}

		[DataContract(Name = "Viewer", Namespace = _namespace)]
		public class Viewer
		{
			[DataMember(IsRequired = true)]
			public Guid Identifier { get; set; }

			[DataMember(IsRequired = true)]
			public string PrimaryStudyInstanceUid { get; set; }
		}

		[DataContract(Name = "GetActiveViewersResult", Namespace = _namespace)]
		public class GetActiveViewersResult
		{
			[DataMember(IsRequired = true)]
			public List<Viewer> ActiveViewers { get; set; }
		}

		[DataContract(Name = "GetViewersRequest", Namespace = _namespace)]
		public class GetViewersRequest {}

		[DataContract(Name = "GetViewersResult", Namespace = _namespace)]
		public class GetViewersResult
		{
			[DataMember(IsRequired = true)]
			public List<Viewer> Viewers { get; set; }
		}

		[DataContract(Name = "GetViewerInfoRequest", Namespace = _namespace)]
		public class GetViewerInfoRequest
		{
			[DataMember(IsRequired = true)]
			public Viewer Viewer { get; set; }
		}

		[DataContract(Name = "GetViewerInfoResult", Namespace = _namespace)]
		public class GetViewerInfoResult
		{
			[DataMember(IsRequired = true)]
			public List<string> AdditionalStudyInstanceUids { get; set; }
		}

		[DataContract(Name = "OpenStudiesResult", Namespace = _namespace)]
		public class OpenStudiesResult
		{
			[DataMember(IsRequired = true)]
			public Viewer Viewer { get; set; }
		}

		[DataContract(Name = "OpenStudyInfo", Namespace = _namespace)]
		public class OpenStudyInfo
		{
			[DataMember(IsRequired = true)]
			public string StudyInstanceUid { get; set; }

			[DataMember(IsRequired = false)]
			public string SourceAETitle { get; set; }
		}

		[DataContract(Name = "OpenFilesRequest", Namespace = _namespace)]
		public class OpenFilesRequest
		{
			[DataMember(IsRequired = true)]
			public List<string> Files { get; set; }

			[DataMember(IsRequired = false)]
			public bool? WaitForFilesToOpen { get; set; }

			[DataMember(IsRequired = false)]
			public bool? ReportFaultToUser { get; set; }
		}

		[DataContract(Name = "OpenFilesResult", Namespace = _namespace)]
		public class OpenFilesResult
		{
			[DataMember]
			public Viewer Viewer { get; set; }
		}

		[DataContract(Name = "OpenStudiesRequest", Namespace = _namespace)]
		public class OpenStudiesRequest
		{
			[DataMember(IsRequired = true)]
			public List<OpenStudyInfo> StudiesToOpen { get; set; }

			[DataMember(IsRequired = false)]
			public bool? ActivateIfAlreadyOpen { get; set; }
		}

		[DataContract(Name = "CloseViewerRequest", Namespace = _namespace)]
		public class CloseViewerRequest
		{
			[DataMember(IsRequired = true)]
			public Viewer Viewer { get; set; }
		}

		[DataContract(Name = "ActivateViewerRequest", Namespace = _namespace)]
		public class ActivateViewerRequest
		{
			[DataMember(IsRequired = true)]
			public Viewer Viewer { get; set; }
		}

		[DataContract(Name = "DicomExplorerNotFoundFault", Namespace = _namespace)]
		public class DicomExplorerNotFoundFault {}

		[DataContract(Name = "ServerNotFoundFault", Namespace = _namespace)]
		public class ServerNotFoundFault {}

		[DataContract(Name = "NoLocalStoreFault", Namespace = _namespace)]
		public class NoLocalStoreFault {}

		[DataContract(Name = "DicomExplorerSearchCriteria", Namespace = _namespace)]
		public class DicomExplorerSearchCriteria
		{
			[DataMember(IsRequired = true)]
			public DateTime? StudyDateFrom { get; set; }

			[DataMember(IsRequired = true)]
			public DateTime? StudyDateTo { get; set; }

			[DataMember(IsRequired = true)]
			public string PatientId { get; set; }

			[DataMember(IsRequired = true)]
			public string PatientsName { get; set; }

			[DataMember(IsRequired = true)]
			public string AccessionNumber { get; set; }

			[DataMember(IsRequired = true)]
			public string StudyDescription { get; set; }

			[DataMember(IsRequired = false)]
			public string ReferringPhysiciansName { get; set; }

			[DataMember(IsRequired = true)]
			public List<string> Modalities { get; set; }
		}

		[DataContract(Name = "SearchStudiesRequest", Namespace = _namespace)]
		public abstract class SearchStudiesRequest
		{
			[DataMember(IsRequired = true)]
			public DicomExplorerSearchCriteria SearchCriteria { get; set; }
		}

		[DataContract(Name = "SearchLocalStudiesRequest", Namespace = _namespace)]
		public class SearchLocalStudiesRequest : SearchStudiesRequest {}

		[DataContract(Name = "SearchLocalStudiesResult", Namespace = _namespace)]
		public class SearchLocalStudiesResult {}

		[DataContract(Name = "SearchRemoteStudiesRequest", Namespace = _namespace)]
		public class SearchRemoteStudiesRequest : SearchStudiesRequest
		{
			[DataMember(IsRequired = true)]
			public string AETitle { get; set; }
		}

		[DataContract(Name = "SearchRemoteStudiesResult", Namespace = _namespace)]
		public class SearchRemoteStudiesResult {}
	}
}

#endif