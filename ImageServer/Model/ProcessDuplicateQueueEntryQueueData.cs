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

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents the state of the work queue processing.
    /// </summary>
    public class ProcessDuplicateQueueState
    {
        public bool ExistingStudyUpdated { get; set; }

        public bool HistoryLogged { get; set; }
        
    }

    /// <summary>
    /// Represents the contents in the Data column of the <see cref="WorkQueue"/> entry.
    /// </summary>
    public class ProcessDuplicateQueueEntryQueueData
    {

        public ProcessDuplicateQueueEntryQueueData()
        {
            State = new ProcessDuplicateQueueState();
        }

        public ProcessDuplicateAction Action { get; set; }

        public string DuplicateSopFolder { get; set; }

        public ProcessDuplicateQueueState State { get; set; }

        public string UserName { get; set; }
    }
}