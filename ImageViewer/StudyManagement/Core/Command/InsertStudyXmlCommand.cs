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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
    public class InsertStudyXmlCommand : CommandBase
    {
        #region Private Members

        private readonly DicomFile _file;
        private readonly StudyXml _studyXml;
        private readonly StudyLocation _studyStorageLocation;
        private readonly bool _writeFile;
        private bool _duplicate;
        private readonly StudyXmlOutputSettings _settings;
        #endregion

        public InsertStudyXmlCommand(DicomFile file, StudyXml studyXml, StudyLocation storageLocation, bool writeFile)
            : base("Insert Study Xml", true)
        {
            _file = file;
            _studyXml = studyXml;
            _studyStorageLocation = storageLocation;
            _writeFile = writeFile;

            _settings = new StudyXmlOutputSettings
                            {
                                IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag,
                                IncludeUnknownTags = StudyXmlTagInclusion.IgnoreTag,
                                IncludeLargeTags = StudyXmlTagInclusion.IncludeTagExclusion,
                                MaxTagLength = 2048,
                                IncludeSourceFileName = true
                            };
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            long fileSize = 0;
            if (File.Exists(_file.Filename))
            {
                var finfo = new FileInfo(_file.Filename);
                fileSize = finfo.Length;
            }

            String seriesInstanceUid = _file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
            String sopInstanceUid = _file.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);

            if (_studyXml.Contains(seriesInstanceUid,sopInstanceUid))
            {
                _duplicate = true;                
            }

            // Setup the insert parameters
            if (false == _studyXml.AddFile(_file, fileSize, _settings))
            {
                Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
                             _file.Filename);
                throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
                                               _file.MediaStorageSopInstanceUid);
            }

            if (_writeFile)
            {
                // Write it back out.  We flush it out with every added image so that if a failure happens,
                // we can recover properly.
                bool fileCreated;
                _studyStorageLocation.SaveStudyXml(_studyXml, out fileCreated);
            }
        }

        protected override void OnUndo()
        {
            if (!_duplicate)
            {
                _studyXml.RemoveFile(_file);

                if (_writeFile)
                {
                    bool fileCreated;
                    _studyStorageLocation.SaveStudyXml(_studyXml, out fileCreated);
                }
            }
        }
    }
}
