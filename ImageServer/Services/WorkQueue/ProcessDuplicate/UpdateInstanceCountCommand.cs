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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    internal class UpdateInstanceCountCommand : ServerDatabaseCommand
    {

        #region Private Members

        private readonly StudyStorageLocation _studyLocation;
        private readonly DicomFile _file;

        #endregion

        #region Constructors

        public UpdateInstanceCountCommand(StudyStorageLocation studyLocation, DicomFile file)
            :base("Update Study Count")
        {
            _studyLocation = studyLocation;
            _file = file;
        }

        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            String seriesUid = _file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            String instanceUid = _file.DataSet[DicomTags.SopInstanceUid].ToString();
            
            var deleteInstanceBroker = updateContext.GetBroker<IDeleteInstance>();
            var parameters = new DeleteInstanceParameters
                                                      {
                                                          StudyStorageKey = _studyLocation.GetKey(),
                                                          SeriesInstanceUid = seriesUid,
                                                          SOPInstanceUid = instanceUid
                                                      };
            if (!deleteInstanceBroker.Execute(parameters))
            {
                throw new ApplicationException("Unable to update instance count in db");
            }

        }

        #endregion

    }
}