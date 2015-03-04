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

using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Holds parameters that control the creation of a <see cref="Workspace"/>.
	/// </summary>
	public class WorkspaceCreationArgs : DesktopObjectCreationArgs
	{
		private IApplicationComponent _component;
		private bool _userClosable = true; // default to true

		/// <summary>
		/// Constructor
		/// </summary>
		public WorkspaceCreationArgs() {}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="component"></param>
		/// <param name="title"></param>
		/// <param name="name"></param>
		public WorkspaceCreationArgs(IApplicationComponent component, [param : Localizable(true)] string title, string name)
			: base(title, name)
		{
			_component = component;
		}

		/// <summary>
		/// Gets or sets the hosted component.
		/// </summary>
		public IApplicationComponent Component
		{
			get { return _component; }
			set { _component = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this workspace can be closed directly by the user.
		/// </summary>
		public bool UserClosable
		{
			get { return _userClosable; }
			set { _userClosable = value; }
		}
	}
}