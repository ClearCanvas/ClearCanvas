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
using ClearCanvas.ImageServer.Core.Validation;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Attribute to specify what type of validation must be made when a work queue entry is processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StudyIntegrityValidationAttribute:Attribute
    {
        #region Private Memebers
        private StudyIntegrityValidationModes _validationTypes;
        private RecoveryModes _Recovery;
        #endregion

        #region Public Properties
        public StudyIntegrityValidationModes ValidationTypes
        {
            get { return _validationTypes; }
            set { _validationTypes = value; }
        }

        public RecoveryModes Recovery
        {
            get { return _Recovery; }
            set { _Recovery = value; }
        }

        #endregion
    }
}