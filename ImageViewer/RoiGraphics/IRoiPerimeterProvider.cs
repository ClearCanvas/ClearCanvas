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

using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Common interface for regions of interest that have the notion of perimeter
	/// </summary>
	public interface IRoiPerimeterProvider
	{
		/// <summary>
		/// Gets the perimeter of the <see cref="Roi"/> in pixels.
		/// </summary>
		double PixelPerimeter { get; }

		/// <summary>
		/// Gets the perimeter of the <see cref="Roi"/> in millimetres.
		/// </summary>
		/// <exception cref="UncalibratedImageException">If the image has no pixel spacing
		/// information and has not been calibrated.</exception>
		double Perimeter { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated, and hence the <see cref="Perimeter"/> property is available.
		/// </summary>
		bool IsCalibrated { get; }
	}
}