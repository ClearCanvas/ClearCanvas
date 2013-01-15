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
	/// Defines the property get the length of a region of interest.
	/// </summary>
	public interface IRoiLengthProvider
	{
		/// <summary>
		/// Gets or sets the units of length with which to compute the value of <see cref="Length"/>.
		/// </summary>
		Units Units { get; set; }

		/// <summary>
		/// Gets the length of the region of interest in units of length as specified by <see cref="Units"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">If <see cref="Units"/> is a physical
		/// unit of measurement and the image has no pixel spacing information, nor has it been calibrated.</exception>
		double Length { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated with physical dimensions.
		/// </summary>
		bool IsCalibrated { get; }
	}
}