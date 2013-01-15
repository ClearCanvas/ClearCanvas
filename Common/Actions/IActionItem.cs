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

namespace ClearCanvas.Common.Actions
{
    /// <summary>
    /// Performs an action using an implementation specific context.
    /// </summary>
    public interface IActionItem<T>
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">An implementation specific context for the action.</param>
        /// <returns>true on success, false on failure.</returns>
        bool Execute(T context);

        /// <summary>
        /// A descriptive reason for a failure of the action.  This property is populated when <see cref="IActionItem{T}.Execute"/> returns false.
        /// </summary>
        string FailureReason { get; }
    }
}