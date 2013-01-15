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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.SeriesGridView.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Series list panel within the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.SeriesGridView",
                           ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.SeriesGridView.js")]
    public partial class SeriesGridView : ScriptUserControl
    {
        #region Private members

        private ServerPartition _serverPartition;
        private StudySummary _study;
        private IList<Series> _series;
        #endregion Private members

        #region Public properties
        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public ServerPartition Partition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public StudySummary Study
        {
            get { return _study; }
            set { _study = value; }
        }

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        protected IList<Series> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        public IList<Series> SelectedItems
        {
            get
            {
                if (!SeriesListControl.IsDataBound) DataBind();

                if (Series == null || Series.Count == 0)
                    return null;

                int[] rows = SeriesListControl.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<Series> seriesItems = new List<Series>();
                for (int i = 0; i < rows.Length; i++)
                {
                    if (rows[i] < Series.Count)
                    {
                        seriesItems.Add(Series[rows[i]]);
                    }
                }

                return seriesItems;
            }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SeriesListClientID")]
        public string SeriesListClientID
        {
            get { return GridView1.ClientID; }
        }   

        [ExtenderControlProperty]
        [ClientPropertyName("OpenSeriesPageUrl")]
        public string OpenSeriesPageUrl
        {
            get { return  Page.ResolveClientUrl(ImageServerConstants.PageURLs.SeriesDetailsPage); }
        }      

        public Web.Common.WebControls.UI.GridView SeriesListControl
        {
            get { return GridView1; }
        }

         

        #endregion Public properties

        #region Constructors

        public SeriesGridView()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        #endregion Constructors

            
        #region Protected methods

        public override void DataBind()
        {
            if (Study != null && Partition != null)
            {
                var seriesAdaptor = new SeriesSearchAdaptor();
                var criteria = new SeriesSelectCriteria();
                criteria.StudyKey.EqualTo(Study.Key);
                criteria.ServerPartitionKey.EqualTo(Partition.Key);

                Series = seriesAdaptor.Get(criteria);

                GridView1.DataSource = Series;
            }

            base.DataBind();
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

			// Get a count of the number of SIQ entries for the StudyStorageLocation.  If there's
			// any, we don't enable the Delete button.
			var siqController = new StudyIntegrityQueueController();
			var criteria = new StudyIntegrityQueueSelectCriteria();
			criteria.StudyStorageKey.EqualTo(Study.TheStudyStorage.Key);
        	int siqCount = siqController.GetReconcileQueueItemsCount(criteria);

            string reason;
            bool seriesDelete = Study.CanScheduleSeriesDelete(out reason);

            foreach(GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = GridView1.PageIndex * GridView1.PageSize + row.RowIndex;
                    Series series = _series[index];

                    row.Attributes["serverae"] = _serverPartition.AeTitle;
                    row.Attributes["studyuid"] = _study.StudyInstanceUid;
                    row.Attributes["seriesuid"] = series.SeriesInstanceUid;

                    var controller = new StudyController();
                    if (controller.CanManipulateSeries(Study.TheStudyStorage.Key))
                    {
                        
						if (siqCount==0 && seriesDelete)
							row.Attributes.Add("candelete", "true");

                        row.Attributes.Add("canmove", "true");
                    }
                }
            }

            
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            //This sets the GridView Page Size to the number of series. Needs to be done in the OnInit method,
            //since the page size needs to be set here, and the Study and Partition aren't set until the databind
            //happens in StudyDetailsTabs.
            
            var studyInstanceUID = Request.QueryString[ImageServerConstants.QueryStrings.StudyInstanceUID];
            var serverAE = Request.QueryString[ImageServerConstants.QueryStrings.ServerAE];

            if (!String.IsNullOrEmpty(studyInstanceUID) && !String.IsNullOrEmpty(serverAE))
            {
                var adaptor = new ServerPartitionDataAdapter();
                var partitionCriteria = new ServerPartitionSelectCriteria();
                partitionCriteria.AeTitle.EqualTo(serverAE);
                IList<ServerPartition> partitions = adaptor.Get(partitionCriteria);
                if (partitions != null && partitions.Count > 0)
                {
                    if (partitions.Count == 1)
                    {
                        var partition = partitions[0];

                        var studyAdaptor = new StudyAdaptor();
                        var studyCriteria = new StudySelectCriteria();
                        studyCriteria.StudyInstanceUid.EqualTo(studyInstanceUID);
                        studyCriteria.ServerPartitionKey.EqualTo(partition.GetKey());
                        var study = studyAdaptor.GetFirst(studyCriteria);

                        if (study!=null)
                        {
                            var seriesAdaptor = new SeriesSearchAdaptor();
                            var criteria = new SeriesSelectCriteria();
                            criteria.StudyKey.EqualTo(study.GetKey());
                            criteria.ServerPartitionKey.EqualTo(partition.GetKey());

                            Series = seriesAdaptor.Get(criteria);

                            GridView1.PageSize = Series.Count;
                        }
                        
                    }
                }
            }
            else
            {
                GridView1.PageSize = 150;   //Set it to a large number to ensure that all series are displayed if more than 25.
            }         
        }

        #endregion Protected methods

    }
}