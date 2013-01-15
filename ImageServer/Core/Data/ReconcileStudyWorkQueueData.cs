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
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
    /// <summary>
    /// Series mapping used when creating new study from images.
    /// </summary>
    public class SeriesMapping
    {
        #region Private Members

    	#endregion

		#region Constructors
		/// <summary>
		/// Constructor.  For Serialization.
		/// </summary>
		public SeriesMapping()
		{}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originalUid"></param>
		/// <param name="newUid"></param>
		public SeriesMapping(string originalUid, string newUid)
		{
			OriginalSeriesUid = originalUid;
			NewSeriesUid = newUid;
		}
    	#endregion

		#region Public Properties

    	public string OriginalSeriesUid { get; set; }

    	public string NewSeriesUid { get; set; }

    	#endregion
    }

	/// <summary>
	/// Represents the information encoded in the <see cref="StudyIntegrityQueue.Details"/> column of a <see cref="Model.StudyIntegrityQueue"/> record.
	/// </summary>
	public class ReconcileStudyWorkQueueData
	{
        #region Private members

		#endregion

        #region Public Properties

		public string StoragePath { get; set; }

		public ImageSetDetails Details { get; set; }

		public string UserId { get; set; }

		public DateTime? TimeStamp { get; set; }

		#endregion
	}

    /// <summary>
    /// Represents the information encoded in the <see cref="StudyIntegrityQueue.Details"/> column of a <see cref="Model.StudyIntegrityQueue"/> record.
    /// </summary>
    public class DuplicateSIQQueueData : ReconcileStudyWorkQueueData
    {
    	public List<DicomAttributeComparisonResult> ComparisonResults { get; set; }
    }
}