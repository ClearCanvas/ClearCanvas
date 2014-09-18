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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines a base interface for views that serve desktop objects.
	/// </summary>
	/// <remarks>
	/// The view provides the on-screen representation of the object.
	/// </remarks>
	public interface IDesktopObjectView : IView, IDisposable
	{
		/// <summary>
		/// Occurs when the <see cref="Visible"/> property changes.
		/// </summary>
		event EventHandler VisibleChanged;

		/// <summary>
		/// Occurs when the <see cref="Active"/> property changes.
		/// </summary>
		event EventHandler ActiveChanged;

		/// <summary>
		/// Occurs when the user has requested that the object be closed.
		/// </summary>
		event EventHandler CloseRequested;

		/// <summary>
		/// Sets the title that is displayed to the user.
		/// </summary>
		void SetTitle([param : Localizable(true)] string title);

		/// <summary>
		/// Opens the view (makes it first visible on the screen).
		/// </summary>
		void Open();

		/// <summary>
		/// Shows the view.
		/// </summary>
		void Show();

		/// <summary>
		/// Hides the view.
		/// </summary>
		void Hide();

		/// <summary>
		/// Activates the view.
		/// </summary>
		void Activate();

		/// <summary>
		/// Gets a value indicating whether the view is visible on the screen.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets a value indicating whether the view is active.
		/// </summary>
		bool Active { get; }
	}
}