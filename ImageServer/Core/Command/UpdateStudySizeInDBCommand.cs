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
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Command
{
    /// <summary>
    /// Command to update the Study Size in the database.
    /// </summary>
    public class UpdateStudySizeInDBCommand : ServerDatabaseCommand
    {
        #region Private Members
        const decimal KB = 1024;
        private readonly StudyStorageLocation _location;
        private decimal _studySizeInKB;
        private readonly RebuildStudyXmlCommand _rebuildCommand;
        #endregion

        #region Constructors
        public UpdateStudySizeInDBCommand(StudyStorageLocation location)
            : base("Update Study Size In DB")
        {
            _location = location;

            // this may take a few ms so it's better to do it here instead in OnExecute()
            StudyXml studyXml = _location.LoadStudyXml();
            _studySizeInKB = studyXml.GetStudySize() / KB;
        }

        public UpdateStudySizeInDBCommand(StudyStorageLocation location, RebuildStudyXmlCommand rebuildCommand)
            : base("Update Study Size In DB")
        {
            _location = location;

            _rebuildCommand = rebuildCommand;
        }
        #endregion

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            if (_rebuildCommand != null) _studySizeInKB = _rebuildCommand.StudyXml.GetStudySize() / KB;

            Study study = _location.Study ?? Study.Find(updateContext, _location.Key);

            if (study != null && study.StudySizeInKB != _studySizeInKB)
            {
                var broker = updateContext.GetBroker<IStudyEntityBroker>();
                var parameters = new StudyUpdateColumns
                                     {
                                         StudySizeInKB = _studySizeInKB
                                     };
                if (!broker.Update(study.Key, parameters))
                    throw new ApplicationException("Unable to update study size in the database");
            }
        }
    }
}