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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;
using SR = Resources.SR;
using Resources;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.js")]
    public partial class AlertsPanel : AJAXScriptControl
    {
        #region Private Members

        // used for database interaction
        private AlertController _theController = new AlertController();

        #endregion Private Members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteAlertButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteAllButtonClientID")]
        public string DeleteAllButtonClientID
        {
            get { return DeleteAllAlertsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("AlertListClientID")]
        public string AlertListClientID
        {
            get { return AlertsGridPanel.AlertGrid.ClientID; }
        }

        // Sets/Gets the controller used to retrieve load partitions.
        public AlertController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

        #endregion Public Properties

        #region Private Properties

        void DeleteAllConfirmDialog_Confirmed(object data)
        {
            AlertsGridPanel.RefreshCurrentPage();
            
            AlertController controller = new AlertController();
                    
                AlertItemCollection items = AlertsGridPanel.AlertItems;

                bool successful = false;

                for (int i = 0; i < items.Count; i++)
                {
                    successful = controller.DeleteAlert(items[i].TheAlertItem);
                    if (!successful) break;
                }
             
                if (successful)
                {
                    Platform.Log(LogLevel.Info, "All Alert items deleted by user.");
                }
                else
                {
                    Platform.Log(LogLevel.Error,
                                     "PreResetConfirmDialog_Confirmed: Unable to delete all Alert items.");

                        MessageBox.Message = ErrorMessages.AlertDeleteFailed;
                        MessageBox.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                }

                OnAllAlertsDeleted();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (ComponentFilter.TrimText.Length > 0 || InsertDateFilter.Text.Length > 0 || LevelFilter.SelectedIndex > 0 || CategoryFilter.SelectedIndex > 0)
                return true;
            else
                return false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(SR.GridPagerAlertSingleItemFound, SR.GridPagerAlertMultipleItemsFound, AlertsGridPanel.AlertGrid, delegate { return AlertsGridPanel.ResultCount; }, ImageServerConstants.GridViewPagerPosition.Top);
            AlertsGridPanel.Pager = GridPagerTop;
            GridPagerTop.Reset();

            ClearInsertDateButton.OnClientClick = ScriptHelper.ClearDate(InsertDateFilter.ClientID, InsertDateCalendarExtender.ClientID);
            
            IList<AlertLevelEnum> levelEnums = AlertLevelEnum.GetAll();
            IList<AlertCategoryEnum> categoryEnums = AlertCategoryEnum.GetAll();

            int prevSelectedIndex = LevelFilter.SelectedIndex;
            LevelFilter.Items.Clear();
            LevelFilter.Items.Add(new ListItem(SR.Any, string.Empty));
            foreach (AlertLevelEnum ale in levelEnums)
                LevelFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(ale), ale.Lookup));
            LevelFilter.SelectedIndex = prevSelectedIndex;

            prevSelectedIndex = CategoryFilter.SelectedIndex;
            CategoryFilter.Items.Clear();
            CategoryFilter.Items.Add(new ListItem(SR.Any, string.Empty));
            foreach (AlertCategoryEnum ace in categoryEnums)
                CategoryFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(ace), ace.Lookup));
            CategoryFilter.SelectedIndex = prevSelectedIndex;

            DeleteAllAlertsButton.Roles =
                AuthorityTokens.Admin.Alert.Delete;
            DeleteAlertButton.Roles =
                AuthorityTokens.Admin.Alert.Delete;

            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            AlertsGridPanel.DataSourceCreated += delegate(AlertDataSource source)
                            {
                                if (!String.IsNullOrEmpty(ComponentFilter.TrimText))
                                    source.Component = SearchHelper.LeadingAndTrailingWildCard(ComponentFilter.TrimText);
                                if (LevelFilter.SelectedIndex > 0)
                                    source.Level = AlertLevelEnum.GetEnum(LevelFilter.SelectedValue);
                                if (CategoryFilter.SelectedIndex > 0)
                                    source.Category = AlertCategoryEnum.GetEnum(CategoryFilter.SelectedValue);
                                source.DateFormats = InsertDateCalendarExtender.Format;
                                if (!String.IsNullOrEmpty(InsertDateFilter.Text))
                                    source.InsertTime = InsertDateFilter.Text;
                            };

            DeleteConfirmationBox.Confirmed += delegate(object data)
                            {
                                if (data == null) return;

                                ArrayList keys = (ArrayList) data;

                                for (int i = 0; i < keys.Count; i++ )
                                {
                                    _theController.DeleteAlertItem(keys[i] as ServerEntityKey);
                                }

                                OnAlertDeleted();
                            };

            DeleteAllConfirmationBox.Confirmed += DeleteAllConfirmDialog_Confirmed;

            // TODO: We should do this in GridPager.InitializeGridPager()
            AlertsGridPanel.TheGrid.DataBound += delegate { GridPagerTop.Refresh(); };
        }

        protected void Clear()
        {
            ComponentFilter.Text = string.Empty;
            InsertDateFilter.Text = string.Empty;
            LevelFilter.SelectedIndex = 0;
            LevelFilter.Text = string.Empty;
        }


        private void OnAllAlertsDeleted()
        {
            DataBind();
        }

        private void OnAlertDeleted()
        {
            DataBind();
        }

        
        protected void DeleteAlertButton_Click(object sender, ImageClickEventArgs e)
        {
            AlertsGridPanel.RefreshCurrentPage();

            IList<Model.Alert> items = AlertsGridPanel.SelectedItems;

            ArrayList keyArray = new ArrayList();


            if (items != null && items.Count > 0)
            {
                if (items.Count > 1) DeleteConfirmationBox.Message = string.Format(SR.MultipleAlertDelete);
                else DeleteConfirmationBox.Message = string.Format(SR.SingleAlertDelete);

                DeleteConfirmationBox.Message += "<div id='DeleteAlertTableContainer' class='DeleteAlertTableContainer'> <table class='DeleteAlertConfirmTable' border='0' cellspacing='0' cellpadding='0'>";
                DeleteConfirmationBox.Message +=
                    string.Format("<thead class='DeleteAlertFixedHeader' id='DeleteAlertFixedHeader'><tr class='GlobalGridViewHeader'><th align='left'>{0}</th><th>{1}</th><th>{2}</th><th>{3}</th></tr></thead><tbody class='DeleteAlertScrollContent'>",
                        ColumnHeaders.AlertComponent, ColumnHeaders.AlertInsertDate, ColumnHeaders.AlertLevel, ColumnHeaders.AlertContent);

                foreach (Model.Alert item in items)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    System.IO.StringWriter sw = new System.IO.StringWriter(sb);
                    System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(sw);

                    item.Content.Save(xtw);

                    DeleteConfirmationBox.Message += String.Format("<tr class='DeleteAlertConfirmTableRow' align='left'><td>{0}</td><td>{1}</td><td>{2}</td><td width='300'>{3}</td></tr>",
                                   item.Component, item.InsertTime.ToString("g"), item.AlertLevelEnum, sb);
                    keyArray.Add(item.Key);
                }
                DeleteConfirmationBox.Message += "</tbody></table></div>";

                DeleteConfirmationBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
                DeleteConfirmationBox.Data = keyArray;
                DeleteConfirmationBox.MessageStyle = "color: #ff0000; font-weight: bold;";
                DeleteConfirmationBox.Show();
            }
        }
        
        protected void DeleteAllAlertsButton_Click(object sender, ImageClickEventArgs e)
        {
            DeleteAllConfirmationBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteAllConfirmationBox.Message = SR.AlertDeleteAllConfirm;
            DeleteAllConfirmationBox.MessageStyle = "color: #ff0000; font-weight: bold;";
            DeleteAllConfirmationBox.Show();    
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
          	AlertsGridPanel.Refresh();
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            // refresh the list
            Clear();
            UpdateUI();
        }

        #endregion Protected Methods

        #region Public Methods

        public override void DataBind()
        {
            base.DataBind();

            // Whenever data is bound, we have to force the panel to refresh.
            // Otherwise, databind that occurs during postback event will not be sent back to the client.
            SearchUpdatePanel.Update();
        }


        public void UpdateUI()
        {
            AlertsGridPanel.Refresh();   
            SearchUpdatePanel.Update();

        }

        #endregion Public methods
       
    }
}