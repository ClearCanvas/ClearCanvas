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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
	/// <summary>
	/// An alternate implementation of <see cref="RenameFileCommand"/> that trades robustness for performance gains.
	/// </summary>
	public class FastRenameFileCommand : CommandBase, IDisposable
	{
		#region Private Members

		private readonly string _sourceFile;
		private readonly string _destinationFile;
		private string _destBackupFile;
		private readonly bool _failIfExists;
		private bool _sourceRenamed;

		#endregion

		public FastRenameFileCommand(string sourceFile, string destinationFile, bool failIfExists)
			: base(String.Format("Rename {0} to {1}", sourceFile, destinationFile), true)
		{
			Platform.CheckForNullReference(sourceFile, "Source filename");
			Platform.CheckForNullReference(destinationFile, "Destination filename");

			_sourceFile = sourceFile;
			_destinationFile = destinationFile;
			_failIfExists = failIfExists;
		}

		private string GetBackupPath(string actualPath)
		{
			var count = 0;

			var baseName = Path.Combine(ProcessorContext.BackupDirectory, Path.GetFileName(actualPath) ?? new Guid().ToString());
			var path = baseName + ".bak";

			while (File.Exists(path))
			{
				DateTime creationTime = File.GetCreationTime(path);
				DateTime currentTime = Platform.Time;

				// Arbitrary check of 12 hour old file.  if the file is more than 12 hours old,
				// we're assuming its an orphan, and an error occured when creating, so it can 
				// be overwritten.
				if (creationTime < currentTime.AddHours(-12.0d))
				{
					try
					{
						FileUtils.Delete(path);
						return path;
					}
					catch (IOException)
					{
						// if it ultimately just fails to delete anyway, try another one
					}
				}

				count++;
				path = String.Format("{0}.({1}).bak", baseName, count);
			}

			return path;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			Platform.CheckTrue(File.Exists(_sourceFile), String.Format("Source file '{0}' doesn't exist", _sourceFile));

			if (_failIfExists && File.Exists(_destinationFile))
			{
				throw new ApplicationException(String.Format("Destination file already exists: {0}", _destinationFile));
			}

			_destBackupFile = RequiresRollback ? GetBackupPath(_destinationFile) : null;

			FileUtils.TrySetFileReadOnly(_sourceFile, false);
			FileUtils.TrySetFileReadOnly(_destinationFile, false);
			FileUtils.TrySetFileReadOnly(_destBackupFile, false);
			FileUtils.Replace(_sourceFile, _destinationFile, _destBackupFile);

			_sourceRenamed = true;
		}

		protected override void OnUndo()
		{
			if (!string.IsNullOrEmpty(_destBackupFile) && File.Exists(_destBackupFile))
			{
				try
				{
					// restore destination
					Platform.Log(LogLevel.Debug, "Restoring destination {0}", _destinationFile);
					FileUtils.Replace(_destBackupFile, _destinationFile, _sourceFile);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, "Error occured when rolling back destination file in FastRenameFileCommand: {0}", e.Message);
				}
			}
			else if (_sourceRenamed)
			{
				// only happens if rollback was not specified, so we can't fully undo, just move destination back to source and ignore the lost original destination
				FileUtils.Replace(_destinationFile, _sourceFile, null);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!String.IsNullOrEmpty(_destBackupFile))
			{
				try
				{
					FileUtils.Delete(_destBackupFile);
				}
				catch (IOException) {}
			}
		}

		#endregion
	}
}