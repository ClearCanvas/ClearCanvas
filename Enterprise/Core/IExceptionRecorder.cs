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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines an interface for creating an <see cref="ExceptionLogEntry"/> that records
    /// information about an exception.
    /// </summary>
    public interface IExceptionRecorder
    {
        /// <summary>
        /// Creates a <see cref="ExceptionLogEntry"/> for the specified operation and exception.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="e">The exception that was thrown.</param>
        /// <returns></returns>
        ExceptionLogEntry CreateLogEntry(string operation, Exception e);
    }
}
