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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Data.Model;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class SearchResultGridView : GridViewPanel
    {
        #region Private Fields

        private DeletedStudyDataSource _dataSource;

        #endregion

        #region Public Properties

        public GridView GridViewControl
        {
            get { return ListControl; }
        }

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new DeletedStudyDataSource();
                }
                return _dataSource.SelectCount();
            }
        }

        public DeletedStudyInfo SelectedItem
        {
            get { return _dataSource.Find(ListControl.SelectedValue); }
        }

        public ObjectDataSource DataSourceContainer
        {
            get { return DataSource; }
        }

        #endregion

        #region Overriden Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DataSourceContainer.ObjectCreated += DataSourceContainer_ObjectCreated;
            ListControl.DataSource = DataSource;
            TheGrid = ListControl;
        }

        #endregion

        #region Private Methods

        private void DataSourceContainer_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            // keep a reference to the data source created, used for other purposes
            _dataSource = e.ObjectInstance as DeletedStudyDataSource;
        }

        #endregion
    }
}