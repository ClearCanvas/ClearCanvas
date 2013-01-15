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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.ServiceModel;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using Resources;
using SR = Resources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    //
    //  Used to display the list of devices.
    //
    public partial class FileSystemsGridView : GridViewPanel
    {
        #region private members

        private IList<Filesystem> _fileSystems;
        private Unit _height;
        #endregion Private members

        #region protected properties

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Gets/Sets the height of the filesystem list panel.
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

        /// <summary>
        /// Gets/Sets the current selected FileSystem.
        /// </summary>
        public Filesystem SelectedFileSystem
        {
            get
            {
                if (FileSystems.Count == 0 || FSGridView.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = FSGridView.PageIndex*FSGridView.PageSize + FSGridView.SelectedIndex;

                if (index < 0 || index > FileSystems.Count - 1)
                    return null;

                return FileSystems[index];
            }
            set
            {
                FSGridView.SelectedIndex = FileSystems.IndexOf(value);
                if (FileSystemSelectionChanged != null)
                    FileSystemSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of file systems rendered on the screen.
        /// </summary>
        public IList<Filesystem> FileSystems
        {
            get { return _fileSystems; }
            set
            {
                _fileSystems = value;
                FSGridView.DataSource = _fileSystems; // must manually call DataBind() later
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Defines the handler for <seealso cref="FileSystemSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedFileSystem"></param>
        public delegate void FileSystemSelectedEventHandler(object sender, Filesystem selectedFileSystem);

        /// <summary>
        /// Occurs when the selected filesystem in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected filesystem can change programmatically or by users selecting the filesystem in the list.
        /// </remarks>
        public event FileSystemSelectedEventHandler FileSystemSelectionChanged;

        #endregion // Events


        #region private methods

        protected void FSGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (FSGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeUsageColumn(e.Row);
                    CustomizePathColumn(e.Row);
                    CustomizeEnabledColumn(e);
                    CustomizeReadColumn(e);
                    CustomizeWriteColumn(e);
                    CustomizeFilesystemTierColumn(e.Row);
                }
            }
        }


        private float GetFilesystemUsedPercentage(Filesystem fs)
        {
            if (!IsServiceAvailable())
                return float.NaN;

            try
            {
                FilesystemInfo fsInfo = null;
                Platform.GetService(delegate(IFilesystemService service)
                {
                    fsInfo = service.GetFilesystemInfo(fs.FilesystemPath);
                });

                _serviceIsOffline = false;
                _lastServiceAvailableTime = Platform.Time;
                return 100.0f - ((float)fsInfo.FreeSizeInKB) / fsInfo.SizeInKB * 100.0F;
            }
            catch (Exception)
            {
                _serviceIsOffline = true;
                _lastServiceAvailableTime = Platform.Time;
            }

            return float.NaN;
        }

        private void CustomizeUsageColumn(GridViewRow row)
        {
            var fs = row.DataItem as Filesystem;
            var img = row.FindControl("UsageImage") as Image;

            float usage = GetFilesystemUsedPercentage(fs);
            if (fs != null && img != null)
            {
                img.ImageUrl = string.Format(ImageServerConstants.PageURLs.BarChartPage,
                                             usage,
                                             fs.HighWatermark,
                                             fs.LowWatermark);
                img.AlternateText = string.Format(Server.HtmlEncode(Tooltips.AdminFilesystem_DiskUsage).Replace(Environment.NewLine, "<br/>"),
                                  float.IsNaN(usage) ? SR.Unknown : usage.ToString(),
                                  fs.HighWatermark, fs.LowWatermark);
            }
        }

        private void CustomizePathColumn(GridViewRow row)
        {
            var fs = row.DataItem as Filesystem;
            var lbl = row.FindControl("PathLabel") as Label; // The label is added in the template

            if (fs != null && lbl!=null && fs.FilesystemPath != null)
            {
                // truncate it
                if (fs.FilesystemPath.Length > 50)
                {
                    lbl.Text = fs.FilesystemPath.Substring(0, 45) + "...";
                    lbl.ToolTip = string.Format("{0}: {1}", fs.Description, fs.FilesystemPath);
                }
                else
                {
                    lbl.Text = fs.FilesystemPath;
                }
            }
        }

        private void CustomizeFilesystemTierColumn(GridViewRow row)
        {
            var fs = row.DataItem as Filesystem;
            var lbl = row.FindControl("FilesystemTierDescription") as Label; // The label is added in the template
            if (fs != null && lbl != null)
                lbl.Text = ServerEnumDescription.GetLocalizedDescription(fs.FilesystemTierEnum);
        }


        private void CustomizeBooleanColumn(GridViewRow row, string controlName, string fieldName)
        {
            var img = ((Image)row.FindControl(controlName));
            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(row.DataItem, fieldName));
                img.ImageUrl = active ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }
        }

        #endregion

        #region protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var theController = new FileSystemsConfigurationController();
            var criteria = new FilesystemSelectCriteria();
            criteria.FilesystemTierEnum.SortAsc(0);
            criteria.Description.SortAsc(1);
            FileSystems = theController.GetFileSystems(criteria);

            TheGrid = FSGridView;

            GridPagerTop.InitializeGridPager(SR.GridPagerFileSystemSingleItem, SR.GridPagerFileSystemMultipleItems, TheGrid,
                                             () => FileSystems.Count, ImageServerConstants.GridViewPagerPosition.Top);
            Pager = GridPagerTop;
            GridPagerTop.Reset();

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            DataBind();
        }

        #endregion   

        protected void CustomizeReadColumn(GridViewRowEventArgs e)
        {
            var img = ((Image) e.Row.FindControl("ReadImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canRead = enabled && (readOnly || ( /*not readonly and */ !writeOnly));

                img.ImageUrl = canRead ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }
        }

        protected void CustomizeEnabledColumn(GridViewRowEventArgs e)
        {
            CustomizeBooleanColumn(e.Row, "EnabledImage", "Enabled");
        }

        protected void CustomizeWriteColumn(GridViewRowEventArgs e)
        {
            var img = ((Image) e.Row.FindControl("WriteImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canWrite = enabled && (writeOnly || ( /*not write only and also */ !readOnly));

                img.ImageUrl = canWrite ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }
        }

        #region Private Static members
        static private bool _serviceIsOffline;
        static private DateTime _lastServiceAvailableTime = Platform.Time;

        /// <summary>
        /// Return a value indicating whether the last web service call was successful.
        /// </summary>
        /// <returns></returns>
        static private bool IsServiceAvailable()
        {
            TimeSpan elapsed = Platform.Time - _lastServiceAvailableTime;
            return (!_serviceIsOffline || /*service was offline but */ elapsed.Seconds > 15);
        }

        #endregion Private Static members

    }
}
