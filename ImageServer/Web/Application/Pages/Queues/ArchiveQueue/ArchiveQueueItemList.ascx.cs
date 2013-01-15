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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue
{
    //
    //  Used to display the list of Archive Queue Items.
    //
    public partial class ArchiveQueueItemList : GridViewPanel
    {
		#region Delegates
		public delegate void ArchiveQueueDataSourceCreated(ArchiveQueueDataSource theSource);
		public event ArchiveQueueDataSourceCreated DataSourceCreated;
		#endregion

        #region Private members
        // list of studies to display
        private IList<ArchiveQueueSummary> _queueItems;
        private Unit _height;
    	private ArchiveQueueDataSource _dataSource;
        #endregion Private members

        #region Public properties

		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new ArchiveQueueDataSource();

					_dataSource.ArchiveQueueFoundSet += delegate(IList<ArchiveQueueSummary> newlist)
											{
												Items = newlist;
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
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public Web.Common.WebControls.UI.GridView ArchiveQueueGrid
        {
            get { return ArchiveQueueGridView; }
        }


        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<Model.ArchiveQueue> SelectedItems
        {
            get
            {
                if (!ArchiveQueueGridView.IsDataBound) ArchiveQueueGridView.DataBind();

                if (Items==null || Items.Count == 0)
                    return null;

                int[] rows = ArchiveQueueGridView.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

				IList<Model.ArchiveQueue> queueItems = new List<Model.ArchiveQueue>();
                for(int i=0; i<rows.Length; i++)
                {
                    if (rows[i] < Items.Count)
                    {
                        queueItems.Add(Items[rows[i]].TheArchiveQueueItem);
                    }
                }

                return queueItems;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<ArchiveQueueSummary> Items
        {
            get
            {
                return _queueItems;
            }
            set
            {
                _queueItems = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get {
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

        #endregion

        #region Events
        /// <summary>
		/// Defines the handler for <seealso cref="OnQueueItemSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedStudies"></param>
        public delegate void QueueItemSelectedEventHandler(object sender, IList<Model.ArchiveQueue> selectedStudies);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event QueueItemSelectedEventHandler OnQueueItemSelectionChanged;

        #endregion // Events
        
        #region protected methods
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = ArchiveQueueGridView;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            ArchiveQueueGridView.SelectedIndexChanged += ArchiveQueueGridView_SelectedIndexChanged;

            if (IsPostBack || Page.IsAsync)
            {
                TheGrid.DataSource = ArchiveQueueDataSourceObject;
            } 
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

			if (Items == null)
				return;

            foreach (GridViewRow row in ArchiveQueueGridView.Rows)
            {
                if (row.RowType==DataControlRowType.DataRow)
                {
					ArchiveQueueSummary item = Items[row.RowIndex];
                    
                    if (item!=null)
                    {
                        row.Attributes.Add("instanceuid", item.StudyStorage.StudyInstanceUid);
                        row.Attributes.Add("serverae", item.ThePartition.AeTitle);
                    	row.Attributes.Add("canreset",item.TheArchiveQueueItem.ArchiveQueueStatusEnum.Equals(ArchiveQueueStatusEnum.Failed).ToString().ToLower());
                    }
                }
            }
        }


        protected void ArchiveQueueGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<Model.ArchiveQueue> queueItems = SelectedItems;
            if (queueItems != null)
                if (OnQueueItemSelectionChanged != null)
                    OnQueueItemSelectionChanged(this, queueItems);            
        }

        protected void ArchiveQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            ArchiveQueueGrid.DataBind();
        }

        protected void ArchiveQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ArchiveQueueGridView.PageIndex = e.NewPageIndex;
            ArchiveQueueGrid.DataBind();
        }

		protected void GetArchiveQueueDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
				_dataSource = new ArchiveQueueDataSource();

				_dataSource.ArchiveQueueFoundSet += delegate(IList<ArchiveQueueSummary> newlist)
										{
											Items = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);

		}

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    if (ArchiveQueueGrid.DataSource == null)
                    {
                        message.Message = Labels.ArchiveQueue_EnterSearchCriteria;
                    }
                    else
                    {
                        message.Message = Labels.ArchiveQueue_NoResults;
                    }
                }

            }
        }

		public void SetDataSource()
		{
			ArchiveQueueGridView.DataSource = ArchiveQueueDataSourceObject;
		}

        #endregion

    }

}
