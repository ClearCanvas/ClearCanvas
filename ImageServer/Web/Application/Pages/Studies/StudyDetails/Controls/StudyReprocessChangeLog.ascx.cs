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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyReprocessChangeLogControl : System.Web.UI.UserControl
    {
        private ReprocessStudyChangeLog _changeLog;

        public StudyHistory HistoryRecord;

        protected ReprocessStudyChangeLog ChangeLog
        {
            get
            {
                if (_changeLog == null)
                {
                    _changeLog = XmlUtils.Deserialize<ReprocessStudyChangeLog>(HistoryRecord.ChangeDescription);
                }

                return _changeLog;
            }
        }

        protected string ChangeDescription
        {
            get
            {
                if (!String.IsNullOrEmpty(ChangeLog.User))
                {
                    return String.Format(SR.StudyDetails_History_Reconcile_ReprocessedBecause, ChangeLog.Reason);
                }
                else
                {
                    return String.Format(SR.StudyDetails_History_Reconcile_ReprocessedByBecause, ChangeLog.User, ChangeLog.Reason);
                }
            }
        }

    }
}