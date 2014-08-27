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
    public class CopyDirectoryCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly RateStatistics _copySpeed = new RateStatistics("CopySpeed", RateType.BYTES);
        private readonly string _src;
        private readonly string _dest;
        private readonly DirectoryUtility.CopyProcessCallback _callback;
        private readonly TimeSpanStatistics _backupTime = new TimeSpanStatistics();
        private bool _copied;
        private string _backupDestDir; 
        #endregion

        #region Constructors
        public CopyDirectoryCommand(string src, string dest, DirectoryUtility.CopyProcessCallback callback)
            : base(String.Format("CopyDirectory {0}", src), true)
        {
            _src = src;
            _dest = dest;
            _callback = callback;
        } 
        #endregion

        #region Public Properties

        public RateStatistics CopySpeed
        {
            get { return _copySpeed; }
        }
        public TimeSpanStatistics BackupTime
        {
            get { return _backupTime; }
        } 
        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (!Directory.Exists(_src))
                throw new DirectoryNotFoundException(string.Format("Source directory {0} does not exist", _src));

            if (RequiresRollback)
            {
                Backup();
            }


            CopySpeed.Start();
            _copied = true;
            ulong bytesCopied = DirectoryUtility.Copy(_src, _dest, _callback);
            CopySpeed.SetData(bytesCopied);
            CopySpeed.End();
        }


        protected override void OnUndo()
        {
            if (_copied)
            {
                if (!string.IsNullOrEmpty(_backupDestDir) && Directory.Exists(_backupDestDir))
                {
                    try
                    {
                        DirectoryUtility.DeleteIfExists(_dest);
                    }
                    catch (Exception ex)
                    {
                    	// ignore it, will overwrite anyway
                        Platform.Log(LogLevel.Warn, "Unexpected exeception attempting to delete on undo '{0}': {1}", _dest, ex.Message);
                    }

                    // restore
                    try
                    {
                        DirectoryUtility.Copy(_backupDestDir, _dest);
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Warn, "Error occurred when rolling back CopyDirectoryCommand: {0}", ex.Message);
                    }
                }
            }
        } 
        #endregion

        #region Private Members

        private void Backup()
        {
            if (Directory.Exists(_dest))
            {
                BackupTime.Start();
                _backupDestDir = Path.Combine(ProcessorContext.BackupDirectory, "DestFolder");

                Platform.Log(LogLevel.Info, "Backing up original destination folder {0}", _dest);
				// Will create the destination folder, if need be.
				DirectoryUtility.Copy(_dest, _backupDestDir);
                Platform.Log(LogLevel.Info, "Original destination folder {0} is backed up to {1}", _dest, _backupDestDir);
                BackupTime.End();
            }
        } 
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
				if (!string.IsNullOrEmpty(_backupDestDir))
					DirectoryUtility.DeleteIfExists(_backupDestDir);
            }
            catch (Exception x)
            {
            	//ignore
				Platform.Log(LogLevel.Warn, "Unexpected exeception attempting to delete backup directory '{0}': {1}", _backupDestDir, x.Message);
            }
        }

        #endregion
    }
}