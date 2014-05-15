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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that displays a text box into which the user can type.
	/// </summary>
	public interface ITextBoxAction : IAction
	{
		/// <summary>
		/// Occurs when the value of <see cref="TextValue"/> changes.
		/// </summary>
		event EventHandler TextValueChanged;

		/// <summary>
		/// Gets or sets the value of the text displayed in the text box.
		/// </summary>
		string TextValue { get; set; }

		/// <summary>
		/// Occurs when the value of <see cref="CueText"/> changes.
		/// </summary>
		event EventHandler CueTextChanged;

		/// <summary>
		/// Gets or sets the cue text displayed in the text box when it does not have focus.
		/// </summary>
		[Localizable(true)]
		string CueText { get; set; }
	}
}