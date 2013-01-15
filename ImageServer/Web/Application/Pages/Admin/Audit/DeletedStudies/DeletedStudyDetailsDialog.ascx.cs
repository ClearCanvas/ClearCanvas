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
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    /// <summary>
    /// View model for <see cref="DeletedStudyDetailsDialog"/>
    /// </summary>
    internal class DeletedStudyDetailsDialogViewModel
    {
        public DeletedStudyInfo DeletedStudyRecord { get; set; }
    }

    public partial class DeletedStudyDetailsDialog : UserControl
    {
        #region Private Fields

        #endregion

        #region Internal Properties

        internal DeletedStudyDetailsDialogViewModel ViewModel { get; set; }

        #endregion

        #region Public Methods

        public void Show()
        {
            DialogContent.ViewModel = ViewModel;
            DataBind();
            ModalDialog.Show();
        }

        #endregion

        #region Protected Methods

        protected void CloseClicked(object sender, ImageClickEventArgs e)
        {
            ModalDialog.Hide();
        }

        #endregion
    }
}