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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Base StudyProcess Preprocessor class.
    /// </summary>
    public abstract class BasePreprocessor
    {
        #region Private Members

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="AutoReconciler"/> to update
        /// a DICOM file according to the history.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="storageLocation"></param>
        public BasePreprocessor(string description, StudyStorageLocation storageLocation)
        {
            Platform.CheckForEmptyString(description, "description");
            Platform.CheckForNullReference(storageLocation, "storageLocation");

            StorageLocation = storageLocation;
            Description = description;
        }
        
        #endregion

        #region Public Properties

        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StudyStorageLocation"/> of the study which the 
        /// DICOM file(s) belong to.
        /// </summary>
        public StudyStorageLocation StorageLocation { get; set; }

        #endregion

    }
}