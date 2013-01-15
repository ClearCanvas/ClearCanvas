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

using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.ReapplyRules
{
    internal class ReapplyRulesItemProcessor : BaseItemProcessor<ReapplyRulesRequest, ReapplyRulesProgress>
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

            Progress.IsCancelable = false;
            Proxy.UpdateProgress();

            var processor = new ReapplyRulesUtility(Request);

            processor.Initialize();


            // Reset progress, in case of retry
            Progress.TotalStudiesToProcess = processor.DatabaseStudiesToScan;
            Progress.StudiesProcessed = 0;
            Progress.Complete = false;

            Proxy.UpdateProgress();

            processor.StudyProcessedEvent += delegate(object sender, ReapplyRulesUtility.StudyEventArgs e)
                                                 {
                                                     Progress.StudiesProcessed++;
													 Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
													 Proxy.UpdateProgress();
                                                 };
            processor.Process();

            Progress.Complete = true;
            Proxy.Complete();
        }
    }
}
