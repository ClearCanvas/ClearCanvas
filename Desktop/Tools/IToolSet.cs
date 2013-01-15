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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Defines the external interface to a tool set, which manages a set of tools.
    /// </summary>
    public interface IToolSet : IDisposable
    {
        /// <summary>
        /// Gets the tools contained in this tool set.
        /// </summary>
        ITool[] Tools { get; }

		/// <summary>
		/// Finds the tool of the specified type.
		/// </summary>
		/// <typeparam name="TTool"></typeparam>
		/// <returns>The instance of the tool of the specified type, or null if no such exists.</returns>
		TTool Find<TTool>()
			where TTool: ITool;
        
        /// <summary>
        /// Returns the union of all actions defined by all tools in this tool set.
        /// </summary>
        IActionSet Actions { get; }
    }
}
