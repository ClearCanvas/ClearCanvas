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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Event fired when mouse capture is gained or lost.
	/// </summary>
	public class MouseCaptureChangedEventArgs : EventArgs
	{
		private readonly bool _gained;
		private readonly ITile _tile;

		internal MouseCaptureChangedEventArgs(ITile tile, bool gained)
		{
			_gained = gained;
			_tile = tile;
		}

		/// <summary>
		/// Gets the affected tile.
		/// </summary>
		public ITile Tile
		{
			get { return _tile; }
		}
	
		/// <summary>
		/// True if mouse capture has been gained, false if it was lost.
		/// </summary>
		public bool Gained
		{
			get { return _gained; }
		}
	}
}
