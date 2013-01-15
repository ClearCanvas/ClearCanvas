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
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
	public partial class ApplicationLogGridView : GridViewPanel
	{
		#region Delegates
		public delegate void ApplicationLogDataSourceCreated(ApplicationLogDataSource theSource);
		public event ApplicationLogDataSourceCreated DataSourceCreated;
		#endregion

		#region Private Members
		private ApplicationLogDataSource _dataSource = null;
		private IList<Model.ApplicationLog> _logs;
        private Unit _height;
		#endregion

		#region Properties
		/// <summary>
		/// Gets/Sets the list of devices rendered on the screen.
		/// </summary>
		public IList<Model.ApplicationLog> ApplicationLogs
		{
			get
			{
				return _logs;
			}
			set
			{
				_logs = value;
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

		public Web.Common.WebControls.UI.GridView ApplicationLogListGrid
		{
			get { return ApplicationLogListControl; }
		}

		/// <summary>
		/// Gets/Sets the current selected device.
		/// </summary>
		public IList<Model.ApplicationLog> SelectedStudies
		{
			get
			{
				if (ApplicationLogs == null || ApplicationLogs.Count == 0)
					return null;

				int[] rows = ApplicationLogListControl.SelectedIndices;
				if (rows == null || rows.Length == 0)
					return null;

				IList<Model.ApplicationLog> studies = new List<Model.ApplicationLog>();
				for (int i = 0; i < rows.Length; i++)
				{
					if (rows[i] < ApplicationLogs.Count)
						studies.Add(ApplicationLogs[rows[i]]);
				}

				return studies;
			}
		}
		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new ApplicationLogDataSource();

					_dataSource.ApplicationLogFoundSet += delegate(IList<Model.ApplicationLog> newlist)
											{
												ApplicationLogs = newlist;
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
		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);           

            if (_height!=Unit.Empty)
                ContainerTable.Height = _height;

            if(IsPostBack || Page.IsAsync)
            {
                ApplicationLogListControl.DataSource = ApplicationLogDataSourceObject;
            }

		    TheGrid = ApplicationLogListControl;
		    
		}

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    if (ApplicationLogListControl.DataSource == null)
                    {
                        message.Message = SR.AppLogPleaseEnterSearchCriteria;
                    }
                    else
                    {
                        message.Message = SR.AppLogNotFound;
                    }
                }

            }
        }

		protected void GetApplicationLogDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
				_dataSource = new ApplicationLogDataSource();

				_dataSource.ApplicationLogFoundSet += delegate(IList<Model.ApplicationLog> newlist)
										{
											ApplicationLogs = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);
		}

		protected void DisposeApplicationLogDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
		{
			e.Cancel = true;
		}

		protected void ApplicationLogListControl_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			ApplicationLogListControl.PageIndex = e.NewPageIndex;
			ApplicationLogListControl.DataBind();
		}

        public void SetDataSource()
        {
            ApplicationLogListControl.DataSource = ApplicationLogDataSourceObject;
        }

	}
}