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

using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Defines a method to get exported <see cref="IAction"/>s.
	/// </summary>
	public interface IExportedActionsProvider
	{
		/// <summary>
		/// Gets a set of exported <see cref="IAction"/>s.
		/// </summary>
		/// <remarks>
		/// This mechanism is useful when a particular component defines generally useful <see cref="IAction"/>s
		/// without requiring specific knowledge of the action model sites that the client code uses.
		/// </remarks>
		/// <param name="site">The action model site at which the actions should reside.</param>
		/// <param name="mouseInformation">The mouse input when the action model was requested, such as in response to a context menu request.</param>
		/// <returns>A set of exported <see cref="IAction"/>s.</returns>
		IActionSet GetExportedActions(string site, IMouseInformation mouseInformation);
	}
}