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
    public class DeleteDirectoryCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly string _dir;
        private readonly bool _failIfError;
        private bool _sourceDirRenamed;
        private readonly TimeSpanStatistics _deleteTime = new TimeSpanStatistics("DeleteDirectoryTime");
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether a log will be 
        /// generated when the specified directory is deleted.
        /// </summary>
        public bool Log { get; set; }

        #region Constructors

        public DeleteDirectoryCommand(string dir, bool failIfError)
            : base(String.Format("DeleteDirectory {0}", dir), true)
        {
            _dir = dir;
            _failIfError = failIfError;
        }

		public DeleteDirectoryCommand(string dir, bool failIfError, bool deleteOnlyIfEmpty)
			: base(String.Format("DeleteDirectory {0}", dir), true)
		{
			_dir = dir;
			_failIfError = failIfError;
			DeleteOnlyIfEmpty = deleteOnlyIfEmpty;
		}

		/// <summary>
		/// Gets the time spent on deleting the directory.
		/// </summary>
        public TimeSpanStatistics DeleteTime
        {
            get { return _deleteTime; }
        }

        public bool DeleteOnlyIfEmpty { get; set; }

        #endregion

        #region Overridden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
        {
			try
			{
				if (DeleteOnlyIfEmpty && !DirectoryUtility.IsEmpty(_dir))
				{
					return;
				}

				if (Log)
					Platform.Log(LogLevel.Info, "Deleting {0}", _dir);

				Directory.Move(_dir, _dir + ".deleted");
				_sourceDirRenamed = true;

			}
			catch (DirectoryNotFoundException e)
			{
				// ignore this error since we want to delete the directory anyway
			}
            catch (Exception ex)
            {
                if (_failIfError)
                    throw;
            	// ignore it
            	Platform.Log(LogLevel.Warn, ex, "Unexpected exception occurred when deleting {0}. It is ignored.", _dir);
            }
        }

        protected override void OnUndo()
        {
            // the directory has been backed up.. it can be restored
            try
            {
                if (_sourceDirRenamed)
                {
                    Directory.Move(_dir + ".deleted", _dir);
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error occurred while restoring {0}", _dir);
                throw;
            }
            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        	// If not rolling back and the dir was renamed by this command 
        	// then delete it. Otherwise, just leave the ".deleted" directory
            if (!RollBackRequested && _sourceDirRenamed)
            {
                DeleteTime.Start();
                DirectoryUtility.DeleteIfExists(_dir + ".deleted"); 
                DeleteTime.End();
            }
        }

        #endregion
    }
}
