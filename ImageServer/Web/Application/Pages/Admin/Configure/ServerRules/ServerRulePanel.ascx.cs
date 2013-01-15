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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using Resources;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel.js",
        "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
{
    [ClientScriptResource(
        ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel",
        ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel.js")]
    public partial class ServerRulePanel : AJAXScriptControl
    {
        private readonly ServerRuleController _controller = new ServerRuleController();

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteServerRuleButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditServerRuleButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ServerRuleListClientID")]
        public string ServerRuleListClientID
        {
            get { return ServerRuleGridViewControl.TheGrid.ClientID; }
        }

        public ServerPartition ServerPartition { get; set; }

        public Default EnclosingPage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void LoadRules()
        {
            var criteria = new ServerRuleSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(ServerPartition.GetKey());

            if (!String.IsNullOrEmpty(RuleApplyTimeDropDownList.Text))
            {
                if (!RuleApplyTimeDropDownList.SelectedValue.Equals("ALL"))
                {
                    ServerRuleApplyTimeEnum en =
                        ServerRuleApplyTimeEnum.GetEnum(RuleApplyTimeDropDownList.SelectedItem.Value);
                    criteria.ServerRuleApplyTimeEnum.EqualTo(en);
                }
            }
            if (!String.IsNullOrEmpty(RuleTypeDropDownList.Text))
            {
                if (!RuleTypeDropDownList.SelectedValue.Equals("ALL"))
                {
                    ServerRuleTypeEnum en = ServerRuleTypeEnum.GetEnum(RuleTypeDropDownList.SelectedItem.Value);
                    criteria.ServerRuleTypeEnum.EqualTo(en);
                }
                else
                    criteria.ServerRuleTypeEnum.NotEqualTo(ServerRuleTypeEnum.DataAccess);
            }
            else
                criteria.ServerRuleTypeEnum.NotEqualTo(ServerRuleTypeEnum.DataAccess);

            if (StatusFilter.SelectedIndex != 0)
            {
                criteria.Enabled.EqualTo(StatusFilter.SelectedIndex == 1);
            }

            if (DefaultFilter.SelectedIndex != 0)
            {
                criteria.DefaultRule.EqualTo(DefaultFilter.SelectedIndex == 1);
            }

            ServerRuleGridViewControl.ServerRules = _controller.GetServerRules(criteria);
            ServerRuleGridViewControl.DataBind();
        }

        /// <summary>
        /// Updates the rules list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadRules()"/> instead.
        /// </remarks>
        public void Refresh()
        {
            LoadRules();

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            SearchUpdatePanel.Update();
        }

        internal void Reset()
        {
            Clear();
            ServerRuleGridViewControl.Reset();
        }

        public override void DataBind()
        {
            LoadRules();
            base.DataBind();
        }

        public void OnRowSelected(int index)
        {
        }

        #endregion Public Methods

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            ServerRuleGridViewControl.ServerRulePanel = this;

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerServerRulesSingleItem,
                                             SR.GridPagerServerRulesMultipleItems,
                                             ServerRuleGridViewControl.TheGrid,
                                             () => ServerRuleGridViewControl.ServerRules == null
                                                       ? 0
                                                       : ServerRuleGridViewControl.ServerRules.Count,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            ServerRuleGridViewControl.Pager = GridPagerTop;
            GridPagerTop.Reset();


            int prevSelectIndex = RuleApplyTimeDropDownList.SelectedIndex;
            RuleApplyTimeDropDownList.Items.Clear();
            RuleApplyTimeDropDownList.Items.Add(new ListItem(SR.All, "ALL"));
            foreach (ServerRuleApplyTimeEnum applyTimeEnum in ServerRuleApplyTimeEnum.GetAll())
            {
                RuleApplyTimeDropDownList.Items.Add(
                    new ListItem(ServerEnumDescription.GetLocalizedDescription( applyTimeEnum), applyTimeEnum.Lookup));
            }
            RuleApplyTimeDropDownList.SelectedIndex = prevSelectIndex;


            prevSelectIndex = RuleTypeDropDownList.SelectedIndex;
            RuleTypeDropDownList.Items.Clear();
            RuleTypeDropDownList.Items.Add(new ListItem(SR.All, "ALL"));
            foreach (ServerRuleTypeEnum typeEnum in ServerRuleTypeEnum.GetAll())
            {
                RuleTypeDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(typeEnum), typeEnum.Lookup));
            }
            RuleTypeDropDownList.SelectedIndex = prevSelectIndex;

            if (Page.IsPostBack)
                DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StatusFilter.Items.Add(new ListItem(SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled));

            DefaultFilter.Items.Add(new ListItem(SR.All));
            DefaultFilter.Items.Add(new ListItem(SR.Default));
            DefaultFilter.Items.Add(new ListItem(SR.NotDefault));
        }

        protected override void OnPreRender(EventArgs e)
        {
            ServerRule rule = ServerRuleGridViewControl.SelectedRule;
            if (rule == null)
            {
                // no rule being selected

                EditServerRuleButton.Enabled = false;

                DeleteServerRuleButton.Enabled = false;
            }
            else
            {
                EditServerRuleButton.Enabled = true;

                DeleteServerRuleButton.Enabled = !rule.DefaultRule;
            }

            base.OnPreRender(e);
            Refresh();
        }

        protected void AddServerRuleButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddRule(null, ServerPartition);
        }

        protected void EditServerRuleButton_Click(object sender, ImageClickEventArgs e)
        {
            if (ServerRuleGridViewControl.SelectedRule != null)
                EnclosingPage.OnEditRule(ServerRuleGridViewControl.SelectedRule, ServerPartition);
        }

        protected void DeleteServerRuleButton_Click(object sender, ImageClickEventArgs e)
        {
            if (ServerRuleGridViewControl.SelectedRule != null)
                EnclosingPage.OnDeleteRule(ServerRuleGridViewControl.SelectedRule, ServerPartition);
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            RuleApplyTimeDropDownList.SelectedIndex = 0;
            RuleTypeDropDownList.SelectedIndex = 0;
            StatusFilter.SelectedIndex = 0;
            DefaultFilter.SelectedIndex = 0;

            LoadRules();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            DataBind();
        }

        private void Clear()
        {
            RuleTypeDropDownList.SelectedIndex = 0;
            RuleApplyTimeDropDownList.SelectedIndex = 0;
            StatusFilter.SelectedIndex = 0;
            DefaultFilter.SelectedIndex = 0;
        }
        #endregion Protected Methods
    }
}