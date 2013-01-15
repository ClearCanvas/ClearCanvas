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
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.Move)]
    public partial class Default : BasePage
    {
        #region constants
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "studyuid";
        private const string QUERY_KEY_SERVER_AE = "serverae";
        #endregion constants

        #region Private Members
        private readonly IDictionary<string, string> _uids = new Dictionary<string, string>();
        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            StudyController studyController = new StudyController();
            ServerPartitionConfigController partitionConfigController = new ServerPartitionConfigController();

            string serverae = Request.QueryString[QUERY_KEY_SERVER_AE];
            if (!String.IsNullOrEmpty(serverae))
            {
                // Load the Partition
                ServerPartitionSelectCriteria partitionCriteria = new ServerPartitionSelectCriteria();
                partitionCriteria.AeTitle.EqualTo(serverae);
                IList<ServerPartition> list = partitionConfigController.GetPartitions(partitionCriteria);
                this.Move.Partition = list[0];

                for (int i = 1;; i++)
                {
                    string studyuid = Request.QueryString[String.Format("{0}{1}", QUERY_KEY_STUDY_INSTANCE_UID, i)];

                    if (!String.IsNullOrEmpty(studyuid))
                    {
                        _uids.Add(studyuid, serverae);

                        StudySelectCriteria studyCriteria = new StudySelectCriteria();
                        studyCriteria.StudyInstanceUid.EqualTo(studyuid);
                        studyCriteria.ServerPartitionKey.EqualTo(list[0].GetKey());

                        IList<Study> studyList = studyController.GetStudies(studyCriteria);

                        this.Move.StudyGridView.StudyList.Add(studyList[0]);
                        this.Move.StudyGridView.Partition = this.Move.Partition;
                    }
                    else
                        break;
                }
            }

            SetPageTitle(Titles.MoveStudiesPageTitle);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the UserPanel information
            IMasterProperties master = Master as IMasterProperties;
            master.DisplayUserInformationPanel = false;
        }
    }
}
