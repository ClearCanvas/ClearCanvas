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
	/// <summary>
	/// <see cref="CommandBase"/> for deleting a file, not that there is no rollback.  Rollback
	/// should be accomplished by other means.  IE, do a rename of the file, then delete when everything
	/// else is done.
	/// </summary>
	public class FileDeleteCommand : CommandBase, IDisposable
	{
        private string _backupFile;
        private bool _backedUp;
        private readonly string _originalFile;

		public FileDeleteCommand(string path, bool requiresRollback) 
            : base(String.Format("Delete {0}", path), requiresRollback)
		{
            Platform.CheckForNullReference(path, "path");
			_originalFile = path;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
            if (RequiresRollback)
            {
                Backup();
            }

			if (File.Exists(_originalFile))
			{
			    FileUtils.Delete(_originalFile);
			}
            else
			{
			    Platform.Log(LogLevel.Warn, "Attempted to delete file which doesn't exist: {0}", _originalFile);
			}
		}

	    private void Backup()
	    {
            if (File.Exists(_originalFile))
            {
                _backupFile = FileUtils.Backup(_originalFile, ProcessorContext.BackupDirectory);
                _backedUp = true;
            }            
	    }

	    protected override void OnUndo()
		{
			if (_backedUp)
			{
			    try
			    {
                    Platform.Log(LogLevel.Debug, "Restoring {0} ...", _originalFile);
                    File.Copy(_backupFile, _originalFile, true);
                    FileUtils.Delete(_backupFile);
			    }
			    catch (Exception e)
			    {
                    Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to backup deleted file: {0}", _backupFile);
			    }
			}
		}

        public override string ToString()
        {
            return String.Format("Delete file {0}", _originalFile);
        }

        #region IDisposable Members

		public void Dispose()
		{
			// Will check for existance
			FileUtils.Delete(_backupFile);
		}

		#endregion
    }
}