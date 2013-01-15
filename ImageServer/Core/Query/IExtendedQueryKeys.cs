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

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Query
{
    public interface IExtendedQueryKeys
    {
        void OnReceivePatientLevelQuery(DicomMessage message, DicomTag tag, PatientSelectCriteria criteria, StudySelectCriteria studyCriteria, out bool studySubSelect);
        void OnReceiveStudyLevelQuery(DicomMessage message, DicomTag tag, StudySelectCriteria criteria);
        void OnReceiveSeriesLevelQuery(DicomMessage message, DicomTag tag, SeriesSelectCriteria criteria);

        void PopulatePatient(DicomMessageBase response, DicomTag tag, Patient row, Study theStudy);
        void PopulateStudy(DicomMessageBase response, DicomTag tag, Study row);
        void PopulateSeries(DicomMessageBase response, DicomTag tag, Series row);
    }

    public class QueryKeysExtensionPoint : ExtensionPoint<IExtendedQueryKeys>
    {
    }
}
