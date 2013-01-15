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

using System.Web.UI;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue;
using ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    public partial class AlertHoverPopupDetails : System.Web.UI.UserControl
    {
        #region Private Members
        private AlertSummary _alert; 
        #endregion
        
        #region Public Properties
        public AlertSummary Alert
        {
            get { return _alert; }
            set { _alert = value; }
        } 
        #endregion

        public override void DataBind()
        {
            if (Alert!=null && Alert.ContextData!=null)
            {
                IAlertPopupView popupView = null;
                
                if (Alert.ContextData is WorkQueueAlertContextData)
                {
                    popupView = Page.LoadControl("~/Pages/Admin/Alerts/WorkQueueAlertContextDataView.ascx") as IAlertPopupView;
                }
                if (Alert.ContextData is StudyAlertContextInfo)
                {
                    popupView = Page.LoadControl("~/Pages/Admin/Alerts/StudyAlertContextInfoView.ascx") as IAlertPopupView;
                }

                if (popupView != null)
                {
                    popupView.SetAlert(Alert);
                    DetailsPlaceHolder.Controls.Add(popupView as UserControl);
                }
            }
            base.DataBind();
        }
    }
}