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

using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Implements an <see cref="ISpatialTransform"/> which is invariant in the destination coordinate system with respect to scale, flip and rotation.
	/// </summary>
	[Cloneable]
	public sealed class InvariantSpatialTransform : SpatialTransform
	{
		/// <summary>
		/// Initializes a new <see cref="InvariantSpatialTransform"/>.
		/// </summary>
		/// <param name="ownerGraphic">The graphic for which this <see cref="InvariantSpatialTransform"/> is being constructed.</param>
		public InvariantSpatialTransform(IGraphic ownerGraphic) : base(ownerGraphic) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private InvariantSpatialTransform(InvariantSpatialTransform source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Called by the base <see cref="SpatialTransform"/> to post-multiply an operation to the overall transformation matrix.
		/// </summary>
		protected override void CalculatePostTransform(Matrix cumulativeTransform)
		{
			cumulativeTransform.Reset();
			cumulativeTransform.Translate(this.TranslationX, this.TranslationY);
		}

		/// <summary>
		/// Called by the base <see cref="SpatialTransform"/> to pre-multiply an operation to the overall transformation matrix.
		/// </summary>
		protected override void CalculatePreTransform(Matrix cumulativeTransform) {}
	}
}