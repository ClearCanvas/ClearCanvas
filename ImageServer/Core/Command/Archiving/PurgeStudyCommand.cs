#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Command.Archiving
{
	/// <summary>
	/// Command to purge a specified study from the filesystem and set the study to Nearline.
	/// Note: the study must be archived first.
	/// </summary>
	public sealed class PurgeStudyCommand : AggregateCommand, IDisposable
	{
		public PurgeStudyCommand(StudyStorage studyStorage) : base()
		{
			var storageLocations = StudyStorageLocation.FindStorageLocations(studyStorage);
			AddSubCommand(new VerifyStudyHasBeenArchivedCommand(studyStorage));
			foreach (var location in storageLocations)
			{
				string path = location.GetStudyPath();
				AddSubCommand(new Dicom.Utilities.Command.DeleteDirectoryCommand(path, false));
			}

			AddSubCommand(new SetStudyStatusNearlineCommand(studyStorage));
		}

		protected override void OnUndo()
		{
			
		}

		public void Dispose()
		{
		}

	}

	/// <summary>
	/// Command to verify a study has been archived.
	/// </summary>
	class VerifyStudyHasBeenArchivedCommand : CommandBase
	{
		private readonly StudyStorage _storage;

		public VerifyStudyHasBeenArchivedCommand(StudyStorage storage)
			: base("Verify Study has been archived", true)
		{
			_storage = storage;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			var locations = StudyStorageLocation.FindStorageLocations(_storage);
			if (locations.Any(location => location.ArchiveLocations.Any()))
			{
				return;
			}
			throw new ApplicationException("Cannot purge study which has not been archived");
		}

		protected override void OnUndo()
		{
			// no-op
		}
	}

	/// <summary>
	/// Command to set a study status to Nearline
	/// </summary>
	class SetStudyStatusNearlineCommand : ServerDatabaseCommand
	{
		private readonly StudyStorage _storage;

		public SetStudyStatusNearlineCommand(StudyStorage studyStorage)
			: base("Set Study to Nearline")
		{
			_storage = studyStorage;
		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			// Setup the delete parameters
			var parms = new DeleteFilesystemStudyStorageParameters();

			parms.ServerPartitionKey = _storage.ServerPartitionKey;
			parms.StudyStorageKey = _storage.GetKey();
			parms.StudyStatusEnum = StudyStatusEnum.Nearline; // TODO: Don't we set Nearline only if all the storage location are purged?
			
			// Get the Insert Instance broker and do the insert
			var delete = updateContext.GetBroker<IDeleteFilesystemStudyStorage>();

			if (false == delete.Execute(parms))
			{
				Platform.Log(LogLevel.Error, "Unexpected error when trying to delete study: {0} on partition {1}", _storage.StudyInstanceUid, _storage.ServerPartition.Description);
			}
			else
			{
				// Unlock the study, too
				var studyLock = updateContext.GetBroker<ILockStudy>();
				var lockParms = new LockStudyParameters();
				lockParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
				lockParms.StudyStorageKey = _storage.GetKey();
				studyLock.Execute(lockParms);
			}
		}
	}
}
