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
using ClearCanvas.ImageServer.Common.StudyHistory;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Decoded information of the ChangeDescription field of a <see cref="StudyHistory"/> 
	/// record whose type is "WebEdited"
	/// </summary>
	[ImageServerStudyHistoryType("49E19BBC-DAF3-40CD-BEAB-FC15A9028572")]
	public class WebEditStudyHistoryChangeDescription : ImageServerStudyHistory
	{
		#region Public Properties

		/// <summary>
		/// Type of the edit operation occured on the study.
		/// </summary>
		[XmlElement("EditType")]
		public EditType EditType { get; set; }

		/// <summary>
		/// Reason that the study is being editted
		/// </summary>
		[XmlElement("Reason")]
		public string Reason { get; set; }

		/// <summary>
		/// List of <see cref="BaseImageLevelUpdateCommand"/> that were executed on the study.
		/// </summary>
		[XmlArrayItem("Command", Type = typeof (AbstractProperty<BaseImageLevelUpdateCommand>))]
		public List<BaseImageLevelUpdateCommand> UpdateCommands { get; set; }

		[XmlElement("UserId")]
		public string UserId { get; set; }

		[XmlElement("TimeStamp")]
		public DateTime? TimeStamp { get; set; }

		#endregion
	}
}