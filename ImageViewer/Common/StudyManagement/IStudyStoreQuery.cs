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

using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    /// <summary>
    /// Service interface for querying the (local) study store.
    /// </summary>
    /// <remarks>
    /// It is generally reasonable to assume that any DICOM viewer is going to have a local store of studies. This interface
    /// allows for typical DICOM queries, plus any other common query operations, like getting the total number of studies in the store.
    /// </remarks>
    [ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IStudyStoreQuery", Namespace = StudyManagementNamespace.Value)]
    public interface IStudyStoreQuery
    {
        [OperationContract]
        GetStudyCountResult GetStudyCount(GetStudyCountRequest request);

        [OperationContract]
        GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request);

        [OperationContract]
        [FaultContract(typeof(StudyNotFoundFault))]
        GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request);

        [OperationContract]
        [FaultContract(typeof(StudyNotFoundFault))]
        [FaultContract(typeof(SeriesNotFoundFault))]
        GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request);
    }
}
