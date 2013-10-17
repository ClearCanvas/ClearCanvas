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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Events;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Rules;
using SaveDicomFileCommand = ClearCanvas.ImageServer.Core.Command.SaveDicomFileCommand;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Enum for telling the processor if a NewSop or UpdatedSop is being imported
	/// </summary>
	public enum SopInstanceProcessorSopType
	{
		NewSop,
		UpdatedSop,
		ReprocessedSop
	}

	/// <summary>
	/// Encapsulates the context of the 'StudyProcess' operation.
	/// </summary>
	public class StudyProcessorContext
	{
		#region Private Members

	    private readonly object _syncLock = new object();
        private readonly StudyStorageLocation _storageLocation;
	    private readonly WorkQueue _workQueue;
		private Study _study;
		private ServerRulesEngine _sopProcessedRulesEngine;
		private ServerRulesEngine _sopCompressionRulesEngine;
		private List<BaseImageLevelUpdateCommand> _updateCommands;
	    #endregion

		#region Constructors

		public StudyProcessorContext(StudyStorageLocation storageLocation, WorkQueue queue)
		{
		    Platform.CheckForNullReference(storageLocation, "storageLocation");
		    _storageLocation = storageLocation;
		    _workQueue = queue;
		}

        public StudyProcessorContext(StudyStorageLocation storageLocation)
        {
            Platform.CheckForNullReference(storageLocation, "storageLocation");
            _storageLocation = storageLocation;
            _workQueue = null;
        }

		#endregion

		#region Public Properties

        /// <summary>
        /// Gets
        /// </summary>
        internal ServerPartition Partition
		{
            get { return _storageLocation.ServerPartition; }
		}

		internal Study Study
		{
            get
            {
                if (_study==null)
                {
                    lock (_syncLock)
                    {
                        _study = _storageLocation.LoadStudy(ServerExecutionContext.Current.PersistenceContext);    
                    }
                    
                }
                return _study;
            }

		}

	    public ServerRulesEngine SopProcessedRulesEngine
		{
			get
			{
				lock (_syncLock)
				{
					if (_sopProcessedRulesEngine == null)
					{
						_sopProcessedRulesEngine =
							new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Partition.GetKey());
						_sopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
						_sopProcessedRulesEngine.Load();
					}
				}
				return _sopProcessedRulesEngine;
			}
			set
			{
                lock (_syncLock)
                {
                    _sopProcessedRulesEngine = value;
                }
			}
		}

		public ServerRulesEngine SopCompressionRulesEngine
		{
			get
			{
				lock (_syncLock)
				{
					if (_sopCompressionRulesEngine == null)
					{
						_sopCompressionRulesEngine =
							new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Partition.GetKey());
						_sopCompressionRulesEngine.AddIncludeType(ServerRuleTypeEnum.SopCompress);
						_sopCompressionRulesEngine.Load();
					}
				}
				return _sopCompressionRulesEngine;
			}
			set
			{
				lock (_syncLock)
				{
					_sopCompressionRulesEngine = value;
				}
			}
		}

	    public StudyStorageLocation StorageLocation
	    {
	        get { return _storageLocation; }
	    }

        public WorkQueue WorkQueueEntry
        {
            get { return _workQueue; }
        }

		public List<BaseImageLevelUpdateCommand> UpdateCommands
		{
			get
			{
				lock (_syncLock)
				{
					if (_updateCommands == null)
					{
						_updateCommands = new List<BaseImageLevelUpdateCommand>();
					}
					return _updateCommands;
				}
			}
		}
	    #endregion
	}

    /// <summary>
    /// Represents an event that occurs when a sop instance is being inserted into the study by the <see cref="SopInstanceProcessor"/>
    /// </summary>
    public class SopInsertingEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="ServerCommandProcessor"/> used by the <see cref="SopInstanceProcessor"/> to insert the sop instance into the study.
        /// </summary>
        public ServerCommandProcessor Processor { get; set; }
    }

	    

	/// <summary>
	/// Processor for Sop Instances being inserted into the database.
	/// </summary>
	public class SopInstanceProcessor
    {
        
        #region Private Members

        private readonly StudyProcessorContext _context;
		private readonly InstanceStatistics _instanceStats = new InstanceStatistics();
		private string _modality;
	    private readonly PatientNameRules _patientNameRules;

	    #endregion

		#region Constructors

		public SopInstanceProcessor(StudyProcessorContext context)
		{
            Platform.CheckForNullReference(context, "context");
		    _context = context;
		    _patientNameRules = new PatientNameRules(context.Study);
		}

		#endregion

		#region Public Properties

        /// <summary>
        /// Gets or sets a value to indicate whether to apply the Patient's Name rules when processing a Dicom object.
        /// </summary>
        public bool EnforceNameRules { get; set; }

		public string Modality
		{
			get { return _modality; }
		}

		public InstanceStatistics InstanceStats
		{
			get { return _instanceStats; }
		}

		#endregion

        #region Events
        /// <summary>
        /// Occurs when the SOP instance is about to be added to the study.
        /// </summary>
        public event EventHandler<SopInsertingEventArgs> OnInsertingSop;
        #endregion

		#region Public Methods

		/// <summary>
		/// Process a specific DICOM file related to a <see cref="WorkQueue"/> request.
		/// </summary>
		/// <remarks>
		/// <para>
		/// On success and if <see cref="uid"/> is set, the <see cref="WorkQueueUid"/> field is deleted.
		/// </para>
		/// </remarks>
		/// <param name="stream">The <see cref="StudyXml"/> file to update with information from the file.</param>
		/// <param name="group">A group the sop is associated with.</param>
		/// <param name="file">The file to process.</param>
		/// <param name="compare">Flag to compare the demographics of <see cref="file"/> with the demographics in the database</param>
		/// <param name="retry">Flag telling if the item should be retried on failure.  Note that if the item is a duplicate, the WorkQueueUid item is not failed. </param>
		/// <param name="uid">An optional WorkQueueUid associated with the entry, that will be deleted upon success or failed on failure.</param>
		/// <param name="deleteFile">An option file to delete as part of the process</param>
		/// <param name="sopType">Flag telling if the SOP is a new or updated SOP</param>
        /// <exception cref="Exception"/>
        /// <exception cref="DicomDataException"/>
		public  ProcessingResult ProcessFile(string group, DicomFile file, StudyXml stream, bool compare, bool retry, WorkQueueUid uid, string deleteFile, SopInstanceProcessorSopType sopType)
		{
		    Platform.CheckForNullReference(file, "file");

            try
            {
                CheckDataLength(file);

                _instanceStats.ProcessTime.Start();
                ProcessingResult result = new ProcessingResult
                                              {
                                                  Status = ProcessingStatus.Success
                                              };

                using (ServerCommandProcessor processor = new ServerCommandProcessor("Process File"))
                {
                    SopInstanceProcessorContext processingContext = new SopInstanceProcessorContext(processor,
                                                                                      _context.StorageLocation, group);

                    if (EnforceNameRules)
                    {
                        _patientNameRules.Apply(file);
                    }

                    if (compare && ShouldReconcile(_context.StorageLocation, file))
                    {
                        ScheduleReconcile(processingContext, file, uid);
                        result.Status = ProcessingStatus.Reconciled;
                    }
                    else
                    {
                        InsertInstance(file, stream, uid, deleteFile,sopType);
                        result.Status = ProcessingStatus.Success;
                    }
                }

                _instanceStats.ProcessTime.End();

                if (_context.SopProcessedRulesEngine.Statistics.LoadTime.IsSet)
                    _instanceStats.SopRulesLoadTime.Add(_context.SopProcessedRulesEngine.Statistics.LoadTime);

                if (_context.SopProcessedRulesEngine.Statistics.ExecutionTime.IsSet)
                    _instanceStats.SopEngineExecutionTime.Add(_context.SopProcessedRulesEngine.Statistics.ExecutionTime);

                _context.SopProcessedRulesEngine.Statistics.Reset();

                //TODO: Should throw exception if result is failed?
                return result;

            }
            catch (Exception e)
            {
                // If its a duplicate, ignore the exception, and just throw it
                if (deleteFile != null && (e is InstanceAlreadyExistsException
                        || e.InnerException is InstanceAlreadyExistsException))
                    throw;

                if (uid != null)
                    FailUid(uid, retry);
                throw;
            }
		}

	    #endregion

		#region Private Methods

        /// <summary>
        /// Checks the data in the message and generates warning logs/alerts if
        /// any of them exceeeds the max size allowed in the database
        /// </summary>
        /// <param name="file"></param>
        private void CheckDataLength(DicomMessageBase file)
        {
            //TODO: Maybe this should be part of the model?

            String studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            
            String patientId = file.DataSet[DicomTags.PatientId].GetString(0, String.Empty);
            String issuerOfPatientId = file.DataSet[DicomTags.IssuerOfPatientId].GetString(0, String.Empty);
            String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
            String patientsBirthDate = file.DataSet[DicomTags.PatientsBirthDate].GetString(0, String.Empty);
            String patientsSex = file.DataSet[DicomTags.PatientsSex].GetString(0, String.Empty);
            String accessionNumber = file.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty);

            bool alert = false;
            if (!string.IsNullOrEmpty(patientId) && patientId.Length>64)
            {
                alert = true;
                Platform.Log(LogLevel.Warn, "Patient ID ({0}) in the dicom message exceeeds 64 characters", patientId);
            }

            if (!string.IsNullOrEmpty(issuerOfPatientId) && issuerOfPatientId.Length > 64)
            {
                alert = true; 
                Platform.Log(LogLevel.Warn, "Issuer Of Patient ID ({0}) in the dicom message exceeeds 64 characters", issuerOfPatientId);
            }
            if (!string.IsNullOrEmpty(patientsName) && patientsName.Length > 64)
            {
                alert = true; 
                Platform.Log(LogLevel.Warn, "Patient's Name ({0}) in the dicom message exceeeds 64 characters", patientsName);
            }
            if (!string.IsNullOrEmpty(patientsBirthDate) && patientsBirthDate.Length > 8)
            {
                alert = true;
                Platform.Log(LogLevel.Warn, "Patient's Birth Date ({0}) in the dicom message exceeeds 8 characters", patientsBirthDate);
            }
            if (!string.IsNullOrEmpty(patientsSex) && patientsSex.Length > 2)
            {
                alert = true; 
                Platform.Log(LogLevel.Warn, "Patient's Sex ({0}) in the dicom message exceeeds 2 characters", patientsSex);
            }
            if (!string.IsNullOrEmpty(accessionNumber) && accessionNumber.Length > 16)
            {
                alert = true; 
                Platform.Log(LogLevel.Warn, "Accession Number ({0}) in the dicom message exceeeds 16 characters", accessionNumber);
            }

            if (alert)
            {
                StudyAlertGenerator.Generate(
                    new StudyAlert("Study Process", _context.Partition.AeTitle, studyInstanceUid, StudyAlertType.BadDicomData, 
                        String.Format("Study {0} contains some bad data which may have been truncated. It may not appear when queried by remote devices.", 
                        studyInstanceUid)));
            }
        }


		public static void FailUid(WorkQueueUid sop, bool retry)
		{
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueUidEntityBroker uidUpdateBroker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();
				if (!retry)
					columns.Failed = true;
				else
				{
					if (sop.FailureCount >= ImageServerCommonConfiguration.WorkQueueMaxFailureCount)
					{
						columns.Failed = true;
					}
					else
					{
						columns.FailureCount = ++sop.FailureCount;
					}
				}

				uidUpdateBroker.Update(sop.GetKey(), columns);
				updateContext.Commit();
			}
		}

		/// <summary>
		/// Returns a value indicating whether the Dicom image must be reconciled.
		/// </summary>
		/// <param name="storageLocation"></param>
		/// <param name="message">The Dicom message</param>
		/// <returns></returns>
		private bool ShouldReconcile(StudyStorageLocation storageLocation, DicomMessageBase message)
		{
			Platform.CheckForNullReference(_context, "_context");
			Platform.CheckForNullReference(message, "message");

			if (_context.Study == null)
			{
				// the study doesn't exist in the database
				return false;
			}

		    StudyComparer comparer = new StudyComparer();
            DifferenceCollection list = comparer.Compare(message, storageLocation.Study, storageLocation.ServerPartition.GetComparisonOptions());

		    if (list != null && list.Count > 0)
		    {
		        LogDifferences(message, list);
		        return true;
		    }
		    return false;
		}

		private static void LogDifferences(DicomMessageBase message, DifferenceCollection list)
		{
			string sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Found {0} issue(s) in SOP {1}\n", list.Count, sopInstanceUid);
			sb.Append(list.ToString());
			Platform.Log(LogLevel.Warn, sb.ToString());
		}

		/// <summary>
		/// Schedules a reconciliation for the specified <see cref="DicomFile"/>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="file"></param>
		/// <param name="uid"></param>
		private static void ScheduleReconcile(SopInstanceProcessorContext context, DicomFile file, WorkQueueUid uid)
		{
			ImageReconciler reconciler = new ImageReconciler(context);
			reconciler.ScheduleReconcile(file, StudyIntegrityReasonEnum.InconsistentData, uid);
		}

       
		private void InsertInstance(DicomFile file, StudyXml stream, WorkQueueUid uid, string deleteFile, SopInstanceProcessorSopType sopType)
		{
			using (var processor = new ServerCommandProcessor("Processing WorkQueue DICOM file"))
			{
			    EventsHelper.Fire(OnInsertingSop, this, new SopInsertingEventArgs {Processor = processor });

				InsertInstanceCommand insertInstanceCommand = null;
				InsertStudyXmlCommand insertStudyXmlCommand = null;

				String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
				_modality = file.DataSet[DicomTags.Modality].GetString(0, String.Empty);

				if (_context.UpdateCommands.Count > 0)
				{
					foreach (BaseImageLevelUpdateCommand command in _context.UpdateCommands)
					{
						command.File = file;
						processor.AddCommand(command);
					}
				}
				try
				{
					// Create a context for applying actions from the rules engine
					ServerActionContext context =
						new ServerActionContext(file, _context.StorageLocation.FilesystemKey, _context.Partition, _context.StorageLocation.Key);
					context.CommandProcessor = processor;

					_context.SopCompressionRulesEngine.Execute(context);
                    String seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                    String sopUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                    String finalDest = _context.StorageLocation.GetSopInstancePath(seriesUid, sopUid);

					if (_context.UpdateCommands.Count > 0)
					{
						processor.AddCommand(new SaveDicomFileCommand(_context.StorageLocation, file, file.Filename != finalDest));
					}
					else if (file.Filename != finalDest || processor.CommandCount > 0)
                    {
						// Have to be careful here about failure on exists vs. not failing on exists
						// because of the different use cases of the importer.
                        // save the file in the study folder, or if its been compressed
						processor.AddCommand(new SaveDicomFileCommand(finalDest, file, file.Filename != finalDest));
                    }

					// Update the StudyStream object
					insertStudyXmlCommand = new InsertStudyXmlCommand(file, stream, _context.StorageLocation);
					processor.AddCommand(insertStudyXmlCommand);

					// Have the rules applied during the command processor, and add the objects.
					processor.AddCommand(new ApplySopRulesCommand(context,_context.SopProcessedRulesEngine));

					// If specified, delete the file
					if (deleteFile != null)
						processor.AddCommand(new FileDeleteCommand(deleteFile, true));

					// Insert into the database, but only if its not a duplicate so the counts don't get off
					insertInstanceCommand = new InsertInstanceCommand(file, _context.StorageLocation);
					processor.AddCommand(insertInstanceCommand);
					
					// Do a check if the StudyStatus value should be changed in the StorageLocation.  This
					// should only occur if the object has been compressed in the previous steps.
					processor.AddCommand(new UpdateStudyStatusCommand(_context.StorageLocation, file));

					if (uid!=null)
						processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

					// Do the actual processing
					if (!processor.Execute())
					{
						Platform.Log(LogLevel.Error, "Failure processing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
						Platform.Log(LogLevel.Error, "File that failed processing: {0}", file.Filename);
						throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid, processor.FailureException);
					}
					Platform.Log(ServerPlatform.InstanceLogLevel, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);

					// Fire NewSopEventArgs or UpdateSopEventArgs Event
					// Know its a duplicate if we have to delete the duplicate object
					if (sopType == SopInstanceProcessorSopType.NewSop)
						EventManager.FireEvent(this, new NewSopEventArgs { File = file, ServerPartitionEntry = _context.Partition, WorkQueueUidEntry = uid, WorkQueueEntry = _context.WorkQueueEntry, FileLength = InstanceStats.FileSize });
					else if (sopType == SopInstanceProcessorSopType.UpdatedSop)
						EventManager.FireEvent(this, new UpdateSopEventArgs {File = file,ServerPartitionEntry = _context.Partition,WorkQueueUidEntry = uid, WorkQueueEntry = _context.WorkQueueEntry, FileLength = InstanceStats.FileSize});
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
					             processor.Description);
					processor.Rollback();
					if (sopType == SopInstanceProcessorSopType.NewSop)
						EventManager.FireEvent(this, new FailedNewSopEventArgs { File = file, ServerPartitionEntry = _context.Partition, WorkQueueUidEntry = uid, WorkQueueEntry = _context.WorkQueueEntry, FileLength = InstanceStats.FileSize, FailureMessage = e.Message });
					else
						EventManager.FireEvent(this, new FailedUpdateSopEventArgs { File = file, ServerPartitionEntry = _context.Partition, WorkQueueUidEntry = uid, WorkQueueEntry = _context.WorkQueueEntry, FileLength = InstanceStats.FileSize, FailureMessage = e.Message });
					throw new ApplicationException("Unexpected exception when processing file.", e);
				}
				finally
				{
					if (insertInstanceCommand != null && insertInstanceCommand.Statistics.IsSet)
						_instanceStats.InsertDBTime.Add(insertInstanceCommand.Statistics);
					if (insertStudyXmlCommand != null && insertStudyXmlCommand.Statistics.IsSet)
						_instanceStats.InsertStreamTime.Add(insertStudyXmlCommand.Statistics);
				}
			}
		}
		#endregion
	}

    public class ProcessingResult
    {
        public ProcessingStatus Status { get; set; }
    }

    public enum ProcessingStatus
    {
        Success,
        Reconciled
    }
}