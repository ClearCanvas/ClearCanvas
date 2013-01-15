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
using System.Security.Permissions;
using System.Threading;
using System.Web.UI;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.View)]
    public partial class Default : BasePage
    {
        #region Constants

        private const string QUERY_KEY_SERVER_AE = "serverae";
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "studyuid";
        private const string QUERY_KEY_SERIES_INSTANCE_UID = "seriesuid";

        #endregion Constants

        #region Private mmembers

        private string _serverae = null;
        private string _studyInstanceUid = null;
        private string _seriesInstanceUid = null;

        private ServerPartition _partition;
        private Study _study;
        private Series _series;

        #endregion Private mmembers

        #region Public Properties

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            _studyInstanceUid = Request.QueryString[QUERY_KEY_STUDY_INSTANCE_UID];
            _serverae = Request.QueryString[QUERY_KEY_SERVER_AE];
            _seriesInstanceUid = Request.QueryString[QUERY_KEY_SERIES_INSTANCE_UID];

            LoadSeriesDetails();

            //Hide the UserPanel information
            IMasterProperties master = Master as IMasterProperties;
            master.DisplayUserInformationPanel = false;
        }

        #endregion Protected Methods

        #region Public Methods

        protected override void  OnPreRender(EventArgs e)
        {
            if (_series == null)
            {
                Response.Write("<Br>NO  SUCH SERIES FOUND<Br>");

                SeriesDetailsPanel1.Visible = false;
            }
            else
            {
                SetPageTitle(String.Format("{0}:{1} (Series: {2})", NameFormatter.Format(_study.PatientsName) , _study.PatientId, _series.SeriesNumber), false);
            }
 	        
            base.OnPreRender(e);
        }

        #endregion Public Methods


        #region Private Methods

        private void LoadSeriesDetails()
        {

            if (!String.IsNullOrEmpty(_serverae) && !String.IsNullOrEmpty(_studyInstanceUid) && !String.IsNullOrEmpty(_seriesInstanceUid))
            {
                StudyAdaptor studyAdaptor = new StudyAdaptor();
                SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
                        
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                criteria.AeTitle.EqualTo(_serverae);

                Model.ServerPartition partition = adaptor.GetFirst(criteria);
                if (partition != null)
                {
                    Partition = partition;

                    StudySelectCriteria studyCriteria = new StudySelectCriteria();
                    studyCriteria.StudyInstanceUid.EqualTo(_studyInstanceUid);
                    studyCriteria.ServerPartitionKey.EqualTo(Partition.GetKey());
                    Model.Study study = studyAdaptor.GetFirst(studyCriteria);

                    if (study != null)
                    {
                        // there should be only one study
                        _study = study;

                        SeriesSelectCriteria seriesCriteria = new SeriesSelectCriteria();
                        seriesCriteria.SeriesInstanceUid.EqualTo(_seriesInstanceUid);
                        Series series = seriesAdaptor.GetFirst(seriesCriteria);

                        if (series != null)
                        {
                            _series = series;
                        }
                    }

                }
            }

            if (_study!=null && _series != null)
            {
                SeriesDetailsPanel1.Study = _study;
                SeriesDetailsPanel1.Series = _series;
            }

            DataBind();
        }

        #endregion Private Methods

    }
}
