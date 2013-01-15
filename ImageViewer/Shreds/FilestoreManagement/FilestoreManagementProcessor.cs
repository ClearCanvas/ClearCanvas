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
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.FilestoreManagement
{
	internal sealed class FilestoreManagementProcessor : QueueProcessor<Study>
	{
		public FilestoreManagementProcessor(int batchSize, TimeSpan sleepTime)
			: base(batchSize, sleepTime)
		{
		}

		protected override IList<Study> GetNextBatch(int batchSize)
		{
			using(var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();
				return broker.GetStudiesForDeletion(Platform.Time, batchSize);
			}
		}

		protected override void ProcessItem(Study study)
		{
			var result = EventResult.Success;
			try
			{
			    WorkItem item;
				using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
				{
					item = ProcessStudy(study.Oid, context);
					context.Commit();
				}
                if (item != null)
                        WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemsChangedEventType.Update, WorkItemDataHelper.FromWorkItem(item));

			}
			catch (Exception)
			{
				result = EventResult.MajorFailure;
				throw;
			}
			finally
			{
				var instances = new AuditedInstances();
				instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

				AuditHelper.LogDeleteStudies(AuditHelper.LocalAETitle, instances, EventSource.CurrentUser, result);
			}
		}

		private static WorkItem ProcessStudy(long studyOid, DataAccessContext context)
		{
			var studyBroker = context.GetStudyBroker();
			var study = studyBroker.GetStudy(studyOid);

			// on the off chance it was deleted by another thread/process, there's nothing to do here
			if (study.Deleted)
				return null;

			// TODO (Marmot): this code was duplicated from DeleteClient.DeleteStudy
			var now = Platform.Time;
			var item = new WorkItem
			           	{
			           		Request = new DeleteStudyRequest
			           		          	{
			           		          		Study = new WorkItemStudy(study),
			           		          		Patient = new WorkItemPatient(study)
			           		          	},
			           		Type = DeleteStudyRequest.WorkItemTypeString,
			           		Priority = WorkItemPriorityEnum.Normal,
                            ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Default.InsertDelaySeconds),
                            ProcessTime = now.AddSeconds(WorkItemServiceSettings.Default.InsertDelaySeconds),
			           		DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes),
			           		ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Default.ExpireDelaySeconds),
                            RequestedTime = now,
			           		Status = WorkItemStatusEnum.Pending,
			           		StudyInstanceUid = study.StudyInstanceUid
			           	};

			var workItemBroker = context.GetWorkItemBroker();
			workItemBroker.AddWorkItem(item);
			study.Deleted = true;
		    return item;
		}
	}
}
