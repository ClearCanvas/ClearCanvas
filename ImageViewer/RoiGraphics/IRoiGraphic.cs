﻿#region License

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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Represents a graphic where the selected subject is a region of interest.
	/// </summary>
	public interface IRoiGraphic : IGraphic
	{
		/// <summary>
		/// Gets the subject graphic of this <see cref="IRoiGraphic"/>.
		/// </summary>
		IGraphic Subject { get; }

		/// <summary>
		/// Gets an object representing the data of the current region of interest.
		/// </summary>
		Roi Roi { get; }

		/// <summary>
		/// Occurs when the region of interest changes, thereby changing the data and invalidating any existing, computed statistics.
		/// </summary>
		event EventHandler RoiChanged;

		/// <summary>
		/// Refreshes the annotation graphic by recomputing the callout position and redrawing the graphic.
		/// </summary>
		void Refresh();
	}
}