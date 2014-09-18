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
using ClearCanvas.ImageServer.Common.WorkQueue;

namespace ClearCanvas.ImageServer.Core.Edit
{
    /// <summary>
    /// Enumerated values for different levels of deletion.
    /// </summary>
    public enum DeletionLevel
    {
        Study,
        Series,
        Instance
    }

    /// <summary>
    /// Enumerated values for different levels of deletion.
    /// </summary>
    public enum MoveLevel
    {
        Study,
        Series,
        Instance
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// </summary>
    [XmlRoot("WebDeleteWorkQueueEntry")]
    [WorkQueueDataType("CA9B7077-01AC-4889-8499-B13F9041DED0")]
    public class WebDeleteWorkQueueEntryData : WorkQueueData
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
    /// Encapsulate the object stored in the Data column of the "WebMoveStudy" WorkQueue entry
    /// used for study level move.
    /// </summary>
    [XmlRoot("WebMoveWorkQueueEntry")]
    [WorkQueueDataType("B9B1C2EB-3BFC-43DE-8DA5-0B0018B9C50A")]
    public class WebMoveWorkQueueEntryData : WorkQueueData
    {
    	public MoveLevel Level { get; set; }

    	public string UserId { get; set; }

    	public DateTime Timestamp { get; set; }

    	public WebMoveWorkQueueEntryData()
        {
            Level = MoveLevel.Study;
        }
    }

    [XmlRoot("WebDeleteWorkQueueEntry")]
    [WorkQueueDataType("21CA3197-3F41-48EA-ABDE-35BF5E9F27D0")]
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
    [WorkQueueDataType("40E53536-E448-43B8-A823-024719994373")]
    public class WebDeleteSeriesLevelQueueData : WebDeleteWorkQueueEntryData
    {
        public WebDeleteSeriesLevelQueueData()
        {
            Level = DeletionLevel.Series;
        }
    }


    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// used for series level deletion.
    /// </summary>
    [XmlRoot("WebDeleteWorkQueueEntry")]
    [WorkQueueDataType("B0CC1DF3-BFEB-4288-AF3D-6E1534530E03")]
    public class WebDeleteInstanceLevelQueueData : WebDeleteWorkQueueEntryData
    {
        public WebDeleteInstanceLevelQueueData()
        {
            Level = DeletionLevel.Instance;
        }
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "MoveSeries" WorkQueue entry.
    /// </summary>
    [XmlRoot("WebMoveWorkQueueEntry")]
    [WorkQueueDataType("8538C0E7-BA54-4B56-8E54-F58A3AA154B7")]
    public class WebMoveSeriesLevelQueueData : WebMoveWorkQueueEntryData
    {
		public WebMoveSeriesLevelQueueData()
		{
			Level = MoveLevel.Series;
		}

        public List<string> SeriesInstanceUids { get; set; }
        }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "MoveSeries" WorkQueue entry.
    /// </summary>
    [XmlRoot("WebMoveWorkQueueEntry")]
    [WorkQueueDataType("5EC2D085-7F98-4C3F-AB50-50C43C102121")]
    public class WebMoveInstanceLevelQueueData : WebMoveWorkQueueEntryData
    {
        public WebMoveInstanceLevelQueueData()
        {
            Level = MoveLevel.Instance;
        }

        public string SeriesInstanceUid { get; set; }

        public List<string> SopInstanceUids { get; set; }
    }
}