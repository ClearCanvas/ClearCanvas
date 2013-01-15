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
using System.Configuration;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    //
    //  Used to display the list of Archive Queue Items.
    //
    public partial class AlertsGridPanel : GridViewPanel
    {
        #region Delegates
        public delegate void AlertDataSourceCreated(AlertDataSource theSource);
        public event AlertDataSourceCreated DataSourceCreated;
        #endregion

        #region Private members
        // list of studies to display
        private AlertItemCollection _alertCollection;
        private Unit _height;
        private AlertDataSource _dataSource;
        #endregion Private members

        #region Public properties

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new AlertDataSource();

                    _dataSource.AlertFoundSet += delegate(IList<AlertSummary> newlist)
                                            {
                                                AlertItems = new AlertItemCollection(newlist);
                                            };
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);
                    _dataSource.SelectCount();
                }
                if (DataSourceCreated != null)
                      DataSourceCreated(_dataSource);

                return _dataSource.SelectCount();
            }
        }

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView AlertGrid
        {
            get { return AlertGridView; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<Model.Alert> SelectedItems
        {
            get
            {
                if(!AlertGridView.IsDataBound) AlertGridView.DataBind();
                
                if (AlertItems == null || AlertItems.Count == 0)
                    return null;

                int[] rows = AlertGridView.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<Alert> queueItems = new List<Model.Alert>();
                for (int i = 0; i < rows.Length; i++)
                {
                    if (rows[i] < AlertItems.Count)
                    {
                        queueItems.Add(AlertItems[rows[i]].TheAlertItem);
                    }
                }

                return queueItems;
            }
        }

        /// <summary>
        /// Gets/Sets the list of Alert Items
        /// </summary>
        public AlertItemCollection AlertItems
        {
            get
            {
                return _alertCollection;
            }
            set
            {
                _alertCollection = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get
            {
                if (ContainerTable != null)
                    return ContainerTable.Height;
                else
                    return _height;
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        /// <summary>
        /// Gets/Sets a key of the selected work queue item.
        /// </summary>
        public AlertSummary SelectedAlert
        {
            get
            {
                if (SelectedAlertKey != null && AlertItems.ContainsKey(SelectedAlertKey))
                {
                    return AlertItems[SelectedAlertKey];
                }
                else
                    return null;
            }
            set
            {
                SelectedAlertKey = value.Key;
                AlertGridView.SelectedIndex = AlertItems.RowIndexOf(SelectedAlertKey, AlertGridView);
            }
        }

        #endregion

        #region protected methods

        protected ServerEntityKey SelectedAlertKey
        {
            set
            {
                ViewState["SelectedAlertKey"] = value;
            }
            get
            {
                return ViewState["SelectedAlertKey"] as ServerEntityKey;
            }
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = AlertGridView;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            AlertGridView.DataSource = AlertDataSourceObject;


        }

        protected void AlertGridView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedAlertKey != null)
            {
                AlertGridView.SelectedIndex = AlertItems.RowIndexOf(SelectedAlertKey, AlertGridView);
            }
        }

        protected void AlertGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (AlertGridView.EditIndex != e.Row.RowIndex)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {                   
                    AlertSummary alert = e.Row.DataItem as AlertSummary;
                    Label level = e.Row.FindControl("Level") as Label;
                   
                    if(level != null && alert != null)
                    {
                        if (alert.LevelIsErrorOrCritical)
                        {
                            level.ForeColor = Color.Red;
                        }
                        level.Text = alert.Level;

                        LinkButton appLogLink = e.Row.FindControl("AppLogLink") as LinkButton;


                        int timeRange = int.Parse(ConfigurationManager.AppSettings["AlertTimeRange"]);
                        string hostname = GetHostName(alert.Source);

                            DateTime startTime = alert.InsertTime.AddSeconds(-timeRange/2);
                            DateTime endTime = alert.InsertTime.AddSeconds(timeRange / 2);
                            appLogLink.PostBackUrl = ImageServerConstants.PageURLs.ApplicationLog + "?From=" +
                                                     HttpUtility.UrlEncode(startTime.ToString("yyyy-MM-dd") + " " +
                                                                           startTime.ToString("HH:mm:ss")) + "&To=" +
                                                     HttpUtility.UrlEncode(endTime.ToString("yyyy-MM-dd") + " " +
                                                                           endTime.ToString("HH:mm:ss")) +
                                                     "&HostName=" + hostname;

                    }

                    if (alert.ContextData!=null)
                    {
                        AlertHoverPopupDetails ctrl =
                       Page.LoadControl("AlertHoverPopupDetails.ascx") as AlertHoverPopupDetails;

                        ctrl.Alert = alert;

                        e.Row.FindControl("DetailsHoverPlaceHolder").Controls.Add(ctrl);
                        ctrl.DataBind();
                    }
                   
                }
            }
        }

        protected void DisposeAlertDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected void GetAlertDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new AlertDataSource();

                _dataSource.AlertFoundSet += delegate(IList<AlertSummary> newlist)
                                        {
                                            AlertItems = new AlertItemCollection(newlist);
                                        };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);

        }

        #endregion

        protected bool HasContextData(AlertSummary item)
        {
            return item.ContextData != null;
        }

        private string GetHostName(string source)
        {
            source = source.Substring(source.IndexOf("Host=") + 5);
            source = source.Substring(0, source.IndexOf("/"));
            return source;
        }
    }

}
