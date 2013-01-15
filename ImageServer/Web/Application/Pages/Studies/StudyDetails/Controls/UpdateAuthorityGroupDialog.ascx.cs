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
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class AddAuthorityGroupDialog : UserControl
    {
        private EventHandler<EventArgs> _authorityGroupEditedHandler;

        public event EventHandler<EventArgs> AuthorityGroupsEdited
        {
            add { _authorityGroupEditedHandler += value; }
            remove { _authorityGroupEditedHandler -= value; }
        }

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public StudySummary Study
        {
            get; set;
        }

 

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            //If the validation failed, keep everything as is, and 
            //make sure the dialog stays visible.
            if (!Page.IsValid)
            {
                ModalDialog.Show();
                return;
            }

            if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess) && Study != null)
            {
                AuthorityGroupCheckBoxList.Items.Clear();

                var controller = new StudyDataAccessController();
                var list = controller.ListDataAccessGroupsForStudy(Study.TheStudyStorage.Key);

                var adapter = new ServerPartitionDataAdapter();
                IList<AuthorityGroupDetail> accessAllStudiesList;
                var groups = adapter.GetAuthorityGroupsForPartition(Study.ThePartition.Key, true, out accessAllStudiesList);


                IList<ListItem> items = CollectionUtils.Map(
                    accessAllStudiesList,
                    delegate(AuthorityGroupDetail group)
                        {

                            var item = new ListItem(@group.Name,
                                                    @group.AuthorityGroupRef.ToString(false, false))
                                           {
                                               Enabled = false,
                                               Selected = true
                                           };
                            item.Attributes["title"] = @group.Description;
                            return item;
                        });

                foreach (var group in groups)
                {
                    var item = new ListItem(@group.Name,
                                              @group.AuthorityGroupRef.ToString(false, false));
                    item.Attributes["title"] = @group.Description;

                    foreach (AuthorityGroupStudyAccessInfo s in list)
                    {
                        if (s.AuthorityOID.Equals(group.AuthorityGroupRef.ToString(false, false)))
                            item.Selected = true;
                    }

                    items.Add(item);
                }

                AuthorityGroupCheckBoxList.Items.AddRange(CollectionUtils.ToArray(items));
            }

            CancelButton.Visible = true;
            UpdateButton.Visible = true;

            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        protected void UpdateButton_Click(object sender, ImageClickEventArgs e)
        {
            if (Page.IsValid)
            {
                var assignedGroups = new List<string>();
                foreach (ListItem item in AuthorityGroupCheckBoxList.Items)
                {
                    if (item.Selected && item.Enabled)
                        assignedGroups.Add(item.Value);
                }

                var controller = new StudyDataAccessController();
                controller.UpdateStudyAuthorityGroups(Study.StudyInstanceUid, Study.AccessionNumber, Study.TheStudyStorage.Key, assignedGroups);
                
                OnAuthorityGroupsUpdated();
                
                Close();
            }
            else
            {
                Show();
            }
        }

        protected void CancelButton_Click(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        private void OnAuthorityGroupsUpdated()
        {
            var args = new EventArgs();
            EventsHelper.Fire(_authorityGroupEditedHandler, this, args);
        }
    }
}