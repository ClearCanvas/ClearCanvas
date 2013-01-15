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

using System.Xml.Serialization;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using System.IO;
using ClearCanvas.Common;
using System;
using NUnit.Framework;
using System.Text;
using ClearCanvas.ImageServer.Core.ModelExtensions;

namespace ClearCanvas.ImageServer.Core.Data
{
    /// <summary>
    /// Represents the data in the <see cref="WorkQueueTypeEnum.ReprocessStudy"/> <see cref="WorkQueue"/> item
    /// </summary>
    public class ReprocessStudyQueueData
    {
        #region Private Fields

        private ReprocessStudyState _state;
        private ReprocessStudyChangeLog _changeLog;

        #endregion

        #region Public Properties

        /// <summary>
        /// Represents the state of the processing (used by the service only)
        /// </summary>
        public ReprocessStudyState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Represents the changelog which will end up in the Study History record.
        /// Filled in by the process which requests the study to be reprocessed
        /// </summary>
        public ReprocessStudyChangeLog ChangeLog
        {
            get { return _changeLog; }
            set { _changeLog = value; }
        }

        /// <summary>
        /// (Optional) Additional files which the reprocess request initiator fill in to request
        /// the service to include when the study is reprocessed
        /// </summary>
        public List<string> AdditionalFiles { get; set; }

        #endregion
    }

    public class ReprocessStudyState
    {
        #region Private Fields
        
        private bool _executeAtLeastOnce;
        private bool _completed;
        private int _completeAttemptCount;

        #endregion

        #region Public Properties

        public bool ExecuteAtLeastOnce
        {
            get { return _executeAtLeastOnce; }
            set { _executeAtLeastOnce = value; }
        }

        public bool Completed
        {
            get { return _completed; }
            set { _completed = value; }
        }

        public int CompleteAttemptCount
        {
            get { return _completeAttemptCount; }
            set { _completeAttemptCount = value; }
        }

        #endregion

    }
}