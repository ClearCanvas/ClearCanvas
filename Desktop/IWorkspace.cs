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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines the public interface to a <see cref="Workspace"/>.
	/// </summary>
	/// <remarks>
	/// This interface exists mainly for backward compatibility.  New application
	/// code should use the <see cref="Workspace"/> class.
	/// </remarks>
	public interface IWorkspace : IDesktopObject
	{
		/// <summary>
		/// Gets the desktop window that owns this workspace.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the hosted component.
		/// </summary>
		object Component { get; }

		/// <summary>
		/// Gets the command history associated with this workspace.
		/// </summary>
		CommandHistory CommandHistory { get; }

		/// <summary>
		/// Gets a value indicating whether this workspace can be closed directly by the user.
		/// </summary>
		bool UserClosable { get; }

		/// <summary>
		/// Shows a dialog box in front of this workspace.
		/// </summary>
		/// <param name="args">Arguments used to create the dialog box.</param>
		/// <returns>The newly created dialog box object.</returns>
		WorkspaceDialogBox ShowDialogBox(DialogBoxCreationArgs args);
	}
}
