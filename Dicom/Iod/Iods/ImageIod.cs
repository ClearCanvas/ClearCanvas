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

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// Generic Image IOD.  Note, in progress.
    /// </summary>
    public class ImageIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageIod"/> class.
        /// </summary>
        public ImageIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageIod"/> class.
        /// </summary>
        public ImageIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the patient module.
        /// </summary>
        /// <value>The patient module.</value>
        public PatientIdentificationModuleIod PatientIdentificationModule
        {
            get { return base.GetModuleIod<PatientIdentificationModuleIod>(); }
        }

        /// <summary>
        /// Gets the study module.
        /// </summary>
        /// <value>The study module.</value>
        public StudyModuleIod StudyModule
        {
            get { return base.GetModuleIod<StudyModuleIod>(); }
        }
        #endregion

    }
}
