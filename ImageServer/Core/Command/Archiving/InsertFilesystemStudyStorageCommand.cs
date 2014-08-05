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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Command.Archiving
{
	/// <summary>
	/// Command to insert a <see cref="FilesystemStudyStorage"/> record in the database
	/// and update the Study status.
	/// </summary>
	public sealed class InsertFilesystemStudyStorageCommand : ServerDatabaseCommand
	{
		private readonly ServerEntityKey _serverPartitionKey;
		private readonly string _studyInstanceUid;
		private readonly string _folder;
		private readonly ServerEntityKey _filesystemKey;
		private readonly TransferSyntax _transfersyntax;
		private StudyStorageLocation _location;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="serverPartitionKey">The <see cref="ServerPartition"/> the study belongs to.</param>
		/// <param name="studyInstanceUid">The Study</param>
		/// <param name="folder">The folder (typically the study date) where the study is stored.</param>
		/// <param name="filesystemKey">The filesystem the study is stored on.</param>
		/// <param name="transferSyntax">The <see cref="TransferSyntax"/> of the study.</param>
		public InsertFilesystemStudyStorageCommand(ServerEntityKey serverPartitionKey,
													 string studyInstanceUid,
													 string folder,
													 ServerEntityKey filesystemKey,
													 TransferSyntax transferSyntax)
			: base("Insert FilesystemStudyStorage")
		{
			_serverPartitionKey = serverPartitionKey;
			_studyInstanceUid = studyInstanceUid;
			_folder = folder;
			_filesystemKey = filesystemKey;
			_transfersyntax = transferSyntax;
		}

		/// <summary>
		/// The <see cref="StudyStorageLocation"/> inserted.
		/// </summary>
		public StudyStorageLocation Location
		{
			get { return _location; }
		}

	    /// <summary>
	    /// Execute the insert.
	    /// </summary>
	    /// <param name="theProcessor">The command processor calling us</param>
	    /// <param name="updateContext">The persistent store connection to use for the update.</param>
	    protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			var locInsert = updateContext.GetBroker<IInsertStudyStorage>();
	        var insertParms = new InsertStudyStorageParameters
	                              {
	                                  ServerPartitionKey = _serverPartitionKey,
	                                  StudyInstanceUid = _studyInstanceUid,
	                                  Folder = _folder,
	                                  FilesystemKey = _filesystemKey,
	                                  QueueStudyStateEnum = QueueStudyStateEnum.Idle
	                              };

	        if (_transfersyntax.LosslessCompressed)
			{
				insertParms.TransferSyntaxUid = _transfersyntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
			}
			else if (_transfersyntax.LossyCompressed)
			{
				insertParms.TransferSyntaxUid = _transfersyntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
			}
			else
			{
                insertParms.TransferSyntaxUid = _transfersyntax.UidString;
				insertParms.StudyStatusEnum = StudyStatusEnum.Online;
			}

			// Find one so we don't uselessly process all the results.
			_location = locInsert.FindOne(insertParms);
		}
	}
}
