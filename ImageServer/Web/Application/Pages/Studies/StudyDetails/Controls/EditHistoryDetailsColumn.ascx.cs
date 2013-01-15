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
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class EditHistoryDetailsColumn : System.Web.UI.UserControl
    {
        private StudyHistory _historyRecord;
        private WebEditStudyHistoryChangeDescription _description;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public StudyHistory HistoryRecord
        {
            set { _historyRecord = value; }
        }

        public WebEditStudyHistoryChangeDescription EditHistory
        {
            get
            {
                if (_description == null && _historyRecord != null)
                {
                    _description = XmlUtils.Deserialize<WebEditStudyHistoryChangeDescription>(_historyRecord.ChangeDescription.DocumentElement);
                }
                return _description;
            }
        }

        public string GetReason(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason.Length > 0 ? reason[0] : string.Empty;
        }

        public string GetComment(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason.Length > 1 ? reason[1] : string.Empty;
        }

        protected string ChangeSummaryText
        {
            get{
                return String.Format(SR.EditBy, EditTypeTranslator.Translate(EditHistory.EditType), EditHistory.UserId ?? SR.Unknown);
            }
        }
    }

    public static class EditTypeTranslator{
        public static string Translate(EditType type)
        {
            switch (type)
            {
                case EditType.WebEdit:
                    return HtmlUtility.Encode(SR.StudyDetails_WebEdit_Description);
                case EditType.WebServiceEdit:
                    return HtmlUtility.Encode(SR.StudyDetails_WebServiceEdit_Description);
            }

            return HtmlUtility.Encode(HtmlUtility.GetEnumInfo(type).LongDescription);
        }
    }
}