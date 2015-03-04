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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.ProcessStudy
{
	/// <summary>
	/// Processor for <see cref="ProcessStudyRequest"/> entries.
	/// </summary>
	internal class StudyProcessProcessor : BaseItemProcessor<ProcessStudyRequest, ProcessStudyProgress>
	{
		#region Public Properties

		public Study Study { get; set; }

		#endregion

		/// <summary>
		/// Cleanup any failed items in the queue and delete the queue entry.
		/// </summary>
		public override void Delete()
		{
			LoadUids();
			var studyXml = Location.LoadStudyXml();

			foreach (WorkItemUid sop in WorkQueueUidList)
			{
				var defaultFilePath = String.IsNullOrEmpty(sop.File);
				var file = GetFilePath(sop);
				if (sop.Failed || !sop.Complete)
				{
					if (!defaultFilePath)
					{
						try
						{
							FileUtils.Delete(file);
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Unexpected exception attempting to cleanup file for Work Item {0}", Proxy.Item.Oid);
						}
					}
					else
					{
						try
						{
							// Only delete the file if its not in the study Xml file.  This should handle collisions with 
							// multiple WorkItems that may have been canceled when others succeeded.
							if (!studyXml.Contains(sop.SeriesInstanceUid, sop.SopInstanceUid))
							{
								FileUtils.Delete(file);
							}
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Unexpected exception attempting to cleanup file for Work Item {0}",
							             Proxy.Item.Oid);
						}
					}
				}
			}

			try
			{
				DirectoryUtility.DeleteIfEmpty(Location.StudyFolder);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception attempting to delete folder: {0}",
				             Location.StudyFolder);
			}

			// Now cleanup the actual WorkItemUid references
			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemUidBroker();
				var uidBroker = context.GetWorkItemUidBroker();

				var list = broker.GetWorkItemUidsForWorkItem(Proxy.Item.Oid);
				foreach (WorkItemUid sop in list)
				{
					uidBroker.Delete(sop);
				}
				context.Commit();
			}

			Proxy.Delete();
		}

		/// <summary>
		/// Main Processing routine.
		/// </summary>
		public override void Process()
		{
			bool stillProcessing = ProcessUidList();

			if (CancelPending)
			{
				Proxy.Cancel();
				return;
			}

			if (StopPending)
			{
				Proxy.Idle();
				return;
			}

			if (!stillProcessing)
			{
				bool failed = false;
				bool complete = true;
				bool filesMissing = false;
				foreach (WorkItemUid sop in WorkQueueUidList)
				{
					if (sop.Failed)
					{
						//If any items failed simply because the file doesn't exist, then fail outright.
						if (!File.Exists(GetFilePath(sop)))
							filesMissing = true;

						failed = true;
						break;
					}

					if (!sop.Complete)
					{
						complete = false;
						break;
					}
				}

				DateTime now = Platform.Time;

				if (failed)
				{
					var failureType = filesMissing ? WorkItemFailureType.Fatal : WorkItemFailureType.NonFatal;
					Proxy.Fail(failureType);

					if (Proxy.Item.Status == WorkItemStatusEnum.Failed)
					{
						var auditedInstances = new AuditedInstances();

						auditedInstances.AddInstance(Request.Patient.PatientId, Request.Patient.PatientsName, Request.Study.StudyInstanceUid);

						AuditHelper.LogImportStudies(auditedInstances,
						                             string.IsNullOrEmpty(Request.UserName)
						                             	? EventSource.CurrentProcess
						                             	: EventSource.GetUserEventSource(Request.UserName),
						                             EventResult.MajorFailure);
					}
				}
				else if (!complete)
				{
					Proxy.Idle();
				}
				else if (now > Proxy.Item.ExpirationTime)
				{
					if (Study == null)
						Study = LoadRelatedStudy();

					var ruleOptions = new RulesEngineOptions
					                  	{
					                  		ApplyDeleteActions = true,
					                  		ApplyRouteActions = true
					                  	};
					RulesEngine.Create().ApplyStudyRules(Study.ToStoreEntry(), ruleOptions);

					Proxy.Complete();

					var auditedInstances = new AuditedInstances();

					auditedInstances.AddInstance(Request.Patient.PatientId, Request.Patient.PatientsName,
					                             Request.Study.StudyInstanceUid);

					AuditHelper.LogImportStudies(auditedInstances,
					                             string.IsNullOrEmpty(Request.UserName)
					                             	? EventSource.CurrentProcess
					                             	: EventSource.GetUserEventSource(Request.UserName),
					                             EventResult.Success);
				}
				else
				{
					Proxy.Idle();
				}
			}
			else
			{
				Proxy.Idle();
			}
		}

		/// <summary>
		/// Process all of the SOP Instances associated with a <see cref="WorkItem"/> item.
		/// </summary>
		/// <returns>Number of instances that have been processed successfully.</returns>
		private bool ProcessUidList()
		{
			StudyXml studyXml = Location.LoadStudyXml();

			int successfulProcessCount = 0;
			int lastSuccessProcessCount = -1;

			bool filesStillBeingAdded = false;

			// Loop through requerying the database
			while (successfulProcessCount > lastSuccessProcessCount)
			{
				// If we're just doing a few at a time, less than the batch size, Postpone for now
				if (lastSuccessProcessCount != -1 && (successfulProcessCount - lastSuccessProcessCount) < WorkItemServiceSettings.Default.StudyProcessBatchSize)
					break;

				lastSuccessProcessCount = successfulProcessCount;

				LoadUids();

				//Keep idling as long as there's new stuff being added to process, regardless of success.
				if (Progress.TotalFilesToProcess != WorkQueueUidList.Count)
					filesStillBeingAdded = true;

				Progress.TotalFilesToProcess = WorkQueueUidList.Count;
				Proxy.UpdateProgress(true);

				int maxBatch = WorkItemServiceSettings.Default.StudyProcessBatchSize;
				var fileList = new List<WorkItemUid>(maxBatch);

				foreach (WorkItemUid sop in WorkQueueUidList)
				{
					if (sop.Failed)
						continue;
					if (sop.Complete)
						continue;

					if (CancelPending)
					{
						Platform.Log(LogLevel.Info, "Processing of study canceled: {0}",
						             Location.Study.StudyInstanceUid);
						return successfulProcessCount > 0;
					}

					if (StopPending)
					{
						Platform.Log(LogLevel.Info, "Processing of study stopped: {0}",
						             Location.Study.StudyInstanceUid);
						return successfulProcessCount > 0;
					}

					if (sop.FailureCount > 0)
					{
						// Failed SOPs we process individually
						// All others we batch
						if (fileList.Count > 0)
						{
							if (ProcessWorkQueueUids(fileList, studyXml))
								successfulProcessCount++;
							fileList = new List<WorkItemUid>();
						}

						fileList.Add(sop);

						if (ProcessWorkQueueUids(fileList, studyXml))
							successfulProcessCount++;

						fileList = new List<WorkItemUid>();
					}
					else
					{
						fileList.Add(sop);

						if (fileList.Count >= maxBatch)
						{
							// TODO (CR Jun 2012 - Med): This method indicates there is a relation between "process count" and the number
							// of SOPs processed, but successfulProcessCount is only incremented by 1 for all the SOPs processed here.
							// Will this unnecessarily slow processing down?
							// Maybe ProcessWorkQueueUids should return the number processed successfully?
							// (SW) - The inner loop through the WorkQueueUidList causes all the files that were available at the start of the processing of the WorkItem to be available.
							// I don't think this is a significant issue, but it is ugly code.  We could just increment successfulProcessCount by fileList.Count to make it consistent.
							if (ProcessWorkQueueUids(fileList, studyXml))
								successfulProcessCount++;

							fileList = new List<WorkItemUid>();
						}
					}
				}

				if (fileList.Count > 0)
				{
					if (ProcessWorkQueueUids(fileList, studyXml))
						successfulProcessCount++;
				}
			}

			int failureItems = WorkQueueUidList.Count(s => s.Failed);
			if (failureItems != Progress.NumberOfProcessingFailures)
			{
				Progress.NumberOfProcessingFailures = failureItems;
				Proxy.UpdateProgress(true);
				return true;
			}

			return successfulProcessCount > 0 || filesStillBeingAdded;
		}

		/// <summary>
		/// Process a specified <see cref="WorkItemUid"/>
		/// </summary>
		/// <param name="sops">The <see cref="WorkItemUid"/> being processed</param>
		/// <param name="studyXml">The <see cref="StudyXml"/> object for the study being processed</param>
		/// <returns>true if the <see cref="WorkItemUid"/> is successfully processed. false otherwise</returns>
		protected virtual bool ProcessWorkQueueUids(List<WorkItemUid> sops, StudyXml studyXml)
		{
			Platform.CheckForNullReference(sops, "sops");
			Platform.CheckForNullReference(studyXml, "studyXml");

			string path = null;

			try
			{
				var fileList = new List<ProcessStudyUtility.ProcessorFile>();

				foreach (var uid in sops)
				{
					path = GetFilePath(uid);
					fileList.Add(new ProcessStudyUtility.ProcessorFile(path, uid));
				}

				var processor = new ProcessStudyUtility(Location);

				processor.ProcessBatch(fileList, studyXml);

				Progress.NumberOfFilesProcessed += fileList.Count;
				//If the file was not found, it'll fail outright.
				Progress.StatusDetails = string.Empty;
				Proxy.UpdateProgress(true);
				Study = processor.StudyLocation.Study;
				return true;
			}
			catch (Exception e)
			{
				foreach (var sop in sops)
				{
					try
					{
						var updatedSop = FailWorkItemUid(sop, true);
						sop.Failed = updatedSop.Failed;
						sop.FailureCount = updatedSop.FailureCount;

						Platform.Log(LogLevel.Error, e,
						             "Unexpected exception when processing file: {0} SOP Instance: {1}",
						             path ?? string.Empty,
						             sop.SopInstanceUid);
						Progress.StatusDetails = e.InnerException != null
						                         	? String.Format("{0}:{1}", e.GetType().Name,
						                         	                e.InnerException.Message)
						                         	: String.Format("{0}:{1}", e.GetType().Name, e.Message);
						if (sop.Failed)
							Progress.NumberOfProcessingFailures++;
					}
					catch (Exception x)
					{
						Platform.Log(LogLevel.Error, "Unable to fail WorkItemUid {0}: {1}", sop.Oid, x.Message);
					}
				}

				Proxy.UpdateProgress(true);

				return false;
			}
		}

		private string GetFilePath(WorkItemUid sop)
		{
			return String.IsNullOrEmpty(sop.File)
			       	? Location.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid)
			       	: Path.Combine(Location.StudyFolder, sop.File);
		}
	}
}