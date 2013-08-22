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
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    /// <summary>
    /// <see cref="ServerDatabaseCommand"/> derived class for deleting <see cref="FilesystemQueue"/> entries for a
    /// specific study from the database.
    /// </summary>
    public class DeleteFilesystemQueueCommand : ServerDatabaseCommand
    {
        private readonly ServerEntityKey _storageLocationKey;
    	private readonly ServerRuleApplyTimeEnum _applyTime;

		public DeleteFilesystemQueueCommand(ServerEntityKey storageLocationKey, ServerRuleApplyTimeEnum applyTime)
			: base("Delete FilesystemQueue")
		{
			_storageLocationKey = storageLocationKey;
			_applyTime = applyTime;
		}

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            var filesystemQueueBroker= updateContext.GetBroker<IFilesystemQueueEntityBroker>();
            var criteria = new FilesystemQueueSelectCriteria();
            criteria.StudyStorageKey.EqualTo(_storageLocationKey);
            IList<FilesystemQueue> filesystemQueueItems = filesystemQueueBroker.Find(criteria);

            var workQueueBroker = updateContext.GetBroker<IWorkQueueEntityBroker>();
            var workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(_storageLocationKey);
			workQueueCriteria.WorkQueueTypeEnum.In(new[] { WorkQueueTypeEnum.PurgeStudy, WorkQueueTypeEnum.DeleteStudy, WorkQueueTypeEnum.CompressStudy, WorkQueueTypeEnum.MigrateStudy });
            IList<WorkQueue> workQueueItems = workQueueBroker.Find(workQueueCriteria);

            foreach (FilesystemQueue queue in filesystemQueueItems)
            {
            	bool delete = false;
				if (_applyTime.Equals(ServerRuleApplyTimeEnum.StudyArchived))
				{
					if (queue.FilesystemQueueTypeEnum.Equals(FilesystemQueueTypeEnum.PurgeStudy))
						delete = true;
				}
				else
				{
					delete = true;
				}

				if (delete)
				{
					if (!filesystemQueueBroker.Delete(queue.GetKey()))
						throw new ApplicationException("Unable to delete items in the filesystem queue");
				}
            }

			if (!_applyTime.Equals(ServerRuleApplyTimeEnum.StudyArchived))
			{
				// delete work queue
				foreach (Model.WorkQueue item in workQueueItems)
				{
					if (!item.Delete(updateContext))
						throw new ApplicationException("Unable to delete items in the work queue");
				}
			}
        }
    }
}
