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
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.Automation
{
	/// <summary>
	/// The namespace for all the automation data and service contracts.
	/// </summary>
	public static class AutomationNamespace
	{
		/// <summary>
		/// The namespace for all the automation data and service contracts.
		/// </summary>
		public const string Value = "http://www.clearcanvas.ca/imageViewer/automation";
	}

	#region Viewer Automation

	/// <summary>
	/// Data contract for fault when there are no active viewers.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	[Obsolete("Used only by the deprecated GetActiveViewers method. Replaced by NoViewersFault, which is thrown by the GetViewers method.")]
	public class NoActiveViewersFault {}

	/// <summary>
	/// Data contract for fault when there are no active viewers.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class NoViewersFault {}

	/// <summary>
	/// Data contract for fault when the supplied <see cref="Viewer"/> no longer exists.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ViewerNotFoundFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerNotFoundFault() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerNotFoundFault(string failureDescription)
		{
			FailureDescription = failureDescription;
		}

		/// <summary>
		/// Textual description of the failure.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string FailureDescription { get; set; }
	}

	/// <summary>
	/// Data contract for when a failure occurs opening the requested study (or studies).
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesFault() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesFault(string failureDescription)
		{
			FailureDescription = failureDescription;
		}

		/// <summary>
		/// Textual description of the failure.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string FailureDescription { get; set; }
	}

	/// <summary>
	/// Data contract for when a failure occurs opening the requested study (or studies).
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenFilesFault
	{
		/// <summary>
		/// Textual description of the failure.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string FailureDescription { get; set; }
	}

	/// <summary>
	/// Data contract representing a viewer component or workspace.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class Viewer
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public Viewer(Guid identifier, StudyRootStudyIdentifier primaryStudyIdentifier)
		{
			Identifier = identifier;
			PrimaryStudyIdentifier = primaryStudyIdentifier;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// Deprecated. Consider returning complete primary study identifier values using the overload <see cref="Viewer(Guid,StudyRootStudyIdentifier)"/>.
		/// </remarks>
		[Obsolete("Consider returning complete primary study identifier values using the overload ctor(Guid, StudyRootStudyIdentifier)")]
		public Viewer(Guid identifier, string primaryStudyInstanceUid)
		{
			Identifier = identifier;
			PrimaryStudyInstanceUid = primaryStudyInstanceUid;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Viewer(Guid identifier)
			: this(identifier, (StudyRootStudyIdentifier) null) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Viewer() {}

		/// <summary>
		/// Gets or sets the unique identifier of this <see cref="Viewer"/>.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid Identifier { get; set; }

		/// <summary>
		/// Gets or sets the identifying details of the primary study, or study of interest.
		/// </summary>
		[DataMember(IsRequired = false)]
		public StudyRootStudyIdentifier PrimaryStudyIdentifier { get; set; }

		/// <summary>
		/// Gets or sets the study instance uid of the primary study, or study of interest.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string PrimaryStudyInstanceUid
		{
			get { return PrimaryStudyIdentifier != null ? PrimaryStudyIdentifier.StudyInstanceUid : null; }
			set { if (PrimaryStudyIdentifier == null || PrimaryStudyIdentifier.StudyInstanceUid != value) PrimaryStudyIdentifier = !string.IsNullOrEmpty(value) ? new StudyRootStudyIdentifier {StudyInstanceUid = value} : null; }
		}

		public override bool Equals(object obj)
		{
			if (obj is Viewer)
				return (obj as Viewer).Identifier == Identifier;
			return false;
		}

		public override int GetHashCode()
		{
			return Identifier.GetHashCode();
		}

		public override string ToString()
		{
			return Identifier.ToString();
		}

		public static bool operator ==(Viewer viewer1, Viewer viewer2)
		{
			return Equals(viewer1, viewer2);
		}

		public static bool operator !=(Viewer viewer1, Viewer viewer2)
		{
			return !Equals(viewer1, viewer2);
		}
	}

	/// <summary>
	/// Data contract for results returned from <see cref="IViewerAutomation.GetActiveViewers"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	[Obsolete("Used only by the deprecated GetActiveViewers method. Replaced by GetViewersResult, which is returned by the GetViewers method.")]
	public class GetActiveViewersResult
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public GetActiveViewersResult()
		{
			ActiveViewers = new List<Viewer>();
		}

		/// <summary>
		/// The currently active <see cref="Viewer"/>s.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<Viewer> ActiveViewers { get; set; }
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewersRequest {}

	/// <summary>
	/// Data contract for results returned from <see cref="IViewerAutomation.GetViewers"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewersResult
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public GetViewersResult()
		{
			Viewers = new List<Viewer>();
		}

		/// <summary>
		/// The currently open <see cref="Viewer"/>s.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<Viewer> Viewers { get; set; }
	}

	/// <summary>
	/// Data contract for requests via <see cref="IViewerAutomation.GetViewerInfo"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerInfoRequest
	{
		/// <summary>
		/// Gets or sets the viewer whose info is to be retrieved.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer { get; set; }
	}

	/// <summary>
	/// Data contract for results returned from <see cref="IViewerAutomation.GetViewerInfo"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class GetViewerInfoResult
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public GetViewerInfoResult()
		{
			AdditionalStudyInstanceUids = new List<string>();
		}

		/// <summary>
		/// Gets or sets the study instance uids contained within the <see cref="GetViewerInfoRequest.Viewer"/>,
		/// not including the <see cref="Viewer.PrimaryStudyInstanceUid"/>, or study of interest.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<string> AdditionalStudyInstanceUids { get; set; }

		//TODO: later, could add layout information, visible display sets, etc.
	}

	/// <summary>
	/// Data contracts for results returned from <see cref="IViewerAutomation.OpenStudies"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesResult
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesResult(Viewer viewer)
		{
			Viewer = viewer;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudiesResult() {}

		/// <summary>
		/// Gets or sets the <see cref="Viewer"/> in which the studies were opened.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer { get; set; }
	}

	/// <summary>
	/// Data contract for defining studies to be opened via <see cref="IViewerAutomation.OpenStudies"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudyInfo
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo(StudyIdentifier studyIdentifier)
			: this(studyIdentifier.StudyInstanceUid, studyIdentifier.RetrieveAeTitle) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo(string studyInstanceUid, string sourceAETitle)
		{
			StudyInstanceUid = studyInstanceUid;
			SourceAETitle = sourceAETitle;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo(string studyInstanceUid)
			: this(studyInstanceUid, null) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OpenStudyInfo() {}

		/// <summary>
		/// The Study Instance Uid of the study to be opened.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid { get; set; }

		// TODO (CR Mar 2012): Pass in an AE object?
		/// <summary>
		/// The AE Title where the study is known to reside.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string SourceAETitle { get; set; }
	}

	/// <summary>
	/// Data contract for defining files/directories to be opened via <see cref="IViewerAutomation.OpenFiles"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenFilesRequest
	{
		/// <summary>
		/// A list of files and/or directories that will be loaded recursively into a single viewer.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<string> Files { get; set; }

		/// <summary>
		/// Specifies whether or not the call should block waiting for the files to load into a viewer;
		/// if unspecified, the service will block waiting for the files to open before returning.
		/// </summary>
		[DataMember(IsRequired = false)]
		public bool? WaitForFilesToOpen { get; set; }

		[DataMember(IsRequired = false)]
		public bool? ReportFaultToUser { get; set; }
	}

	/// <summary>
	/// Data contracts for results returned from <see cref="IViewerAutomation.OpenFiles"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenFilesResult
	{
		/// <summary>
		/// Identifier for the viewer in which the files were opened.
		/// </summary>
		/// <remarks>
		/// If <see cref="OpenFilesRequest.WaitForFilesToOpen"/> is false, this value will be null.
		/// </remarks>
		[DataMember]
		public Viewer Viewer { get; set; }
	}

	/// <summary>
	/// Data contract for open studies requests via <see cref="IViewerAutomation.OpenStudies"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class OpenStudiesRequest
	{
		/// <summary>
		/// Gets or sets the list of studies to open; the first study in the list
		/// will be taken to be the primary study or study of interest (<see cref="Viewer.PrimaryStudyInstanceUid"/>).
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<OpenStudyInfo> StudiesToOpen { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not to simply
		/// activate the viewer if the requested primary study is already
		/// the primary study in an existing <see cref="Viewer"/>.
		/// </summary>
		/// <remarks>
		/// When this value is false, a new <see cref="Viewer"/> will always be opened
		/// whether or not the primary study is already the primary study in other <see cref="Viewer"/>s.
		/// </remarks>
		[DataMember(IsRequired = false)]
		public bool? ActivateIfAlreadyOpen { get; set; }

		/// <summary>
		/// Specifies whether or not prior studies should be automatically searched for
		/// and loaded into the viewer along with the requested studies.
		/// </summary>
		/// <remarks>
		/// If not specified, the default value is true.
		/// </remarks>
		[DataMember(IsRequired = false)]
		public bool? LoadPriors { get; set; }

		/// <summary>
		/// When the primary study cannot be opened, a fault exception will be thrown.
		/// This flag specifies whether or not to report the error to the user in the workstation.
		/// </summary>
		/// <remarks>
		/// Regardless of the value of this property, the fault exception is always thrown.
		/// </remarks>
		[DataMember(IsRequired = false)]
		public bool ReportFaultToUser { get; set; }

		//TODO: add study source(s), viewer layout, hanging protocols.
	}

	/// <summary>
	/// Data contract for a request to close a <see cref="Viewer"/> via <see cref="IViewerAutomation.CloseViewer"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class CloseViewerRequest
	{
		/// <summary>
		/// Gets or sets the <see cref="Viewer"/> to be closed.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer { get; set; }
	}

	/// <summary>
	/// Data contract for a request to activate a <see cref="Viewer"/>.
	/// </summary>
	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ActivateViewerRequest
	{
		/// <summary>
		/// Gets or sets the <see cref="Viewer"/> to activate.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Viewer Viewer { get; set; }
	}

	#endregion

	#region Dicom Explorer Automation

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class DicomExplorerNotFoundFault {}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class ServerNotFoundFault {}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class NoLocalStoreFault {}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class DicomExplorerSearchCriteria
	{
		private List<string> _modalities = new List<string>();

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
		public List<string> Modalities
		{
			get { return _modalities; }
			set { _modalities = value; }
		}
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public abstract class SearchStudiesRequest
	{
		[DataMember(IsRequired = true)]
		public DicomExplorerSearchCriteria SearchCriteria { get; set; }
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchLocalStudiesRequest : SearchStudiesRequest {}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchLocalStudiesResult {}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchRemoteStudiesRequest : SearchStudiesRequest
	{
		[DataMember(IsRequired = true)]
		public string AETitle { get; set; }
	}

	[DataContract(Namespace = AutomationNamespace.Value)]
	public class SearchRemoteStudiesResult {}

	#endregion
}