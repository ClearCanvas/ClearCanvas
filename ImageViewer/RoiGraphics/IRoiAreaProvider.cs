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

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Defines the property to get the area of a region of interest.
	/// </summary>
	public interface IRoiAreaProvider
	{
		/// <summary>
		/// Gets or sets the units of area with which to compute the value of <see cref="Area"/>.
		/// </summary>
		Units Units { get; set; }

		/// <summary>
		/// Gets the area of the region of interest in the units of area as specified by <see cref="Units"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">If <see cref="Units"/> is a physical
		/// unit of measurement and the image has no pixel spacing information, nor has it been calibrated.</exception>
		double Area { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated with physical dimensions.
		/// </summary>
		bool IsCalibrated { get; }
	}
}