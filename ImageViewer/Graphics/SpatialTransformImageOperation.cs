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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A specialization of the <see cref="BasicImageOperation"/> where the
	/// originator is an <see cref="ISpatialTransform"/>.
	/// </summary>
	public class SpatialTransformImageOperation : BasicImageOperation<ISpatialTransform>
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public SpatialTransformImageOperation(ApplyDelegate applyDelegate)
			: base(GetTransform, applyDelegate) {}

		/// <summary>
		/// Returns the <see cref="ISpatialTransform"/> associated with the 
		/// <see cref="IPresentationImage"/>, or null.
		/// </summary>
		/// <remarks>
		/// When used in conjunction with an <see cref="ImageOperationApplicator"/>,
		/// it is always safe to cast the return value directly to <see cref="ISpatialTransform"/>
		/// without checking for null from within the <see cref="BasicImageOperation.ApplyDelegate"/> 
		/// specified in the constructor.
		/// </remarks>
		public override ISpatialTransform GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image);
		}

		private static ISpatialTransform GetTransform(IPresentationImage image)
		{
			return image is ISpatialTransformProvider ? ((ISpatialTransformProvider) image).SpatialTransform : null;
		}
	}
}