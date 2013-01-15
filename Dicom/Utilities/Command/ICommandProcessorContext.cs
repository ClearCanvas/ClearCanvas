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

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Interfacce for the context that the <see cref="CommandProcessor"/> is run within.
    /// </summary>
    /// <remarks>
    /// Note that a <see cref="CommandProcessor"/> should only access a single instance of 
    /// ICommandProcessorContext.  Onces the <see cref="CommandProcessor"/> is disposed, the
    /// <see cref="ICommandProcessorContext"/> will also be disposed.
    /// </remarks>
    public interface ICommandProcessorContext : IDisposable
    {
        /// <summary>
        /// Called by the <see cref="CommandProcessor"/> before an <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="command">The command being executed.</param>
        void PreExecute(ICommand command);

        /// <summary>
        /// Called when the <see cref="CommandProcessor"/> commits its <see cref="ICommand"/>s.
        /// </summary>
        void Commit();

        /// <summary>
        /// Called when the <see cref="CommandProcessor"/> rolls back its <see cref="ICommand"/>
        /// </summary>
        void Rollback();

        /// <summary>
        /// Temporary directory path that can be used by <see cref="ICommand"/> instances to store
        /// temporary files.
        /// </summary>
        String TempDirectory { get; }

        /// <summary>
        /// Directory to backup directory that can be used by <see cref="ICommand"/> instances to store backup files.
        /// </summary>
        string BackupDirectory { get; set; }
    }
}
