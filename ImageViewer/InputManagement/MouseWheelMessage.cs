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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to handle mouse wheel messages.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="MouseWheelShortcut"/>
	/// <seealso cref="TileController"/>
	public sealed class MouseWheelMessage
	{
		private readonly int _wheelDelta;
		private readonly MouseWheelShortcut _wheelShortcut;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelMessage(int wheelDelta, bool control, bool alt, bool shift)
		{
			_wheelDelta = wheelDelta;
			_wheelShortcut = new MouseWheelShortcut(control, alt, shift);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelMessage(int wheelDelta)
			: this(wheelDelta, false, false, false)
		{
		}

		/// <summary>
		/// Gets the wheel delta.
		/// </summary>
		public int WheelDelta
		{
			get { return _wheelDelta; }
		}

		/// <summary>
		/// Gets the associated <see cref="MouseWheelShortcut"/>.
		/// </summary>
		public MouseWheelShortcut Shortcut
		{
			get { return _wheelShortcut; }
		}
	}
}
