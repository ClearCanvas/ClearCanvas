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
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class ServerPartitionGridPanel : GridViewPanel
    {
        #region Private Members

        /// <summary>
        /// list of partitions rendered on the screen.
        /// </summary>
        private IList<ServerPartition> _partitions;
        private Unit _height;
		private readonly ServerPartitionConfigController _theController = new ServerPartitionConfigController();
        #endregion private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the list of partitions rendered on the screen.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            get { return _partitions; }
            set
            {
                _partitions = value;
                PartitionGridView.DataSource = _partitions;
            }
        }

        /// <summary>
        /// Retrieve the current selected partition.
        /// </summary>
        public ServerPartition SelectedPartition
        {
            get
            {
                TheGrid.DataBind();
                int index = TheGrid.PageIndex*TheGrid.PageSize + TheGrid.SelectedIndex;

                if (index < 0 || index >= Partitions.Count)
                    return null;

                return Partitions[index];
            }
        }

        /// <summary>
        /// Gets/Sets the height of server partition list panel.
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

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = PartitionGridView;

            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

        }

        protected void PartitionGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (PartitionGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeActiveColumn(e);
                    CustomizeAcceptAnyDeviceColumn(e);
                    CustomizeDuplicateSopPolicyColumn(e.Row);
                    CustomizePartitionStorageConfiguration(e.Row);                    

                    var partition = e.Row.DataItem as ServerPartition;

                    if (partition != null)
                    {
                        if (_theController.CanDelete(partition))
                            e.Row.Attributes.Add("candelete", "true");
                    }
                }
            }
        }

        private void CustomizePartitionStorageConfiguration(GridViewRow row)
        {
            ServerPartition partition = row.DataItem as ServerPartition;
            Label lbl = row.FindControl("PartitionStorageConfigurationLabel") as Label; // The label is added in the template
            if (lbl != null)
            {
                if (partition.HasEnabledDeleteRules)
                    lbl.Text = Resources.SR.PartitionStorageConfiguration_DeleteRuleExists;
                else
                {
                    lbl.Text = partition.ArchiveExists
                            ? Resources.SR.PartitionStorageConfiguration_ArchiveConfiguredNoDeleteRule
                            : Resources.SR.PartitionStorageConfiguration_NoArchiveConfiguredNoDeleteRule;
                }
            }            
        }

        protected void CustomizeActiveColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("ActiveImage"));

            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                if (active)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        protected void CustomizeAcceptAnyDeviceColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("AcceptAnyDeviceImage"));

            if (img != null)
            {
                bool acceptAnyDevice = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AcceptAnyDevice"));
                if (acceptAnyDevice)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        private void CustomizeDuplicateSopPolicyColumn(GridViewRow row)
        {
            ServerPartition partition = row.DataItem as ServerPartition;
            Label lbl = row.FindControl("DuplicateSopDescription") as Label; // The label is added in the template
            lbl.Text = ServerEnumDescription.GetLocalizedDescription(partition.DuplicateSopPolicyEnum);
        }

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            Refresh();
        }

        #endregion Public methods
    }
}