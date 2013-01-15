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
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public class GridViewPanel : UserControl
    {
        private GridPager _gridPager;
        private GridView _theGrid;
        private bool _dataBindOnPreRender = false;
        
        public GridPager Pager
        {
            set { _gridPager = value; }
            get { return _gridPager;  }
        }

        public bool DataBindOnPreRender
        {
            set { _dataBindOnPreRender = value;  }
            get { return _dataBindOnPreRender;  }

        }

        public GridView TheGrid
        {
            set { _theGrid = value; }
            get { return _theGrid; }
        }

        public void Reset()
        {
            if (_gridPager != null) _gridPager.Reset();
            _theGrid.ClearSelections();
            _theGrid.PageIndex = 0;
            _theGrid.DataSource = null;
            _theGrid.DataBind();
        }

        public void Refresh()
        {
            if(_gridPager != null) _gridPager.Reset();
            _theGrid.ClearSelections();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshWithoutPagerUpdate()
        {
            _theGrid.ClearSelections();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshAndKeepSelections()
        {
            if (_gridPager != null) _gridPager.Reset();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshCurrentPage()
        {
            if(_gridPager != null) _gridPager.Reset();
            _theGrid.DataBind();
        }

        protected void DisposeDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
        if(!_theGrid.IsDataBound && _dataBindOnPreRender) _theGrid.DataBind();
        }
    }
}
