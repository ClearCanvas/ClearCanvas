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
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Core.Edit;

namespace ClearCanvas.ImageServer.Core
{

    /// <summary>
    /// Represents the result of the DICOM file preprocessing.
    /// </summary>
    public class InstancePreProcessingResult
    {
        #region Private Members

        #endregion

        /// <summary>
        /// Indicates whether the file has been updated 
        /// as part of auto-reconciliation process.
        /// </summary>
        public bool AutoReconciled { get; set; }

        /// <summary>
        /// Indicates whether or not the file should be discarded.
        /// </summary>
        public bool DiscardImage { get; set; }

        public bool Modified { get; set; }
    }

    /// <summary>
    /// Represents changes applied to a DICOM
    /// </summary>
    public class UpdateItem
    {
        #region Private Members
        private readonly DicomTag _tag;
        private readonly string _originalValue;
        private readonly string _newValue;
        
        #endregion
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="originalValue"></param>
        /// <param name="newValue"></param>
        public UpdateItem(uint tag, string originalValue, string newValue)
        {
            _tag = DicomTagDictionary.GetDicomTag(tag);
            _originalValue = originalValue;
            _newValue = newValue;
        }

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="file"></param>
        public UpdateItem(IUpdateImageTagCommand command, DicomFile file)
        {
            _tag = command.UpdateEntry.TagPath.Tag;
            _originalValue = file.DataSet[Tag].ToString();
            _newValue = command.UpdateEntry.Value != null ? command.UpdateEntry.Value.ToString() : String.Empty;
        }
        
        #endregion
        #region Public Properties

        /// <summary>
        /// Gets the DICOM tag being updated.
        /// </summary>
        public DicomTag Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// Gets the original value of the DICOM tag being updated.
        /// </summary>
        public string OriginalValue
        {
            get { return _originalValue; }
        }

        /// <summary>
        /// Gets the new value of the DICOM tag being updated.
        /// </summary>
        public string NewValue
        {
            get { return _newValue; }
        } 
        #endregion
    }
}
