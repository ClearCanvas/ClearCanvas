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

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Extends the <see cref="IAction"/> interface for actions that have single-click
    /// behaviour, such as menu items and toolbar buttons.
    /// </summary>
    public interface IClickAction : IAction
    {
        /// <summary>
        /// Occurs when the <see cref="Checked"/> property of this action changes.
        /// </summary>
        event EventHandler CheckedChanged;
        
        /// <summary>
        /// Gets a value indicating whether this action is a "check" action, that is, an action that behaves as a toggle.
        /// </summary>
        bool IsCheckAction { get; }

        /// <summary>
        /// Gets the checked state that the action should present in the UI, if this is a "check" action.
        /// </summary>
        /// <remarks>
        /// This property has no meaning if <see cref="IsCheckAction"/> returns false.
        /// </remarks>
        bool Checked { get; }

		/// <summary>
		/// Gets a value indicating whether parent items should be checked if this
		/// <see cref="IClickAction"/> is checked.
		/// </summary>
		bool CheckParents { get; }

		/// <summary>
		/// Gets or sets the keystroke that the UI should attempt to intercept to invoke the action.
		/// </summary>
		XKeys KeyStroke { get; set; }

        /// <summary>
        /// Called by the UI when the user clicks on the action.
        /// </summary>
        void Click();
    }
}
