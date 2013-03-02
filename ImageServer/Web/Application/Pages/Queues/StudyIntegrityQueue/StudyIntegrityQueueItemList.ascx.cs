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
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    //
    //  Used to display the list of Restore Queue Items.
    //
    public partial class StudyIntegrityQueueItemList : GridViewPanel
    {
        #region Delegates

        public delegate void StudyIntegrityQueueDataSourceCreated(StudyIntegrityQueueDataSource theSource);

        #endregion

        private const string HighlightCssClass = "ConflictField";

        public event StudyIntegrityQueueDataSourceCreated DataSourceCreated;

        #region Private members

        // list of studies to display
        private StudyIntegrityQueueDataSource _dataSource;
        private Unit _height;
        private IList<StudyIntegrityQueueSummary> _integrityQueueItems; 
        private Dictionary<string,StudyIntegrityQueueSummary> _integrityQueueDictionary = new Dictionary<string, StudyIntegrityQueueSummary>(); 
        #endregion Private members

        #region Public properties

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new StudyIntegrityQueueDataSource();

                    _dataSource.StudyIntegrityQueueFoundSet +=
                        delegate(IList<StudyIntegrityQueueSummary> newlist) { Items = newlist; };
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
        public GridView StudyIntegrityQueueGrid
        {
            get { return StudyIntegrityQueueGridView; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<StudyIntegrityQueueSummary> SelectedItems
        {
            get
            {
                if (!StudyIntegrityQueueGridView.IsDataBound) StudyIntegrityQueueGridView.DataBind();

                if (Items == null || Items.Count == 0)
                    return null;

                string[] rows = StudyIntegrityQueueGridView.SelectedDataKeys;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<StudyIntegrityQueueSummary> queueItems = new List<StudyIntegrityQueueSummary>();
                foreach (string t in rows)
                {
                    if (_integrityQueueDictionary.ContainsKey(t))
                        queueItems.Add(_integrityQueueDictionary[t]);
                }
                if (queueItems.Count == 0)
                    return null;

                return queueItems;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<StudyIntegrityQueueSummary> Items
        {
            get
            {
                return _integrityQueueItems;
            }
            set
            {
                _integrityQueueItems = value;
                _integrityQueueDictionary.Clear();
                foreach (StudyIntegrityQueueSummary study in _integrityQueueItems)
                {
                    _integrityQueueDictionary.Add(study.TheStudyIntegrityQueueItem.Key.ToString(), study);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get { return ContainerTable != null ? ContainerTable.Height : _height; }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        #endregion

        #region Events

        #region Delegates

        /// <summary>
        /// Defines the handler for <seealso cref="OnQueueItemSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItems"></param>
        public delegate void QueueItemSelectedEventHandler(
            object sender, IList<StudyIntegrityQueueSummary> selectedItems);

        #endregion

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

            TheGrid = StudyIntegrityQueueGridView;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            TheGrid.SelectedIndexChanged += StudyIntegrityQueueGridView_SelectedIndexChanged;

            if (IsPostBack || Page.IsAsync)
            {
                TheGrid.DataSource = StudyIntegrityQueueDataSourceObject;
            }
        }

        protected void StudyIntegrityQueueGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<StudyIntegrityQueueSummary> queueItems = SelectedItems;
            if (queueItems != null)
                if (OnQueueItemSelectionChanged != null)
                    OnQueueItemSelectionChanged(this, queueItems);
        }

        protected void StudyIntegrityQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            StudyIntegrityQueueGridView.DataBind();
        }

        protected void StudyIntegrityQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StudyIntegrityQueueGridView.PageIndex = e.NewPageIndex;
            StudyIntegrityQueueGridView.DataBind();
        }

        protected void GetStudyIntegrityQueueDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new StudyIntegrityQueueDataSource();

                _dataSource.StudyIntegrityQueueFoundSet +=
                    delegate(IList<StudyIntegrityQueueSummary> newlist) { Items = newlist; };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);
        }

        protected void StudyIntegrityQueueItemList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                var message =
                    (EmptySearchResultsMessage) e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    message.Message = StudyIntegrityQueueGridView.DataSource == null
                                          ? SR.PleaseEnterSearchCriteriaToFindStudies
                                          : SR.NoSIQItemsFound;
                }
            }
            else
            {
                GridViewRow row = e.Row;

                if (StudyIntegrityQueueGridView.EditIndex != e.Row.RowIndex)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        var item = row.DataItem as StudyIntegrityQueueSummary;
                        if (item != null)
                        {
                            row.FindControl("ExistingStudyTable").Visible = item.StudyExists;
                            row.FindControl("StudyNotAvailableLabel").Visible = !item.StudyExists;
                            if (item.StudyExists)
                            {
                                CustomizeRowAttribute(e.Row);
                                HighlightDifference(e.Row);
                            }

                            var tooltipControl = row.FindControl("TooltipContent") as SIQEntryTooltip;
                            if (tooltipControl != null)
                            {
                                tooltipControl.SIQItem = item;
                                tooltipControl.DataBind();
                            }
                        }
                    }
                }
            }
        }

        private static void HighlightDifference(GridViewRow row)
        {
            var item = row.DataItem as StudyIntegrityQueueSummary;
            if (item != null)
            {
                var existingAccessionNumber = row.FindControl("ExistingAccessionNumber") as Label;
                var conflictingAccessionNumber = row.FindControl("ConflictingAccessionNumber") as Label;
                var existingPatientId = row.FindControl("ExistingPatientId") as Label;
                var conflictingPatientId = row.FindControl("ConflictingPatientId") as Label;
                var existingPatientName = row.FindControl("ExistingPatientName") as Label;
                var conflictingPatientName = row.FindControl("ConflictingPatientName") as Label;

                Highlight(item.ExistingPatientId, item.ConflictingPatientId, existingPatientId, conflictingPatientId);
                Highlight(item.ExistingPatientName, item.ConflictingPatientName, existingPatientName,
                          conflictingPatientName);
                Highlight(item.ExistingAccessionNumber, item.ConflictingAccessionNumber, existingAccessionNumber,
                          conflictingAccessionNumber);
            }
        }

        private static void Highlight(String value1, String value2, WebControl existingFieldControl,
                                      WebControl conflictingFieldControl)
        {
            if (existingFieldControl != null && conflictingFieldControl != null)
            {
                if (!StringUtils.AreEqual(value1, value2, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingFieldControl.CssClass += " " + HighlightCssClass;
                    conflictingFieldControl.CssClass += " " + HighlightCssClass;
                }
            }
        }

        private static void CustomizeRowAttribute(GridViewRow row)
        {
            var item = row.DataItem as StudyIntegrityQueueSummary;
            if (item != null)
            {
                row.Attributes["canreconcile"] = item.CanReconcile ? "true" : "false";
            }
        }

        public void SetDataSource()
        {
            StudyIntegrityQueueGridView.DataSource = StudyIntegrityQueueDataSourceObject;
        }

        #endregion
    }
}