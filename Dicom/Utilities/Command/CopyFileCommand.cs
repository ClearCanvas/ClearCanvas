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

namespace ClearCanvas.Dicom.Utilities.Command
{
    public class CopyFileCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly string _sourceFile;
        private readonly string _destinationFile;
        private string _destBackupFile;
        private readonly bool _failIfExists;
        #endregion

        public CopyFileCommand(string sourceFile, string destinationFile, bool failIfExists)
            : base(String.Format("Copy {0} to {1}", sourceFile, destinationFile), true)
        {
            Platform.CheckForNullReference(sourceFile, "Source filename");
            Platform.CheckForNullReference(destinationFile, "Destination filename");

            _sourceFile = sourceFile;
            _destinationFile = destinationFile;
            _failIfExists = failIfExists;
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            Platform.CheckTrue(File.Exists(_sourceFile), String.Format("Source file '{0}' doesn't exist", _sourceFile));

            if (File.Exists(_destinationFile))
            {
                if (_failIfExists)
                    throw new ApplicationException(String.Format("Destination file already exists: {0}", _destinationFile));
            }

            if (RequiresRollback)
                Backup();

            if (File.Exists(_destinationFile))
                FileUtils.Copy(_sourceFile, _destinationFile, !_failIfExists);
            else
                FileUtils.Copy(_sourceFile, _destinationFile, false);

            try
            {
                if ((File.GetAttributes(_destinationFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(_destinationFile, FileAttributes.Normal);
            }
            catch (Exception)
            { }
        }

        private void Backup()
        {
            if (File.Exists(_destinationFile))
            {
                _destBackupFile = FileUtils.Backup(_destinationFile, ProcessorContext.BackupDirectory);
            }
        }

        protected override void OnUndo()
        {
            // restore destination
            if (File.Exists(_destBackupFile))
            {
                try
                {
                    Platform.Log(LogLevel.Debug, "Restoring failed file: {0}", _destinationFile);
                    FileUtils.Copy(_destBackupFile, _destinationFile, true);
                    FileUtils.Delete(_destBackupFile);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, "Error occured when rolling back destination file in CopyFileCommand: {0}", e.Message);
                }
            }
        }

        #region IDisposable Members

	    public void Dispose()
	    {
		    try
		    {
			    FileUtils.Delete(_destBackupFile);
		    }
		    catch (Exception)
		    {
		    }
	    }

	    #endregion
    }
}
