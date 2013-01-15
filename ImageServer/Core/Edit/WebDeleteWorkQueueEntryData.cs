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
using System.Xml.Serialization;

namespace ClearCanvas.ImageServer.Core.Edit
{
    /// <summary>
    /// Enumerated values for different levels of deletion.
    /// </summary>
    public enum DeletionLevel
    {
        Study,
        Series
    }

    /// <summary>
    /// Enumerated values for different levels of deletion.
    /// </summary>
    public enum MoveLevel
    {
        Study,
        Series
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// </summary>
    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteWorkQueueEntryData
    {
        #region Constructors

        public WebDeleteWorkQueueEntryData()
        {
            Level = DeletionLevel.Study;
        } 
        #endregion

		#region Public Properties

    	/// <summary>
    	/// Gets the Deletion Level.
    	/// </summary>
    	public DeletionLevel Level { get; set; }

    	/// <summary>
    	/// User-specified reason for deletion.
    	/// </summary>
    	public string Reason { get; set; }

    	/// <summary>
    	/// Gets the user who entered the delete request.
    	/// </summary>
    	public string UserId { get; set; }

    	/// <summary>
    	/// Gets the timestamp when the delete request was entered.
    	/// </summary>
    	public DateTime Timestamp { get; set; }

		/// <summary>
		/// Gets the user name who entered the delete request
		/// </summary>
    	public string UserName { get; set; }

    	#endregion
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// used for study level deletion.
    /// </summary>
    [XmlRoot("WebMoveWorkQueueEntry")]
    public class WebMoveWorkQueueEntryData
    {
    	public string Reason { get; set; }

    	public MoveLevel Level { get; set; }

    	public string UserId { get; set; }

    	public DateTime Timestamp { get; set; }

    	public WebMoveWorkQueueEntryData()
        {
            Level = MoveLevel.Study;
        }
    }

    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteStudyLevelQueueData : WebDeleteWorkQueueEntryData
    {
        public WebDeleteStudyLevelQueueData()
        {
            Level = DeletionLevel.Study;
        }
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// used for series level deletion.
    /// </summary>
    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteSeriesLevelQueueData : WebDeleteWorkQueueEntryData
    {
        public WebDeleteSeriesLevelQueueData()
        {
            Level = DeletionLevel.Series;
        }
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "MoveSeries" WorkQueue entry.
    /// </summary>
    [XmlRoot("WebMoveWorkQueueEntry")]
    public class WebMoveSeriesLevelQueueData : WebMoveWorkQueueEntryData
    {
		public WebMoveSeriesLevelQueueData()
		{
			Level = MoveLevel.Series;
		}

        private List<string> _seriesInstanceUids;

        public List<string> SeriesInstanceUids
        {
            get { return _seriesInstanceUids; }
            set { _seriesInstanceUids = value; }
        }

    }
}