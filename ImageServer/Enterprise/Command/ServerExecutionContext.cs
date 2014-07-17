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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Enterprise.Command
{
    /// <summary>
    /// Represents the execution context of the current operation.
    /// </summary>
    public class ServerExecutionContext: ICommandProcessorContext
    {
        #region Private Fields

        [ThreadStatic]
        private static ServerExecutionContext _current;
        protected string _backupDirectory;
        protected string _tempDirectory;
        private readonly object _sync = new object();
        private readonly ServerExecutionContext _inheritFrom;
        private IReadContext _readContext;
        private readonly string _contextId;
        private IUpdateContext _updateContext;

        private bool _ownsUpdateContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an execution scope in current thread
        /// where persistence context can be re-used.
        /// </summary>
        public ServerExecutionContext()
            : this(Guid.NewGuid().ToString())
        {

        }


        /// <summary>
        /// Creates an instance of <see cref="ServerExecutionContext"/> inherited
        /// the current ExecutionContext in the thread, if it exists. This context
        /// becomes the current context of the thread until it is disposed.
        /// </summary>
        public ServerExecutionContext(String contextId)
            : this(contextId, Current)
        {
         
        }

        public ServerExecutionContext(String contextId, ServerExecutionContext inheritFrom)
        {
            Platform.CheckForNullReference(contextId, "contextId");
            
            _contextId = contextId;
            _inheritFrom = inheritFrom;
            _current = this;
        }

        #endregion

        #region Virtual Protected Methods

        protected virtual string GetTemporaryPath()
        {
			string path;
			if (_inheritFrom != null)
            {
				path = Path.Combine(_inheritFrom.TempDirectory, _contextId);

				for (int i = 1; i <= _contextId.Length; i++)
				{
					path = Path.Combine(_inheritFrom.TempDirectory, _contextId.Substring(0, i));
					if (!Directory.Exists(path))
						break;
				}
            }
            else
            {
				// if specified in the config, use it
				string tempDir = !String.IsNullOrEmpty(ImageServerCommonConfiguration.TemporaryPath)
					                 ? ImageServerCommonConfiguration.TemporaryPath
					                 : // Use the OS temp folder instead, assuming it's not too long.
                // Avoid creating a temp folder under the installation directory because it could
                // lead to PathTooLongException.
					                 Path.Combine(Path.GetTempPath(), "ImageServer");

            // make sure it exists
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

			    path = Path.Combine(tempDir, _contextId);
		    }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        #endregion

        protected object SyncRoot
        {
            get { return _sync; }
        }

        public String TempDirectory
        {
            get { return _tempDirectory ?? (_tempDirectory = GetTemporaryPath()); }
        }

        public virtual String BackupDirectory
        {
            get
            {
                if (_backupDirectory == null)
                {
                    lock (SyncRoot)
                    {
                        String baseBackupDirectory = Path.Combine(TempDirectory, "BackupFiles");
                        _backupDirectory = baseBackupDirectory;
                    }
                }

                if (!Directory.Exists(_backupDirectory))
                    Directory.CreateDirectory(_backupDirectory);

                return _backupDirectory;
            }
            set
            {
                _backupDirectory = value;
            }
        }

        public static ServerExecutionContext Current
        {
            get { return _current; }
        }

        public IPersistenceContext ReadContext
        {
            get
            {
                lock(SyncRoot)
                {
					if (_inheritFrom != null)
						return _inheritFrom.ReadContext;

                    if (_readContext == null)
                        _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
                }

                return _readContext;
            }
        }

        public IPersistenceContext PersistenceContext
        {
            get
            {
            	if (_updateContext != null)
                    return _updateContext;

            	return ReadContext;
            }
        }

		public ServerEntityKey PrimaryServerPartitionKey { get; set; }
		public ServerEntityKey PrimaryStudyKey { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (!DirectoryUtility.DeleteIfEmpty(_backupDirectory))
                {
                    Platform.Log(LogLevel.Warn, "Some backup files can be found left in {0}", BackupDirectory);
                }

                if (Platform.IsLogLevelEnabled(LogLevel.Debug) && Directory.Exists(_tempDirectory))
                    Platform.Log(LogLevel.Debug, "Deleting temp folder: {0}", _tempDirectory);
                
                DirectoryUtility.DeleteIfEmpty(_tempDirectory);
            }
            finally
            {
                if (_updateContext != null)
                    Rollback();

            	if (_readContext!=null)
            	{
            		_readContext.Dispose();
            		_readContext = null;
            	}
                // reset the current context for the thread
                _current = _inheritFrom;
            }            
        }

        #endregion

        protected static string GetTempPathRoot()
        {
            String basePath = String.Empty;
            if (!String.IsNullOrEmpty(ImageServerCommonConfiguration.TemporaryPath))
            {
                if (Directory.Exists(ImageServerCommonConfiguration.TemporaryPath))
                    basePath = ImageServerCommonConfiguration.TemporaryPath;
                else
                {
                    // try to create it
                    try
                    {
                        Directory.CreateDirectory(ImageServerCommonConfiguration.TemporaryPath);
                    }
                    catch(Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex);
                    }
                }
            }
            return basePath;
        }

        public void PreExecute(ICommand command)
        {
            var dbCommand = command as ServerDatabaseCommand;
            if (dbCommand!= null)
            {
                    if (_updateContext == null)
                    {
                        _updateContext =
                            PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
                        _ownsUpdateContext = true;
                    }

                dbCommand.UpdateContext = _updateContext;
            }            
        }

        public void Commit()
        {
            if (_updateContext != null)
            {
                _updateContext.Commit();
                _updateContext.Dispose();
                _updateContext = null;
            }
        }

        public void Rollback()
        {
            if (_updateContext != null && _ownsUpdateContext)
            {
                _updateContext.Dispose(); // Rollback the db
                _updateContext = null;
            }
        }
    }
}