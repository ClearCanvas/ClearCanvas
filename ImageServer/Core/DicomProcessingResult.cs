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
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Class used for returning result information when processing.  Used for importing and duplicate processing.
	/// </summary>
	public class DicomProcessingResult
	{
		public String AccessionNumber;
		public String StudyInstanceUid;
		public String SeriesInstanceUid;
		public String SopInstanceUid;
        public String SopClassUid;
        public bool Successful;
		public String ErrorMessage;
		public DicomStatus DicomStatus;
        public bool RestoreRequested;

		/// <summary>
		/// Indicates whether the sop being processed is a duplicate.
		/// </summary>
		/// <remarks>
		/// The result of the processing depends on the duplicate policy used.
		/// </remarks>
		public bool Duplicate;

		public void SetError(DicomStatus status, String message)
		{
			Successful = false;
			DicomStatus = status;
			ErrorMessage = message;
		}
	}
}
