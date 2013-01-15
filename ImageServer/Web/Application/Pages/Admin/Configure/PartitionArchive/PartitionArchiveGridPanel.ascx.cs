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
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class PartitionArchiveGridPanel : GridViewPanel
    {
        #region Private Members

        /// <summary>
        /// list of partitions rendered on the screen.
        /// </summary>
        private IList<Model.PartitionArchive> _partitions;
        private Unit _height;
		private readonly PartitionArchiveConfigController _theController = new PartitionArchiveConfigController();

        #endregion private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the list of partitions rendered on the screen.
        /// </summary>
        public IList<Model.PartitionArchive> Partitions
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
        public Model.PartitionArchive SelectedPartition
        {
            get
            {
                if (Partitions.Count == 0 || PartitionGridView.SelectedIndex < 0)
                    return null;
                
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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			foreach (GridViewRow row in TheGrid.Rows)
			{
				if (row.RowType == DataControlRowType.DataRow)
				{
					Model.PartitionArchive partition = Partitions[row.RowIndex];

					if (partition != null)
					{
						if (_theController.CanDelete(partition))
							row.Attributes.Add("candelete", "true");
					}
				}
			}
		}

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            DataBind();
        }

        protected void PartitionGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (PartitionGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Model.PartitionArchive pa = e.Row.DataItem as Model.PartitionArchive;
                    Label archiveTypeLabel = e.Row.FindControl("ArchiveType") as Label;
                    archiveTypeLabel.Text = ServerEnumDescription.GetLocalizedDescription(pa.ArchiveTypeEnum);

                    Label configXml = e.Row.FindControl("ConfigurationXML") as Label;
                    configXml.Text = XmlUtils.GetXmlDocumentAsString(pa.ConfigurationXml, true);

                    Image img = ((Image) e.Row.FindControl("EnabledImage"));
                    if (img != null)
                    {
                        img.ImageUrl = pa.Enabled
                                           ? ImageServerConstants.ImageURLs.Checked
                                           : ImageServerConstants.ImageURLs.Unchecked;
                    }

                    img = ((Image) e.Row.FindControl("ReadOnlyImage"));
                    if (img != null)
                    {
                        img.ImageUrl = pa.ReadOnly
                                           ? ImageServerConstants.ImageURLs.Checked
                                           : ImageServerConstants.ImageURLs.Unchecked;
                    }
                }
            }
        }

        #endregion Public methods
    }
}