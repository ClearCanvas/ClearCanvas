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

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the study is nearline.
    /// </summary>
    public class StudyIsNearlineException : SopInstanceProcessingException
    {
        private readonly bool _restoreRequested;

        public StudyIsNearlineException(bool restoreRequested) : base("Study is in Nearline state.")
        {
            _restoreRequested = restoreRequested;
        }

        public bool RestoreRequested
        {
            get { return _restoreRequested; }
        }

		/// <summary>
        /// The study instance UID of the study that is nearline.
        /// </summary>		
        public string StudyInstanceUid { get; set; }

    }
}