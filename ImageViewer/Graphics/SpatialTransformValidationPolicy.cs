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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Base class for validating <see cref="SpatialTransform"/> objects.
	/// </summary>
	/// <remarks>
	/// It is not always desirable to allow an <see cref="IGraphic"/> to be transformed
	/// in arbitrary ways.  For example, at present, images can only be rotated in
	/// 90 degree increments.  This class allows a validation policy to be defined on a
	/// per graphic basis.  If validation fails, an <see cref="ArgumentException"/> is thrown.
	/// </remarks>
	[Cloneable(true)]
	public abstract class SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SpatialTransformValidationPolicy"/>.
		/// </summary>
		protected SpatialTransformValidationPolicy()
		{

		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// Implementors should throw an <see cref="ArgumentException"/> if validation fails.
		/// </remarks>
		public abstract void Validate(ISpatialTransform transform);
	}
}
