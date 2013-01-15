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
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.DataRules
{
    [PrincipalPermission(SecurityAction.Demand, Role = ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Configuration.DataAccessRules)]
    public partial class Default : BasePage
    {
        private readonly ServerRuleController _controller = new ServerRuleController();

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
                                                            {
                                                                SearchPanel.ServerPartition = partition;
                                                                SearchPanel.Reset();
                                                            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);
            SearchPanel.EnclosingPage = this;

            ConfirmDialog.Confirmed += delegate(object data)
            {
                // delete the device and reload the affected partition.
                var key = data as ServerEntityKey;

                ServerRule rule = ServerRule.Load(key);

                _controller.DeleteServerRule(rule);

                SearchPanel.Refresh();
            };


            AddEditDataRuleControl.OKClicked += delegate(ServerRule rule)
            {
                if (AddEditDataRuleControl.Mode == AddEditDataRuleDialogMode.Edit)
                {
                    // Commit the change into database
                    if (!_controller.UpdateServerRule(rule))
                    {
                        // TODO: alert user
                    }
                }
                else
                {
                    // Create new device in the database
                    ServerRule newRule = _controller.AddServerRule(rule);
                    if (newRule == null)
                    {
                        //TODO: alert user
                    }
                }

                SearchPanel.Refresh();                
            };


            SetPageTitle(Titles.DataRulesPageTitle);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ServerPartitionSelector.IsEmpty)
            {
                SearchPanel.Visible = false;
            }
            else
            {
                SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;

                if (!Page.IsPostBack)
                {
                    SearchPanel.Refresh();
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Displays a popup dialog box for users to edit a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnEditRule(ServerRule rule, ServerPartition partition)
        {
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.Edit;
            AddEditDataRuleControl.ServerRule = rule;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to edit a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnCopyRule(ServerRule rule, ServerPartition partition)
        {
            var copiedRule = new ServerRule(rule.RuleName + " (Copy)",rule.ServerPartitionKey,rule.ServerRuleTypeEnum, rule.ServerRuleApplyTimeEnum, rule.Enabled, rule.DefaultRule, rule.ExemptRule, (XmlDocument)rule.RuleXml.CloneNode(true));

            // Store a dummy entity key
            copiedRule.SetKey(new ServerEntityKey("ServerRule",Guid.NewGuid()));
 
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.Copy;
            AddEditDataRuleControl.ServerRule = copiedRule;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to delete a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnDeleteRule(ServerRule rule, ServerPartition partition)
        {
            ConfirmDialog.Message = string.Format(SR.AdminServerRules_DeleteDialog_AreYouSure, rule.RuleName, partition.AeTitle);
            ConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
            ConfirmDialog.Data = rule.GetKey();
            ConfirmDialog.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to add a new rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnAddRule(ServerRule rule, ServerPartition partition)
        {
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.New;
            AddEditDataRuleControl.ServerRule = null;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        #endregion Public Methods
    }
}
