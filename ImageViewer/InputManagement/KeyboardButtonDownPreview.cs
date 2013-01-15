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

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to preview a keyboard button message before it is processed.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="KeyboardButtonShortcut"/>
	/// <seealso cref="TileController"/>
	public sealed class KeyboardButtonDownPreview
	{
		private readonly KeyboardButtonShortcut _buttonShortcut;

		/// <summary>
		/// Constructor.
		/// </summary>
		public KeyboardButtonDownPreview(XKeys keyData)
		{
			_buttonShortcut = new KeyboardButtonShortcut(keyData);
		}

		/// <summary>
		/// Gets the <see cref="KeyboardButtonShortcut"/> to be previewed.
		/// </summary>
		public KeyboardButtonShortcut Shortcut
		{
			get { return _buttonShortcut; }
		}
	}
}
