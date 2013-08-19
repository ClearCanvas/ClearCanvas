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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving
{
	/// <summary>
	/// Command to insert an <see cref="ArchiveStudyStorage"/> record and update the <see cref="ArchiveQueue"/>
	/// </summary>
	public class InsertArchiveStudyStorageCommand : ServerDatabaseCommand
	{
		private readonly ServerEntityKey _studyStorageKey;
		private readonly ServerEntityKey _partitionArchiveKey;
		private readonly ServerEntityKey _archiveQueueKey;
		private readonly ServerEntityKey _serverTransferSyntaxKey;
		private readonly XmlDocument _archiveXml;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="studyStorageKey">The <see cref="StudyStorage"/> table key.</param>
		/// <param name="partitionArchiveKey">The <see cref="PartitionArchive"/> table key.</param>
		/// <param name="archiveQueueKey">The <see cref="ArchiveQueue"/> entry key.</param>
		/// <param name="serverTransferSyntaxKey">The <see cref="ServerTransferSyntax"/> entity key.</param>
		/// <param name="archiveXml">XML Specific archive data to be stored in the <see cref="ArchiveStudyStorage"/> table.</param>
		public InsertArchiveStudyStorageCommand(ServerEntityKey studyStorageKey,
		                                        ServerEntityKey partitionArchiveKey,
		                                        ServerEntityKey archiveQueueKey,
		                                        ServerEntityKey serverTransferSyntaxKey,
		                                        XmlDocument archiveXml)
			: base("Insert ArchiveStudyStorage")
		{
			_studyStorageKey = studyStorageKey;
			_partitionArchiveKey = partitionArchiveKey;
			_archiveQueueKey = archiveQueueKey;
			_serverTransferSyntaxKey = serverTransferSyntaxKey;
			_archiveXml = archiveXml;
		}

		/// <summary>
		/// Execute the command
		/// </summary>
		/// <param name="updateContext">Database update context.</param>
		/// <param name="theProcessor">The processor executing the command.</param>
		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
		    var columns = new ArchiveStudyStorageUpdateColumns
		                      {
		                          ArchiveTime = Platform.Time,
		                          PartitionArchiveKey = _partitionArchiveKey,
		                          StudyStorageKey = _studyStorageKey,
		                          ArchiveXml = _archiveXml,
		                          ServerTransferSyntaxKey = _serverTransferSyntaxKey
		                      };


		    var insertBroker = updateContext.GetBroker<IArchiveStudyStorageEntityBroker>();

			ArchiveStudyStorage storage = insertBroker.Insert(columns);


		    var parms = new UpdateArchiveQueueParameters
		                    {
		                        ArchiveQueueKey = _archiveQueueKey,
		                        ArchiveQueueStatusEnum = ArchiveQueueStatusEnum.Completed,
		                        ScheduledTime = Platform.Time,
		                        StudyStorageKey = _studyStorageKey
		                    };


		    var broker = updateContext.GetBroker<IUpdateArchiveQueue>();

            if (!broker.Execute(parms))
                throw new ApplicationException("InsertArchiveStudyStorageCommand failed");
		}
	}
}