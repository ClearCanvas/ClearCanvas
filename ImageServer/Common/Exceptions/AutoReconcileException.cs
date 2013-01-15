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

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that occured during auto-reconciliation
    /// is nearline.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class AutoReconcileException : Exception
    {
        public AutoReconcileException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Represents an exception that occured when the target study during auto-reconciliation
    /// is not in the right state (eg, it is lossy compressed but has been archived as lossless).
    /// </summary>
    public class TargetStudyInvalidStateException: AutoReconcileException
    {
        public TargetStudyInvalidStateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// The Study Instance UID of the study that causes the issue.
        /// </summary>
        public string StudyInstanceUid { get; set; }
    }
}