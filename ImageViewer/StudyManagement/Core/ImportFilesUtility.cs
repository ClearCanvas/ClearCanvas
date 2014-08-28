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

#region Additional permission to link with SQL Server Compact Edition

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with SQL Server Compact Edition (or a modified version of that library),
// containing parts covered by the terms of the SQL Server Compact Edition
// EULA, the licensors of this Program grant you additional permission to
// convey the resulting work.

#endregion

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Command;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;
using AuditedInstances = ClearCanvas.Dicom.Audit.AuditedInstances;
using EventResult = ClearCanvas.Dicom.Audit.EventResult;
using EventSource = ClearCanvas.Dicom.Audit.EventSource;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
	/// <summary>
	/// Class used for returning result information when processing.  Used for importing.
	/// </summary>
	public class DicomProcessingResult
	{
		public String AccessionNumber;
		public String StudyInstanceUid;
		public String SeriesInstanceUid;
		public String SopInstanceUid;
		public String PatientsName;
		public String PatientId;
		public bool Successful;
		public String ErrorMessage;
		public DicomStatus DicomStatus;
		public bool RestoreRequested;
		public bool RetrySuggested;

		public void SetError(DicomStatus status, String message)
		{
			Successful = false;
			DicomStatus = status;
			ErrorMessage = message;
		}

		public void Initialize()
		{
			Successful = true;
			ErrorMessage = string.Empty;
			DicomStatus = DicomStatuses.Success;
		}
	}

	/// <summary>
	/// A context object used when importing a batch of DICOM SOP Instances from a DICOM association.
	/// </summary>
	public class DicomReceiveImportContext : ImportFilesContext
	{
		#region Private Members

		private readonly IWorkItemActivityMonitor _monitor;
		private readonly IDicomServiceNode _dicomServerNode;
		private readonly string _hostname;

		private IWorkItemActivityMonitorService _activityMonitorService;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sourceAE">The AE title of the remote application sending the SOP Instances.</param>
		/// <param name="configuration">Storage configuration. </param>
		/// <param name="hostname">The IP Address the remote app is connecting with.</param>
		/// <param name="auditSource">The source of the request for auditing purposes </param>
		public DicomReceiveImportContext(string sourceAE, string hostname, StorageConfiguration configuration, EventSource auditSource) : base(sourceAE, configuration, auditSource)
		{
			_monitor = WorkItemActivityMonitor.Create(false);
			_monitor.WorkItemsChanged += WorkItemsChanged;

			var serverList = ServerDirectory.GetRemoteServersByAETitle(sourceAE);
			if (serverList.Count == 1)
				_dicomServerNode = CollectionUtils.FirstElement(serverList);

			_hostname = hostname;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Publishes changing work item activity to the <see cref="IWorkItemActivityMonitorService"/>.
		/// </summary>
		/// <param name="item">The work item data</param>
		public void PublishWorkItemActivity(WorkItemData item)
		{
			if (_activityMonitorService == null)
				_activityMonitorService = Platform.GetService<IWorkItemActivityMonitorService>();

			_activityMonitorService.Publish(new WorkItemPublishRequest {Item = item});
		}

		/// <summary>
		/// Create a StudyProcessRequest object for a specific SOP Instance.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public override ProcessStudyRequest CreateRequest(DicomMessageBase message)
		{
			var request = new DicomReceiveRequest
			              {
				              // TODO (CR Jun 2012): Ok, but anyone writing code that relied on these request objects may take
				              // for granted that it is in fact always a server name. Might be better to put 3 properties on the request
				              // and document how they work (SourceServerName, SourceAE, Hostname), or change the property to a
				              // data contract object (ServerDescriptor).
				              SourceServerName = _dicomServerNode == null ? string.Format("{0}/{1}", SourceAE, _hostname) : _dicomServerNode.Name,
				              Priority = WorkItemPriorityEnum.High,
				              Patient = new WorkItemPatient(message.DataSet),
				              Study = new WorkItemStudy(message.DataSet)
			              };

			return request;
		}

		/// <summary>
		/// Create a <see cref="ProcessStudyProgress"/> object for the type of import.
		/// </summary>
		/// <returns></returns>
		public override ProcessStudyProgress CreateProgress()
		{
			return new ProcessStudyProgress {IsCancelable = false, TotalFilesToProcess = 1};
		}

		/// <summary>
		/// Cleanup the activity monitor connections
		/// </summary>
		public void Cleanup()
		{
			var disposableService = _activityMonitorService as IDisposable;
			if (disposableService != null) disposableService.Dispose();
			_activityMonitorService = null;

			_monitor.WorkItemsChanged -= WorkItemsChanged;
			_monitor.Dispose();
		}

		#endregion

		#region Private Methods

		private void WorkItemsChanged(object sender, WorkItemsChangedEventArgs e)
		{
			foreach (var item in e.ChangedItems)
			{
				if (item.Request is DicomReceiveRequest)
				{
					lock (StudyWorkItemsSyncLock)
					{
						WorkItem workItem;
						if (StudyWorkItems.TryGetValue(item.StudyInstanceUid, out workItem))
						{
							if (workItem.Oid == item.Identifier)
							{
								workItem.Status = item.Status;
								workItem.Progress = item.Progress;
							}
						}
					}
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// A context object used when importing a batch of DICOM SOP Instances from disk.
	/// </summary>
	public class ImportStudyContext : ImportFilesContext
	{
		#region Contructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sourceAE">The local AE title of the application importing the studies.</param>
		/// <param name="configuration">The storage configuration. </param>
		/// <param name="auditSource">The source of the import. </param>
		public ImportStudyContext(string sourceAE, StorageConfiguration configuration, EventSource auditSource)
			: base(sourceAE, configuration, auditSource) {}

		#endregion

		#region Public Methods

		/// <summary>
		/// Create a <see cref="ProcessStudyRequest"/> object for a specific SOP Instnace.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public override ProcessStudyRequest CreateRequest(DicomMessageBase message)
		{
			var request = new ImportStudyRequest
			              {
				              Priority = WorkItemPriorityEnum.High,
				              Patient = new WorkItemPatient(message.DataSet),
				              Study = new WorkItemStudy(message.DataSet)
			              };
			var participant = (AuditActiveParticipant) AuditSource;
			if (participant != null)
				request.UserName = participant.UserName;
			return request;
		}

		/// <summary>
		/// Create a <see cref="ProcessStudyProgress"/> object for the type of import.
		/// </summary>
		/// <returns></returns>
		public override ProcessStudyProgress CreateProgress()
		{
			return new ProcessStudyProgress {IsCancelable = false, TotalFilesToProcess = 1};
		}

		#endregion
	}

	/// <summary>
	/// Encapsulates the context of the application when <see cref="ImportFilesUtility"/> is called.
	/// </summary>
	public abstract class ImportFilesContext
	{
		#region Constructors

		/// <summary>
		/// Creates an instance of <see cref="ImportFilesContext"/> to be used
		/// by <see cref="ImportFilesUtility"/> 
		/// </summary>
		protected ImportFilesContext(string sourceAE, StorageConfiguration configuration, EventSource auditSource)
		{
			StudyWorkItems = new ObservableDictionary<string, WorkItem>();
			FailedStudyAudits = new Dictionary<string, string>();
			SourceAE = sourceAE;
			StorageConfiguration = configuration;
			AuditSource = auditSource;
			ExpirationDelaySeconds = WorkItemServiceSettings.Default.ExpireDelaySeconds;
		}

		#endregion

		#region Public Properties

		// TODO (CR Jul 2012): Ideally, this could all be managed internally in this class.
		/// <summary>
		/// Sync object used when accessing <see cref="StudyWorkItems"/> 
		/// </summary>
		public readonly object StudyWorkItemsSyncLock = new object();

		/// <summary>
		/// Gets the source AE title where the image(s) are imported from
		/// </summary>
		public string SourceAE { get; private set; }

		/// <summary>
		/// Map of the studies and corresponding WorkItems items for the current context
		/// </summary>
		public ObservableDictionary<string, WorkItem> StudyWorkItems { get; private set; }

		/// <summary>
		/// Dictionary of all the studies that have audited a failure import, so we don't duplicate
		/// </summary>
		public Dictionary<string, string> FailedStudyAudits { get; private set; }

		/// <summary>
		/// Storage configuration.
		/// </summary>
		public StorageConfiguration StorageConfiguration { get; private set; }

		/// <summary>
		/// True if a fatal error occurred during import that would cause subsequent imports to also fail.
		/// </summary>
		public bool FatalError { get; set; }

		/// <summary>
		/// Delay to expire inserted WorkItems
		/// </summary>
		public int ExpirationDelaySeconds { get; set; }

		/// <summary>
		/// Event Source for Audit messages
		/// </summary>
		public EventSource AuditSource { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Abstract method for creating a <see cref="ProcessStudyRequest"/> object for the given DICOM message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public abstract ProcessStudyRequest CreateRequest(DicomMessageBase message);

		/// <summary>
		/// Abstract method for creating a <see cref="ProcessStudyProgress"/> object for the request.
		/// </summary>
		/// <returns></returns>
		public abstract ProcessStudyProgress CreateProgress();

		#endregion
	}

	/// <summary>
	/// Import utility for importing specific SOP Instances, either in memory from the network or on disk.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that the files being imported do not have to belong to the same study.  ImportFilesUtility will 
	/// automatically detect the study the files belong to, and import them to the proper location.
	/// </para>
	/// </remarks>
	public class ImportFilesUtility
	{
		#region Private Members

		private readonly ImportFilesContext _context;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates an instance of <see cref="ImportFilesUtility"/> to import DICOM object(s)
		/// into the system.
		/// </summary>
		/// <param name="context">The context of the operation.</param>
		public ImportFilesUtility(ImportFilesContext context)
		{
			Platform.CheckForNullReference(context, "context");
			_context = context;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Imports the specified <see cref="DicomMessageBase"/> object into the system.
		/// The object will be inserted into the <see cref="WorkItem"/> for processing
		/// </summary>
		/// <param name="message">The DICOM object to be imported.</param>
		/// <param name="badFileBehavior"> </param>
		/// <param name="fileImportBehaviour"> </param>
		/// <returns>An instance of <see cref="DicomProcessingResult"/> that describes the result of the processing.</returns>
		/// <exception cref="DicomDataException">Thrown when the DICOM object contains invalid data</exception>
		public DicomProcessingResult Import(DicomMessageBase message, BadFileBehaviourEnum badFileBehavior, FileImportBehaviourEnum fileImportBehaviour)
		{
			Platform.CheckForNullReference(message, "message");
			String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
			String accessionNumber = message.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);
			String patientsName = message.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);
			String patientId = message.DataSet[DicomTags.PatientId].GetString(0, string.Empty);

			var result = new DicomProcessingResult
			             {
				             Successful = true,
				             StudyInstanceUid = studyInstanceUid,
				             SeriesInstanceUid = seriesInstanceUid,
				             SopInstanceUid = sopInstanceUid,
				             AccessionNumber = accessionNumber,
				             PatientsName = patientsName,
				             PatientId = patientId,
			             };

			WorkItem workItem;
			lock (_context.StudyWorkItemsSyncLock)
				_context.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem);

			try
			{
				Validate(message);
			}
			catch (DicomDataException e)
			{
				result.SetError(DicomStatuses.ProcessingFailure, e.Message);
				AuditFailure(result);
				return result;
			}

			if (workItem != null)
			{
				if (workItem.Status == WorkItemStatusEnum.Deleted || workItem.Status == WorkItemStatusEnum.DeleteInProgress || workItem.Status == WorkItemStatusEnum.Canceled || workItem.Status == WorkItemStatusEnum.Canceling)
				{
					// TODO Marmot (CR June 2012): not DicomStatuses.Cancel?
					result.SetError(DicomStatuses.StorageStorageOutOfResources, "Canceled by user");
					AuditFailure(result);
					return result;
				}
			}

			if (LocalStorageMonitor.IsMaxUsedSpaceExceeded)
			{
				//The input to this method is a VALID DICOM file, and we know we should have stored it if it weren't for
				//the fact that we're out of disk space. So, we insert the work item UID anyway, knowing that it'll cause
				//the work item to fail. In fact, that's why we're doing it.
				result.SetError(DicomStatuses.StorageStorageOutOfResources, string.Format("Import failed, disk space usage exceeded"));
				InsertFailedWorkItemUid(workItem, message, result);

				_context.FatalError = true;
				AuditFailure(result);
				return result;
			}

			Process(message, fileImportBehaviour, workItem, result);

			if (result.DicomStatus != DicomStatuses.Success)
			{
				if (result.RetrySuggested)
				{
					Platform.Log(LogLevel.Warn,
					             "Failure importing file with retry suggested, retrying Import of file: {0}",
					             sopInstanceUid);

					Process(message, fileImportBehaviour, workItem, result);
				}
			}

			if (result.DicomStatus != DicomStatuses.Success)
			{
				//The input to this method is a VALID DICOM file, and we know we should have stored it if it weren't for
				//the fact that we're out of disk space. So, we insert the work item UID anyway, knowing that it'll cause
				//the work item to fail. In fact, that's why we're doing it.
				InsertFailedWorkItemUid(workItem, message, result);

				AuditFailure(result);
			}

			return result;
		}

		public void InsertFailedWorkItemUid(WorkItem workItem, DicomMessageBase message, DicomProcessingResult result, int tryCount = 1)
		{
			if (tryCount < 0)
				tryCount = 1;

			int tries = 0;
			while (tries++ < tryCount)
			{
				using (var commandProcessor = new ViewerCommandProcessor(String.Format("Processing Sop Instance {0}", result.SopInstanceUid)))
				{
					var fileName = Guid.NewGuid().ToString() + ".dcm";
					var insertWorkItemCommand = CreateWorkItemCommand(workItem, result, message, true, fileName);
					//Fail the Uid immediately, since we know the file isn't there.
					insertWorkItemCommand.WorkItemUid.Failed = true;
					commandProcessor.AddCommand(insertWorkItemCommand);

					if (commandProcessor.Execute())
					{
						IncrementTotalFiles(insertWorkItemCommand, result.StudyInstanceUid, result.ErrorMessage);
						return;
					}
				}

				Thread.Sleep(10);
			}

			Platform.Log(LogLevel.Error, "Failed to insert failed work item UID after {0} attempts (Sop Instance UID={1}).", tries, result.SopInstanceUid);
		}

		/// <summary>
		/// Resets any idle study process work items associated with the files imported in the current import context
		/// </summary>
		public void PulseStudyWorkItems()
		{
			try
			{
				lock (_context.StudyWorkItemsSyncLock)
				{
					using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
					{
						var broker = context.GetWorkItemBroker();
						var scheduledTime = Platform.Time;
                        
						foreach (var workItem in _context.StudyWorkItems.Values.Select(x => broker.GetWorkItem(x.Oid)).Where(x => x != null && x.Status == WorkItemStatusEnum.Idle))
						{
							workItem.ProcessTime = scheduledTime;
							workItem.ExpirationTime = scheduledTime;
						}

						context.Commit();
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to pulse study work items");
			}
		}

		public void AuditFailure(DicomProcessingResult result)
		{
			// This primarily happens with DICOMDIR files
			if (string.IsNullOrEmpty(result.StudyInstanceUid))
				return;

			if (!_context.FailedStudyAudits.ContainsKey(result.StudyInstanceUid))
			{
				var auditedInstances = new AuditedInstances();

				auditedInstances.AddInstance(result.PatientId, result.PatientsName,
				                             result.StudyInstanceUid);

				AuditHelper.LogImportStudies(auditedInstances, _context.AuditSource, EventResult.MajorFailure);

				_context.FailedStudyAudits.Add(result.StudyInstanceUid, result.StudyInstanceUid);
			}
		}

		#endregion

		#region Private Methods

		private void Process(DicomMessageBase message, FileImportBehaviourEnum fileImportBehaviour, WorkItem workItem, DicomProcessingResult result)
		{
			result.Initialize();

			// Use the command processor for rollback capabilities.
			using (var commandProcessor = new ViewerCommandProcessor(String.Format("Processing Sop Instance {0}", result.SopInstanceUid)))
			{
				try
				{
					var studyLocation = new StudyLocation(result.StudyInstanceUid);
					String destinationFile = studyLocation.GetSopInstancePath(result.SeriesInstanceUid, result.SopInstanceUid);

					DicomFile file = ConvertToDicomFile(message, destinationFile, _context.SourceAE);

					// Create the Study Folder, if need be
					commandProcessor.AddCommand(new CreateDirectoryCommand(studyLocation.StudyFolder));

					bool duplicateFile = false;
					string dupName = null;

					if (File.Exists(destinationFile))
					{
						// TODO (CR Jun 2012): Shouldn't the commands themselves make this decision at the time
						// the file is being saved? Otherwise, what happens if the same SOP were being saved 2x simultaneously.
						// I know the odds are low, but just pointing it out.
						duplicateFile = true;
						dupName = Guid.NewGuid().ToString() + ".dcm";
						destinationFile = Path.Combine(Path.GetDirectoryName(destinationFile) ?? string.Empty, dupName);
					}

					if (fileImportBehaviour == FileImportBehaviourEnum.Move)
					{
						commandProcessor.AddCommand(CommandFactory.CreateRenameFileCommand(file.Filename, destinationFile, true));
					}
					else if (fileImportBehaviour == FileImportBehaviourEnum.Copy)
					{
						commandProcessor.AddCommand(CommandFactory.CreateCopyFileCommand(file.Filename, destinationFile, true));
					}
					else if (fileImportBehaviour == FileImportBehaviourEnum.Save)
					{
						commandProcessor.AddCommand(CommandFactory.CreateSaveDicomFileCommand(destinationFile, file, true));
					}

					var insertWorkItemCommand = CreateWorkItemCommand(workItem, result, file, duplicateFile, dupName);
					commandProcessor.AddCommand(insertWorkItemCommand);

					if (commandProcessor.Execute())
					{
						result.DicomStatus = DicomStatuses.Success;
						IncrementTotalFiles(insertWorkItemCommand, result.StudyInstanceUid);
					}
					else
					{
						if (commandProcessor.FailureException is ChangeConflictException
						    || commandProcessor.FailureException is SqlCeLockTimeoutException)
							result.RetrySuggested = true; // Change conflict or lock timeout may work if we just retry

						Platform.Log(LogLevel.Warn, "Failure Importing file: {0}", file.Filename);
						string failureMessage = String.Format(
							"Failure processing message: {0}. Sending failure status.",
							commandProcessor.FailureReason);
						result.SetError(DicomStatuses.ProcessingFailure, failureMessage);

						// processor already rolled back
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", commandProcessor.Description);
					commandProcessor.Rollback();
					result.SetError(result.DicomStatus ?? DicomStatuses.ProcessingFailure, e.Message);
				}
			}
		}

		private void IncrementTotalFiles(InsertWorkItemCommand insertWorkItemCommand, string studyInstanceUid, string errorMessage = null)
		{
			bool foundStudy;
			lock (_context.StudyWorkItemsSyncLock)
				foundStudy = _context.StudyWorkItems.ContainsKey(studyInstanceUid);
			if (foundStudy)
			{
				// First image imported already has the TotalFilesToProcess pre-set to 1, so only update after the first
				var progress = insertWorkItemCommand.WorkItem.Progress as ProcessStudyProgress;
				if (progress != null)
				{
					using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
					{
						var broker = context.GetWorkItemBroker();

						insertWorkItemCommand.WorkItem = broker.GetWorkItem(insertWorkItemCommand.WorkItem.Oid);
						progress = insertWorkItemCommand.WorkItem.Progress as ProcessStudyProgress;
						if (progress != null)
						{
							progress.TotalFilesToProcess++;
							if (!string.IsNullOrEmpty(errorMessage))
								progress.StatusDetails = errorMessage;

							insertWorkItemCommand.WorkItem.Progress = progress;
						}

						context.Commit();
					}
				}
			}
			// Save the updated WorkItem, note that this also publishes the workitem automatically
			lock (_context.StudyWorkItemsSyncLock)
				_context.StudyWorkItems[studyInstanceUid] = insertWorkItemCommand.WorkItem;
		}

		private InsertWorkItemCommand CreateWorkItemCommand(WorkItem workItem, DicomProcessingResult result, DicomMessageBase file, bool duplicateFile, string duplicateFileName)
		{
			InsertWorkItemCommand insertWorkItemCommand;
			if (duplicateFile)
			{
				insertWorkItemCommand = workItem != null
					? new InsertWorkItemCommand(workItem, result.StudyInstanceUid,
					                            result.SeriesInstanceUid, result.SopInstanceUid,
					                            duplicateFileName)
					: new InsertWorkItemCommand(_context.CreateRequest(file),
					                            _context.CreateProgress(), result.StudyInstanceUid,
					                            result.SeriesInstanceUid, result.SopInstanceUid,
					                            duplicateFileName);
			}
			else
			{
				insertWorkItemCommand = workItem != null
					? new InsertWorkItemCommand(workItem, result.StudyInstanceUid,
					                            result.SeriesInstanceUid, result.SopInstanceUid)
					: new InsertWorkItemCommand(_context.CreateRequest(file),
					                            _context.CreateProgress(), result.StudyInstanceUid,
					                            result.SeriesInstanceUid, result.SopInstanceUid);
			}

			insertWorkItemCommand.ExpirationDelaySeconds = _context.ExpirationDelaySeconds;
			return insertWorkItemCommand;
		}

		private static void Validate(DicomMessageBase message)
		{
			String sopClassUid = message.MetaInfo[DicomTags.MediaStorageSopClassUid].GetString(0, string.Empty);
			if (sopClassUid.Equals(SopClass.MediaStorageDirectoryStorageUid))
				throw new DicomDataException("Unable to import DICOMDIR files");

			String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);

			if (string.IsNullOrEmpty(studyInstanceUid))
				throw new DicomDataException("Study Instance UID does not have a value.");

			String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);

			if (string.IsNullOrEmpty(seriesInstanceUid))
				throw new DicomDataException("Series Instance UID does not have a value.");

			String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
			if (string.IsNullOrEmpty(sopInstanceUid))
				throw new DicomDataException("SOP Instance UID does not have a value.");
		}

		private static DicomFile ConvertToDicomFile(DicomMessageBase message, string filename, string sourceAE)
		{
			// This routine sets some of the group 0x0002 elements.
			DicomFile file;
			if (message is DicomFile)
			{
				file = message as DicomFile;
			}
			else if (message is DicomMessage)
			{
				file = new DicomFile(message as DicomMessage, filename);
			}
			else
			{
				throw new NotSupportedException(String.Format("Cannot convert {0} to DicomFile", message.GetType()));
			}

			file.SourceApplicationEntityTitle = sourceAE;
			file.TransferSyntax = message.TransferSyntax.Encapsulated ? message.TransferSyntax : TransferSyntax.ExplicitVrLittleEndian;

			return file;
		}

		#endregion
	}
}