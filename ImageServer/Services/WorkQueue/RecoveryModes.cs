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

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// List of recovery mechanisms used by different work queue processors when the entry
    /// is failed because of mismatch number of instances in the study xml and the database.
    /// </summary>
    public enum RecoveryModes
    {
        /// <summary>
        /// Users will handle it manually.
        /// </summary>
        Manual,

        /// <summary>
        /// The server will trigger a reprocess of the study.
        /// </summary>
        Automatic
    }
}