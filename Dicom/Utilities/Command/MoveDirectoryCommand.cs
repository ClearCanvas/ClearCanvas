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
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// A ServerCommand derived class for moving a directory.
    /// </summary>
    /// <remark>
    /// If <see cref="CommandBase.RequiresRollback"/> is set to <b>true</b>, the <see cref="MoveDirectoryCommand"/>
    /// will perform necessary backup so that the original source and destination directories can be restored when <see cref="OnUndo"/> is called.
    /// </remark>
    public class MoveDirectoryCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly string _src;
        private readonly string _dest;
        private string _backupDestDir ;
        private readonly TimeSpanStatistics _backupTime = new TimeSpanStatistics();
        private readonly RateStatistics _moveSpeed = new RateStatistics("MoveSpeed", RateType.BYTES);
        private readonly DirectoryUtility.CopyProcessCallback _callback;
        #endregion

        public MoveDirectoryCommand(string src, string dest, DirectoryUtility.CopyProcessCallback callback)
            : base(String.Format("MoveDirectory {0}", src), true)
        {
            Platform.CheckForNullReference(src, "src");
            Platform.CheckForNullReference(dest, "dest");

            _src = src;
            _dest = dest;
            _callback = callback;
        }

        public TimeSpanStatistics BackupTime
        {
            get { return _backupTime; }
        }

        public RateStatistics MoveSpeed
        {
            get { return _moveSpeed; }
        }

		protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (!Directory.Exists(_src))
                throw new DirectoryNotFoundException(string.Format("Source directory {0} does not exist", _src));
            
            if (RequiresRollback)
            {
                Backup();
            }

            MoveSpeed.Start();
            ulong bytesCopied = DirectoryUtility.Copy(_src, _dest, _callback);
            MoveSpeed.SetData(bytesCopied);
            MoveSpeed.End();
        }

        private void Backup()
        {
            if (Directory.Exists(_dest))
            {
                BackupTime.Start();
                _backupDestDir = Path.Combine(ProcessorContext.BackupDirectory, "DestStudy");
                Directory.CreateDirectory(_backupDestDir);
                Platform.Log(LogLevel.Info, "Backing up original destination folder {0}", _dest);
                DirectoryUtility.Copy(_dest, _backupDestDir);
                Platform.Log(LogLevel.Info, "Original destination folder {0} is backed up to {1}", _dest, _backupDestDir);
                BackupTime.End();
            }

        }

        protected override void OnUndo()
        {
            try
            {
                DirectoryUtility.DeleteIfExists(_dest, true);
            }
            finally
            {
                if (Directory.Exists(_backupDestDir))
                {
                    Platform.Log(LogLevel.Info, "Restoring original destination folder {0}", _dest);
                    DirectoryUtility.Copy(_backupDestDir, _dest);
                    Platform.Log(LogLevel.Info, "Original destination folder {0} is restored", _dest);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!RollBackRequested)
            {
                DirectoryUtility.DeleteIfExists(_src);
            }

            DirectoryUtility.DeleteIfExists(_backupDestDir);
        }

        #endregion
    }
}
