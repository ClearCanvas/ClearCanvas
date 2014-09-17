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
	/// Holds parameters that control the creation of a <see cref="Shelf"/>.
	/// </summary>
	public class ShelfCreationArgs : DesktopObjectCreationArgs
	{
		private IApplicationComponent _component;
		private ShelfDisplayHint _displayHint;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ShelfCreationArgs() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="component">The <see cref="IApplicationComponent"/> that is to be hosted in the <see cref="Shelf"/>.</param>
		/// <param name="title">The title of the <see cref="Shelf"/>.</param>
		/// <param name="name">A name/identifier for the <see cref="Shelf"/>.</param>
		/// <param name="displayHint">A hint for how the <see cref="Shelf"/> should be initially displayed.</param>
		public ShelfCreationArgs(IApplicationComponent component, [param : Localizable(true)] string title, string name, ShelfDisplayHint displayHint)
			: base(title, name)
		{
			_component = component;
			_displayHint = displayHint;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="component">The <see cref="IApplicationComponent"/> that is to be hosted in the <see cref="Shelf"/>.</param>
		/// <param name="title">The title of the <see cref="Shelf"/>.</param>
		/// <param name="name">A name/identifier for the <see cref="Shelf"/>.</param>
		public ShelfCreationArgs(IApplicationComponent component, [param : Localizable(true)] string title, string name)
			: this(component, name, title, ShelfDisplayHint.None) {}

		/// <summary>
		/// Gets or sets the component to host.
		/// </summary>
		public IApplicationComponent Component
		{
			get { return _component; }
			set { _component = value; }
		}

		/// <summary>
		/// Gets or sets the display hint that affects the initial positioning of the shelf.
		/// </summary>
		public ShelfDisplayHint DisplayHint
		{
			get { return _displayHint; }
			set { _displayHint = value; }
		}
	}
}