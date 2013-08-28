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

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    /// <summary>
    /// Server parition panel  used in <seealso cref="Default"/> web page.
    /// </summary>
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.js")]
    public partial class PartitionArchivePanel : AJAXScriptControl
    {
        #region Private Members

        // list of partitions displayed in the list
        private IList<Model.PartitionArchive> _partitionArchives = new List<Model.PartitionArchive>();
        // used for database interaction
        private PartitionArchiveConfigController _theController;

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
        public string RestoreButtonClientID
        {
            get { return EditPartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("PartitionArchiveListClientID")]
        public string PartitionArchiveListClientID
        {
            get { return PartitionArchiveGridPanel.TheGrid.ClientID; }
        }

        // Sets/Gets the list of partitions displayed in the panel
        public IList<Model.PartitionArchive> PartitionArchives
        {
            get { return _partitionArchives; }
            set
            {
                _partitionArchives = value;
                PartitionArchiveGridPanel.Partitions = _partitionArchives;
            }
        }

        // Sets/Gets the controller used to retrieve load partitions.
        public PartitionArchiveConfigController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            int archiveSelectedIndex = ArchiveTypeFilter.SelectedIndex;

            ArchiveTypeFilter.Items.Clear();
            ArchiveTypeFilter.Items.Add(new ListItem(SR.All));
            foreach (ArchiveTypeEnum archiveTypeEnum in ArchiveTypeEnum.GetAll())
            {
                ArchiveTypeFilter.Items.Add(
                    new ListItem(ServerEnumDescription.GetLocalizedDescription(archiveTypeEnum), archiveTypeEnum.Lookup));
            }
            ArchiveTypeFilter.SelectedIndex = archiveSelectedIndex;

            int statusSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem(SR.All, SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled, SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled, SR.Disabled));
            StatusFilter.SelectedIndex = statusSelectedIndex;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Refresh();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new PartitionArchiveConfigController();

            GridPagerTop.InitializeGridPager(SR.GridPagerPartitionSingleItem, SR.GridPagerPartitionMultipleItems, PartitionArchiveGridPanel.TheGrid,
                                             () => PartitionArchives.Count, ImageServerConstants.GridViewPagerPosition.Top);
            PartitionArchiveGridPanel.Pager = GridPagerTop;

        }

        public override void DataBind()
        {
            LoadData();
            base.DataBind();
        }

        protected void LoadData()
        {
            var criteria = new PartitionArchiveSelectCriteria();

            if (String.IsNullOrEmpty(DescriptionFilter.TrimText) == false)
            {
                QueryHelper.SetGuiStringCondition(criteria.Description,
                                                   SearchHelper.TrailingWildCard(DescriptionFilter.TrimText));
            }

            if (StatusFilter.SelectedIndex > 0)
            {
                criteria.Enabled.EqualTo(StatusFilter.SelectedIndex == 1);
            }

        	criteria.ServerPartitionKey.EqualTo(ServerPartition.Key);

            PartitionArchives =
                _theController.GetPartitions(criteria);
            PartitionArchiveGridPanel.RefreshCurrentPage();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            DataBind();
        }

        protected void AddPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
           ((Default)Page).AddPartition(ServerPartition);
        }

        protected void EditPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).EditPartition(selectedPartition);
            }
        }

        protected void DeletePartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).DeletePartition(selectedPartition);
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public void Refresh()
        {
            LoadData();
            PartitionArchiveGridPanel.UpdateUI();

            SearchUpdatePanel.Update();
        }

        internal void Reset()
        {
            //Clear();
            PartitionArchiveGridPanel.Reset();
        }
        #endregion Public methods       
    }
}