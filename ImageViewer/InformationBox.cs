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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A <see cref="Tile"/> can show a textual information box at a given point
	/// in it's client rectangle when one has been supplied.
	/// </summary>
	/// <seealso cref="Tile"/>
	/// <seealso cref="Tile.InformationBox"/>
	public class InformationBox
	{
		private string _data;
		private Point _destinationPoint;
		private bool _visible;

		private event EventHandler _updated;

		/// <summary>
		/// Constructor.
		/// </summary>
		public InformationBox()
		{
			_visible = false;
		}

		/// <summary>
		/// Fires when any of the following properties changes: <see cref="Data"/>, <see cref="Visible"/>, <see cref="DestinationPoint"/>.
		/// </summary>
		public event EventHandler Updated
		{
			add { _updated += value; }
			remove { _updated -= value; }
		}

		/// <summary>
		/// Gets or set the text data that is to be displayed to the user.
		/// </summary>
		public string Data
		{
			get { return _data; }
			set
			{
				if (_data == value)
					return;

				_data = value;

				EventsHelper.Fire(_updated, this, new EventArgs());
			}
		}

		/// <summary>
		/// The point at which the <see cref="InformationBox"/> should be displayed.
		/// </summary>
		public Point DestinationPoint
		{
			get { return _destinationPoint; }
			set
			{
				if (value == _destinationPoint)
					return;

				_destinationPoint = value;

				EventsHelper.Fire(_updated, this, new EventArgs());
			}
		}

		/// <summary>
		/// Gets or sets whether or not the <see cref="InformationBox"/> should be visible.
		/// </summary>
		public bool Visible
		{
			get
			{ return _visible; }
			set
			{
				if (value == _visible)
					return;

				_visible = value;

				EventsHelper.Fire(_updated, this, new EventArgs());
			}
		}

		/// <summary>
		/// Updates both the <see cref="Data"/> and <see cref="DestinationPoint"/> properties simultaneously.
		/// </summary>
		public void Update(string data, Point destinationPoint)
		{
			bool changed = false;

			if (!_visible || data != _data || destinationPoint != _destinationPoint)
				changed = true;

			_visible = true;
			_data = data;
			_destinationPoint = destinationPoint;
			
			if (changed)
				EventsHelper.Fire(_updated, this, new EventArgs());
		}
	}
}
