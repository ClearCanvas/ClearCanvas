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
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Study level detailed information panel within the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public partial class StudyStorageView : System.Web.UI.UserControl
    {
        #region Private members

        private Unit _width;
        private Study _study;

        #endregion Private members
        
        #region Public Properties

        /// <summary>
        /// Sets or gets the Study
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public override void DataBind()
        {           
            StudyStorageViewControl.DataSource = null;
            StudyController studyController = new StudyController();
            StudyStorageViewControl.DataSource = studyController.GetStudyStorageLocation(_study);    
            
            base.DataBind();
        }

        protected void StudyStorageView_DataBound(object sender, EventArgs e)
        {
            StudyStorageLocation ssl = (StudyStorageViewControl.DataItem) as StudyStorageLocation;
            if (ssl != null)
            {
                Label statusLabel = StudyStorageViewControl.FindControl("Status") as Label;
                if (statusLabel != null)
                {
                    statusLabel.Text =  ServerEnumDescription.GetLocalizedDescription(ssl.StudyStatusEnum);
                }
				Label queueStateLable = StudyStorageViewControl.FindControl("QueueState") as Label;
				if (queueStateLable != null)
				{
				    queueStateLable.Text = ServerEnumDescription.GetLocalizedDescription(ssl.QueueStudyStateEnum);
				}
				Label studyFolder = StudyStorageViewControl.FindControl("StudyFolder") as Label;
                if (studyFolder != null)
                {
                    studyFolder.Text = ssl.GetStudyPath();
                }
                Label transferSyntaxUID = StudyStorageViewControl.FindControl("TransferSyntaxUID") as Label;
                if (transferSyntaxUID != null)
                {
                    transferSyntaxUID.Text = TransferSyntax.GetTransferSyntax(ssl.TransferSyntaxUid).Name;
                }
				Label tier = StudyStorageViewControl.FindControl("Tier") as Label;
				if (tier != null)
				{
					tier.Text =  ServerEnumDescription.GetLocalizedDescription(ssl.FilesystemTierEnum);
				}

                Label studySize = StudyStorageViewControl.FindControl("StudySize") as Label;
                if (studySize!=null)
                {
                    ulong sizeInBytes = (ulong) (ssl.Study.StudySizeInKB*1024);
                    studySize.Text = ByteCountFormatter.Format(sizeInBytes);
                }
			}
        }

        #endregion Protected Methods

    }
}