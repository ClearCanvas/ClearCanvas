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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Command.Archiving
{
	/// <summary>
	/// Command to archive a specified study in a specified archive
	/// </summary>
	public sealed class ArchiveStudyCommand : AggregateCommand
	{
		
		private XmlDocument _archiveXml;
		private readonly StudyStorageLocation _storageLocation;
		private readonly string _hsmPath;
		private StudyXml _studyXml;
		private readonly string _tempPath;
		private readonly PartitionArchive _archive;
		private string _zipFilename;

		
		public ArchiveStudyCommand(StudyStorageLocation storageLocation, string hsmPath, string tempPath, PartitionArchive archive)
		{
			_storageLocation = storageLocation;
			_hsmPath = hsmPath;
			_tempPath = tempPath;
			_archive = archive;

			CreateSubCommands();
		}

		public string OutputZipFilePath
		{
			get { return _zipFilename; }
		}

		public bool ForceCompress { get; set; }

		public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

		private void CreateSubCommands()
		{
			_archiveXml = new XmlDocument();
			_studyXml = _storageLocation.LoadStudyXml();
			string studyFolder = _storageLocation.GetStudyPath();

			// Create the study date folder
			_zipFilename = Path.Combine(_hsmPath, _storageLocation.StudyFolder);
			AddSubCommand(new CreateDirectoryCommand(_zipFilename));

			// Create a folder for the study
			_zipFilename = Path.Combine(_zipFilename, _storageLocation.StudyInstanceUid);
			AddSubCommand(new CreateDirectoryCommand(_zipFilename));

			// Save the archive data in the study folder, based on a filename with a date / time stamp
			string filename = String.Format("{0}.zip", Platform.Time.ToString("yyyy-MM-dd-HHmm"));
			_zipFilename = Path.Combine(_zipFilename, filename);


			// Create the Xml data to store in the ArchiveStudyStorage table telling
			// where the archived study is located.
			XmlElement hsmArchiveElement = _archiveXml.CreateElement("HsmArchive");
			_archiveXml.AppendChild(hsmArchiveElement);
			XmlElement studyFolderElement = _archiveXml.CreateElement("StudyFolder");
			hsmArchiveElement.AppendChild(studyFolderElement);
			studyFolderElement.InnerText = _storageLocation.StudyFolder;
			XmlElement filenameElement = _archiveXml.CreateElement("Filename");
			hsmArchiveElement.AppendChild(filenameElement);
			filenameElement.InnerText = filename;
			XmlElement studyInstanceUidElement = _archiveXml.CreateElement("Uid");
			hsmArchiveElement.AppendChild(studyInstanceUidElement);
			studyInstanceUidElement.InnerText = _storageLocation.StudyInstanceUid;

			// Create the Zip file
			var zipStudyCommand = new CreateStudyZipCommand(_zipFilename, _studyXml, studyFolder, _tempPath)
			{
				ForceCompress = this.ForceCompress
			};
			zipStudyCommand.ProgressUpdated += (s, e) => EventsHelper.Fire(this.ProgressUpdated, this, e);
			AddSubCommand(zipStudyCommand);

			// Update the database.
			AddSubCommand(new InsertArchiveStudyStorageCommand(_storageLocation.GetKey(), _archive.GetKey(), _storageLocation.ServerTransferSyntaxKey, _archiveXml));
		}
	}

	/// <summary>
	/// Command to insert an <see cref="ArchiveStudyStorage"/> record
	/// </summary>
	class InsertArchiveStudyStorageCommand : ServerDatabaseCommand
	{
		private readonly ServerEntityKey _studyStorageKey;
		private readonly ServerEntityKey _partitionArchiveKey;
		private readonly ServerEntityKey _serverTransferSyntaxKey;
		private readonly XmlDocument _archiveXml;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="studyStorageKey">The <see cref="StudyStorage"/> table key.</param>
		/// <param name="partitionArchiveKey">The <see cref="PartitionArchive"/> table key.</param>
		/// <param name="serverTransferSyntaxKey">The <see cref="ServerTransferSyntax"/> entity key.</param>
		/// <param name="archiveXml">XML Specific archive data to be stored in the <see cref="ArchiveStudyStorage"/> table.</param>
		public InsertArchiveStudyStorageCommand(ServerEntityKey studyStorageKey,
												ServerEntityKey partitionArchiveKey,
												ServerEntityKey serverTransferSyntaxKey,
												XmlDocument archiveXml)
			: base("Insert ArchiveStudyStorage")
		{
			_studyStorageKey = studyStorageKey;
			_partitionArchiveKey = partitionArchiveKey;
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

			insertBroker.Insert(columns);
		}
	}

}