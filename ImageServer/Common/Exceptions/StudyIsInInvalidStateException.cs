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

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the study is in invalid state.
    /// </summary>
    public class StudyIsInInvalidStateException: Exception
    {
        public StudyIsInInvalidStateException(string state, string studyInstanceUid, string message)
            :base(message)
        {
            CurrentState = state;
            StudyInstanceUid = studyInstanceUid;
            //   CurrentState = location.StudyStatusEnum.Description;
            // StudyInstanceUid = location.StudyInstanceUid;
        }

        public string CurrentState { get; set; }
        /// <summary>
        /// The study instance UID of the study that is nearline.
        /// </summary>		
        public string StudyInstanceUid { get; set; }
    }
}