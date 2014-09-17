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
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Command
{
	/// <summary>
	/// Specific exception if a file exists when saving.
	/// </summary>
	public class InstanceAlreadyExistsException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">Message describing the exception.</param>
		public InstanceAlreadyExistsException(string message)
			: base(message)
		{}

        /// <summary>
        /// The Series Instance UID of the existing instance.
        /// </summary>        
		public string SeriesInstanceUid { get; set; }
		
        /// <summary>
        /// The SOP Instance UID of the existing instance.
        /// </summary>
		public string SopInstanceUid { get; set; }
	}

	/// <summary>
	/// Class for saving a DicomFile instance in memory to disk.
	/// </summary>
	public class SaveDicomFileCommand : CommandBase, IDisposable, IAggregateCommand
	{
        public delegate string GetFilePathDelegateMethod();

		#region Private Members
		private string _path;
        private string _backupPath;
		private readonly DicomFile _file;
		private readonly bool _failOnExists;
		private bool _fileCreated;
		private readonly StudyStorageLocation _storageLocation;
	    readonly RateStatistics _backupSpeed = new RateStatistics("BackupSpeed", RateType.BYTES);
	    readonly RateStatistics _saveSpeed = new RateStatistics("SaveSpeed", RateType.BYTES);
		private readonly Stack<ICommand> _aggregateStack = new Stack<ICommand>();
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to save the file.</param>
		/// <param name="file">The file to save.</param>
		/// <param name="failOnExists">If the file already exists, the file will save.</param>
		public SaveDicomFileCommand(string path, DicomFile file, bool failOnExists)
			: base("Save DICOM Message", true)
		{
			Platform.CheckForNullReference(path, "File name");
			Platform.CheckForNullReference(file, "Dicom File object");

			_path = path;
			_file = file;
			_failOnExists = failOnExists;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="file">The file to save.</param>
		/// <param name="failOnExists">If the file already exists, the file will save.</param>
		/// <param name="location">The storage location for the file.</param>
		public SaveDicomFileCommand(StudyStorageLocation location, DicomFile file, bool failOnExists)
			: base("Save DICOM Message", true)
		{
			Platform.CheckForNullReference(file, "Dicom File object");
			_storageLocation = location;
			_path = null;
			_file = file;
			_failOnExists = failOnExists;
		}

		public Stack<ICommand> AggregateCommands
		{
			get { return _aggregateStack; }
		}

	    private void Backup()
	    {
            if (File.Exists(_path))
            {
				if (_failOnExists)
					throw new InstanceAlreadyExistsException(String.Format("DICOM File unexpectedly already exists: {0}", _path));
				try
				{
                    _backupSpeed.Start();
                    var fi = new FileInfo(_path);
                    _backupPath = FileUtils.Backup(_path, ProcessorContext.BackupDirectory);
                    _backupSpeed.SetData(fi.Length);
                    _backupSpeed.End();
				}
				catch (IOException)
				{
					_backupPath = null;
					throw;
				}
            }
	    }
		private string GetTempPath()
		{
			int count = 0;

		    string path = String.Format("{0}_tmp", _path);

			while (File.Exists(path))
			{
				DateTime creationTime = File.GetCreationTime(path);
				DateTime currentTime = Platform.Time;
				// Arbitrary check of 12 hour old file.  if the file is more than 12 hours old,
				// we're assuming its an orphan, and an error occured when creating, so it can 
				// be overwritten.
				if (creationTime < currentTime.AddHours(-12.0d))
				{
					//TODO: FileUtils.Delete does throw exceptions ... shouldn't we just keep iterating?
					FileUtils.Delete(path);
					return path;
				}

				count++;
				path = String.Format("{0}_{1}tmp", _path, count);
			}

			return path;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			if (_path == null)
			{
				String seriesUid = _file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
				String sopUid = _file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
				_path = _storageLocation.GetSopInstancePath(seriesUid, sopUid);
			}

			// Make sure the directory exists where we're storing the file.
		    var p = Path.GetDirectoryName(_path);
			if (string.IsNullOrEmpty(p) || !Directory.Exists(p))
			{
				if (!theProcessor.ExecuteSubCommand(this, new CreateDirectoryCommand(Path.GetDirectoryName(_path))))
					throw new ApplicationException(theProcessor.FailureReason);
			}

            if (RequiresRollback)
                Backup();

	        string path = GetTempPath();

	    	using (FileStream stream = FileStreamOpener.OpenForSoleUpdate(path, FileMode.Create))
			{
				// Set _fileCreated here, because the file has been opened.
                _saveSpeed.Start();
				_file.Save(stream, DicomWriteOptions.Default);
				stream.Flush();
				stream.Close();
                _saveSpeed.End();

                var fi = new FileInfo(path);
                _saveSpeed.SetData(fi.Length);
                
			}

				if (File.Exists(_path))
				{
				    if (_failOnExists)
					{
						try
						{
							FileUtils.Delete(path);
						}
						catch (Exception x)
						{
	                    throw new ApplicationException(
	                        String.Format("DICOM File unexpectedly already exists: {0}", _path), x);
						}
						throw new ApplicationException(String.Format("DICOM File unexpectedly already exists: {0}", _path));
					}
				}

	        FileUtils.Copy(path, _path, true);
				_fileCreated = true;
	        FileUtils.Delete(path);
		}

		protected override void OnUndo()
		{
            if (false == String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                // restore original file
                File.Copy(_backupPath, _path, true);
                FileUtils.Delete(_backupPath);
                _backupPath = null;
            }
            else if (_fileCreated)
                FileUtils.Delete(_path);
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (false == String.IsNullOrEmpty(_backupPath))
            {
                FileUtils.Delete(_backupPath);
            }
        }

        #endregion
    }
}