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
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Validation
{
    public enum ValidationErrors
    {
        // Object count in db and study xml do not match
        InconsistentObjectCount
    }

    /// <summary>
    /// Represents exception thrown when study validation fails.
    /// </summary>
    public class StudyIntegrityValidationFailure : Exception
    {
        #region Private Members
        private readonly ValidationErrors _error;
        private readonly ValidationStudyInfo _validationStudyInfo;
        #endregion
        #region Constructors

        public StudyIntegrityValidationFailure(ValidationErrors error, ValidationStudyInfo validationStudyInfo, string details)
            : base(details)
        {
            Platform.CheckForNullReference(validationStudyInfo, "validationStudyInfo");

            _error = error;
            _validationStudyInfo = validationStudyInfo;
        }
        
        #endregion

        #region Public Properties

        #endregion

        /// <summary>
        /// Gets the <see cref="ValidationStudyInfo"/> for the study that failed the validation.
        /// </summary>
        public ValidationStudyInfo ValidationStudyInfo
        {
            get { return _validationStudyInfo; }
        }

        /// <summary>
        /// Gets the <see cref="ValidationErrors"/>.
        /// </summary>
        public ValidationErrors Error
        {
            get { return _error; }
        }
    }
}