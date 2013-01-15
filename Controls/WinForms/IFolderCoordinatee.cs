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
using System.ComponentModel;

namespace ClearCanvas.Controls.WinForms
{
	internal interface IFolderCoordinatee
	{
		/// <summary>
		/// Gets the <see cref="ClearCanvas.Controls.WinForms.Pidl"/> to which the coordinatee is synchronized.
		/// </summary>
		/// <remarks>
		/// Implementations should <b>NOT</b> return a new instance of a <see cref="ClearCanvas.Controls.WinForms.Pidl"/>,
		/// as consumers will not take over ownership and disposal responsibility of the returned object.
		/// </remarks>
		Pidl Pidl { get; }

		/// <summary>
		/// Fired before <see cref="Pidl"/> changes.
		/// </summary>
		event CancelEventHandler PidlChanging;

		/// <summary>
		/// Fired when <see cref="Pidl"/> changes.
		/// </summary>
		event EventHandler PidlChanged;

		/// <summary>
		/// Synchronizes the coordinatee to the specified <see cref="ClearCanvas.Controls.WinForms.Pidl"/>.
		/// </summary>
		/// <remarks>
		/// Implementations should <b>NOT</b> use the provided <paramref name="pidl"/> as is, but rather
		/// clone a new instance for which it will assume ownership and responsibility.
		/// </remarks>
		/// <param name="pidl">The <see cref="ClearCanvas.Controls.WinForms.Pidl"/> to which the coordinatee should be synchronized.</param>
		void BrowseTo(Pidl pidl);

		/// <summary>
		/// Reloads the coordinatee's view of the currently synchronized <see cref="Pidl"/>.
		/// </summary>
		void Reload();
	}
}