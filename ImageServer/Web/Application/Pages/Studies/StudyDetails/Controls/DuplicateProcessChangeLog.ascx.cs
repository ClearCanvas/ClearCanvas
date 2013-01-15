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
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class DuplicateProcessChangeLog : System.Web.UI.UserControl
    {
        private ProcessDuplicateChangeLog _changeLog;
        public StudyHistory HistoryRecord;

        protected ProcessDuplicateChangeLog ChangeLog
        {
            get
            {
                if (_changeLog==null)
                {
                    _changeLog = XmlUtils.Deserialize<ProcessDuplicateChangeLog>(HistoryRecord.ChangeDescription);
                }

                return _changeLog;
            }
        }

        /// <summary>
        /// </summary>
        protected String ActionDescription
        {
            get
            {
                if (ChangeLog == null)
                {
                    return SR.Unknown;
                }
                else
                {
                    switch(ChangeLog.Action)
                    {
                        case ProcessDuplicateAction.Delete:
                            return SR.StudyDetails_History_Duplicate_Delete;
                        case ProcessDuplicateAction.OverwriteAsIs:
                            return SR.StudyDetails_History_Duplicate_OverwriteAsIs;
                        case ProcessDuplicateAction.OverwriteUseDuplicates:
                            return SR.StudyDetails_History_Duplicate_OverwriteWithDuplicateData;

                        case ProcessDuplicateAction.OverwriteUseExisting:
                            return SR.StudyDetails_History_Duplicate_OverwriteWithExistingData;

                        default:
                            return ChangeLog.Action.ToString();

                    }
                }
            }
        }

        protected String ChangeLogShortDescription
        {
            get
            {
                if (ChangeLog == null)
                {
                    return SR.Unknown;
                }

                switch (ChangeLog.Action)
                {
                    case ProcessDuplicateAction.Delete:
                        return SR.StudyDetails_History_Duplicate_Delete;
                    case ProcessDuplicateAction.OverwriteAsIs:
                        return SR.StudyDetails_History_Duplicate_OverwriteAsIs;
                    case ProcessDuplicateAction.OverwriteUseDuplicates:
                        return SR.StudyDetails_History_Duplicate_OverwriteWithDuplicateData;

                    case ProcessDuplicateAction.OverwriteUseExisting:
                        return SR.StudyDetails_History_Duplicate_OverwriteWithExistingData;

                    default:
                        return ChangeLog.Action.ToString();

                }
            }
        }

    }
}