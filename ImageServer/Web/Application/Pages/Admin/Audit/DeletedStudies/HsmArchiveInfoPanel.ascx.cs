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
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class HsmArchiveInfoPanel : BaseDeletedStudyArchiveUIPanel
    {
        public override void DataBind()
        {
            IList<DeletedStudyArchiveInfo> infoList = new List<DeletedStudyArchiveInfo>();
            if (ArchiveInfo != null)
                infoList.Add(ArchiveInfo);

            ArchiveInfoView.DataSource = CollectionUtils.Map(
                infoList,
                delegate(DeletedStudyArchiveInfo info)
                    {
                        var dataModel = new HsmArchivePanelInfoDataModel
                                            {
                                                PartitionArchive = info.PartitionArchive,
                                                ArchiveTime = info.ArchiveTime,
                                                TransferSyntaxUid = info.TransferSyntaxUid,
                                                ArchiveXml = XmlUtils.Deserialize<HsmArchiveXml>(info.ArchiveXml)
                                            };
                        return dataModel;
                    });

            base.DataBind();
        }
    }

    /// <summary>
    /// View Model for the <see cref="HsmArchiveInfoPanel"/>
    /// </summary>
    public class HsmArchivePanelInfoDataModel
    {
        #region Private Fields

        private PartitionArchive _archive;
        private string _archivePath;

        #endregion

        #region Public Properties

        public DateTime ArchiveTime { get; set; }

        public string TransferSyntaxUid { get; set; }

        public HsmArchiveXml ArchiveXml { get; set; }


        public string ArchiveFolderPath
        {
            get
            {
                if (String.IsNullOrEmpty(_archivePath) && _archive != null)
                {
                    var config = XmlUtils.Deserialize<HsmArchiveConfigXml>(_archive.ConfigurationXml);
                    _archivePath = StringUtilities.Combine(new[]
                                                               {
                                                                   config.RootDir, ArchiveXml.StudyFolder,
                                                                   ArchiveXml.Uid, ArchiveXml.Filename
                                                               }, String.Format("{0}", Path.DirectorySeparatorChar));
                }
                return _archivePath;
            }
        }

        public PartitionArchive PartitionArchive
        {
            get { return _archive; }
            set { _archive = value; }
        }

        #endregion
    }


    /// <summary>
    /// Represents the data of an Hsm Archive entry.
    /// </summary>
    [Serializable]
    [XmlRoot("HsmArchive")]
    public class HsmArchiveXml
    {
        #region Private Fields

        private string _filename;
        private string _studyFolder;
        private string _uid;

        #endregion

        #region Public Properties

        public string StudyFolder
        {
            get { return _studyFolder; }
            set { _studyFolder = value; }
        }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        #endregion
    }

    /// <summary>
    /// Represents the Hsm Archive configuration
    /// </summary>
    [Serializable]
    [XmlRoot("HsmArchive")]
    public class HsmArchiveConfigXml
    {
        #region Private Fields

        private string _rootDir;

        #endregion

        #region Public Properties

        public string RootDir
        {
            get { return _rootDir; }
            set { _rootDir = value; }
        }

        #endregion
    }
}