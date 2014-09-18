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
	/// Abstract base class for passing creation parameters to desktop object factories.
	/// </summary>
	public abstract class DesktopObjectCreationArgs
	{
		private string _name;
		private string _title;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected DesktopObjectCreationArgs() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title">The title for the <see cref="DesktopObject"/>.</param>
		/// <param name="name">The name/identifier of the <see cref="DesktopObject"/>.</param>
		protected DesktopObjectCreationArgs([param : Localizable(true)] string title, string name)
		{
			_name = name;
			_title = title;
		}

		/// <summary>
		/// Gets or sets the name for the desktop object.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the title for the desktop object.
		/// </summary>
		[Localizable(true)]
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}
	}
}