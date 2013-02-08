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
using ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class SeriesDeleteChangeLog : System.Web.UI.UserControl
    {
        private SeriesDeletionChangeLog _changeLog;

        public SeriesDeletionChangeLog ChangeLog
        {
            get { return _changeLog; }
            set { _changeLog = value; }
        }

        public string GetReason(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason[0];
        }

        public string GetComment(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            if (reason.Length == 1) return SR.NoneSpecified;
            return reason[1];
        }

        protected string ChangeSummaryText
        {
            get
            {
                return ChangeLog.Series.Count == 1
                    ? string.Format(SR.StudyDetails_History_OneSeriesDeletedBy, ChangeLog.Series.Count, ChangeLog.UserId ?? SR.Unknown)
                    : string.Format(SR.StudyDetails_History_MultipleSeriesDeletedBy, ChangeLog.Series.Count, ChangeLog.UserId ?? SR.Unknown);
            }
        }
    }
}