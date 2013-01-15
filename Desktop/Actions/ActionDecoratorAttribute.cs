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

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to decorate an action
    /// once it has been declared by an <see cref="ActionInitiatorAttribute"/>.
    /// </summary>
    public abstract class ActionDecoratorAttribute : ActionAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The unique identifer of the action.</param>
        protected ActionDecoratorAttribute(string actionID)
            : base(actionID)
        {
        }
    }
}
