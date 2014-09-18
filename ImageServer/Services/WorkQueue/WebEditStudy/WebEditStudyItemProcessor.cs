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

using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
	[StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    public class WebEditStudyItemProcessor : BaseItemProcessor
    {
        #region Private Fields

        private ServerFilesystemInfo _filesystem;
        private Study _study;
        private Patient _patient;

        #endregion

        #region Private Methods

        private void LoadAdditionalEntities()
        {
            Debug.Assert(ServerPartition != null);
            Debug.Assert(StorageLocation != null);

			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				if (_filesystem != null)
					_filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
				_study = StorageLocation.LoadStudy(context.ReadContext);
				_patient = Patient.Load(context.ReadContext, _study.PatientKey);
			}
        }

        #endregion

        #region Overriden Protected Methods
		protected override void ProcessItem(Model.WorkQueue item)
		{
			LoadAdditionalEntities();

			using (StudyEditor editor = new StudyEditor(ServerPartition, StorageLocation, _patient, Study, item))
			{
				if (!editor.Edit(item.Data.DocumentElement))
				{
					FailQueueItem(WorkQueueItem, editor.FailureReason);
					Platform.Log(LogLevel.Error, "Study Edit failed. WorkQueueKey:{0}. Reason={1}", WorkQueueItem.Key,
					             editor.FailureReason);
				}
				else
				{
					// update this to reflect any changes to the storage location, eg Study Folder
					StorageLocation = editor.NewStorageLocation;
					Platform.CheckForNullReference(StorageLocation, "StorageLocation");
					Complete();
				}
			}
		}

    	#endregion

        #region Protected Methods
        protected void Complete()
        {
			PostProcessing(WorkQueueItem, 
				WorkQueueProcessorStatus.Complete, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            Platform.Log(LogLevel.Info, "Study Edit completed. WorkQueueKey={0}", WorkQueueItem.Key);
        }

        protected override bool CanStart()
        {
            Model.WorkQueue item = WorkQueueItem;

            IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(item,
                new []
                {
                    WorkQueueTypeEnum.StudyProcess,
                    WorkQueueTypeEnum.ReconcileStudy,
					WorkQueueTypeEnum.ProcessDuplicate,
            		WorkQueueTypeEnum.CleanupDuplicate,
        	        WorkQueueTypeEnum.CleanupStudy
                }, null);

            if (! (relatedItems == null || relatedItems.Count == 0))
            {
                PostponeItem("Study is being processed or reconciled.");
            	return false;
            }

			_filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
        	Platform.CheckForNullReference(_filesystem, "filesystem");
			if (_filesystem.Full)
			{
				PostponeItem("The Filesystem is full.");
				return false;
			}

			if (!_filesystem.Writeable)
			{
				PostponeItem("The Filesystem is not writeable.");
				return false;
			}
			
        	return true;
        }

        #endregion
    }
}
