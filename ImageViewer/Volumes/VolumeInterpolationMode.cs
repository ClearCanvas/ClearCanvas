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

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Specifies the type of interpolation to be used when creating image data in the volume framework.
	/// </summary>
	public enum VolumeInterpolationMode
	{
		/// <summary>
		/// Specifies the use of bilinear interpolation (in the context of 2D images) or trilinear interpolation (in the context of 3D volumes).
		/// </summary>
		Linear,

		/// <summary>
		/// Specifies the use of bicubic interpolation (in the context of 2D images) or tricubic interpolation (in the context of 3D volumes). 
		/// </summary>
		Cubic,

		/// <summary>
		/// Specifies the use of nearest-neighbor interpolation.
		/// </summary>
		NearestNeighbor
	}
}