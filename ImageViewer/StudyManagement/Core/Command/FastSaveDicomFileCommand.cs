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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
	/// <summary>
	/// An alternate implementation of <see cref="SaveDicomFileCommand"/> that trades robustness for performance gains.
	/// </summary>
	public class FastSaveDicomFileCommand : CommandBase, IAggregateCommand, IDisposable
	{
		#region Private Members

		private readonly Stack<ICommand> _aggregateStack = new Stack<ICommand>();

		private readonly string _path;
		private readonly DicomFile _file;
		private string _backupPath;
		private readonly bool _failOnExists;
		private bool _fileCreated;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="file">The file to save.</param>
		/// <param name="failOnExists">If the file already exists, the file will save.</param>
		/// <param name="path">The path for the file.</param>
		public FastSaveDicomFileCommand(string path, DicomFile file, bool failOnExists)
			: base("Save DICOM File", true)
		{
			Platform.CheckForNullReference(file, "Dicom File object");
			Platform.CheckForNullReference(path, "Path to save file");

			_path = path;
			_file = file;
			_failOnExists = failOnExists;
		}

		public Stack<ICommand> AggregateCommands
		{
			get { return _aggregateStack; }
		}

		private string GetBackupPath()
		{
			var count = 0;

			var baseName = Path.Combine(ProcessorContext.BackupDirectory, Path.GetFileName(_path) ?? new Guid().ToString());
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

		private string GetTempPath()
		{
			int count = 0;

			string path = String.Concat(_path, "_tmp");

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
				path = String.Format("{0}_{1}tmp", _path, count);
			}

			return path;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			// Make sure the directory exists where we're storing the file.
			var p = Path.GetDirectoryName(_path);
			if (string.IsNullOrEmpty(p) || !Directory.Exists(p))
			{
				if (!theProcessor.ExecuteSubCommand(this, new CreateDirectoryCommand(Path.GetDirectoryName(_path))))
					throw new ApplicationException(theProcessor.FailureReason);
			}

			string path = GetTempPath();

			var fileMode = _failOnExists ? FileMode.CreateNew : FileMode.Create;
			using (var stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None, 65536, FileOptions.SequentialScan))
			{
				_file.Save(stream, DicomWriteOptions.Default);
				stream.Flush();
				stream.Close();
			}

			if (_failOnExists && File.Exists(_path))
			{
				// Do check *after* creating the temp file in case another thread received file at the same time (and finished before us!)
				try
				{
					// Delete the temp file we saved
					FileUtils.Delete(path);
				}
				catch (Exception x)
				{
					throw new ApplicationException(String.Format("DICOM File unexpectedly already exists: {0}", _path), x);
				}
				throw new ApplicationException(String.Format("DICOM File unexpectedly already exists: {0}", _path));
			}

			_backupPath = RequiresRollback ? GetBackupPath() : null;

			FileUtils.TrySetFileReadOnly(path, false);
			FileUtils.TrySetFileReadOnly(_path, false);
			FileUtils.TrySetFileReadOnly(_backupPath, false);
			FileUtils.Replace(path, _path, _backupPath);

			_fileCreated = true;
		}

		protected override void OnUndo()
		{
			if (!String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
			{
				try
				{
					// restore original file
					Platform.Log(LogLevel.Debug, "Restoring original {0}", _path);
					FileUtils.Replace(_backupPath, _path, null);
					_backupPath = null;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, "Error occured when rolling back destination file in FastSaveDicomFileCommand: {0}", e.Message);
				}
			}
			else if (_fileCreated)
			{
				// only happens if rollback was not specified, so we can't fully undo, just delete the file
				FileUtils.Delete(_path); // Will check for existance
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!String.IsNullOrEmpty(_backupPath))
			{
				try
				{
					// Will check for existence
					FileUtils.Delete(_backupPath);
				}
				catch (IOException) {}
			}
		}

		#endregion
	}
}