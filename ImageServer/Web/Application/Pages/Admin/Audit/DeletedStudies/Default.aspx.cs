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
using System.Security.Permissions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using Resources;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.StudyDeleteHistory.Search)]
    public partial class Default : BaseAdminPage
    {
        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            SearchPanel.ViewDetailsClicked += SearchPanel_ViewDetailsClicked;
            SearchPanel.DeleteClicked += SearchPanel_DeleteClicked;
            DeleteConfirmMessageBox.Confirmed += DeleteConfirmMessageBox_Confirmed;
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetPageTitle(Titles.DeletedStudiesPageTitle);

            if (Page.IsPostBack)
            {
            	// Reload the data on post-back
            	// Note: databinding also happens on initial rendering because the grid pager 
            	// does so on Page_Load.
                DataBind();
            }
        }

        #endregion

        #region Private Methods

        private void DeleteConfirmMessageBox_Confirmed(object data)
        {
            try
            {
                var record = data as ServerEntityKey;
                var controller = new DeletedStudyController();
                controller.Delete(record);
            }
            finally
            {
                SearchPanel.Refresh();
            }
        }

        private void SearchPanel_DeleteClicked(object sender, DeletedStudyDeleteClickedEventArgs e)
        {
            DeleteConfirmMessageBox.Data = e.SelectedItem.DeleteStudyRecord;
            DeleteConfirmMessageBox.Show();
        }

        private void SearchPanel_ViewDetailsClicked(object sender, DeletedStudyViewDetailsClickedEventArgs e)
        {
            var dialogViewModel = new DeletedStudyDetailsDialogViewModel {DeletedStudyRecord = e.DeletedStudyInfo};
            DetailsDialog.ViewModel = dialogViewModel;
            DetailsDialog.Show();
        }

        #endregion
    }
}