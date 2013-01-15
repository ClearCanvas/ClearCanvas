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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks
{
    //
    //  Used to display the list of services.
    //
    public partial class ServiceLockGridView : GridViewPanel
    {
        #region private members
        private ServiceLockCollection _services;
        private Unit _height;
        FileSystemsConfigurationController _fsController = new FileSystemsConfigurationController();
        #endregion Private members

        #region protected properties

        #endregion protected properties

        #region public properties

       /// <summary>
        /// Gets/Sets the current selected service.
        /// </summary>
        public ServiceLock SelectedServiceLock
        {
            get
            {
                if (ServiceLocks==null || ServiceLocks.Count == 0 || GridView.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView.PageIndex*GridView.PageSize + GridView.SelectedIndex;

                if (index < 0 || index > ServiceLocks.Count - 1)
                    return null;

                return ServiceLocks[index];
            }
            set
            {
                GridView.SelectedIndex = ServiceLocks.IndexOf(value);
                if (OnServiceLockSelectionChanged != null)
                    OnServiceLockSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of services rendered on the screen.
        /// </summary>
        public ServiceLockCollection ServiceLocks
        {
            get { return _services; }
            set
            {
                _services = value;
            }
        }


        /// <summary>
        /// Gets/Sets the height of service list panel.
        /// </summary>
        public Unit Height
        {
            get { return ContainerTable == null ? _height : ContainerTable.Height; }
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
        /// Defines the handler for <seealso cref="OnServiceLockSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedServiceLock"></param>
        public delegate void ServiceLockSelectedEventHandler(object sender, ServiceLock selectedServiceLock);

        /// <summary>
        /// Occurs when the selected service in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected service can change programmatically or by users selecting the service in the list.
        /// </remarks>
        public event ServiceLockSelectedEventHandler OnServiceLockSelectionChanged;

        #endregion // Events

        #region protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            TheGrid = GridView;
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeTypeColumn(e.Row);
                    CustomizeDescriptionColumn(e.Row);
                    CustomizeEnabledColumn(e.Row);
                    CustomizeLockColumn(e.Row);
                    CustomizeFilesystemColumn(e.Row);
                }
            }
        }

        protected void CustomizeTypeColumn(GridViewRow row)
        {
            Label typeLabel = row.FindControl("Type") as Label;

            ServiceLock item = row.DataItem as ServiceLock;
            if (typeLabel != null && item!=null)
            {
                typeLabel.Text = ServerEnumDescription.GetLocalizedDescription(item.ServiceLockTypeEnum);
            }
        }

        protected void CustomizeDescriptionColumn(GridViewRow row)
        {
            Label descLabel = row.FindControl("Description") as Label;

            ServiceLock item = row.DataItem as ServiceLock;
            if (descLabel != null && item != null)
            {
                descLabel.Text = ServerEnumDescription.GetLocalizedLongDescription(item.ServiceLockTypeEnum);
            }
        }

        protected void CustomizeEnabledColumn(GridViewRow row)
        {
            Image img = row.FindControl("EnabledImage") as Image;

            ServiceLock item = row.DataItem as ServiceLock;
            if (img!=null && item != null)
            {
                img.ImageUrl = item.Enabled ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }
        }
        protected void CustomizeLockColumn(GridViewRow row)
        {
            Image img = row.FindControl("LockedImage") as Image;

            ServiceLock item = row.DataItem as ServiceLock;
            if (img != null && item != null)
            {
                img.ImageUrl = item.Lock ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }
        }
        protected void CustomizeFilesystemColumn(GridViewRow row)
        {
            Label text = row.FindControl("Filesystem") as Label;

            ServiceLock item = row.DataItem as ServiceLock;
            if (text != null && item != null)
                text.Text = item.FilesystemKey == null ? "N/A" : _fsController.LoadFileSystem(item.FilesystemKey).Description;
        }

        protected void GridView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedServiceLock != null)
            {
                GridView.SelectedIndex = ServiceLocks.RowIndexOf(SelectedServiceLock, GridView);
            }
        }

        protected void GridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="ServiceLocks"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            if (ServiceLocks!=null)
            {
                GridView.DataSource = ServiceLocks.Values;
            }

            base.DataBind();
        }


        public void RefreshGridPanel()
        {
            UpdatePanel.Update();
        }

        #endregion // public methods
    }
}
