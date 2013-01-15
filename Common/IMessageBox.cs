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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Enum flags describing the buttons/options that should be available to the user in a dialog box.
    /// </summary>
    /// <remarks>
    /// These flags can be combined together using the | operator to specify multiple buttons.
    /// </remarks>
	[Flags]
    public enum DialogBoxAction
    {
		/// <summary>
		/// An Ok button should be shown.
		/// </summary>
        Ok      = 0x0001,

		/// <summary>
		/// A Cancel button should be shown.
		/// </summary>
        Cancel  = 0x0002,

		/// <summary>
		/// A Yes button should be shown.
		/// </summary>
        Yes     = 0x0004,

		/// <summary>
		/// A No button should be shown.
		/// </summary>
        No      = 0x0008,
    }

	/// <summary>
	/// Enum flags specific to message boxes, which are just 
	/// commonly used combinations of <see cref="DialogBoxAction"/>s.
	/// </summary>
    public enum MessageBoxActions
    {
        /// <summary>
        /// An Ok button should be shown.
        /// </summary>
		Ok = DialogBoxAction.Ok,

		/// <summary>
		/// Both an Ok and a Cancel button should be shown.
		/// </summary>
        OkCancel = DialogBoxAction.Ok | DialogBoxAction.Cancel,

		/// <summary>
		/// Both a Yes and No button should be shown.
		/// </summary>
        YesNo = DialogBoxAction.Yes | DialogBoxAction.No,

		/// <summary>
		/// Yes, No and Cancel buttons should be shown.
		/// </summary>
        YesNoCancel = DialogBoxAction.Yes | DialogBoxAction.No | DialogBoxAction.Cancel
    }

	/// <summary>
	/// An interface for a message box.
	/// </summary>
	public interface IMessageBox
	{
		/// <summary>
		/// Shows a message box displaying the input <paramref name="message"/>, 
		/// usually with an Ok button.
		/// </summary>
		void Show(string message);

		/// <summary>
		/// Shows a message box displaying the input <paramref name="message"/>
		/// and the specified <paramref name="buttons"/>.
		/// </summary>
		DialogBoxAction Show(string message, MessageBoxActions buttons);
	}
}
