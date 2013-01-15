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
using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    /// <summary>
    /// Fault contract indicating the requested study cannot be accessed because it is being used on the server.
    /// </summary>
    [DataContract]
    public class StudyIsInUseFault
    {
        #region Private Members
        private String _studyState;
        #endregion

        #region Constructors
        public StudyIsInUseFault(string state)
        {
            _studyState = state;
        }
        #endregion

        /// <summary>
        /// Gets or sets the current state of the study.
        /// </summary>
        /// 
        [DataMember(IsRequired = false)]
        public String StudyState
        {
            get { return _studyState; }
            set { _studyState = value; }
        }
    }
}