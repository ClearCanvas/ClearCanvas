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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.StudyHistory;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
	[ImageServerStudyHistoryType("FD296EB8-97A1-419F-845C-C971CB768836")]
    public class ProcessDuplicateChangeLog : ImageServerStudyHistory
    {
        #region Constructors

    	public ProcessDuplicateChangeLog()
    	{
    		TimeStamp = Platform.Time;
    	}

    	#endregion

        #region Public Properties

    	public DateTime TimeStamp { get; set; }

    	public ProcessDuplicateAction Action { get; set; }

    	public ImageSetDetails DuplicateDetails { get; set; }

    	public StudyInformation StudySnapShot { get; set; }

    	[XmlArray("StudyUpdateCommands")]
    	[XmlArrayItem("Command", Type = typeof (AbstractProperty<BaseImageLevelUpdateCommand>))]
    	public List<BaseImageLevelUpdateCommand> StudyUpdateCommands { get; set; }

        public string UserName { get; set; }

    	#endregion
    }
}