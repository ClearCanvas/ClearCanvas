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
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class ServerPartitionGridView : GridViewPanel
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

            var criteria = new ServerPartitionSelectCriteria();
            criteria.AeTitle.SortAsc(0);
            Partitions = _theController.GetPartitions(criteria);

            TheGrid = PartitionGridView;

            GridPagerTop.InitializeGridPager(SR.GridPagerPartitionSingleItem, SR.GridPagerPartitionMultipleItems, TheGrid,
                                             () => Partitions.Count, ImageServerConstants.GridViewPagerPosition.Top);
            Pager = GridPagerTop;
            GridPagerTop.Reset();

            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            DataBind();
        }

		protected override void OnPreRender(EventArgs e)
		{
			foreach (GridViewRow row in TheGrid.Rows)
			{
				if (row.RowType == DataControlRowType.DataRow)
				{
					ServerPartition partition = Partitions[row.RowIndex];

					if (partition != null)
					{
						if (_theController.CanDelete(partition))
							row.Attributes.Add("candelete", "true");
					}
				}
			}
			base.OnPreRender(e);
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
            var img = ((Image) e.Row.FindControl("ActiveImage"));

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
            var img = ((Image) e.Row.FindControl("AcceptAnyDeviceImage"));

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
            var partition = row.DataItem as ServerPartition;
            if (partition == null)
                return;

            var lbl = row.FindControl("DuplicateSopDescription") as Label; // The label is added in the template
            if (lbl!=null)
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