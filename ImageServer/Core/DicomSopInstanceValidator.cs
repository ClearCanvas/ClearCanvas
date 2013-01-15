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

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Represent a class to validate a DICOM message. Used by <see cref="SopInstanceImporter"/>.
    /// Note: This class does not perform DICOM conformance validation.
    /// </summary>
    internal class DicomSopInstanceValidator
    {
        /// <summary>
        /// Validates the contents in the <see cref="DicomMessageBase"/> object.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="DicomDataException"/> is thrown if the DICOM object fails the validation.
        public void Validate(DicomMessageBase message)
        {
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
            String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
            String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);

            if (String.IsNullOrEmpty(studyInstanceUid))
            {
                throw new DicomDataException("Study Instance UID is missing or empty");
            }

            if (studyInstanceUid.Length > 64 || seriesInstanceUid.Length > 64 || sopInstanceUid.Length > 64)
            {
            	if (studyInstanceUid.Length > 64)
                    throw new DicomDataException(string.Format("Study Instance UID is > 64 bytes in the SOP Instance : {0}", studyInstanceUid));

            	if (seriesInstanceUid.Length > 64)
            		throw new DicomDataException(string.Format("Series Instance UID is > 64 bytes in the SOP Instance : {0}", seriesInstanceUid));

            	throw new DicomDataException(string.Format("SOP Instance UID is > 64 bytes in the SOP Instance : {0}", sopInstanceUid));
            }

			if (studyInstanceUid.EndsWith("."))
				throw new DicomDataException(string.Format("Study Instance UID ends with period : {0}", studyInstanceUid));

			if (seriesInstanceUid.EndsWith("."))
				throw new DicomDataException(string.Format("Series Instance UID ends with period : {0}", seriesInstanceUid));

			if (sopInstanceUid.EndsWith("."))
				throw new DicomDataException(string.Format("SOP Instance UID ends with period : {0}", sopInstanceUid));
		}
    }
}