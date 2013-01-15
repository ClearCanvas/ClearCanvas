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
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DeleteStudy
{
    /// <summary>
    /// Class for processing requests to delete studies.
    /// </summary>
    internal class DeleteStudyItemProcessor : BaseItemProcessor<DeleteStudyRequest, DeleteProgress>
    {        
        public override void Process()
        {
            if (CancelPending)
            {
                Proxy.Cancel();
                return;
            }
            if (StopPending)
            {
                Proxy.Postpone();
                return;
            }

            try
            {
                // Reset progress, in case of retry
                Progress.IsCancelable = false;
                Progress.ImagesDeleted = 0;
                Progress.TotalImagesToDelete = 0;

                var delete = new DeleteStudyUtility();

                delete.Initialize(Location);

                Progress.TotalImagesToDelete = delete.NumberOfStudyRelatedInstances;
                Proxy.UpdateProgress();

                delete.Process();
                Progress.ImagesDeleted = delete.NumberOfStudyRelatedInstances;

                Proxy.Complete();
            }
            catch (Exception)
            {
                Progress.IsCancelable = true;
                throw;
            }
        }
    }
}
