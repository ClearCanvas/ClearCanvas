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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudyPostProcessing
{
	/// <summary>
	/// ReconcilePostProcessing processor
	/// </summary>
	/// <remarks>
	/// This functionality was replaced in the 2.0 (Server - Performance milestone) release,
	/// and will be removed in a future relase.
	/// </remarks>
	[Obsolete("This functionality is now integrated in with the Reconcile processor")]
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    class ReconcilePostProcessingProcessor : StudyProcessItemProcessor
	{
        protected override InstancePreProcessingResult PreProcessFile(Model.WorkQueueUid uid, DicomFile file)
        {
            // Return a result indicating the file has been reconciled.
            InstancePreProcessingResult result = new InstancePreProcessingResult {AutoReconciled = true};

        	return result;
        }

		protected override void ProcessItem(Model.WorkQueue item)
		{
			LoadUids(item);

			if (WorkQueueUidList.Count == 0)
			{
				// Delete the queue entry.
				PostProcessing(item,
							   WorkQueueProcessorStatus.Complete,
							   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			}
			else
				base.ProcessItem(item);
		}

		protected override void ProcessFile(Model.WorkQueueUid queueUid, DicomFile file, ClearCanvas.Dicom.Utilities.Xml.StudyXml stream, bool compare)
		{
            Platform.CheckFalse(compare, "compare");

			var processor = new SopInstanceProcessor(Context);

			var fileInfo = new FileInfo(file.Filename);
			long fileSize = fileInfo.Length;
			processor.InstanceStats.FileSize = (ulong)fileSize;
			string sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "File:" + fileInfo.Name);
			processor.InstanceStats.Description = sopInstanceUid;

			if (Study != null)
			{
				var comparer = new StudyComparer();
				DifferenceCollection list = comparer.Compare(file, Study, ServerPartition.GetComparisonOptions());
				if (list != null && list.Count > 0)
				{
					Platform.Log(LogLevel.Warn, "Dicom file contains information inconsistent with the study in the system");
				}
			}

		    string groupID = ServerHelper.GetUidGroup(file, StorageLocation.ServerPartition, WorkQueueItem.InsertTime);
			processor.ProcessFile(groupID, file, stream, false, false, null, null, SopInstanceProcessorSopType.NewSop);

			Statistics.StudyInstanceUid = StorageLocation.StudyInstanceUid;
			if (String.IsNullOrEmpty(processor.Modality) == false)
				Statistics.Modality = processor.Modality;

			// Update the statistics
			Statistics.NumInstances++;
		}
	}
}
