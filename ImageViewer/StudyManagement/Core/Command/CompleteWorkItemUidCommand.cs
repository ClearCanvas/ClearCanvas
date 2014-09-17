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

using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
	/// <summary>
	/// Complete a specific <see cref="WorkItemUid"/> record in the database.
	/// </summary>
	public class CompleteWorkItemUidCommand : DataAccessCommand
	{
		private WorkItemUid _uid;

		public CompleteWorkItemUidCommand(WorkItemUid uid)
			: base("Complete WorkItemUid")
		{
			_uid = uid;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			var broker = DataAccessContext.GetWorkItemUidBroker();
			_uid = broker.GetWorkItemUid(_uid.Oid);
			_uid.Complete = true;
		}

		protected override void OnUndo()
		{
			// TODO (CR Jun 2012): Possible for it to have already been true when OnExecute fired?
			_uid.Complete = false;
		}
	}
}