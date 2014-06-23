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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Class for extracting a file from a zip over the top of another file, and preserving
	/// the old file for restoring on failure.
	/// </summary>
	public class ExtractZipFileAndReplaceCommand : CommandBase, IDisposable
	{
		private readonly string _zipFile;
		private readonly string _destinationFolder;
		private readonly string _sourceFile;
		private bool _fileBackedup;
		private string _storageFile = String.Empty;
		private string _backupFile = String.Empty;

		public ExtractZipFileAndReplaceCommand(string zipFile, string sourceFile, string destinationFolder)
			: base("Extract file from Zip and replace existing file", true)
		{
			_zipFile = zipFile;
			_destinationFolder = destinationFolder;
			_sourceFile = sourceFile;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			_storageFile = Path.Combine(_destinationFolder, _sourceFile);
			_backupFile = Path.Combine(ProcessorContext.TempDirectory, _sourceFile);


			string baseDirectory = _backupFile.Substring(0, _backupFile.LastIndexOfAny(new [] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar }));
			if (!Directory.Exists(baseDirectory))
				Directory.CreateDirectory(baseDirectory);

			if (File.Exists(_storageFile))
			{
				File.Move(_storageFile, _backupFile);
				_fileBackedup = true;
			}
		    var zipService = Platform.GetService<IZipService>();
            using (var zipReader = zipService.OpenRead(_zipFile))
            {
                zipReader.Extract(_sourceFile, _destinationFolder, true);
			}
		}

		protected override void OnUndo()
		{
			if (_fileBackedup)
			{
				if (File.Exists(_storageFile))
					File.Delete(_storageFile);
				File.Move(_backupFile, _storageFile);
				_fileBackedup = false;
			}
		}

		public void Dispose()
		{
			File.Delete(_backupFile);
		}
	}
}
