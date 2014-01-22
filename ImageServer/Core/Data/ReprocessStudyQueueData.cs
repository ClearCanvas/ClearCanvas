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
using ClearCanvas.Common.Serialization;
using ClearCanvas.ImageServer.Common.WorkQueue;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
    /// <summary>
    /// Represents the data in the <see cref="WorkQueueTypeEnum.ReprocessStudy"/> <see cref="WorkQueue"/> item
    /// </summary>
    [WorkQueueDataType("4424D7DB-8120-4C10-ABB8-B22BFC984152")]
    public class ReprocessStudyQueueData : WorkQueueData
    {
        #region Private Fields

        #endregion

        #region Public Properties

        /// <summary>
        /// Represents the state of the processing (used by the service only)
        /// </summary>
        public ReprocessStudyState State { get; set; }

        /// <summary>
        /// Represents the changelog which will end up in the Study History record.
        /// Filled in by the process which requests the study to be reprocessed
        /// </summary>
        public ReprocessStudyChangeLog ChangeLog { get; set; }

        /// <summary>
        /// (Optional) Additional files which the reprocess request initiator fill in to request
        /// the service to include when the study is reprocessed
        /// </summary>
        public List<string> AdditionalFiles { get; set; }

        #endregion
    }

    [WorkQueueDataType("AE4AE732-CC0B-4605-9C15-4AAEC98A0CC2")]
    public class ReprocessStudyState : DataContractBase
    {
        #region Private Fields

        #endregion

        #region Public Properties

        public bool ExecuteAtLeastOnce { get; set; }

        public bool Completed { get; set; }

        public int CompleteAttemptCount { get; set; }

        #endregion

    }
}