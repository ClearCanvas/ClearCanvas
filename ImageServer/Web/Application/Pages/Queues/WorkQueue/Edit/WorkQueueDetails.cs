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
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Base class encapsulating the detailed information of a <see cref="WorkQueue"/> item in the context of a WorkQueue details page.
    /// </summary>
    public class WorkQueueDetails
    {
        #region Private members

        #endregion Private members

        #region Public Properties

        public DateTime ScheduledDateTime { get; set; }

        public DateTime? ExpirationTime { get; set; }

        public DateTime InsertTime { get; set; }

        public int FailureCount { get; set; }

        public WorkQueueTypeEnum Type { get; set; }

        public WorkQueueStatusEnum Status { get; set; }

        public StudyDetails Study { get; set; }

        public string ServerDescription { get; set; }

        public int NumInstancesPending { get; set; }

        public int NumSeriesPending { get; set; }

        public ServerEntityKey Key { get; set; }

        public WorkQueuePriorityEnum Priority { get; set; }

        public string FailureDescription { get; set; }

        public string StorageLocationPath { get; set; }

        public string DuplicateStorageLocationPath { get; set; }

        public UpdateItem[] EditUpdateItems { get; set; }

        #endregion Public Properties
    }
}