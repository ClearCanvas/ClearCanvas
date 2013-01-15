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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the public interface to a <see cref="DesktopObject"/>.
    /// </summary>
    public interface IDesktopObject
    {
        /// <summary>
        /// Gets the runtime name of the object, or null if the object is not named.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the title that is presented to the user on the screen.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the current state of the object.
        /// </summary>
        DesktopObjectState State { get; }

        /// <summary>
        /// Activates the object.
        /// </summary>
        void Activate();

        /// <summary>
        /// Tries to close the object, interacting with the user if necessary.
        /// </summary>
        /// <returns>True if the object is closed, otherwise false.</returns>
        bool Close();

        /// <summary>
        /// Tries to close the object, interacting with the user only if specified.
        /// </summary>
        /// <param name="interactive">A value specifying whether user interaction is allowed.</param>
        /// <returns>True if the object is closed, otherwise false.</returns>
        bool Close(UserInteraction interactive);

        /// <summary>
        /// Checks if the object is in a closable state (would be able to close without user interaction).
        /// </summary>
        /// <returns>True if the object can be closed without user interaction.</returns>
        bool QueryCloseReady();

        /// <summary>
        /// Gets a value indicating whether this object is currently active.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Gets a value indicating whether this object is currently visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Occurs when the <see cref="Active"/> property changes.
        /// </summary>
        event EventHandler ActiveChanged;

        /// <summary>
        /// Occurs when the <see cref="Visible"/> property changes.
        /// </summary>
        event EventHandler VisibleChanged;

        /// <summary>
        /// Occurs when the <see cref="Title"/> property changes.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Occurs when the object is about to close.
        /// </summary>
        event EventHandler<ClosingEventArgs> Closing;

        /// <summary>
        /// Occurs when the object has closed.
        /// </summary>
        event EventHandler<ClosedEventArgs> Closed;
    }
}
