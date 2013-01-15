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
    public class RemoveSeriesFromStudyXml : CommandBase
    {
        private readonly StudyXml _studyXml;
        private readonly string _seriesUid;
        private SeriesXml _oldSeriesXml;
        private readonly string _studyInstanceUid;

        public RemoveSeriesFromStudyXml(StudyXml studyXml, string seriesUid)
            : base(String.Format("Remove series {0} from study XML of study {1}", seriesUid, studyXml.StudyInstanceUid), true)
        {
            _studyXml = studyXml;
            _seriesUid = seriesUid;
            _studyInstanceUid = studyXml.StudyInstanceUid;
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            // backup
            if (_studyXml.Contains(_seriesUid))
            {
                Platform.Log(LogLevel.Info, "Removing series {0} from StudyXML for study {1}", _seriesUid, _studyInstanceUid);
                _oldSeriesXml = _studyXml[_seriesUid];
                if (!_studyXml.RemoveSeries(_seriesUid))
                    throw new ApplicationException(String.Format("Could not remove series {0} from study {1}", _seriesUid, _studyInstanceUid));
            }
        }

        protected override void OnUndo()
        {
            if (_oldSeriesXml != null)
            {
                Platform.Log(LogLevel.Info, "Restoring series {0} in StudyXML for study {1}", _seriesUid, _studyInstanceUid);
                _studyXml[_seriesUid] = _oldSeriesXml;
            }
        }
    }
}
