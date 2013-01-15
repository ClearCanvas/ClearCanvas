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
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    // Note, although this is specified as a Shred, we don't actual use it as a Shred
    // due to wanting it to be in the same app domain as the WorkItemActivityMonitorService
    internal class WorkItemProcessorExtension : QueueProcessorShred
    {
        public override string GetDisplayName()
        {
            return SR.WorkItemProcessorService;
        }

        public override string GetDescription()
        {
            return SR.WorkItemProcessorDescription;
        }

        protected override IList<QueueProcessor> GetProcessors()
        {
            WorkItemProcessor.CreateProcessor(WorkItemServiceSettings.Default.StatThreadCount, WorkItemServiceSettings.Default.NormalThreadCount, GetDisplayName());
            return new List<QueueProcessor> { WorkItemProcessor.Instance };
        }
    }
}
