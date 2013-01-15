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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyStateAlertPanel : System.Web.UI.UserControl
    {
        private StudySummary _studySummary;

        /// <summary>
        /// Message displayed
        /// </summary>
        protected Label Message;

        public StudySummary Study
        {
            get { return _studySummary; }
            set { _studySummary = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            Visible = false;
            Message.Text = String.Empty;
            base.OnInit(e);
        }

        public override void DataBind()
        {
            if (_studySummary!=null)
            {
                if (_studySummary.IsProcessing)
                {
                    ShowAlert(SR.StudyBeingProcessed);
                }
                else if (_studySummary.IsLocked)
                {
                    ShowAlert(ServerEnumDescription.GetLocalizedLongDescription(_studySummary.QueueStudyStateEnum));
                }
                else if (_studySummary.IsNearline)
                {
                    ShowAlert(SR.StudyIsNearline);
                }
                else if (_studySummary.IsReconcileRequired)
                {
                    ShowAlert(SR.StudyRequiresReconcilie);
                }
                else if (_studySummary.HasPendingExternalEdit)
                {
                    ShowAlert(SR.StudyScheduledForEdit);
                }
            }

            base.DataBind();
        }

        private void ShowAlert(string message)
        {
            Message.Text = message;
            Visible = true;
        }
    }
}