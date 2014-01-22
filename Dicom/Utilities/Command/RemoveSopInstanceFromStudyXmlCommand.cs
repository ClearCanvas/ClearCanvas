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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.Utilities.Command
{
    public class RemoveSopInstanceFromStudyXmlCommand : CommandBase
    {
        private readonly StudyXml _studyXml;
        private readonly string _seriesUid;
        private readonly string _sopInstanceUid;
        private InstanceXml _oldInstanceXml;
        private readonly string _studyInstanceUid;

        public RemoveSopInstanceFromStudyXmlCommand(StudyXml studyXml, string seriesUid, string sopInstanceUid)
            : base(String.Format("Remove sop {0} from study XML of study {1}", sopInstanceUid, studyXml.StudyInstanceUid), true)
        {
            _studyXml = studyXml;
            _seriesUid = seriesUid;
            _sopInstanceUid = sopInstanceUid;
            _studyInstanceUid = studyXml.StudyInstanceUid;
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            // backup
            if (_studyXml.Contains(_seriesUid))
            {
                Platform.Log(LogLevel.Info, "Removing SOP {0} from StudyXML for study {1}", _sopInstanceUid, _studyInstanceUid);
                _oldInstanceXml = _studyXml[_seriesUid][_sopInstanceUid];
                if (!_studyXml.RemoveInstance(_seriesUid, _sopInstanceUid))
                    throw new ApplicationException(String.Format("Could not remove SOP Instance {0} from study {1}", _sopInstanceUid, _studyInstanceUid));
            }
        }

        protected override void OnUndo()
        {
            if (_oldInstanceXml != null)
            {
                Platform.Log(LogLevel.Info, "Restoring sop instance {0} in StudyXML for study {1}", _sopInstanceUid, _studyInstanceUid);
                _studyXml[_seriesUid][_sopInstanceUid] = _oldInstanceXml;
            }
        }
    }
}
