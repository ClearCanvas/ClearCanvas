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
using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using AuthorityTokens=ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens;
using Resources;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.Search)]
    public partial class Default : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
			DeleteStudyConfirmDialog.StudyDeleted += DeleteStudyConfirmDialogStudyDeleted;

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
                                                            {
                                                                SearchPanel.ServerPartition = partition;
                                                                SearchPanel.Reset();
                                                            };
            SetPageTitle(Titles.StudiesPageTitle);

            ServerPartitionSelector.SetUpdatePanel(PageContent);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ServerPartitionSelector.IsEmpty)
            {
                SearchPanel.Visible = false;
            }
            else
            {
                SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;
                SearchPanel.DeleteButtonClicked += SearchPanelDeleteButtonClicked;
                SearchPanel.AssignAuthorityGroupsButtonClicked += SearchPanelAssignAuthorityGroupsButtonClicked;
            }
            
        }

        private void DeleteStudyConfirmDialogStudyDeleted(object sender, DeleteStudyConfirmDialogStudyDeletedEventArgs e)
        {
            SearchPanel.Refresh();
        }

        private void SearchPanelDeleteButtonClicked(object sender, SearchPanelButtonClickedEventArgs e)
        {
            var list = new List<StudySummary>();
            list.AddRange(e.SelectedStudies);
            ShowDeletedDialog(list);
        }

        protected void ShowDeletedDialog(IList<StudySummary> studyList)
        {
            DeleteStudyConfirmDialog.Initialize(CollectionUtils.Map(
                studyList,
                delegate(StudySummary study)
                {
                    var info = new DeleteStudyInfo
                                   {
                                       StudyKey = study.Key,
                                       ServerPartitionAE = study.ThePartition.AeTitle,
                                       AccessionNumber = study.AccessionNumber,
                                       Modalities = study.ModalitiesInStudy,
                                       PatientId = study.PatientId,
                                       PatientsName = study.PatientsName,
                                       StudyDate = study.StudyDate,
                                       StudyDescription = study.StudyDescription,
                                       StudyInstanceUid = study.StudyInstanceUid
                                   };
                    return info;
                }
                ));
            DeleteStudyConfirmDialog.Show();
        }

        private void SearchPanelAssignAuthorityGroupsButtonClicked(object sender, SearchPanelButtonClickedEventArgs e)
        {
            var list = new List<StudySummary>();
            list.AddRange(e.SelectedStudies);
            ShowAddAuthorityGroupDialog(list);
        }

        protected void ShowAddAuthorityGroupDialog(IList<StudySummary> studyList)
        {
            AddAuthorityGroupsDialog.Initialize(studyList);
            AddAuthorityGroupsDialog.Show();
        }
    }
}
