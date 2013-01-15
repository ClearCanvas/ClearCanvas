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
    /// Defines the interface for an extension of <see cref="GuiToolkitExtensionPoint"/>.
    /// <remarks>
    /// One extension must exist or the desktop application will not run.
    /// The purpose of the extension is to bootstrap a GUI subsystem such as Windows Forms or GTK.
    /// </remarks>
    /// </summary>
    public interface IGuiToolkit
    {
        /// <summary>
        /// Gets the ID of the toolkit.
        /// </summary>
        string ToolkitID { get; }

        /// <summary>
        /// Occurs when the toolkit has successfully started (e.g. its message loop is active).
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Initializes the GUI toolkit and starts the internal message loop,
        /// blocking until <see cref="Terminate"/> is called.
        /// </summary>
        /// <remarks>
        /// This method must block until <see cref="Terminate"/> is called.  This method must also ensure
        /// that the <see cref="Started"/> event is raised from within the message loop of the GUI system.
        /// </remarks>
        void Run();

        /// <summary>
        /// Terminates the GUI toolkit, shutting down the internal message loop and releasing the
        /// blocked <see cref="Run"/> method.
        /// </summary>
        void Terminate();
    }
}
