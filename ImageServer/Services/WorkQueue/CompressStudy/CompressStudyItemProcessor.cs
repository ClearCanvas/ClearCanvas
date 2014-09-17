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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.StudyHistory;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Events;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;
using SaveDicomFileCommand = ClearCanvas.ImageServer.Core.Command.SaveDicomFileCommand;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CompressStudy
{

	[StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
	public class CompressStudyItemProcessor : BaseItemProcessor, ICancelable
	{
		#region Private Members

		private CompressInstanceStatistics _instanceStats;
		private CompressStudyStatistics _studyStats;

		#endregion

		#region Protected Properties

		/// <summary>
		/// Gets the new transfer syntax of the study after the compression.
		/// </summary>
		protected TransferSyntax CompressTransferSyntax { get; private set; }

		#endregion

		protected override bool Initialize(Model.WorkQueue item, out string failureDescription)
		{
			if (!base.Initialize(item, out failureDescription))
				return false;

			XmlElement element = WorkQueueItem.Data.DocumentElement;
			string syntax = element.Attributes["syntax"].Value;
			CompressTransferSyntax = TransferSyntax.GetTransferSyntax(syntax);
			return true;
		}

		#region Private Methods

		private bool ProcessWorkQueueUid(Model.WorkQueue item, WorkQueueUid sop, StudyXml studyXml,
		                                 IDicomCodecFactory theCodecFactory)
		{
			Platform.CheckForNullReference(item, "item");
			Platform.CheckForNullReference(sop, "sop");
			Platform.CheckForNullReference(studyXml, "studyXml");

			if (!studyXml.Contains(sop.SeriesInstanceUid, sop.SopInstanceUid))
			{
				// Uid was inserted but not in the study xml.
				// Auto-recovery might have detect problem with that file and remove it from the study.
				// Assume the study xml has been corrected and ignore the uid.
				Platform.Log(LogLevel.Warn, "Skipping SOP {0} in series {1}. It is no longer part of the study.", sop.SopInstanceUid,
				             sop.SeriesInstanceUid);

				// Delete it out of the queue
				DeleteWorkQueueUid(sop);
				return true;
			}

			string basePath = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
			basePath = Path.Combine(basePath, sop.SopInstanceUid);
			string path;
			if (sop.Extension != null)
				path = basePath + "." + sop.Extension;
			else
				path = basePath + ServerPlatform.DicomFileExtension;

			try
			{
				ProcessFile(item, sop, path, studyXml, theCodecFactory);

				// WorkQueueUid has been deleted out by the processor

				return true;
			}
			catch (Exception e)
			{
				if (e.InnerException != null && e.InnerException is DicomCodecUnsupportedSopException)
				{
					Platform.Log(LogLevel.Warn, e, "Instance not supported for compressor: {0}.  Deleting WorkQueue entry for SOP {1}",
					             e.Message, sop.SopInstanceUid);

					item.FailureDescription = e.InnerException != null ? e.InnerException.Message : e.Message;

					// Delete it out of the queue
					DeleteWorkQueueUid(sop);

					return false;
				}
				Platform.Log(LogLevel.Error, e, "Unexpected exception when compressing file: {0} SOP Instance: {1}", path,
				             sop.SopInstanceUid);
				item.FailureDescription = e.InnerException != null ? e.InnerException.Message : e.Message;

				sop.FailureCount++;

				UpdateWorkQueueUid(sop);

				return false;
			}
		}

		private void ReinsertFilesystemQueue(TimeSpan delay)
		{
			using (
				IUpdateContext updateContext =
					PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueUidEntityBroker broker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidSelectCriteria workQueueUidCriteria = new WorkQueueUidSelectCriteria();
				workQueueUidCriteria.WorkQueueKey.EqualTo(WorkQueueItem.Key);
				broker.Delete(workQueueUidCriteria);

				FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters();
				parms.FilesystemQueueTypeEnum = CompressTransferSyntax.LosslessCompressed
					                                ? FilesystemQueueTypeEnum.LosslessCompress
					                                : FilesystemQueueTypeEnum.LossyCompress;
				parms.ScheduledTime = Platform.Time + delay;
				parms.StudyStorageKey = WorkQueueItem.StudyStorageKey;
				parms.FilesystemKey = StorageLocation.FilesystemKey;

				parms.QueueXml = WorkQueueItem.Data;

				IInsertFilesystemQueue insertQueue = updateContext.GetBroker<IInsertFilesystemQueue>();

				if (false == insertQueue.Execute(parms))
				{
					Platform.Log(LogLevel.Error, "Unexpected failure inserting FilesystemQueue entry");
				}
				else
					updateContext.Commit();
			}
		}

		#endregion

		/// <summary>
		/// Process all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
		/// </summary>
		/// <param name="item">The <see cref="WorkQueue"/> item.</param>
		/// <returns>A value indicating whether the uid list has been successfully processed</returns>
		/// <param name="theCodecFactory">The factor for doing the compression</param>
		protected bool ProcessUidList(Model.WorkQueue item, IDicomCodecFactory theCodecFactory)
		{
			StudyXml studyXml = LoadStudyXml(StorageLocation);

			int successfulProcessCount = 0;
			int totalCount = WorkQueueUidList.Count;
			foreach (WorkQueueUid sop in WorkQueueUidList)
			{
				if (sop.Failed)
					continue;

				if (CancelPending)
				{
					Platform.Log(LogLevel.Info,
					             "Received cancel request while compressing study {0}.  {1} instances successfully processed.",
					             StorageLocation.StudyInstanceUid, successfulProcessCount);

					return successfulProcessCount > 0;
				}

				if (ProcessWorkQueueUid(item, sop, studyXml, theCodecFactory))
					successfulProcessCount++;
			}

			if (successfulProcessCount > 0)
				Platform.Log(LogLevel.Info, "Completed compression of study {0}. {1} instances successfully processed.",
				             StorageLocation.StudyInstanceUid, successfulProcessCount);

			return successfulProcessCount > 0 && totalCount == successfulProcessCount;
		}

		protected void ProcessFile(Model.WorkQueue item, WorkQueueUid sop, string path, StudyXml studyXml,
		                           IDicomCodecFactory theCodecFactory)
		{
			DicomFile file = null;

			_instanceStats = new CompressInstanceStatistics();

			_instanceStats.ProcessTime.Start();

			// Use the command processor for rollback capabilities.
			using (ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue Compress DICOM File"))
			{
				string modality = String.Empty;

				try
				{
					file = new DicomFile(path);

					_instanceStats.FileLoadTime.Start();
					file.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);
					_instanceStats.FileLoadTime.End();

					modality = file.DataSet[DicomTags.Modality].GetString(0, String.Empty);

					FileInfo fileInfo = new FileInfo(path);
					_instanceStats.FileSize = (ulong) fileInfo.Length;

					// Get the Patients Name for processing purposes.
					String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

					if (file.TransferSyntax.Equals(theCodecFactory.CodecTransferSyntax))
					{
						// Delete the WorkQueueUid item
						processor.AddCommand(new DeleteWorkQueueUidCommand(sop));

						// Do the actual processing
						if (!processor.Execute())
						{
							Platform.Log(LogLevel.Warn, "Failure deleteing WorkQueueUid: {0} for SOP: {1}", processor.Description,
							             file.MediaStorageSopInstanceUid);
							Platform.Log(LogLevel.Warn, "Compression file that failed: {0}", file.Filename);
						}
						else
						{
							Platform.Log(LogLevel.Warn, "Skip compressing SOP {0}. Its current transfer syntax is {1}",
							             file.MediaStorageSopInstanceUid, file.TransferSyntax.Name);
						}
					}
					else
					{
						IDicomCodec codec = theCodecFactory.GetDicomCodec();

						// Create a context for applying actions from the rules engine
						var context = new ServerActionContext(file, StorageLocation.FilesystemKey, ServerPartition, item.StudyStorageKey,
						                                      processor);

						var parms = theCodecFactory.GetCodecParameters(item.Data);
						var compressCommand =
							new DicomCompressCommand(context.Message, theCodecFactory.CodecTransferSyntax, codec, parms);
						processor.AddCommand(compressCommand);

						var save = new SaveDicomFileCommand(file.Filename, file, false);
						processor.AddCommand(save);

						// Update the StudyStream object, must be done after compression
						// and after the compressed image has been successfully saved
						var insertStudyXmlCommand = new UpdateStudyXmlCommand(file, studyXml, StorageLocation);
						processor.AddCommand(insertStudyXmlCommand);

						// Delete the WorkQueueUid item
						processor.AddCommand(new DeleteWorkQueueUidCommand(sop));

						// Do the actual processing
						if (!processor.Execute())
						{
							EventManager.FireEvent(this,
							                       new FailedUpdateSopEventArgs
								                       {
									                       File = file,
									                       ServerPartitionEntry = context.ServerPartition,
									                       WorkQueueUidEntry = sop,
									                       WorkQueueEntry = WorkQueueItem,
									                       FileLength = (ulong) insertStudyXmlCommand.FileSize,
									                       FailureMessage = processor.FailureReason
								                       });

							_instanceStats.CompressTime.Add(compressCommand.CompressTime);
							Platform.Log(LogLevel.Error, "Failure compressing command {0} for SOP: {1}", processor.Description,
							             file.MediaStorageSopInstanceUid);
							Platform.Log(LogLevel.Error, "Compression file that failed: {0}", file.Filename);
							throw new ApplicationException(
								"Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " +
								file.MediaStorageSopInstanceUid, processor.FailureException);
						}
						_instanceStats.CompressTime.Add(compressCommand.CompressTime);
						Platform.Log(ServerPlatform.InstanceLogLevel, "Compress SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid,
						             patientsName);

						EventManager.FireEvent(this,
						                       new UpdateSopEventArgs
							                       {
								                       File = file,
								                       ServerPartitionEntry = context.ServerPartition,
								                       WorkQueueUidEntry = sop,
								                       WorkQueueEntry = WorkQueueItem,
								                       FileLength = (ulong) insertStudyXmlCommand.FileSize
							                       });
					}

				}
				catch (Exception e)
				{
					EventManager.FireEvent(this,
					                       new FailedUpdateSopEventArgs
						                       {
							                       File = file,
							                       ServerPartitionEntry = ServerPartition,
							                       WorkQueueUidEntry = sop,
							                       WorkQueueEntry = WorkQueueItem,
							                       FileLength = (ulong) new FileInfo(path).Length,
							                       FailureMessage = processor.FailureReason
						                       });

					Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
					             processor.Description);
					processor.Rollback();

					throw;
				}
				finally
				{
					_instanceStats.ProcessTime.End();
					_studyStats.AddSubStats(_instanceStats);

					_studyStats.StudyInstanceUid = StorageLocation.StudyInstanceUid;
					if (String.IsNullOrEmpty(modality) == false)
						_studyStats.Modality = modality;

					// Update the statistics
					_studyStats.NumInstances++;
				}
			}
		}

		protected override void ProcessItem(Model.WorkQueue item)
		{
			LoadUids(item);

			if (WorkQueueUidList.Count == 0)
			{
				// No UIDs associated with the WorkQueue item.  Set the status back to idle
				PostProcessing(item,
				               WorkQueueProcessorStatus.Idle,
				               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				return;
			}


			XmlElement element = item.Data.DocumentElement;

			string syntax = element.Attributes["syntax"].Value;

			TransferSyntax compressSyntax = TransferSyntax.GetTransferSyntax(syntax);
			if (compressSyntax == null)
			{
				item.FailureDescription =
					String.Format("Invalid transfer syntax in compression WorkQueue item: {0}", element.Attributes["syntax"].Value);
				Platform.Log(LogLevel.Error, "Error with work queue item {0}: {1}", item.GetKey(), item.FailureDescription);
				base.PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
				return;
			}

			if (Study == null)
			{
				item.FailureDescription =
					String.Format("Compression item does not have a linked Study record");
				Platform.Log(LogLevel.Error, "Error with work queue item {0}: {1}", item.GetKey(), item.FailureDescription);
				base.PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
				return;
			}

			Platform.Log(LogLevel.Info,
			             "Compressing study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4} to {5}",
			             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
			             Study.AccessionNumber, ServerPartition.Description, compressSyntax.Name);

			IDicomCodecFactory[] codecs = DicomCodecRegistry.GetCodecFactories();
			IDicomCodecFactory theCodecFactory = null;
			foreach (IDicomCodecFactory codec in codecs)
				if (codec.CodecTransferSyntax.Equals(compressSyntax))
				{
					theCodecFactory = codec;
					break;
				}

			if (theCodecFactory == null)
			{
				item.FailureDescription = String.Format("Unable to find codec for compression: {0}", compressSyntax.Name);
				Platform.Log(LogLevel.Error, "Error with work queue item {0}: {1}", item.GetKey(), item.FailureDescription);
				base.PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
				return;
			}

			if (!ProcessUidList(item, theCodecFactory))
				PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal);
			else
			{
				Platform.Log(LogLevel.Info,
				             "Completed Compressing study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4} to {5}",
				             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
				             Study.AccessionNumber, ServerPartition.Description, compressSyntax.Name);

				// Save a StudyHistory Record
				SaveStudyHistory(compressSyntax.UidString);

				if (compressSyntax.LossyCompressed)
					UpdateStudyStatus(StorageLocation, StudyStatusEnum.OnlineLossy, compressSyntax);
				else
					UpdateStudyStatus(StorageLocation, StudyStatusEnum.OnlineLossless, compressSyntax);

				PostProcessing(item,
				               WorkQueueProcessorStatus.Pending,
				               WorkQueueProcessorDatabaseUpdate.None); // batch processed, not complete
			}
		}

		protected override void OnProcessItemEnd(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");
			base.OnProcessItemEnd(item);

			_studyStats.UidsLoadTime.Add(UidsLoadTime);
			_studyStats.StorageLocationLoadTime.Add(StorageLocationLoadTime);
			_studyStats.StudyXmlLoadTime.Add(StudyXmlLoadTime);
			_studyStats.DBUpdateTime.Add(DBUpdateTime);

			if (_studyStats.NumInstances > 0)
			{
				_studyStats.CalculateAverage();
				StatisticsLogger.Log(LogLevel.Info, false, _studyStats);
			}
		}

		protected override void OnProcessItemBegin(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");

			_studyStats = new CompressStudyStatistics
				{
					Description = String.Format("{0}", item.WorkQueueTypeEnum)
				};
		}

		protected override bool CanStart()
		{

			IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem,
			                                                                new[]
				                                                                {
					                                                                WorkQueueTypeEnum.StudyProcess,
					                                                                WorkQueueTypeEnum.ReconcileStudy,
					                                                                WorkQueueTypeEnum.WebEditStudy,
					                                                                WorkQueueTypeEnum.ReprocessStudy,
					                                                                WorkQueueTypeEnum.ReconcilePostProcess,
					                                                                WorkQueueTypeEnum.ReconcileCleanup,
					                                                                WorkQueueTypeEnum.ProcessDuplicate
				                                                                }, null);

			TimeSpan delay;

			if (relatedItems == null || relatedItems.Count == 0)
			{
				// Don't compress lossy if the study needs to be reconciled.
				// It's time wasting if we go ahead with the compression because later on
				// users will have to restore the study in order to reconcile the images in SIQ.
				if (CompressTransferSyntax.LossyCompressed && StorageLocation.IsReconcileRequired)
				{
					Platform.Log(LogLevel.Info,
					             "Study {0} cannot be compressed to lossy at this time because of pending reconciliation. Reinserting into FilesystemQueue",
					             StorageLocation.StudyInstanceUid);
					delay = TimeSpan.FromMinutes(60);
					ReinsertFilesystemQueue(delay);
					PostProcessing(WorkQueueItem, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
					return false;
				}
				return true;
			}
			Platform.Log(LogLevel.Info,
			             "Compression entry for study {0} has existing WorkQueue entry, reinserting into FilesystemQueue",
			             StorageLocation.StudyInstanceUid);
			delay = TimeSpan.FromMinutes(60);
			ReinsertFilesystemQueue(delay);
			PostProcessing(WorkQueueItem, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			return false;
		}

		private void SaveStudyHistory(string compressSyntax)
		{
			using (
				IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				var history = StudyHistoryHelper.CreateStudyHistoryRecord(ctx, StorageLocation, StorageLocation,
				                                                          StudyHistoryTypeEnum.StudyCompress, null,
				                                                          new CompressionStudyHistory
					                                                          {
						                                                          TimeStamp = Platform.Time,
						                                                          FinalTransferSyntaxUid = compressSyntax,
						                                                          OriginalTransferSyntaxUid =
							                                                          StorageLocation.TransferSyntaxUid
					                                                          });
				if (history != null)
					ctx.Commit();
			}
		}
	}
}