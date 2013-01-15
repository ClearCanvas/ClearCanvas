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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Defines a <see cref="SpatialTransformValidationPolicy"/> for <see cref="RoiGraphic"/>s.
	/// </summary>
	[Cloneable(true)]
	public class RoiTransformPolicy : SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RoiTransformPolicy()
		{
		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// At present, validation amounts to ensuring the rotation is always zero. 
		/// <see cref="RoiGraphic"/>s are prohibited from being rotated
		/// because calculation of ROI related statistics, such as mean and standard deviation,
		/// currently only work with unrotated ROIs.
		/// </remarks>
		public override void Validate(ISpatialTransform transform)
		{
			if (transform.RotationXY != 0)
				throw new ArgumentException("ROIs cannot be rotated.");
		}
	}
}