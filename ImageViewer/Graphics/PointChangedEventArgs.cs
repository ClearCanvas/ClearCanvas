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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for "point changed" events.
	/// </summary>
	public class PointChangedEventArgs : EventArgs
	{
		private readonly PointF _point;
		private readonly CoordinateSystem _coordinateSystem;

		/// <summary>
		/// Initializes a new instance of <see cref="PointChangedEventArgs"/>
		/// with the specified point.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="coordinateSystem"></param>
		public PointChangedEventArgs(PointF point, CoordinateSystem coordinateSystem)
		{
			_point = point;
			_coordinateSystem = coordinateSystem;
		}

		/// <summary>
		/// Gets the point.
		/// </summary>
		public PointF Point
		{
			get { return _point; }
		}

		/// <summary>
		/// Gets the coordinate system in which the value of <see cref="Point"/> is expressed.
		/// </summary>
		public CoordinateSystem CoordinateSystem
		{
			get { return _coordinateSystem; }
		}
	}
}