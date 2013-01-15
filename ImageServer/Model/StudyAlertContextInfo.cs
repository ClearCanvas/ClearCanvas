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

namespace ClearCanvas.ImageServer.Model
{

    /// <summary>
    /// Contains the info of the study associated with an <see cref="Alert"/>
    /// </summary>
    public class StudyAlertContextInfo
    {
        public string StudyInstanceUid { get; set; }
        public string ServerPartitionAE { get; set; }

        public StudyAlertContextInfo(){}

        public StudyAlertContextInfo(string serverPartitionAE, string studyInstanceUid)
        {
            ServerPartitionAE = serverPartitionAE;
            StudyInstanceUid = studyInstanceUid;
        }
    }
}
