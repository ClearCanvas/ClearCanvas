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
using ClearCanvas.Common.Serialization;
using ClearCanvas.ImageServer.Common.StudyHistory;
using ClearCanvas.ImageServer.Common.WorkQueue;

namespace ClearCanvas.ImageServer.Core.Data
{
	[ImageServerStudyHistoryType("4EE14CE6-8F2B-4A99-A89F-1AE96BDF2A21")]
    [WorkQueueDataType("0579EBC2-44EA-4C0F-B30B-2B2D9647B1E1")]
    public class ReprocessStudyChangeLog : DataContractBase
    {
        #region Private Members

    	#endregion

        #region Public Properties

    	public DateTime TimeStamp { get; set; }

    	public string Reason { get; set; }

    	public string User { get; set; }

    	public string StudyInstanceUid { get; set; }

    	#endregion
    }
}