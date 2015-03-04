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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines the interface to a view for the <see cref="Application"/> object.
	/// </summary>
	public interface IApplicationView : IView
	{
		/// <summary>
		/// Creates a view for the specified desktop window.
		/// </summary>
		IDesktopWindowView CreateDesktopWindowView(DesktopWindow window);

		/// <summary>
		/// Displays a message box.
		/// </summary>
		/// <param name="message">The message to display in the mesage box.</param>
		/// <param name="buttons">The buttons to display in the message box.</param>
		/// <returns>The result of the user dismissing the message box.</returns>
		DialogBoxAction ShowMessageBox([param : Localizable(true)] string message, MessageBoxActions buttons);
	}
}