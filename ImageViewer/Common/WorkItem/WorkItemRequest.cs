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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	public static class WorkItemRequestTypeProvider
	{
		private static List<Type> _knownTypes;
		private static readonly object SyncLock = new Object();

		public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignored)
		{
			lock (SyncLock)
			{
				// TODO (CR Jun 2012): Just do static initialization? Then there's no need for the lock.
				if (_knownTypes == null)
				{
					// build the contract map by finding all types having a T attribute
					_knownTypes = (from p in Platform.PluginManager.Plugins
					               from t in p.Assembly.Resolve().GetTypes()
					               let a = AttributeUtils.GetAttribute<WorkItemKnownTypeAttribute>(t)
					               where (a != null)
					               select t).ToList();
				}

				return _knownTypes;
			}
		}
	}

	/// <summary>
	/// Interface for WorkItems that support scheduling within a time window.
	/// </summary>
	public interface IWorkItemRequestTimeWindow
	{
		int? TimeWindowStart { get; set; }

		int? TimeWindowEnd { get; set; }

		DateTime GetScheduledTime(DateTime currentTime, int postponeSeconds);
	}

	/// <summary>
	/// Base Request object for the creation of <see cref="WorkItem"/>s.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("b2d86945-96b7-4563-8281-02142e84ffc3")]
	[WorkItemKnownType]
	public abstract class WorkItemRequest : DataContractBase
	{
		protected WorkItemRequest()
		{
			Priority = WorkItemPriorityEnum.Normal;
		}

		public abstract WorkItemConcurrency ConcurrencyType { get; }

		[DataMember]
		public WorkItemPriorityEnum Priority { get; set; }

		[DataMember]
		public string WorkItemType { get; set; }

		[DataMember]
		public string UserName { get; set; }

		public abstract string ActivityDescription { get; }

		public abstract string ActivityTypeString { get; }

		[DataMember]
		public bool CancellationCanResultInPartialStudy { get; protected set; }
	}

	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("00e165d6-44db-4bf4-b607-a8e82a395964")]
	[WorkItemKnownType]
	public class WorkItemPatient : PatientRootPatientIdentifier
	{
		public WorkItemPatient() {}

		public WorkItemPatient(IPatientData p)
			: base(p) {}

		public WorkItemPatient(DicomAttributeCollection c)
			: base(c) {}
	}

	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("3366be52-823c-484e-b0a7-7344fed16457")]
	[WorkItemKnownType]
	public class WorkItemStudy : StudyIdentifier
	{
		public WorkItemStudy() {}

		public WorkItemStudy(IStudyData s)
			: base(s) {}

		public WorkItemStudy(DicomAttributeCollection c)
			: base(c)
		{
			string modality = c[DicomTags.Modality].ToString();
			ModalitiesInStudy = new[] {modality};
		}
	}

	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("E0BF69EF-1854-441c-9C1B-5D334094CB85")]
	[WorkItemKnownType]
	public abstract class WorkItemStudyRequest : WorkItemRequest
	{
		[DataMember(IsRequired = true)]
		public WorkItemStudy Study { get; set; }

		[DataMember(IsRequired = true)]
		public WorkItemPatient Patient { get; set; }
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("c6a4a14e-e877-45a3-871d-bb06054dd837")]
	[WorkItemKnownType]
	public abstract class DicomSendRequest : WorkItemStudyRequest
	{
		public static string WorkItemTypeString = "DicomSend";

		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.StudyRead; }
		}

		[DataMember]
		public string DestinationServerName { get; set; }

		[DataMember(IsRequired = false)]
		public string DestinationServerHostname { get; set; }

		[DataMember(IsRequired = false)]
		public string DestinationServerAETitle { get; set; }

		[DataMember(IsRequired = false)]
		public int DestinationServerPort { get; set; }

		[DataMember]
		public CompressionType CompressionType { get; set; }

		// TODO (CR Jun 2012 - Med): Expected as a percent?
		[DataMember]
		public int CompressionLevel { get; set; }
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("F0C1BA64-06BD-4E97-BE55-183915656811")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DicomSendStudyRequest : DicomSendRequest
	{
		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.StudyRead; }
		}

		public DicomSendStudyRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
			CancellationCanResultInPartialStudy = true;
		}

		public override string ActivityDescription
		{
			get { return string.Format(SR.DicomSendStudyRequest_ActivityDescription, DestinationServerName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDicomSendStudy; }
		}
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for sending series to a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("EF7A33C7-6B8A-470D-98F4-796780D8E50E")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DicomSendSeriesRequest : DicomSendRequest
	{
		public DicomSendSeriesRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
			CancellationCanResultInPartialStudy = true;
		}

		[DataMember(IsRequired = false)]
		public List<string> SeriesInstanceUids { get; set; }

		public override string ActivityDescription
		{
			get { return string.Format(SR.DicomSendSeriesRequest_ActivityDescription, DestinationServerName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDicomSendSeries; }
		}
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for sending series to a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("75BD907A-45D3-471B-AD0A-DE13D422A794")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DicomSendSopRequest : DicomSendRequest
	{
		public DicomSendSopRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.Normal;
			CancellationCanResultInPartialStudy = true;
		}

		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid { get; set; }

		[DataMember(IsRequired = true)]
		public List<string> SopInstanceUids { get; set; }

		public override string ActivityDescription
		{
			get { return string.Format(SR.DicomSendSopRequest_ActivityDescription, DestinationServerName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDicomSendSop; }
		}
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for publishing files to a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("46DCBBF6-A8B3-4F5E-8611-5712A2BBBEFC")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class PublishFilesRequest : DicomSendRequest
	{
		public PublishFilesRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.Normal;
			DeletionBehaviour = DeletionBehaviour.None;
			CancellationCanResultInPartialStudy = true;
		}

		[DataMember(IsRequired = false)]
		public List<string> SeriesInstanceUids { get; set; }

		[DataMember(IsRequired = false)]
		public List<string> FilePaths { get; set; }

		[DataMember(IsRequired = true)]
		public DeletionBehaviour DeletionBehaviour { get; set; }

		public override string ActivityDescription
		{
			get { return string.Format(SR.PublishFilesRequest_ActivityDescription, DestinationServerName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumPublishFiles; }
		}
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("1c63c863-aa4e-4672-bee5-8aa3db16edd5")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DicomAutoRouteRequest : DicomSendRequest, IWorkItemRequestTimeWindow
	{
		public DicomAutoRouteRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.Normal;
			CancellationCanResultInPartialStudy = true;
		}

		[DataMember(IsRequired = false)]
		public int? TimeWindowStart { get; set; }

		[DataMember(IsRequired = false)]
		public int? TimeWindowEnd { get; set; }

		public override string ActivityDescription
		{
			get { return string.Format(SR.DicomAutoRouteRequest_ActivityDescription, DestinationServerName, Patient.PatientsName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumAutoRoute; }
		}

		public DateTime GetScheduledTime(DateTime currentTime, int postponeSeconds)
		{
			if (!TimeWindowStart.HasValue || !TimeWindowEnd.HasValue || Priority == WorkItemPriorityEnum.Stat)
				return currentTime.AddSeconds(postponeSeconds);

			if (TimeWindowStart.Value > TimeWindowEnd.Value)
			{
				if (currentTime.Hour >= TimeWindowStart.Value
				    || currentTime.Hour < TimeWindowEnd.Value)
				{
					return currentTime.AddSeconds(postponeSeconds);
				}

				return currentTime.Date.AddHours(TimeWindowStart.Value);
			}

			if (currentTime.Hour >= TimeWindowStart.Value
			    && currentTime.Hour < TimeWindowEnd.Value)
			{
				return currentTime.AddSeconds(postponeSeconds);
			}

			return currentTime.Hour < TimeWindowStart.Value
			       	? currentTime.Date.AddHours(TimeWindowStart.Value)
			       	: currentTime.Date.Date.AddDays(1d).AddHours(TimeWindowStart.Value);
		}
	}

	[DataContract(Name = "DeletionBehaviour", Namespace = ImageViewerWorkItemNamespace.Value)]
	public enum DeletionBehaviour
	{
		[EnumMember]
		DeleteOnSuccess = 0,

		[EnumMember]
		DeleteAlways,

		[EnumMember]
		None
	}

	[DataContract(Name = "BadFileBehaviour", Namespace = ImageViewerWorkItemNamespace.Value)]
	public enum BadFileBehaviourEnum
	{
		[EnumMember]
		Ignore = 0,

		[EnumMember]
		Move,

		[EnumMember]
		Delete
	}

	[DataContract(Name = "FileImportBehaviour", Namespace = ImageViewerWorkItemNamespace.Value)]
	public enum FileImportBehaviourEnum
	{
		[EnumMember]
		Move = 0,

		[EnumMember]
		Copy,

		[EnumMember]
		Save
	}

	[DataContract(Name = "WorkItemConcurrency", Namespace = ImageViewerWorkItemNamespace.Value)]
	public enum WorkItemConcurrency
	{
		[EnumMember]
		Exclusive,
		//Note: This is unlikely to be used for anything other than retrieves, but we want anything "study related" to wait for other study related things,
		//but we also need retrieves and "study receive/process" items to be able to run concurrently. Also, since we know a retrieve will ultimately trigger
		//a study process, it is reasonable to make, say, a send for the same study wait for the retrieve to finish.
		[EnumMember]
		StudyUpdateTrigger,

		[EnumMember]
		StudyUpdate,

		[EnumMember]
		StudyDelete,

		[EnumMember]
		StudyRead,

		[EnumMember]
		NonExclusive
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for importing files/studies.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("02b7d427-1107-4458-ade3-67ee6779a766")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class ImportFilesRequest : WorkItemRequest
	{
		public static string WorkItemTypeString = "Import";

		public ImportFilesRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
			CancellationCanResultInPartialStudy = true;
		}

		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.NonExclusive; }
		}

		[DataMember(IsRequired = true)]
		public bool Recursive { get; set; }

		[DataMember(IsRequired = true)]
		public List<string> FileExtensions { get; set; }

		[DataMember(IsRequired = true)]
		public List<string> FilePaths { get; set; }

		[DataMember(IsRequired = true)]
		public BadFileBehaviourEnum BadFileBehaviour { get; set; }

		[DataMember(IsRequired = true)]
		public FileImportBehaviourEnum FileImportBehaviour { get; set; }

		public override string ActivityDescription
		{
			get
			{
				return string.Format(FilePaths.Count > 1
				                     	? SR.ImportFilesRequest_ActivityDescriptionPlural
				                     	: SR.ImportFilesRequest_ActivityDescription, FilePaths.Count);
			}
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumImportFiles; }
		}
	}

	/// <summary>
	/// DICOM Retrieve Request
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("0e04fa53-3f45-4ae2-9444-f3208047757c")]
	[WorkItemKnownType]
	public abstract class DicomRetrieveRequest : WorkItemStudyRequest
	{
		public static string WorkItemTypeString = "DicomRetrieve";

		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.StudyUpdateTrigger; }
		}

		[DataMember]
		public string ServerName { get; set; }

		[DataMember(IsRequired = false)]
		public string ServerHostname { get; set; }

		[DataMember(IsRequired = false)]
		public string ServerAETitle { get; set; }

		[DataMember(IsRequired = false)]
		public int ServerPort { get; set; }
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for retrieving a study from a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("61AB2801-5284-480B-B054-F0314865D84F")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DicomRetrieveStudyRequest : DicomRetrieveRequest
	{
		public DicomRetrieveStudyRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
			CancellationCanResultInPartialStudy = true;
		}

		public override string ActivityDescription
		{
			get { return string.Format(SR.DicomRetreiveRequest_ActivityDescription, ServerName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDicomRetrieve; }
		}
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for retrieving a study from a DICOM AE.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("09547DF4-E8B8-45E8-ABAF-33159E2C7098")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DicomRetrieveSeriesRequest : DicomRetrieveRequest
	{
		public DicomRetrieveSeriesRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
			CancellationCanResultInPartialStudy = true;
		}

		[DataMember(IsRequired = false)]
		public List<string> SeriesInstanceUids { get; set; }

		public override string ActivityDescription
		{
			get { return string.Format(SR.DicomRetreiveSeriesRequest_ActivityDescription, ServerName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDicomRetrieve; }
		}
	}

	/// <summary>
	/// Abstract Study Process Request
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("4d22984a-e750-467c-ab89-f680be38c6c1")]
	[WorkItemKnownType]
	public abstract class ProcessStudyRequest : WorkItemStudyRequest
	{
		public static string WorkItemTypeString = "ProcessStudy";

		protected ProcessStudyRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
			CancellationCanResultInPartialStudy = true;
		}
	}

	/// <summary>
	/// DICOM Receive Study Request
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("146cc54f-7b98-468b-948a-415eeffd3d7f")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DicomReceiveRequest : ProcessStudyRequest
	{
		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.StudyUpdate; }
		}

		[DataMember(IsRequired = true)]
		public string SourceServerName { get; set; }

		public override string ActivityDescription
		{
			get { return string.Format(SR.DicomReceiveRequest_ActivityDescription, SourceServerName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDicomReceive; }
		}
	}

	/// <summary>
	/// DICOM Import Study Request
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("2def790a-8039-4fc5-85d6-f4d3be3f2d8e")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class ImportStudyRequest : ProcessStudyRequest
	{
		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.StudyUpdate; }
		}

		public override string ActivityDescription
		{
			get { return string.Format(SR.ImportStudyRequest_AcitivityDescription); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumImportStudy; }
		}
	}

	/// <summary>
	/// ReindexRequest Request
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("875D13F2-621D-4277-8A32-34D9BF5AE40B")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class ReindexRequest : WorkItemRequest
	{
		public static string WorkItemTypeString = "ReIndex";

		public ReindexRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
		}

		public override WorkItemConcurrency ConcurrencyType
		{
			// TODO (CR Jun 2012): NonStudy seems a bit odd, since it's actually rewriting all studies.
			get { return WorkItemConcurrency.Exclusive; }
		}

		public override string ActivityDescription
		{
			get { return SR.ReindexRequest_ActivityDescription; }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumReIndex; }
		}
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for deleting a study.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("0A7BE406-E5BE-4E10-997F-BCA37D53FED7")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DeleteStudyRequest : WorkItemStudyRequest
	{
		public static string WorkItemTypeString = "DeleteStudy";

		public DeleteStudyRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
		}

		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.StudyDelete; }
		}

		public override string ActivityDescription
		{
			get { return string.Format(SR.DeleteStudyRequest_ActivityDescription); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDeleteStudy; }
		}
	}

	/// <summary>
	/// <see cref="WorkItemRequest"/> for deleting series.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("64BCF5FF-1AFD-409B-B0EB-1E576124D61E")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class DeleteSeriesRequest : WorkItemStudyRequest
	{
		public static string WorkItemTypeString = "DeleteSeries";

		public DeleteSeriesRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.High;
		}

		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.StudyDelete; }
		}

		[DataMember(IsRequired = false)]
		public List<string> SeriesInstanceUids { get; set; }

		public override string ActivityDescription
		{
			get { return string.Format(SR.DeleteSeriesRequest_ActivityDescription); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumDeleteSeries; }
		}
	}

	/// <summary>
	/// ReapplyRules Request
	/// </summary>
	[DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("9361447F-C14F-498C-B0EA-40664F2BB396")]
	[WorkItemKnownType]
	[WorkItemRequest]
	public class ReapplyRulesRequest : WorkItemRequest
	{
		public static string WorkItemTypeString = "ReapplyRules";

		public ReapplyRulesRequest()
		{
			WorkItemType = WorkItemTypeString;
			Priority = WorkItemPriorityEnum.Normal;
		}

		public override WorkItemConcurrency ConcurrencyType
		{
			get { return WorkItemConcurrency.NonExclusive; }
		}

		/// <summary>
		/// The rule to re-apply.  May be null, in which case all rules are re-applied.
		/// </summary>
		[DataMember]
		public string RuleId { get; set; }

		/// <summary>
		/// The name of the rule to re-apply.  May be null in the case where all rules are being re-applied.
		/// </summary>
		[DataMember]
		public string RuleName { get; set; }

		[DataMember(IsRequired = true)]
		public bool ApplyRouteActions { get; set; }

		[DataMember(IsRequired = true)]
		public bool ApplyDeleteActions { get; set; }

		public override string ActivityDescription
		{
			get { return string.IsNullOrEmpty(RuleId) ? SR.ReapplyRulesRequestMultiple_ActivityDescription : string.Format(SR.ReapplyRulesRequest_ActivityDescription, RuleName); }
		}

		public override string ActivityTypeString
		{
			get { return SR.ActivityTypeEnumReapplyRules; }
		}
	}
}