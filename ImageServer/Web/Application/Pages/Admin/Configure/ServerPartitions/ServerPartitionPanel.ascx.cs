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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using Resources;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.js")]
    /// <summary>
    /// Server parition panel  used in <seealso cref="ServerPartitionPage"/> web page.
    /// </summary>
    public partial class ServerPartitionPanel : AJAXScriptControl
    {
        #region Private Members

        // list of partitions displayed in the list
        private IList<ServerPartition> _partitions = new List<ServerPartition>();
        // used for database interaction
        private ServerPartitionConfigController _theController;
        private Default _enclosingPage;

        #endregion Private Members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeletePartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditPartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ServerPartitionListClientID")]
        public string ServerPartitionListClientID
        {
            get { return ServerPartitionGridPanel.TheGrid.ClientID; }
        }

        // Sets/Gets the list of partitions displayed in the panel
        public IList<ServerPartition> Partitions
        {
            get { return _partitions; }
            set
            {
                _partitions = value;
                ServerPartitionGridPanel.Partitions = _partitions;
            }
        }

        // Sets/Gets the controller used to retrieve load partitions.
        public ServerPartitionConfigController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

        public Default EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (AETitleFilter.TrimText.Length > 0 || DescriptionFilter.TrimText.Length > 0 || StatusFilter.SelectedIndex > 0)
                return true;
            else
                return false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateUI();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(SR.GridPagerPartitionSingleItem, SR.GridPagerPartitionMultipleItems, ServerPartitionGridPanel.TheGrid, delegate { return Partitions.Count; }, ImageServerConstants.GridViewPagerPosition.Top);
            ServerPartitionGridPanel.Pager = GridPagerTop;
            GridPagerTop.Reset();

            StatusFilter.Items.Add(new ListItem(SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled));

        }

        public override void DataBind()
        {
            LoadData();
            base.DataBind();
        }


        protected void Clear()
        {
            AETitleFilter.Text = string.Empty;
            DescriptionFilter.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
        }

        protected void LoadData()
        {
            var criteria = new ServerPartitionSelectCriteria();

            if (String.IsNullOrEmpty(AETitleFilter.TrimText) == false)
            {
                QueryHelper.SetGuiStringCondition(criteria.AeTitle,
                                   SearchHelper.TrailingWildCard(AETitleFilter.TrimText));
            }

            if (String.IsNullOrEmpty(DescriptionFilter.TrimText) == false)
            {
                QueryHelper.SetGuiStringCondition(criteria.Description,
                                   SearchHelper.TrailingWildCard(DescriptionFilter.TrimText));
            }

            if (StatusFilter.SelectedIndex != 0)
            {
                if (StatusFilter.SelectedIndex == 1)
                    criteria.Enabled.EqualTo(true);
                else
                    criteria.Enabled.EqualTo(false);
            }

        	criteria.AeTitle.SortAsc(0);

            Partitions =
                _theController.GetPartitions(criteria);
            ServerPartitionGridPanel.RefreshCurrentPage();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void AddPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.AddPartition();
        }

        protected void EditPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadData();
            ServerPartition selectedPartition =
                ServerPartitionGridPanel.SelectedPartition;
            
            if (selectedPartition != null)
            {
                EnclosingPage.EditPartition(selectedPartition);
            }
        }

        protected void DeletePartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            ServerPartition selectedPartition =
                ServerPartitionGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                EnclosingPage.DeletePartition(selectedPartition);
            }
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            // refresh the list
            Clear();
            LoadData();
            UpdateUI();
        }

        #endregion Protected Methods

        #region Public Methods

        public void UpdateUI()
        {
            LoadData();
            SearchUpdatePanel.Update();
            //ServerPartitionGridPanel.Refresh();
        }

        #endregion Public methods
       
    }
}