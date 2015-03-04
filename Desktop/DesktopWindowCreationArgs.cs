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
	/// Holds parameters that control the creation of a <see cref="DesktopWindow"/>.
	/// </summary>
	public class DesktopWindowCreationArgs : DesktopObjectCreationArgs
	{
		private string _toolbarSite;
		private string _menuSite;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DesktopWindowCreationArgs() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public DesktopWindowCreationArgs([param : Localizable(true)] string title, string name)
			: base(title, name) {}

		/// <summary>
		/// Gets or sets the toolbar site that this window will use.
		/// </summary>
		public string ToolbarSite
		{
			get { return _toolbarSite; }
			set { _toolbarSite = value; }
		}

		/// <summary>
		/// Gets or sets the menu site that this window will use.
		/// </summary>
		public string MenuSite
		{
			get { return _menuSite; }
			set { _menuSite = value; }
		}
	}
}