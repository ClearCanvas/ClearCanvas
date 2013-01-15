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
using System.Drawing;

namespace ClearCanvas.Ris.Client
{
    public enum LoginDialogMode
    {
        InitialLogin,
        RenewLogin
    }

	/// <summary>
	/// Defines an interface to a login dialog that interacts with a user to obtain login credentials.
	/// </summary>
    public interface ILoginDialog : IDisposable
    {
		/// <summary>
		/// Shows the dialog, returning true if the user pressed OK, or false if cancelled.
		/// </summary>
		/// <returns></returns>
        bool Show();

		/// <summary>
		/// Gets or set the location of the dialog on the screen.
		/// </summary>
		Point Location { get; set; }

		/// <summary>
		/// Gets or sets the dialogs mode.
		/// </summary>
        LoginDialogMode Mode { get; set; }

		/// <summary>
		/// Gets or sets the list of facility choices.
		/// </summary>
        string[] FacilityChoices { get; set; }

		/// <summary>
		/// Gets or sets the facility.
		/// </summary>
        string Facility { get; set; }

		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
        string UserName { get; set; }

		/// <summary>
		/// Gets the password entered by the user.
		/// </summary>
        string Password { get; }
    }
}
