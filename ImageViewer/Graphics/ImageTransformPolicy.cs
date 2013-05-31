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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a <see cref="SpatialTransformValidationPolicy"/> for images.
	/// </summary>
	[Cloneable(true)]
	public class ImageTransformPolicy : SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ImageTransformPolicy()
		{
		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// At present, validation amounts to ensuring that rotations are
		/// only in increments of 90 degrees.
		/// </remarks>
		public override void Validate(ISpatialTransform transform)
		{
			PointF xVector = new PointF(100, 0);
			SizeF xVectorTransformed = transform.ConvertToDestination(new SizeF(xVector));

			//figure out where the source x-axis went in destination
			int rotation = (int) Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));
			if (rotation < 0)
				rotation += 360;

			if ((rotation % 90) != 0)
				throw new ArgumentException("Images can only be rotated by multiples of 90 degrees.");
		}
	}
}
