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
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
    /// <summary>
    /// Context object used by the <see cref="ViewerCommandProcessor"/>.
    /// </summary>
    public class ViewerCommandProcessorContext : ICommandProcessorContext
    {
        private bool _disposed;
        private DataAccessContext _context;

        public DataAccessContext DataAccessContext
        {
            get { return _context ?? (_context = new DataAccessContext(DataAccessContext.WorkItemMutex)); }
        }

        public Study ContextStudy { get; set; }

        public ViewerCommandProcessorContext()
        {
            BackupDirectory = Path.Combine(Platform.ApplicationDataDirectory, "TemporaryFiles");
            Directory.CreateDirectory(BackupDirectory);
        }

        public void Dispose()
        {
            if (_disposed)
                throw new InvalidOperationException("Already disposed.");
            
            _disposed = true;
            
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public void PreExecute(ICommand command)
        {
            var dataAccessComand = command as DataAccessCommand;
            if (dataAccessComand != null)
                dataAccessComand.DataAccessContext = DataAccessContext;
        }

        public void Commit()
        {
            if (DataAccessContext == null) 
                throw new ApplicationException("Unable to commit, no DataAccessContext.");

            DataAccessContext.Commit();
        }

        public void Rollback()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public string TempDirectory
        {
            get { return BackupDirectory; }
        }

        public string BackupDirectory
        {
            get;
            set;
        }
    }
}
