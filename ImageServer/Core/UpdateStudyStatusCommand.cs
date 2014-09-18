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

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.StudyHistory;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Update the <see cref="StudyStorage"/> and <see cref="FilesystemStudyStorage"/> tables according to 
	/// a new Transfersyntax the Study is encoded as.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This method checks the status and transfer syntax set in the current <see cref="StudyStorageLocation"/>
	/// instance passed to the constructor and compares it to the <see cref="DicomFile"/> passed to the constructor.
	/// If they vary, it will then update the database and update the structure with the new values.  If they're
	/// the same, a database update is bypassed.
	/// </para>
	/// <para>
	/// For efficiency sake, this assumes that the same <see cref="StudyStorageLocation"/> instance is used for 
	/// consecutive calls to this <see cref="ServerDatabaseCommand"/> object.
	/// </para>
	/// </remarks>
	public class UpdateStudyStatusCommand : ServerDatabaseCommand
	{
		private readonly StudyStorageLocation _location;
		private readonly DicomFile _file;

		public UpdateStudyStatusCommand(StudyStorageLocation location, DicomFile file)
			: base("Update StudyStorage and FilesystemStudyStorage")
		{
			_location = location;
			_file = file;
		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			// Check if the File is the same syntax as the 
			TransferSyntax fileSyntax = _file.TransferSyntax;
			TransferSyntax dbSyntax = TransferSyntax.GetTransferSyntax(_location.TransferSyntaxUid);

			// Check if the syntaxes match the location
			if ((!fileSyntax.Encapsulated && !dbSyntax.Encapsulated)
			  || (fileSyntax.LosslessCompressed && dbSyntax.LosslessCompressed)
			  || (fileSyntax.LossyCompressed && dbSyntax.LossyCompressed))
			{
				// no changes necessary, just return;
				return;
			}

			// Select the Server Transfer Syntax
			var syntaxCriteria = new ServerTransferSyntaxSelectCriteria();
			var syntaxBroker = updateContext.GetBroker<IServerTransferSyntaxEntityBroker>();
			syntaxCriteria.Uid.EqualTo(fileSyntax.UidString);

			ServerTransferSyntax serverSyntax = syntaxBroker.FindOne(syntaxCriteria);
			if (serverSyntax == null)
			{
				Platform.Log(LogLevel.Error, "Unable to load ServerTransferSyntax for {0}.  Unable to update study status.", fileSyntax.Name);
				return;
			}

			// Get the FilesystemStudyStorage update broker ready
			var filesystemStudyStorageEntityBroker = updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
			var filesystemStorageUpdate = new FilesystemStudyStorageUpdateColumns();
			var filesystemStorageCritiera = new FilesystemStudyStorageSelectCriteria();

			filesystemStorageUpdate.ServerTransferSyntaxKey = serverSyntax.Key;
			filesystemStorageCritiera.StudyStorageKey.EqualTo(_location.Key);

			// Save a StudyHistory record for the compression that occurred.
			StudyHistoryHelper.CreateStudyHistoryRecord(updateContext, _location, _location, StudyHistoryTypeEnum.SopCompress,
			                                            null, new CompressionStudyHistory
				                                            {
					                                            TimeStamp = Platform.Time,
					                                            OriginalTransferSyntaxUid = dbSyntax.UidString,
					                                            FinalTransferSyntaxUid = fileSyntax.UidString
				                                            });

			// Get the StudyStorage update broker ready
			var studyStorageBroker =
				updateContext.GetBroker<IStudyStorageEntityBroker>();
			var studyStorageUpdate = new StudyStorageUpdateColumns();
			StudyStatusEnum statusEnum = _location.StudyStatusEnum;
			if (fileSyntax.LossyCompressed)
				studyStorageUpdate.StudyStatusEnum = statusEnum = StudyStatusEnum.OnlineLossy;
			else if (fileSyntax.LosslessCompressed)
				studyStorageUpdate.StudyStatusEnum = statusEnum = StudyStatusEnum.OnlineLossless;

			studyStorageUpdate.LastAccessedTime = Platform.Time;

			if (!filesystemStudyStorageEntityBroker.Update(filesystemStorageCritiera, filesystemStorageUpdate))
			{
				Platform.Log(LogLevel.Error, "Unable to update FilesystemQueue row: Study {0}, Server Entity {1}",
							 _location.StudyInstanceUid, _location.ServerPartitionKey);

			}
			else if (!studyStorageBroker.Update(_location.GetKey(), studyStorageUpdate))
			{
				Platform.Log(LogLevel.Error, "Unable to update StudyStorage row: Study {0}, Server Entity {1}",
							 _location.StudyInstanceUid, _location.ServerPartitionKey);
			}
			else
			{
				// Update the location, so the next time we come in here, we don't try and update the database
				// for another sop in the study.
				_location.StudyStatusEnum = statusEnum;
				_location.TransferSyntaxUid = fileSyntax.UidString;
				_location.ServerTransferSyntaxKey = serverSyntax.Key;
			}
		}
	}
}
