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

using System.Collections.Generic;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Events
{
	/// <summary>
	/// Event called after a study has been successfully edited.
	/// </summary>
	[ImageServerEvent]
	public class StudyEditedEventArgs : ImageServerEventArgs
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the value indicating how the edit operation was triggered.
		/// </summary>
		public EditType EditType { get; set; }

		/// <summary>
		/// List of command executed on the images.
		/// </summary>
		public List<BaseImageLevelUpdateCommand> EditCommands { get; set; }

		/// <summary>
		/// Gets or sets the reference to the <see cref="StudyEditor"/>
		/// </summary>
		public StudyEditor WorkQueueProcessor { get; set; }

		/// <summary>
		/// Gets or sets the reference to the <see cref="ServerCommandProcessor"/> currently used.
		/// </summary>
		/// <remarks>
		/// Different <see cref="ServerCommandProcessor"/> may be used per images/series.
		/// </remarks>
		public ServerCommandProcessor CommandProcessor { get; set; }

		/// <summary>
		/// Gets or sets the original (prior to update) <see cref="StudyStorageLocation"/> object.
		/// </summary>
		/// <remarks>
		/// This property is a snapshot of the study location before the edit is executed. 
		/// Once the study has been updated, this object may contain invalid information.
		/// </remarks>
		public StudyStorageLocation OriginalStudyStorageLocation { get; set; }

		/// <summary>
		/// Gets or sets the new (updated) <see cref="StudyStorageLocation"/> object.
		/// </summary>
		/// <remarks>
		/// This property may be null if the study hasn't been updated or hasn't been determined. 
		/// Depending on what is modified, it may have the same or different data 
		/// compared with <see cref="OriginalStudyStorageLocation"/>.
		/// </remarks>
		public StudyStorageLocation NewStudyStorageLocation { get; set; }

		/// <summary>
		/// Gets or sets the original <see cref="Study"/>
		/// </summary>
		/// <remarks>
		/// This property is a snapshot of the study before the edit is executed. 
		/// Once the study has been updated, this object may contain invalid information.
		/// </remarks>
		public Study OriginalStudy { get; set; }

		/// <summary>
		/// Gets or sets the reference to the original <see cref="Patient"/> before the study is updated.
		/// </summary>
		/// <remarks>
		/// This property is a snapshot of the patient before the edit is executed. 
		/// Once the study has been updated, this object may contain invalid information.
		/// </remarks>
		public Patient OrginalPatient { get; set; }

		/// <summary>
		/// Gets or sets the id of the user who requested the edit.
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the reason that the study is being editted.
		/// </summary>
		public string Reason { get; set; }

		#endregion
	}
}
