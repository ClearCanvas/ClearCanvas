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

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Enumerated values defining the units of measurement used in various calculations in the <see cref="ClearCanvas.ImageViewer.RoiGraphics"/> namespace.
	/// </summary>
	/// <remarks>
	/// Depending on the specific context, the enumerated values can also represent areas or volumes. For example, if a method that computes area
	/// is given an argument of <see cref="Centimeters"/>, then the output should be interpreted to be in square centimetres. Similarly, if a
	/// method that computes volume is given <see cref="Pixels"/>, then the output should be interpreted to be in cubic pixels.
	/// </remarks>
	public enum Units
	{
		/// <summary>
		/// Indicates that the measurement is in units of image pixels (or square pixels, or cubic pixels).
		/// </summary>
		Pixels,

		/// <summary>
		/// Indicates that the measurement is in units of millimetres (or square millimetres, or cubic millimetres).
		/// </summary>
		Millimeters,

		/// <summary>
		/// Indicates that the measurement is int units of centimetres (or square centimetres, or cubic centimetres).
		/// </summary>
		Centimeters
	}
}