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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines the interface to an application component host as seen by the hosted application component.
	/// </summary>
	public interface IApplicationComponentHost
	{
		/// <summary>
		/// Instructs the host to terminate if, for instance, the user has pressed an OK or Cancel button.
		/// </summary>
		/// <remarks>
		/// The host will subsequently call <see cref="IApplicationComponent.Stop"/>.  Not all hosts
		/// support this method.
		/// </remarks>
		void Exit();

		/// <summary>
		/// Asks the host to display a message box to the user.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="buttons">The buttons to display.</param>
		/// <returns>A result indicating which button the user pressed.</returns>
		DialogBoxAction ShowMessageBox([param : Localizable(true)] string message, MessageBoxActions buttons);

		/// <summary>
		/// Asks the host to set the title for this component in the UI.
		/// </summary>
		/// <remarks>
		/// Not all hosts support this method.
		/// </remarks>
		[Obsolete("Use the IApplicationComponentHost.Title property instead.")]
		void SetTitle([param : Localizable(true)] string title);

		/// <summary>
		/// Gets or sets the title that the host displays in the UI above this component.
		/// </summary>
		/// <remarks>
		/// Not all hosts support this property.
		/// </remarks>
		[Localizable(true)]
		string Title { get; set; }

		/// <summary>
		/// Gets the <see cref="CommandHistory"/> object associated with this host.
		/// </summary>
		/// <remarks>
		/// Not all hosts support this property.
		/// </remarks>
		CommandHistory CommandHistory { get; }

		/// <summary>
		/// Gets the <see cref="DesktopWindow"/> associated with this host.
		/// </summary>
		DesktopWindow DesktopWindow { get; }
	}
}