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
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    public partial class DataAccessGroupPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                InitDataAccessGroupList();
            
        }

        public ServerPartition Partition
        {    
            set
            {
                var current = ViewState["CurrentPartition"] as ServerPartition; ;
                if (ReferenceEquals(current, value))
                    return;


                ViewState["CurrentPartition"] = value;
                if (value==null)
                {
                    // adding a new partition, clear the list
                    DataAccessGroupCheckBoxList.Items.Clear();
                    InitDataAccessGroupList();
                }
                else
                {
                    // key == null when new partition is added (OK is pressed)
                    if (value.Key == null)
                    {
                        // if the current partition was not the same one then refresh the list
                        if (current!=null && current.Key!=null)
                        {
                            DataAccessGroupCheckBoxList.Items.Clear();
                            InitDataAccessGroupList();
                        }
                    }
                    else
                    {
                        // Refresh if no partition is assigned or new one was assigned or
                        // a different one was assigned
                        if (current == null || current.Key==null || !current.Key.Equals(value.Key))
                        {
                            DataAccessGroupCheckBoxList.Items.Clear();
                            InitDataAccessGroupList();
                        }
                    }
                    
                        
                }
                
                
                
            }
            get { return ViewState["CurrentPartition"] as ServerPartition; }
        }

        private void InitDataAccessGroupList()
        {
            if (DataAccessGroupCheckBoxList.Items.Count != 0)
                return;

            if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup))
            {
                var list = LoadDataAccessGroupInfo();

                DataAccessGroupCheckBoxList.Items.Clear();
                var listItems = CollectionUtils.Map<DataAccessGroupInfo, ListItem>(list, DataAccessGroupListItemConverter.Convert);
                DataAccessGroupCheckBoxList.Items.AddRange(listItems.ToArray());

                Legends.Visible = list.ContainsGroupWithAllPartitionAccess || list.ContainsGroupWithAllStudiesAccess;
            }
        }

        private static bool HasToken(List<AuthorityTokenSummary> tokens, string token)
        {
            return tokens.Exists(t => t.Name.Equals(token, StringComparison.InvariantCultureIgnoreCase));
        }


        private DataAccessGroupInfoCollection LoadDataAccessGroupInfo()
        {
            using (AuthorityManagement service = new AuthorityManagement())
            {
                var dataGroups = service.ListDataAccessAuthorityGroupDetails();

                // Include those that are not data access groups but have access to all partitions
                var accessToAllPartitionGroups = CollectionUtils.Select(service.ListAllAuthorityGroupDetails(),
                            g => HasToken(g.AuthorityTokens, ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions));

                var combinedGroups = new List<AuthorityGroupDetail>();
                combinedGroups.AddRange(dataGroups);
                foreach(var g in accessToAllPartitionGroups){
                    if (combinedGroups.Find(item=>item.AuthorityGroupRef.Equals(g.AuthorityGroupRef, true))==null)
                    {
                        combinedGroups.Add(g);
                    }
                }

                //convert to DataAccessGroupInfo for sorting
                var list = new DataAccessGroupInfoCollection(CollectionUtils.Map<AuthorityGroupDetail, DataAccessGroupInfo>(combinedGroups,
                    (group) =>
                    {

                        var authorityRecordRef = group.AuthorityGroupRef.ToString(false, false);
                        var fullServerPartitionAccess = HasToken(group.AuthorityTokens, ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions);
                        var allStudiesAccess = HasToken(group.AuthorityTokens, ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies);
                        return new DataAccessGroupInfo(authorityRecordRef, group.Name)
                        {
                            Description = group.Description,
                            HasAccessToCurrentPartition = fullServerPartitionAccess || (Partition != null && Partition.Key != null && Partition.IsAuthorityGroupAllowed(authorityRecordRef)),
                            CanAccessAllPartitions = fullServerPartitionAccess,
                            CanAccessAllStudies = allStudiesAccess
                        };
                    }));

                list.Sort(new DatagroupComparer());

                return list;
            }
        }

        public IEnumerable<string> SelectedDataAccessGroupRefs
        {
            get
            {
                foreach (ListItem item in DataAccessGroupCheckBoxList.Items)
                {
                    if (item.Selected && item.Enabled)
                        yield return item.Value;
                }

            }
        }
    }
}