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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
    /// <summary>
    /// A specialized panel that displays a list of <see cref="WorkQueue"/> entries.
    /// </summary>
    /// <remarks>
    /// <see cref="WorkQueueItemList"/> wraps around <see cref="System.Web.UI.WebControls.GridView"/> control to specifically display
    /// <see cref="WorkQueue"/> entries on a web page. The <see cref="WorkQueue"/> entries are set through <see cref="WorkQueueItems"/>. 
    /// User of this control can set the <see cref="Height"/> of the panel. The panel always expands to fit the width of the
    /// parent control. 
    /// 
    /// </remarks>
    public partial class WorkQueueItemList : GridViewPanel
	{
		#region Delegates
		public delegate void WorkQueueDataSourceCreated(WorkQueueDataSource theSource);
		public event WorkQueueDataSourceCreated DataSourceCreated;
		#endregion

		#region Private Members
		private WorkQueueItemCollection _workQueueItems;
        private Unit _height;
    	private WorkQueueDataSource _dataSource;
        private ServerPartition _serverPartition;
        #endregion Private Members

        #region Public Properties
		public int ResultCount
		{
			get
			{
                if (_dataSource == null)
                {
                    _dataSource = new WorkQueueDataSource();

                    _dataSource.WorkQueueFoundSet += delegate(IList<WorkQueueSummary> newlist)
                                            {
                                                WorkQueueItems = new WorkQueueItemCollection(newlist);
                                            };
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);
                    _dataSource.SelectCount();
                }
                if (_dataSource.ResultCount == 0)
                {
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);

                    _dataSource.SelectCount();
                }
                return _dataSource.ResultCount;
			}
		}

        /// <summary>
        /// Gets/Sets the height of the panel
        /// </summary>
        public Unit Height
        {
            set
            {
                _height = value;
                if (ListContainerTable != null)
                    ListContainerTable.Height = value;
            }
            get
            {
                if (ListContainerTable != null)
                    return ListContainerTable.Height;
                else
                    return _height;
            }
        }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        /// <summary>
        /// Gets a reference to the work queue item list <see cref="System.Web.UI.WebControls.GridView"/>
        /// </summary>
        public GridView WorkQueueItemGridView
        {
            get { return WorkQueueGridView; }
        }

        /// <summary>
        /// Gets/Sets a value indicating paging is enabled.
        /// </summary>
        public WorkQueueItemCollection WorkQueueItems
        {
            get { return _workQueueItems; }
            set { _workQueueItems = value; }
        }

        public ServerEntityKey SelectedDataKey
        {
            get
            {
                if (WorkQueueGridView.SelectedDataKeys == null) return null;
                return new ServerEntityKey("WorkQueue", new Guid(WorkQueueGridView.SelectedDataKeys[0]));
            }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = WorkQueueGridView;

            if (_height!=Unit.Empty)
                ListContainerTable.Height = _height;

            if (IsPostBack || Page.IsAsync)
            {
                WorkQueueGridView.DataSource = WorkQueueDataSourceObject;
            }


        }
      
        protected ServerEntityKey SelectedWorkQueueItemKey
        {
            set
            {
                ViewState[ "_SelectedWorkQueueItemKey"] = value;
            }
            get
            {
                return ViewState[ "_SelectedWorkQueueItemKey"] as ServerEntityKey;
            }
        }

        protected ServerEntityKey GetRowItemKey(int rowIndex)
        {
			if (WorkQueueItems == null) return null;

            string workQueueItems = "\n";
            for (int i = 0; i < WorkQueueItems.Count; i++)
            {
                workQueueItems += "[i=" + i + " ][ItemKey=" + WorkQueueItems[i].Key + "]\n";
            }

			return WorkQueueItems[rowIndex].Key;
        }

        protected void WorkQueueListView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    if (WorkQueueGridView.DataSource == null)
                    {
                        message.Message = SR.WorkQueuePleaseEnterSearchCritera;
                    }
                    else
                    {
                        message.Message = SR.NoWorkQueueFound;
                    }
                }

            }
            else
            {
                if (WorkQueueGridView.EditIndex != e.Row.RowIndex)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CustomizeColumns(e.Row);
                        CustomizeRowAttribute(e.Row);
                    }
                }
            }
        }

        private void CustomizeColumns(GridViewRow row)
        {
			WorkQueueSummary summary = row.DataItem as WorkQueueSummary;

			if (summary != null)
            {
            	PersonNameLabel nameLabel = row.FindControl("PatientName") as PersonNameLabel;
				if (nameLabel != null)
            		nameLabel.PersonName = summary.PatientsName;
            }
        }

        private void CustomizeRowAttribute(GridViewRow row)
        {
            WorkQueueSummary item = row.DataItem as WorkQueueSummary;
            row.Attributes["WorkQueueUID"] = item.Key.Key.ToString();
            row.Attributes["canreschedule"] = WorkQueueController.CanReschedule(item.TheWorkQueueItem).ToString().ToLower();
            row.Attributes["canreset"] = WorkQueueController.CanReset(item.TheWorkQueueItem).ToString().ToLower();
            row.Attributes["candelete"] = WorkQueueController.CanDelete(item.TheWorkQueueItem).ToString().ToLower();
            row.Attributes["canreprocess"] = WorkQueueController.CanReprocess(item.TheWorkQueueItem).ToString().ToLower();
        }

        protected void WorkQueueListView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedWorkQueueItemKey != null)
            {
                WorkQueueGridView.SelectedIndex = WorkQueueItems.RowIndexOf(SelectedWorkQueueItemKey, WorkQueueGridView);
            }
        }

    	protected void GetWorkQueueDataSource(object sender, ObjectDataSourceEventArgs e)
    	{
			if (_dataSource == null)
			{
				_dataSource = new WorkQueueDataSource();
			    _dataSource.Partition = ServerPartition;
				_dataSource.WorkQueueFoundSet += delegate(IList<WorkQueueSummary> newlist)
				                                 	{
				                                 		WorkQueueItems = new WorkQueueItemCollection(newlist);
				                                 	};
			}

    		e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);
    	}

		#endregion Protected Methods

		public bool SelectedItemExists()
		{
			if (SelectedDataKey == null)
				RefreshWithoutPagerUpdate();

			if (SelectedDataKey != null)
			{
				if (Model.WorkQueue.Load(SelectedDataKey) == null) return false;
				return true;
			}
			else
			{
				return false;
			}
		}

        public void SetDataSource()
        {
            WorkQueueGridView.DataSource = WorkQueueDataSourceObject;
        }
	}
}