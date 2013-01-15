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
    /// Defines a tool.
    /// </summary>
    /// <remarks>
	/// Developers are encouraged to subclass <see cref="Tool{TContextInterface}"/> 
	/// or one of its subclasses rather than implement this interface directly.
	/// </remarks>
    public interface ITool : IDisposable
    {
        /// <summary>
        /// Called by the framework to set the tool context.
        /// </summary>
        void SetContext(IToolContext context);

        /// <summary>
        /// Called by the framework to allow the tool to initialize itself.
        /// </summary>
        /// <remarks>
		/// This method will be called after <see cref="SetContext"/> has been called, 
		/// which guarantees that the tool will have access to its context when this method is called.
		/// </remarks>
        void Initialize();

        /// <summary>
        /// Gets the set of actions that act on this tool.
        /// </summary>
        /// <remarks>
		/// This property is not guaranteed to be a dynamic property - that is, you should not assume
		/// this property will always return a different set of actions depending on the internal state 
		/// of the tool.  The class that owns the tool decides when to access this property, and 
		/// whether or not the actions can be dynamic will be dependent on the implementation of that class.
		/// </remarks>
        IActionSet Actions { get; }
    }
}
